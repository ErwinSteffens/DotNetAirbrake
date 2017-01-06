using System.Threading.Tasks;
using DotNetAirbrake.Models;

namespace DotNetAirbrake
{
    public interface IAirbrakeClient
    {
        Task SendAsync(AirbrakeCreateNoticeMessage notice);
        void Configure(AirbrakeOptions options);
        void Configure(string serverUrl, string projectId, string projectKey);
    }
}