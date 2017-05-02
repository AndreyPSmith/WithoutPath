using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using WithoutPath.BL;
using WithoutPath.DTO;
using WithoutPath.EVEAPI;
using System.Web.Mvc;
using WithoutPath.Attribute;

namespace WithoutPath.Hubs
{
    public class MapHub : Hub
    {
        private static IHubContext hubContext;

        public static IHubContext HubContext
        {
            get
            {
                if (hubContext == null)
                    hubContext = GlobalHost.ConnectionManager.GetHubContext<MapHub>();
                return hubContext;
            }
        }

        private IEVEProvider _provider;
        public IEVEProvider Provider
        {
            get
            {
                if (_provider == null)
                    _provider = DependencyResolver.Current.GetService<IEVEProvider>();
                return _provider;
            }
        }

        private ILogic _logic;
        public ILogic Logic
        {
            get
            {
                if (_logic == null)
                    _logic = DependencyResolver.Current.GetService<ILogic>();
                return _logic;
            }
        }

        static MapModel Map { get; set; }

        public static void BroadcastSystems()
        {
            var logic = DependencyResolver.Current.GetService<ILogic>();
            logic.UpdateCharactersLocation();
            Map = logic.GetMap();
            HubContext.Clients.All.addMessage(Map);
        }

        // Подключение нового пользователя
        [AuthorizeMapHab]
        public void Connect()
        {
            Groups.Add(Context.ConnectionId, Context.User.Identity.GetUserId());
            // Посылаем сообщение текущему подключению
            Clients.Caller.onConnected(Map);
        }

        // Отключение пользователя
        public override Task OnDisconnected(bool stopCalled)
        {
            Groups.Remove(Context.ConnectionId, Context.User.Identity.GetUserId());
            return base.OnDisconnected(stopCalled);
        }

        // Удалить ссылки на систему
        public IResult RemoveSystem(int systemId)
        {
            var result = Logic.RemoveSystem(systemId);
            if (!result.IsError)
            {
                Map = Logic.GetMap();
                HubContext.Clients.All.addMessage(Map);
            }
            return result;
        }

        public IResult RemoveLink(string system_1, string system_2)
        {
            var result = Logic.RemoveLink(system_1, system_2);
            if (!result.IsError)
            {
                Map = Logic.GetMap();
                HubContext.Clients.All.addMessage(Map);
            }
            return result;
        }

        public IResult UpdateSystem(int Id, string note, bool warning)
        {
            var result = Logic.UpdateSystemStatus(Id, note, warning);
            if (!result.IsError)
            {
                Map = Logic.GetMap();
                HubContext.Clients.All.addMessage(Map);
            }
            return result;
        }

        public IResult UpdateLink(int Id, int status)
        {
            var result = Logic.UpdateLinkStatus(Id, status);
            if (!result.IsError)
            {
                Map = Logic.GetMap();
                HubContext.Clients.All.addMessage(Map);
            }
            return result;
        }

        public void SetDestination(long systemId)
        {
            var userId = Context.User.Identity.GetUserId();
            var user = Logic.GetUser(userId);
            var token = Logic.GetMainCharacterAccessToken(user.Id);
            var result = Provider.SetWaypoint(systemId, token);
        }
    }
}