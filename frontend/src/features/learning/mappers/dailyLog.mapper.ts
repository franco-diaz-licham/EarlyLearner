import type { CreateDailyLogRequest, DailyLogModel, DailyLogResponse, LearningLogFormModel, LearningMomentFeedModel, LearningMomentFeedResponse, UpdateLearningMomentRequest } from '../types/dailyLog.types';

export const mapDailyLogResponseToModel = (response: DailyLogResponse): DailyLogModel => ({
  dailyLogId: response.dailyLogId,
  householdId: response.householdId,
  childId: response.childId,
  logDate: response.logDate,
  learningMomentCount: response.learningMomentCount,
  learningMoments: response.learningMoments.map((moment) => ({
    learningMomentId: moment.learningMomentId,
    kind: moment.kind,
    title: moment.title,
    notes: moment.notes,
    learningOutcomeIds: moment.learningOutcomeIds
  }))
});

export const mapDailyLogResponsesToModels = (responses: DailyLogResponse[]): DailyLogModel[] => responses.map(mapDailyLogResponseToModel);

export const mapLearningMomentFeedResponseToModel = (response: LearningMomentFeedResponse): LearningMomentFeedModel => ({
  learningMomentId: response.learningMomentId,
  dailyLogId: response.dailyLogId,
  householdId: response.householdId,
  childId: response.childId,
  logDate: response.logDate,
  kind: response.kind,
  title: response.title,
  notes: response.notes,
  learningOutcomeIds: response.learningOutcomeIds
});

export const mapLearningLogFormToCreateDailyLogRequest = (form: LearningLogFormModel): CreateDailyLogRequest => ({
  childId: form.childId,
  logDate: form.logDate,
  kind: form.kind,
  title: form.title,
  notes: form.notes,
  learningOutcomeIds: form.learningOutcomeIds
});

export const mapLearningLogFormToUpdateLearningMomentRequest = (form: LearningLogFormModel): UpdateLearningMomentRequest => ({
  kind: form.kind,
  title: form.title,
  notes: form.notes,
  learningOutcomeIds: form.learningOutcomeIds
});