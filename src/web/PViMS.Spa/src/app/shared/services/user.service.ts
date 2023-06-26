import { Injectable } from '@angular/core';
import { HttpClient, HttpRequest, HttpEvent, HttpHeaders } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';
import { ParameterKeyValueModel } from '../models/parameter.keyvalue.model';
import { UserIdentifierWrapperModel } from '../models/user/user.identifier.model';
import { RoleIdentifierWrapperModel } from '../models/user/role.identifier.model';
import { UserDetailModel } from '../models/user/user.detail.model';
import { AuditLogDetailWrapperModel } from '../models/user/audit-log.detail.model';
import { Observable, EMPTY } from 'rxjs';
import { FilterModel } from '../models/grid.model';
import { expand, map, reduce } from 'rxjs/operators';
import { UserRoleForUpdateModel } from '../models/user/user-role-for-update.model';
import { UserFacilityForUpdateModel } from '../models/user/user-facility-for-update.model';

@Injectable({ providedIn: 'root' })
export class UserService extends BaseService {

  constructor(
      protected httpClient: HttpClient, protected eventService: EventService) {
      super(httpClient, eventService);
      this.apiController = "";
  }

  getAllAuditLogs(filterModel: any): any {
    filterModel.currentPage = 1;
    filterModel.recordsPerPage = 50;
    return this.getAuditLogs(filterModel)
      .pipe( 
        expand(response => {
          let typedResponse = response as AuditLogDetailWrapperModel;
          let next = typedResponse.links.find(l => l.rel == 'nextPage');
          return next ? this.GetByAddress<AuditLogDetailWrapperModel>(next.href, 'application/vnd.pvims.detail.v1+json') : EMPTY;
        }),
        map(response => {
          let typedResponse = response as AuditLogDetailWrapperModel;
          return typedResponse.value;
        }),
        reduce((accData, data) => accData.concat(data), [])
      );
  }  

  getAuditLogs(filterModel: any): any {
    let parameters: ParameterKeyValueModel[] = [];

    parameters.push(<ParameterKeyValueModel> { key: 'searchFrom', value: filterModel.searchFrom.format("YYYY-MM-DD") });
    parameters.push(<ParameterKeyValueModel> { key: 'searchTo', value: filterModel.searchTo.format("YYYY-MM-DD") });
    parameters.push(<ParameterKeyValueModel> { key: 'auditType', value: filterModel.auditType });
    parameters.push(<ParameterKeyValueModel> { key: 'facilityId', value: filterModel.facilityId });
    parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
    parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

    return this.Get<AuditLogDetailWrapperModel>('/auditlogs', 'application/vnd.pvims.detail.v1+json', parameters);
  }

  downloadAuditLogDataset(filterModel: any): any {
    let parameters: ParameterKeyValueModel[] = [];

    parameters.push(<ParameterKeyValueModel> { key: 'searchFrom', value: filterModel.searchFrom.format("YYYY-MM-DD") });
    parameters.push(<ParameterKeyValueModel> { key: 'searchTo', value: filterModel.searchTo.format("YYYY-MM-DD") });
    parameters.push(<ParameterKeyValueModel> { key: 'auditType', value: filterModel.auditType });
    parameters.push(<ParameterKeyValueModel> { key: 'facilityId', value: filterModel.facilityId });
    parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
    parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

    return this.Download('/auditlogs', 'application/vnd.pvims.dataset.v1+json', parameters);
  }

  getRoleList(): any {
    let parameters: ParameterKeyValueModel[] = [];

    parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: '1'});
    parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: '999'});

    return this.Get<RoleIdentifierWrapperModel>('/roles', 'application/vnd.pvims.identifier.v1+json', parameters);
  }

  getAllUsers(): any {
    let filter = new FilterModel();
    filter.recordsPerPage = 50;
    filter.currentPage = 1;

    return this.getUsers(filter)
      .pipe( 
        expand(response => {
          let typedResponse = response as UserIdentifierWrapperModel;
          let next = typedResponse.links.find(l => l.rel == 'nextPage');
          return next ? this.GetByAddress<UserIdentifierWrapperModel>(next.href, 'application/vnd.pvims.identifier.v1+json') : EMPTY;
        }),
        map(response => {
          let typedResponse = response as UserIdentifierWrapperModel;
          return typedResponse.value;
        }),
        reduce((accData, data) => accData.concat(data), [])
      );
  }  

  getUsers(filterModel: any): any {
    let parameters: ParameterKeyValueModel[] = [];

    parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
    parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});
    if(filterModel.userName != undefined) {
      parameters.push(<ParameterKeyValueModel> { key: 'searchTerm', value: filterModel.userName});
    }

    return this.Get<UserIdentifierWrapperModel>('/users', 'application/vnd.pvims.identifier.v1+json', parameters);
  }

  getUserDetail(id: number): any {
    let parameters: ParameterKeyValueModel[] = [];
    parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

    return this.Get<UserDetailModel>('/users', 'application/vnd.pvims.detail.v1+json', parameters);
  }  

  saveUser(id: number, model: any): any {
    if(id == 0) {
      return this.Post(`users`, model);
    }
    else {
      return this.Put(`users/${id}`, model);
    }
  }

  saveUserRole(userId: number, role: any): any {
    const roleForUpdate: UserRoleForUpdateModel = { role };
    return this.Post(`users/${userId}/roles`, roleForUpdate);
  }

  deleteUserRole(userId: number, role: string): any {
    return this.Delete(`users/${userId}/roles/${role}`);
  }

  saveUserFacility(userId: number, facilityId: any): any {
    const facilityForUpdate: UserFacilityForUpdateModel = { facilityId };
    return this.Post(`users/${userId}/facilities`, facilityForUpdate);
  }

  deleteUserFacility(userId: number, facilityId: number): any {
    return this.Delete(`users/${userId}/facilities/${facilityId}`);
  }

  resetPassword(id: number, model: any): any {
    return this.Put(`users/${id}/password`, model);
  }

  deleteUser(id: number): any {
    return this.Delete(`users/${id}`);
  }

  downloadAuditLog(id: number): any {
    let parameters: ParameterKeyValueModel[] = [];
    parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

    return this.Download('/auditlogs', 'application/vnd.pvims.auditlog.v1+xml', parameters);
  }

  acceptEula(id: number): any {
    return this.Put(`users/${id}/accepteula`, null);
  }
}
