import type {
  HomeChildModel,
  HomeChildResponse,
  HomeMetricModel,
  HomeMetricResponse,
  HomeModel,
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

export const mapHomeRecentActivityResponseToModel = (response: HomeRecentActivityResponse): HomeRecentActivityModel => ({
  dailyLogId: response.dailyLogId,
  childId: response.childId,
  logDate: response.logDate,
  learningMomentCount: response.learningMomentCount
});

export const mapHomeResponseToModel = (response: HomeResponse): HomeModel => ({
  children: response.children.map(mapHomeChildResponseToModel),
  metrics: response.metrics.map(mapHomeMetricResponseToModel),
  recentActivities: response.recentActivities.map(mapHomeRecentActivityResponseToModel)
});
