import { AppButton } from '../../../shared/ui/AppButton';
import { AppDialog } from '../../../shared/ui/AppDialog';
import { AppInputText } from '../../../shared/ui/AppInputText';
import { AppInputTextArea } from '../../../shared/ui/AppInputTextArea';
import type { LearningPlanModel } from '../types/learningPlan.types';

interface LearningPlanDialogProps {
  learningPlan: LearningPlanModel;
  saving?: boolean;
  visible: boolean;
  onChange: (learningPlan: LearningPlanModel) => void;
  onHide: () => void;
  onSave: () => void;
}

export const LearningPlanDialog = ({ learningPlan, saving = false, visible, onChange, onHide, onSave }: LearningPlanDialogProps) => {
  const canSave = Boolean(learningPlan.childId && learningPlan.startDate && learningPlan.endDate && learningPlan.focus.trim());

  const updateField = (field: keyof LearningPlanModel, value: string) => {
    onChange({ ...learningPlan, [field]: value });
  };

  return (
    <AppDialog header="New learning plan" visible={visible} onHide={onHide}>
      <div className="space-y-4 pt-4">
        <label className="block">
          <span className="mb-2 block text-sm font-semibold text-brand-heading">Child ID</span>
          <AppInputText
            value={learningPlan.childId}
            onChange={(event) => {
              updateField('childId', event.target.value);
            }}
          />
        </label>

        <div className="grid gap-4 sm:grid-cols-2">
          <label className="block">
            <span className="mb-2 block text-sm font-semibold text-brand-heading">Start date</span>
            <AppInputText
              type="date"
              value={learningPlan.startDate}
              onChange={(event) => {
                updateField('startDate', event.target.value);
              }}
            />
          </label>

          <label className="block">
            <span className="mb-2 block text-sm font-semibold text-brand-heading">End date</span>
            <AppInputText
              type="date"
              value={learningPlan.endDate}
              onChange={(event) => {
                updateField('endDate', event.target.value);
              }}
            />
          </label>
        </div>

        <label className="block">
          <span className="mb-2 block text-sm font-semibold text-brand-heading">Focus</span>
          <AppInputTextArea
            autoResize
            rows={4}
            value={learningPlan.focus}
            onChange={(event) => {
              updateField('focus', event.target.value);
            }}
          />
        </label>

        <div className="flex justify-end gap-3 pt-2">
          <AppButton disabled={saving} label="Cancel" variant="secondary" onClick={onHide} />
          <AppButton disabled={!canSave || saving} label={saving ? 'Saving...' : 'Create plan'} onClick={onSave} />
        </div>
      </div>
    </AppDialog>
  );
};
