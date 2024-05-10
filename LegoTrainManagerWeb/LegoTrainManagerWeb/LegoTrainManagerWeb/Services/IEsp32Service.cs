using Microsoft.AspNetCore.Mvc;

namespace LegoTrainManagerWeb.Services
{
    public interface IEsp32Service
    {
        public Task<string> GetRoot();
        public Task<string> GetCurrentTrainState();
        public Task<string> StopTrain();
        public Task<string> SetSpeed(int speed);
        public Task<string> SetRGB(byte r, byte g , byte b);
    }
}
