using MediatR;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Commands.MetaFormAggregate
{
    [DataContract]
    public class DeleteMetaFormCategoryCommand
        : IRequest<bool>
    {
        [DataMember]
        public int MetaFormId { get; private set; }

        [DataMember]
        public int MetaFormCategoryId { get; private set; }

        public DeleteMetaFormCategoryCommand()
        {
        }

        public DeleteMetaFormCategoryCommand(int metaFormId, int metaFormCategoryId) : this()
        {
            MetaFormId = metaFormId;
            MetaFormCategoryId = metaFormCategoryId;
        }
    }
}
