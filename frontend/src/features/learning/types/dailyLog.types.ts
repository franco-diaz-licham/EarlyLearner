export interface DailyLogResponse {
  dailyLogId: string;
  householdId: string;
  childId: string;
  logDate: string;
  learningMomentCount: number;
}

export interface DailyLogModel {
  dailyLogId: string;
  householdId: string;
  childId: string;
  logDate: string;
  learningMomentCount: number;
}

export interface CreateDailyLogRequest {
  childId: string;
  logDate: string;
}
