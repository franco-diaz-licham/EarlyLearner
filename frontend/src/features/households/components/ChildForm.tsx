import { AppAvatar } from '../../../shared/ui/AppAvatar';
import { AppButton } from '../../../shared/ui/AppButton';
import { AppDialog } from '../../../shared/ui/AppDialog';
import { AppInputText } from '../../../shared/ui/AppInputText';
import { useState } from 'react';
import { useAddChildForm } from '../hooks/useChildForm';
import type { AddChildForm, ChildModel, HouseholdModel, SaveChildForm } from '../types/household.types';

interface ChildFormProps {
  child: ChildModel | null;
  household: HouseholdModel | null;
  saving: boolean;
  visible: boolean;
  onHide: () => void;
  onSave: (form: SaveChildForm) => void;
}

const getChildForm = (child: ChildModel): AddChildForm => ({
  firstName: child.firstName,
  lastName: child.lastName,
  dateOfBirth: child.dateOfBirth,
  avatarStoredFileId: child.avatarStoredFileId
});

export const ChildForm = ({ child, household, saving, visible, onHide, onSave }: ChildFormProps) => {
  const form = useAddChildForm(child ? getChildForm(child) : undefined);
  const [avatarFile, setAvatarFile] = useState<File | null>(null);
  const { draft, errors } = form;
  const hasErrors = Object.keys(errors).length > 0;
  const title = child ? 'Edit child' : 'Add child';
  const savingLabel = child ? 'Saving...' : 'Adding...';
  const saveLabel = child ? 'Save child' : 'Add child';
  const childInitials = `${draft.firstName.charAt(0)}${draft.lastName.charAt(0)}`;

  const handleSave = () => {
    const validForm = form.getValidForm();
    if (!validForm) return;
    onSave({ child: validForm, avatarFile });
  };

  const handleCancel = () => {
    form.reset();
    setAvatarFile(null);
    onHide();
  };

  const handleFileChanged = (file: File | null) => {
    setAvatarFile(file);
  };

  return (
    <AppDialog header={household ? `${title} in ${household.name}` : title} visible={visible} onHide={onHide}>
      <div className="space-y-4 pt-4">
        <AppAvatar key={`${child?.id ?? 'new'}-${visible ? 'visible' : 'hidden'}`} alt="Child avatar preview" disabled={saving} initials={childInitials} size="lg" onFileChange={handleFileChanged} />
        <div className="grid gap-4 sm:grid-cols-2">
          <div>
            <AppInputText
              autoFocus
              label="First name"
              placeholder="Sophia"
              required
              value={draft.firstName}
              onChange={(event) => {
                form.updateField('firstName', event.target.value);
              }}
            />
            {errors.firstName ? <span className="mt-1 block text-sm font-semibold text-red-600">{errors.firstName}</span> : null}
          </div>
          <div>
            <AppInputText
              label="Last name"
              placeholder="Rivera"
              required
              value={draft.lastName}
              onChange={(event) => {
                form.updateField('lastName', event.target.value);
              }}
            />
            {errors.lastName ? <span className="mt-1 block text-sm font-semibold text-red-600">{errors.lastName}</span> : null}
          </div>
        </div>

        <div>
          <AppInputText
            label="Date of birth"
            required
            type="date"
            value={draft.dateOfBirth}
            onChange={(event) => {
              form.updateField('dateOfBirth', event.target.value);
            }}
          />
          {errors.dateOfBirth ? <span className="mt-1 block text-sm font-semibold text-red-600">{errors.dateOfBirth}</span> : null}
        </div>

        <div className="flex justify-end gap-3 pt-2">
          <AppButton label="Cancel" variant="secondary" onClick={handleCancel} />
          <AppButton disabled={!household || saving || !form.isValid || hasErrors} label={saving ? savingLabel : saveLabel} onClick={handleSave} />
        </div>
      </div>
    </AppDialog>
  );
};
