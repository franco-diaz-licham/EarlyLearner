import { UilEditAlt, UilEstate, UilPlus, UilUserPlus } from '@iconscout/react-unicons';
import { AppButton } from '../../../shared/ui/AppButton';
import { AppCard } from '../../../shared/ui/AppCard';
import { AppIconButton } from '../../../shared/ui/AppIconButton';
import { AppInputText } from '../../../shared/ui/AppInputText';
import type { HouseholdModel } from '../types/household.types';

interface HouseholdHeaderProps {
  household: HouseholdModel | null;
  isEditingName: boolean;
  isLoading: boolean;
  isError: boolean;
  isSavingName: boolean;
  nameDraft: string;
  onAddChild: (household: HouseholdModel) => void;
  onCancelRename: () => void;
  onInviteCarer: (household: HouseholdModel) => void;
  onNameDraftChange: (name: string) => void;
  onSaveRename: () => void;
  onStartRename: (household: HouseholdModel) => void;
}

export const HouseholdHeader = ({ household, isEditingName, isLoading, isError, isSavingName, nameDraft, onAddChild, onCancelRename, onInviteCarer, onNameDraftChange, onSaveRename, onStartRename }: HouseholdHeaderProps) => {
  return (
    <AppCard>
      {isLoading ? <p className="text-sm text-brand-muted">Loading household...</p> : null}
      {isError ? <p className="text-sm font-semibold text-red-600">Household could not be loaded.</p> : null}
      {!isLoading && !household ? (
        <div className="rounded-md border border-dashed border-brand-border p-6 text-center">
          <div className="mx-auto flex h-12 w-12 items-center justify-center rounded-md bg-brand-primary-soft text-brand-primary">
            <UilEstate aria-hidden="true" className="h-6 w-6" />
          </div>
          <h2 className="mt-4 text-lg font-bold text-brand-heading">No household yet</h2>
          <p className="mt-2 text-sm leading-6 text-brand-muted">Finish signup to create your household and unlock planning and learning records.</p>
        </div>
      ) : null}

      {household ? (
        <div className="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
          <div className="min-w-0">
            <p className="text-sm font-semibold text-brand-muted">Current household</p>
            {isEditingName ? (
              <div className="mt-2 flex max-w-xl items-center gap-2">
                <AppInputText
                  aria-label="Household name"
                  autoFocus
                  value={nameDraft}
                  onChange={(event) => {
                    onNameDraftChange(event.target.value);
                  }}
                  onKeyDown={(event) => {
                    if (event.key === 'Enter') onSaveRename();
                    if (event.key === 'Escape') onCancelRename();
                  }}
                />
                <AppButton disabled={isSavingName} label={isSavingName ? 'Saving...' : 'Save'} onClick={onSaveRename} />
                <AppButton label="Cancel" variant="secondary" onClick={onCancelRename} />
              </div>
            ) : (
              <div className="mt-1 flex min-w-0 items-center gap-2">
                <h1 id="households-title" className="truncate text-3xl font-bold text-brand-heading">
                  {household.name}
                </h1>
                <AppIconButton
                  aria-label={`Rename ${household.name}`}
                  icon={<UilEditAlt aria-hidden="true" className="h-5 w-5" />}
                  onClick={() => {
                    onStartRename(household);
                  }}
                />
              </div>
            )}
            <p className="mt-2 max-w-2xl text-brand-muted">Manage the people who can access this household and keep their responsibilities clear.</p>
          </div>
          <div className="flex flex-wrap gap-3">
            <AppButton
              icon={<UilPlus aria-hidden="true" className="h-5 w-5" />}
              label="Add child"
              onClick={() => {
                onAddChild(household);
              }}
            />
            <AppButton
              icon={<UilUserPlus aria-hidden="true" className="h-5 w-5" />}
              label="Invite carer"
              variant="secondary"
              onClick={() => {
                onInviteCarer(household);
              }}
            />
          </div>
        </div>
      ) : null}
    </AppCard>
  );
};
