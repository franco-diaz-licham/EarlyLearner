export interface HomeResponse {
  children: HomeChildResponse[];
  metrics: HomeMetricResponse[];
  upcomingSessions: HomePlannedSessionResponse[];
  recentActivities: HomeRecentActivityResponse[];
}

export interface HomeChildResponse {
  childId: string;
  givenName: string;
  dateOfBirth: string;
}

export interface HomeMetricResponse {
  label: string;
  value: number;
  detail: string;
}

export interface HomePlannedSessionResponse {
  sessionId: string;
  learningPlanId: string;
  plannedDate: string;
  title: string;
  status: string;
}

export interface HomeRecentActivityResponse {
  dailyLogId: string;
  childId: string;
  logDate: string;
  completedActivityCount: number;
  readingEntryCount: number;
  routineEntryCount: number;
}

export interface HomeModel {
  children: HomeChildModel[];
  metrics: HomeMetricModel[];
  upcomingSessions: HomePlannedSessionModel[];
  recentActivities: HomeRecentActivityModel[];
}

export interface HomeChildModel {
  childId: string;
  givenName: string;
  dateOfBirth: string;
}

export interface HomeMetricModel {
  label: string;
  value: number;
  detail: string;
}

export interface HomePlannedSessionModel {
  sessionId: string;
  learningPlanId: string;
  plannedDate: string;
  title: string;
  status: string;
}

export interface HomeRecentActivityModel {
  dailyLogId: string;
  childId: string;
  logDate: string;
  completedActivityCount: number;
  readingEntryCount: number;
  routineEntryCount: number;
}

export interface GetHomeParams {
  householdId: string;
  today?: string;
}
