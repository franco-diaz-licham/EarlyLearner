export interface HomeResponse {
  children: HomeChildResponse[];
  metrics: HomeMetricResponse[];
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

export interface HomeRecentActivityResponse {
  dailyLogId: string;
  childId: string;
  logDate: string;
  learningMomentCount: number;
}

export interface HomeModel {
  children: HomeChildModel[];
  metrics: HomeMetricModel[];
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

export interface HomeRecentActivityModel {
  dailyLogId: string;
  childId: string;
  logDate: string;
  learningMomentCount: number;
}

export interface GetHomeParams {
  today?: string;
}
