import { getErrorFeedbackDetail, getErrorFeedbackSummary } from '../api/apiError';

export type AppFeedbackSeverity = 'success' | 'info' | 'warn' | 'error';

export interface AppFeedbackMessage {
  severity: AppFeedbackSeverity;
  summary: string;
  detail?: string;
}

type FeedbackListener = (message: AppFeedbackMessage) => void;

const listeners = new Set<FeedbackListener>();

export const subscribeToFeedback = (listener: FeedbackListener): (() => void) => {
  listeners.add(listener);
  return () => {
    listeners.delete(listener);
  };
};

export const publishFeedback = (message: AppFeedbackMessage): void => {
  for (const listener of listeners) listener(message);
};

export const publishErrorFeedback = (error: unknown): void => {
  publishFeedback({
    severity: 'error',
    summary: getErrorFeedbackSummary(error),
    detail: getErrorFeedbackDetail(error)
  });
};
