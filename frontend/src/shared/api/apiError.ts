import type { AxiosError } from 'axios';
import type { ApiError, ApiErrorResponse, ApiValidationError } from './api.types';

const isObject = (value: unknown): value is Record<string, unknown> => typeof value === 'object' && value !== null;

const hasNumber = <TKey extends string>(value: Record<string, unknown>, key: TKey): value is Record<TKey, number> => typeof value[key] === 'number';

const hasString = <TKey extends string>(value: Record<string, unknown>, key: TKey): value is Record<TKey, string> => typeof value[key] === 'string';

const hasStringArray = <TKey extends string>(value: Record<string, unknown>, key: TKey): value is Record<TKey, string[]> => Array.isArray(value[key]) && value[key].every((item) => typeof item === 'string');

export const isApiValidationError = (error: unknown): error is ApiValidationError => {
  if (!isObject(error)) return false;
  return hasNumber(error, 'statusCode') && (!('validationErrors' in error) || hasStringArray(error, 'validationErrors'));
};

export const isApiError = (error: unknown): error is ApiError => {
  if (!isObject(error)) return false;
  return hasNumber(error, 'statusCode') && hasString(error, 'details');
};

export const isApiErrorResponse = (error: unknown): error is Exclude<ApiErrorResponse, null> => isApiError(error) || isApiValidationError(error);

export const getApiErrorResponse = (error: unknown): ApiErrorResponse => {
  if (isApiErrorResponse(error)) return error;

  const axiosError = error as AxiosError;
  const responseData = axiosError.response?.data;
  if (isApiErrorResponse(responseData)) return responseData;

  return null;
};

export const getErrorStatusCode = (error: unknown): number | undefined => {
  const apiError = getApiErrorResponse(error);
  if (apiError) return apiError.statusCode;

  const axiosError = error as AxiosError;
  return axiosError.response?.status;
};

export const getErrorFeedbackSummary = (error: unknown): string => {
  const apiError = getApiErrorResponse(error);
  return apiError?.message ?? 'Something went wrong';
};

export const getErrorFeedbackDetail = (error: unknown): string => {
  const apiError = getApiErrorResponse(error);

  if (apiError && isApiError(apiError)) return apiError.details;
  if (apiError?.validationErrors?.length) return apiError.validationErrors.join('\n');
  if (apiError?.message) return apiError.message;
  if (error instanceof Error) return error.message;

  return String(error);
};
