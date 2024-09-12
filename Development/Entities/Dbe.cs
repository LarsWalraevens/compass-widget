namespace Vvoids.Api.Entities;

/// <summary>
/// Add-on for <see cref="Dbo"/> or <see cref="Dbv"/> using inheritance.
/// </summary>
public class Dbe
{
    public class RefinedWaypoint
    {
        public RefinedWaypoint() { }
        public RefinedWaypoint(Dbv.VesselWaypoint waypoint)
        {
            VesselId = waypoint.VesselId;
            Latitude = waypoint.Latitude;
            Longitude = waypoint.Longitude;
            Bearing = waypoint.Bearing;
            Heading = waypoint.Heading;
            Speed = waypoint.Speed;
            Timestamp = waypoint.Timestamp;
            Number = byte.MinValue;
            IsAdded = false;
            RouteData = default;
            IsGreatCircle = false;
            DistanceToNext = byte.MinValue;
            DistanceToEnd = byte.MinValue;
            PassageRouteId = default;
        }
        public RefinedWaypoint(Dbv.PassageRouteWaypoint waypoint)
        {
            VesselId = waypoint.VesselId;
            Latitude = waypoint.Latitude;
            Longitude = waypoint.Longitude;
            Bearing = waypoint.Bearing;
            Heading = byte.MinValue;
            Speed = waypoint.Speed;
            Timestamp = waypoint.Timestamp;
            Number = waypoint.Number;
            IsAdded = false;
            RouteData = default;
            IsGreatCircle = waypoint.IsGreatCircle;
            DistanceToNext = waypoint.DistanceToNext;
            DistanceToEnd = waypoint.DistanceToEnd;
            PassageRouteId = waypoint.PassageRouteId;
        }

        public Guid? PassageRouteId { get; set; }
        public Guid VesselId { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public int Bearing { get; set; }
        public int? Heading { get; set; }
        public int? Speed { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public int Number { get; set; }
        public bool IsAdded { get; set; }
        public Dbx.Tvos.RouteData RouteData { get; set; }
        public bool? IsGreatCircle { get; set; }
        public float? DistanceToNext { get; set; }
        public float? DistanceToEnd { get; set; }
    }
}