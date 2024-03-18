import { _paths } from './paths';

export const _routes = {
  error: {
    _401: `${_paths.error}/${_paths.errorPath._401}`,
    _403: `${_paths.error}/${_paths.errorPath._403}`,
    _404: `${_paths.error}/${_paths.errorPath._404}`,
    _500: `${_paths.error}/${_paths.errorPath._500}`,
    _501: `${_paths.error}/${_paths.errorPath._501}`,
    general: `${_paths.error}/${_paths.errorPath.general}`,
  },
  public: {
      spontaneous: `${_paths.public}/${_paths.publicPath.spontaneous}`,
  },
  security: {
      landing: `${_paths.security}/${_paths.securityPath.landing}`,
      login: `${_paths.security}/${_paths.securityPath.login}`,
      forgotPassword: `${_paths.security}/${_paths.securityPath.forgotPassword}`,
      lockscreen: `${_paths.security}/${_paths.securityPath.lockscreen}`
  },
  clinical: {
      patients: {
          search: `${_paths.clinical}/${_paths.clinicalPath.patients.search}`,
          view(patientId: number): string {
              return `${_paths.clinical}/${_paths.clinicalPath.patients.view.replace(':patientId', patientId.toString())}`;
          }
      },
      encounters: {
          search: `${_paths.clinical}/${_paths.clinicalPath.encounters.search}`,
          view(patientId: number, encounterId: number): string {
              return `${_paths.clinical}/${_paths.clinicalPath.encounters.view.replace(':encounterId', encounterId.toString()).replace(':patientId', patientId.toString())}`;
          }
      },
      cohorts: {
        search: `${_paths.clinical}/${_paths.clinicalPath.cohorts.search}`,
        view(cohortGroupId: number): string {
            return `${_paths.clinical}/${_paths.clinicalPath.cohorts.view.replace(':cohortGroupId', cohortGroupId.toString())}`;
        }
      },      
      forms: {
          landing: `${_paths.clinical}/${_paths.clinicalPath.forms.landing}`,
          formSelect(cohortGroupId: number): string {
            return `${_paths.clinical}/${_paths.clinicalPath.forms.formselect.replace(':cohortGroupId', cohortGroupId.toString())}`;
          },          
          listForm(type: string): string {
            return `${_paths.clinical}/${_paths.clinicalPath.forms.list.replace(':type', type)}`;
          },
          synchroniseForm(type: string): string {
            return `${_paths.clinical}/${_paths.clinicalPath.forms.synchronise.replace(':type', type)}`;
          },
          viewFormDynamic(metaFormId: number, formId: number): string {
            return `${_paths.clinical}/${_paths.clinicalPath.forms.formdynamic.replace(':metaFormId', metaFormId.toString()).replace(':formId', formId.toString())}`;
        }
      }
    },
    analytical: {
      landing: `${_paths.analytical}/${_paths.analyticalPath.landing}`,
      reports: {
        search(workFlowId: string): string {
          return `${_paths.analytical}/${_paths.analyticalPath.reports.search.replace(':wuid', workFlowId)}`;
        },
        searchByQualifiedName(workFlowId: string, qualifiedName: string): string {
          return `${_paths.analytical}/${_paths.analyticalPath.reports.searchqualified.replace(':wuid', workFlowId).replace(':qualifiedName', qualifiedName)}`;
        },
        activity(workFlowId: string, reportInstanceId: number): string {
          return `${_paths.analytical}/${_paths.analyticalPath.reports.activity.replace(':wuid', workFlowId).replace(':reportinstanceid', reportInstanceId.toString())}`;
        },
        clinical(patientId: number, patientClinicalEventId: number): string {
          return `${_paths.analytical}/${_paths.analyticalPath.reports.clinical.replace(':patientId', patientId.toString()).replace(':clinicalEventId', patientClinicalEventId.toString())}`;
        },
        task(workFlowId: string, reportInstanceId: number): string {
          return `${_paths.analytical}/${_paths.analyticalPath.reports.task.replace(':wuid', workFlowId).replace(':reportinstanceid', reportInstanceId.toString())}`;
        }        
      }
    },
    reports: {
      patienttreatment: `${_paths.reports}/${_paths.reportPath.patienttreatment}`
    },
    information: {
      view(id: number): string {
          return `${_paths.information}/${_paths.informationPath.view.replace(':id', id.toString())}`;
      }
    },
    administration: {
      landing: `${_paths.administration}/${_paths.administrationPath.landing}`,
      work: {
        form(formId: number): string {
          return `${_paths.administration}/${_paths.administrationPath.work.form.replace(':formid', formId.toString())}`;
        },         
        dataset: `${_paths.administration}/${_paths.administrationPath.work.dataset}`,
        datasetelementsub(datasetElementId: number): string {
          return `${_paths.administration}/${_paths.administrationPath.work.datasetelementsub.replace(':datasetelementid', datasetElementId.toString())}`;
        },         
        datasetcategoryView(datasetId: number): string {
            return `${_paths.administration}/${_paths.administrationPath.work.datasetcategory.replace(':datasetid', datasetId.toString())}`;
        },
        datasetcategoryElementView(datasetId: number, datasetCategoryId: number): string {
          return `${_paths.administration}/${_paths.administrationPath.work.datasetcategoryelement.replace(':datasetid', datasetId.toString()).replace(':datasetcategoryid', datasetCategoryId.toString())}`;
        }
      }
    }
};