using System.Collections.Generic;

namespace OpenRIMS.PV.Main.Core.Entities
{
    public partial class AttachmentType : EntityBase
    {
        protected AttachmentType()
        {
        }

        public AttachmentType(string description, string key)
        {
            _attachments = new List<Attachment>();

            Description = description;
            Key = key;
        }

        public string Description { get; private set; }
        public string Key { get; private set; }


        private List<Attachment> _attachments;
        public IEnumerable<Attachment> Attachments => _attachments.AsReadOnly();
    }
}
