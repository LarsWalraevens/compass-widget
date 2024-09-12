using System.Collections.ObjectModel;
using System.ComponentModel;
using Vvoids.Api.Functions.Methods;

namespace Widget.Forecast
{

    public class WidgetForecastService
    {
        public WidgetForecastInstance GetForecastInstanceFromForecastData(Methods.SelectWidgetCompassResult data)
        {
            try
            {
                decimal speedKnots = 8m;

                WidgetForecastInstance instance = new WidgetForecastInstance()
                {
                    DegreeBearing = (decimal?)data.Planned?.Vessel?.Bearing ?? 0,
                    DegreePositioning = (decimal?)data.Dot?.Direction ?? 0,
                    Items = new List<WidgetForecastItem>()
                };

                // Planned Vessel Heading
                if (data.Planned?.Vessel != null)
                {
                    instance.Items.Add(new WidgetForecastItem()
                    {
                        Degree = (decimal)data.Planned.Vessel.Heading,
                        Type = WidgetForecast.Types.FirstOrDefault(x => x.Name == "Speed"),
                        IsActual = false,
                        IsVisible = true,
                        ValueText = $"{speedKnots:0.##}kn, {data.Planned.Vessel.Heading}°",
                        ValueInPercent = 100
                    });
                }

                // Sailed Vessel Heading
                if (data.Sailed?.Vessel != null)
                {
                    instance.Items.Add(new WidgetForecastItem()
                    {
                        Degree = (decimal)data.Sailed.Vessel.Heading,
                        Type = WidgetForecast.Types.FirstOrDefault(x => x.Name == "Speed"),
                        IsActual = true,
                        IsVisible = true,
                        ValueText = $"{speedKnots:0.##}kn, {data.Sailed.Vessel.Heading}°",
                        ValueInPercent = 100
                    });
                }

                // Planned Geo - WaveTotalSea
                if (data.Planned?.Geo != null)
                {
                    instance.Items.Add(new WidgetForecastItem()
                    {
                        Degree = (decimal)data.Planned.Geo.WaveTotalSeaDirection,
                        Type = WidgetForecast.Types.FirstOrDefault(x => x.Name == "Wave"),
                        IsActual = false,
                        IsVisible = true,
                        ValueText = $"{data.Planned.Geo.WaveTotalSeaSignificantWaveHeight:0.##}m, {data.Planned.Geo.WaveTotalSeaDirection}°",
                        ValueInPercent = (decimal)data.Planned.Geo.WaveTotalSeaSignificantWaveHeight * (WidgetForecast.Types.FirstOrDefault(x => x.Name == "Wave")?.Threshold ?? 0) / 100
                    });
                }

                // Sailed Geo - WaveTotalSea
                if (data.Sailed?.Geo != null)
                {
                    instance.Items.Add(new WidgetForecastItem()
                    {
                        Degree = (decimal)data.Sailed.Geo.WaveTotalSeaDirection,
                        Type = WidgetForecast.Types.FirstOrDefault(x => x.Name == "Wave"),
                        IsActual = true,
                        IsVisible = true,
                        ValueText = $"{data.Sailed.Geo.WaveTotalSeaSignificantWaveHeight:0.##}m, {data.Sailed.Geo.WaveTotalSeaDirection}°",
                        ValueInPercent = (decimal)data.Sailed.Geo.WaveTotalSeaSignificantWaveHeight / 100 * (WidgetForecast.Types.FirstOrDefault(x => x.Name == "Wave")?.Threshold ?? 0)
                    });
                }

                // Sailed Geo - WaveWindSea
                if (data.Sailed?.Geo != null)
                {
                    instance.Items.Add(new WidgetForecastItem()
                    {
                        Degree = (decimal)data.Sailed.Geo.WaveWindSeaDirection,
                        Type = WidgetForecast.Types.FirstOrDefault(x => x.Name == "Wind"),
                        IsActual = true,
                        IsVisible = true,
                        ValueText = $"{data.Sailed.Geo.WaveWindSeaSignificantWaveHeight:0.##}kn, {data.Sailed.Geo.WaveWindSeaDirection}°",
                        ValueInPercent = (decimal)data.Sailed.Geo.WaveWindSeaSignificantWaveHeight / 100 * (WidgetForecast.Types.FirstOrDefault(x => x.Name == "Wind")?.Threshold ?? 0)
                    });
                }

                // Planned Geo - WaveWindSea
                if (data.Planned?.Geo != null)
                {
                    instance.Items.Add(new WidgetForecastItem()
                    {
                        Degree = (decimal)data.Planned.Geo.WaveWindSeaDirection,
                        Type = WidgetForecast.Types.FirstOrDefault(x => x.Name == "Wind"),
                        IsActual = false,
                        IsVisible = true,
                        ValueText = $"{data.Planned.Geo.WaveWindSeaSignificantWaveHeight:0.##}kn, {data.Planned.Geo.WaveWindSeaDirection}°",
                        ValueInPercent = (decimal)data.Planned.Geo.WaveWindSeaSignificantWaveHeight / 100 * (WidgetForecast.Types.FirstOrDefault(x => x.Name == "Wind")?.Threshold ?? 0)
                    });
                }

                // Planned Geo - OceanCurrent
                if (data.Planned?.Geo != null)
                {
                    instance.Items.Add(new WidgetForecastItem()
                    {
                        Degree = (decimal)data.Planned.Geo.OceanCurrentDirection,
                        Type = WidgetForecast.Types.FirstOrDefault(x => x.Name == "Current"),
                        IsActual = false,
                        IsVisible = true,
                        ValueText = $"{data.Planned.Geo.OceanCurrentValue:0.##}kn, {data.Planned.Geo.OceanCurrentDirection}°",
                        ValueInPercent = (decimal)data.Planned.Geo.OceanCurrentValue / 100 * (WidgetForecast.Types.FirstOrDefault(x => x.Name == "Current")?.Threshold ?? 0)
                    });
                }

                // Sailed Geo - OceanCurrent
                if (data.Sailed?.Geo != null)
                {
                    instance.Items.Add(new WidgetForecastItem()
                    {
                        Degree = (decimal)data.Sailed.Geo.OceanCurrentDirection,
                        Type = WidgetForecast.Types.FirstOrDefault(x => x.Name == "Current"),
                        IsActual = true,
                        IsVisible = true,
                        ValueText = $"{data.Sailed.Geo.OceanCurrentValue:0.##}kn, {data.Sailed.Geo.OceanCurrentDirection}°",
                        ValueInPercent = (decimal)data.Sailed.Geo.OceanCurrentValue / 100 * (WidgetForecast.Types.FirstOrDefault(x => x.Name == "Current")?.Threshold ?? 0)
                    });
                }

                return instance;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in WidgetData.GetForecastData: " + ex.Message);
                return null;
            }
        }
    }

