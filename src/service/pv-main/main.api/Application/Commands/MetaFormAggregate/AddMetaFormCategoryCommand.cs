using MediatR;
using OpenRIMS.PV.Main.API.Models;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Commands.MetaFormAggregate
{
    [DataContract]
    public class AddMetaFormCategoryCommand
        : IRequest<MetaFormCategoryDto>
    {
        [DataMember]
        public int MetaFormId { get; private set; }

        [DataMember]
        public int MetaTableId { get; private set; }

        [DataMember]
        public string CategoryName { get; private set; }

        [DataMember]
        public string Help { get; private set; }

        [DataMember]
        public string Icon { get; private set; }

        public AddMetaFormCategoryCommand()
        {
        }

        public AddMetaFormCategoryCommand(int metaFormId, int metaTableId, string categoryName, string help, string icon) : this()
        {
            MetaFormId = metaFormId;
            MetaTableId = metaTableId;
            CategoryName = categoryName;
            Help = help;
            Icon = icon;
        }
    }
}