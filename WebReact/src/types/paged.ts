export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageSize: number;
  currentPage: number;
}
