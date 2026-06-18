import type {
  HomeChildModel,
  HomeChildResponse,
  HomeMetricModel,
  HomeMetricResponse,
  HomeModel,
  HomePlannedSessionModel,
  HomePlannedSessionResponse,
  HomeRecentActivityModel,
  HomeRecentActivityResponse,
  HomeResponse
} from '../types/home.types';

export const mapHomeChildResponseToModel = (response: HomeChildResponse): HomeChildModel => ({
  childId: response.childId,
  givenName: response.givenName,
  dateOfBirth: response.dateOfBirth
});

export const mapHomeMetricResponseToModel = (response: HomeMetricResponse): HomeMetricModel => ({
  label: response.label,
  value: response.value,
  detail: response.detail
});

export const mapHomePlannedSessionResponseToModel = (response: HomePlannedSessionResponse): HomePlannedSessionModel => ({
  sessionId: response.sessionId,
  learningPlanId: response.learningPlanId,
  plannedDate: response.plannedDate,
  title: response.title,
  status: response.status
});

export const mapHomeRecentActivityResponseToModel = (response: HomeRecentActivityResponse): HomeRecentActivityModel => ({
  dailyLogId: response.dailyLogId,
  childId: response.childId,
  logDate: response.logDate,
  completedActivityCount: response.completedActivityCount,
  readingEntryCount: response.readingEntryCount,
  routineEntryCount: response.routineEntryCount
});

export const mapHomeResponseToModel = (response: HomeResponse): HomeModel => ({
  children: response.children.map(mapHomeChildResponseToModel),
  metrics: response.metrics.map(mapHomeMetricResponseToModel),
  upcomingSessions: response.upcomingSessions.map(mapHomePlannedSessionResponseToModel),
  recentActivities: response.recentActivities.map(mapHomeRecentActivityResponseToModel)
});
