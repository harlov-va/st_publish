using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;

namespace arkAS.SignalR
{
    public class BaseHub : Hub
    {
        private static Dictionary<string, string> _connections = new Dictionary<string, string>();

        public override Task OnConnected()
        {
            _connections.Add(Context.ConnectionId, Context.QueryString["userData"]);
            UpdateConnections();
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            _connections.Remove(Context.ConnectionId);
            UpdateConnections();
            return base.OnDisconnected(stopCalled);
        }

        public void SendData(string connectionId, string data)
        {
            Clients.Client(connectionId).dataReceived(data);
        }

        public void SendDataAll(string data)
        {
            Clients.Others.dataReceived(data);
        }

        public void SendCommand(string connectionId, string command, string data)
        {
            Clients.Client(connectionId).commandReceived(command, data);
        }

        public void SendCommandAll(string command, string data)
        {
            Clients.Others.commandReceived(command, data);
        }

        public void GetConnections()
        {
            Clients.Caller.connectionsReceived(GetConnectionsInfo().ToArray());
        }

        protected void UpdateConnections()
        {
            Clients.All.connectionsReceived(GetConnectionsInfo().ToArray());
        }

        protected ConnectionInfo[] GetConnectionsInfo()
        {
            return _connections.Select(k => new ConnectionInfo { id = k.Key, data = k.Value }).ToArray();
        }        
    }
}