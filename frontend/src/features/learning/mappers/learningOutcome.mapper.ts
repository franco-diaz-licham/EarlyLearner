import type { LearningOutcomeModel, LearningOutcomeResponse } from '../types/learningOutcome.types';

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
