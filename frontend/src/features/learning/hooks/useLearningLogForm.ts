import { useCallback, useMemo, useState } from 'react';
import { z } from 'zod';
import type { LearningLogFormModel } from '../types/dailyLog.types';

const learningLogSchema = z.object({
  childId: z.string().trim().min(1, 'Child is required.'),
  logDate: z
    .string()
    .trim()
    .regex(/^\d{4}-\d{2}-\d{2}$/, 'Date is required.'),
  kind: z.enum(['activity', 'observation', 'reading', 'routine']),
  title: z.string().trim().min(1, 'Title is required.').max(220, 'Title must be 220 characters or fewer.'),
  notes: z.string().trim().min(1, 'Notes are required.').max(2000, 'Notes must be 2000 characters or fewer.'),
  readinessOutcomeIds: z.array(z.string().trim().min(1)).min(1, 'At least one readiness outcome is required.')
});

type LearningLogFormErrors = Partial<Record<keyof LearningLogFormModel, string>>;
type LearningLogTouchedFields = Partial<Record<keyof LearningLogFormModel, boolean>>;

const formatDateInputValue = (date: Date) => date.toISOString().slice(0, 10);

export const createEmptyLearningLogForm = (childId = ''): LearningLogFormModel => ({
  childId,
  logDate: formatDateInputValue(new Date()),
  kind: 'activity',
  title: '',
  notes: '',
  readinessOutcomeIds: []
});

const getLearningLogFormErrors = (draft: LearningLogFormModel): LearningLogFormErrors => {
  const result = learningLogSchema.safeParse(draft);
  if (result.success) return {};

  return result.error.issues.reduce<LearningLogFormErrors>((errors, issue) => {
    const field = issue.path[0] as keyof LearningLogFormModel | undefined;
    if (field && !errors[field]) errors[field] = issue.message;
    return errors;
  }, {});
};

const getVisibleLearningLogFormErrors = (validationErrors: LearningLogFormErrors, touchedFields: LearningLogTouchedFields, hasSubmitted: boolean): LearningLogFormErrors => {
  if (hasSubmitted) return validationErrors;

  return Object.entries(validationErrors).reduce<LearningLogFormErrors>((visibleErrors, [field, message]) => {
    const formField = field as keyof LearningLogFormModel;
    if (touchedFields[formField]) visibleErrors[formField] = message;
    return visibleErrors;
  }, {});
};

export const useLearningLogForm = (initialDraft: LearningLogFormModel = createEmptyLearningLogForm()) => {
  const [draft, setDraft] = useState<LearningLogFormModel>(() => initialDraft);
  const [hasSubmitted, setHasSubmitted] = useState(false);
  const [touchedFields, setTouchedFields] = useState<LearningLogTouchedFields>({});

  const validationErrors = useMemo(() => getLearningLogFormErrors(draft), [draft]);
  const errors = useMemo(() => getVisibleLearningLogFormErrors(validationErrors, touchedFields, hasSubmitted), [hasSubmitted, touchedFields, validationErrors]);
  const isValid = useMemo(() => Object.keys(validationErrors).length === 0, [validationErrors]);

  const reset = useCallback(
    (nextDraft = initialDraft) => {
      setHasSubmitted(false);
      setTouchedFields({});
      setDraft(nextDraft);
    },
    [initialDraft]
  );

  const getValidForm = useCallback((): LearningLogFormModel | null => {
    setHasSubmitted(true);
    const result = learningLogSchema.safeParse(draft);
    return result.success ? result.data : null;
  }, [draft]);

  const updateField = <TField extends keyof LearningLogFormModel>(field: TField, value: LearningLogFormModel[TField]) => {
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
