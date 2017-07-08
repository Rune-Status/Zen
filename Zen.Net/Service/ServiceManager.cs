using System.Threading.Tasks;
using Zen.Game.IO;

namespace Zen.Net.Service
{
    public class ServiceManager
    {
        public LoginService LoginService { get; } = new LoginService(new DummyPlayerSerializer());
        public UpdateService UpdateService { get; } = new UpdateService();

        public void StartAll()
        {
            Task.Factory.StartNew(() => { UpdateService.Start(); });
            Task.Factory.StartNew(() => { LoginService.Start(); });
        }
    }
}