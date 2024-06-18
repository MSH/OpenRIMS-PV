export const _paths = {
  error: `error`,
  errorPath: {
      _401: `401`,
      _403: `403`,
      _404: `404`,
      _500: `500`,
      _501: `501`,
      general: `general`,
  },
  public: `public`,
  publicPath: {
      spontaneous: `spontaneous`,
  },
  security: `security`,
  securityPath: {
      landing: `landing`,
      login: `login`,
      forgotPassword: `forgot-password`,
      lockscreen: `lockscreen`
  },
  clinical: `clinical`,
  clinicalPath: {
      patients: {
          search: `patientsearch`,
          view: `patientview/:patientId`
      },
      encounters: {
          search: `encountersearch`,
          view: `encounterview/:patientId/:encounterId`
      },
      cohorts: {
          search: `cohortsearch`,
          view: `cohortenrolment/:cohortGroupId`
      },
      forms: {
          formselect: `form-select/:cohortGroupId`,
          list: `form-list/:type`,
          synchronise: `synchronise/:type`,
          landing: `form-select`,
          formdynamic: `form-dynamic/:metaFormId/:formId`,
      }
  },
  analytical: `analytical`,
  analyticalPath: {
    landing: `landing`,
    reports: {
      search: `reportsearch/:wuid`,
      searchqualified: `reportsearch/:wuid/:qualifiedName`,
      activity: `activityhistory/:wuid/:reportinstanceid`,
      clinical: `clinicaldetails/:patientId/:clinicalEventId`,
      task: `reporttask/:wuid/:reportinstanceid`
    }
  },
  reports: `reports`,
  reportPath: {
    patienttreatment: `system/patienttreatment`
  },
  information: `information`,
  informationPath: {
    view: `pageviewer/:id`
  },
  administration: `administration`,
  administrationPath: {
    landing: `landing`,
    work: {
      form: `work/form-view/:formid`,
      dataset: `work/dataset`,
      datasetelementsub: `work/datasetelement/:datasetelementid/datasetelementsub`,
      datasetcategory: `work/datasetcategory/:datasetid`,
      datasetcategoryelement: `work/datasetcategoryelement/:datasetid/:datasetcategoryid`
    }
  }
};
