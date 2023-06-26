import { Injectable } from '@angular/core';
import { HttpClient } from "@angular/common/http";
import { BaseService } from '../base/base.service';
import { EventService } from './event.service';
import { ParameterKeyValueModel } from '../models/parameter.keyvalue.model';
import { PatientDetailWrapperModel, PatientDetailModel } from '../models/patient/patient.detail.model';
import { PatientExpandedModel } from '../models/patient/patient.expanded.model';
import { AdverseEventReportWrapperModel } from '../models/adverseevent.report.model';
import { AdverseEventFrequencyReportWrapperModel } from '../models/adverseeventfrequency.report.model';
import { PatientConditionDetailModel } from '../models/patient/patient-condition.detail.model';
import { PatientClinicalEventDetailModel } from '../models/patient/patient-clinical-event.detail.model';
import { PatientClinicalEventExpandedModel } from '../models/patient/patient-clinical-event.expanded.model';
import { PatientLabTestDetailModel } from '../models/patient/patient-lab-test.detail.model';
import { PatientMedicationReportWrapperModel } from '../models/patient/patient-medication.report.model';
import { PatientTreatmentReportWrapperModel } from '../models/patient/patient-treatment.report.model';
import { PatientCustomAttributesForUpdateModel } from '../models/patient/patient-custom-attributes-for-update.model';
import { PatientDateOfBirthForUpdateModel } from '../models/patient/patient-date-of-birth-for-update.model';
import { PatientFacilityForUpdateModel } from '../models/patient/patient-facility-for-update.model';
import { PatientNotesForUpdateModel } from '../models/patient/patient-notes-for-update.model';
import { PatientNameForUpdateModel } from '../models/patient/patient-name-for-update.model';
import { PatientForCreationModel } from '../models/patient/patient-for-creation.model';

@Injectable({ providedIn: 'root' })
export class PatientService extends BaseService {

    constructor(
        protected httpClient: HttpClient, protected eventService: EventService) {
        super(httpClient, eventService);
        this.apiController = "/patients";
    }

    searchPatient(filterModel: any): any {
      let parameters: ParameterKeyValueModel[] = [];
      parameters.push(<ParameterKeyValueModel> { key: 'facilityName', value: filterModel.facilityName });
      if (filterModel.patientId != null && filterModel.patientId != '') {
          parameters.push(<ParameterKeyValueModel> { key: 'patientId', value: filterModel.patientId });
      }
      if (filterModel.firstName != null) {
          parameters.push(<ParameterKeyValueModel> { key: 'firstName', value: filterModel.firstName });
      }
      if (filterModel.lastName != null) {
          parameters.push(<ParameterKeyValueModel> { key: 'lastName', value: filterModel.lastName });
      }
      if (filterModel.caseNumber != null) {
        parameters.push(<ParameterKeyValueModel> { key: 'caseNumber', value: filterModel.caseNumber });
      }
      if (filterModel.dateOfBirth != null) {
          parameters.push(<ParameterKeyValueModel> { key: 'dateOfBirth', value: filterModel.dateOfBirth.format("YYYY-MM-DD") });
      }
      if (filterModel.customAttributeId > 0) {
        parameters.push(<ParameterKeyValueModel> { key: 'customAttributeId', value: filterModel.customAttributeId });
        parameters.push(<ParameterKeyValueModel> { key: 'customAttributeValue', value: filterModel.customAttributeValue });
      }
      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

      return this.Get<PatientDetailWrapperModel>('', 'application/vnd.pvims.detail.v1+json', parameters);
    }

