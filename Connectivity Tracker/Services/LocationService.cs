using Windows.Devices.Geolocation;

namespace Connectivity_Tracker.Services
{
    public class LocationService
    {
        private Geolocator? _geolocator;
        private bool _isEnabled = false;
        private GeolocationAccessStatus _accessStatus;

        public bool IsLocationAvailable => _isEnabled && _accessStatus == GeolocationAccessStatus.Allowed;
        public string LocationStatus { get; private set; } = "Standort unbekannt";

        public async Task<bool> InitializeAsync()
        {
            try
            {
                _accessStatus = await Geolocator.RequestAccessAsync();

                if (_accessStatus == GeolocationAccessStatus.Allowed)
                {
                    _geolocator = new Geolocator
                    {
                        DesiredAccuracy = PositionAccuracy.Default,
                        ReportInterval = 0
                    };

                    _isEnabled = true;
                    LocationStatus = "Standort aktiviert";
                    return true;
                }
                else
                {
                    _isEnabled = false;
                    LocationStatus = _accessStatus switch
                    {
                        GeolocationAccessStatus.Denied => "Standort verweigert",
                        GeolocationAccessStatus.Unspecified => "Standort Fehler",
                        _ => "Standort unbekannt"
                    };
                    return false;
                }
            }
            catch (Exception ex)
            {
                _isEnabled = false;
                LocationStatus = $"Standort Fehler: {ex.Message}";
                return false;
            }
        }

        public async Task<(double? Latitude, double? Longitude)> GetCurrentLocationAsync()
        {
            if (!_isEnabled || _geolocator == null)
            {
                return (null, null);
            }

            try
            {
                var position = await _geolocator.GetGeopositionAsync();

                if (position?.Coordinate != null)
                {
                    return (position.Coordinate.Point.Position.Latitude,
                           position.Coordinate.Point.Position.Longitude);
                }
            }
            catch (Exception ex)
            {
                LocationStatus = $"Standort Fehler: {ex.Message}";
            }

            return (null, null);
        }

        public void Disable()
        {
            _isEnabled = false;
            _geolocator = null;
            LocationStatus = "Standort deaktiviert";
        }
    }
}
