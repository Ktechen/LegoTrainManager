using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;
using static System.Net.WebRequestMethods;

namespace LegoTrainManagerWeb.Services
{
    public class Esp32Service : IEsp32Service
    {
        public const string IPAdress = "192.168.2.141";
        public const string URL = $"http://{IPAdress}/";

        private readonly HttpClient _httpClient;
        private readonly ILogger<Esp32Service> _logger;

        public Esp32Service(HttpClient client, ILogger<Esp32Service> logger)
        {
            _httpClient = client;
            _httpClient.BaseAddress = new Uri(URL);
            _logger = logger;
        }

        public async Task<string> GetRoot()
        {
            try
            {
                var request = await _httpClient.GetStringAsync("/");
                _logger.LogInformation($"request msg: {request}");
                return request;
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception msg: {e}");
                return e.Message;
            }
        }

        public async Task<string> GetCurrentTrainState()
        {
            try
            {
                var request = await _httpClient.GetStringAsync("/currentTrainState");
                _logger.LogInformation($"request msg: {request}");
                return request;
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception msg: {e}");
                return e.Message;
            }
        }

        public async Task<string> StopTrain()
        {
            try
            {
                var request = await _httpClient.GetStringAsync("/stop");
                _logger.LogInformation($"request msg: {request}");
                return request;
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception msg: {e}");
                return e.Message;
            }
        }

        public async Task<string> SetSpeed(int speed = 15)
        {
            try
            {
                var url = $"speed?speed={speed}";
                using var content = new StringContent("", Encoding.UTF8, "text/plain");
                var request = await _httpClient.PostAsync(url, content);
                _logger.LogInformation($"request msg: {request}");
                return request.IsSuccessStatusCode ? "Speed Update" : "Speed Error";
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception msg: {e}");
                return e.Message;
            }
        }

        public async Task<string> SetRGB(byte r, byte g, byte b)
        {
            try
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("r", r.ToString()),
                    new KeyValuePair<string, string>("g", g.ToString()),
                    new KeyValuePair<string, string>("b", b.ToString())
                });

                var request = await _httpClient.PostAsync($"/setRGB", content);
                _logger.LogInformation($"request msg: {request}");
                return request.IsSuccessStatusCode ? "Update LED" : "Error LED";
            }
            catch (Exception e)
            {
                _logger.LogError($"Exception msg: {e}");
                return e.Message;
            }
        }
    }
}
