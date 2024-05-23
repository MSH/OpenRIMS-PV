using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using OpenRIMS.PV.Main.Core.Aggregates.ContactAggregate;
using OpenRIMS.PV.Main.Core.Aggregates.UserAggregate;
using OpenRIMS.PV.Main.Core.Entities;
using OpenRIMS.PV.Main.Core.ValueTypes;
using OpenRIMS.PV.Main.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Infrastructure
{
    public class MainContextSeed
    {
        public async Task SeedAsync(MainDbContext context, IWebHostEnvironment env, bool seedData, ILogger<MainContextSeed> logger)
        {
            var policy = CreatePolicy(logger, nameof(MainContextSeed));

            await policy.ExecuteAsync(async () =>
            {
                var contentRootPath = env.ContentRootPath;

                if (seedData)
                {
                    using (context)
                    {
                        context.Database.Migrate();

                        context.Users.AddRange(PrepareUsers(context));
                        await context.SaveEntitiesAsync();

                        context.WorkFlows.AddRange(PrepareWorkFlows(context));
                        await context.SaveEntitiesAsync();
                        context.Activities.AddRange(PrepareActivities(context));
                        await context.SaveEntitiesAsync();
                        context.ActivityExecutionStatuses.AddRange(PrepareActivityExecutionStatusesForActiveSurveillance(context));
                        context.ActivityExecutionStatuses.AddRange(PrepareActivityExecutionStatusesForSpontaneousSurveilliance(context));
                        await context.SaveEntitiesAsync();

                        context.AttachmentTypes.AddRange(PrepareAttachmentTypes(context));
                        context.CareEvents.AddRange(PrepareCareEvents(context));
                        context.Configs.AddRange(PrepareConfigs(context));
                        context.ContextTypes.AddRange(PrepareContextTypes(context));
                        context.DatasetElementTypes.AddRange(PrepareDatasetElementTypes(context));
                        context.FacilityTypes.AddRange(PrepareFacilityTypes(context));
                        context.FieldTypes.AddRange(PrepareFieldTypes(context));
                        context.LabResults.AddRange(PrepareLabResults(context));
                        context.LabTestUnits.AddRange(PrepareLabTestUnits(context));
                        context.MetaColumnTypes.AddRange(PrepareMetaColumnTypes(context));
                        context.MetaTableTypes.AddRange(PrepareMetaTableTypes(context));
                        context.MetaWidgetTypes.AddRange(PrepareMetaWidgetTypes(context));
                        context.OrgUnitTypes.AddRange(PrepareOrgUnitTypes(context));
                        context.Outcomes.AddRange(PrepareOutcomes(context));
                        context.PatientStatuses.AddRange(PreparePatientStatus(context));
                        context.Priorities.AddRange(PreparePriorities(context));
                        context.SiteContactDetails.AddRange(PrepareSiteContactDetails(context));
                        context.TreatmentOutcomes.AddRange(PrepareTreatmentOutcomes(context));
                        await context.SaveEntitiesAsync();

                        context.MetaTables.AddRange(PrepareMetaTables(context));
                        await context.SaveEntitiesAsync();

                        // test environment
                        context.EncounterTypes.AddRange(PrepareEncounterTypes(context));
                        context.TerminologyMedDras.AddRange(PrepareMeddraTerms(context));
                        context.MedicationForms.AddRange(PrepareMedicationForms(context));
                        await context.SaveEntitiesAsync();
                        context.Conditions.AddRange(PrepareConditions(context));
                        await context.SaveEntitiesAsync();
                        context.CohortGroups.AddRange(PrepareCohortGroups(context));
                        await context.SaveEntitiesAsync();
                    }
                }
            });
        }

        private static IEnumerable<User> PrepareUsers(MainDbContext context)
        {
            List<User> users = new List<User>();

            if (!context.Users.Any(ce => ce.UserName == "Admin"))
            {
                users.Add(new User("Admin", "Admin", "Admin", "admin@mail.com", Guid.Empty, null));
            }

            return users;
        }

        private static IEnumerable<Activity> PrepareActivities(MainDbContext context)
        {
            List<Activity> activities = new List<Activity>();

            var workFlow = context.WorkFlows.Single(wf => wf.Description == "New Active Surveilliance Report");
            if (!context.Activities.Any(a => a.WorkFlow.Id == workFlow.Id && a.QualifiedName == "Confirm Report Data"))
            {
                activities.Add(new Activity { QualifiedName = "Confirm Report Data", WorkFlow = workFlow, ActivityType = ActivityTypes.UserDrivenActivity });
            }
            if (!context.Activities.Any(a => a.WorkFlow.Id == workFlow.Id && a.QualifiedName == "Set MedDRA and Causality"))
            {
                activities.Add(new Activity { QualifiedName = "Set MedDRA and Causality", WorkFlow = workFlow, ActivityType = ActivityTypes.UserDrivenActivity });
            }
            if (!context.Activities.Any(a => a.WorkFlow.Id == workFlow.Id && a.QualifiedName == "Extract E2B"))
            {
                activities.Add(new Activity { QualifiedName = "Extract E2B", WorkFlow = workFlow, ActivityType = ActivityTypes.UserDrivenActivity });
            }

            var spontaneousWorkFlow = context.WorkFlows.Single(wf => wf.Description == "New Spontaneous Surveilliance Report");
            if (!context.Activities.Any(a => a.WorkFlow.Id == spontaneousWorkFlow.Id && a.QualifiedName == "Confirm Report Data"))
            {
                activities.Add(new Activity { QualifiedName = "Confirm Report Data", WorkFlow = spontaneousWorkFlow, ActivityType = ActivityTypes.UserDrivenActivity });
            }
            if (!context.Activities.Any(a => a.WorkFlow.Id == spontaneousWorkFlow.Id && a.QualifiedName == "Set MedDRA and Causality"))
            {
                activities.Add(new Activity { QualifiedName = "Set MedDRA and Causality", WorkFlow = spontaneousWorkFlow, ActivityType = ActivityTypes.UserDrivenActivity });
            }
            if (!context.Activities.Any(a => a.WorkFlow.Id == spontaneousWorkFlow.Id && a.QualifiedName == "Extract E2B"))
            {
                activities.Add(new Activity { QualifiedName = "Extract E2B", WorkFlow = spontaneousWorkFlow, ActivityType = ActivityTypes.UserDrivenActivity });
            }

            return activities;
        }

        private static IEnumerable<ActivityExecutionStatus> PrepareActivityExecutionStatusesForActiveSurveillance(MainDbContext context)
        {
            List<ActivityExecutionStatus> activityExecutionStatuses = new List<ActivityExecutionStatus>();

            var activity = context.Activities.Single(a => a.WorkFlow.Description == "New Active Surveilliance Report" && a.QualifiedName == "Confirm Report Data");
            if (!context.ActivityExecutionStatuses.Any(a => a.Activity.WorkFlow.Description == "New Active Surveilliance Report" && a.Activity.QualifiedName == "Confirm Report Data" && a.Description == "UNCONFIRMED"))
            {
                activityExecutionStatuses.Add(new ActivityExecutionStatus { Description = "UNCONFIRMED", Activity = activity, FriendlyDescription = "Report submitted for confirmation" });
            }
            if (!context.ActivityExecutionStatuses.Any(a => a.Activity.WorkFlow.Description == "New Active Surveilliance Report" && a.Activity.QualifiedName == "Confirm Report Data" && a.Description == "CONFIRMED"))
            {
                activityExecutionStatuses.Add(new ActivityExecutionStatus { Description = "CONFIRMED", Activity = activity, FriendlyDescription = "Report confirmed by technician" });
            }
            if (!context.ActivityExecutionStatuses.Any(a => a.Activity.WorkFlow.Description == "New Active Surveilliance Report" && a.Activity.QualifiedName == "Confirm Report Data" && a.Description == "DELETED"))
            {
                activityExecutionStatuses.Add(new ActivityExecutionStatus { Description = "DELETED", Activity = activity, FriendlyDescription = "Report deleted by technician" });
            }

            var meddraActivity = context.Activities.Single(a => a.WorkFlow.Description == "New Active Surveilliance Report" && a.QualifiedName == "Set MedDRA and Causality");
            if (!context.ActivityExecutionStatuses.Any(a => a.Activity.WorkFlow.Description == "New Active Surveilliance Report" && a.Activity.QualifiedName == "Set MedDRA and Causality" && a.Description == "NOTSET"))
            {
                activityExecutionStatuses.Add(new ActivityExecutionStatus { Description = "NOTSET", Activity = meddraActivity, FriendlyDescription = "Report ready for MedDRA and Causality" });
            }
            if (!context.ActivityExecutionStatuses.Any(a => a.Activity.WorkFlow.Description == "New Active Surveilliance Report" && a.Activity.QualifiedName == "Set MedDRA and Causality" && a.Description == "MEDDRASET"))
            {
                activityExecutionStatuses.Add(new ActivityExecutionStatus { Description = "MEDDRASET", Activity = meddraActivity, FriendlyDescription = "MedDRA term set by technician" });
            }
            if (!context.ActivityExecutionStatuses.Any(a => a.Activity.WorkFlow.Description == "New Active Surveilliance Report" && a.Activity.QualifiedName == "Set MedDRA and Causality" && a.Description == "CLASSIFICATIONSET"))
            {
                activityExecutionStatuses.Add(new ActivityExecutionStatus { Description = "CLASSIFICATIONSET", Activity = meddraActivity, FriendlyDescription = "Report classified by technician" });
            }
            if (!context.ActivityExecutionStatuses.Any(a => a.Activity.WorkFlow.Description == "New Active Surveilliance Report" && a.Activity.QualifiedName == "Set MedDRA and Causality" && a.Description == "CAUSALITYSET"))
            {
                activityExecutionStatuses.Add(new ActivityExecutionStatus { Description = "CAUSALITYSET", Activity = meddraActivity, FriendlyDescription = "Causality set by technician" });
            }
            if (!context.ActivityExecutionStatuses.Any(a => a.Activity.WorkFlow.Description == "New Active Surveilliance Report" && a.Activity.QualifiedName == "Set MedDRA and Causality" && a.Description == "CAUSALITYCONFIRMED"))
            {
                activityExecutionStatuses.Add(new ActivityExecutionStatus { Description = "CAUSALITYCONFIRMED", Activity = meddraActivity, FriendlyDescription = "Causality confirmed by technician" });
            }

            var e2bActivity = context.Activities.Single(a => a.WorkFlow.Description == "New Active Surveilliance Report" && a.QualifiedName == "Extract E2B");
            if (!context.ActivityExecutionStatuses.Any(a => a.Activity.WorkFlow.Description == "New Active Surveilliance Report" && a.Activity.QualifiedName == "Extract E2B" && a.Description == "NOTGENERATED"))
            {
                activityExecutionStatuses.Add(new ActivityExecutionStatus { Description = "NOTGENERATED", Activity = e2bActivity, FriendlyDescription = "Report ready for E2B submission" });
            }
            if (!context.ActivityExecutionStatuses.Any(a => a.Activity.WorkFlow.Description == "New Active Surveilliance Report" && a.Activity.QualifiedName == "Extract E2B" && a.Description == "E2BINITIATED"))
            {
                activityExecutionStatuses.Add(new ActivityExecutionStatus { Description = "E2BINITIATED", Activity = e2bActivity, FriendlyDescription = "E2B data generated for report" });
            }
            if (!context.ActivityExecutionStatuses.Any(a => a.Activity.WorkFlow.Description == "New Active Surveilliance Report" && a.Activity.QualifiedName == "Extract E2B" && a.Description == "E2BGENERATED"))
            {
                activityExecutionStatuses.Add(new ActivityExecutionStatus { Description = "E2BGENERATED", Activity = e2bActivity, FriendlyDescription = "E2B report generated" });
            }
            if (!context.ActivityExecutionStatuses.Any(a => a.Activity.WorkFlow.Description == "New Active Surveilliance Report" && a.Activity.QualifiedName == "Extract E2B" && a.Description == "E2BSUBMITTED"))
            {
                activityExecutionStatuses.Add(new ActivityExecutionStatus { Description = "E2BSUBMITTED", Activity = e2bActivity, FriendlyDescription = "E2B report submitted" });
            }

            return activityExecutionStatuses;
        }

        private static IEnumerable<ActivityExecutionStatus> PrepareActivityExecutionStatusesForSpontaneousSurveilliance(MainDbContext context)
        {
            List<ActivityExecutionStatus> activityExecutionStatuses = new List<ActivityExecutionStatus>();

            var activity = context.Activities.Single(a => a.WorkFlow.Description == "New Spontaneous Surveilliance Report" && a.QualifiedName == "Confirm Report Data");
            if (!context.ActivityExecutionStatuses.Any(a => a.Activity.WorkFlow.Description == "New Spontaneous Surveilliance Report" && a.Activity.QualifiedName == "Confirm Report Data" && a.Description == "UNCONFIRMED"))
            {
                activityExecutionStatuses.Add(new ActivityExecutionStatus { Description = "UNCONFIRMED", Activity = activity, FriendlyDescription = "Report submitted for confirmation" });
            }
            if (!context.ActivityExecutionStatuses.Any(a => a.Activity.WorkFlow.Description == "New Spontaneous Surveilliance Report" && a.Activity.QualifiedName == "Confirm Report Data" && a.Description == "CONFIRMED"))
            {
                activityExecutionStatuses.Add(new ActivityExecutionStatus { Description = "CONFIRMED", Activity = activity, FriendlyDescription = "Report confirmed by technician" });
            }
            if (!context.ActivityExecutionStatuses.Any(a => a.Activity.WorkFlow.Description == "New Spontaneous Surveilliance Report" && a.Activity.QualifiedName == "Confirm Report Data" && a.Description == "DELETED"))
            {
                activityExecutionStatuses.Add(new ActivityExecutionStatus { Description = "DELETED", Activity = activity, FriendlyDescription = "Report deleted by technician" });
            }

            var meddraActivity = context.Activities.Single(a => a.WorkFlow.Description == "New Spontaneous Surveilliance Report" && a.QualifiedName == "Set MedDRA and Causality");
            if (!context.ActivityExecutionStatuses.Any(a => a.Activity.WorkFlow.Description == "New Spontaneous Surveilliance Report" && a.Activity.QualifiedName == "Set MedDRA and Causality" && a.Description == "NOTSET"))
            {
                activityExecutionStatuses.Add(new ActivityExecutionStatus { Description = "NOTSET", Activity = meddraActivity, FriendlyDescription = "Report ready for MedDRA and Causality" });
            }
            if (!context.ActivityExecutionStatuses.Any(a => a.Activity.WorkFlow.Description == "New Spontaneous Surveilliance Report" && a.Activity.QualifiedName == "Set MedDRA and Causality" && a.Description == "MEDDRASET"))
            {
                activityExecutionStatuses.Add(new ActivityExecutionStatus { Description = "MEDDRASET", Activity = meddraActivity, FriendlyDescription = "MedDRA term set by technician" });
            }
            if (!context.ActivityExecutionStatuses.Any(a => a.Activity.WorkFlow.Description == "New Spontaneous Surveilliance Report" && a.Activity.QualifiedName == "Set MedDRA and Causality" && a.Description == "CLASSIFICATIONSET"))
            {
                activityExecutionStatuses.Add(new ActivityExecutionStatus { Description = "CLASSIFICATIONSET", Activity = meddraActivity, FriendlyDescription = "Report classified by technician" });
            }
            if (!context.ActivityExecutionStatuses.Any(a => a.Activity.WorkFlow.Description == "New Spontaneous Surveilliance Report" && a.Activity.QualifiedName == "Set MedDRA and Causality" && a.Description == "CAUSALITYSET"))
            {
                activityExecutionStatuses.Add(new ActivityExecutionStatus { Description = "CAUSALITYSET", Activity = meddraActivity, FriendlyDescription = "Causality set by technician" });
            }
            if (!context.ActivityExecutionStatuses.Any(a => a.Activity.WorkFlow.Description == "New Spontaneous Surveilliance Report" && a.Activity.QualifiedName == "Set MedDRA and Causality" && a.Description == "CAUSALITYCONFIRMED"))
            {
                activityExecutionStatuses.Add(new ActivityExecutionStatus { Description = "CAUSALITYCONFIRMED", Activity = meddraActivity, FriendlyDescription = "Causality confirmed by technician" });
            }

            var e2bActivity = context.Activities.Single(a => a.WorkFlow.Description == "New Spontaneous Surveilliance Report" && a.QualifiedName == "Extract E2B");
            if (!context.ActivityExecutionStatuses.Any(a => a.Activity.WorkFlow.Description == "New Spontaneous Surveilliance Report" && a.Activity.QualifiedName == "Extract E2B" && a.Description == "NOTGENERATED"))
            {
                activityExecutionStatuses.Add(new ActivityExecutionStatus { Description = "NOTGENERATED", Activity = e2bActivity, FriendlyDescription = "Report ready for E2B submission" });
            }
            if (!context.ActivityExecutionStatuses.Any(a => a.Activity.WorkFlow.Description == "New Spontaneous Surveilliance Report" && a.Activity.QualifiedName == "Extract E2B" && a.Description == "E2BINITIATED"))
            {
                activityExecutionStatuses.Add(new ActivityExecutionStatus { Description = "E2BINITIATED", Activity = e2bActivity, FriendlyDescription = "E2B data generated for report" });
            }
            if (!context.ActivityExecutionStatuses.Any(a => a.Activity.WorkFlow.Description == "New Spontaneous Surveilliance Report" && a.Activity.QualifiedName == "Extract E2B" && a.Description == "E2BGENERATED"))
            {
                activityExecutionStatuses.Add(new ActivityExecutionStatus { Description = "E2BGENERATED", Activity = e2bActivity, FriendlyDescription = "E2B report generated" });
            }
            if (!context.ActivityExecutionStatuses.Any(a => a.Activity.WorkFlow.Description == "New Spontaneous Surveilliance Report" && a.Activity.QualifiedName == "Extract E2B" && a.Description == "E2BSUBMITTED"))
            {
                activityExecutionStatuses.Add(new ActivityExecutionStatus { Description = "E2BSUBMITTED", Activity = e2bActivity, FriendlyDescription = "E2B report submitted" });
            }

            return activityExecutionStatuses;
        }

        private static IEnumerable<AttachmentType> PrepareAttachmentTypes(MainDbContext context)
        {
            List<AttachmentType> attachmentTypes = new List<AttachmentType>();

            if (!context.AttachmentTypes.Any(ce => ce.Key == "doc"))
                attachmentTypes.Add(new AttachmentType("MS Word 2003-2007 Document", "doc"));

            if (!context.AttachmentTypes.Any(ce => ce.Key == "xls"))
                attachmentTypes.Add(new AttachmentType("MS Excel 2003-2007 Document", "xls"));

            if (!context.AttachmentTypes.Any(ce => ce.Key == "docx"))
                attachmentTypes.Add(new AttachmentType("MS Word Document", "docx"));

            if (!context.AttachmentTypes.Any(ce => ce.Key == "xlsx"))
                attachmentTypes.Add(new AttachmentType("MS Excel Document", "xlsx"));

            if (!context.AttachmentTypes.Any(ce => ce.Key == "pdf"))
                attachmentTypes.Add(new AttachmentType("Portable Document Format", "pdf"));

            if (!context.AttachmentTypes.Any(ce => ce.Key == "jpg"))
                attachmentTypes.Add(new AttachmentType("Image | JPEG", "jpg"));

            if (!context.AttachmentTypes.Any(ce => ce.Key == "jpeg"))
                attachmentTypes.Add(new AttachmentType("Image | JPEG", "jpeg"));

            if (!context.AttachmentTypes.Any(ce => ce.Key == "png"))
                attachmentTypes.Add(new AttachmentType("Image | PNG", "png"));

            if (!context.AttachmentTypes.Any(ce => ce.Key == "bmp"))
                attachmentTypes.Add(new AttachmentType("Image | BMP", "bmp"));

            if (!context.AttachmentTypes.Any(ce => ce.Key == "xml"))
                attachmentTypes.Add(new AttachmentType("XML Document", "xml"));

            return attachmentTypes;
        }

        private static IEnumerable<CareEvent> PrepareCareEvents(MainDbContext context)
        {
            List<CareEvent> careEvents = new List<CareEvent>();

            if (!context.CareEvents.Any(ce => ce.Description == "Capture Vitals"))
                careEvents.Add(new CareEvent { Description = "Capture Vitals" });

            if (!context.CareEvents.Any(ce => ce.Description == "Doctor Assessment"))
                careEvents.Add(new CareEvent { Description = "Doctor Assessment" });

            if (!context.CareEvents.Any(ce => ce.Description == "Nurse Assessment"))
                careEvents.Add(new CareEvent { Description = "Nurse Assessment" });

            return careEvents;
        }

        private static IEnumerable<CohortGroup> PrepareCohortGroups(MainDbContext context)
        {
            List<CohortGroup> cohorts = new List<CohortGroup>();

            if (!context.CohortGroups.Any(ce => ce.CohortName == "Pyramax"))
            {
                var condition = context.Conditions.Single(c => c.Description == "TB");
                cohorts.Add(new CohortGroup("Pyramax", "PYR", condition, DateTime.Now.Date, null));
            }

            return cohorts;
        }

        private static IEnumerable<Condition> PrepareConditions(MainDbContext context)
        {
            List<Condition> conditions = new List<Condition>();

            if (!context.Conditions.Any(ce => ce.Description == "TB"))
            {
                var term = context.TerminologyMedDras.Single(t => t.MedDraTerm == "Tuberculosis");
                var condition = new Condition { Description = "TB", Chronic = true, Active = true };
                condition.ConditionMedDras.Add(new ConditionMedDra { TerminologyMedDra = term });
                conditions.Add(condition);
            }

            return conditions;
        }

        private static IEnumerable<Config> PrepareConfigs(MainDbContext context)
        {
            List<Config> configs = new List<Config>();

            if (!context.Configs.Any(ce => ce.ConfigType == ConfigType.E2BVersion))
            {
                var e2bConfig = new Config { ConfigType = ConfigType.E2BVersion, ConfigValue = "E2B(R2) ICH Report" };
                e2bConfig.AuditStamp(context.Users.Single(u => u.UserName == "Admin"));
                configs.Add(e2bConfig);
            }

            if (!context.Configs.Any(ce => ce.ConfigType == ConfigType.WebServiceSubscriberList))
            {
                var subscriberConfig = new Config { ConfigType = ConfigType.WebServiceSubscriberList, ConfigValue = "NOT SPECIFIED" };
                subscriberConfig.AuditStamp(context.Users.Single(u => u.UserName == "Admin"));
                configs.Add(subscriberConfig);
            }

            if (!context.Configs.Any(ce => ce.ConfigType == ConfigType.AssessmentScale))
            {
                var assessmentConfig = new Config { ConfigType = ConfigType.AssessmentScale, ConfigValue = "Both Scales" };
                assessmentConfig.AuditStamp(context.Users.Single(u => u.UserName == "Admin"));
                configs.Add(assessmentConfig);
            }

            if (!context.Configs.Any(ce => ce.ConfigType == ConfigType.MedDRAVersion))
            {
                var meddraConfig = new Config { ConfigType = ConfigType.MedDRAVersion, ConfigValue = "23.0" };
                meddraConfig.AuditStamp(context.Users.Single(u => u.UserName == "Admin"));
                configs.Add(meddraConfig);
            }

            if (!context.Configs.Any(ce => ce.ConfigType == ConfigType.ReportInstanceNewAlertCount))
            {
                var instanceConfig = new Config { ConfigType = ConfigType.ReportInstanceNewAlertCount, ConfigValue = "0" };
                instanceConfig.AuditStamp(context.Users.Single(u => u.UserName == "Admin"));
                configs.Add(instanceConfig);
            }

            if (!context.Configs.Any(ce => ce.ConfigType == ConfigType.MedicationOnsetCheckPeriodWeeks))
            {
                var onsetConfig = new Config { ConfigType = ConfigType.MedicationOnsetCheckPeriodWeeks, ConfigValue = "5" };
                onsetConfig.AuditStamp(context.Users.Single(u => u.UserName == "Admin"));
                configs.Add(onsetConfig);
            }

            if (!context.Configs.Any(ce => ce.ConfigType == ConfigType.MetaDataLastUpdated))
            {
                var metaConfig = new Config { ConfigType = ConfigType.MetaDataLastUpdated, ConfigValue = DateTime.Now.ToString("yyyy-MM-dd HH:mm") };
                metaConfig.AuditStamp(context.Users.Single(u => u.UserName == "Admin"));
                configs.Add(metaConfig);
            }

            if (!context.Configs.Any(ce => ce.ConfigType == ConfigType.PharmadexLink))
            {
                var siteConfig = new Config { ConfigType = ConfigType.PharmadexLink, ConfigValue = "NOT SPECIFIED" };
                siteConfig.AuditStamp(context.Users.Single(u => u.UserName == "Admin"));
                configs.Add(siteConfig);
            }

            if (!context.Configs.Any(ce => ce.ConfigType == ConfigType.ReportInstanceFeedbackAlertCount))
            {
                var instanceConfig = new Config { ConfigType = ConfigType.ReportInstanceFeedbackAlertCount, ConfigValue = "0" };
                instanceConfig.AuditStamp(context.Users.Single(u => u.UserName == "Admin"));
                configs.Add(instanceConfig);
            }

            return configs;
        }

        private static IEnumerable<ContextType> PrepareContextTypes(MainDbContext context)
        {
            List<ContextType> contextTypes = new List<ContextType>();

            if (!context.ContextTypes.Any(ce => ce.Description == "Encounter"))
                contextTypes.Add(new ContextType { Description = "Encounter" });

            if (!context.ContextTypes.Any(ce => ce.Description == "Patient"))
                contextTypes.Add(new ContextType { Description = "Patient" });

            if (!context.ContextTypes.Any(ce => ce.Description == "Pregnancy"))
                contextTypes.Add(new ContextType { Description = "Pregnancy" });

            if (!context.ContextTypes.Any(ce => ce.Description == "Global"))
                contextTypes.Add(new ContextType { Description = "Global" });

            if (!context.ContextTypes.Any(ce => ce.Description == "PatientClinicalEvent"))
                contextTypes.Add(new ContextType { Description = "PatientClinicalEvent" });

            if (!context.ContextTypes.Any(ce => ce.Description == "DatasetInstance"))
                contextTypes.Add(new ContextType { Description = "DatasetInstance" });

            return contextTypes;
        }

        private static IEnumerable<DatasetElementType> PrepareDatasetElementTypes(MainDbContext context)
        {
            List<DatasetElementType> datasetElementTypes = new List<DatasetElementType>();

            if (!context.DatasetElementTypes.Any(ce => ce.Description == "Generic"))
                datasetElementTypes.Add(new DatasetElementType { Description = "Generic" });

            return datasetElementTypes;
        }

        private static IEnumerable<EncounterType> PrepareEncounterTypes(MainDbContext context)
        {
            List<EncounterType> encounterTypes = new List<EncounterType>();

            if (!context.EncounterTypes.Any(ce => ce.Description == "Pre-Treatment Visit"))
                encounterTypes.Add(new EncounterType { Description = "Pre-Treatment Visit" });

            if (!context.EncounterTypes.Any(ce => ce.Description == "Treatment Initiation Visit"))
                encounterTypes.Add(new EncounterType { Description = "Treatment Initiation Visit" });

            if (!context.EncounterTypes.Any(ce => ce.Description == "Scheduled Follow-Up Visit"))
                encounterTypes.Add(new EncounterType { Description = "Scheduled Follow-Up Visit" });

            if (!context.EncounterTypes.Any(ce => ce.Description == "Unscheduled Visit"))
                encounterTypes.Add(new EncounterType { Description = "Unscheduled Visit" });

            return encounterTypes;
        }

        private static IEnumerable<FacilityType> PrepareFacilityTypes(MainDbContext context)
        {
            List<FacilityType> facilityTypes = new List<FacilityType>();

            if (!context.FacilityTypes.Any(ce => ce.Description == "Unknown"))
                facilityTypes.Add(new FacilityType { Description = "Unknown" });

            if (!context.FacilityTypes.Any(ce => ce.Description == "Hospital"))
                facilityTypes.Add(new FacilityType { Description = "Hospital" });

            if (!context.FacilityTypes.Any(ce => ce.Description == "CHC"))
                facilityTypes.Add(new FacilityType { Description = "CHC" });

            if (!context.FacilityTypes.Any(ce => ce.Description == "PHC"))
                facilityTypes.Add(new FacilityType { Description = "PHC" });

            return facilityTypes;
        }

        private static IEnumerable<FieldType> PrepareFieldTypes(MainDbContext context)
        {
            List<FieldType> fieldTypes = new List<FieldType>();

            if (!context.FieldTypes.Any(ce => ce.Description == "Listbox"))
                fieldTypes.Add(new FieldType { Description = "Listbox" });

            if (!context.FieldTypes.Any(ce => ce.Description == "DropDownList"))
                fieldTypes.Add(new FieldType { Description = "DropDownList" });

            if (!context.FieldTypes.Any(ce => ce.Description == "AlphaNumericTextbox"))
                fieldTypes.Add(new FieldType { Description = "AlphaNumericTextbox" });

            if (!context.FieldTypes.Any(ce => ce.Description == "NumericTextbox"))
                fieldTypes.Add(new FieldType { Description = "NumericTextbox" });

            if (!context.FieldTypes.Any(ce => ce.Description == "YesNo"))
                fieldTypes.Add(new FieldType { Description = "YesNo" });

            if (!context.FieldTypes.Any(ce => ce.Description == "Date"))
                fieldTypes.Add(new FieldType { Description = "Date" });

            if (!context.FieldTypes.Any(ce => ce.Description == "Table"))
                fieldTypes.Add(new FieldType { Description = "Table" });

            if (!context.FieldTypes.Any(ce => ce.Description == "System"))
                fieldTypes.Add(new FieldType { Description = "System" });

            return fieldTypes;
        }

        private static IEnumerable<LabResult> PrepareLabResults(MainDbContext context)
        {
            List<LabResult> labResults = new List<LabResult>();

            if (!context.LabResults.Any(ce => ce.Description == "Positive"))
                labResults.Add(new LabResult { Description = "Positive" });

            if (!context.LabResults.Any(ce => ce.Description == "Negative"))
                labResults.Add(new LabResult { Description = "Negative" });

            if (!context.LabResults.Any(ce => ce.Description == "Borderline"))
                labResults.Add(new LabResult { Description = "Borderline" });

            if (!context.LabResults.Any(ce => ce.Description == "Inconclusive"))
                labResults.Add(new LabResult { Description = "Inconclusive" });

            if (!context.LabResults.Any(ce => ce.Description == "Normal"))
                labResults.Add(new LabResult { Description = "Normal" });

            if (!context.LabResults.Any(ce => ce.Description == "Abnormal"))
                labResults.Add(new LabResult { Description = "Abnormal" });

            if (!context.LabResults.Any(ce => ce.Description == "Seronegative"))
                labResults.Add(new LabResult { Description = "Seronegative" });

            if (!context.LabResults.Any(ce => ce.Description == "Seropositive"))
                labResults.Add(new LabResult { Description = "Seropositive" });

            if (!context.LabResults.Any(ce => ce.Description == "Improved"))
                labResults.Add(new LabResult { Description = "Improved" });

            if (!context.LabResults.Any(ce => ce.Description == "Stable"))
                labResults.Add(new LabResult { Description = "Stable" });

            if (!context.LabResults.Any(ce => ce.Description == "Progressed"))
                labResults.Add(new LabResult { Description = "Progressed" });

            return labResults;
        }

        private static IEnumerable<LabTestUnit> PrepareLabTestUnits(MainDbContext context)
        {
            List<LabTestUnit> labTestUnits = new List<LabTestUnit>();

            if (!context.LabTestUnits.Any(ce => ce.Description == "N/A"))
                labTestUnits.Add(new LabTestUnit { Description = "N/A" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "%"))
                labTestUnits.Add(new LabTestUnit { Description = "%" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "% hearing loss left ear"))
                labTestUnits.Add(new LabTestUnit { Description = "% hearing loss left ear" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "% hearing loss right ear"))
                labTestUnits.Add(new LabTestUnit { Description = "% hearing loss right ear" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "µg/dL"))
                labTestUnits.Add(new LabTestUnit { Description = "µg/dL" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "µg/L"))
                labTestUnits.Add(new LabTestUnit { Description = "µg/L" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "Beats per minute"))
                labTestUnits.Add(new LabTestUnit { Description = "Beats per minute" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "Breaths per minute"))
                labTestUnits.Add(new LabTestUnit { Description = "Breaths per minute" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "Cavities"))
                labTestUnits.Add(new LabTestUnit { Description = "Cavities" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "cells/mm 3"))
                labTestUnits.Add(new LabTestUnit { Description = "cells/mm 3" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "g/dL"))
                labTestUnits.Add(new LabTestUnit { Description = "g/dL" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "g/L"))
                labTestUnits.Add(new LabTestUnit { Description = "g/L" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "IU/L"))
                labTestUnits.Add(new LabTestUnit { Description = "IU/L" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "kg/m 2"))
                labTestUnits.Add(new LabTestUnit { Description = "kg/m 2" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "mEq/L"))
                labTestUnits.Add(new LabTestUnit { Description = "mEq/L" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "mg/24 hr"))
                labTestUnits.Add(new LabTestUnit { Description = "mg/24 hr" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "mg/dL"))
                labTestUnits.Add(new LabTestUnit { Description = "mg/dL" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "min"))
                labTestUnits.Add(new LabTestUnit { Description = "min" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "mL/min"))
                labTestUnits.Add(new LabTestUnit { Description = "mL/min" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "mm Hg"))
                labTestUnits.Add(new LabTestUnit { Description = "mm Hg" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "mm/h"))
                labTestUnits.Add(new LabTestUnit { Description = "mm/h" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "mmol/kg"))
                labTestUnits.Add(new LabTestUnit { Description = "mmol/kg" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "mmol/L"))
                labTestUnits.Add(new LabTestUnit { Description = "mmol/L" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "mOsm/kg"))
                labTestUnits.Add(new LabTestUnit { Description = "mOsm/kg" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "ms"))
                labTestUnits.Add(new LabTestUnit { Description = "ms" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "ng/dL"))
                labTestUnits.Add(new LabTestUnit { Description = "ng/dL" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "ng/L"))
                labTestUnits.Add(new LabTestUnit { Description = "ng/L" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "ng/mL"))
                labTestUnits.Add(new LabTestUnit { Description = "ng/mL" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "ng/mL/hr"))
                labTestUnits.Add(new LabTestUnit { Description = "ng/mL/hr" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "nmol/L"))
                labTestUnits.Add(new LabTestUnit { Description = "nmol/L" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "pg/mL"))
                labTestUnits.Add(new LabTestUnit { Description = "pg/mL" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "pH"))
                labTestUnits.Add(new LabTestUnit { Description = "pH" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "pmol/L"))
                labTestUnits.Add(new LabTestUnit { Description = "pmol/L" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "sec"))
                labTestUnits.Add(new LabTestUnit { Description = "sec" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "X 10 3 /mm 3"))
                labTestUnits.Add(new LabTestUnit { Description = "X 10 3 /mm 3" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "X 10 6 /mm 3"))
                labTestUnits.Add(new LabTestUnit { Description = "X 10 6 /mm 3" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "X 10 9 /L"))
                labTestUnits.Add(new LabTestUnit { Description = "X 10 9 /L" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "μmol/L"))
                labTestUnits.Add(new LabTestUnit { Description = "μmol/L" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "μU/L"))
                labTestUnits.Add(new LabTestUnit { Description = "μU/L" });

            if (!context.LabTestUnits.Any(ce => ce.Description == "μU/mL"))
                labTestUnits.Add(new LabTestUnit { Description = "μU/mL" });

            return labTestUnits;
        }

        private static IEnumerable<MedicationForm> PrepareMedicationForms(MainDbContext context)
        {
            List<MedicationForm> medicationForms = new List<MedicationForm>();

            if (!context.MedicationForms.Any(ce => ce.Description == "Tablet"))
                medicationForms.Add(new MedicationForm { Description = "Tablet" });

            if (!context.MedicationForms.Any(ce => ce.Description == "Capsule"))
                medicationForms.Add(new MedicationForm { Description = "Capsule" });

            if (!context.MedicationForms.Any(ce => ce.Description == "Injection"))
                medicationForms.Add(new MedicationForm { Description = "Injection" });

            return medicationForms;
        }

        private static IEnumerable<MetaColumnType> PrepareMetaColumnTypes(MainDbContext context)
        {
            List<MetaColumnType> metaColumnTypes = new List<MetaColumnType>();

            if (!context.MetaColumnTypes.Any(ce => ce.Description == "bigint"))
                metaColumnTypes.Add(new MetaColumnType { MetaColumnTypeGuid = new Guid("6E0741BE-C983-4144-A580-743739E0F058"), Description = "bigint" });

            if (!context.MetaColumnTypes.Any(ce => ce.Description == "binary"))
                metaColumnTypes.Add(new MetaColumnType { MetaColumnTypeGuid = new Guid("F7A38360-A0B1-44F7-BB62-B6F452768201"), Description = "binary" });

            if (!context.MetaColumnTypes.Any(ce => ce.Description == "bit"))
                metaColumnTypes.Add(new MetaColumnType { MetaColumnTypeGuid = new Guid("95C0CB7D-59AD-4A17-A63D-D87DE6C057F4"), Description = "bit" });

            if (!context.MetaColumnTypes.Any(ce => ce.Description == "char"))
                metaColumnTypes.Add(new MetaColumnType { MetaColumnTypeGuid = new Guid("D4A35D44-2A62-46BA-90B7-E0C1D3D92D06"), Description = "char" });

            if (!context.MetaColumnTypes.Any(ce => ce.Description == "date"))
                metaColumnTypes.Add(new MetaColumnType { MetaColumnTypeGuid = new Guid("0FCDDAF2-BB13-4969-8662-EAFA364F60F0"), Description = "date" });

            if (!context.MetaColumnTypes.Any(ce => ce.Description == "datetime"))
                metaColumnTypes.Add(new MetaColumnType { MetaColumnTypeGuid = new Guid("2E20E27D-5BCC-4B30-B72E-5187239D69EC"), Description = "datetime" });

            if (!context.MetaColumnTypes.Any(ce => ce.Description == "decimal"))
                metaColumnTypes.Add(new MetaColumnType { MetaColumnTypeGuid = new Guid("C9D0FDE5-6905-4640-96B0-F241EE4AD9C9"), Description = "decimal" });

            if (!context.MetaColumnTypes.Any(ce => ce.Description == "image"))
                metaColumnTypes.Add(new MetaColumnType { MetaColumnTypeGuid = new Guid("0D7F973E-E20A-4D52-9312-096C89CCDCCC"), Description = "image" });

            if (!context.MetaColumnTypes.Any(ce => ce.Description == "int"))
                metaColumnTypes.Add(new MetaColumnType { MetaColumnTypeGuid = new Guid("79BF57DA-FFB4-42B3-B062-29FF0D041F9F"), Description = "int" });

            if (!context.MetaColumnTypes.Any(ce => ce.Description == "nchar"))
                metaColumnTypes.Add(new MetaColumnType { MetaColumnTypeGuid = new Guid("C870A7A6-6D44-4034-88A4-D29F6E175B0E"), Description = "nchar" });

            if (!context.MetaColumnTypes.Any(ce => ce.Description == "nvarchar"))
                metaColumnTypes.Add(new MetaColumnType { MetaColumnTypeGuid = new Guid("8DA14F19-4BF6-41BE-A5D9-EDC112322874"), Description = "nvarchar" });

            if (!context.MetaColumnTypes.Any(ce => ce.Description == "smallint"))
                metaColumnTypes.Add(new MetaColumnType { MetaColumnTypeGuid = new Guid("88353A5F-48BF-4DE5-9549-F66E29CC6C5A"), Description = "smallint" });

            if (!context.MetaColumnTypes.Any(ce => ce.Description == "time"))
                metaColumnTypes.Add(new MetaColumnType { MetaColumnTypeGuid = new Guid("6C556210-EAAF-4044-8C86-59AA468A113D"), Description = "time" });

            if (!context.MetaColumnTypes.Any(ce => ce.Description == "tinyint"))
                metaColumnTypes.Add(new MetaColumnType { MetaColumnTypeGuid = new Guid("C2161983-0FBB-452F-94BD-E51DCD88A2D6"), Description = "tinyint" });

            if (!context.MetaColumnTypes.Any(ce => ce.Description == "uniqueidentifier"))
                metaColumnTypes.Add(new MetaColumnType { MetaColumnTypeGuid = new Guid("D8DF321C-4FA5-4BCD-AF16-9049DB860F46"), Description = "uniqueidentifier" });

            if (!context.MetaColumnTypes.Any(ce => ce.Description == "varbinary"))
                metaColumnTypes.Add(new MetaColumnType { MetaColumnTypeGuid = new Guid("46055898-959D-4264-A6BD-0015E2FC352A"), Description = "varbinary" });

            if (!context.MetaColumnTypes.Any(ce => ce.Description == "varchar"))
                metaColumnTypes.Add(new MetaColumnType { MetaColumnTypeGuid = new Guid("289C8787-9103-44ED-B101-99C7BF2270DB"), Description = "varchar" });

            return metaColumnTypes;
        }

        private static IEnumerable<MetaTable> PrepareMetaTables(MainDbContext context)
        {
            List<MetaTable> metaTables = new List<MetaTable>();

            if (!context.MetaTables.Any(mt => mt.TableName == "Patient"))
            {
                var tableType = context.MetaTableTypes.Single(t => t.Description == "Core");
                metaTables.Add(new MetaTable { MetaTableGuid = new Guid("BF8CB037-CFE6-4904-99E2-A9AD5648C1E9"), TableName = "Patient", FriendlyName = "Patient", FriendlyDescription = "Core patient table", TableType = tableType });
            }

            if (!context.MetaTables.Any(mt => mt.TableName == "PatientClinicalEvent"))
            {
                var tableType = context.MetaTableTypes.Single(t => t.Description == "CoreChild");
                metaTables.Add(new MetaTable { MetaTableGuid = new Guid("47B9AB15-D2F6-4A9E-BB5D-9ECB0BA4E962"), TableName = "PatientClinicalEvent", FriendlyName = "Patient Adverse Events", FriendlyDescription = "Patient adverse event history", TableType = tableType });
            }

            if (!context.MetaTables.Any(mt => mt.TableName == "PatientCondition"))
            {
                var tableType = context.MetaTableTypes.Single(t => t.Description == "CoreChild");
                metaTables.Add(new MetaTable { MetaTableGuid = new Guid("198FC6F6-F3A3-4D14-957C-89D2036D70E9"), TableName = "PatientCondition", FriendlyName = "Patient Conditions", FriendlyDescription = "Patient condition history", TableType = tableType });
            }

            if (!context.MetaTables.Any(mt => mt.TableName == "PatientFacility"))
            {
                var tableType = context.MetaTableTypes.Single(t => t.Description == "History");
                metaTables.Add(new MetaTable { MetaTableGuid = new Guid("7F5A0F37-11EE-425B-BC2B-BDF990BF5C86"), TableName = "PatientFacility", FriendlyName = "Current Facility", FriendlyDescription = "Current facility", TableType = tableType });
            }

            if (!context.MetaTables.Any(mt => mt.TableName == "PatientLabTest"))
            {
                var tableType = context.MetaTableTypes.Single(t => t.Description == "CoreChild");
                metaTables.Add(new MetaTable { MetaTableGuid = new Guid("8C90FB6B-BCA8-4FEF-8EE2-020902A400DA"), TableName = "PatientLabTest", FriendlyName = "Patien Lab Tests", FriendlyDescription = "Patient evaluation history", TableType = tableType });
            }

            if (!context.MetaTables.Any(mt => mt.TableName == "PatientMedication"))
            {
                var tableType = context.MetaTableTypes.Single(t => t.Description == "CoreChild");
                metaTables.Add(new MetaTable { MetaTableGuid = new Guid("A90FBBD8-DC06-41F4-B30E-681A68B5E1AE"), TableName = "PatientMedication", FriendlyName = "Patient Medications", FriendlyDescription = "Patient medication history", TableType = tableType });
            }

            if (!context.MetaTables.Any(mt => mt.TableName == "Encounter"))
            {
                var tableType = context.MetaTableTypes.Single(t => t.Description == "Child");
                metaTables.Add(new MetaTable { MetaTableGuid = new Guid("047EDD87-0DAB-4A7E-ABD8-8AAAF8CCBFF6"), TableName = "Encounter", FriendlyName = "Patient Encounters", FriendlyDescription = "Patient encounter history", TableType = tableType });
            }

            if (!context.MetaTables.Any(mt => mt.TableName == "CohortGroupEnrolment"))
            {
                var tableType = context.MetaTableTypes.Single(t => t.Description == "CoreChild");
                metaTables.Add(new MetaTable { MetaTableGuid = new Guid("EC1F2FFF-5416-4E63-855B-731872136C1A"), TableName = "CohortGroupEnrolment", FriendlyName = "Cohort Enrolment", FriendlyDescription = "Patient cohort enrolments", TableType = tableType });
            }

            return metaTables;
        }

        private static IEnumerable<MetaTableType> PrepareMetaTableTypes(MainDbContext context)
        {
            List<MetaTableType> metaTableTypes = new List<MetaTableType>();

            if (!context.MetaTableTypes.Any(ce => ce.Description == "Core"))
                metaTableTypes.Add(new MetaTableType { MetaTableTypeGuid = new Guid("7C1FE90D-2082-464E-AEFD-337E81088312"), Description = "Core" });

            if (!context.MetaTableTypes.Any(ce => ce.Description == "CoreChild"))
                metaTableTypes.Add(new MetaTableType { MetaTableTypeGuid = new Guid("94040819-D706-40BF-B3D1-66A0655A0BEF"), Description = "CoreChild" });

            if (!context.MetaTableTypes.Any(ce => ce.Description == "Child"))
                metaTableTypes.Add(new MetaTableType { MetaTableTypeGuid = new Guid("E4E98018-D67B-417E-973D-E25E36FF23DB"), Description = "Child" });

            if (!context.MetaTableTypes.Any(ce => ce.Description == "History"))
                metaTableTypes.Add(new MetaTableType { MetaTableTypeGuid = new Guid("F80D75CF-AAC7-48CE-A6FF-B8188F4B284C"), Description = "History" });

            if (!context.MetaTableTypes.Any(ce => ce.Description == "Lookup"))
                metaTableTypes.Add(new MetaTableType { MetaTableTypeGuid = new Guid("68FDA9E5-911A-48DD-895B-8DFD038BE41D"), Description = "Lookup" });

            return metaTableTypes;
        }

        private static IEnumerable<MetaWidgetType> PrepareMetaWidgetTypes(MainDbContext context)
        {
            List<MetaWidgetType> metaWidgetTypes = new List<MetaWidgetType>();

            if (!context.MetaWidgetTypes.Any(ce => ce.Description == "General"))
                metaWidgetTypes.Add(new MetaWidgetType { MetaWidgetTypeGuid = new Guid("806DF7BC-404C-4DA5-A726-880E8BB0FEAA"), Description = "General" });

            if (!context.MetaWidgetTypes.Any(ce => ce.Description == "SubItems"))
                metaWidgetTypes.Add(new MetaWidgetType { MetaWidgetTypeGuid = new Guid("58DD7339-B9E0-4CBB-A47B-B2C027D6DF86"), Description = "SubItems" });

            if (!context.MetaWidgetTypes.Any(ce => ce.Description == "ItemList"))
                metaWidgetTypes.Add(new MetaWidgetType { MetaWidgetTypeGuid = new Guid("7179F12D-668E-4A5D-A676-D9C1FEBDC6A4"), Description = "ItemList" });

            return metaWidgetTypes;
        }

        private static IEnumerable<OrgUnitType> PrepareOrgUnitTypes(MainDbContext context)
        {
            List<OrgUnitType> orgUnitTypes = new List<OrgUnitType>();

            if (!context.OrgUnitTypes.Any(ou => ou.Description == "Region"))
                orgUnitTypes.Add(new OrgUnitType("Region", null));

            return orgUnitTypes;
        }

        private static IEnumerable<Outcome> PrepareOutcomes(MainDbContext context)
        {
            List<Outcome> outcomes = new List<Outcome>();

            if (!context.Outcomes.Any(ce => ce.Description == "Recovered/Resolved"))
                outcomes.Add(new Outcome { Description = "Recovered/Resolved" });

            if (!context.Outcomes.Any(ce => ce.Description == "Recovered/Resolved With Sequelae"))
                outcomes.Add(new Outcome { Description = "Recovered/Resolved With Sequelae" });

            if (!context.Outcomes.Any(ce => ce.Description == "Recovering/Resolving"))
                outcomes.Add(new Outcome { Description = "Recovering/Resolving" });

            if (!context.Outcomes.Any(ce => ce.Description == "Not Recovered/Not Resolved"))
                outcomes.Add(new Outcome { Description = "Not Recovered/Not Resolved" });

            if (!context.Outcomes.Any(ce => ce.Description == "Fatal"))
                outcomes.Add(new Outcome { Description = "Fatal" });

            if (!context.Outcomes.Any(ce => ce.Description == "Unknown"))
                outcomes.Add(new Outcome { Description = "Unknown" });

            return outcomes;
        }

        private static IEnumerable<PatientStatus> PreparePatientStatus(MainDbContext context)
        {
            List<PatientStatus> patientStatuses = new List<PatientStatus>();

            if (!context.PatientStatuses.Any(ce => ce.Description == "Active"))
            {
                patientStatuses.Add(new PatientStatus { Description = "Active" });
            }

            if (!context.PatientStatuses.Any(ce => ce.Description == "Suspended"))
            {
                patientStatuses.Add(new PatientStatus { Description = "Suspended" });
            }

            if (!context.PatientStatuses.Any(ce => ce.Description == "Transferred Out"))
            {
                patientStatuses.Add(new PatientStatus { Description = "Transferred Out" });
            }

            if (!context.PatientStatuses.Any(ce => ce.Description == "Died"))
            {
                patientStatuses.Add(new PatientStatus { Description = "Died" });
            }

            return patientStatuses;
        }

        private static IEnumerable<Priority> PreparePriorities(MainDbContext context)
        {
            List<Priority> priorities = new List<Priority>();

            if (!context.Priorities.Any(ce => ce.Description == "Not Set"))
                priorities.Add(new Priority { Description = "Not Set" });

            if (!context.Priorities.Any(ce => ce.Description == "Urgent"))
                priorities.Add(new Priority { Description = "Urgent" });

            if (!context.Priorities.Any(ce => ce.Description == "High"))
                priorities.Add(new Priority { Description = "High" });

            if (!context.Priorities.Any(ce => ce.Description == "Medium"))
                priorities.Add(new Priority { Description = "Medium" });

            if (!context.Priorities.Any(ce => ce.Description == "Low"))
                priorities.Add(new Priority { Description = "Low" });

            return priorities;
        }

        private static IEnumerable<SiteContactDetail> PrepareSiteContactDetails(MainDbContext context)
        {
            List<SiteContactDetail> siteContactDetails = new List<SiteContactDetail>();

            if (!context.SiteContactDetails.Any(ce => ce.ContactType == ContactType.SendingAuthority))
            {
                var sendingAuthority = new SiteContactDetail(ContactType.SendingAuthority, OrganisationType.RegulatoryAuthority, "", "", "Not", "Specified", "", "", "", "", "", "", "");
                sendingAuthority.AuditStamp(context.Users.Single(u => u.UserName == "Admin"));
                siteContactDetails.Add(sendingAuthority);
            }

            if (!context.SiteContactDetails.Any(ce => ce.ContactType == ContactType.ReceivingAuthority))
            {
                var receivingAuthority = new SiteContactDetail(ContactType.ReceivingAuthority, OrganisationType.WHOCollaboratingCenterForInternationalDrugMonitoring, "UMC", "", "Not", "Specified", "Bredgrand 7B", "Uppsala", "46", "75320", "", "18656060", "info@who-umc.org");
                receivingAuthority.AuditStamp(context.Users.Single(u => u.UserName == "Admin"));
                siteContactDetails.Add(receivingAuthority);
            }

            return siteContactDetails;
        }

        private static IEnumerable<TerminologyMedDra> PrepareMeddraTerms(MainDbContext context)
        {
            List<TerminologyMedDra> terms = new List<TerminologyMedDra>();

            if (!context.TerminologyMedDras.Any(ce => ce.MedDraTerm == "Tuberculosis"))
                terms.Add(new TerminologyMedDra { MedDraTerm = "Tuberculosis", MedDraCode = "10044755", MedDraTermType = "LLT ", MedDraVersion = "23.0", Common = false });

            if (!context.TerminologyMedDras.Any(ce => ce.MedDraTerm == "HIV infection"))
                terms.Add(new TerminologyMedDra { MedDraTerm = "HIV infection", MedDraCode = "10020161", MedDraTermType = "LLT ", MedDraVersion = "23.0", Common = false });

            return terms;
        }

        private static IEnumerable<TreatmentOutcome> PrepareTreatmentOutcomes(MainDbContext context)
        {
            List<TreatmentOutcome> treatmentOutcomes = new List<TreatmentOutcome>();

            if (!context.TreatmentOutcomes.Any(ce => ce.Description == "Cured"))
                treatmentOutcomes.Add(new TreatmentOutcome { Description = "Cured" });

            if (!context.TreatmentOutcomes.Any(ce => ce.Description == "Treatment Completed"))
                treatmentOutcomes.Add(new TreatmentOutcome { Description = "Treatment Completed" });

            if (!context.TreatmentOutcomes.Any(ce => ce.Description == "Treatment Failed"))
                treatmentOutcomes.Add(new TreatmentOutcome { Description = "Treatment Failed" });

            if (!context.TreatmentOutcomes.Any(ce => ce.Description == "Died"))
                treatmentOutcomes.Add(new TreatmentOutcome { Description = "Died" });

            if (!context.TreatmentOutcomes.Any(ce => ce.Description == "Lost to Follow-up"))
                treatmentOutcomes.Add(new TreatmentOutcome { Description = "Lost to Follow-up" });

            if (!context.TreatmentOutcomes.Any(ce => ce.Description == "Not Evaluated"))
                treatmentOutcomes.Add(new TreatmentOutcome { Description = "Not Evaluated" });

            return treatmentOutcomes;
        }

        private static IEnumerable<WorkFlow> PrepareWorkFlows(MainDbContext context)
        {
            List<WorkFlow> workFlows = new List<WorkFlow>();

            if (!context.WorkFlows.Any(wf => wf.Description == "New Active Surveilliance Report"))
                workFlows.Add(new WorkFlow("New Active Surveilliance Report", new Guid("892F3305-7819-4F18-8A87-11CBA3AEE219")));

            if (!context.WorkFlows.Any(wf => wf.Description == "New Spontaneous Surveilliance Report"))
                workFlows.Add(new WorkFlow("New Spontaneous Surveilliance Report", new Guid("4096D0A3-45F7-4702-BDA1-76AEDE41B986")));

            return workFlows;
        }

        private static AsyncRetryPolicy CreatePolicy(ILogger<MainContextSeed> logger, string prefix, int retries = 3)
        {
            return Policy.Handle<SqlException>().
                WaitAndRetryAsync(
                    retryCount: retries,
                    sleepDurationProvider: retry => TimeSpan.FromSeconds(5),
                    onRetry: (exception, timeSpan, retry, ctx) =>
                    {
                        logger.LogWarning(exception, "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}", prefix, exception.GetType().Name, exception.Message, retry, retries);
                    }
                );
        }
    }
}
