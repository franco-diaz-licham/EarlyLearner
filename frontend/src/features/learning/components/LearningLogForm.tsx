import { useMemo } from 'react';
import { AppButton } from '../../../shared/ui/AppButton';
import { AppDialog } from '../../../shared/ui/AppDialog';
import { AppInputText } from '../../../shared/ui/AppInputText';
import { AppInputTextArea } from '../../../shared/ui/AppInputTextArea';
import { AppSelect } from '../../../shared/ui/AppSelect';
import type { ChildModel } from '../../households/types/household.types';
import type { ReadinessOutcomeModel } from '../../readiness/types/readinessOutcome.types';
import { createEmptyLearningLogForm, useLearningLogForm } from '../hooks/useLearningLogForm';
import type { LearningLogFormModel, LearningMomentKind } from '../types/dailyLog.types';

interface LearningLogFormProps {
  children: ChildModel[];
  learningOutcomes: ReadinessOutcomeModel[];
  saving: boolean;
  visible: boolean;
  onHide: () => void;
  onSave: (form: LearningLogFormModel) => void;
}

const learningKindOptions = [
  { label: 'Activity', value: 'activity' },
  { label: 'Observation', value: 'observation' },
  { label: 'Reading', value: 'reading' },
  { label: 'Routine', value: 'routine' }
] satisfies Array<{ label: string; value: LearningMomentKind }>;

const kindLabel = (kind: LearningMomentKind) => learningKindOptions.find((option) => option.value === kind)?.label ?? kind;

export const LearningLogForm = ({ children, learningOutcomes, saving, visible, onHide, onSave }: LearningLogFormProps) => {
  const initialDraft = useMemo(() => createEmptyLearningLogForm(children[0]?.id ?? ''), [children]);
  const form = useLearningLogForm(initialDraft);
  const { draft, errors } = form;
  const hasErrors = Object.keys(errors).length > 0;

  const childOptions = useMemo(
    () =>
      children.map((child) => ({
        label: `${child.firstName} ${child.lastName}`,
        value: child.id
      })),
    [children]
  );

  const handleOutcomeToggle = (learningOutcomeId: string) => {
    form.updateField('learningOutcomeIds', draft.learningOutcomeIds.includes(learningOutcomeId) ? draft.learningOutcomeIds.filter((id) => id !== learningOutcomeId) : [...draft.learningOutcomeIds, learningOutcomeId]);
  };

  const handleSave = () => {
    const validForm = form.getValidForm();
    if (!validForm) return;
    onSave(validForm);
  };

  const handleCancel = () => {
    form.reset();
    onHide();
  };

  return (
    <AppDialog header="Add learning log" visible={visible} onHide={handleCancel}>
      <div className="space-y-4 pt-4">
        <div className="grid gap-4 md:grid-cols-3">
          <AppSelect label="Child" options={childOptions} value={draft.childId} onChange={(value) => form.updateField('childId', value)} disabled={childOptions.length === 0} error={errors.childId} required />
          <AppInputText
            error={errors.logDate}
            label="Date"
            required
            type="date"
            value={draft.logDate}
            onChange={(event) => {
              form.updateField('logDate', event.target.value);
            }}
          />
          <AppSelect label="Type" options={learningKindOptions} value={draft.kind} onChange={(value) => form.updateField('kind', value)} required />
        </div>

        <AppInputText
          autoFocus
          error={errors.title}
          label="Title"
          placeholder="Butterfly craft"
          required
          value={draft.title}
          onChange={(event) => {
            form.updateField('title', event.target.value);
          }}
        />

        <div>
          <AppInputTextArea
            autoResize
            label="Notes"
            className="min-h-36"
            placeholder="What happened? What did they try, say, notice, or enjoy?"
            required
            rows={6}
            value={draft.notes}
            onChange={(event) => {
              form.updateField('notes', event.target.value);
            }}
          />
          {errors.notes ? <span className="mt-1 block text-sm font-semibold text-red-600">{errors.notes}</span> : null}
        </div>

        <div>
          <p className="mb-2 text-sm font-semibold text-brand-heading">
            Learning outcomes
            <span aria-hidden="true" className="ml-1 text-red-600">
              *
            </span>
          </p>
          <div className="grid max-h-48 gap-2 overflow-y-auto pr-1 md:grid-cols-2">
            {learningOutcomes.map((outcome) => (
              <label className="flex cursor-pointer items-start gap-3 rounded-md border border-brand-border bg-white p-3 text-sm text-brand-text transition hover:bg-brand-surface-soft" key={outcome.readinessOutcomeId}>
                <input
                  className="mt-1 h-4 w-4 accent-brand-primary"
                  type="checkbox"
                  checked={draft.learningOutcomeIds.includes(outcome.readinessOutcomeId)}
                  onChange={() => {
                    handleOutcomeToggle(outcome.readinessOutcomeId);
                  }}
                />
                <span>
                  <span className="block font-semibold text-brand-heading">{outcome.name}</span>
                  <span className="mt-1 block text-xs text-brand-muted">{outcome.category}</span>
                </span>
              </label>
            ))}
          </div>
          {errors.learningOutcomeIds ? <span className="mt-1 block text-sm font-semibold text-red-600">{errors.learningOutcomeIds}</span> : null}
        </div>

        <div className="flex justify-end gap-3 pt-2">
          <AppButton label="Cancel" type="button" variant="secondary" onClick={handleCancel} />
          <AppButton disabled={saving || !form.isValid || hasErrors} label={saving ? 'Saving...' : `Save ${kindLabel(draft.kind).toLowerCase()}`} type="button" onClick={handleSave} />
        </div>
      </div>
    </AppDialog>
  );
};
