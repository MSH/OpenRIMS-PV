namespace OpenRIMS.PV.Main.Core.Entities
{
    public class DatasetMappingValue : EntityBase
    {
        public DatasetMappingValue()
        {
            Active = true;
        }

        public string SourceValue { get; set; }
        public string DestinationValue { get; set; }
        public bool Active { get; set; }
        public int MappingId { get; set; }
        public int? SubMappingId { get; set; }

        public virtual DatasetMapping Mapping { get; set; }
        public virtual DatasetMappingSub SubMapping { get; set; }
    }
}