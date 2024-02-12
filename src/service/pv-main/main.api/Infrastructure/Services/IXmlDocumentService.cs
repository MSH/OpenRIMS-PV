using OpenRIMS.PV.Main.Core.Aggregates.DatasetAggregate;
using OpenRIMS.PV.Main.Core.Models;
using System.Threading.Tasks;

namespace OpenRIMS.PV.Main.API.Infrastructure.Services
{
    public interface IXmlDocumentService
    {
        Task<ArtefactInfoModel> CreateE2BDocumentAsync(DatasetInstance e2bInstance);
    }
}
