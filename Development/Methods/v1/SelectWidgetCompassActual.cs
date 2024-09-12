using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;
using Vvoids.Api.Base;
using Vvoids.Api.Entities;
using Vvoids.Api.Services;

namespace Vvoids.Api.Functions.Methods;

public partial class Methods
{
    public class SelectWidgetCompassActualBody
    {
        public string VesselId { get; set; }
    }

    public async Task<SelectWidgetCompassActualResult> SelectWidgetCompassActual(ILogger iLog, HttpClient httpClient, SelectWidgetCompassActualBody body)
    {
        #region Checks
        Properties<object> props = new Properties<object>(await Task.FromResult(iLog));

        if (body.IsUndefined() || body.VesselId.IsUndefined() || httpClient.IsUndefined())
        {
            return new SelectWidgetCompassActualResult() { Condition = HttpsStatus.MissingParameters };
        }
        if (body.VesselId.TryConvertGuid().IsUndefined())
        {
            return new SelectWidgetCompassActualResult() { Details = "Vessel GUID is required", Condition = HttpsStatus.MissingParameters };
        }
        #endregion

        Dbx.Tvos.AccessToken token = default;
        #region Retrieve TVos Token
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.Token}");

        HttpResponseMessage httpResponseMessage = await httpClient.PostAsync($"https://auth.theyr.com/v1.3/License", new StringContent(JsonConvert.SerializeObject(new { key = "bogerd-martin-admin-5286-9348-5348-8047" }), Encoding.UTF8, "application/json"));
        HttpResponseMessage response = httpResponseMessage;
        string responseString = await response.Content.ReadAsStringAsync();
        #endregion
        token = JsonConvert.DeserializeObject<Dbx.Tvos.AccessToken>(responseString);
        if (token.IsUndefined() || token.Token.IsUndefined())
        {
            props.Services.Log.Error("No token");
            return new SelectWidgetCompassActualResult() { Condition = HttpsStatus.Error };
        }

        props.Database.OverwriteActiveSqlString(Settings.ChartTrackLNTestConnectionString);

        #region Get Actual Sailed Point
        HttpsData<Dbo.VesselLocation> sailedPoint = props.Services.Query.GetRecord<Dbo.VesselLocation>($@"SELECT TOP(1) dbo.VesselLocation.* FROM dbo.VesselLocation INNER JOIN dbo.Vessel ON dbo.Vessel.VesselID = dbo.VesselLocation.VesselID WHERE dbo.Vessel.VesselGUID={body.VesselId} ORDER BY dbo.VesselLocation.Timestamp DESC", true);

        if (sailedPoint.Error)
        {
            return new SelectWidgetCompassActualResult() { Condition = HttpsStatus.Error };
        }
        if (sailedPoint.HasNoData)
        {
            return new SelectWidgetCompassActualResult() { Details = "There is no data yet for this Vessel", Condition = HttpsStatus.Success };
        }
        #endregion
        Dbo.RefinedWaypoint sailedConvertedPoint = new Dbo.RefinedWaypoint()
        {
            Bearing = sailedPoint.Data.Course,
            Heading = sailedPoint.Data.Heading,
            Latitude = (float)sailedPoint.Data.Lat,
            Longitude = (float)sailedPoint.Data.Long,
            RecordChanged = sailedPoint.Data.RecordChanged,
            RecordCreated = sailedPoint.Data.RecordCreated,
            Speed = sailedPoint.Data.Speed,
            VesselId = body.VesselId.TryConvertGuid(),
            Timestamp = sailedPoint.Data.LogDate,
            IsOriginal = true
        };

        #region Get Actual Planned Point
        HttpsData<Dbo.RefinedWaypoint> plannedPoint = props.Services.Query.GetRecord<Dbo.RefinedWaypoint>($@"SELECT TOP(1) * FROM dbo.RefinedWaypoint WHERE VesselId={body.VesselId} AND PassageRouteId IS NOT NULL ORDER BY ABS(DATEDIFF(SECOND, Timestamp, {sailedPoint.Data.LogDate}))", true);

        if (plannedPoint.Error)
        {
            return new SelectWidgetCompassActualResult() { Condition = HttpsStatus.Error };
        }
        #endregion
        if (plannedPoint.HasData)
        {
            //Adjust timestamp minute based
            plannedPoint.Data.Timestamp = new DateTimeOffset(plannedPoint.Data.Timestamp.Year, plannedPoint.Data.Timestamp.Month, plannedPoint.Data.Timestamp.Day, plannedPoint.Data.Timestamp.Hour, 10 * ((plannedPoint.Data.Timestamp.Minute + 5) / 10) % 60, byte.MinValue, byte.MinValue, byte.MinValue, plannedPoint.Data.Timestamp.Offset);
        }

