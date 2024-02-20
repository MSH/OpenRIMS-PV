using MediatR;
using System.Runtime.Serialization;

namespace OpenRIMS.PV.Main.API.Application.Commands.MetaFormAggregate
{
    [DataContract]
    public class ChangeMetaFormCategoryDetailsCommand
        : IRequest<bool>
    {
        [DataMember]
        public int MetaFormId { get; private set; }

        [DataMember]
        public int MetaFormCategoryId { get; private set; }

        [DataMember]
        public string CategoryName { get; private set; }

        [DataMember]
        public string Help { get; private set; }

        [DataMember]
        public string Icon { get; private set; }

        public ChangeMetaFormCategoryDetailsCommand()
        {
        }

        public ChangeMetaFormCategoryDetailsCommand(int metaFormId, int metaFormCategoryId, string categoryName, string help, string icon): this()
        {
            MetaFormId = metaFormId;
            MetaFormCategoryId = metaFormCategoryId;
            CategoryName = categoryName;
            Help = help;
            Icon = icon;
        }
    }
}