    getPatientByCondition(filterModel: any): any {
      let parameters: ParameterKeyValueModel[] = [];

      parameters.push(<ParameterKeyValueModel> { key: 'caseNumber', value: filterModel.caseNumber });
      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: '1'});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: '10'});

      return this.Get<PatientExpandedModel>('', 'application/vnd.pvims.expanded.v1+json', parameters);
    }    

    getPatientExpanded(id: number): any {
        let parameters: ParameterKeyValueModel[] = [];
        parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

        return this.Get<PatientExpandedModel>('', 'application/vnd.pvims.expanded.v1+json', parameters);
    } 

    getPatientDetail(id: number): any {
      let parameters: ParameterKeyValueModel[] = [];
      parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

      return this.Get<PatientDetailModel>('', 'application/vnd.pvims.detail.v1+json', parameters);
    }

    getPatientConditionDetail(patientId: number, id: number): any {
      let parameters: ParameterKeyValueModel[] = [];
      parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

      return this.Get<PatientConditionDetailModel>(`/patients/${patientId}/conditions`, 'application/vnd.pvims.detail.v1+json', parameters);
    } 

    getPatientClinicalEventDetail(patientId: number, id: number): any {
      let parameters: ParameterKeyValueModel[] = [];
      parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

      return this.Get<PatientClinicalEventDetailModel>(`/patients/${patientId}/clinicalevents`, 'application/vnd.pvims.detail.v1+json', parameters);
    }

    getPatientClinicalEventExpanded(patientId: number, id: number): any {
      let parameters: ParameterKeyValueModel[] = [];
      parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

      return this.Get<PatientClinicalEventExpandedModel>(`/patients/${patientId}/clinicalevents`, 'application/vnd.pvims.expanded.v1+json', parameters);
    }

    getPatientMedicationDetail(patientId: number, id: number): any {
      let parameters: ParameterKeyValueModel[] = [];
      parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

      return this.Get<PatientClinicalEventDetailModel>(`/patients/${patientId}/medications`, 'application/vnd.pvims.detail.v1+json', parameters);
    }

    getPatientLabTestDetail(patientId: number, id: number): any {
      let parameters: ParameterKeyValueModel[] = [];
      parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });

      return this.Get<PatientLabTestDetailModel>(`/patients/${patientId}/labtests`, 'application/vnd.pvims.detail.v1+json', parameters);
    }

    getAdverseEventReport(filterModel: any): any {
        let parameters: ParameterKeyValueModel[] = [];

        parameters.push(<ParameterKeyValueModel> { key: 'adverseEventStratifyCriteria', value: filterModel.stratifyId == null ? 1 : filterModel.stratifyId });
        parameters.push(<ParameterKeyValueModel> { key: 'searchFrom', value: filterModel.searchFrom.format("YYYY-MM-DD") });
        parameters.push(<ParameterKeyValueModel> { key: 'searchTo', value: filterModel.searchTo.format("YYYY-MM-DD") });
        parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
        parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});
        parameters.push(<ParameterKeyValueModel> { key: 'ageGroupCriteria', value: filterModel.ageGroupCriteria == null ? 0 : filterModel.ageGroupCriteria });
        parameters.push(<ParameterKeyValueModel> { key: 'genderId', value: filterModel.genderId == null ? '' : filterModel.genderId });
        parameters.push(<ParameterKeyValueModel> { key: 'regimenId', value: filterModel.regimenId == null ? '' : filterModel.regimenId });
        parameters.push(<ParameterKeyValueModel> { key: 'organisationUnitId', value: filterModel.organisationUnitId == null ? 0 : filterModel.organisationUnitId });
        parameters.push(<ParameterKeyValueModel> { key: 'outcomeId', value: filterModel.outcomeId == null ? 0 : filterModel.outcomeId });
        parameters.push(<ParameterKeyValueModel> { key: 'isSeriousId', value: filterModel.isSeriousId == null ? 0 : filterModel.isSeriousId });
        parameters.push(<ParameterKeyValueModel> { key: 'seriousnessId', value: filterModel.seriousnessId == null ? 0 : filterModel.seriousnessId });
        parameters.push(<ParameterKeyValueModel> { key: 'classificationId', value: filterModel.classificationId == null ? 0 : filterModel.classificationId });

        return this.Get<AdverseEventReportWrapperModel>('/patients', 'application/vnd.pvims.adverseventreport.v1+json', parameters);
    }    

    getAdverseEventFrequencyReport(filterModel: any): any {
        let parameters: ParameterKeyValueModel[] = [];

        parameters.push(<ParameterKeyValueModel> { key: 'frequencyCriteria', value: filterModel.criteriaId == null ? 1 : filterModel.criteriaId });
        parameters.push(<ParameterKeyValueModel> { key: 'searchFrom', value: filterModel.searchFrom.format("YYYY-MM-DD") });
        parameters.push(<ParameterKeyValueModel> { key: 'searchTo', value: filterModel.searchTo.format("YYYY-MM-DD") });
        parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
        parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

        return this.Get<AdverseEventFrequencyReportWrapperModel>('/patients', 'application/vnd.pvims.adverseventfrequencyreport.v1+json', parameters);
    }    

    getPatientTreatmentReport(filterModel: any): any {
        let parameters: ParameterKeyValueModel[] = [];

        parameters.push(<ParameterKeyValueModel> { key: 'patientOnStudyCriteria', value: filterModel.criteriaId == null ? 1 : filterModel.criteriaId });
        parameters.push(<ParameterKeyValueModel> { key: 'searchFrom', value: filterModel.searchFrom.format("YYYY-MM-DD") });
        parameters.push(<ParameterKeyValueModel> { key: 'searchTo', value: filterModel.searchTo.format("YYYY-MM-DD") });
        parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
        parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});

        return this.Get<PatientTreatmentReportWrapperModel>('/patients', 'application/vnd.pvims.patienttreatmentreport.v1+json', parameters);
    }

    getPatientMedicationReport(filterModel: any): any {
      let parameters: ParameterKeyValueModel[] = [];

      parameters.push(<ParameterKeyValueModel> { key: 'pageNumber', value: filterModel.currentPage});
      parameters.push(<ParameterKeyValueModel> { key: 'pageSize', value: filterModel.recordsPerPage});
      parameters.push(<ParameterKeyValueModel> { key: 'searchTerm', value: filterModel.conceptName});

      return this.Get<PatientMedicationReportWrapperModel>('/patients', 'application/vnd.pvims.patientmedicationreport.v1+json', parameters);
    }
  
    downloadAttachment(patientId: number, id: number): any {
      let parameters: ParameterKeyValueModel[] = [];
      parameters.push(<ParameterKeyValueModel> { key: 'id', value: id.toString() });
  
      return this.Download(`/patients/${patientId}/attachments`, 'application/vnd.pvims.attachment.v1+xml', parameters);
    }

    downloadAllAttachment(patientId: number): any {
      let parameters: ParameterKeyValueModel[] = [];
  
      return this.Download(`/patients/${patientId}/attachments`, 'application/vnd.pvims.attachment.v1+xml', parameters);
    }

    savePatient(model: PatientForCreationModel): any {
      let shallowModel = this.transformModelForDate(model);
      return this.Post('', shallowModel);
    }

    updatePatientCustomAttributes(id: number, model: PatientCustomAttributesForUpdateModel): any {
      let shallowModel = this.transformModelForDate(model);
      if(id == 0) {
        return null;
      }
      return this.Put(`${id}/custom`, shallowModel);
    }

    updatePatientDateOfBirth(id: number, model: PatientDateOfBirthForUpdateModel): any {
      if(id == 0) {
        return null;
      }
      return this.Put(`${id}/dateofbirth`, model);
    }

    updatePatientFacility(id: number, model: PatientFacilityForUpdateModel): any {
      if(id == 0) {
        return null;
      }
      return this.Put(`${id}/facility`, model);
    }

    updatePatientName(id: number, model: PatientNameForUpdateModel): any {
      if(id == 0) {
        return null;
      }
      return this.Put(`${id}/name`, model);
    }

    updatePatientNotes(id: number, model: PatientNotesForUpdateModel): any {
      if(id == 0) {
        return null;
      }
      return this.Put(`${id}/notes`, model);
    }

    savePatientCondition(patientId: number, id: number, model: any): any {
      let shallowModel = this.transformModelForDate(model);
      if(id == 0) {
        return this.Post(`${patientId}/conditions`, shallowModel);
      }
      else {
        return this.Put(`${patientId}/conditions/${id}`, shallowModel);
      }
    }    

    savePatientClinicalEvent(patientId: number, id: number, model: any): any {
      let shallowModel = this.transformModelForDate(model);
      if(id == 0) {
        return this.Post(`${patientId}/clinicalevents`, shallowModel);
      }
      else {
        return this.Put(`${patientId}/clinicalevents/${id}`, shallowModel);
      }
    }    

    savePatientMedication(patientId: number, id: number, model: any): any {
      let shallowModel = this.transformModelForDate(model);
      if(id == 0) {
        return this.Post(`${patientId}/medications`, shallowModel);
      }
      else {
        return this.Put(`${patientId}/medications/${id}`, shallowModel);
      }
    }

    savePatientLabTest(patientId: number, id: number, model: any): any {
      let shallowModel = this.transformModelForDate(model);
      if(id == 0) {
        return this.Post(`${patientId}/labtests`, shallowModel);
      }
      else {
        return this.Put(`${patientId}/labtests/${id}`, shallowModel);
      }
    }    

    saveAttachment(patientId: number, fileToUpload: File, description: string): any {
      const formData: FormData = new FormData();
      formData.append('description', description);
      formData.append('attachment', fileToUpload, fileToUpload.name);

      return this.PostFile(`${patientId}/attachments`, formData);
    }

    archivePatient(id: number, model: any): any {
      return this.Put(`${id}/archive`, model);
    }

    archiveAttachment(patientId: number, id: number, model: any): any {
      return this.Put(`${patientId}/attachments/${id}/archive`, model);
    }

    archivePatientCondition(patientId: number, id: number, model: any): any {
      return this.Put(`${patientId}/conditions/${id}/archive`, model);
    }

    archivePatientClinicalEvent(patientId: number, id: number, model: any): any {
      return this.Put(`${patientId}/clinicalevents/${id}/archive`, model);
    }

    archivePatientMedication(patientId: number, id: number, model: any): any {
      return this.Put(`${patientId}/medications/${id}/archive`, model);
    }

    archivePatientLabTest(patientId: number, id: number, model: any): any {
      return this.Put(`${patientId}/labtests/${id}/archive`, model);
    }

}
