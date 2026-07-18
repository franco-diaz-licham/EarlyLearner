import type {
  HomeChildModel,
  HomeChildResponse,
  HomeMetricModel,
  HomeMetricResponse,
  HomeModel,
  HomeOutcomeCoverageModel,
  HomeOutcomeCoverageResponse,
  HomeRecentActivityModel,
  HomeRecentActivityResponse,
  HomeRecentMomentModel,
  HomeRecentMomentResponse,
  HomeTodaySummaryModel,
  HomeTodaySummaryResponse,
  HomeResponse
} from '../types/home.types';

export const mapHomeChildResponseToModel = (response: HomeChildResponse): HomeChildModel => ({
  childId: response.childId,
  givenName: response.givenName,
  dateOfBirth: response.dateOfBirth,
  avatarStoredFileId: response.avatarStoredFileId
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

export const mapHomeTodaySummaryResponseToModel = (response: HomeTodaySummaryResponse): HomeTodaySummaryModel => ({
  dailyLogCount: response.dailyLogCount,
  learningMomentCount: response.learningMomentCount,
  childrenObservedCount: response.childrenObservedCount
});

export const mapHomeOutcomeCoverageResponseToModel = (response: HomeOutcomeCoverageResponse): HomeOutcomeCoverageModel => ({
  activeOutcomeCount: response.activeOutcomeCount,
  touchedThisWeekCount: response.touchedThisWeekCount,
  untouchedActiveOutcomeCount: response.untouchedActiveOutcomeCount
});

export const mapHomeRecentMomentResponseToModel = (response: HomeRecentMomentResponse): HomeRecentMomentModel => ({
  dailyLogId: response.dailyLogId,
  learningMomentId: response.learningMomentId,
  childId: response.childId,
  childName: response.childName,
  logDate: response.logDate,
  kind: response.kind,
  title: response.title,
  notes: response.notes,
  outcomeNames: response.outcomeNames
});

export const mapHomeResponseToModel = (response: HomeResponse): HomeModel => ({
  children: response.children.map(mapHomeChildResponseToModel),
  metrics: response.metrics.map(mapHomeMetricResponseToModel),
  recentActivities: response.recentActivities.map(mapHomeRecentActivityResponseToModel),
  today: mapHomeTodaySummaryResponseToModel(response.today),
  outcomeCoverage: mapHomeOutcomeCoverageResponseToModel(response.outcomeCoverage),
  recentMoments: response.recentMoments.map(mapHomeRecentMomentResponseToModel)
});
