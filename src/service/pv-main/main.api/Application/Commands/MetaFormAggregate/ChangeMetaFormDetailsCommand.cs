using MediatR;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Commands.MetaFormAggregate
{
    [DataContract]
    public class ChangeMetaFormDetailsCommand
        : IRequest<bool>
    {
        [DataMember]
        public int MetaFormId { get; private set; }

        [DataMember]
        public string FormName { get; private set; }

        [DataMember]
        public string ActionName { get; private set; }

        public ChangeMetaFormDetailsCommand()
        {
        }

        public ChangeMetaFormDetailsCommand(int metaFormId, string formName, string actionName): this()
        {
            MetaFormId = metaFormId;
            FormName = formName;
            ActionName = actionName;
        }
    }
}