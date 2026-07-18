import { useMemo } from 'react';
import { AppButton } from '../../../shared/ui/AppButton';
import { AppDialog } from '../../../shared/ui/AppDialog';
import { AppInputText } from '../../../shared/ui/AppInputText';
import { AppInputTextArea } from '../../../shared/ui/AppInputTextArea';
import { AppMultiCheckboxSelector } from '../../../shared/ui/AppMultiCheckboxSelector';
import { AppSelect } from '../../../shared/ui/AppSelect';
import type { ChildModel } from '../../households/types/household.types';
import { createEmptyLearningLogForm, getLearningLogForm, useLearningLogForm } from '../hooks/useLearningLogForm';
import type { LearningLogFormModel, LearningMomentFeedModel, LearningMomentKind } from '../types/dailyLog.types';
import type { LearningOutcomeModel } from '../types/learningOutcome.types';

interface LearningLogFormProps {
  children: ChildModel[];
  moment?: LearningMomentFeedModel | null;
  learningOutcomes: LearningOutcomeModel[];
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
] satisfies { label: string; value: LearningMomentKind }[];

export const LearningLogForm = ({ children, moment, learningOutcomes, saving, visible, onHide, onSave }: LearningLogFormProps) => {
  const initialDraft = useMemo(() => (moment ? getLearningLogForm(moment) : createEmptyLearningLogForm(children[0]?.id ?? '')), [children, moment]);
  const form = useLearningLogForm(initialDraft);
  const { draft, errors } = form;
  const hasErrors = Object.keys(errors).length > 0;
  const title = moment ? 'Edit learning log' : 'Add learning log';

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
    <AppDialog header={title} visible={visible} onHide={handleCancel}>
      <div className="space-y-4 pt-4">
        <div className="grid gap-4 md:grid-cols-3">
          <AppSelect
            label="Child"
            options={childOptions}
            value={draft.childId}
            onChange={(value) => {
              form.updateField('childId', value);
            }}
            disabled={childOptions.length === 0 || Boolean(moment)}
            error={errors.childId}
            required
          />
          <AppInputText
            error={errors.logDate}
            label="Date"
            required
            disabled={Boolean(moment)}
            type="date"
            value={draft.logDate}
            onChange={(event) => {
              form.updateField('logDate', event.target.value);
            }}
          />
          <AppSelect
            label="Type"
            options={learningKindOptions}
            value={draft.kind}
            onChange={(value) => {
              form.updateField('kind', value);
            }}
            required
          />
        </div>
        <AppInputText
          autoFocus={!moment}
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
        <AppMultiCheckboxSelector
          error={errors.learningOutcomeIds}
          label="Learning outcomes"
          options={learningOutcomes.map((outcome) => ({
            inputId: `learning-outcome-${outcome.learningOutcomeId}`,
            label: (
              <>
                <span className="block font-semibold text-brand-heading">{outcome.name}</span>
                <span className="mt-1 block text-xs text-brand-muted">{outcome.category}</span>
              </>
            ),
            value: outcome.learningOutcomeId
          }))}
          required
          selectedValues={draft.learningOutcomeIds}
          onToggle={handleOutcomeToggle}
        />

        <div className="flex justify-end gap-3 pt-2">
          <AppButton label="Cancel" type="button" variant="secondary" onClick={handleCancel} />
          <AppButton disabled={saving || !form.isValid || hasErrors} label={saving ? 'Saving...' : moment ? 'Save log' : 'Add log'} type="button" onClick={handleSave} />
        </div>
      </div>
    </AppDialog>
  );
};
