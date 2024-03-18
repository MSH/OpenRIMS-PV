using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Commands.MetaFormAggregate
{
    [DataContract]
    public class AddMetaFormCategoryAttributeCommand
        : IRequest<MetaFormCategoryAttributeDto>
    {
        [DataMember]
        public int MetaFormId { get; private set; }

        [DataMember]
        public int MetaFormCategoryId { get; private set; }

        [DataMember]
        public string AttributeName { get; private set; }

        [DataMember]
        public int? CustomAttributeConfigurationId { get; private set; }

        [DataMember]
        public string Label { get; private set; }

        [DataMember]
        public string Help { get; private set; }

        public AddMetaFormCategoryAttributeCommand()
        {
        }

        public AddMetaFormCategoryAttributeCommand(int metaFormId, int metaFormCategoryId, string attributeName, int? customAttributeConfigurationId, string label, string help): this()
        {
            MetaFormId = metaFormId;
            MetaFormCategoryId = metaFormCategoryId;
            AttributeName = attributeName;
            CustomAttributeConfigurationId = customAttributeConfigurationId;
            Label = label;
            Help = help;
        }
    }
}