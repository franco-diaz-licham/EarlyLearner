export interface HomeResponse {
  children: HomeChildResponse[];
  metrics: HomeMetricResponse[];
  recentActivities: HomeRecentActivityResponse[];
  today: HomeTodaySummaryResponse;
  outcomeCoverage: HomeOutcomeCoverageResponse;
  recentMoments: HomeRecentMomentResponse[];
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

export interface HomeRecentActivityResponse {
  dailyLogId: string;
  childId: string;
  logDate: string;
  learningMomentCount: number;
}

export interface HomeTodaySummaryResponse {
  dailyLogCount: number;
  learningMomentCount: number;
  childrenObservedCount: number;
}

export interface HomeOutcomeCoverageResponse {
  activeOutcomeCount: number;
  touchedThisWeekCount: number;
  untouchedActiveOutcomeCount: number;
}

export interface HomeRecentMomentResponse {
  dailyLogId: string;
  learningMomentId: string;
  childId: string;
  childName: string;
  logDate: string;
  kind: string;
  title: string;
  notes: string;
  outcomeNames: string[];
}

export interface HomeModel {
  children: HomeChildModel[];
  metrics: HomeMetricModel[];
  recentActivities: HomeRecentActivityModel[];
  today: HomeTodaySummaryModel;
  outcomeCoverage: HomeOutcomeCoverageModel;
  recentMoments: HomeRecentMomentModel[];
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

export interface HomeRecentActivityModel {
  dailyLogId: string;
  childId: string;
  logDate: string;
  learningMomentCount: number;
}

export interface HomeTodaySummaryModel {
  dailyLogCount: number;
  learningMomentCount: number;
  childrenObservedCount: number;
}

export interface HomeOutcomeCoverageModel {
  activeOutcomeCount: number;
  touchedThisWeekCount: number;
  untouchedActiveOutcomeCount: number;
}

export interface HomeRecentMomentModel {
  dailyLogId: string;
  learningMomentId: string;
  childId: string;
  childName: string;
  logDate: string;
  kind: string;
  title: string;
  notes: string;
  outcomeNames: string[];
}

export interface GetHomeParams {
  today?: string;
}
