using OpenRIMS.PV.Main.API.Models;
using OpenRIMS.PV.Main.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Infrastructure.Services
{
    public interface IExcelDocumentService
    {
        void CreateDocument(ArtifactDto model);

        void AddSheet(string sheetName, List<List<string>> data);

        ArtefactInfoModel CreateActiveDatasetForDownload(long[] patientIds, long cohortGroupId);

        ArtefactInfoModel CreateSpontaneousDatasetForDownload();

        Task<ArtefactInfoModel> CreateDatasetInstanceForDownloadAsync(long datasetInstanceId);
    }
}
