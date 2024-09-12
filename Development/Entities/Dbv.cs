namespace Vvoids.Api.Entities;

/// <summary>
/// One on One from Database View to C# Class Object.
/// </summary>
public class Dbv
{
    public class AuxPowerOverTimeByAuxEngineAmountBody
    {
        public double TotalPower { get; set; }
        public int AuxEnginesAmount { get; set; }
        public DateTime Timestamp { get; set; }
    }
    public class VesselData
    {
        public int VesselParticularsID { get; set; }
        public int? MaxSpeed { get; set; }
        public int? Length { get; set; }
        public int? Beam { get; set; }
        public int? Depth { get; set; }
        public int? Draft { get; set; }
        public int? CIITypeID { get; set; }
        public int? DWT { get; set; }
        public int? GT { get; set; }
        public int? VolumetricDisplacement { get; set; }
        public int? CoefficientBlock { get; set; }
        public int? CoefficientMidship { get; set; }
        public int? CoefficientWaterplane { get; set; }
        public int? WetArea { get; set; }
        public int? AppendagesWetArea { get; set; }
        public int? Trim { get; set; }
        public int? FormFactor { get; set; }
        public int? AppendagesFormFactor { get; set; }
        public int? AngleEntrance { get; set; }
        public int? VerticalDistance { get; set; }
        public int? TransverseSectionAreaImmersed { get; set; }
        public int? TransverseRatio { get; set; }
        public int? CoefficientWind { get; set; }
        public int? EfficiencyHull { get; set; }
        public int? EfficiencyPropeller { get; set; }
        public int? EfficiencyShaft { get; set; }
        public int? LimitWindSpeedService { get; set; }
        public int? LimitWaveHeightService { get; set; }
        public int? LimitWindSpeedCruise { get; set; }
        public int? LimitWaveHeightCruise { get; set; }
        public int? MaxTurningAngle { get; set; }
        public int? TurningRadiusNM { get; set; }
        public int? UKC { get; set; }
        public int? MinSpeedCruise { get; set; }
        public int? MaxSpeedCruise { get; set; }
        public int? MinSpeedService { get; set; }
        public int? MaxSpeedService { get; set; }
        public int? MinPowerService { get; set; }
        public int? MaxPowerService { get; set; }
        public int? MinRPMService { get; set; }
        public int? MaxRPMService { get; set; }
        public string PST { get; set; }
        public Guid VesselGUID { get; set; }
        public int VesselID { get; set; }
        public int VesselServiceTypeID { get; set; }
        public int ManagerID { get; set; }
        public int? ResponsibleID { get; set; }
        public int? ChartagentTeamID { get; set; }
        public bool? VesselValidationAllowed { get; set; }
        public int? VesselTypeID { get; set; }
        public int VesselNumber { get; set; }
        public string VesselName { get; set; }
        public int? IMONumber { get; set; }
        public int? MMSI { get; set; }
        public int? CountryID { get; set; }
        public string Email { get; set; }
        public int? LanguageID { get; set; }
        public int? Reference { get; set; }
        public int? OraID { get; set; }
        public string Name { get; set; }
        public string LongName { get; set; }
        public int? ISOCode { get; set; }
        public string ISOShort { get; set; }
        public string ISOLong { get; set; }
        public int? UNCode { get; set; }
        public string Capital { get; set; }
        public string FlagIcon { get; set; }
        public DateTime? LastSeenDateTime { get; set; }
        public string VesselTypeName { get; set; }
    }
    public class Timerange
    {
        public DateTimeOffset Minimum { get; set; }
        public DateTimeOffset Maximum { get; set; }
    }
    public class LastRefined
    {
        public DateTimeOffset Timestamp { get; set; }
        public int Number { get; set; }
    }
    public class Vessel
    {
        public Guid VesselId { get; set; }
    }
    public class PassageRouteWaypoint
    {
        public Guid PassageRouteId { get; set; }
        public Guid VesselId { get; set; }
        public bool IsGreatCircle { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public int Number { get; set; }
        public float DistanceToNext { get; set; }
        public float DistanceToEnd { get; set; }
        public int Bearing { get; set; }
        public int Speed { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
    public class VesselWaypoint
    {
        public Guid VesselId { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public int Bearing { get; set; }
        public int Heading { get; set; }
        public int Speed { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}