using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WithoutPath.EVEAPI.JsonTypes;
using System.Net;
using System.Reactive.Subjects;

namespace WithoutPath.EVEAPI
{
    public interface IEVEProvider 
    {
        string Authorize(string redirectTo);

        AuthResponse GetAccessToken(string code, bool isRefresh = false);

        VerifyCharacterResponse VerifyCharacter(string Token);

        CharacterResponse GetCharacter(long Id);

        LocationResponseCrest GetCharacterLocationCrest(long Id, string Token);

        LocationResponse GetCharacterLocation(long Id, string Token);

        ShipResponse GetCharacterShip(long Id, string Token);

        SolarSystemResponse GetSolarSystem(long systemId);

        ShipTypeResponse GetShipType(long typeId);

        HttpStatusCode SetWaypoint(long destinationId, string Token);
    }
}
