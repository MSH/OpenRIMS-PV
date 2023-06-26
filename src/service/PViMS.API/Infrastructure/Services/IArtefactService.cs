using PVIMS.Core.Models;
using System;
using System.Threading.Tasks;

namespace PVIMS.API.Infrastructure.Services
{
    public interface IArtefactService
    {
        Task<ArtefactInfoModel> CreatePatientSummaryForActiveReportAsync(Guid contextGuid);

        Task<ArtefactInfoModel> CreatePatientSummaryForSpontaneousReportAsync(Guid contextGuid);
    }
}
