export interface IPageResult<TResult> {
    currentPage: number;
    pageCount: number;
    pageSize: number;
    rowCount: number;
    queryable: TResult[];
}
