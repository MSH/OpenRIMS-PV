using System.ComponentModel;

namespace OpenRIMS.PV.Main.Core.ValueTypes
{
    public enum AdverseEventCriteria
    {
        [Description("Report Source")]
        ReportSource = 1,
        [Description("MedDRA Terminology")]
        MedDRATerminology = 2
    }

    public enum AdverseEventStratifyCriteria
    {
        AgeGroup = 1,
        Facility = 2,
        Drug = 3,
        Cohort = 4,
        FacilityRegion = 5,
        Outcome = 6,
        Gender = 7,
        Regimen = 8,
        IsSerious = 9,
        Seriousness = 10,
        Classification = 11
    }

    public enum AgeGroupCriteria
    {
        None = 0,
        ZeroToFour = 1,
        FiveToFourteen = 2,
        FifteenToTwentyFour = 3,
        TwentyFiveToThirtyFour = 4,
        ThirtyFiveToFortyFour = 5,
        FortyFiveToFivetyFour = 6,
        FifetyFiveToSixtyFour = 7,
        AboveSixtyFour = 8
    }
}
