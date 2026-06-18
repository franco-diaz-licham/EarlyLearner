export interface HomeDashboardResponse {
  children: HomeDashboardChildResponse[];
  metrics: HomeDashboardMetricResponse[];
  upcomingSessions: HomeDashboardPlannedSessionResponse[];
  recentActivities: HomeDashboardRecentActivityResponse[];
}

export interface HomeDashboardChildResponse {
  childId: string;
  givenName: string;
  dateOfBirth: string;
}

export interface HomeDashboardMetricResponse {
  label: string;
  value: number;
  detail: string;
}

export interface HomeDashboardPlannedSessionResponse {
  sessionId: string;
  learningPlanId: string;
  plannedDate: string;
  title: string;
  status: string;
}

export interface HomeDashboardRecentActivityResponse {
  dailyLogId: string;
  childId: string;
  logDate: string;
  completedActivityCount: number;
  readingEntryCount: number;
  routineEntryCount: number;
}

export interface HomeDashboardModel {
  children: HomeDashboardChildModel[];
  metrics: HomeDashboardMetricModel[];
  upcomingSessions: HomeDashboardPlannedSessionModel[];
  recentActivities: HomeDashboardRecentActivityModel[];
}

export interface HomeDashboardChildModel {
  childId: string;
  givenName: string;
  dateOfBirth: string;
}

export interface HomeDashboardMetricModel {
  label: string;
  value: number;
  detail: string;
}

export interface HomeDashboardPlannedSessionModel {
  sessionId: string;
  learningPlanId: string;
  plannedDate: string;
  title: string;
  status: string;
}

export interface HomeDashboardRecentActivityModel {
  dailyLogId: string;
  childId: string;
  logDate: string;
  completedActivityCount: number;
  readingEntryCount: number;
  routineEntryCount: number;
}

export interface GetHomeDashboardParams {
  householdId: string;
  today?: string;
}
