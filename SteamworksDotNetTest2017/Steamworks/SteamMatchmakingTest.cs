//using UnityEngine;
using System.Collections;
using Steamworks;
using System;
using System.Net;
using SteamworksDotNetTest2017;

public class SteamMatchmakingTest
{
    public string debugString = "";

    public static CSteamID m_Lobby;

    protected Callback<FavoritesListChanged_t> m_FavoritesListChanged;
    protected Callback<LobbyInvite_t> m_LobbyInvite;
    protected Callback<LobbyEnter_t> m_LobbyEnter;
    protected Callback<LobbyDataUpdate_t> m_LobbyDataUpdate;
    protected Callback<LobbyChatUpdate_t> m_LobbyChatUpdate;
    protected Callback<LobbyChatMsg_t> m_LobbyChatMsg;
    protected Callback<LobbyGameCreated_t> m_LobbyGameCreated;
    protected Callback<LobbyKicked_t> m_LobbyKicked;
    protected Callback<FavoritesListAccountsUpdated_t> m_FavoritesListAccountsUpdated;

    private CallResult<LobbyEnter_t> OnLobbyEnterCallResult;
    private CallResult<LobbyMatchList_t> OnLobbyMatchListCallResult;
    private CallResult<LobbyCreated_t> OnLobbyCreatedCallResult;

    public SteamMatchmakingTest()
    {
        m_FavoritesListChanged = Callback<FavoritesListChanged_t>.Create(OnFavoritesListChanged);
        m_LobbyInvite = Callback<LobbyInvite_t>.Create(OnLobbyInvite);
        m_LobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
        m_LobbyDataUpdate = Callback<LobbyDataUpdate_t>.Create(OnLobbyDataUpdate);
        m_LobbyChatUpdate = Callback<LobbyChatUpdate_t>.Create(OnLobbyChatUpdate);
        m_LobbyChatMsg = Callback<LobbyChatMsg_t>.Create(OnLobbyChatMsg);
        m_LobbyGameCreated = Callback<LobbyGameCreated_t>.Create(OnLobbyGameCreated);
        m_LobbyKicked = Callback<LobbyKicked_t>.Create(OnLobbyKicked);
        m_FavoritesListAccountsUpdated = Callback<FavoritesListAccountsUpdated_t>.Create(OnFavoritesListAccountsUpdated);

        OnLobbyEnterCallResult = CallResult<LobbyEnter_t>.Create(OnLobbyEnter);
        OnLobbyMatchListCallResult = CallResult<LobbyMatchList_t>.Create(OnLobbyMatchList);
        OnLobbyCreatedCallResult = CallResult<LobbyCreated_t>.Create(OnLobbyCreated);
    }

    public SteamAPICall_t createLobbyGameServer(ELobbyType ELobbyType)
    {
        SteamAPICall_t handle = SteamMatchmaking.CreateLobby(ELobbyType, 250);
        OnLobbyCreatedCallResult.Set(handle);
        Console.WriteLine("SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 1) : " + handle);
        return handle;
    }

    public void setGameServerSettings(CSteamID m_Lobby, string gameServerIP, string port)
    {
        uint gameServerIPuint = SteamAntiCheat.ToUInt(gameServerIP);
        Int32 portNum;
        if (Int32.TryParse(port, out portNum))
        {
            SteamMatchmaking.SetLobbyGameServer(m_Lobby, gameServerIPuint, (ushort)portNum, CSteamID.NonSteamGS); //127.0.0.1
            //SteamMatchmaking.SetLobbyGameServer(m_Lobby, 2130706433, 1337, CSteamID.NonSteamGS); //127.0.0.1
            Console.WriteLine("SteamMatchmaking.SetLobbyGameServer(" + m_Lobby + " " + gameServerIPuint + " "+ (ushort)portNum + ", CSteamID.NonSteamGS)");
            bool reasult = SteamMatchmaking.SetLobbyJoinable(m_Lobby, true);
            Console.WriteLine("SteamMatchmaking.SetLobbyJoinable(" + m_Lobby + ", true) : " + reasult);
            bool reasult2 = SteamMatchmaking.SetLobbyType(m_Lobby, ELobbyType.k_ELobbyTypePublic);
            SteamMatchmaking.JoinLobby(m_Lobby);
        }
    }

