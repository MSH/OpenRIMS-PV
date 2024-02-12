import { ElementRef, EventEmitter, Output, Directive } from '@angular/core';
import { MatPaginator } from '@angular/material/paginator';
import { MatSort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { fromEvent } from 'rxjs';
import { takeUntil, debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Directive()
export class GridModel<T> {
    constructor(displayColumns: string[]) {
        this.updateDisplayedColumns(displayColumns);
    }

    private isBasic: boolean = true;
    private filterElement: ElementRef | null;
    private sort: MatSort | null;
    private paginator: MatPaginator | null;

    @Output() filtersChanged = new EventEmitter<void>();

    displayedColumns: string[] = [];
    records: MatTableDataSource<T> = new MatTableDataSource<T>();
    count: number | null;

    setupBasic(filter: ElementRef, matSort: MatSort, matPaginator: MatPaginator): void {
        let self = this;
        self.filterElement = filter;
        self.sort = matSort;
        self.paginator = matPaginator;
        if (self.isBasic) {
            self.updateBasic([]);
            self.records.sort = self.sort;
            self.records.paginator = self.paginator;
            if (self.filterElement) {
                fromEvent(self.filterElement.nativeElement, 'keyup')
                    .pipe(debounceTime(500), distinctUntilChanged())
                    .subscribe(() => { self.records.filter = self.filterElement.nativeElement.value; });
            }
        }
    }

    updateBasic(records: T[]) {
        let self = this;
        self.setDataSource(records);
        self.count = records.length as number;
    }

    setupAdvance(filter: ElementRef, matSort: MatSort, matPaginator: MatPaginator): EventEmitter<void> {
        let self = this;
        self.isBasic = false;
        self.setupBasic(filter, matSort, matPaginator);

        if (self.sort) {
            self.sort.sortChange
                .pipe(distinctUntilChanged())
                .subscribe(() => {
                    self.filtersChanged.emit();
                });
        }

        if (self.paginator) {
            self.paginator.page
                .pipe(distinctUntilChanged())
                .subscribe(() => {
                    self.filtersChanged.emit();
                });
        }

        if (self.filterElement) {
            fromEvent(self.filterElement.nativeElement, 'keyup')
                .pipe(debounceTime(500), distinctUntilChanged())
                .subscribe(() => {
                    self.paginator.pageIndex = 0;
                    self.filtersChanged.emit();
                });
        }

        return self.filtersChanged;
    }

    updateAdvance(result: any) 
    {
        let self = this;
        self.setDataSource(result.value);
        self.count = result.recordCount as number;
    }

    public clearDataSource() {
      let self = this;
      self.records.data = [];
      self.count = null;
    }

    private setDataSource(dataSource: T[]) 
    {
        let self = this;
        if (dataSource) {
            self.records.data = dataSource;
        } else {
            self.records.data = [];
            self.count = null;
        }
    }

    updateDisplayedColumns(displayColumns: string[]) {
        this.displayedColumns = displayColumns;
    }

    filterModel(): FilterModel {
        let self = this;
        let filterModel = new FilterModel();
        if (self.paginator) {
            filterModel.currentPage = self.paginator.pageIndex + 1;
            filterModel.recordsPerPage = self.paginator.pageSize;
        }
        if (self.sort) {
            filterModel.sortKey = self.sort.active;
            filterModel.sortAsc = (self.sort.direction.toString() === 'asc');
        }
        if (self.filterElement) {
            filterModel.searchfor = self.filterElement.nativeElement.value;
        }
        return filterModel;
    }

    customFilterModel(customFilters: any): any {
        let self = this;

        let filterModel = self.filterModel();
        customFilters.currentPage = filterModel.currentPage;
        customFilters.recordsPerPage = filterModel.recordsPerPage;
        customFilters.sortKey = filterModel.sortKey;
        customFilters.sortAsc = filterModel.sortAsc;
        customFilters.searchfor = filterModel.searchfor;

        return customFilters;
    }
}

export class FilterModel {
    currentPage: number = 0;
    recordsPerPage: number = 10;
    sortKey: string = "";
    sortAsc: boolean = true;
    searchfor: string = "";
}