    public class WidgetForecastInstance : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private WidgetForecastItem _focusedItem;
        public WidgetForecastItem FocusedItem
        {
            get => _focusedItem;
            set
            {
                if (_focusedItem != value)
                {
                    _focusedItem = value;
                    OnPropertyChanged(nameof(FocusedItem)); // Notify UI
                }
            }
        }
        public void SetFocusedForecastItem(WidgetForecastItem newItem)
        {
            FocusedItem = newItem;
        }

        public List<WidgetForecastItem> Items { get; set; }

        public int TargetDotSizeInPx { get; set; } = 60;

        public decimal DepthValueInMeter { get; set; }

        public int DiameterInPx { get; set; } = 250;

        public decimal? DegreePositioning { get; set; }
        public bool? PositioningIsWarning { get; set; }

        public decimal? DegreeBearing { get; set; }
    }

    public class WidgetForecastType
    {
        public int Id { get; set; }

        public string Icon { get; set; }
        public string Name { get; set; }
        public decimal? Threshold { get; set; }


    }


    public class WidgetForecastItem
    {
        // private Guid Id { get; set; }s
        public WidgetForecastType Type { get; set; }
        public bool IsActual { get; set; }

        public decimal Degree { get; set; }

        public decimal ValueInPercent { get; set; }
        public string ValueText { get; set; }

        public bool IsVisible { get; set; }

    }

    public class WidgetForecast
    {
        public static readonly ReadOnlyCollection<WidgetForecastType> Types = new ReadOnlyCollection<WidgetForecastType>(new List<WidgetForecastType> {
            new WidgetForecastType { Id = 1, Icon = "bi-speedometer2", Name = "Speed" },
            new WidgetForecastType { Id = 2, Icon = "bi-wind", Name = "Wind", Threshold= 50m},
            new WidgetForecastType { Id = 3, Icon = "bi-tsunami", Name = "Wave", Threshold= 2.5m },
            new WidgetForecastType { Id = 3, Icon = "bi-arrow-repeat", Name = "Current", Threshold= 1m }
        });
    }
}

