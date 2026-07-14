//#region Server responses

/** Paginated response. */
export interface PaginatedResponse {
  pageNumber: number;
  totalPages: number;
  pageSize: number;
  totalCount: number;
}

/** Generic API wrapper from server, handle a list of responses. */
export interface ApiPaginatedResponse<T> {
  statusCode: number;
  message: string;
  data: {
    items: T[];
    pagination: PaginatedResponse;
  };
}

/** API response wrapper, handles a singular data response. */
export interface ApiSingleResponse<T> {
  statusCode: number;
  message: string;
  data: T;
}
//#endregion

//#region Errors

/** Base Api error model. */
export interface ApiBaseError {
  statusCode: number;
  message?: string;
}

/** Api validatio error and contains validation error information. */
export interface ApiValidationError extends ApiBaseError {
  validationErrors?: string[];
}

/** Api problem response model. All errors different from validation. e.g. 500 */
export interface ApiError extends ApiBaseError {
  details: string;
}

/** Master error api model. */
export type ApiErrorResponse = ApiValidationError | ApiError | null;

//#endregion

/** Final frontend shape based on pagination informaion in the header and the data in the body.
 * Items translate to the "data" coming from ApiPaginatedResponse.
 */
export interface PaginatedResult<T> {
  items: T[];
  pagination: PaginatedResponse;
}

/**
 * Raw TanStack Query cache shape for `useInfiniteQuery` when the queryFn returns `PaginatedResult<T>`.
 * Each entry in `pages` corresponds to one fetched page (one `queryFn` call).
 * Use this as the type argument to `queryClient.getQueriesData` / `setQueryData` in `onMutate`.
 *
 * Note: the `select` transform in the composable maps `items` through a model mapper, so the
 * _selected_ data will have `EducationXxxModel[]` — but the **cache** always holds the raw DTO shape.
 */
export interface InfiniteQueryCache<T> {
  pages: PaginatedResult<T>[];
  pageParams: number[];
}

export interface QueryFilter {
  field: string;
  operator: FilterOperators;
  value: unknown;
}

export type ApiQueryParams = Record<string, unknown>;

/** Queries for all query models. */
export interface BaseQueryParams {
  pageNumber: number;
  pageSize: number;
  sortBy: string | null;
  sortDirection: SortByDirection;
  searchBy: string | null;
  searchTerm: string | null;
  filterMatch: FilterMatch | null;
  filters: QueryFilter[];
  include: string | null;
}

/** Status response codes. */
export const StatusCode = {
  Okay: 200,
  Accepted: 201,
  BadRequest: 400,
  Unauthorized: 401,
  Forbidden: 403,
  NotFound: 404,
  ServerError: 500
};

export type SortByDirection = 'asc' | 'desc' | null;

export type FilterMatch = 'all' | 'any';

export type FilterOperators = StringFilterOperators | DateFilterOperators | NumericFilterOperstors;

export type StringFilterOperators = 'equals' | 'notEquals' | 'contains' | 'notContains' | 'startsWith' | 'endsWith';

export type DateFilterOperators = 'before' | 'after' | 'between';

export type NumericFilterOperstors = 'greaterThan' | 'lessThan' | 'greaterThanOrEqual' | 'lessThanOrEqual';
