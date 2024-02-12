using OpenRIMS.PV.BuildingBlocks.EventBus.Events;
using System;
using System.Text.Json.Serialization;

namespace OpenRIMS.PV.Main.API.Application.IntegrationEvents.Events
{
    public record PatientAddedIntegrationEvent : IntegrationEvent
    {
        [JsonInclude]
        public int Id { get; private init; }

        [JsonInclude]
        public int Episode_Id { get; private init; }

        [JsonInclude]
        public Guid Patient_Guid { get; private init; }

        [JsonInclude]
        public string Regimen { get; private init; }

        [JsonInclude]
        public int Registration_Id { get; private init; }

        [JsonInclude]
        public int Registration_No { get; private init; }

        [JsonInclude]
        public string Registration_Type { get; private init; }

        [JsonInclude]
        public string Treatment_Start_Date { get; private init; }

        [JsonInclude]
        public string Date_Of_Birth { get; private init; }

        [JsonInclude]
        public string Employment_Status { get; private init; }

        [JsonInclude]
        public string Facility_Name { get; private init; }

        [JsonInclude]
        public string First_Name { get; private init; }

        [JsonInclude]
        public string Last_Name { get; private init; }

        [JsonInclude]
        public string Gender { get; private init; }

        [JsonInclude]
        public string Marital_Status { get; private init; }

        [JsonInclude]
        public string Medical_Record_Number { get; private init; }

        [JsonInclude]
        public string Medical_Record_Number_Type { get; private init; }

        [JsonInclude]
        public string Middle_Name { get; private init; }

        [JsonInclude]
        public string Notes { get; private init; }

        [JsonInclude]
        public string Occupation { get; private init; }

        [JsonInclude]
        public string Patient_Identity_Number { get; private init; }

        [JsonInclude]
        public string Patient_Identity_Type { get; private init; }

    }
}