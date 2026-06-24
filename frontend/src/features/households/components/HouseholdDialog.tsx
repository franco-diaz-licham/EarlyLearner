import { AppButton } from '../../../shared/ui/AppButton';
import { AppDialog } from '../../../shared/ui/AppDialog';
import { AppInputText } from '../../../shared/ui/AppInputText';

interface HouseholdDialogProps {
  saving: boolean;
  visible: boolean;
  name: string;
  onChange: (name: string) => void;
  onHide: () => void;
  onSave: () => void;
}

export const HouseholdDialog = ({ name, saving, visible, onChange, onHide, onSave }: HouseholdDialogProps) => {
  return (
    <AppDialog header="Rename household" visible={visible} onHide={onHide}>
      <div className="space-y-4 pt-4">
        <label className="block">
          <span className="mb-2 block text-sm font-semibold text-brand-heading">Household name</span>
          <AppInputText
            autoFocus
            placeholder="Sophia's household"
            value={name}
            onChange={(event) => {
              onChange(event.target.value);
            }}
          />
        </label>
        <div className="flex justify-end gap-3 pt-2">
          <AppButton label="Cancel" variant="secondary" onClick={onHide} />
          <AppButton disabled={saving} label={saving ? 'Saving...' : 'Save household'} onClick={onSave} />
        </div>
      </div>
    </AppDialog>
  );
};
