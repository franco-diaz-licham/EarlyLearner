import { useMemo } from 'react';
import { AppButton } from '../../../shared/ui/AppButton';
import { AppDialog } from '../../../shared/ui/AppDialog';
import { AppInputText } from '../../../shared/ui/AppInputText';
import { AppInputTextArea } from '../../../shared/ui/AppInputTextArea';
import { createEmptyLearningOutcomeForm, getLearningOutcomeForm, useLearningOutcomeForm, type LearningOutcomeFormModel } from '../hooks/useLearningOutcomeForm';
import type { LearningOutcomeModel } from '../types/learningOutcome.types';

interface LearningOutcomeFormProps {
  outcome?: LearningOutcomeModel | null;
  saving: boolean;
  visible: boolean;
  onHide: () => void;
  onSave: (form: LearningOutcomeFormModel) => void;
}

export const LearningOutcomeForm = ({ outcome, saving, visible, onHide, onSave }: LearningOutcomeFormProps) => {
  const initialDraft = useMemo(() => (outcome ? getLearningOutcomeForm(outcome) : createEmptyLearningOutcomeForm()), [outcome]);
  const form = useLearningOutcomeForm(initialDraft);
  const { draft, errors } = form;
  const hasErrors = Object.keys(errors).length > 0;
  const title = outcome ? 'Edit learning outcome' : 'Add learning outcome';

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
        <div className="grid gap-4 md:grid-cols-[minmax(0,1fr)_8rem]">
          <AppInputText
            autoFocus={!outcome}
            disabled={Boolean(outcome)}
            error={errors.code}
            label="Code"
            placeholder="language-listening"
            required
            value={draft.code}
            onChange={(event) => {
              form.updateField('code', event.target.value);
            }}
          />
          <AppInputText
            error={errors.sortOrder}
            label="Order"
            min={0}
            required
            type="number"
            value={String(draft.sortOrder)}
            onChange={(event) => {
              form.updateField('sortOrder', Number(event.target.value));
            }}
          />
        </div>
        <AppInputText
          autoFocus={Boolean(outcome)}
          error={errors.name}
          label="Name"
          placeholder="Listens and responds"
          required
          value={draft.name}
          onChange={(event) => {
            form.updateField('name', event.target.value);
          }}
        />
        <AppInputText
          error={errors.category}
          label="Category"
          placeholder="Language"
          required
          value={draft.category}
          onChange={(event) => {
            form.updateField('category', event.target.value);
          }}
        />
        <div>
          <AppInputTextArea
            autoResize
            className="min-h-32"
            label="Description"
            placeholder="What does this outcome represent?"
            required
            rows={5}
            value={draft.description}
            onChange={(event) => {
              form.updateField('description', event.target.value);
            }}
          />
          {errors.description ? <span className="mt-1 block text-sm font-semibold text-red-600">{errors.description}</span> : null}
        </div>
        <div className="flex justify-end gap-3 pt-2">
          <AppButton label="Cancel" type="button" variant="secondary" onClick={handleCancel} />
          <AppButton disabled={saving || !form.isValid || hasErrors} label={saving ? 'Saving...' : outcome ? 'Save outcome' : 'Add outcome'} type="button" onClick={handleSave} />
        </div>
      </div>
    </AppDialog>
  );
};
