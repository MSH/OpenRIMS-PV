using OpenRIMS.PV.Main.Core.Models;
using System;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Infrastructure.Services
{
    public interface IArtefactService
    {
        Task<ArtefactInfoModel> CreatePatientSummaryForActiveReportAsync(Guid contextGuid);

        Task<ArtefactInfoModel> CreatePatientSummaryForSpontaneousReportAsync(Guid contextGuid);
    }
}
