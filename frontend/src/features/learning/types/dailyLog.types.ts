export interface DailyLogResponse {
  dailyLogId: string;
  householdId: string;
  childId: string;
  logDate: string;
  completedActivityCount: number;
  readingEntryCount: number;
  routineEntryCount: number;
}

export interface DailyLogModel {
  dailyLogId: string;
  householdId: string;
  childId: string;
  logDate: string;
  completedActivityCount: number;
  readingEntryCount: number;
  routineEntryCount: number;
}

export interface CreateDailyLogRequest {
  householdId: string;
  childId: string;
  logDate: string;
}
