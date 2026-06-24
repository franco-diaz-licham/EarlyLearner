import { AppButton } from '../../../shared/ui/AppButton';
import { AppDialog } from '../../../shared/ui/AppDialog';
import { AppInputText } from '../../../shared/ui/AppInputText';
import { useAddChildForm } from '../hooks/useAddChildForm';
import type { AddChildForm, ChildModel, HouseholdModel } from '../types/household.types';

interface AddChildDialogProps {
  child: ChildModel | null;
  household: HouseholdModel | null;
  saving: boolean;
  visible: boolean;
  onHide: () => void;
  onSave: (form: AddChildForm) => void;
}

const getChildForm = (child: ChildModel): AddChildForm => ({
  firstName: child.firstName,
  lastName: child.lastName,
  dateOfBirth: child.dateOfBirth
});

export const AddChildDialog = ({ child, household, saving, visible, onHide, onSave }: AddChildDialogProps) => {
  const form = useAddChildForm(child ? getChildForm(child) : undefined);
  const { draft, errors } = form;
  const hasErrors = Object.keys(errors).length > 0;
  const title = child ? 'Edit child' : 'Add child';
  const savingLabel = child ? 'Saving...' : 'Adding...';
  const saveLabel = child ? 'Save child' : 'Add child';

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
    <AppDialog header={household ? `${title} in ${household.name}` : title} visible={visible} onHide={onHide}>
      <div className="space-y-4 pt-4">
        <div className="grid gap-4 sm:grid-cols-2">
          <label className="block">
            <span className="mb-2 block text-sm font-semibold text-brand-heading">First name</span>
            <AppInputText
              autoFocus
              placeholder="Sophia"
              value={draft.firstName}
              onChange={(event) => {
                form.updateField('firstName', event.target.value);
              }}
            />
            {errors.firstName ? <span className="mt-1 block text-sm font-semibold text-red-600">{errors.firstName}</span> : null}
          </label>
          <label className="block">
            <span className="mb-2 block text-sm font-semibold text-brand-heading">Last name</span>
            <AppInputText
              placeholder="Rivera"
              value={draft.lastName}
              onChange={(event) => {
                form.updateField('lastName', event.target.value);
              }}
            />
            {errors.lastName ? <span className="mt-1 block text-sm font-semibold text-red-600">{errors.lastName}</span> : null}
          </label>
        </div>

        <label className="block">
          <span className="mb-2 block text-sm font-semibold text-brand-heading">Date of birth</span>
          <AppInputText
            type="date"
            value={draft.dateOfBirth}
            onChange={(event) => {
              form.updateField('dateOfBirth', event.target.value);
            }}
          />
          {errors.dateOfBirth ? <span className="mt-1 block text-sm font-semibold text-red-600">{errors.dateOfBirth}</span> : null}
        </label>

        <div className="flex justify-end gap-3 pt-2">
          <AppButton label="Cancel" variant="secondary" onClick={handleCancel} />
          <AppButton disabled={!household || saving || (!form.isValid && hasErrors)} label={saving ? savingLabel : saveLabel} onClick={handleSave} />
        </div>
      </div>
    </AppDialog>
  );
};
