using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Steamworks;
using System.Net;
using System.IO;
//using Microsoft.Xna.Framework.Audio;

/// <summary>
/// https://partner.steamgames.com/documentation/matchmaking
/// </summary>
public static class SteamLobby
{
    //SteamAPICall_t CreateLobby( ELobbyType eLobbyType, int cMaxMembers );
    public static SteamAPICall_t createLobby(ELobbyType eLobbyType, int cMaxMembers)
    {
         return SteamMatchmaking.CreateLobby(eLobbyType, cMaxMembers);
    }

    public static SteamAPICall_t joinLobby(CSteamID steamIDLobby)
    {

        return SteamMatchmaking.JoinLobby(steamIDLobby);
    }
}
