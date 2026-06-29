import { useCallback, useMemo, useState } from 'react';
import { z } from 'zod';
import type { AddChildForm } from '../types/household.types';

const addChildSchema = z.object({
  firstName: z.string().trim().min(1, 'First name is required.'),
  lastName: z.string().trim().min(1, 'Last name is required.'),
  dateOfBirth: z
    .string()
    .trim()
    .regex(/^\d{4}-\d{2}-\d{2}$/, 'Date of birth is required.'),
  avatarStoredFileId: z.string().nullable()
});

type AddChildFormErrors = Partial<Record<keyof AddChildForm, string>>;
type AddChildTouchedFields = Partial<Record<keyof AddChildForm, boolean>>;

const createEmptyAddChildForm = (): AddChildForm => ({
  firstName: '',
  lastName: '',
  dateOfBirth: '',
  avatarStoredFileId: null
});

const getAddChildFormErrors = (draft: AddChildForm): AddChildFormErrors => {
  const result = addChildSchema.safeParse(draft);
  if (result.success) return {};

  return result.error.issues.reduce<AddChildFormErrors>((errors, issue) => {
    const field = issue.path[0] as keyof AddChildForm | undefined;
    if (field && !errors[field]) errors[field] = issue.message;
    return errors;
  }, {});
};

const getVisibleAddChildFormErrors = (validationErrors: AddChildFormErrors, touchedFields: AddChildTouchedFields, hasSubmitted: boolean): AddChildFormErrors => {
  if (hasSubmitted) return validationErrors;

  return Object.entries(validationErrors).reduce<AddChildFormErrors>((visibleErrors, [field, message]) => {
    const formField = field as keyof AddChildForm;
    if (touchedFields[formField]) visibleErrors[formField] = message;
    return visibleErrors;
  }, {});
};

export const useAddChildForm = (initialDraft: AddChildForm = createEmptyAddChildForm()) => {
  const [draft, setDraft] = useState<AddChildForm>(() => initialDraft);
  const [hasSubmitted, setHasSubmitted] = useState(false);
  const [touchedFields, setTouchedFields] = useState<AddChildTouchedFields>({});

  const validationErrors = useMemo(() => getAddChildFormErrors(draft), [draft]);
  const errors = useMemo(() => getVisibleAddChildFormErrors(validationErrors, touchedFields, hasSubmitted), [hasSubmitted, touchedFields, validationErrors]);
  const isValid = useMemo(() => Object.keys(validationErrors).length === 0, [validationErrors]);

  const reset = useCallback(
    (nextDraft = initialDraft) => {
      setHasSubmitted(false);
      setTouchedFields({});
      setDraft(nextDraft);
    },
    [initialDraft]
  );

  const getValidForm = useCallback((): AddChildForm | null => {
    setHasSubmitted(true);
    const result = addChildSchema.safeParse(draft);
    return result.success ? result.data : null;
  }, [draft]);

  const updateField = <TField extends keyof AddChildForm>(field: TField, value: AddChildForm[TField]) => {
    setDraft((currentDraft) => ({ ...currentDraft, [field]: value }));
    setTouchedFields((currentFields) => ({ ...currentFields, [field]: true }));
  };

  return {
    draft,
    errors,
    isValid,
    getValidForm,
    reset,
    updateField
  };
};