        Dbx.Tvos.RouteData geoSailed;
        #region Retrieve TVos Sailed
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.Token}");

        httpResponseMessage = await httpClient.PostAsync($"https://rdas.theyr.com/v2.2/RouteData", new StringContent(JsonConvert.SerializeObject(new
        {
            options = new
            {
                units = new
                {
                    direction = "degree",
                    speed = "kn",
                    height = "meter"
                }
            },
            points = new[] { new { lat = sailedConvertedPoint.Latitude, lon = sailedConvertedPoint.Longitude, t = sailedConvertedPoint.Timestamp.UtcDateTime } }
        }), Encoding.UTF8, "application/json"));
        response = httpResponseMessage;
        responseString = await response.Content.ReadAsStringAsync();
        #endregion
        geoSailed = JsonConvert.DeserializeObject<Dbx.Tvos.RouteData>(responseString);

        Dbx.Tvos.RouteData.Data geoDataSailed = geoSailed.DataPoints.First().Data;
        Dbo.GeoData geoConvertedDataSailed = new Dbo.GeoData()
        {
            Timestamp = sailedConvertedPoint.Timestamp.UtcDateTime,
            Latitude = (decimal)sailedConvertedPoint.Latitude,
            Longitude = (decimal)sailedConvertedPoint.Longitude,
            AirTemperatureHeight = (float)(geoDataSailed.AirTemperature.Values.Average(x => x.Height).IsUndefined() ? null : geoDataSailed.AirTemperature.Values.Average(x => x.Height)),
            AirTemperatureValue = (float)(geoDataSailed.AirTemperature.Values.Average(x => x.Value).IsUndefined() ? null : geoDataSailed.AirTemperature.Values.Average(x => x.Value)),
            IceFractionValue = (float)(geoDataSailed.IceFraction.Value.IsUndefined() ? null : geoDataSailed.IceFraction.Value),
            OceanCurrentDepth = (float)(geoDataSailed.OceanCurrent.Values.Average(x => x.Depth).IsUndefined() ? null : geoDataSailed.OceanCurrent.Values.Average(x => x.Depth)),
            OceanCurrentDirection = (float)(geoDataSailed.OceanCurrent.Values.Average(x => x.Direction).IsUndefined() ? null : geoDataSailed.OceanCurrent.Values.Average(x => x.Direction)),
            OceanCurrentValue = (float)(geoDataSailed.OceanCurrent.Values.Average(x => x.Speed).IsUndefined() ? null : geoDataSailed.OceanCurrent.Values.Average(x => x.Speed)),
            SeaFloorDepthValue = (float)(geoDataSailed.SeaFloorDepth.Value.IsUndefined() ? null : geoDataSailed.SeaFloorDepth.Value),
            SeaSurfaceSalinityValue = (float)(geoDataSailed.SeaSurfaceSalinity.Value.IsUndefined() ? null : geoDataSailed.SeaSurfaceSalinity.Value),
            SeaSurfaceTemperatureValue = (float)(geoDataSailed.SeaSurfaceTemperature.Value.IsUndefined() ? null : geoDataSailed.SeaSurfaceTemperature.Value),
            StandardMeteorologyMeanSeaLevelPressure = (float)(geoDataSailed.StandardMeteorology.MeanSeaLevelPressure.IsUndefined() ? null : geoDataSailed.StandardMeteorology.MeanSeaLevelPressure),
            StandardMeteorologyRelativeHumidity = (float)(geoDataSailed.StandardMeteorology.RelativeHumidity.IsUndefined() ? null : geoDataSailed.StandardMeteorology.RelativeHumidity),
            TideDirection = (float)(geoDataSailed.Tide.Direction.IsUndefined() ? null : geoDataSailed.Tide.Direction),
            TideHeight = (float)(geoDataSailed.Tide.Height.IsUndefined() ? null : geoDataSailed.Tide.Height),
            TideSpeed = (float)(geoDataSailed.Tide.Speed.IsUndefined() ? null : geoDataSailed.Tide.Speed),
            WaveSwellDirection = (float)(geoDataSailed.Wave.Swell.Direction.IsUndefined() ? null : geoDataSailed.Wave.Swell.Direction),
            WaveSwellMaximumWaveHeight = (float)(geoDataSailed.Wave.Swell.MaximumWaveHeight.IsUndefined() ? null : geoDataSailed.Wave.Swell.MaximumWaveHeight),
            WaveSwellMeanPeriod = (float)(geoDataSailed.Wave.Swell.MeanPeriod.IsUndefined() ? null : geoDataSailed.Wave.Swell.MeanPeriod),
            WaveSwellPeakDirection = (float)(geoDataSailed.Wave.Swell.PeakDirection.IsUndefined() ? null : geoDataSailed.Wave.Swell.PeakDirection),
            WaveSwellPeakPeriod = (float)(geoDataSailed.Wave.Swell.PeakPeriod.IsUndefined() ? null : geoDataSailed.Wave.Swell.PeakPeriod),
            WaveSwellSignificantWaveHeight = (float)(geoDataSailed.Wave.Swell.SignificantWaveHeight.IsUndefined() ? null : geoDataSailed.Wave.Swell.SignificantWaveHeight),
            WaveSwellZeroCrossingPeriod = (float)(geoDataSailed.Wave.Swell.ZeroCrossingPeriod.IsUndefined() ? null : geoDataSailed.Wave.Swell.ZeroCrossingPeriod),
            WaveTotalSeaDirection = (float)(geoDataSailed.Wave.TotalSea.Direction.IsUndefined() ? null : geoDataSailed.Wave.TotalSea.Direction),
            WaveTotalSeaMaximumWaveHeight = (float)(geoDataSailed.Wave.TotalSea.MaximumWaveHeight.IsUndefined() ? null : geoDataSailed.Wave.TotalSea.MaximumWaveHeight),
            WaveTotalSeaMeanPeriod = (float)(geoDataSailed.Wave.TotalSea.MeanPeriod.IsUndefined() ? null : geoDataSailed.Wave.TotalSea.MeanPeriod),
            WaveTotalSeaPeakDirection = (float)(geoDataSailed.Wave.TotalSea.PeakDirection.IsUndefined() ? null : geoDataSailed.Wave.TotalSea.PeakDirection),
            WaveTotalSeaPeakPeriod = (float)(geoDataSailed.Wave.TotalSea.PeakPeriod.IsUndefined() ? null : geoDataSailed.Wave.TotalSea.PeakPeriod),
            WaveTotalSeaSignificantWaveHeight = (float)(geoDataSailed.Wave.TotalSea.SignificantWaveHeight.IsUndefined() ? null : geoDataSailed.Wave.TotalSea.SignificantWaveHeight),
            WaveTotalSeaZeroCrossingPeriod = (float)(geoDataSailed.Wave.TotalSea.ZeroCrossingPeriod.IsUndefined() ? null : geoDataSailed.Wave.TotalSea.ZeroCrossingPeriod),
            WaveWindSeaDirection = (float)(geoDataSailed.Wave.WindSea.Direction.IsUndefined() ? null : geoDataSailed.Wave.WindSea.Direction),
            WaveWindSeaMaximumWaveHeight = (float)(geoDataSailed.Wave.WindSea.MaximumWaveHeight.IsUndefined() ? null : geoDataSailed.Wave.WindSea.MaximumWaveHeight),
            WaveWindSeaMeanPeriod = (float)(geoDataSailed.Wave.WindSea.MeanPeriod.IsUndefined() ? null : geoDataSailed.Wave.WindSea.MeanPeriod),
            WaveWindSeaPeakDirection = (float)(geoDataSailed.Wave.WindSea.PeakDirection.IsUndefined() ? null : geoDataSailed.Wave.WindSea.PeakDirection),
            WaveWindSeaPeakPeriod = (float)(geoDataSailed.Wave.WindSea.PeakPeriod.IsUndefined() ? null : geoDataSailed.Wave.WindSea.PeakPeriod),
            WaveWindSeaSignificantWaveHeight = (float)(geoDataSailed.Wave.WindSea.SignificantWaveHeight.IsUndefined() ? null : geoDataSailed.Wave.WindSea.SignificantWaveHeight),
            WaveWindSeaZeroCrossingPeriod = (float)(geoDataSailed.Wave.WindSea.ZeroCrossingPeriod.IsUndefined() ? null : geoDataSailed.Wave.WindSea.ZeroCrossingPeriod)
        };

        Dbx.Tvos.RouteData geoPlanned;
        #region Retrieve TVos Planned
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token.Token}");

        httpResponseMessage = await httpClient.PostAsync($"https://rdas.theyr.com/v2.2/RouteData", new StringContent(JsonConvert.SerializeObject(new
        {
            options = new
            {
                units = new
                {
                    direction = "degree",
                    speed = "kn",
                    height = "meter"
                }
            },
            points = new[] { new { lat = plannedPoint.Data.Latitude, lon = plannedPoint.Data.Longitude, t = plannedPoint.Data.Timestamp.UtcDateTime } }
        }), Encoding.UTF8, "application/json"));
        response = httpResponseMessage;
        responseString = await response.Content.ReadAsStringAsync();
        #endregion
        geoPlanned = JsonConvert.DeserializeObject<Dbx.Tvos.RouteData>(responseString);

        Dbx.Tvos.RouteData.Data geoDataPlanned = geoPlanned.DataPoints.First().Data;
        Dbo.GeoData geoConvertedDataPlanned = new Dbo.GeoData()
        {
            Timestamp = plannedPoint.Data.Timestamp.UtcDateTime,
            Latitude = (decimal)plannedPoint.Data.Latitude,
            Longitude = (decimal)plannedPoint.Data.Longitude,
            AirTemperatureHeight = (float)(geoDataPlanned.AirTemperature.Values.Average(x => x.Height).IsUndefined() ? null : geoDataPlanned.AirTemperature.Values.Average(x => x.Height)),
            AirTemperatureValue = (float)(geoDataPlanned.AirTemperature.Values.Average(x => x.Value).IsUndefined() ? null : geoDataPlanned.AirTemperature.Values.Average(x => x.Value)),
            IceFractionValue = (float)(geoDataPlanned.IceFraction.Value.IsUndefined() ? null : geoDataPlanned.IceFraction.Value),
            OceanCurrentDepth = (float)(geoDataPlanned.OceanCurrent.Values.Average(x => x.Depth).IsUndefined() ? null : geoDataPlanned.OceanCurrent.Values.Average(x => x.Depth)),
            OceanCurrentDirection = (float)(geoDataPlanned.OceanCurrent.Values.Average(x => x.Direction).IsUndefined() ? null : geoDataPlanned.OceanCurrent.Values.Average(x => x.Direction)),
            OceanCurrentValue = (float)(geoDataPlanned.OceanCurrent.Values.Average(x => x.Speed).IsUndefined() ? null : geoDataPlanned.OceanCurrent.Values.Average(x => x.Speed)),
            SeaFloorDepthValue = (float)(geoDataPlanned.SeaFloorDepth.Value.IsUndefined() ? null : geoDataPlanned.SeaFloorDepth.Value),
            SeaSurfaceSalinityValue = (float)(geoDataPlanned.SeaSurfaceSalinity.Value.IsUndefined() ? null : geoDataPlanned.SeaSurfaceSalinity.Value),
            SeaSurfaceTemperatureValue = (float)(geoDataPlanned.SeaSurfaceTemperature.Value.IsUndefined() ? null : geoDataPlanned.SeaSurfaceTemperature.Value),
            StandardMeteorologyMeanSeaLevelPressure = (float)(geoDataPlanned.StandardMeteorology.MeanSeaLevelPressure.IsUndefined() ? null : geoDataPlanned.StandardMeteorology.MeanSeaLevelPressure),
            StandardMeteorologyRelativeHumidity = (float)(geoDataPlanned.StandardMeteorology.RelativeHumidity.IsUndefined() ? null : geoDataPlanned.StandardMeteorology.RelativeHumidity),
            TideDirection = (float)(geoDataPlanned.Tide.Direction.IsUndefined() ? null : geoDataPlanned.Tide.Direction),
            TideHeight = (float)(geoDataPlanned.Tide.Height.IsUndefined() ? null : geoDataPlanned.Tide.Height),
            TideSpeed = (float)(geoDataPlanned.Tide.Speed.IsUndefined() ? null : geoDataPlanned.Tide.Speed),
            WaveSwellDirection = (float)(geoDataPlanned.Wave.Swell.Direction.IsUndefined() ? null : geoDataPlanned.Wave.Swell.Direction),
            WaveSwellMaximumWaveHeight = (float)(geoDataPlanned.Wave.Swell.MaximumWaveHeight.IsUndefined() ? null : geoDataPlanned.Wave.Swell.MaximumWaveHeight),
            WaveSwellMeanPeriod = (float)(geoDataPlanned.Wave.Swell.MeanPeriod.IsUndefined() ? null : geoDataPlanned.Wave.Swell.MeanPeriod),
            WaveSwellPeakDirection = (float)(geoDataPlanned.Wave.Swell.PeakDirection.IsUndefined() ? null : geoDataPlanned.Wave.Swell.PeakDirection),
            WaveSwellPeakPeriod = (float)(geoDataPlanned.Wave.Swell.PeakPeriod.IsUndefined() ? null : geoDataPlanned.Wave.Swell.PeakPeriod),
            WaveSwellSignificantWaveHeight = (float)(geoDataPlanned.Wave.Swell.SignificantWaveHeight.IsUndefined() ? null : geoDataPlanned.Wave.Swell.SignificantWaveHeight),
            WaveSwellZeroCrossingPeriod = (float)(geoDataPlanned.Wave.Swell.ZeroCrossingPeriod.IsUndefined() ? null : geoDataPlanned.Wave.Swell.ZeroCrossingPeriod),
            WaveTotalSeaDirection = (float)(geoDataPlanned.Wave.TotalSea.Direction.IsUndefined() ? null : geoDataPlanned.Wave.TotalSea.Direction),
            WaveTotalSeaMaximumWaveHeight = (float)(geoDataPlanned.Wave.TotalSea.MaximumWaveHeight.IsUndefined() ? null : geoDataPlanned.Wave.TotalSea.MaximumWaveHeight),
            WaveTotalSeaMeanPeriod = (float)(geoDataPlanned.Wave.TotalSea.MeanPeriod.IsUndefined() ? null : geoDataPlanned.Wave.TotalSea.MeanPeriod),
            WaveTotalSeaPeakDirection = (float)(geoDataPlanned.Wave.TotalSea.PeakDirection.IsUndefined() ? null : geoDataPlanned.Wave.TotalSea.PeakDirection),
            WaveTotalSeaPeakPeriod = (float)(geoDataPlanned.Wave.TotalSea.PeakPeriod.IsUndefined() ? null : geoDataPlanned.Wave.TotalSea.PeakPeriod),
            WaveTotalSeaSignificantWaveHeight = (float)(geoDataPlanned.Wave.TotalSea.SignificantWaveHeight.IsUndefined() ? null : geoDataPlanned.Wave.TotalSea.SignificantWaveHeight),
            WaveTotalSeaZeroCrossingPeriod = (float)(geoDataPlanned.Wave.TotalSea.ZeroCrossingPeriod.IsUndefined() ? null : geoDataPlanned.Wave.TotalSea.ZeroCrossingPeriod),
            WaveWindSeaDirection = (float)(geoDataPlanned.Wave.WindSea.Direction.IsUndefined() ? null : geoDataPlanned.Wave.WindSea.Direction),
            WaveWindSeaMaximumWaveHeight = (float)(geoDataPlanned.Wave.WindSea.MaximumWaveHeight.IsUndefined() ? null : geoDataPlanned.Wave.WindSea.MaximumWaveHeight),
            WaveWindSeaMeanPeriod = (float)(geoDataPlanned.Wave.WindSea.MeanPeriod.IsUndefined() ? null : geoDataPlanned.Wave.WindSea.MeanPeriod),
            WaveWindSeaPeakDirection = (float)(geoDataPlanned.Wave.WindSea.PeakDirection.IsUndefined() ? null : geoDataPlanned.Wave.WindSea.PeakDirection),
            WaveWindSeaPeakPeriod = (float)(geoDataPlanned.Wave.WindSea.PeakPeriod.IsUndefined() ? null : geoDataPlanned.Wave.WindSea.PeakPeriod),
            WaveWindSeaSignificantWaveHeight = (float)(geoDataPlanned.Wave.WindSea.SignificantWaveHeight.IsUndefined() ? null : geoDataPlanned.Wave.WindSea.SignificantWaveHeight),
            WaveWindSeaZeroCrossingPeriod = (float)(geoDataPlanned.Wave.WindSea.ZeroCrossingPeriod.IsUndefined() ? null : geoDataPlanned.Wave.WindSea.ZeroCrossingPeriod)
        };

        return new SelectWidgetCompassActualResult() { Condition = HttpsStatus.Success, Dot = new SelectWidgetCompassResult.DotData() { Direction = props.Services.Specific.CalculateBearing(plannedPoint.Data.Latitude, plannedPoint.Data.Longitude, sailedPoint.Data.Lat, sailedPoint.Data.Long), IsWarning = false }, Planned = new SelectWidgetCompassResult.CompassData() { Vessel = plannedPoint.Data, Geo = geoConvertedDataPlanned }, Sailed = new SelectWidgetCompassResult.CompassData() { Vessel = sailedConvertedPoint, Geo = geoConvertedDataSailed } };
    }

    public class SelectWidgetCompassActualResult : SelectWidgetCompassResult { }
}