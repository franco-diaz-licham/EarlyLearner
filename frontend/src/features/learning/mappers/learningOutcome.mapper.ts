import type { LearningOutcomeFormModel } from '../hooks/useLearningOutcomeForm';
import type { CreateLearningOutcomeRequest, LearningOutcomeModel, LearningOutcomeResponse, UpdateLearningOutcomeRequest } from '../types/learningOutcome.types';

export const mapLearningOutcomeResponseToModel = (response: LearningOutcomeResponse): LearningOutcomeModel => ({
  learningOutcomeId: response.learningOutcomeId,
  code: response.code,
  name: response.name,
  description: response.description,
  category: response.category,
  sortOrder: response.sortOrder,
  status: response.status
});

export const mapLearningOutcomeResponsesToModels = (responses: LearningOutcomeResponse[]): LearningOutcomeModel[] => responses.map(mapLearningOutcomeResponseToModel);

export const mapLearningOutcomeFormToCreateLearningOutcomeRequest = (form: LearningOutcomeFormModel): CreateLearningOutcomeRequest => ({
  code: form.code,
  name: form.name,
  description: form.description,
  category: form.category,
  sortOrder: form.sortOrder
});

export const mapLearningOutcomeFormToUpdateLearningOutcomeRequest = (form: LearningOutcomeFormModel): UpdateLearningOutcomeRequest => ({
  name: form.name,
  description: form.description,
  category: form.category,
  sortOrder: form.sortOrder
});