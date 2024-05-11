using System.Net.NetworkInformation;
using System.Text;

namespace LegoTrainManagerWeb.Services
{
    public class Esp32Service : IEsp32Service
    {
        public const string IPAdress = "192.168.2.141";
        public const string URL = $"http://{IPAdress}/";

        private readonly HttpClient _httpClient;
        private readonly ILogger<Esp32Service> _logger;

        private readonly Ping _ping;

        public Esp32Service(HttpClient client, ILogger<Esp32Service> logger)
        {
            _httpClient = client;
            _httpClient.BaseAddress = new Uri(URL);
            _logger = logger;
            _ping = new Ping();
        }

        public async Task<bool> IsAvailable()
        {
            try
            {
                var pingResult = await _ping.SendPingAsync(IPAdress);
                return pingResult.Status == IPStatus.Success;
            }
            catch (Exception e) when (e is PingException or InvalidOperationException)
            {
                _logger.LogError("Exception msg: {e}");
                return false;
            }
        }

        public async Task<string> GetRoot()
        {
            var request = string.Empty;
            try
            {
                request = await _httpClient.GetStringAsync("/");
                _logger.LogInformation($"request msg: {request}");
                return request;
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception msg: {e}");
            }

            return request;
        }

        public async Task<string> GetCurrentTrainState()
        {
            var request = string.Empty;
            try
            {
                request = await _httpClient.GetStringAsync("/currentTrainState");
                _logger.LogInformation($"request msg: {request}");
                return request;
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception msg: {e}");
            }

            return request;
        }

        public async Task<string> StopTrain()
        {
            var request = string.Empty;
            try
            {
                request = await _httpClient.GetStringAsync("/stop");
                _logger.LogInformation($"request msg: {request}");
                return request;
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception msg: {e}");
            }

            return request;
        }

        public async Task<string> SetSpeed(int speed = 15)
        {
            var request = string.Empty;
            try
            {
                var url = $"speed?speed={speed}";
                using var content = new StringContent("", Encoding.UTF8, "text/plain");
                var httpContent = await _httpClient.PostAsync(url, content);
                _logger.LogInformation($"request msg: {request}");
                request = httpContent.IsSuccessStatusCode ? $"Speed Update {speed}" : "Speed Error";
                return request;
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception msg: {e}");
            }

            return request;
        }

        public async Task<string> SetRGB(byte r, byte g, byte b)
        {
            var request = string.Empty;
            try
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("r", r.ToString()),
                    new KeyValuePair<string, string>("g", g.ToString()),
                    new KeyValuePair<string, string>("b", b.ToString())
                });

                var httpContent = await _httpClient.PostAsync($"/setRGB", content);
                request = httpContent.IsSuccessStatusCode ? "Update LED" : "Error LED";
                _logger.LogInformation($"request msg: {request}");
                return request;
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception msg: {e}");
            }

            return request;
        }
    }
}
