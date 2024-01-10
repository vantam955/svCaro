using Microsoft.AspNetCore.SignalR;
namespace svCaro
{
    public class chatHub:Hub
    {
        private static int MaxUsers = 2;
        private static int ConnectedUsers = 0;
        private static List<string> ConnectedUserIds = new List<string>();
        private static EStatus currentStatus = EStatus.X;
        private static List<string> rooms = new List<string>() { "Room1", "Room2", "Room3" };
        private static List<string> HDD = new List<string>();
        private static List<string> HDD2 = new List<string>();
        public async Task RequestConnection()
        {
            if (ConnectedUsers < MaxUsers)
            {
                ConnectedUsers++;
                ConnectedUserIds.Add(Context.ConnectionId);
                await Groups.AddToGroupAsync(Context.ConnectionId, "ChatRoom");
                await Clients.Group("ChatRoom").SendAsync("UserJoined", $"{Context.ConnectionId} đã vào phòng");
                await Clients.Caller.SendAsync("NotifyStatus", ConnectedUserIds.Count == 1 ? "X" : "O");
            }
            else
            {
                await Clients.Caller.SendAsync("RoomFull", "Phòng đã đầy");
            }
        }
        public async Task LoadRooms()
        {
            await Clients.Caller.SendAsync("Rooms", rooms);
        }


        public async Task Click(int x, int status)
        {
            if (ConnectedUsers < MaxUsers)
            {
                await Clients.Caller.SendAsync("RoomFull", "Cần đủ 2 người chơi");
                return;
            }
            var index = ConnectedUserIds.IndexOf(Context.ConnectionId);
            if ((index == 0 && currentStatus == EStatus.X) || (index == 1 && currentStatus == EStatus.O))
            {
                await Clients.All.SendAsync("ClickAtPoint", x, status);
                currentStatus = currentStatus == EStatus.X ? EStatus.O : EStatus.X;
                await Clients.All.SendAsync("ChangeTurn", currentStatus);
            }

        }
        public enum EStatus
        {
            None,
            X,
            O
        }
    }
}
