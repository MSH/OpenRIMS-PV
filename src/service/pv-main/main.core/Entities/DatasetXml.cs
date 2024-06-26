﻿using OpenRIMS.PV.Main.Core.Aggregates.DatasetAggregate;
using System.Collections.Generic;

namespace OpenRIMS.PV.Main.Core.Entities
{
    public class DatasetXml : AuditedEntityBase
    {
        public DatasetXml()
        {
            Datasets = new HashSet<Dataset>();
            ChildrenNodes = new HashSet<DatasetXmlNode>();
        }

        public string Description { get; set; }

        public virtual ICollection<Dataset> Datasets { get; set; }
        public virtual ICollection<DatasetXmlNode> ChildrenNodes { get; set; }
    }
}