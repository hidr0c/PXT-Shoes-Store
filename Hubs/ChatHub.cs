using Microsoft.AspNet.SignalR;
using System;

public class ChatHub : Hub
{
    public void SendMessage(string userName, string message)
    {
        string timeStamp = DateTime.Now.ToString("HH:mm");
        Clients.All.receiveMessage(userName, message, timeStamp);
    }
}
