using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WithoutPath.DAL;
using WithoutPath.DTO;

namespace WithoutPath.BL
{
    public interface ILogic
    {
        MapModel GetMap();

        IResult UpdateSystemStatus(int Id, string note, bool warning);

        IResult RemoveSystem(int systemId);

        IResult AddLink(string from, string to, int userId);

        IResult UpdateLinkStatus(int Id, int status);

        IResult UpdateCharactersLocation();

        string GetMainCharacterAccessToken(int userId);

        IResult RemoveLink(string system_1, string system_2);

        User GetUser(string email);
    }
}