    public struct gameServer
    {
        public gameServer(CSteamID _m_Lobby, uint _GameServerIP, uint _GameServerPort, CSteamID _SteamIDGameServer)
        {
            this.m_Lobby = _m_Lobby;
            this.GameServerIP = _GameServerIP;
            this.GameServerPort = _GameServerPort;
            this.SteamIDGameServer = _SteamIDGameServer;
        }
        public CSteamID m_Lobby;
        public uint GameServerIP;
        public uint GameServerPort;
        public CSteamID SteamIDGameServer;
    }

    /// <summary>
    /// You MUST joinLobby before you can get info. Autojoin will do this for you. getLobbyInfo should then be called on the join callback?????
    /// </summary>
    /// <param name="m_Lobby"></param>
    /// <returns></returns>
    public gameServer getLobbyInfo(CSteamID m_Lobby)
    {
        //if (autoJoin)
        //    SteamMatchmaking.JoinLobby(m_Lobby);  //A test, maybe we need to join to get info?????????
        SteamAPI.RunCallbacks();
        uint GameServerIP;
        ushort GameServerPort;
        CSteamID SteamIDGameServer;
        bool ret = SteamMatchmaking.GetLobbyGameServer(m_Lobby, out GameServerIP, out GameServerPort, out SteamIDGameServer);
        Console.WriteLine("GetLobbyGameServer(m_Lobby, out GameServerIP, out GameServerPort, out SteamIDGameServer) : " + ret + " -- " + GameServerIP + " -- " + GameServerPort + " -- " + SteamIDGameServer);
        SteamworksDotNetTest.debugString = "GetLobbyGameServer: " + m_Lobby + " -- " + GameServerIP + " -- " + GameServerPort + " -- " + SteamIDGameServer + "\n" + SteamworksDotNetTest.debugString;
        
        return new gameServer(m_Lobby, GameServerIP, GameServerPort, SteamIDGameServer);
    }

    //SteamAPICall_t CreateLobby( ELobbyType eLobbyType, int cMaxMembers );
    public SteamAPICall_t createLobby(ELobbyType eLobbyType, int cMaxMembers)
    {
        return SteamMatchmaking.CreateLobby(eLobbyType, cMaxMembers);
    }

    public SteamAPICall_t joinLobby(CSteamID steamIDLobby)
    {
        return SteamMatchmaking.JoinLobby(steamIDLobby);
    }

