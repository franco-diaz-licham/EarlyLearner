import { AppButton } from '../../../shared/ui/AppButton';
import { AppDialog } from '../../../shared/ui/AppDialog';
import { AppInputText } from '../../../shared/ui/AppInputText';
import { AppSelect } from '../../../shared/ui/AppSelect';
import { useInviteCarerForm } from '../hooks/useInviteCarerForm';
import type { HouseholdModel, HouseholdRole, InviteCarerForm } from '../types/household.types';

interface InviteCarerDialogProps {
  household: HouseholdModel | null;
  saving: boolean;
  visible: boolean;
  onHide: () => void;
  onSave: (form: InviteCarerForm) => void;
}

const roleOptions: { label: string; value: HouseholdRole }[] = [
  { label: 'Caregiver', value: 'caregiver' },
  { label: 'Viewer', value: 'viewer' }
];

export const InviteCarerDialog = ({ household, saving, visible, onHide, onSave }: InviteCarerDialogProps) => {
  const form = useInviteCarerForm();
  const { draft, errors } = form;
  const hasErrors = Object.keys(errors).length > 0;

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
    <AppDialog header={household ? `Invite to ${household.name}` : 'Invite carer'} visible={visible} onHide={onHide}>
      <div className="space-y-4 pt-4">
        <label className="block">
          <span className="mb-2 block text-sm font-semibold text-brand-heading">Email</span>
          <AppInputText
            autoFocus
            placeholder="parent@example.com"
            value={draft.email}
            onChange={(event) => {
              form.updateField('email', event.target.value);
            }}
          />
          {errors.email ? <span className="mt-1 block text-sm font-semibold text-red-600">{errors.email}</span> : null}
        </label>

        <label className="block">
          <span className="mb-2 block text-sm font-semibold text-brand-heading">Role</span>
          <AppSelect
            options={roleOptions}
            value={draft.role}
            onChange={(value) => {
              form.updateField('role', value);
            }}
          />
          {errors.role ? <span className="mt-1 block text-sm font-semibold text-red-600">{errors.role}</span> : null}
        </label>

        <div className="flex justify-end gap-3 pt-2">
          <AppButton label="Cancel" variant="secondary" onClick={handleCancel} />
          <AppButton disabled={!household || saving || (!form.isValid && hasErrors)} label={saving ? 'Inviting...' : 'Invite'} onClick={handleSave} />
        </div>
      </div>
    </AppDialog>
  );
};
