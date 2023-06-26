using PVIMS.Core.Models;
using System.Threading.Tasks;

namespace PVIMS.API.Infrastructure.Services
{
    public interface IExcelDocumentService
    {
        ArtefactInfoModel CreateActiveDatasetForDownload(long[] patientIds, long cohortGroupId);

        ArtefactInfoModel CreateSpontaneousDatasetForDownload();

        Task<ArtefactInfoModel> CreateDatasetInstanceForDownloadAsync(long datasetInstanceId);
    }
}