    public void RenderOnGUI()
    {
        //GUILayout.BeginArea(new Rect(Screen.width - 120, 0, 120, Screen.height));
        Console.WriteLine("Variables:");
        Console.WriteLine("m_Lobby: " + m_Lobby);
        //GUILayout.EndArea();

        Console.WriteLine("GetFavoriteGameCount() : " + SteamMatchmaking.GetFavoriteGameCount());

        {
            AppId_t AppID;
            uint IP;
            ushort ConnPort;
            ushort QueryPort;
            uint Flags;
            uint LastPlayedOnServer;
            bool ret = SteamMatchmaking.GetFavoriteGame(0, out AppID, out IP, out ConnPort, out QueryPort, out Flags, out LastPlayedOnServer);
            Console.WriteLine("GetFavoriteGame(0, out AppID, out IP, out ConnPort, out QueryPort, out Flags, out LastPlayedOnServer) : " + ret + " -- " + AppID + " -- " + IP + " -- " + ConnPort + " -- " + QueryPort + " -- " + Flags + " -- " + LastPlayedOnServer);
        }

        // 3494815209 = 208.78.165.233 = Valve Payload Server (Virginia srcds150 #1)
        //if (GUILayout.Button("AddFavoriteGame((AppId_t)480, 3494815209, 27015, 27015, Constants.k_unFavoriteFlagFavorite, CurrentUnixTime"))
        {
            uint CurrentUnixTime = (uint)(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds;
            Console.WriteLine("SteamMatchmaking.AddFavoriteGame(" + (AppId_t)480 + ", 3494815209, 27015, 27015, Constants.k_unFavoriteFlagFavorite, " + CurrentUnixTime + ") : " + SteamMatchmaking.AddFavoriteGame((AppId_t)480, 3494815209, 27015, 27015, Constants.k_unFavoriteFlagFavorite, CurrentUnixTime));
        }

        //if (GUILayout.Button("RemoveFavoriteGame((AppId_t)480, 3494815209, 27015, 27015, Constants.k_unFavoriteFlagFavorite)"))
        {
            Console.WriteLine("SteamMatchmaking.RemoveFavoriteGame(" + (AppId_t)480 + ", 3494815209, 27015, 27015, Constants.k_unFavoriteFlagFavorite) : " + SteamMatchmaking.RemoveFavoriteGame((AppId_t)480, 3494815209, 27015, 27015, Constants.k_unFavoriteFlagFavorite));
        }

        //if (GUILayout.Button("RequestLobbyList()"))
        {
            SteamAPICall_t handle = SteamMatchmaking.RequestLobbyList();
            OnLobbyMatchListCallResult.Set(handle);
            Console.WriteLine("SteamMatchmaking.RequestLobbyList() : " + handle);
        }

        //if (GUILayout.Button("AddRequestLobbyListStringFilter(\"SomeStringKey\", \"SomeValue\", ELobbyComparison.k_ELobbyComparisonNotEqual)"))
        {
            SteamMatchmaking.AddRequestLobbyListStringFilter("SomeStringKey", "SomeValue", ELobbyComparison.k_ELobbyComparisonNotEqual);
            Console.WriteLine("SteamMatchmaking.AddRequestLobbyListStringFilter(\"SomeStringKey\", \"SomeValue\", ELobbyComparison.k_ELobbyComparisonNotEqual)");
        }

        //if (GUILayout.Button("AddRequestLobbyListNumericalFilter(\"SomeIntKey\", 0, ELobbyComparison.k_ELobbyComparisonNotEqual)"))
        {
            SteamMatchmaking.AddRequestLobbyListNumericalFilter("SomeIntKey", 0, ELobbyComparison.k_ELobbyComparisonNotEqual);
            Console.WriteLine("SteamMatchmaking.AddRequestLobbyListNumericalFilter(\"SomeIntKey\", 0, ELobbyComparison.k_ELobbyComparisonNotEqual)");
        }

        //if (GUILayout.Button("AddRequestLobbyListNearValueFilter(\"SomeIntKey\", 0)"))
        {
            SteamMatchmaking.AddRequestLobbyListNearValueFilter("SomeIntKey", 0);
            Console.WriteLine("SteamMatchmaking.AddRequestLobbyListNearValueFilter(\"SomeIntKey\", 0)");
        }

        //if (GUILayout.Button("AddRequestLobbyListFilterSlotsAvailable(3)"))
        {
            SteamMatchmaking.AddRequestLobbyListFilterSlotsAvailable(3);
            Console.WriteLine("SteamMatchmaking.AddRequestLobbyListFilterSlotsAvailable(3)");
        }

        //if (GUILayout.Button("AddRequestLobbyListDistanceFilter(ELobbyDistanceFilter.k_ELobbyDistanceFilterWorldwide)"))
        {
            SteamMatchmaking.AddRequestLobbyListDistanceFilter(ELobbyDistanceFilter.k_ELobbyDistanceFilterWorldwide);
            Console.WriteLine("SteamMatchmaking.AddRequestLobbyListDistanceFilter(ELobbyDistanceFilter.k_ELobbyDistanceFilterWorldwide)");
        }

        //if (GUILayout.Button("AddRequestLobbyListResultCountFilter(1)"))
        {
            SteamMatchmaking.AddRequestLobbyListResultCountFilter(1);
            Console.WriteLine("SteamMatchmaking.AddRequestLobbyListResultCountFilter(1)");
        }

        //if (GUILayout.Button("AddRequestLobbyListCompatibleMembersFilter((CSteamID)0)"))
        {
            SteamMatchmaking.AddRequestLobbyListCompatibleMembersFilter((CSteamID)0);
            Console.WriteLine("SteamMatchmaking.AddRequestLobbyListCompatibleMembersFilter((CSteamID)0)");
        }

        //if (GUILayout.Button("GetLobbyByIndex(0)"))
        {
            m_Lobby = SteamMatchmaking.GetLobbyByIndex(0);
            Console.WriteLine("SteamMatchmaking.SteamMatchmaking.GetLobbyByIndex(0) : " + m_Lobby);
        }

        //if (GUILayout.Button("CreateLobby(ELobbyType.k_ELobbyTypePublic, 1)"))
        {
            SteamAPICall_t handle = SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 1);
            OnLobbyCreatedCallResult.Set(handle);
            Console.WriteLine("SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 1) : " + handle);
        }

        //if (GUILayout.Button("JoinLobby(m_Lobby)"))
        {
            SteamAPICall_t handle = SteamMatchmaking.JoinLobby(m_Lobby);
            OnLobbyEnterCallResult.Set(handle);
            Console.WriteLine("SteamMatchmaking.JoinLobby(" + m_Lobby + ") : " + handle);
        }

        //if (GUILayout.Button("LeaveLobby(m_Lobby)"))
        {
            SteamMatchmaking.LeaveLobby(m_Lobby);
            m_Lobby = CSteamID.Nil;
            Console.WriteLine("SteamMatchmaking.LeaveLobby(" + m_Lobby + ")");
        }

        //if (GUILayout.Button("InviteUserToLobby(m_Lobby, SteamUser.GetSteamID())"))
        {
            Console.WriteLine("SteamMatchmaking.InviteUserToLobby(" + m_Lobby + ", SteamUser.GetSteamID()) : " + SteamMatchmaking.InviteUserToLobby(m_Lobby, SteamUser.GetSteamID()));
        }

        Console.WriteLine("GetNumLobbyMembers(m_Lobby) : " + SteamMatchmaking.GetNumLobbyMembers(m_Lobby));

        Console.WriteLine("GetLobbyMemberByIndex(m_Lobby, 0) : " + SteamMatchmaking.GetLobbyMemberByIndex(m_Lobby, 0));

        Console.WriteLine("SteamMatchmaking.GetLobbyData(m_Lobby, \"name\") : " + SteamMatchmaking.GetLobbyData(m_Lobby, "name"));

        //if (GUILayout.Button("SetLobbyData(m_Lobby, \"name\", \"Test Lobby!\")"))
        {
            Console.WriteLine("SteamMatchmaking.SetLobbyData(" + m_Lobby + ", \"name\", \"Test Lobby!\") : " + SteamMatchmaking.SetLobbyData(m_Lobby, "name", "Test Lobby!"));
        }

        Console.WriteLine("GetLobbyDataCount(m_Lobby) : " + SteamMatchmaking.GetLobbyDataCount(m_Lobby));

        {
            string Key;
            string Value;
            bool ret = SteamMatchmaking.GetLobbyDataByIndex(m_Lobby, 0, out Key, 255, out Value, 255);
            Console.WriteLine("SteamMatchmaking.GetLobbyDataByIndex(m_Lobby, 0, out Key, 255, out Value, 255) : " + ret + " -- " + Key + " -- " + Value);
        }

        //if (GUILayout.Button("DeleteLobbyData(m_Lobby, \"name\")"))
        {
            Console.WriteLine("SteamMatchmaking.DeleteLobbyData(" + m_Lobby + ", \"name\") : " + SteamMatchmaking.DeleteLobbyData(m_Lobby, "name"));
        }

        {
            Console.WriteLine("SteamMatchmaking.GetLobbyMemberData(m_Lobby, SteamUser.GetSteamID(), \"test\") : " + SteamMatchmaking.GetLobbyMemberData(m_Lobby, SteamUser.GetSteamID(), "test"));
        }

        //if (GUILayout.Button("SetLobbyMemberData(m_Lobby, \"test\", \"This is a test Key!\")"))
        {
            SteamMatchmaking.SetLobbyMemberData(m_Lobby, "test", "This is a test Key!");
            Console.WriteLine("SteamMatchmaking.SetLobbyMemberData(" + m_Lobby + ", \"test\", \"This is a test Key!\")");
        }

        //if (GUILayout.Button("SendLobbyChatMsg(m_Lobby, MsgBody, MsgBody.Length)"))
        {
            byte[] MsgBody = System.Text.Encoding.UTF8.GetBytes("Test Message!");
            Console.WriteLine("SteamMatchmaking.SendLobbyChatMsg(" + m_Lobby + ", MsgBody, MsgBody.Length) : " + SteamMatchmaking.SendLobbyChatMsg(m_Lobby, MsgBody, MsgBody.Length));
        }

        //GetLobbyChatEntry(CSteamID steamIDLobby, int iChatID, out CSteamID pSteamIDUser, byte[] pvData, int cubData, out EChatEntryType peChatEntryType); // Only called from within OnLobbyChatMsg!

        //if (GUILayout.Button("RequestLobbyData(m_Lobby)"))
        {
            Console.WriteLine("SteamMatchmaking.RequestLobbyData(" + m_Lobby + ") : " + SteamMatchmaking.RequestLobbyData(m_Lobby));
        }

        //if (GUILayout.Button("SetLobbyGameServer(m_Lobby, 2130706433, 1337, CSteamID.NonSteamGS)"))
        {
            SteamMatchmaking.SetLobbyGameServer(m_Lobby, 2130706433, 1337, CSteamID.NonSteamGS); //127.0.0.1
            Console.WriteLine("SteamMatchmaking.SetLobbyGameServer(" + m_Lobby + ", 2130706433, 1337, CSteamID.NonSteamGS)");
        }

        {
            uint GameServerIP;
            ushort GameServerPort;
            CSteamID SteamIDGameServer;
            bool ret = SteamMatchmaking.GetLobbyGameServer(m_Lobby, out GameServerIP, out GameServerPort, out SteamIDGameServer);
            Console.WriteLine("GetLobbyGameServer(m_Lobby, out GameServerIP, out GameServerPort, out SteamIDGameServer) : " + ret + " -- " + GameServerIP + " -- " + GameServerPort + " -- " + SteamIDGameServer);
        }

        //if (GUILayout.Button("SetLobbyMemberLimit(m_Lobby, 6)"))
        {
            Console.WriteLine("SteamMatchmaking.SteamMatchmaking.SetLobbyMemberLimit(" + m_Lobby + ", 6) : " + SteamMatchmaking.SetLobbyMemberLimit(m_Lobby, 6));
        }

        Console.WriteLine("GetLobbyMemberLimit(m_Lobby) : " + SteamMatchmaking.GetLobbyMemberLimit(m_Lobby));

        //if (GUILayout.Button("SetLobbyType(m_Lobby, ELobbyType.k_ELobbyTypePublic))"))
        {
            Console.WriteLine("SteamMatchmaking.SetLobbyType(" + m_Lobby + ", ELobbyType.k_ELobbyTypePublic)) : " + SteamMatchmaking.SetLobbyType(m_Lobby, ELobbyType.k_ELobbyTypePublic));
        }

        //if (GUILayout.Button("SetLobbyJoinable(m_Lobby, true)"))
        {
            Console.WriteLine("SteamMatchmaking.SetLobbyJoinable(" + m_Lobby + ", true) : " + SteamMatchmaking.SetLobbyJoinable(m_Lobby, true));
        }

        //if (GUILayout.Button("GetLobbyOwner(m_Lobby)"))
        {
            Console.WriteLine("SteamMatchmaking.GetLobbyOwner(" + m_Lobby + ") : " + SteamMatchmaking.GetLobbyOwner(m_Lobby));
        }

        //if (GUILayout.Button("SetLobbyOwner(m_Lobby, SteamUser.GetSteamID())"))
        {
            Console.WriteLine("SteamMatchmaking.SteamMatchmaking.SetLobbyOwner(" + m_Lobby + ", SteamUser.GetSteamID()) : " + SteamMatchmaking.SetLobbyOwner(m_Lobby, SteamUser.GetSteamID()));
        }

        //if (GUILayout.Button("SetLinkedLobby(m_Lobby, m_Lobby)"))
        {
            Console.WriteLine("SteamMatchmaking.SteamMatchmaking.SetLinkedLobby(" + m_Lobby + ", " + m_Lobby + ") : " + SteamMatchmaking.SetLinkedLobby(m_Lobby, m_Lobby));
        }
    }

