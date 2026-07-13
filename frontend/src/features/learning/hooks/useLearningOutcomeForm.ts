import { useCallback, useMemo, useState } from 'react';
import { z } from 'zod';
import type { LearningOutcomeModel } from '../types/learningOutcome.types';

export interface LearningOutcomeFormModel {
  code: string;
  name: string;
  description: string;
  category: string;
  sortOrder: number;
}

const learningOutcomeSchema = z.object({
  code: z.string().trim().min(1, 'Code is required.'),
  name: z.string().trim().min(1, 'Name is required.').max(180, 'Name must be 180 characters or fewer.'),
  description: z.string().trim().min(1, 'Description is required.').max(1200, 'Description must be 1200 characters or fewer.'),
  category: z.string().trim().min(1, 'Category is required.').max(120, 'Category must be 120 characters or fewer.'),
  sortOrder: z.coerce.number().int('Sort order must be a whole number.').min(0, 'Sort order must be 0 or more.')
});

type LearningOutcomeFormErrors = Partial<Record<keyof LearningOutcomeFormModel, string>>;
type LearningOutcomeTouchedFields = Partial<Record<keyof LearningOutcomeFormModel, boolean>>;

export const createEmptyLearningOutcomeForm = (): LearningOutcomeFormModel => ({
  code: '',
  name: '',
  description: '',
  category: '',
  sortOrder: 0
});

export const getLearningOutcomeForm = (outcome: LearningOutcomeModel): LearningOutcomeFormModel => ({
  code: outcome.code,
  name: outcome.name,
  description: outcome.description,
  category: outcome.category,
  sortOrder: outcome.sortOrder
});

const getLearningOutcomeFormErrors = (draft: LearningOutcomeFormModel): LearningOutcomeFormErrors => {
  const result = learningOutcomeSchema.safeParse(draft);
  if (result.success) return {};

  return result.error.issues.reduce<LearningOutcomeFormErrors>((errors, issue) => {
    const field = issue.path[0] as keyof LearningOutcomeFormModel | undefined;
    if (field && !errors[field]) errors[field] = issue.message;
    return errors;
  }, {});
};

const getVisibleLearningOutcomeFormErrors = (validationErrors: LearningOutcomeFormErrors, touchedFields: LearningOutcomeTouchedFields, hasSubmitted: boolean): LearningOutcomeFormErrors => {
  if (hasSubmitted) return validationErrors;

  return Object.entries(validationErrors).reduce<LearningOutcomeFormErrors>((visibleErrors, [field, message]) => {
    const formField = field as keyof LearningOutcomeFormModel;
    if (touchedFields[formField]) visibleErrors[formField] = message;
    return visibleErrors;
  }, {});
};

export const useLearningOutcomeForm = (initialDraft: LearningOutcomeFormModel = createEmptyLearningOutcomeForm()) => {
  const [draft, setDraft] = useState<LearningOutcomeFormModel>(() => initialDraft);
  const [hasSubmitted, setHasSubmitted] = useState(false);
  const [touchedFields, setTouchedFields] = useState<LearningOutcomeTouchedFields>({});

  const validationErrors = useMemo(() => getLearningOutcomeFormErrors(draft), [draft]);
  const errors = useMemo(() => getVisibleLearningOutcomeFormErrors(validationErrors, touchedFields, hasSubmitted), [hasSubmitted, touchedFields, validationErrors]);
  const isValid = useMemo(() => Object.keys(validationErrors).length === 0, [validationErrors]);

  const reset = useCallback(
    (nextDraft = initialDraft) => {
      setHasSubmitted(false);
      setTouchedFields({});
      setDraft(nextDraft);
    },
    [initialDraft]
  );

  const getValidForm = useCallback((): LearningOutcomeFormModel | null => {
    setHasSubmitted(true);
    const result = learningOutcomeSchema.safeParse(draft);
    return result.success ? result.data : null;
  }, [draft]);

  const updateField = <TField extends keyof LearningOutcomeFormModel>(field: TField, value: LearningOutcomeFormModel[TField]) => {
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
