using OpenRIMS.PV.Main.Core.Models;
using System.Collections.Generic;

namespace OpenRIMS.PV.Main.API.Infrastructure.Services
{
    public interface IWordDocumentService
    {
        void CreateDocument(ArtefactInfoModel model);

        void AddPageHeader(string header);

        void AddTableHeader(string header);

        void AddFourColumnTable(List<KeyValuePair<string, string>> rows);

        void AddTwoColumnTable(List<KeyValuePair<string, string>> rows);

        void AddOneColumnTable(List<string> rows);

        void AddRowTable(List<string[]> rows, int[] cellWidths);
    }
}