    void OnFavoritesListChanged(FavoritesListChanged_t pCallback)
    {
        Console.WriteLine("[" + FavoritesListChanged_t.k_iCallback + " - FavoritesListChanged] - " + pCallback.m_nIP + " -- " + pCallback.m_nQueryPort + " -- " + pCallback.m_nConnPort + " -- " + pCallback.m_nAppID + " -- " + pCallback.m_nFlags + " -- " + pCallback.m_bAdd + " -- " + pCallback.m_unAccountId);
    }

    void OnLobbyInvite(LobbyInvite_t pCallback)
    {
        Console.WriteLine("[" + LobbyInvite_t.k_iCallback + " - LobbyInvite] - " + pCallback.m_ulSteamIDUser + " -- " + pCallback.m_ulSteamIDLobby + " -- " + pCallback.m_ulGameID);
    }

    void OnLobbyEnter(LobbyEnter_t pCallback)
    {
        Console.WriteLine("[" + LobbyEnter_t.k_iCallback + " - LobbyEnter] - " + pCallback.m_ulSteamIDLobby + " -- " + pCallback.m_rgfChatPermissions + " -- " + pCallback.m_bLocked + " -- " + pCallback.m_EChatRoomEnterResponse);
        SteamMatchmakingTest.gameServer tempServer = SteamManager.SteamMatchmakingClass.getLobbyInfo(m_Lobby);  //(CSteamID)pCallback.m_steamIDLobby.m_SteamID   //pCallback.m_steamIDLobby
        Console.WriteLine("[" + GameLobbyJoinRequested_t.k_iCallback + " - SERVER INFO: IP] - " + tempServer.GameServerIP + " -- Port: " + tempServer.GameServerPort);
        m_Lobby = (CSteamID)pCallback.m_ulSteamIDLobby;
    }

