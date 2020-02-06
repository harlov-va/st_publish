namespace arkAS.SignalR
{
    public class SignalRHub : BaseHub
    {
        public void UserMethod(string data)
        {
            Clients.Caller.userMethodReceived(data);
        }

    }
}