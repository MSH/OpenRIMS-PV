using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Commands.MetaFormAggregate
{
    [DataContract]
    public class AddMetaFormCommand
        : IRequest<MetaFormIdentifierDto>
    {
        [DataMember]
        public int CohortGroupId { get; private set; }

        [DataMember]
        public string FormName { get; private set; }

        [DataMember]
        public string ActionName { get; private set; }

        public AddMetaFormCommand()
        {
        }

        public AddMetaFormCommand(int cohortGroupId, string formName, string actionName) : this()
        {
            CohortGroupId = cohortGroupId;
            FormName = formName;
            ActionName = actionName;
        }
    }
}
