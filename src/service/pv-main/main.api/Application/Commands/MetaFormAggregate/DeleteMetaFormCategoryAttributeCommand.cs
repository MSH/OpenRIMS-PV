using MediatR;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Commands.MetaFormAggregate
{
    [DataContract]
    public class DeleteMetaFormCategoryAttributeCommand
        : IRequest<bool>
    {
        [DataMember]
        public int MetaFormId { get; private set; }

        [DataMember]
        public int MetaFormCategoryId { get; private set; }

        [DataMember]
        public int MetaFormCategoryAttributeId { get; private set; }

        public DeleteMetaFormCategoryAttributeCommand()
        {
        }

        public DeleteMetaFormCategoryAttributeCommand(int metaFormId, int metaFormCategoryId, int metaFormCategoryAttributeId) : this()
        {
            MetaFormId = metaFormId;
            MetaFormCategoryId = metaFormCategoryId;
            MetaFormCategoryAttributeId = metaFormCategoryAttributeId;
        }
    }
}
