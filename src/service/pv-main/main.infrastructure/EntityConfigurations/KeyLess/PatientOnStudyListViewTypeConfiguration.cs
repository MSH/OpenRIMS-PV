﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OpenRIMS.PV.Main.Core.Entities.Keyless;

namespace OpenRIMS.PV.Main.Infrastructure.EntityConfigurations.KeyLess
{
    class PatientOnStudyListViewTypeConfiguration : IEntityTypeConfiguration<PatientOnStudyList>
    {
        public void Configure(EntityTypeBuilder<PatientOnStudyList> configuration)
        {
            configuration.ToView("vwPatientOnStudyVisitList");

            configuration.HasNoKey();
        }
    }
}
