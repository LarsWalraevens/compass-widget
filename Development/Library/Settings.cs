using Microsoft.Extensions.Logging;
using Vvoids.Api.Configuration;
using Vvoids.Api.Entities;

namespace Vvoids.Api.Services;

/// <summary>
/// This is the main config file of the API, some elements are hidden and need to be stored elsewhere, the hidden elements are summarized and retrieved using <see cref="Tags"/>.
/// </summary>
public class Settings : Options.Required.Method.ISettings
{
    public enum LogAction
    {
        LOG = 0,
        EMAIL,
        EMAIL_CLICK
    }

    public class Tvos
    {
        public enum Meteo
        {
            None = 0,
            Wind = 1,
            Pressure = 2,
            Precipitation = 4,
            Temperature = 8,
            Waves = 16,
            Currents = 512,
            Swell = 8192,
            WindWave = 16384
        }
    }

    public static string ChartTrackCTIConnectionString => Extensions.CreateSqlString("sptest-dbserver.database.windows.net", "CTI", "CTApp", "BogerdM!123");
    public static string ChartTrackDevConnectionString => Extensions.CreateSqlString("charttrackdev.database.windows.net", "charttrackdev", "ctadmin", "Charttrackadmin1!");
    public static string ChartTrackLNTestConnectionString => Extensions.CreateSqlString("sql01.charttrack.com", "LNTest", "CTApp", "BogerdM!123");

    public void LogToDatabase(Properties<object> props, LogLevel severity, string message, string action, string location)
    {
        string copyConnectionString = new string(props.Database.SqlStringActive);

        props.Database.OverwriteActiveSqlString(ChartTrackDevConnectionString);

        props.Services.Query.Execute($@"INSERT INTO dbo.[ErrorLog] ([PersonID],[Message],[Detail]) VALUES ({Guid.Empty.ToString().Replace("00000000-", "01234567-")}, {severity}, {message})", false);

        props.Database.OverwriteActiveSqlString(copyConnectionString);
    }
    public void LogMailToDatabase(Properties<object> props, (string email, Guid id) receiver, (string subject, string html, Guid id) content, (Guid id, string status) azure, string location) { }

    public static List<Dbe.RefinedWaypoint> SelectWaypointsByInterval(List<Dbe.RefinedWaypoint> waypoints, TimeSpan interval)
    {
        if (waypoints.IsUndefined()) { return new List<Dbe.RefinedWaypoint>(); }

        List<Dbe.RefinedWaypoint> selectedWaypoints = new List<Dbe.RefinedWaypoint>();
        selectedWaypoints.Add(waypoints[byte.MinValue]);
        DateTimeOffset lastSelectedTime = waypoints[byte.MinValue].Timestamp;

        while (true)
        {
            DateTimeOffset targetTime = lastSelectedTime + interval;

            Dbe.RefinedWaypoint closestWaypoint = waypoints.Where(wp => wp.Timestamp > lastSelectedTime).OrderBy(wp => Math.Abs((wp.Timestamp - targetTime).Ticks)).FirstOrDefault();

            if (closestWaypoint.IsUndefined()) { break; }

            selectedWaypoints.Add(closestWaypoint);
            lastSelectedTime = closestWaypoint.Timestamp;
        }

        return selectedWaypoints;
    }
    public static List<Dbv.PassageRouteWaypoint> SelectWaypointsByInterval(List<Dbv.PassageRouteWaypoint> waypoints, TimeSpan interval)
    {
        if (waypoints.IsUndefined()) { return new List<Dbv.PassageRouteWaypoint>(); }

        List<Dbv.PassageRouteWaypoint> selectedWaypoints = new List<Dbv.PassageRouteWaypoint>();
        selectedWaypoints.Add(waypoints[byte.MinValue]);
        DateTimeOffset lastSelectedTime = waypoints[byte.MinValue].Timestamp;

        while (true)
        {
            DateTimeOffset targetTime = lastSelectedTime + interval;

            Dbv.PassageRouteWaypoint closestWaypoint = waypoints.Where(wp => wp.Timestamp > lastSelectedTime).OrderBy(wp => Math.Abs((wp.Timestamp - targetTime).Ticks)).FirstOrDefault();

            if (closestWaypoint.IsUndefined()) { break; }

            selectedWaypoints.Add(closestWaypoint);
            lastSelectedTime = closestWaypoint.Timestamp;
        }

        return selectedWaypoints;
    }
}