using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PVIMS.Core.Services;
using PVIMS.Core.ValueTypes;
using PVIMS.Infrastructure;
using PVIMS.Core.Entities.Keyless;

namespace PVIMS.Services
{
    public class ReportService : IReportService 
    {
        private readonly PVIMSDbContext _context;

        public ReportService(PVIMSDbContext dbContext)
        {
            _context = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public ICollection<AdverseEventList> GetAdverseEventItems(DateTime searchFrom, DateTime searchTo, AdverseEventCriteria adverseEventCriteria, AdverseEventStratifyCriteria adverseEventStratifyCriteria)
        {
            string sql = "";

            if (adverseEventCriteria == AdverseEventCriteria.ReportSource)
            {
                switch (adverseEventStratifyCriteria)
                {
                    case AdverseEventStratifyCriteria.AgeGroup:
                        sql = string.Format(@"
                            SELECT mpce.SourceTerminologyMedDra AS 'Description', 
			                            CASE WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) < 5844 THEN '<16'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) BETWEEN 5844 AND 9131 THEN 'Between 16 and 25'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) BETWEEN 9132 AND 12784 THEN 'Between 26 and 35'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) BETWEEN 12785 AND 16436 THEN 'Between 36 and 45'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) BETWEEN 16437 AND 20089 THEN 'Between 46 and 55'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) > 20089 THEN '>55' END AS 'Criteria',
                                    [Istheadverseeventserious?] AS 'Serious',
		                            COUNT(*) AS PatientCount
                            FROM MetaPatientClinicalEvent mpce 
	                            INNER JOIN MetaPatient mp ON mpce.Patient_Id = mp.Id
                            WHERE mpce.OnsetDate BETWEEN '{0}' AND '{1}' 
                            GROUP BY mpce.SourceTerminologyMedDra, 
			                            CASE WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) < 5844 THEN '<16'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) BETWEEN 5844 AND 9131 THEN 'Between 16 and 25'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) BETWEEN 9132 AND 12784 THEN 'Between 26 and 35'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) BETWEEN 12785 AND 16436 THEN 'Between 36 and 45'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) BETWEEN 16437 AND 20089 THEN 'Between 46 and 55'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) > 20089 THEN '>55' END
                                        , [Istheadverseeventserious?]
                            ORDER BY mpce.SourceTerminologyMedDra asc, Criteria asc, [Istheadverseeventserious?] asc"
                                , searchFrom.ToString("yyyy-MM-dd"), searchTo.ToString("yyyy-MM-dd"));
                        break;

                    case AdverseEventStratifyCriteria.Facility:
                        sql = string.Format(@"
                            SELECT mpce.SourceTerminologyMedDra AS 'Description',
			                            mpf.Facility AS Criteria,
                                    [Istheadverseeventserious?] AS 'Serious',
		                            COUNT(*) AS PatientCount
                            FROM MetaPatientClinicalEvent mpce 
	                            INNER JOIN MetaPatient mp ON mpce.Patient_Id = mp.Id
	                            INNER JOIN MetaPatientFacility mpf ON mp.Id = mpf.Patient_Id AND mpf.EnrolledDate = (SELECT MAX(EnrolledDate) FROM MetaPatientFacility impf WHERE impf.Patient_Id = mp.Id)
                            WHERE mpce.OnsetDate BETWEEN '{0}' AND '{1}'
                            GROUP BY mpce.SourceTerminologyMedDra, mpf.Facility, [Istheadverseeventserious?]
                            ORDER BY mpce.SourceTerminologyMedDra asc, mpf.Facility asc, [Istheadverseeventserious?] asc"
                                , searchFrom.ToString("yyyy-MM-dd"), searchTo.ToString("yyyy-MM-dd"));
                        break;

                    case AdverseEventStratifyCriteria.Drug:
                        sql = string.Format(@"
                            SELECT mpce.SourceTerminologyMedDra AS 'Description',
			                            mpm.Medication AS Criteria,
                                    [Istheadverseeventserious?] AS 'Serious',
		                            COUNT(*) AS PatientCount
                            FROM MetaPatientClinicalEvent mpce 
	                            INNER JOIN MetaPatient mp ON mpce.Patient_Id = mp.Id
                                INNER JOIN ReportInstance ri ON ri.ContextGuid = mpce.PatientClinicalEventGuid
                                INNER JOIN ReportInstanceMedication rim ON ri.Id = rim.ReportInstance_Id
                                INNER JOIN MetaPatientMedication mpm ON rim.ReportInstanceMedicationGuid = mpm.PatientMedicationGuid
                            WHERE mpce.OnsetDate BETWEEN '{0}' AND '{1}' 
                            GROUP BY mpce.SourceTerminologyMedDra, mpm.Medication, [Istheadverseeventserious?] 
                            ORDER BY mpce.SourceTerminologyMedDra asc, mpm.Medication asc, [Istheadverseeventserious?] asc"
                                , searchFrom.ToString("yyyy-MM-dd"), searchTo.ToString("yyyy-MM-dd"));
                        break;

                    case AdverseEventStratifyCriteria.Cohort:
                        sql = string.Format(@"
                            SELECT mpce.SourceTerminologyMedDra AS 'Description',
			                            cg.CohortName AS Criteria,
                                    [Istheadverseeventserious?] AS 'Serious',
		                            COUNT(*) AS PatientCount
                            FROM MetaPatientClinicalEvent mpce 
	                            INNER JOIN MetaPatient mp ON mpce.Patient_Id = mp.Id
                                INNER JOIN CohortGroupEnrolment cge ON mp.Id = cge.Patient_Id
                                INNER JOIN CohortGroup cg ON cge.CohortGroup_Id = cg.Id
                            WHERE mpce.OnsetDate BETWEEN '{0}' AND '{1}' 
                            GROUP BY mpce.SourceTerminologyMedDra, cg.CohortName, [Istheadverseeventserious?] 
                            ORDER BY mpce.SourceTerminologyMedDra asc, cg.CohortName asc, [Istheadverseeventserious?] asc"
                                , searchFrom.ToString("yyyy-MM-dd"), searchTo.ToString("yyyy-MM-dd"));
                        break;

                    default:
                        break;
                }
            }
            else // MedDRA
            {
                switch (adverseEventStratifyCriteria)
                {
                    case AdverseEventStratifyCriteria.AgeGroup:
                        sql = string.Format(@"
                            SELECT t.MedDraTerm AS 'Description', 
			                            CASE WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) < 5844 THEN '<16'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) BETWEEN 5844 AND 9131 THEN 'Between 16 and 25'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) BETWEEN 9132 AND 12784 THEN 'Between 26 and 35'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) BETWEEN 12785 AND 16436 THEN 'Between 36 and 45'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) BETWEEN 16437 AND 20089 THEN 'Between 46 and 55'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) > 20089 THEN '>55' END AS 'Criteria',
                                    [Istheadverseeventserious?] AS 'Serious',
		                            COUNT(*) AS PatientCount
                            FROM MetaPatientClinicalEvent mpce 
	                            INNER JOIN MetaPatient mp ON mpce.Patient_Id = mp.Id
                                INNER JOIN ReportInstance ri ON ri.ContextGuid = mpce.PatientClinicalEventGuid
	                            INNER JOIN TerminologyMedDra t ON ri.TerminologyMedDra_Id = t.Id
                            WHERE mpce.OnsetDate BETWEEN '{0}' AND '{1}'
                            GROUP BY t.MedDraTerm, 
			                            CASE WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) < 5844 THEN '<16'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) BETWEEN 5844 AND 9131 THEN 'Between 16 and 25'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) BETWEEN 9132 AND 12784 THEN 'Between 26 and 35'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) BETWEEN 12785 AND 16436 THEN 'Between 36 and 45'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) BETWEEN 16437 AND 20089 THEN 'Between 46 and 55'
				                            WHEN DATEDIFF(dd, mp.DateOfBirth, GETDATE()) > 20089 THEN '>55' END
                                        , [Istheadverseeventserious?]
                            ORDER BY t.MedDraTerm asc, Criteria asc, [Istheadverseeventserious?] asc"
                                , searchFrom.ToString("yyyy-MM-dd"), searchTo.ToString("yyyy-MM-dd"));
                        break;

                    case AdverseEventStratifyCriteria.Facility:
                        sql = string.Format(@"
                            SELECT t.MedDraTerm AS 'Description',
			                            mpf.Facility AS Criteria,
                                    [Istheadverseeventserious?] AS 'Serious',
		                            COUNT(*) AS PatientCount
                            FROM MetaPatientClinicalEvent mpce 
	                            INNER JOIN MetaPatient mp ON mpce.Patient_Id = mp.Id
                                INNER JOIN ReportInstance ri ON ri.ContextGuid = mpce.PatientClinicalEventGuid
	                            INNER JOIN TerminologyMedDra t ON ri.TerminologyMedDra_Id = t.Id
	                            INNER JOIN MetaPatientFacility mpf ON mp.Id = mpf.Patient_Id AND mpf.EnrolledDate = (SELECT MAX(EnrolledDate) FROM MetaPatientFacility impf WHERE impf.Patient_Id = mp.Id)
                            WHERE mpce.OnsetDate BETWEEN '{0}' AND '{1}'
                            GROUP BY t.MedDraTerm, mpf.Facility, [Istheadverseeventserious?]
                            ORDER BY t.MedDraTerm asc, mpf.Facility asc, [Istheadverseeventserious?] asc", searchFrom.ToString("yyyy-MM-dd"), searchTo.ToString("yyyy-MM-dd"));
                        break;

                    case AdverseEventStratifyCriteria.Drug:
                        sql = string.Format(@"
                            SELECT t.MedDraTerm AS 'Description',
			                            mpm.Medication AS Criteria,
                                    [Istheadverseeventserious?] AS 'Serious',
		                            COUNT(*) AS PatientCount
                            FROM MetaPatientClinicalEvent mpce 
	                            INNER JOIN MetaPatient mp ON mpce.Patient_Id = mp.Id
                                INNER JOIN ReportInstance ri ON ri.ContextGuid = mpce.PatientClinicalEventGuid
	                            INNER JOIN TerminologyMedDra t ON ri.TerminologyMedDra_Id = t.Id
                                INNER JOIN ReportInstanceMedication rim ON ri.Id = rim.ReportInstance_Id
                                INNER JOIN MetaPatientMedication mpm ON rim.ReportInstanceMedicationGuid = mpm.PatientMedicationGuid
                            WHERE mpce.OnsetDate BETWEEN '{0}' AND '{1}'
                            GROUP BY t.MedDraTerm, mpm.Medication, [Istheadverseeventserious?]
                            ORDER BY t.MedDraTerm asc, mpm.Medication asc, [Istheadverseeventserious?] asc", searchFrom.ToString("yyyy-MM-dd"), searchTo.ToString("yyyy-MM-dd"));
                        break;

                    case AdverseEventStratifyCriteria.Cohort:
                        sql = string.Format(@"
                            SELECT t.MedDraTerm AS 'Description',
			                            cg.CohortName AS Criteria,
                                    [Istheadverseeventserious?] AS 'Serious',
		                            COUNT(*) AS PatientCount
                            FROM MetaPatientClinicalEvent mpce 
	                            INNER JOIN MetaPatient mp ON mpce.Patient_Id = mp.Id
                                INNER JOIN ReportInstance ri ON ri.ContextGuid = mpce.PatientClinicalEventGuid
	                            INNER JOIN TerminologyMedDra t ON ri.TerminologyMedDra_Id = t.Id
                                INNER JOIN CohortGroupEnrolment cge ON mp.Id = cge.Patient_Id
                                INNER JOIN CohortGroup cg ON cge.CohortGroup_Id = cg.Id
                            WHERE mpce.OnsetDate BETWEEN '{0}' AND '{1}' 
                            GROUP BY t.MedDraTerm, cg.CohortName, [Istheadverseeventserious?] 
                            ORDER BY t.MedDraTerm asc, cg.CohortName asc, [Istheadverseeventserious?] asc"
                                , searchFrom.ToString("yyyy-MM-dd"), searchTo.ToString("yyyy-MM-dd"));
                        break;

                    default:
                        break;
                }
            }

            return _context.AdverseEventLists
                .FromSqlInterpolated($"Exec spAdverseEventList {searchFrom.ToString("yyyy-MM-dd")}, {searchTo.ToString("yyyy-MM-dd")}")
                .ToList();
        }

        public ICollection<DrugList> GetPatientsByDrugItems(string searchTerm)
        {
            string sql = "";

            sql = string.Format(@"
                SELECT CONCAT(c.ConceptName collate SQL_Latin1_General_CP1_CI_AS, ' (', RTRIM(mf.[Description]) collate SQL_Latin1_General_CP1_CI_AS, ')') as ConceptName, c.Id as ConceptId 
	                ,(
		                select count(distinct(ip.Id))
		                from Patient ip
			                inner join PatientMedication ipm on ip.Id = ipm.Patient_Id 
			                inner join Concept ic on ipm.Concept_Id = ic.Id
		                where ic.Id = c.Id and ip.Archived = 0 and ipm.Archived = 0 
	                ) AS PatientCount
                FROM Concept c 
                    INNER JOIN MedicationForm mf on c.MedicationForm_Id = mf.Id 
                {0} 
                ORDER BY c.ConceptName", !String.IsNullOrWhiteSpace(searchTerm) ? $"WHERE c.ConceptName LIKE '%{searchTerm.TrimEnd()}%'" : "");

            return _context.DrugLists
                .FromSqlInterpolated($"Exec spDrugList {searchTerm}")
                .ToList();
        }
    }
}
