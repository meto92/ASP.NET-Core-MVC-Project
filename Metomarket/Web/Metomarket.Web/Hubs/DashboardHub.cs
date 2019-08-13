using Microsoft.AspNetCore.SignalR;

namespace Metomarket.Web.Hubs
{
    public class DashboardHub : Hub
    {
        public const string UserRegisteredMethodName = "UserRegistered";

        public const string ContractCreatedMethodName = "ContractCreated";
    }
}