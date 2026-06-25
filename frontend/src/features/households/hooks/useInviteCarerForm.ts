import { useCallback, useMemo, useState } from 'react';
import { z } from 'zod';
import type { InviteCarerForm } from '../types/household.types';

const inviteCarerSchema = z.object({
  email: z.email('Enter a valid email address.'),
  role: z.enum(['caregiver', 'viewer'])
});

type InviteCarerFormErrors = Partial<Record<keyof InviteCarerForm, string>>;
type InviteCarerTouchedFields = Partial<Record<keyof InviteCarerForm, boolean>>;

const createEmptyInviteCarerForm = (): InviteCarerForm => ({
  email: '',
  role: 'caregiver'
});

const getInviteCarerFormErrors = (draft: InviteCarerForm): InviteCarerFormErrors => {
  const result = inviteCarerSchema.safeParse(draft);
  if (result.success) return {};

  return result.error.issues.reduce<InviteCarerFormErrors>((errors, issue) => {
    const field = issue.path[0] as keyof InviteCarerForm | undefined;
    if (field && !errors[field]) errors[field] = issue.message;
    return errors;
  }, {});
};

const getVisibleInviteCarerFormErrors = (validationErrors: InviteCarerFormErrors, touchedFields: InviteCarerTouchedFields, hasSubmitted: boolean): InviteCarerFormErrors => {
  if (hasSubmitted) return validationErrors;

  return Object.entries(validationErrors).reduce<InviteCarerFormErrors>((visibleErrors, [field, message]) => {
    const formField = field as keyof InviteCarerForm;
    if (touchedFields[formField]) visibleErrors[formField] = message;
    return visibleErrors;
  }, {});
};

export const useInviteCarerForm = () => {
  const [draft, setDraft] = useState<InviteCarerForm>(() => createEmptyInviteCarerForm());
  const [hasSubmitted, setHasSubmitted] = useState(false);
  const [touchedFields, setTouchedFields] = useState<InviteCarerTouchedFields>({});
  const validationErrors = useMemo(() => getInviteCarerFormErrors(draft), [draft]);
  const errors = useMemo(() => getVisibleInviteCarerFormErrors(validationErrors, touchedFields, hasSubmitted), [hasSubmitted, touchedFields, validationErrors]);
  const isValid = useMemo(() => Object.keys(validationErrors).length === 0, [validationErrors]);

  const reset = useCallback(() => {
    setHasSubmitted(false);
    setTouchedFields({});
    setDraft(createEmptyInviteCarerForm());
  }, []);

  const getValidForm = useCallback((): InviteCarerForm | null => {
    setHasSubmitted(true);
    const result = inviteCarerSchema.safeParse(draft);
    return result.success ? result.data : null;
  }, [draft]);

  const updateField = <TField extends keyof InviteCarerForm>(field: TField, value: InviteCarerForm[TField]) => {
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
