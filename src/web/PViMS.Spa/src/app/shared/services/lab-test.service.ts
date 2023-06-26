import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';
import { ParameterKeyValueModel } from '../models/parameter.keyvalue.model';
import { LabTestIdentifierWrapperModel, LabTestIdentifierModel } from '../models/labs/lab-test.identifier.model';
import { LabResultIdentifierWrapperModel, LabResultIdentifierModel } from '../models/labs/lab-result.identifier.model';
import { FilterModel } from '../models/grid.model';
import { expand, map, reduce } from 'rxjs/operators';
import { EMPTY } from 'rxjs';
import { LabTestUnitIdentifierWrapperModel } from '../models/labs/lab-test-unit.identifier.model';

@Injectable({ providedIn: 'root' })
export class LabTestService extends BaseService {

  constructor(
      protected httpClient: HttpClient, protected eventService: EventService) {
      super(httpClient, eventService);
      this.apiController = "";
  }

  getAllLabTests(): any {
    // Return all lab tests from the API
    let filter = new FilterModel();
    filter.recordsPerPage = 50;
    filter.currentPage = 1;

    return this.getLabTests(filter)
      .pipe( 
        expand(response => {
          let typedResponse = response as LabTestIdentifierWrapperModel;
          let next = typedResponse.links.find(l => l.rel == 'nextPage');
          return next ? this.GetByAddress<LabTestIdentifierWrapperModel>(next.href, 'application/vnd.pvims.identifier.v1+json') : EMPTY;
        }),
        map(response => {
          let typedResponse = response as LabTestIdentifierWrapperModel;
          return typedResponse.value;
        }),
        reduce((accData, data) => accData.concat(data), [])
      );
  }

  getLabTests(filterModel: any): any {
      let parameters: ParameterKeyValueModel[] = [];

      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});
      if(filterModel.labTestName != undefined && filterModel.labTestName != '') {
        parameters.push(<ParameterKeyValueModel> { key: 'searchTerm', value: filterModel.labTestName});
      }
      if(filterModel.active != undefined) {
        parameters.push(<ParameterKeyValueModel> { key: 'active', value: filterModel.active});
      }

      return this.Get<LabTestIdentifierWrapperModel>('/labtests', 'application/vnd.pvims.identifier.v1+json', parameters);
  }

  getAllLabResults(): any {
    // Return all lab results from the API
    let filter = new FilterModel();
    filter.recordsPerPage = 50;
    filter.currentPage = 1;

    return this.getLabResults(filter)
      .pipe( 
        expand(response => {
          let typedResponse = response as LabResultIdentifierWrapperModel;
          let next = typedResponse.links.find(l => l.rel == 'nextPage');
          return next ? this.GetByAddress<LabResultIdentifierWrapperModel>(next.href, 'application/vnd.pvims.identifier.v1+json') : EMPTY;
        }),
        map(response => {
          let typedResponse = response as LabResultIdentifierWrapperModel;
          return typedResponse.value;
        }),
        reduce((accData, data) => accData.concat(data), [])
      );
  }  

  getLabResults(filterModel: any): any {
    let parameters: ParameterKeyValueModel[] = [];

    parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
    parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});
    if(filterModel.labResultName != undefined && filterModel.labResultName != '') {
      parameters.push(<ParameterKeyValueModel> { key: 'searchTerm', value: filterModel.labResultName});
    }
    if(filterModel.active != undefined) {
      parameters.push(<ParameterKeyValueModel> { key: 'active', value: filterModel.active});
    }

    return this.Get<LabResultIdentifierWrapperModel>('/labresults', 'application/vnd.pvims.identifier.v1+json', parameters);
  }

  getAllLabTestUnits(): any {
    // Return all lab test units from the API
    let filter = new FilterModel();
    filter.recordsPerPage = 50;
    filter.currentPage = 1;

    return this.getLabTestUnits(filter)
      .pipe( 
        expand(response => {
          let typedResponse = response as LabTestUnitIdentifierWrapperModel;
          let next = typedResponse.links.find(l => l.rel == 'nextPage');
          return next ? this.GetByAddress<LabTestUnitIdentifierWrapperModel>(next.href, 'application/vnd.pvims.identifier.v1+json') : EMPTY;
        }),
        map(response => {
          let typedResponse = response as LabTestUnitIdentifierWrapperModel;
          return typedResponse.value;
        }),
        reduce((accData, data) => accData.concat(data), [])
      );
  }

  getLabTestUnits(filterModel: any): any {
      let parameters: ParameterKeyValueModel[] = [];

      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});
      if(filterModel.labTestUnitName != undefined && filterModel.labTestUnitName != '') {
        parameters.push(<ParameterKeyValueModel> { key: 'searchTerm', value: filterModel.labTestUnitName});
      }

      return this.Get<LabTestUnitIdentifierWrapperModel>('/labtestunits', 'application/vnd.pvims.identifier.v1+json', parameters);
  }

  getLabTestIdentifier(id: number): any {
    let parameters: ParameterKeyValueModel[] = [];
    parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

    return this.Get<LabTestIdentifierModel>('/labtests', 'application/vnd.pvims.identifier.v1+json', parameters);
  }

  getLabResultIdentifier(id: number): any {
    let parameters: ParameterKeyValueModel[] = [];
    parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

    return this.Get<LabResultIdentifierModel>('/labresults', 'application/vnd.pvims.identifier.v1+json', parameters);
  }     

  saveLabTest(id: number, model: any): any {
    if(id == 0) {
      return this.Post(`labtests`, model);
    }
    else {
      return this.Put(`labtests/${id}`, model);
    }
  }

  deleteLabTest(id: number): any {
    return this.Delete(`labtests/${id}`);
  }

  saveLabResult(id: number, model: any): any {
    if(id == 0) {
      return this.Post(`labresults`, model);
    }
    else {
      return this.Put(`labresults/${id}`, model);
    }
  }

  deleteLabResult(id: number): any {
    return this.Delete(`labresults/${id}`);
  }    
}
