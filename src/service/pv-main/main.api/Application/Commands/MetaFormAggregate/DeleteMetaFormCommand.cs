using MediatR;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Commands.MetaFormAggregate
{
    [DataContract]
    public class DeleteMetaFormCommand
        : IRequest<bool>
    {
        [DataMember]
        public int MetaFormId { get; private set; }

        public DeleteMetaFormCommand()
        {
        }

        public DeleteMetaFormCommand(int metaFormId) : this()
        {
            MetaFormId = metaFormId;
        }
    }
}
