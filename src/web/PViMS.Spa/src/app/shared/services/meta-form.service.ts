import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';
import { ParameterKeyValueModel } from '../models/parameter.keyvalue.model';
import { _events } from 'app/config/events';
import { FormWrapperModel } from '../models/form/form.model';
import { MetaFormDetailWrapperModel } from '../models/meta/meta-form.detail.model';
import { IndexedDBService } from './indexed-db.service';
import { Form } from '../indexed-db/appdb';
import { FilterModel } from '../models/grid.model';
import { expand, map, reduce } from 'rxjs/operators';
import { EMPTY } from 'rxjs';
import { FormAttachmentModel } from '../models/form/form-attachment.model';

@Injectable({ providedIn: 'root' })
export class MetaFormService extends BaseService {

    constructor(
        protected httpClient: HttpClient, 
        protected eventService: EventService,
        protected indexdbService: IndexedDBService) 
    {
        super(httpClient, eventService);
        this.apiController = "";
    }

    getAllMetaForms(): any {
      let filter = new FilterModel();
      filter.recordsPerPage = 50;
      filter.currentPage = 1;
  
      return this.getMetaForms(filter)
        .pipe( 
          expand(response => {
            let typedResponse = response as MetaFormDetailWrapperModel;
            let next = typedResponse.links.find(l => l.rel == 'nextPage');
            return next ? this.GetByAddress<MetaFormDetailWrapperModel>(next.href, 'application/vnd.pvims.detail.v1+json') : EMPTY;
          }),
          map(response => {
            let typedResponse = response as MetaFormDetailWrapperModel;
            return typedResponse.value;
          }),
          reduce((accData, data) => accData.concat(data), [])
        );
    }

    getMetaForms(filterModel: any): any {
      let parameters: ParameterKeyValueModel[] = [];
  
      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});
  
      return this.Get<MetaFormDetailWrapperModel>('/metaforms', 'application/vnd.pvims.detail.v1+json', parameters);
    }
  
    checkSynchRequired(): Promise<boolean> {
        this.getSynchStatus().then(result => {
            if(result == undefined) {
                return Promise.resolve(true);
            } else {
                return Promise.resolve(false);
            }
        },
        error => {
            console.log(error);
        });            
        return Promise.resolve(false);
    }

    getFilteredFormsByType(type: string, synchStatus: boolean, compStatus: boolean): Promise<FormWrapperModel> {
        return new Promise((resolve, reject) => {
            this.indexdbService.getFilteredFormsByType(type, synchStatus, compStatus).then(result => {
                let wrapper: FormWrapperModel = {
                    value: result.map(({ id, created, formIdentifier, patientIdentifier, patientName, completeStatus, synchStatus, formType, hasAttachment, hasSecondAttachment }) => ({ id, created, formIdentifier, patientIdentifier, patientName, completeStatus, synchStatus, formType, hasAttachment, hasSecondAttachment })),
                    recordCount: result.length
                  };                

                resolve(wrapper);
            });
        });
    }

    searchFormsByType(type: string, searchTerm: string): Promise<FormWrapperModel> {
      return new Promise((resolve, reject) => {
          this.indexdbService.searchFormsForType(type, searchTerm).then(result => {
              let wrapper: FormWrapperModel = {
                  value: result.map(({ id, created, formIdentifier, patientIdentifier, patientName, completeStatus, synchStatus, formType, hasAttachment, hasSecondAttachment }) => ({ id, created, formIdentifier, patientIdentifier, patientName, completeStatus, synchStatus, formType, hasAttachment, hasSecondAttachment })),
                  recordCount: result.length
                };                

              resolve(wrapper);
          });
      });
    }    

    getAllFormsForType(type: string): Promise<FormWrapperModel> {
      return new Promise((resolve, reject) => {
          this.indexdbService.getAllFormsForType(type).then(result => {
              let wrapper: FormWrapperModel = {
                  value: result.map(({ id, created, formIdentifier, patientIdentifier, patientName, completeStatus, synchStatus, formType, hasAttachment, hasSecondAttachment }) => ({ id, created, formIdentifier, patientIdentifier, patientName, completeStatus, synchStatus, formType, hasAttachment, hasSecondAttachment })),
                  recordCount: result.length
                };                

              resolve(wrapper);
          });
      });
    }

    getForm(id: number): Promise<Form> {
        return new Promise((resolve, reject) => {
            this.indexdbService.getForm(id).then(result => {
                resolve(result);
            });
        });
    }    
    
    saveFormToDatabase(type: string, modelForm: any, patientForm: any, attachments: FormAttachmentModel[], otherModels: any[]): Promise<number> {
        return new Promise((resolve, reject) => {
            this.indexdbService.addNewForm(type, modelForm, patientForm, attachments, otherModels).then(result => {
                resolve(result);
            });
        });
    }

    saveFormToAPI(model: any): any {
        return this.Put(`metaforms`, model);
    }

    updateForm(id: number, modelForm: any, patientForm: any, attachments: FormAttachmentModel[], otherModels: any[]): Promise<boolean> {
        return new Promise((resolve, reject) => {
            this.indexdbService.updateForm(id, modelForm, patientForm, attachments, otherModels).then(result => {
                resolve(true);
            });
        });        
    }

    deleteForm(id: number): Promise<boolean> {
      return new Promise((resolve, reject) => {
          this.indexdbService.deleteForm(id).then(result => {
              resolve(true);
          });
      });        
    }

    markFormAsCompleted(id: number): Promise<boolean> {
      return new Promise((resolve, reject) => {
          this.indexdbService.markFormAsCompleted(id).then(result => {
              resolve(true);
          });
      });        
    }

    markFormAsSynched(id: number): Promise<boolean> {
      return new Promise((resolve, reject) => {
          this.indexdbService.markFormAsSynched(id).then(result => {
              resolve(true);
          });
      });        
    }

    updateAttachment(id: number, imagebin: any, index: number): Promise<boolean> {
      return new Promise((resolve, reject) => {
          this.indexdbService.updateAttachment(id, imagebin, index).then(result => {
              resolve(true);
          });
      });        
    }    

    deleteAttachment(id: number, index: number): Promise<boolean> {
      return new Promise((resolve, reject) => {
          this.indexdbService.deleteAttachment(id, index).then(result => {
              resolve(true);
          });
      });
    }    

    private getSynchStatus(): Promise<unknown>
    {
        return Promise.resolve(true);
    }
}
