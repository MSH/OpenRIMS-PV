using OpenRIMS.PV.Main.Core.Models;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Infrastructure.Services
{
    public interface IExcelDocumentService
    {
        ArtefactInfoModel CreateActiveDatasetForDownload(long[] patientIds, long cohortGroupId);

        ArtefactInfoModel CreateSpontaneousDatasetForDownload();

        Task<ArtefactInfoModel> CreateDatasetInstanceForDownloadAsync(long datasetInstanceId);
    }
}
