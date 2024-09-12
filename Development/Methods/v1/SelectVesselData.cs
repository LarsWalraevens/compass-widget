using Microsoft.Extensions.Logging;
using Vvoids.Api.Base;
using Vvoids.Api.Entities;
using Vvoids.Api.Services;

namespace Vvoids.Api.Functions.Methods;

public partial class Methods
{
    public class SelectVesselDataBody
    {
        public string VesselId { get; set; }
    }

    public async Task<SelectVesselDataResult> SelectVesselData(ILogger iLog, SelectVesselDataBody body)
    {
        Properties<object> props = new Properties<object>(await Task.FromResult(iLog));

        if (body.VesselId.IsUndefined())
        {
            return new SelectVesselDataResult() { Condition = HttpsStatus.MissingParameters };
        }

        props.Database.OverwriteActiveSqlString(Settings.ChartTrackLNTestConnectionString);

        HttpsData<Dbv.VesselData> data = props.Services.Query.GetRecord<Dbv.VesselData>($@"SELECT dbo.Country.Name, dbo.Country.LongName, dbo.Country.ISOCode, dbo.Country.ISOShort, dbo.Country.ISOLong, dbo.Country.UNCode, dbo.Country.Capital, dbo.VesselParticulars.VesselParticularsID, dbo.VesselParticulars.MaxSpeed, dbo.VesselParticulars.Length, dbo.VesselParticulars.Beam, dbo.VesselParticulars.Depth, dbo.VesselParticulars.Draft, dbo.VesselParticulars.CIITypeID, dbo.VesselParticulars.DWT, dbo.VesselParticulars.GT, dbo.VesselParticulars.VolumetricDisplacement, dbo.VesselParticulars.CoefficientBlock, dbo.VesselParticulars.CoefficientMidship, dbo.VesselParticulars.CoefficientWaterplane, dbo.VesselParticulars.WetArea, dbo.VesselParticulars.AppendagesWetArea, dbo.VesselParticulars.Trim, dbo.VesselParticulars.FormFactor, dbo.VesselParticulars.AppendagesFormFactor, dbo.VesselParticulars.AngleEntrance, dbo.VesselParticulars.VerticalDistance, dbo.VesselParticulars.TransverseSectionAreaImmersed, dbo.VesselParticulars.TransverseRatio, dbo.VesselParticulars.CoefficientWind, dbo.VesselParticulars.EfficiencyHull, dbo.VesselParticulars.EfficiencyPropeller, dbo.VesselParticulars.EfficiencyShaft, dbo.VesselParticulars.LimitWindSpeedService, dbo.VesselParticulars.LimitWaveHeightService, dbo.VesselParticulars.LimitWindSpeedCruise, dbo.VesselParticulars.LimitWaveHeightCruise, dbo.VesselParticulars.MaxTurningAngle, dbo.VesselParticulars.TurningRadiusNM, dbo.VesselParticulars.UKC, dbo.VesselParticulars.MinSpeedCruise, dbo.VesselParticulars.MaxSpeedCruise, dbo.VesselParticulars.MinSpeedService, dbo.VesselParticulars.MaxSpeedService, dbo.VesselParticulars.MinPowerService, dbo.VesselParticulars.MaxPowerService, dbo.VesselParticulars.MinRPMService, dbo.VesselParticulars.MaxRPMService, dbo.VesselParticulars.PST, dbo.Vessel.VesselGUID, dbo.Vessel.VesselID, dbo.Vessel.VesselServiceTypeID, dbo.Vessel.ManagerID, dbo.Vessel.VesselTypeID, dbo.Vessel.ResponsibleID, dbo.Vessel.ChartagentTeamID, dbo.Vessel.VesselValidationAllowed, dbo.VesselType.Caption AS VesselTypeName, dbo.Vessel.VesselNumber, dbo.Vessel.VesselName, dbo.Vessel.IMONumber, dbo.Vessel.MMSI, dbo.Vessel.CountryID, dbo.Vessel.Email, dbo.Vessel.LanguageID, dbo.Vessel.Reference, dbo.Vessel.OraID, LastSeen.LogDate AS LastSeenDateTime FROM dbo.Vessel INNER JOIN dbo.VesselParticulars ON dbo.VesselParticulars.VesselID = dbo.Vessel.VesselID LEFT JOIN dbo.Country ON dbo.Country.CountryID = dbo.Vessel.CountryID LEFT JOIN dbo.VesselType ON dbo.VesselType.VesselTypeID = dbo.Vessel.VesselTypeID LEFT JOIN (SELECT MAX(LogDate) AS LogDate, VesselID FROM dbo.VesselLocation GROUP BY VesselID) AS LastSeen ON LastSeen.VesselID = dbo.Vessel.VesselID WHERE dbo.Vessel.VesselGUID = {body.VesselId}", true);

        if (data.Error)
        {
            return new SelectVesselDataResult() { Condition = HttpsStatus.Error };
        }
        if (data.HasNoData)
        {
            return new SelectVesselDataResult() { Condition = HttpsStatus.Success };
        }

        return new SelectVesselDataResult() { Condition = HttpsStatus.Success, Data = data.Data };
    }

    public class SelectVesselDataResult : Dbx.MethodResult
    {
        public Dbv.VesselData Data { get; set; }
    }
}