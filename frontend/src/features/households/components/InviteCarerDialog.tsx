import { AppButton } from '../../../shared/ui/AppButton';
import { AppDialog } from '../../../shared/ui/AppDialog';
import { AppInputText } from '../../../shared/ui/AppInputText';
import type { HouseholdModel, HouseholdRole, InviteHouseholdCarerRequest } from '../types/household.types';

interface InviteCarerDialogProps {
  draft: InviteHouseholdCarerRequest;
  household: HouseholdModel | null;
  saving: boolean;
  visible: boolean;
  onChange: (draft: InviteHouseholdCarerRequest) => void;
  onHide: () => void;
  onSave: () => void;
}

const roleOptions: { label: string; value: HouseholdRole }[] = [
  { label: 'Caregiver', value: 2 },
  { label: 'Viewer', value: 3 }
];

export const InviteCarerDialog = ({ draft, household, saving, visible, onChange, onHide, onSave }: InviteCarerDialogProps) => {
  const canSave = Boolean(household && draft.email.trim() && draft.role);

  const updateField = (field: keyof InviteHouseholdCarerRequest, value: string | HouseholdRole) => {
    onChange({ ...draft, [field]: value });
  };

  return (
    <AppDialog header={household ? `Invite to ${household.name}` : 'Invite carer'} visible={visible} onHide={onHide}>
      <div className="space-y-4 pt-4">
        <label className="block">
          <span className="mb-2 block text-sm font-semibold text-brand-heading">Email</span>
          <AppInputText
            autoFocus
            placeholder="parent@example.com"
            value={draft.email}
            onChange={(event) => {
              updateField('email', event.target.value);
            }}
          />
        </label>

        <label className="block">
          <span className="mb-2 block text-sm font-semibold text-brand-heading">Role</span>
          <select
            className="min-h-10 w-full rounded-md border border-brand-border bg-white px-4 py-2 text-brand-text transition focus:border-brand-primary focus:outline-none focus:ring-2 focus:ring-brand-primary/20"
            value={draft.role}
            onChange={(event) => {
              updateField('role', Number(event.target.value) as HouseholdRole);
            }}
          >
            {roleOptions.map((role) => (
              <option key={role.value} value={role.value}>
                {role.label}
              </option>
            ))}
          </select>
        </label>

        <div className="flex justify-end gap-3 pt-2">
          <AppButton label="Cancel" variant="secondary" onClick={onHide} />
          <AppButton disabled={!canSave || saving} label={saving ? 'Inviting...' : 'Invite'} onClick={onSave} />
        </div>
      </div>
    </AppDialog>
  );
};
