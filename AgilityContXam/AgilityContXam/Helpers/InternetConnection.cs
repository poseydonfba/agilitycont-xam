using Plugin.Connectivity;
using System;
using System.Threading.Tasks;

namespace AgilityContXam.Helpers
{
    public class InternetConnection
    {
        public async static Task<bool> IsConnected()
        {
            var uri = new Uri(Constants.BaseApiAddress);
            bool canReach = await CrossConnectivity.Current.IsRemoteReachable(uri, TimeSpan.FromSeconds(5));
            return canReach;
        }
    }
}
