export type LearningMomentKind = 'activity' | 'observation' | 'reading' | 'routine';

export interface LearningMomentResponse {
  learningMomentId: string;
  kind: LearningMomentKind;
  title: string;
  notes: string;
  learningOutcomeIds: string[];
}

export interface LearningMomentModel {
  learningMomentId: string;
  kind: LearningMomentKind;
  title: string;
  notes: string;
  learningOutcomeIds: string[];
}

export interface DailyLogResponse {
  dailyLogId: string;
  householdId: string;
  childId: string;
  logDate: string;
  learningMomentCount: number;
  learningMoments: LearningMomentResponse[];
}

export interface DailyLogModel {
  dailyLogId: string;
  householdId: string;
  childId: string;
  logDate: string;
  learningMomentCount: number;
  learningMoments: LearningMomentModel[];
}

export interface CreateDailyLogRequest {
  childId: string;
  logDate: string;
  kind: LearningMomentKind;
  title: string;
  notes: string;
  learningOutcomeIds: string[];
}

export interface LearningLogFormModel {
  childId: string;
  logDate: string;
  kind: LearningMomentKind;
  title: string;
  notes: string;
  learningOutcomeIds: string[];
}
