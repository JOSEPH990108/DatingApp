export interface Pagination{
    //variable name must be exactly same as the name from Hearders that passed back from API
    currentPage: number;
    itemsPerPage: number;
    totalItems: number,
    totalPages: number;
}

export class PaginatedResult<T>{
    result?: T;
    pagination?: Pagination;
}