    void OnLobbyEnter(LobbyEnter_t pCallback, bool bIOFailure)
    {
        Console.WriteLine("[" + LobbyEnter_t.k_iCallback + " - LobbyEnter] - " + pCallback.m_ulSteamIDLobby + " -- " + pCallback.m_rgfChatPermissions + " -- " + pCallback.m_bLocked + " -- " + pCallback.m_EChatRoomEnterResponse);
        m_Lobby = (CSteamID)pCallback.m_ulSteamIDLobby;
    }

    void OnLobbyDataUpdate(LobbyDataUpdate_t pCallback)
    {
        Console.WriteLine("[" + LobbyDataUpdate_t.k_iCallback + " - LobbyDataUpdate] - " + pCallback.m_ulSteamIDLobby + " -- " + pCallback.m_ulSteamIDMember + " -- " + pCallback.m_bSuccess);
    }

    void OnLobbyChatUpdate(LobbyChatUpdate_t pCallback)
    {
        Console.WriteLine("[" + LobbyChatUpdate_t.k_iCallback + " - LobbyChatUpdate] - " + pCallback.m_ulSteamIDLobby + " -- " + pCallback.m_ulSteamIDUserChanged + " -- " + pCallback.m_ulSteamIDMakingChange + " -- " + pCallback.m_rgfChatMemberStateChange);
    }

