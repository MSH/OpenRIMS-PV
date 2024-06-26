﻿using System.Collections.Generic;

namespace OpenRIMS.PV.Main.API.Models
{
    public class PatientCustomAttributesForUpdateDto
    {
        /// <summary>
        /// Patient custom attributes and associated values
        /// </summary>
        public ICollection<AttributeValueForPostDto> Attributes { get; set; }
    }
}
