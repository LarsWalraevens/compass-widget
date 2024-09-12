using Newtonsoft.Json;
using System.Globalization;
using Vvoids.Api.Base;
using Vvoids.Api.Services;
using static Vvoids.Api.Entities.OpenAi;

namespace Vvoids.Api.Entities;

// Internal and custom classes
public class Dbx
{
    public class MethodResult
    {
        public HttpsStatus Condition { get; set; }
        public string Details { get; set; }
    }

    public class OpenAi
    {
        public class Structure
        {
            public class Eml<OpenSchema>
            {
                public StructuredField mail_from = new StructuredField { type = StructuredField.FieldType.text };
                public StructuredField mail_sent_by = new StructuredField { type = StructuredField.FieldType.text };
                public StructuredField mail_to = new StructuredField { type = StructuredField.FieldType.text };
                public StructuredField mail_subject = new StructuredField { type = StructuredField.FieldType.text };
                [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "<Pending>")]
                public SchemeTemplate<OpenSchema> mail_body => new SchemeTemplate<OpenSchema>();
            }
            public class MarantPositionReport
            {
                public StructuredField date = new StructuredField { type = StructuredField.FieldType.text };
                public StructuredField from = new StructuredField { type = StructuredField.FieldType.text };
                public StructuredField to = new StructuredField { type = StructuredField.FieldType.text };
                public StructuredField latitude = new StructuredField { type = StructuredField.FieldType.text };
                public StructuredField longitude = new StructuredField { type = StructuredField.FieldType.text };
                public StructuredField eta = new StructuredField { type = StructuredField.FieldType.text };
                public StructuredField total_distance = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField total_distance_to_go = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField speed = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField course = new StructuredField { type = StructuredField.FieldType.text };
                public StructuredField wind = new StructuredField { type = StructuredField.FieldType.text };
                public StructuredField swell = new StructuredField { type = StructuredField.FieldType.text };
                public StructuredField cargo_on_board = new StructuredField { type = StructuredField.FieldType.text };
                public StructuredField ballast_on_board = new StructuredField { type = StructuredField.FieldType.text };
                public StructuredField remarks = new StructuredField { type = StructuredField.FieldType.text };
                public StructuredField mgo_lo_fw_rob_mgo_mt = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField mgo_lo_fw_me_lo_l = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField mgo_lo_fw_ae_lo_l = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField mgo_lo_fw_fw_mt = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField mgo_lo_fw_gb_lo_l = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField mgo_lo_fw_rob_mgo_mt_exp = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField mgo_lo_fw_me_lo_l_exp = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField mgo_lo_fw_ae_lo_l_exp = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField mgo_lo_fw_fw_mt_exp = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField mgo_lo_fw_gb_lo_l_exp = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField consumption_mgo_mt = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField consumption_lo_l = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField rpm_me = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField shaft_rpm = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField fuel_rack = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField governor = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField exhaust_temperatures_t1 = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField exhaust_temperatures_t2 = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField exhaust_temperatures_t3 = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField exhaust_temperatures_t4 = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField exhaust_temperatures_t5 = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField exhaust_temperatures_t6 = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField exhaust_temperatures_t7 = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField exhaust_temperatures_t8 = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField temperature_before_turbo = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField temperature_after_turbo = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField cylinder_cooling_water_in = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField cylinder_cooling_water_out = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField lo_in = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField lo_out = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField oil_in = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField oil_out = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField gb_oil_in = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField gb_oil_out = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField gb_cw_in = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField gb_cw_out = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField charge_air_pressure_temperature = new StructuredField { type = StructuredField.FieldType.text };
                public StructuredField cw_p = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField lo_p = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField fuel_p = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField engine_room_temperature = new StructuredField { type = StructuredField.FieldType.number };
                public StructuredField sea_water_temperature = new StructuredField { type = StructuredField.FieldType.number };
            }
        }
    }

    public class SeaVision
    {
        public class AccessToken
        {
            public string Token { get; set; }
        }

        public class EntityData
        {
            public List<Entity> Data { get; set; }
            public bool HasNext { get; set; }

            public class EntityId
            {
                public string EntityType { get; set; }
                public string Id { get; set; }
            }
            public class Entity
            {
                public EntityId Id { get; set; }
                public long CreatedTime { get; set; }
                public object AdditionalInfo { get; set; }
                public EntityId TenantId { get; set; }
                public EntityId CustomerId { get; set; }
                public string Name { get; set; }
                public string Type { get; set; }
                public string Label { get; set; }
                public object AssetProfileId { get; set; }
                public object ExternalId { get; set; }
                public EntityId OwnerId { get; set; }
            }
        }
        public class CustomerData
        {
            public List<Customer> Data { get; set; }

            public class EntityId
            {
                public string EntityType { get; set; }
                public string Id { get; set; }
            }

            public class Customer
            {
                public EntityId Id { get; set; }
                public string Name { get; set; }
            }
        }
        public class TelemetryData
        {
            public class DataPointTimestamp
            {
                [JsonProperty("ts")]
                public long Timestamp { get; set; }
            }
            public class DataPointJsonString : DataPointTimestamp
            {
                public object ValueData;
                private string _value;
                public string Value
                {
                    get { return _value; }
                    set
                    {
                        _value = value;
                        try
                        {
                            ValueData = JsonConvert.DeserializeObject(value);
                        }
                        catch (Exception)
                        {
                            ValueData = value;
                            throw;
                        }
                    }
                }
            }
            public class DataPointString : DataPointTimestamp
            {
                public string Value { get; set; }
            }
            public class DataPointInt : DataPointTimestamp
            {
                public long Value { get; set; }
            }
            public class DataPointBoolean : DataPointTimestamp
            {
                public byte Value { get; set; }
            }
            public class DataPointDouble : DataPointTimestamp
            {
                public double Value { get; set; }
            }
            public class DataPointDateTime : DataPointTimestamp
            {
                public DateTime Value { get; set; }
            }

            [JsonProperty("ais-other")] public List<DataPointString> AutomaticIdentificationSystemOther { get; set; }
            [JsonProperty("ais-own-status")] public List<DataPointString> AutomaticIdentificationSystemOwnStatus { get; set; }
            [JsonProperty("ais-own-status-text")] public List<DataPointString> AutomaticIdentificationSystemOwnStatusText { get; set; }
            [JsonProperty("aux-blr-drum-steam")] public List<DataPointString> AuxiliaryBoilerDrumSteam { get; set; }
            [JsonProperty("beaufort")] public List<DataPointString> Beaufort { get; set; }
            [JsonProperty("boiler-fuel-flow")] public List<DataPointString> BoilerFuelFlow { get; set; }
            [JsonProperty("boiler-fuel-temp")] public List<DataPointString> BoilerFuelTemperature { get; set; }
            [JsonProperty("boost-air")] public List<DataPointString> BoostAir { get; set; }
            [JsonProperty("brg")] public List<DataPointString> Bearing { get; set; }
            [JsonProperty("consumption-boiler-mtph")] public List<DataPointString> ConsumptionBoilerMetricTonsPerHour { get; set; }
            [JsonProperty("consumption-dg-mtph")] public List<DataPointString> ConsumptionDieselGeneratorMetricTonsPerHour { get; set; }
            [JsonProperty("consumption-me-mtph")] public List<DataPointString> ConsumptionMainEngineMetricTonsPerHour { get; set; }
            [JsonProperty("currentDir")] public List<DataPointString> CurrentDirection { get; set; }
            [JsonProperty("currentSpeed")] public List<DataPointString> CurrentSpeed { get; set; }
            [JsonProperty("dg-fuel-flow")] public List<DataPointString> DieselGeneratorFuelFlow { get; set; }
            [JsonProperty("dg-fuel-temp")] public List<DataPointString> DieselGeneratorFuelTemperature { get; set; }
            [JsonProperty("dg-in-fuel-flow")] public List<DataPointString> DieselGeneratorInletFuelFlow { get; set; }
            [JsonProperty("dg-in-fuel-temp")] public List<DataPointString> DieselGeneratorInletFuelTemperature { get; set; }
            [JsonProperty("dg-out-fuel-flow")] public List<DataPointString> DieselGeneratorOutletFuelFlow { get; set; }
            [JsonProperty("dg-out-fuel-temp")] public List<DataPointString> DieselGeneratorOutletFuelTemperature { get; set; }
            [JsonProperty("dg1-a")] public List<DataPointString> DieselGenerator1Ampere { get; set; }
            [JsonProperty("dg1-hz")] public List<DataPointString> DieselGenerator1Hertz { get; set; }
            [JsonProperty("dg1-kw")] public List<DataPointString> DieselGenerator1Kilowatt { get; set; }
            [JsonProperty("dg1-v")] public List<DataPointString> DieselGenerator1Volt { get; set; }
            [JsonProperty("dg2-a")] public List<DataPointString> DieselGenerator2Ampere { get; set; }
            [JsonProperty("dg2-hz")] public List<DataPointString> DieselGenerator2Hertz { get; set; }
            [JsonProperty("dg2-kw")] public List<DataPointString> DieselGenerator2Kilowatt { get; set; }
            [JsonProperty("dg2-v")] public List<DataPointString> DieselGenerator2Volt { get; set; }
            [JsonProperty("dg3-a")] public List<DataPointString> DieselGenerator3Ampere { get; set; }
            [JsonProperty("dg3-hz")] public List<DataPointString> DieselGenerator3Hertz { get; set; }
            [JsonProperty("dg3-kw")] public List<DataPointString> DieselGenerator3Kilowatt { get; set; }
            [JsonProperty("dg3-v")] public List<DataPointString> DieselGenerator3Volt { get; set; }
            [JsonProperty("fuel-oil")] public List<DataPointString> FuelOil { get; set; }
            [JsonProperty("gen1-boost-air")] public List<DataPointString> Generator1BoostAir { get; set; }
            [JsonProperty("gen1-fuel-oil")] public List<DataPointString> Generator1FuelOil { get; set; }
            [JsonProperty("gen1-high-temp-cw")] public List<DataPointString> Generator1HighTemperatureCoolingWater { get; set; }
            [JsonProperty("gen1-low-temp-cw")] public List<DataPointString> Generator1LowTemperatureCoolingWater { get; set; }
            [JsonProperty("gen1-lub-oil")] public List<DataPointString> Generator1LubricatingOil { get; set; }
            [JsonProperty("gen1-tacho-meter")] public List<DataPointString> Generator1Tachometer { get; set; }
            [JsonProperty("gen1-turbo-char-lub-oil")] public List<DataPointString> Generator1TurbochargerLubricatingOil { get; set; }
            [JsonProperty("hdg")] public List<DataPointString> Heading { get; set; }
            [JsonProperty("high-temp-cw")] public List<DataPointString> HighTemperatureCoolingWater { get; set; }
            [JsonProperty("lat")] public List<DataPointString> Latitude { get; set; }
            [JsonProperty("log")] public List<DataPointString> Logarithm { get; set; }
            [JsonProperty("lon")] public List<DataPointString> Longitude { get; set; }
            [JsonProperty("low-temp-cw")] public List<DataPointString> LowTemperatureCoolingWater { get; set; }
            [JsonProperty("lub-oil")] public List<DataPointString> LubricatingOil { get; set; }
            [JsonProperty("main-air-reserve-1")] public List<DataPointString> MainAirReserve1 { get; set; }
            [JsonProperty("main-air-reserve-2")] public List<DataPointString> MainAirReserve2 { get; set; }
            [JsonProperty("me-fo-cal-value-capture")] public List<DataPointString> MainEngineFuelOilCalorificValueCapture { get; set; }
            [JsonProperty("me-fo-gravity-capture")] public List<DataPointString> MainEngineFuelOilGravityCapture { get; set; }
            [JsonProperty("me-fo-in")] public List<DataPointString> MainEngineFuelOilInlet { get; set; }
            [JsonProperty("me-fo-sulfur-capture")] public List<DataPointString> MainEngineFuelOilSulfurCapture { get; set; }
            [JsonProperty("me-fuel-flow")] public List<DataPointString> MainEngineFuelFlow { get; set; }
            [JsonProperty("me-fuel-index")] public List<DataPointString> MainEngineFuelIndex { get; set; }
            [JsonProperty("me-fuel-index-capture")] public List<DataPointString> MainEngineFuelIndexCapture { get; set; }
            [JsonProperty("me-fuel-temp")] public List<DataPointString> MainEngineFuelTemperature { get; set; }
            [JsonProperty("me-hours-capture")] public List<DataPointString> MainEngineHoursCapture { get; set; }
            [JsonProperty("me-in-fuel-flow")] public List<DataPointString> MainEngineInletFuelFlow { get; set; }
            [JsonProperty("me-in-fuel-temp")] public List<DataPointString> MainEngineInletFuelTemperature { get; set; }
            [JsonProperty("me-jacket-cfw-in")] public List<DataPointString> MainEngineJacketCoolingFluidWaterInlet { get; set; }
            [JsonProperty("me-lo-in")] public List<DataPointString> MainEngineLubricatingOilInlet { get; set; }
            [JsonProperty("me-load")] public List<DataPointString> MainEngineLoad { get; set; }
            [JsonProperty("me-load-capture")] public List<DataPointString> MainEngineLoadCapture { get; set; }
            [JsonProperty("me-maneuv-air")] public List<DataPointString> MainEngineManeuveringAir { get; set; }
            [JsonProperty("me-pi-capture")] public List<DataPointString> MainEnginePressureIndicatorCapture { get; set; }
            [JsonProperty("me-pi1-capture")] public List<DataPointString> MainEnginePressureIndicator1Capture { get; set; }
            [JsonProperty("me-pi2-capture")] public List<DataPointString> MainEnginePressureIndicator2Capture { get; set; }
            [JsonProperty("me-pi3-capture")] public List<DataPointString> MainEnginePressureIndicator3Capture { get; set; }
            [JsonProperty("me-pi4-capture")] public List<DataPointString> MainEnginePressureIndicator4Capture { get; set; }
            [JsonProperty("me-pi5-capture")] public List<DataPointString> MainEnginePressureIndicator5Capture { get; set; }
            [JsonProperty("me-pi6-capture")] public List<DataPointString> MainEnginePressureIndicator6Capture { get; set; }
            [JsonProperty("me-piston-co-in")] public List<DataPointString> MainEnginePistonCoolingInlet { get; set; }
            [JsonProperty("me-pmax-capture")] public List<DataPointString> MainEnginePeakPressureCapture { get; set; }
            [JsonProperty("me-pmax1-capture")] public List<DataPointString> MainEnginePeakPressure1Capture { get; set; }
            [JsonProperty("me-pmax2-capture")] public List<DataPointString> MainEnginePeakPressure2Capture { get; set; }
            [JsonProperty("me-pmax3-capture")] public List<DataPointString> MainEnginePeakPressure3Capture { get; set; }
            [JsonProperty("me-pmax4-capture")] public List<DataPointString> MainEnginePeakPressure4Capture { get; set; }
            [JsonProperty("me-pmax5-capture")] public List<DataPointString> MainEnginePeakPressure5Capture { get; set; }
            [JsonProperty("me-pmax6-capture")] public List<DataPointString> MainEnginePeakPressure6Capture { get; set; }
            [JsonProperty("me-pme")] public List<DataPointString> MainEngineMeanEffectivePressure { get; set; }
            [JsonProperty("me-power")] public List<DataPointString> MainEnginePower { get; set; }
            [JsonProperty("me-power-capture")] public List<DataPointString> MainEnginePowerCapture { get; set; }
            [JsonProperty("me-prop-rpm")] public List<DataPointString> MainEnginePropellerRevolutionsPerMinute { get; set; }
            [JsonProperty("me-rev-counter")] public List<DataPointString> MainEngineRevolutionsCounter { get; set; }
            [JsonProperty("me-rpm")] public List<DataPointString> MainEngineRevolutionsPerMinute { get; set; }
            [JsonProperty("me-rpm-capture")] public List<DataPointString> MainEngineRevolutionsPerMinuteCapture { get; set; }
            [JsonProperty("me-rpm-set-capture")] public List<DataPointString> MainEngineRevolutionsPerMinuteSetCapture { get; set; }
            [JsonProperty("me-scav-air")] public List<DataPointString> MainEngineScavengeAir { get; set; }
            [JsonProperty("me-start-air")] public List<DataPointString> MainEngineStartingAir { get; set; }
            [JsonProperty("me-sw-air-clr-in")] public List<DataPointString> MainEngineSeaWaterAirCoolerInlet { get; set; }
            [JsonProperty("me-tc-lo-in")] public List<DataPointString> MainEngineTurbochargerLubricatingOilInlet { get; set; }
            [JsonProperty("me-torque")] public List<DataPointString> MainEngineTorque { get; set; }
            [JsonProperty("me-torque-capture")] public List<DataPointString> MainEngineTorqueCapture { get; set; }
            [JsonProperty("me-visco-capture")] public List<DataPointString> MainEngineViscosityCapture { get; set; }
            [JsonProperty("no1-dg-lo-in")] public List<DataPointString> DieselGenerator1LubricatingOilInlet { get; set; }
            [JsonProperty("no2-dg-lo-in")] public List<DataPointString> DieselGenerator2LubricatingOilInlet { get; set; }
            [JsonProperty("no3-dg-lo-in")] public List<DataPointString> DieselGenerator3LubricatingOilInlet { get; set; }
            [JsonProperty("rudder")] public List<DataPointString> Rudder { get; set; }
            [JsonProperty("sog")] public List<DataPointString> SpeedOverGround { get; set; }
            [JsonProperty("swell")] public List<DataPointString> Swell { get; set; }
            [JsonProperty("tacho-meter")] public List<DataPointString> Tachometer { get; set; }
            [JsonProperty("temperature")] public List<DataPointString> Temperature { get; set; }
            [JsonProperty("turbo-char-lub-oil")] public List<DataPointString> TurbochargerLubricatingOil { get; set; }
            [JsonProperty("turbo-rpm")] public List<DataPointString> TurbochargerRevolutionsPerMinute { get; set; }
            [JsonProperty("visibility")] public List<DataPointString> Visibility { get; set; }
            [JsonProperty("waveHeight")] public List<DataPointString> WaveHeight { get; set; }
            [JsonProperty("windDir")] public List<DataPointString> WindDirection { get; set; }
            [JsonProperty("windSpeed")] public List<DataPointString> WindSpeed { get; set; }
            [JsonProperty("me-rpm")]
            public List<DataPointString> MainEngineRpm { get; set; }
            [JsonProperty("turbo-rpm")]
            public List<DataPointString> TurboRpm { get; set; }
            [JsonProperty("me-prop-rpm")]
            public List<DataPointString> MainEnginePropellerRpm { get; set; }
            [JsonProperty("gyro-hdg")]
            public List<DataPointString> GyroHeading { get; set; }
            [JsonProperty("me-exhaust-temp")]
            public List<DataPointString> MainEngineExhaustTemperature { get; set; }
            [JsonProperty("gb-lo-in")]
            public List<DataPointString> GearboxLubricatingOilInlet { get; set; }
            [JsonProperty("1-kw")]
            public List<DataPointString> OneKilowatt { get; set; }
            [JsonProperty("2-kw")]
            public List<DataPointString> TwoKilowatt { get; set; }
            [JsonProperty("3-kw")]
            public List<DataPointString> ThreeKilowatt { get; set; }
            [JsonProperty("total-kw")]
            public List<DataPointString> TotalKilowatt { get; set; }
            [JsonProperty("1-v")]
            public List<DataPointString> OneVolt { get; set; }
            [JsonProperty("1-a")]
            public List<DataPointString> OneAmpere { get; set; }
            [JsonProperty("2-v")]
            public List<DataPointString> TwoVolt { get; set; }
            [JsonProperty("3-a")]
            public List<DataPointString> ThreeAmpere { get; set; }
            [JsonProperty("2-a")]
            public List<DataPointString> TwoAmpere { get; set; }
            [JsonProperty("3-v")]
            public List<DataPointString> ThreeVolt { get; set; }
            [JsonProperty("dg-fuel-flow_history")]
            public List<DataPointString> DGFuelFlowHistory { get; set; }
            [JsonProperty("boiler-fuel-flow_history")]
            public List<DataPointString> BoilerFuelFlowHistory { get; set; }
            [JsonProperty("me-fuel-temp_history")]
            public List<DataPointString> MainEngineFuelTemperatureHistory { get; set; }
            [JsonProperty("dg-fuel-temp_history")]
            public List<DataPointString> DGFuelTemperatureHistory { get; set; }
            [JsonProperty("me-fuel-flow_history")]
            public List<DataPointString> MainEngineFuelFlowHistory { get; set; }
            [JsonProperty("me-fuel-flow-new-model1_history")]
            public List<DataPointString> MainEngineFuelFlowHistoryNewModel1 { get; set; }
            [JsonProperty("me-fuel-flow-prev_history")]
            public List<DataPointString> MainEngineFuelFlowHistoryPrevious { get; set; }
            [JsonProperty("me-fuel-flow-model1_history")]
            public List<DataPointString> MainEngineFuelFlowHistoryModel1 { get; set; }
            [JsonProperty("boiler-fuel-temp_history")]
            public List<DataPointString> BoilerFuelTemperatureHistory { get; set; }
            [JsonProperty("me-fo-in")]
            public List<DataPointDouble> MainEngineFuelOilIn { get; set; }
            [JsonProperty("me-jacket-cfw-in")]
            public List<DataPointDouble> MainEngineJacketCoolingFreshWaterIn { get; set; }
            [JsonProperty("me-lo-in")]
            public List<DataPointDouble> MainEngineLubeOilIn { get; set; }
            [JsonProperty("me-piston-co-in")]
            public List<DataPointDouble> MainEnginePistonCoolingOilIn { get; set; }
            [JsonProperty("me-sw-air-clr-in")]
            public List<DataPointDouble> MainEngineSwitchAirClearIn { get; set; }
            [JsonProperty("me-start-air")]
            public List<DataPointDouble> MainEngineStartAir { get; set; }
            [JsonProperty("me-exh-spring-air")]
            public List<DataPointDouble> MainEngineExhaustSpringAir { get; set; }
            [JsonProperty("me-rpm")]
            public List<DataPointDouble> MainEngineRPM { get; set; }
            [JsonProperty("rudder-angle")]
            public List<DataPointDouble> RudderAngle { get; set; }
            [JsonProperty("me-tc-lo-in")]
            public List<DataPointDouble> MainEngineTurbochargerLubeOilIn { get; set; }
            [JsonProperty("no3-dg-lo-in")]
            public List<DataPointDouble> Number3DieselGeneratorLubeOilIn { get; set; }
            [JsonProperty("no1-dg-lo-in")]
            public List<DataPointDouble> Number1DieselGeneratorLubeOilIn { get; set; }
            [JsonProperty("no2-dg-lo-in")]
            public List<DataPointDouble> Number2DieselGeneratorLubeOilIn { get; set; }
            [JsonProperty("pme")]
            public List<DataPointDouble> PeakMeanEffectivePressure { get; set; }
            [JsonProperty("me-pme")]
            public List<DataPointDouble> MainEnginePeakMeanEffectivePressure { get; set; }
            [JsonProperty("report_time")]
            public List<DataPointDateTime> ReportTime { get; set; }
            [JsonProperty("voyage_num")]
            public List<DataPointInt> VoyageNumber { get; set; }
            [JsonProperty("voyage_type")]
            public List<DataPointString> VoyageType { get; set; }
            [JsonProperty("report")]
            public List<DataPointJsonString> Report { get; set; }
            [JsonProperty("report_type")]
            public List<DataPointString> ReportType { get; set; }
            [JsonProperty("main-air-reserve-1-opt")]
            public List<DataPointDouble> MainAirReserve1Optimization { get; set; }
            [JsonProperty("main-air-reserve-2-opt")]
            public List<DataPointDouble> MainAirReserve2Optimization { get; set; }
            [JsonProperty("me-sw-air-clr-in-opt")]
            public List<DataPointDouble> MainEngineSwitchAirClearInOptimization { get; set; }
            [JsonProperty("me-exh-spring-air-opt")]
            public List<DataPointDouble> MainEngineExhaustSpringAirOptimization { get; set; }
            [JsonProperty("me-jacket-cfw-in-opt")]
            public List<DataPointDouble> MainEngineJacketCoolingFreshWaterInOptimization { get; set; }
            [JsonProperty("me-tc-lo-in-opt")]
            public List<DataPointDouble> MainEngineTurbochargerLubeOilInOptimization { get; set; }
            [JsonProperty("me-piston-co-in-opt")]
            public List<DataPointDouble> MainEnginePistonCoolingOilInOptimization { get; set; }
            [JsonProperty("me-start-air-opt")]
            public List<DataPointDouble> MainEngineStartAirOptimization { get; set; }
            [JsonProperty("me-fo-in-opt")]
            public List<DataPointDouble> MainEngineFuelOilInOptimization { get; set; }
            [JsonProperty("me-fuel-index-opt")]
            public List<DataPointDouble> MainEngineFuelIndexOptimization { get; set; }
            [JsonProperty("turbo-rpm-opt")]
            public List<DataPointDouble> TurboRPMOptimization { get; set; }
            [JsonProperty("me-rpm-opt")]
            public List<DataPointDouble> MainEngineRPMOptimization { get; set; }
            [JsonProperty("me-lo-in-opt")]
            public List<DataPointDouble> MainEngineLubeOilInOptimization { get; set; }
            [JsonProperty("me-maneuv-air-opt")]
            public List<DataPointDouble> MainEngineManeuveringAirOptimization { get; set; }
            [JsonProperty("me-scav-air-opt")]
            public List<DataPointDouble> MainEngineScavengeAirOptimization { get; set; }
            [JsonProperty("rudder-opt")]
            public List<DataPointDouble> RudderOptimization { get; set; }
            [JsonProperty("aux-blr-drum-steam-opt")]
            public List<DataPointDouble> AuxiliaryBoilerDrumSteamOptimization { get; set; }
            [JsonProperty("no1-dg-lo-in-opt")]
            public List<DataPointDouble> Number1DieselGeneratorLubeOilInOptimization { get; set; }
            [JsonProperty("no2-dg-lo-in-opt")]
            public List<DataPointDouble> Number2DieselGeneratorLubeOilInOptimization { get; set; }
            [JsonProperty("no3-dg-lo-in-opt")]
            public List<DataPointDouble> Number3DieselGeneratorLubeOilInOptimization { get; set; }
            [JsonProperty("light-1")]
            public List<DataPointBoolean> Light1 { get; set; }
            [JsonProperty("light-2")]
            public List<DataPointBoolean> Light2 { get; set; }
            [JsonProperty("light-3")]
            public List<DataPointBoolean> Light3 { get; set; }
            [JsonProperty("light-4")]
            public List<DataPointBoolean> Light4 { get; set; }
            [JsonProperty("fire-1-1")]
            public List<DataPointBoolean> Fire11 { get; set; }
            [JsonProperty("fire-1-2")]
            public List<DataPointBoolean> Fire12 { get; set; }
            [JsonProperty("fire-2-1")]
            public List<DataPointBoolean> Fire21 { get; set; }
            [JsonProperty("fire-2-2")]
            public List<DataPointBoolean> Fire22 { get; set; }
            [JsonProperty("fire-3-1")]
            public List<DataPointBoolean> Fire31 { get; set; }
            [JsonProperty("fire-3-2")]
            public List<DataPointBoolean> Fire32 { get; set; }
            [JsonProperty("fire-4-1")]
            public List<DataPointBoolean> Fire41 { get; set; }
            [JsonProperty("fire-4-2")]
            public List<DataPointBoolean> Fire42 { get; set; }
            [JsonProperty("fire-5-1")]
            public List<DataPointBoolean> Fire51 { get; set; }
            [JsonProperty("fire-5-2")]
            public List<DataPointBoolean> Fire52 { get; set; }
            [JsonProperty("fire-6-1")]
            public List<DataPointBoolean> Fire61 { get; set; }
            [JsonProperty("fire-6-2")]
            public List<DataPointBoolean> Fire62 { get; set; }
            [JsonProperty("fire-7-1")]
            public List<DataPointBoolean> Fire71 { get; set; }
            [JsonProperty("fire-7-2")]
            public List<DataPointBoolean> Fire72 { get; set; }
            [JsonProperty("light-5")]
            public List<DataPointBoolean> Light5 { get; set; }
            [JsonProperty("light-6")]
            public List<DataPointBoolean> Light6 { get; set; }
            [JsonProperty("light-7")]
            public List<DataPointBoolean> Light7 { get; set; }
            [JsonProperty("light-8")]
            public List<DataPointBoolean> Light8 { get; set; }
            [JsonProperty("counter-1")]
            public List<DataPointBoolean> Counter1 { get; set; }
            [JsonProperty("counter-2")]
            public List<DataPointBoolean> Counter2 { get; set; }
            [JsonProperty("counter-3")]
            public List<DataPointBoolean> Counter3 { get; set; }
            [JsonProperty("counter-4")]
            public List<DataPointBoolean> Counter4 { get; set; }
            [JsonProperty("diesel-generator-3-stop")]
            public List<DataPointBoolean> DieselGenerator3Stop { get; set; }
            [JsonProperty("diesel-generator-3-start")]
            public List<DataPointBoolean> DieselGenerator3Start { get; set; }
            [JsonProperty("diesel-generator-2-stop")]
            public List<DataPointBoolean> DieselGenerator2Stop { get; set; }
            [JsonProperty("diesel-generator-2-start")]
            public List<DataPointBoolean> DieselGenerator2Start { get; set; }
            [JsonProperty("diesel-generator-1-stop")]
            public List<DataPointBoolean> DieselGenerator1Stop { get; set; }
            [JsonProperty("diesel-generator-1-start")]
            public List<DataPointBoolean> DieselGenerator1Start { get; set; }
            [JsonProperty("diesel-generator-3-ready")]
            public List<DataPointBoolean> DieselGenerator3Ready { get; set; }
            [JsonProperty("diesel-generator-2-ready")]
            public List<DataPointBoolean> DieselGenerator2Ready { get; set; }
            [JsonProperty("diesel-generator-1-ready")]
            public List<DataPointBoolean> DieselGenerator1Ready { get; set; }
            [JsonProperty("density")]
            public List<DataPointInt> Density { get; set; }
            [JsonProperty("total-run-hours")]
            public List<DataPointString> TotalRunHours { get; set; }
            [JsonProperty("PMI-power")]
            public List<DataPointDouble> PeakMeanIndicatorPower { get; set; }
            [JsonProperty("PMI")]
            public List<DataPointDouble> PeakMeanIndicator { get; set; }
            [JsonProperty("heat-value")]
            public List<DataPointDouble> HeatValue { get; set; }
            [JsonProperty("RPM")]
            public List<DataPointDouble> Rpm { get; set; }
            [JsonProperty("info")]
            public List<DataPointString> Info { get; set; }
            [JsonProperty("me-rpm-capture")]
            public List<DataPointDouble> MainEngineRPMCapture { get; set; }
            [JsonProperty("me-rpm-set-capture")]
            public List<DataPointDouble> MainEngineRPMSetCapture { get; set; }
            [JsonProperty("me-pi-capture")]
            public List<DataPointDouble> MainEnginePerformanceIndexCapture { get; set; }
            [JsonProperty("me-pmax-capture")]
            public List<DataPointDouble> MainEngineMaximumPressureCapture { get; set; }
            [JsonProperty("led-left-1")]
            public List<DataPointDouble> LedLeft1 { get; set; }
            [JsonProperty("led-left-bot-1")]
            public List<DataPointDouble> LedLeftBot1 { get; set; }
            [JsonProperty("led-right-1")]
            public List<DataPointDouble> LedRight1 { get; set; }
            [JsonProperty("led-right-bot-1")]
            public List<DataPointDouble> LedRightBot1 { get; set; }
            [JsonProperty("led-left")]
            public List<DataPointDouble> LedLeft { get; set; }
            [JsonProperty("led-left-bot")]
            public List<DataPointDouble> LedLeftBot { get; set; }
            [JsonProperty("led-right-bot")]
            public List<DataPointDouble> LedRightBot { get; set; }
            [JsonProperty("led-right")]
            public List<DataPointDouble> LedRight { get; set; }
            [JsonProperty("led-right-2")]
            public List<DataPointDouble> LedRight2 { get; set; }
            [JsonProperty("rev-count")]
            public List<DataPointInt> RevolutionCount { get; set; }
            [JsonProperty("led-right-bot-2")]
            public List<DataPointDouble> LedRightBot2 { get; set; }
        }
    }

    public class Tvos
    {
        public class AccessToken
        {
            [JsonProperty("access_token")]
            public string Token { get; set; }
        }

        public class RouteData
        {
            public List<DataPoint> DataPoints { get; set; }
            public class DataPoint
            {
                [JsonProperty("lat")]
                public double Latitude { get; set; }
                [JsonProperty("lon")]
                public double Longitude { get; set; }
                [JsonProperty("t")]
                public DateTime Timestamp { get; set; }
                public Data Data { get; set; }
            }

            /// <summary>
            /// <inheritdoc cref="Documentation.TvosRouteData"/>
            /// </summary>
            public class Data
            {
                public Wave Wave { get; set; }
                [JsonProperty("stdmet")]
                public StandardMeteorology StandardMeteorology { get; set; }
                public IceFraction IceFraction { get; set; }
                [JsonProperty("sss")]
                public SeaSurfaceSalinity SeaSurfaceSalinity { get; set; }
                [JsonProperty("sst")]
                public SeaSurfaceTemperature SeaSurfaceTemperature { get; set; }
                public SeaFloorDepth SeaFloorDepth { get; set; }
                public OceanCurrent OceanCurrent { get; set; }
                public Tide Tide { get; set; }
                public AirTemperature AirTemperature { get; set; }
            }

            public class Wave
            {
                public Sea TotalSea { get; set; }
                public Sea WindSea { get; set; }
                public Sea Swell { get; set; }
            }

            public class Sea
            {
                [JsonProperty("hs")]
                public double? SignificantWaveHeight { get; set; }
                [JsonProperty("hmax")]
                public double? MaximumWaveHeight { get; set; }
                [JsonProperty("tp")]
                public double? PeakPeriod { get; set; }
                [JsonProperty("tm")]
                public double? MeanPeriod { get; set; }
                [JsonProperty("tz")]
                public double? ZeroCrossingPeriod { get; set; }
                [JsonProperty("theta0")]
                public double? Direction { get; set; }
                [JsonProperty("thetap")]
                public double? PeakDirection { get; set; }
            }

            public class StandardMeteorology
            {
                [JsonProperty("mslp")]
                public double? MeanSeaLevelPressure { get; set; }
                [JsonProperty("rh")]
                public double? RelativeHumidity { get; set; }
            }

            public class IceFraction
            {
                public double? Value { get; set; }
            }

            public class SeaSurfaceSalinity
            {
                public double? Value { get; set; }
            }

            public class SeaSurfaceTemperature
            {
                public double? Value { get; set; }
            }

            public class SeaFloorDepth
            {
                public double? Value { get; set; }
            }

            public class OceanCurrent
            {
                public List<OceanCurrentValue> Values { get; set; }
            }

            public class OceanCurrentValue
            {
                public double? Speed { get; set; }
                public double? Direction { get; set; }
                public double? Depth { get; set; }
            }

            public class Tide
            {
                public double? Height { get; set; }
                public double? Speed { get; set; }
                public double? Direction { get; set; }
            }

            public class AirTemperature
            {
                public List<AirTemperatureValue> Values { get; set; }
            }

            public class AirTemperatureValue
            {
                public double? Height { get; set; }
                public double? Value { get; set; }
            }
        }
    }

    public class NmeaSentence
    {
        public enum NmeaSentenceCategory
        {
            GPRMC,
            GPGGA,
            GPGLL,
            GPVTG,
            GPBOD,
            GPRMB,
            GPGSA,
            GPGSV,
            PGRMM,
            GPWPL,
            PGRME,
            PGRMZ,
            GPRTE
        }

        //public static Dbo.NmeaData GPGGA(string sentence)
        //{
        //    try
        //    {
        //        string[] fields = sentence.Split(',');

        //        if (fields.IsUndefined())
        //        {
        //            throw new Exception();
        //        }

        //        return new Dbo.NmeaData()
        //        {
        //            Timestamp = DateTime.ParseExact(fields[1], "HHmmss.fff", null),
        //            Latitude = fields[2].ConvertToDecimalCoordinate(fields[3][0], true),
        //            Longitude = fields[4].ConvertToDecimalCoordinate(fields[5][0], false),
        //            FixQuality = int.Parse(fields[6], CultureInfo.InvariantCulture),
        //            NumberOfSatellites = int.Parse(fields[7], CultureInfo.InvariantCulture),
        //            HorizontalDilutionOfPrecision = double.Parse(fields[8], CultureInfo.InvariantCulture),
        //            Altitude = double.Parse(fields[9], CultureInfo.InvariantCulture),
        //            HeightOfGeoid = double.Parse(fields[11], CultureInfo.InvariantCulture),
        //            NmeaType = NmeaSentenceCategory.GPGGA.ToString()
        //        };
        //    }
        //    catch (Exception) { }

        //    return default;
        //}
        //public static Dbo.NmeaData GPGLL(string sentence)
        //{
        //    try
        //    {
        //        string[] fields = sentence.Split(',');

        //        if (fields.IsUndefined() || fields[6][0] != 'A')
        //        {
        //            throw new Exception();
        //        }

        //        return new Dbo.NmeaData()
        //        {
        //            Latitude = fields[1].ConvertToDecimalCoordinate(fields[2][0], true),
        //            Longitude = fields[3].ConvertToDecimalCoordinate(fields[4][0], false),
        //            Timestamp = DateTime.ParseExact(fields[5], "HHmmss.fff", null),
        //            NmeaType = NmeaSentenceCategory.GPGLL.ToString()
        //        };
        //    }
        //    catch (Exception) { }

        //    return default;
        //}
        //public static Dbo.NmeaData GPVTG(string sentence)
        //{
        //    try
        //    {
        //        string[] fields = sentence.Split(',');

        //        if (fields.IsUndefined())
        //        {
        //            throw new Exception();
        //        }

        //        return new Dbo.NmeaData()
        //        {
        //            CourseTrue = double.Parse(fields[1], CultureInfo.InvariantCulture),
        //            CourseMagnetic = double.Parse(fields[3], CultureInfo.InvariantCulture),
        //            SpeedKnots = double.Parse(fields[5], CultureInfo.InvariantCulture),
        //            SpeedKmph = double.Parse(fields[7], CultureInfo.InvariantCulture),
        //            NmeaType = NmeaSentenceCategory.GPVTG.ToString()
        //        };
        //    }
        //    catch (Exception) { }

        //    return default;
        //}
        //public static Dbo.NmeaData GPBOD(string sentence)
        //{
        //    try
        //    {
        //        string[] fields = sentence.Split(',');

        //        if (fields.IsUndefined())
        //        {
        //            throw new Exception();
        //        }

        //        return new Dbo.NmeaData()
        //        {
        //            BearingTrue = double.Parse(fields[1], CultureInfo.InvariantCulture),
        //            BearingMagnetic = double.Parse(fields[3], CultureInfo.InvariantCulture),
        //            DestinationWaypointId = fields[5],
        //            OriginWaypointId = fields[6],
        //            NmeaType = NmeaSentenceCategory.GPBOD.ToString()
        //        };
        //    }
        //    catch (Exception) { }

        //    return default;
        //}
        //public static Dbo.NmeaData GPRMB(string sentence)
        //{
        //    try
        //    {
        //        string[] fields = sentence.Split(',');

        //        if (fields.IsUndefined())
        //        {
        //            throw new Exception();
        //        }

        //        return new Dbo.NmeaData()
        //        {
        //            CrossTrackError = double.Parse(fields[2], CultureInfo.InvariantCulture),
        //            DestinationWaypointId = fields[3],
        //            OriginWaypointId = fields[4],
        //            DestinationLatitude = fields[5].ConvertToDecimalCoordinate(fields[6][0], true),
        //            DestinationLongitude = fields[7].ConvertToDecimalCoordinate(fields[8][0], false),
        //            RangeToDestination = double.Parse(fields[9], CultureInfo.InvariantCulture),
        //            BearingToDestination = double.Parse(fields[10], CultureInfo.InvariantCulture),
        //            VelocityToDestination = double.Parse(fields[11], CultureInfo.InvariantCulture),
        //            NmeaType = NmeaSentenceCategory.GPRMB.ToString()
        //        };
        //    }
        //    catch (Exception) { }

        //    return default;
        //}
        //public static Dbo.NmeaData GPWPL(string sentence)
        //{
        //    try
        //    {
        //        string[] fields = sentence.Split(',');

        //        if (fields.IsUndefined())
        //        {
        //            throw new Exception();
        //        }

        //        return new Dbo.NmeaData()
        //        {
        //            Latitude = fields[1].ConvertToDecimalCoordinate(fields[2][0], true),
        //            Longitude = fields[3].ConvertToDecimalCoordinate(fields[4][0], false),
        //            WaypointId = fields[5],
        //            NmeaType = NmeaSentenceCategory.GPWPL.ToString()
        //        };
        //    }
        //    catch (Exception) { }

        //    return default;
        //}
        //public static Dbo.NmeaData PGRMZ(string sentence)
        //{
        //    try
        //    {
        //        string[] fields = sentence.Split(',');

        //        if (fields.IsUndefined())
        //        {
        //            throw new Exception();
        //        }

        //        return new Dbo.NmeaData()
        //        {
        //            //Feet
        //            Altitude = double.Parse(fields[1], CultureInfo.InvariantCulture),
        //            NmeaType = NmeaSentenceCategory.PGRMZ.ToString()
        //        };
        //    }
        //    catch (Exception) { }

        //    return default;
        //}
        //public static Dbo.NmeaData GPRTE(string sentence)
        //{
        //    try
        //    {
        //        string[] fields = sentence.Split(',');

        //        if (fields.IsUndefined())
        //        {
        //            throw new Exception();
        //        }

        //        return new Dbo.NmeaData()
        //        {
        //            NumberOfSentences = int.Parse(fields[1], CultureInfo.InvariantCulture),
        //            SentenceNumber = int.Parse(fields[2], CultureInfo.InvariantCulture),
        //            NmeaType = NmeaSentenceCategory.GPRTE.ToString()
        //        };
        //    }
        //    catch (Exception) { }

        //    return default;
        //}
        public static Dbo.VesselLocation GPRMC(string sentence)
        {
            try
            {
                string[] fields = sentence.Split(',');

                if (fields.IsUndefined() || fields[2][0] != 'A')
                {
                    throw new Exception();
                }

                return new Dbo.VesselLocation()
                {
                    LogDate = DateTime.ParseExact($"{fields[9]} {fields[1]}", "ddMMyy HHmmss.fff", null),
                    //Degrees
                    Course = (int)double.Parse(fields[8], CultureInfo.InvariantCulture),
                    //Knots
                    Speed = (int)double.Parse(fields[7], CultureInfo.InvariantCulture),
                    Lat = fields[3].ConvertToDecimalCoordinate(fields[4][0], true),
                    Long = fields[5].ConvertToDecimalCoordinate(fields[6][0], false),
                    //NmeaType = new string(NmeaSentenceCategory.GPRMC.ToString().Skip(2).ToArray())
                };
            }
            catch (Exception) { }

            return default;
        }
    }
}