    void OnLobbyChatMsg(LobbyChatMsg_t pCallback)
    {
        Console.WriteLine("[" + LobbyChatMsg_t.k_iCallback + " - LobbyChatMsg] - " + pCallback.m_ulSteamIDLobby + " -- " + pCallback.m_ulSteamIDUser + " -- " + pCallback.m_eChatEntryType + " -- " + pCallback.m_iChatID);
        CSteamID SteamIDUser;
        byte[] Data = new byte[4096];
        EChatEntryType ChatEntryType;
        int ret = SteamMatchmaking.GetLobbyChatEntry((CSteamID)pCallback.m_ulSteamIDLobby, (int)pCallback.m_iChatID, out SteamIDUser, Data, Data.Length, out ChatEntryType);
        Console.WriteLine("SteamMatchmaking.GetLobbyChatEntry(" + (CSteamID)pCallback.m_ulSteamIDLobby + ", " + (int)pCallback.m_iChatID + ", out SteamIDUser, Data, Data.Length, out ChatEntryType) : " + ret + " -- " + SteamIDUser + " -- " + System.Text.Encoding.UTF8.GetString(Data) + " -- " + ChatEntryType);
    }

    void OnLobbyGameCreated(LobbyGameCreated_t pCallback)
    {
        Console.WriteLine("[" + LobbyGameCreated_t.k_iCallback + " - LobbyGameCreated] - " + pCallback.m_ulSteamIDLobby + " -- " + pCallback.m_ulSteamIDGameServer + " -- " + pCallback.m_unIP + " -- " + pCallback.m_usPort);
        SteamworksDotNetTest.lobbyHandle = (CSteamID)pCallback.m_ulSteamIDLobby; //getLobbyInfo((CSteamID)pCallback.m_ulSteamIDLobby);
        SteamworksDotNetTest.debugString = "[" + LobbyGameCreated_t.k_iCallback + " - LobbyGameCreated] - " + pCallback.m_ulSteamIDLobby + " -- " + pCallback.m_ulSteamIDGameServer + " -- " + pCallback.m_unIP + " -- " + pCallback.m_usPort + "\n" + debugString;
        //SteamMatchmakingTest.gameServer tempServer1 = SteamManager.SteamMatchmakingClass.getLobbyInfo((CSteamID)pCallback.m_ulSteamIDLobby);  //(CSteamID)pCallback.m_steamIDLobby.m_SteamID   //pCallback.m_steamIDLobby
        SteamMatchmakingTest.gameServer tempServer2 = SteamManager.SteamMatchmakingClass.getLobbyInfo((CSteamID)pCallback.m_ulSteamIDLobby);  //(CSteamID)pCallback.m_steamIDLobby.m_SteamID   //pCallback.m_steamIDLobby
        this.joinLobby(m_Lobby);
    }

