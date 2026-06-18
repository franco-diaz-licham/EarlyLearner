import type {
  HomeDashboardChildModel,
  HomeDashboardChildResponse,
  HomeDashboardMetricModel,
  HomeDashboardMetricResponse,
  HomeDashboardModel,
  HomeDashboardPlannedSessionModel,
  HomeDashboardPlannedSessionResponse,
  HomeDashboardRecentActivityModel,
  HomeDashboardRecentActivityResponse,
  HomeDashboardResponse
} from '../types/dashboard.types';

export const mapHomeDashboardChildResponseToModel = (response: HomeDashboardChildResponse): HomeDashboardChildModel => ({
  childId: response.childId,
  givenName: response.givenName,
  dateOfBirth: response.dateOfBirth
});

export const mapHomeDashboardMetricResponseToModel = (response: HomeDashboardMetricResponse): HomeDashboardMetricModel => ({
  label: response.label,
  value: response.value,
  detail: response.detail
});

export const mapHomeDashboardPlannedSessionResponseToModel = (response: HomeDashboardPlannedSessionResponse): HomeDashboardPlannedSessionModel => ({
  sessionId: response.sessionId,
  learningPlanId: response.learningPlanId,
  plannedDate: response.plannedDate,
  title: response.title,
  status: response.status
});

export const mapHomeDashboardRecentActivityResponseToModel = (response: HomeDashboardRecentActivityResponse): HomeDashboardRecentActivityModel => ({
  dailyLogId: response.dailyLogId,
  childId: response.childId,
  logDate: response.logDate,
  completedActivityCount: response.completedActivityCount,
  readingEntryCount: response.readingEntryCount,
  routineEntryCount: response.routineEntryCount
});

export const mapHomeDashboardResponseToModel = (response: HomeDashboardResponse): HomeDashboardModel => ({
  children: response.children.map(mapHomeDashboardChildResponseToModel),
  metrics: response.metrics.map(mapHomeDashboardMetricResponseToModel),
  upcomingSessions: response.upcomingSessions.map(mapHomeDashboardPlannedSessionResponseToModel),
  recentActivities: response.recentActivities.map(mapHomeDashboardRecentActivityResponseToModel)
});
