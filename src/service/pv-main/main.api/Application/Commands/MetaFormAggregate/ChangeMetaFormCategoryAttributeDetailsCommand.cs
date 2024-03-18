using MediatR;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Commands.MetaFormAggregate
{
    [DataContract]
    public class ChangeMetaFormCategoryAttributeDetailsCommand
        : IRequest<bool>
    {
        [DataMember]
        public int MetaFormId { get; private set; }

        [DataMember]
        public int MetaFormCategoryId { get; private set; }

        [DataMember]
        public int MetaFormCategoryAttributeId { get; private set; }

        [DataMember]
        public string Label { get; private set; }

        [DataMember]
        public string Help { get; private set; }

        public ChangeMetaFormCategoryAttributeDetailsCommand()
        {
        }

        public ChangeMetaFormCategoryAttributeDetailsCommand(int metaFormId, int metaFormCategoryId, int metaFormCategoryAttributeId, string label, string help): this()
        {
            MetaFormId = metaFormId;
            MetaFormCategoryId = metaFormCategoryId;
            MetaFormCategoryAttributeId = metaFormCategoryAttributeId;
            Label = label;
            Help = help;
        }
    }
}