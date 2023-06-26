import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';
import { ParameterKeyValueModel } from '../models/parameter.keyvalue.model';
import { FilterModel } from '../models/grid.model';
import { expand, map, reduce } from 'rxjs/operators';
import { EMPTY } from 'rxjs';
import { MeddraTermIdentifierWrapperModel } from '../models/terminology/meddra-term.identifier.model';
import { MeddraTermDetailWrapperModel, MeddraTermDetailModel } from '../models/terminology/meddra-term.detail.model';

@Injectable({ providedIn: 'root' })
export class MeddraTermService extends BaseService {

    constructor(
        protected httpClient: HttpClient, protected eventService: EventService) {
        super(httpClient, eventService);
        this.apiController = "/meddraterms";
    }

    getAllSOCTerms(filterModel: any): any {
      // Return all meddra terms from the API
      filterModel.recordsPerPage = 50;
      filterModel.currentPage = 1;

      return this.searchTerms(filterModel)
        .pipe( 
          expand(response => {
            let typedResponse = response as MeddraTermIdentifierWrapperModel;
            let next = typedResponse.links.find(l => l.rel == 'nextPage');
            return next ? this.GetByAddress<MeddraTermIdentifierWrapperModel>(next.href, 'application/vnd.pvims.identifier.v1+json') : EMPTY;
          }),
          map(response => {
            let typedResponse = response as MeddraTermIdentifierWrapperModel;
            return typedResponse.value;
          }),
          reduce((accData, data) => accData.concat(data), [])
        );
    }

    getCommonTerms(filterModel: any): any {
      let parameters: ParameterKeyValueModel[] = [];

      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

      return this.Get<MeddraTermIdentifierWrapperModel>('', 'application/vnd.pvims.commonmeddra.v1+json', parameters);
    }

    getMeddraTermDetail(id: number): any {
      let parameters: ParameterKeyValueModel[] = [];
      parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

      return this.Get<MeddraTermDetailModel>('', 'application/vnd.pvims.detail.v1+json', parameters);
    }

    searchTerms(filterModel: any): any {
        let parameters: ParameterKeyValueModel[] = [];

        switch (filterModel.searchOption) {
          case "Code":
            parameters.push(<ParameterKeyValueModel> { key: 'searchCode', value: filterModel.searchCode || ''});
            break;

          case "Term":
            parameters.push(<ParameterKeyValueModel> { key: 'termType', value: filterModel.termType || ''});
            parameters.push(<ParameterKeyValueModel> { key: 'searchTerm', value: filterModel.searchTerm || ''});
            break;

          case "List":
            parameters.push(<ParameterKeyValueModel> { key: 'termType', value: 'LLT'});
            parameters.push(<ParameterKeyValueModel> { key: 'parentSearchTerm', value: filterModel.termPT || ''});
            break;
        }        

        parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
        parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});
  
        return this.Get<MeddraTermIdentifierWrapperModel>('', 'application/vnd.pvims.identifier.v1+json', parameters);
    }

    searchTermsByDetail(filterModel: any): any {
      let parameters: ParameterKeyValueModel[] = [];

      parameters.push(<ParameterKeyValueModel> { key: 'termType', value: filterModel.termType || ''});
      parameters.push(<ParameterKeyValueModel> { key: 'searchTerm', value: filterModel.searchTerm || ''});
      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

      return this.Get<MeddraTermDetailWrapperModel>('', 'application/vnd.pvims.detail.v1+json', parameters);
    }

    saveMedDraFile(fileToUpload: File): any {
      const formData: FormData = new FormData();
      formData.append('source', fileToUpload, fileToUpload.name);

      return this.PostFile('', formData);
    }

}
