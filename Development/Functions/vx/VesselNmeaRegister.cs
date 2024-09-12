using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Vvoids.Api.Base;
using Vvoids.Api.Entities;
using Vvoids.Api.Services;
using static Vvoids.Api.Entities.Dbx.NmeaSentence;

namespace Vvoids.Api.Functions;

public class VesselNmeaRegister(ILogger<VesselNmeaRegister> logger)
{
    public ILogger ILog = logger;

    public class Body
    {
        public Guid VesselId { get; set; }
    }

    [Function(nameof(VesselNmeaRegister))]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, HttpMethodLite.POST, Route = "v1/vessel/nmea/register")] HttpRequest request)
    {
        Properties<Body> props = new Properties<Body>(await Task.FromResult(ILog), request);

        string nmea = await props.Services.Specific.UseExternApi<string>("https://ctdashboardsprototya911.blob.core.windows.net/assets/nmea/output.nmea");

        if (nmea.IsUndefined())
        {
            return props.Services.Reply.Create(HttpsStatus.Error);
        }

        List<string> nmeaSentences = nmea.GetValidNmeaSentences();

        if (nmeaSentences.IsUndefined())
        {
            return props.Services.Reply.Create(HttpsStatus.Error);
        }

        List<(NmeaSentenceCategory, Dbo.VesselLocation)> nmeaObjects = new List<(NmeaSentenceCategory, Dbo.VesselLocation)>();
        Dbo.VesselLocation lastGPRMC = null;
        foreach (string sentence in nmeaSentences)
        {
            NmeaSentenceCategory category = Enum.Parse<NmeaSentenceCategory>(sentence[1..sentence.IndexOf(',')]);
            if (lastGPRMC.IsUndefined())
            {
                if (category != NmeaSentenceCategory.GPRMC)
                {
                    continue;
                }

                Dbo.VesselLocation result = GPRMC(sentence);
                if (!result.IsUndefined())
                {
                    nmeaObjects.Add((category, result));
                    lastGPRMC = result;
                    continue;
                }
            }

            Dbo.VesselLocation nmeaResult = default;
            switch (category)
            {
                //Maybe useless for ChartTrack up to 2024/08
                case NmeaSentenceCategory.GPGSA:
                case NmeaSentenceCategory.GPGSV:
                case NmeaSentenceCategory.PGRMM:
                case NmeaSentenceCategory.GPWPL:
                case NmeaSentenceCategory.PGRME:
                case NmeaSentenceCategory.PGRMZ:
                case NmeaSentenceCategory.GPRTE:
                    continue;
                //$GPVTG, , , , ,groundSpeedKnots, ,groundSpeedKilometersPerHour, , checksum
                case NmeaSentenceCategory.GPVTG:
                //$GPBOD,trueBearingDegrees, ,magneticBearingDegrees, ,distanceToNextNauticalMiles, , , checksum
                case NmeaSentenceCategory.GPBOD:
                //$GPRMB,required=A,distanceMetersOffCourse, , ,trueBearingDegrees, ,groundSpeedKnots,arrivalEstHours, checksum
                case NmeaSentenceCategory.GPRMB:
                    //Useless for ChartTrack up to 2024/08 due to absense of DateTime unless this is from same NMEA file?
                    continue;
                //Location related sentences
                case NmeaSentenceCategory.GPGLL:
                case NmeaSentenceCategory.GPGGA:
                    //Useless for ChartTrack up to 2024/08 due to absense of Date unless this is from same NMEA file?
                    continue;
                case NmeaSentenceCategory.GPRMC:
                    nmeaResult = GPRMC(sentence);
                    lastGPRMC = nmeaResult;
                    break;
                default:
                    props.Services.Log.Database(LogLevel.Warning, 202408011250, nameof(VesselNmeaRegister), $"NMEA Category '{sentence[..sentence.IndexOf(',')]}' is unhandled");
                    continue;
            }

            if (!nmeaResult.IsUndefined())
            {
                nmeaObjects.Add((category, nmeaResult));
                continue;
            }
        }

        //nmeaObjects.ForEach(x => x.Item2.VesselGUID = props.Request.Body.VesselId);

        props.OverwriteActiveSqlString(Settings.ChartTrackDevConnectionString);

        if (!await props.Services.Query.InsertElements(nmeaObjects.Select(x => x.Item2).ToArray(), new[] { nameof(Dbo.VesselLocation.VesselLocationID) }))
        {
            props.Services.Log.Database(LogLevel.Error, 202408191048, nameof(VesselNmeaRegister), $"NMEA failed to register");
            return props.Services.Reply.Create(HttpsStatus.Error);
        }

        if (props.Services.Query.Execute($@"WITH Duplicates AS (SELECT Id, VesselId, Latitude, Longitude, Timestamp, ROW_NUMBER() OVER (PARTITION BY Latitude, Longitude, Timestamp, VesselId ORDER BY Id) AS RowNumber FROM dbo.NmeaData) DELETE FROM Duplicates WHERE RowNumber > 1", false).Error)
        {
            return props.Services.Reply.Create(HttpsStatus.Error);
        }

        return props.Services.Reply.Create(HttpsStatus.Success);
    }
}