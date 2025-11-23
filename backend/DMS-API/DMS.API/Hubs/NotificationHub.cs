using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace DMS.API.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task NotifyAdmin(string message)
        {
            await Clients.Client(connectionId: GetConnectionId()).SendAsync("receive", message);
        }
        public string GetConnectionId() => Context.ConnectionId;

        public override Task OnConnectedAsync()
        {
            // For example, you could add admin users to the "Admins" group
            if (Context.User.IsInRole("Admin"))
            {
                Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
            }

            return base.OnConnectedAsync();
        }


        public override Task OnDisconnectedAsync(Exception exception)
        {
            string userId = Context.UserIdentifier;
            if (userId != null)
            {
                // Remove this connection from the user group
                Groups.RemoveFromGroupAsync(Context.ConnectionId, userId);
            }
            return base.OnDisconnectedAsync(exception);
        }

    }


    
}