    void OnLobbyMatchList(LobbyMatchList_t pCallback, bool bIOFailure)
    {
        Console.WriteLine("[" + LobbyMatchList_t.k_iCallback + " - LobbyMatchList] - " + pCallback.m_nLobbiesMatching);
    }

    void OnLobbyKicked(LobbyKicked_t pCallback)
    {
        Console.WriteLine("[" + LobbyKicked_t.k_iCallback + " - LobbyKicked] - " + pCallback.m_ulSteamIDLobby + " -- " + pCallback.m_ulSteamIDAdmin + " -- " + pCallback.m_bKickedDueToDisconnect);
    }

    void OnLobbyCreated(LobbyCreated_t pCallback, bool bIOFailure)
    {
        Console.WriteLine("[" + LobbyCreated_t.k_iCallback + " - LobbyCreated] - " + pCallback.m_eResult + " -- " + pCallback.m_ulSteamIDLobby);
        m_Lobby = (CSteamID)pCallback.m_ulSteamIDLobby;
        this.setGameServerSettings(m_Lobby, "52.27.130.37", "1244");
    }

    void OnFavoritesListAccountsUpdated(FavoritesListAccountsUpdated_t pCallback)
    {
        Console.WriteLine("[" + FavoritesListAccountsUpdated_t.k_iCallback + " - FavoritesListAccountsUpdated] - " + pCallback.m_eResult);
    }

}
