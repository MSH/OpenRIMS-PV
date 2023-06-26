using PVIMS.Core.Aggregates.DatasetAggregate;
using PVIMS.Core.Models;
using System.Threading.Tasks;

namespace PVIMS.API.Infrastructure.Services
{
    public interface IXmlDocumentService
    {
        Task<ArtefactInfoModel> CreateE2BDocumentAsync(DatasetInstance e2bInstance);
    }
}
