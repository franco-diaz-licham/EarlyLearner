import { AppButton } from '../../../shared/ui/AppButton';
import { AppDialog } from '../../../shared/ui/AppDialog';
import { AppInputText } from '../../../shared/ui/AppInputText';
import type { AddHouseholdChildRequest, HouseholdModel } from '../types/household.types';

interface AddChildDialogProps {
  draft: AddHouseholdChildRequest;
  household: HouseholdModel | null;
  saving: boolean;
  visible: boolean;
  onChange: (draft: AddHouseholdChildRequest) => void;
  onHide: () => void;
  onSave: () => void;
}

export const AddChildDialog = ({ draft, household, saving, visible, onChange, onHide, onSave }: AddChildDialogProps) => {
  const canSave = Boolean(household && draft.firstName.trim() && draft.lastName.trim() && draft.dateOfBirth);

  const updateField = (field: keyof AddHouseholdChildRequest, value: string) => {
    onChange({ ...draft, [field]: value });
  };

  return (
    <AppDialog header={household ? `Add child to ${household.name}` : 'Add child'} visible={visible} onHide={onHide}>
      <div className="space-y-4 pt-4">
        <div className="grid gap-4 sm:grid-cols-2">
          <label className="block">
            <span className="mb-2 block text-sm font-semibold text-brand-heading">First name</span>
            <AppInputText
              autoFocus
              placeholder="Sophia"
              value={draft.firstName}
              onChange={(event) => {
                updateField('firstName', event.target.value);
              }}
            />
          </label>
          <label className="block">
            <span className="mb-2 block text-sm font-semibold text-brand-heading">Last name</span>
            <AppInputText
              placeholder="Rivera"
              value={draft.lastName}
              onChange={(event) => {
                updateField('lastName', event.target.value);
              }}
            />
          </label>
        </div>

        <label className="block">
          <span className="mb-2 block text-sm font-semibold text-brand-heading">Date of birth</span>
          <AppInputText
            type="date"
            value={draft.dateOfBirth}
            onChange={(event) => {
              updateField('dateOfBirth', event.target.value);
            }}
          />
        </label>

        <div className="flex justify-end gap-3 pt-2">
          <AppButton label="Cancel" variant="secondary" onClick={onHide} />
          <AppButton disabled={!canSave || saving} label={saving ? 'Adding...' : 'Add child'} onClick={onSave} />
        </div>
      </div>
    </AppDialog>
  );
};
