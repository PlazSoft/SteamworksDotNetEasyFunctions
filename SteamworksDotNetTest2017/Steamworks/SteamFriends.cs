using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Steamworks;
using Microsoft.Xna.Framework.Graphics;



    public class SteamFriendsA
    {
        private CSteamID m_Friend;
        private CSteamID m_Clan;
        private CSteamID m_CoPlayFriend;
        private Texture2D m_SmallAvatar;
        private Texture2D m_MediumAvatar;
        private Texture2D m_LargeAvatar;

        /// <summary>
        /// Tracks whether a gameLobby join is in progress.
        /// </summary>
        public static bool waitingForLobbyJoin = false;

        protected Callback<PersonaStateChange_t> m_PersonaStateChange;
        protected Callback<GameOverlayActivated_t> m_GameOverlayActivated;
        protected Callback<GameServerChangeRequested_t> m_GameServerChangeRequested;
        protected Callback<GameLobbyJoinRequested_t> m_GameLobbyJoinRequested;
        protected Callback<AvatarImageLoaded_t> m_AvatarImageLoaded;
        protected CallResult<ClanOfficerListResponse_t> OnFriendRichPresenceCallResult;
        protected Callback<FriendRichPresenceUpdate_t> m_FriendRichPresenceUpdate;
        protected Callback<GameRichPresenceJoinRequested_t> m_GameRichPresenceJoinRequested;
        protected Callback<GameConnectedClanChatMsg_t> m_GameConnectedClanChatMsg;
        protected Callback<GameConnectedChatJoin_t> m_GameConnectedChatJoin;
        protected Callback<GameConnectedChatLeave_t> m_GameConnectedChatLeave;
        protected Callback<GameConnectedFriendChatMsg_t> m_GameConnectedFriendChatMsg;

        private CallResult<DownloadClanActivityCountsResult_t> OnDownloadClanActivityCountsResultCallResult;
        private CallResult<JoinClanChatRoomCompletionResult_t> OnJoinClanChatRoomCompletionResultCallResult;
        private CallResult<FriendsGetFollowerCount_t> OnFriendsGetFollowerCountCallResult;
        private CallResult<FriendsIsFollowing_t> OnFriendsIsFollowingCallResult;
        private CallResult<FriendsEnumerateFollowingList_t> OnFriendsEnumerateFollowingListCallResult;
        private CallResult<SetPersonaNameResponse_t> OnSetPersonaNameResponseCallResult;

        public SteamFriendsA()
        {
            m_PersonaStateChange = Callback<PersonaStateChange_t>.Create(OnPersonaStateChange);
            m_GameOverlayActivated = Callback<GameOverlayActivated_t>.Create(OnGameOverlayActivated);
            m_GameServerChangeRequested = Callback<GameServerChangeRequested_t>.Create(OnGameServerChangeRequested);
            m_GameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
            m_AvatarImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnAvatarImageLoaded);
            m_FriendRichPresenceUpdate = Callback<FriendRichPresenceUpdate_t>.Create(OnFriendRichPresenceUpdate);
            m_GameRichPresenceJoinRequested = Callback<GameRichPresenceJoinRequested_t>.Create(OnGameRichPresenceJoinRequested);
            m_GameConnectedClanChatMsg = Callback<GameConnectedClanChatMsg_t>.Create(OnGameConnectedClanChatMsg);
            m_GameConnectedChatJoin = Callback<GameConnectedChatJoin_t>.Create(OnGameConnectedChatJoin);
            m_GameConnectedChatLeave = Callback<GameConnectedChatLeave_t>.Create(OnGameConnectedChatLeave);
            m_GameConnectedFriendChatMsg = Callback<GameConnectedFriendChatMsg_t>.Create(OnGameConnectedFriendChatMsg);

            OnFriendRichPresenceCallResult = CallResult<ClanOfficerListResponse_t>.Create(OnClanOfficerListResponse);
            OnDownloadClanActivityCountsResultCallResult = CallResult<DownloadClanActivityCountsResult_t>.Create(OnDownloadClanActivityCountsResult);
            OnJoinClanChatRoomCompletionResultCallResult = CallResult<JoinClanChatRoomCompletionResult_t>.Create(OnJoinClanChatRoomCompletionResult);
            OnFriendsGetFollowerCountCallResult = CallResult<FriendsGetFollowerCount_t>.Create(OnFriendsGetFollowerCount);
            OnFriendsIsFollowingCallResult = CallResult<FriendsIsFollowing_t>.Create(OnFriendsIsFollowing);
            OnFriendsEnumerateFollowingListCallResult = CallResult<FriendsEnumerateFollowingList_t>.Create(OnFriendsEnumerateFollowingList);
            OnSetPersonaNameResponseCallResult = CallResult<SetPersonaNameResponse_t>.Create(OnSetPersonaNameResponse);
        }

        //public void RenderOnGUI(SteamTest.EGUIState state)
        //{
        //    GUILayout.BeginArea(new Rect(Screen.width - 200, 0, 200, Screen.height));
        //    Console.WriteLine("Variables:");
        //    Console.WriteLine("m_Friend: " + m_Friend);
        //    Console.WriteLine("m_Clan: " + m_Clan);
        //    Console.WriteLine("m_SmallAvatar:");
        //    Console.WriteLine(m_SmallAvatar);
        //    Console.WriteLine("m_MediumAvatar:");
        //    Console.WriteLine(m_MediumAvatar);
        //    Console.WriteLine("m_LargeAvatar:");
        //    // This is an example of how to flip a Texture2D when using OnGUI().
        //    if (m_LargeAvatar)
        //    {
        //        GUI.DrawTexture(new Rect(0, m_LargeAvatar.height * 2 + 85, m_LargeAvatar.width, -m_LargeAvatar.height), m_LargeAvatar);
        //    }
        //    GUILayout.EndArea();

        //    if (state == SteamTest.EGUIState.SteamFriends)
        //    {
        //        RenderPageOne();
        //    }
        //    else
        //    {
        //        RenderPageTwo();
        //    }
        //}

        private void RenderPageOne()
        {
            //Console.WriteLine("SteamFriends.GetPersonaName() : " + SteamFriends.GetPersonaName());

            // if (GUILayout.Button("SteamFriends.SetPersonaName(SteamFriends.GetPersonaName())"))
            {
                SteamAPICall_t handle = SteamFriends.SetPersonaName(SteamFriends.GetPersonaName());
                OnSetPersonaNameResponseCallResult.Set(handle);
                Console.WriteLine("SteamFriends.SetPersonaName(" + SteamFriends.GetPersonaName() + ") : " + handle);
            }

            Console.WriteLine("SteamFriends.GetPersonaState() : " + SteamFriends.GetPersonaState());
            Console.WriteLine("SteamFriends.GetFriendCount(k_EFriendFlagImmediate) : " + SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate));
            if (SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate) == 0)
            {
                Console.WriteLine("You must have atleast one friend to use this Test");
                return;
            }

            m_Friend = SteamFriends.GetFriendByIndex(0, EFriendFlags.k_EFriendFlagImmediate);
            Console.WriteLine("SteamFriends.GetFriendByIndex(0, k_EFriendFlagImmediate) : " + m_Friend);
            Console.WriteLine("SteamFriends.GetFriendRelationship(m_Friend) : " + SteamFriends.GetFriendRelationship(m_Friend));
            Console.WriteLine("SteamFriends.GetFriendPersonaState(m_Friend) : " + SteamFriends.GetFriendPersonaState(m_Friend));
            Console.WriteLine("SteamFriends.GetFriendPersonaName(m_Friend) : " + SteamFriends.GetFriendPersonaName(m_Friend));

            {
                var fgi = new FriendGameInfo_t();
                bool ret = SteamFriends.GetFriendGamePlayed(m_Friend, out fgi);
                Console.WriteLine("SteamFriends.GetFriendGamePlayed(m_Friend, out fgi) : " + ret + " -- " + fgi.m_gameID + " -- " + fgi.m_unGameIP + " -- " + fgi.m_usGamePort + " -- " + fgi.m_usQueryPort + " -- " + fgi.m_steamIDLobby);
            }


            Console.WriteLine("SteamFriends.GetFriendPersonaNameHistory(m_Friend, 1) : " + SteamFriends.GetFriendPersonaNameHistory(m_Friend, 1));
            Console.WriteLine("SteamFriends.GetFriendSteamLevel(m_Friend) : " + SteamFriends.GetFriendSteamLevel(m_Friend));
            Console.WriteLine("SteamFriends.GetPlayerNickname(m_Friend) : " + SteamFriends.GetPlayerNickname(m_Friend));

            {
                int FriendsGroupCount = SteamFriends.GetFriendsGroupCount();
                Console.WriteLine("SteamFriends.GetFriendsGroupCount() : " + FriendsGroupCount);

                if (FriendsGroupCount > 0)
                {
                    FriendsGroupID_t FriendsGroupID = SteamFriends.GetFriendsGroupIDByIndex(0);
                    Console.WriteLine("SteamFriends.GetFriendsGroupIDByIndex(0) : " + FriendsGroupID);
                    Console.WriteLine("SteamFriends.GetFriendsGroupName(FriendsGroupID) : " + SteamFriends.GetFriendsGroupName(FriendsGroupID));

                    int FriendsGroupMembersCount = SteamFriends.GetFriendsGroupMembersCount(FriendsGroupID);
                    Console.WriteLine("SteamFriends.GetFriendsGroupMembersCount(FriendsGroupID) : " + FriendsGroupMembersCount);

                    if (FriendsGroupMembersCount > 0)
                    {
                        CSteamID[] FriendsGroupMembersList = new CSteamID[FriendsGroupMembersCount];
                        SteamFriends.GetFriendsGroupMembersList(FriendsGroupID, FriendsGroupMembersList, FriendsGroupMembersCount);
                        Console.WriteLine("SteamFriends.GetFriendsGroupMembersList(FriendsGroupID, FriendsGroupMembersList, FriendsGroupMembersCount) : " + FriendsGroupMembersList[0]);
                    }
                }
            }

            Console.WriteLine("SteamFriends.HasFriend(m_Friend, k_EFriendFlagImmediate) : " + SteamFriends.HasFriend(m_Friend, EFriendFlags.k_EFriendFlagImmediate));

            Console.WriteLine("SteamFriends.GetClanCount() : " + SteamFriends.GetClanCount());
            if (SteamFriends.GetClanCount() == 0)
            {
                Console.WriteLine("You must have atleast one clan to use this Test");
                return;
            }

            m_Clan = SteamFriends.GetClanByIndex(0);
            Console.WriteLine("SteamFriends.GetClanByIndex(0) : " + m_Clan);
            Console.WriteLine("SteamFriends.GetClanName(m_Clan) : " + SteamFriends.GetClanName(m_Clan));
            Console.WriteLine("SteamFriends.GetClanTag(m_Clan) : " + SteamFriends.GetClanTag(m_Clan));


            {
                int Online;
                int InGame;
                int Chatting;
                bool ret = SteamFriends.GetClanActivityCounts(m_Clan, out Online, out InGame, out Chatting);
                Console.WriteLine("SteamFriends.GetClanActivityCounts(m_Clan, out Online, out InGame, out Chatting) : " + ret + " -- " + Online + " -- " + InGame + " -- " + Chatting);
            }

            //// if (GUILayout.Button("SteamFriends.DownloadClanActivityCounts(m_Clans, 2)"))
            {
                CSteamID[] Clans = { m_Clan, new CSteamID(103582791434672565) }; // m_Clan, Steam Universe
                SteamAPICall_t handle = SteamFriends.DownloadClanActivityCounts(Clans, 2);
                OnDownloadClanActivityCountsResultCallResult.Set(handle); // This call never seems to produce a callback.
                Console.WriteLine("SteamFriends.DownloadClanActivityCounts(" + Clans + ", 2) : " + handle);
            }

            {
                int FriendCount = SteamFriends.GetFriendCountFromSource(m_Clan);
                Console.WriteLine("SteamFriends.GetFriendCountFromSource(m_Clan) : " + FriendCount);

                if (FriendCount > 0)
                {
                    Console.WriteLine("SteamFriends.GetFriendFromSourceByIndex(m_Clan, 0) : " + SteamFriends.GetFriendFromSourceByIndex(m_Clan, 0));
                }
            }

            Console.WriteLine("SteamFriends.IsUserInSource(m_Friend, m_Clan) : " + SteamFriends.IsUserInSource(m_Friend, m_Clan));

            //// if (GUILayout.Button("SteamFriends.SetInGameVoiceSpeaking(SteamUser.GetSteamID(), false)"))
            {
                SteamFriends.SetInGameVoiceSpeaking(SteamUser.GetSteamID(), false);
                Console.WriteLine("SteamClient.SetInGameVoiceSpeaking(" + SteamUser.GetSteamID() + ", false);");
            }

            //// if (GUILayout.Button("SteamFriends.ActivateGameOverlay(\"Friends\")"))
            {
                SteamFriends.ActivateGameOverlay("Friends");
                Console.WriteLine("SteamClient.ActivateGameOverlay(\"Friends\")");
            }

            //// if (GUILayout.Button("SteamFriends.ActivateGameOverlayToUser(\"friendadd\", 76561197991230424)"))
            {
                SteamFriends.ActivateGameOverlayToUser("friendadd", new CSteamID(76561197991230424)); // rlabrecque
                Console.WriteLine("SteamClient.ActivateGameOverlay(\"friendadd\", 76561197991230424)");
            }

            //// if (GUILayout.Button("SteamFriends.ActivateGameOverlayToWebPage(\"http://google.com\")"))
            {
                SteamFriends.ActivateGameOverlayToWebPage("http://google.com");
                Console.WriteLine("SteamClient.ActivateGameOverlay(\"http://google.com\")");
            }

            //// if (GUILayout.Button("SteamFriends.ActivateGameOverlayToStore(440, k_EOverlayToStoreFlag_None)"))
            {
                SteamFriends.ActivateGameOverlayToStore((AppId_t)440, EOverlayToStoreFlag.k_EOverlayToStoreFlag_None); // 440 = TF2
                Console.WriteLine("SteamClient.ActivateGameOverlay(440, k_EOverlayToStoreFlag_None)");
            }

            //// if (GUILayout.Button("SteamFriends.SetPlayedWith(76561197991230424)"))
            {
                SteamFriends.SetPlayedWith(new CSteamID(76561197991230424)); //rlabrecque
                Console.WriteLine("SteamClient.SetPlayedWith(76561197991230424)");
            }

            //// if (GUILayout.Button("SteamFriends.ActivateGameOverlayInviteDialog(76561197991230424)"))
            {
                SteamFriends.ActivateGameOverlayInviteDialog(new CSteamID(76561197991230424)); //rlabrecque
                Console.WriteLine("SteamClient.ActivateGameOverlayInviteDialog(76561197991230424)");
            }

            ////// if (GUILayout.Button("SteamFriends.GetSmallFriendAvatar(m_Friend)"))
            //{
            //    int FriendAvatar = SteamFriends.GetSmallFriendAvatar(m_Friend);
            //    Console.WriteLine("SteamFriends.GetSmallFriendAvatar(" + m_Friend + ") - " + FriendAvatar);

            //    uint ImageWidth;
            //    uint ImageHeight;
            //    bool ret = SteamUtils.GetImageSize(FriendAvatar, out ImageWidth, out ImageHeight);

            //    if (ret && ImageWidth > 0 && ImageHeight > 0)
            //    {
            //        byte[] Image = new byte[ImageWidth * ImageHeight * 4];

            //        ret = SteamUtils.GetImageRGBA(FriendAvatar, Image, (int)(ImageWidth * ImageHeight * 4));

            //        m_SmallAvatar = new Texture2D((int)ImageWidth, (int)ImageHeight, TextureFormat.RGBA32, false, true);
            //        m_SmallAvatar.LoadRawTextureData(Image); // The image is upside down! "@ares_p: in Unity all texture data starts from "bottom" (OpenGL convention)"
            //        m_SmallAvatar.Apply();
            //    }
            //}

            ////// if (GUILayout.Button("SteamFriends.GetMediumFriendAvatar(m_Friend)"))
            //{
            //    int FriendAvatar = SteamFriends.GetMediumFriendAvatar(m_Friend);
            //    Console.WriteLine("SteamFriends.GetMediumFriendAvatar(" + m_Friend + ") - " + FriendAvatar);

            //    uint ImageWidth;
            //    uint ImageHeight;
            //    bool ret = SteamUtils.GetImageSize(FriendAvatar, out ImageWidth, out ImageHeight);

            //    if (ret && ImageWidth > 0 && ImageHeight > 0)
            //    {
            //        byte[] Image = new byte[ImageWidth * ImageHeight * 4];

            //        ret = SteamUtils.GetImageRGBA(FriendAvatar, Image, (int)(ImageWidth * ImageHeight * 4));
            //        m_MediumAvatar = new Texture2D((int)ImageWidth, (int)ImageHeight, TextureFormat.RGBA32, false, true);
            //        m_MediumAvatar.LoadRawTextureData(Image);
            //        m_MediumAvatar.Apply();
            //    }
            //}

            ////// if (GUILayout.Button("SteamFriends.GetLargeFriendAvatar(m_Friend)"))
            //{
            //    int FriendAvatar = SteamFriends.GetLargeFriendAvatar(m_Friend);
            //    Console.WriteLine("SteamFriends.GetLargeFriendAvatar(" + m_Friend + ") - " + FriendAvatar);

            //    uint ImageWidth;
            //    uint ImageHeight;
            //    bool ret = SteamUtils.GetImageSize(FriendAvatar, out ImageWidth, out ImageHeight);

            //    if (ret && ImageWidth > 0 && ImageHeight > 0)
            //    {
            //        byte[] Image = new byte[ImageWidth * ImageHeight * 4];

            //        ret = SteamUtils.GetImageRGBA(FriendAvatar, Image, (int)(ImageWidth * ImageHeight * 4));
            //        if (ret)
            //        {
            //            m_LargeAvatar = new Texture2D((int)ImageWidth, (int)ImageHeight, TextureFormat.RGBA32, false, true);
            //            m_LargeAvatar.LoadRawTextureData(Image);
            //            m_LargeAvatar.Apply();
            //        }
            //    }
            //}
        }

        private void RenderPageTwo()
        {
            //// if (GUILayout.Button("SteamFriends.RequestUserInformation(m_Friend, false)"))
            {
                Console.WriteLine("SteamFriends.RequestUserInformation(" + m_Friend + ", false) - " + SteamFriends.RequestUserInformation(m_Friend, false));
            }

            //// if (GUILayout.Button("SteamFriends.RequestClanOfficerList(m_Clan)"))
            {
                SteamAPICall_t handle = SteamFriends.RequestClanOfficerList(m_Clan);
                OnFriendRichPresenceCallResult.Set(handle);
                Console.WriteLine("SteamFriends.RequestClanOfficerList(" + m_Clan + ") - " + handle);
            }

            Console.WriteLine("SteamFriends.GetClanOwner(m_Clan) : " + SteamFriends.GetClanOwner(m_Clan));
            Console.WriteLine("SteamFriends.GetClanOfficerCount(m_Clan) : " + SteamFriends.GetClanOfficerCount(m_Clan));
            Console.WriteLine("SteamFriends.GetClanOfficerByIndex(m_Clan, 0) : " + SteamFriends.GetClanOfficerByIndex(m_Clan, 0));
            Console.WriteLine("SteamFriends.GetUserRestrictions() : " + SteamFriends.GetUserRestrictions());

            //// if (GUILayout.Button("SteamFriends.SetRichPresence(\"status\", \"Testing 1.. 2.. 3..\")"))
            {
                Console.WriteLine("SteamFriends.SetRichPresence(\"status\", \"Testing 1.. 2.. 3..\") - " + SteamFriends.SetRichPresence("status", "Testing 1.. 2.. 3.."));
            }

            //// if (GUILayout.Button("SteamFriends.ClearRichPresence()"))
            {
                SteamFriends.ClearRichPresence();
                Console.WriteLine("SteamFriends.ClearRichPresence()");
            }

            Console.WriteLine("SteamFriends.GetFriendRichPresence(SteamUser.GetSteamID(), \"status\") : " + SteamFriends.GetFriendRichPresence(SteamUser.GetSteamID(), "status"));

            Console.WriteLine("SteamFriends.GetFriendRichPresenceKeyCount(SteamUser.GetSteamID()) : " + SteamFriends.GetFriendRichPresenceKeyCount(SteamUser.GetSteamID()));
            Console.WriteLine("SteamFriends.GetFriendRichPresenceKeyByIndex(SteamUser.GetSteamID(), 0) : " + SteamFriends.GetFriendRichPresenceKeyByIndex(SteamUser.GetSteamID(), 0));

            //// if (GUILayout.Button("SteamFriends.RequestFriendRichPresence(m_Friend)"))
            {
                SteamFriends.RequestFriendRichPresence(m_Friend);
                Console.WriteLine("SteamFriends.RequestFriendRichPresence(" + m_Friend + ")");
            }

           // // if (GUILayout.Button("SteamFriends.InviteUserToGame(SteamUser.GetSteamID(), \"testing\")"))
            {
                Console.WriteLine("SteamFriends.RequestFriendRichPresence(" + SteamUser.GetSteamID() + ", \"testing\") - " + SteamFriends.InviteUserToGame(SteamUser.GetSteamID(), "testing"));
            }

            Console.WriteLine("SteamFriends.GetCoplayFriendCount() : " + SteamFriends.GetCoplayFriendCount());
            if (SteamFriends.GetCoplayFriendCount() == 0)
            {
                Console.WriteLine("You must have atleast one clan to use this Test");
                return;
            }

            m_CoPlayFriend = SteamFriends.GetCoplayFriend(0);
            Console.WriteLine("SteamFriends.GetCoplayFriend(0) : " + m_CoPlayFriend);
            Console.WriteLine("SteamFriends.GetFriendCoplayTime(m_CoPlayFriend) : " + SteamFriends.GetFriendCoplayTime(m_CoPlayFriend));
            Console.WriteLine("SteamFriends.GetFriendCoplayGame(m_CoPlayFriend) : " + SteamFriends.GetFriendCoplayGame(m_CoPlayFriend));

            // if (GUILayout.Button("SteamFriends.JoinClanChatRoom(m_Clan)"))
            {
                SteamAPICall_t handle = SteamFriends.JoinClanChatRoom(m_Clan);
                OnJoinClanChatRoomCompletionResultCallResult.Set(handle);
                Console.WriteLine("SteamFriends.JoinClanChatRoom(m_Clan) - " + handle);
            }

            //// if (GUILayout.Button("SteamFriends.LeaveClanChatRoom(m_Clan)"))
            {
                Console.WriteLine("SteamFriends.LeaveClanChatRoom(m_Clan) - " + SteamFriends.LeaveClanChatRoom(m_Clan));
            }

            Console.WriteLine("SteamFriends.GetClanChatMemberCount(m_Clan) : " + SteamFriends.GetClanChatMemberCount(m_Clan));
            Console.WriteLine("SteamFriends.GetChatMemberByIndex(m_Clan, 0) : " + SteamFriends.GetChatMemberByIndex(m_Clan, 0));

           // // if (GUILayout.Button("SteamFriends.SendClanChatMessage(m_Clan, \"Test\")"))
            {
                Console.WriteLine("SteamFriends.SendClanChatMessage(m_Clan, \"Test\") - " + SteamFriends.SendClanChatMessage(m_Clan, "Test"));
            }

            //Console.WriteLine("SteamFriends.GetClanChatMessage() : " + SteamFriends.GetClanChatMessage()); // N/A - Must be called from within the callback OnGameConnectedClanChatMsg

            Console.WriteLine("SteamFriends.IsClanChatAdmin(m_Clan, SteamFriends.GetChatMemberByIndex(m_Clan, 0)) : " + SteamFriends.IsClanChatAdmin(m_Clan, SteamFriends.GetChatMemberByIndex(m_Clan, 0)));
            Console.WriteLine("SteamFriends.IsClanChatWindowOpenInSteam(m_Clan) - " + SteamFriends.IsClanChatWindowOpenInSteam(m_Clan));

            //// if (GUILayout.Button("SteamFriends.OpenClanChatWindowInSteam(m_Clan)"))
            {
                Console.WriteLine("SteamFriends.OpenClanChatWindowInSteam(" + m_Clan + ") - " + SteamFriends.OpenClanChatWindowInSteam(m_Clan));
            }

           // // if (GUILayout.Button("SteamFriends.CloseClanChatWindowInSteam(m_Clan)"))
            {
                Console.WriteLine("SteamFriends.CloseClanChatWindowInSteam(" + m_Clan + ") - " + SteamFriends.CloseClanChatWindowInSteam(m_Clan));
            }

            //// if (GUILayout.Button("SteamFriends.SetListenForFriendsMessages(true)"))
            {
                Console.WriteLine("SteamFriends.SetListenForFriendsMessages(true) - " + SteamFriends.SetListenForFriendsMessages(true));
            }

            //// if (GUILayout.Button("SteamFriends.ReplyToFriendMessage(SteamUser.GetSteamID(), \"Testing!\")"))
            {
                Console.WriteLine("SteamFriends.ReplyToFriendMessage(" + SteamUser.GetSteamID() + ", \"Testing!\") - " + SteamFriends.ReplyToFriendMessage(SteamUser.GetSteamID(), "Testing!"));
            }

            //Console.WriteLine("SteamFriends.GetFriendMessage() : " + SteamFriends.GetFriendMessage()); // N/A - Must be called from within the callback OnGameConnectedFriendChatMsg

            //// if (GUILayout.Button("SteamFriends.GetFollowerCount(SteamUser.GetSteamID())"))
            {
                SteamAPICall_t handle = SteamFriends.GetFollowerCount(SteamUser.GetSteamID());
                OnFriendsGetFollowerCountCallResult.Set(handle);
                Console.WriteLine("SteamFriends.GetFollowerCount(" + SteamUser.GetSteamID() + ") - " + handle);
            }

            // if (GUILayout.Button("SteamFriends.IsFollowing(m_Friend)"))
            {
                SteamAPICall_t handle = SteamFriends.IsFollowing(m_Friend);
                OnFriendsIsFollowingCallResult.Set(handle);
                Console.WriteLine("SteamFriends.IsFollowing(m_Friend) - " + handle);
            }

            // if (GUILayout.Button("SteamFriends.EnumerateFollowingList(0)"))
            {
                SteamAPICall_t handle = SteamFriends.EnumerateFollowingList(0);
                OnFriendsEnumerateFollowingListCallResult.Set(handle);
                Console.WriteLine("SteamFriends.EnumerateFollowingList(0) - " + handle);
            }
        }


        void OnPersonaStateChange(PersonaStateChange_t pCallback)
        {
            Console.WriteLine("[" + PersonaStateChange_t.k_iCallback + " - PersonaStateChange] - " + pCallback.m_ulSteamID + " -- " + pCallback.m_nChangeFlags);
        }

        void OnGameOverlayActivated(GameOverlayActivated_t pCallback)
        {
            Console.WriteLine("[" + GameOverlayActivated_t.k_iCallback + " - GameOverlayActivated] - " + pCallback.m_bActive);
        }

        void OnGameServerChangeRequested(GameServerChangeRequested_t pCallback)
        {
            Console.WriteLine("[" + GameServerChangeRequested_t.k_iCallback + " - GameServerChangeRequested] - " + pCallback.m_rgchServer + " -- " + pCallback.m_rgchPassword);

            string[] tempSplit = pCallback.m_rgchServer.Split(':');
            YargisSteam.ChangeServer(tempSplit[0], tempSplit[1], pCallback.m_rgchPassword);
        }

        void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t pCallback)
        {
            //SteamworksDotNetTest.Game1.backgroundColor = Microsoft.Xna.Framework.Color.Green;
            Console.WriteLine("[" + GameLobbyJoinRequested_t.k_iCallback + " - GameLobbyJoinRequested] - " + pCallback.m_steamIDLobby + " -- " + pCallback.m_steamIDFriend);
            SteamMatchmakingTest.m_Lobby = pCallback.m_steamIDLobby;

            SteamMatchmaking.JoinLobby(pCallback.m_steamIDLobby);  //Joins the lobby. VERY IMPORTANT :)
            waitingForLobbyJoin = true;
        }

        void OnAvatarImageLoaded(AvatarImageLoaded_t pCallback)
        {
            Console.WriteLine("[" + AvatarImageLoaded_t.k_iCallback + " - AvatarImageLoaded] - " + pCallback.m_steamID + " -- " + pCallback.m_iImage + " -- " + pCallback.m_iWide + " -- " + pCallback.m_iTall);
        }

        void OnClanOfficerListResponse(ClanOfficerListResponse_t pCallback, bool bIOFailure)
        {
            Console.WriteLine("[" + ClanOfficerListResponse_t.k_iCallback + " - ClanOfficerListResponse] - " + pCallback.m_steamIDClan + " -- " + pCallback.m_cOfficers + " -- " + pCallback.m_bSuccess);
        }

        void OnFriendRichPresenceUpdate(FriendRichPresenceUpdate_t pCallback)
        {
            Console.WriteLine("[" + FriendRichPresenceUpdate_t.k_iCallback + " - FriendRichPresenceUpdate] - " + pCallback.m_steamIDFriend + " -- " + pCallback.m_nAppID);
        }

        void OnGameRichPresenceJoinRequested(GameRichPresenceJoinRequested_t pCallback)
        {
            Console.WriteLine("[" + GameRichPresenceJoinRequested_t.k_iCallback + " - GameRichPresenceJoinRequested] - " + pCallback.m_steamIDFriend + " -- " + pCallback.m_rgchConnect);
        }

        void OnGameConnectedClanChatMsg(GameConnectedClanChatMsg_t pCallback)
        {
            Console.WriteLine("[" + GameConnectedClanChatMsg_t.k_iCallback + " - GameConnectedClanChatMsg] - " + pCallback.m_steamIDClanChat + " -- " + pCallback.m_steamIDUser + " -- " + pCallback.m_iMessageID);

            string Text;
            EChatEntryType ChatEntryType;
            CSteamID Chatter;
            int ret = SteamFriends.GetClanChatMessage(pCallback.m_steamIDClanChat, pCallback.m_iMessageID, out Text, 2048, out ChatEntryType, out Chatter); // Must be called from within OnGameConnectedClanChatMsg
            Console.WriteLine(ret + " " + Chatter + ": " + Text);
        }

        void OnGameConnectedChatJoin(GameConnectedChatJoin_t pCallback)
        {
            Console.WriteLine("[" + GameConnectedChatJoin_t.k_iCallback + " - GameConnectedChatJoin] - " + pCallback.m_steamIDClanChat + " -- " + pCallback.m_steamIDUser);
        }

        void OnGameConnectedChatLeave(GameConnectedChatLeave_t pCallback)
        {
            Console.WriteLine("[" + GameConnectedChatLeave_t.k_iCallback + " - GameConnectedChatLeave] - " + pCallback.m_steamIDClanChat + " -- " + pCallback.m_steamIDUser + " -- " + pCallback.m_bKicked + " -- " + pCallback.m_bDropped);
        }

        void OnDownloadClanActivityCountsResult(DownloadClanActivityCountsResult_t pCallback, bool bIOFailure)
        {
            Console.WriteLine("[" + DownloadClanActivityCountsResult_t.k_iCallback + " - DownloadClanActivityCountsResult] - " + pCallback.m_bSuccess);
        }

        void OnJoinClanChatRoomCompletionResult(JoinClanChatRoomCompletionResult_t pCallback, bool bIOFailure)
        {
            Console.WriteLine("[" + JoinClanChatRoomCompletionResult_t.k_iCallback + " - JoinClanChatRoomCompletionResult] - " + pCallback.m_steamIDClanChat + " -- " + pCallback.m_eChatRoomEnterResponse);
        }

        void OnGameConnectedFriendChatMsg(GameConnectedFriendChatMsg_t pCallback)
        {
            Console.WriteLine("[" + GameConnectedFriendChatMsg_t.k_iCallback + " - GameConnectedFriendChatMsg] - " + pCallback.m_steamIDUser + " -- " + pCallback.m_iMessageID);

            string Text;
            EChatEntryType ChatEntryType;
            int ret = SteamFriends.GetFriendMessage(pCallback.m_steamIDUser, pCallback.m_iMessageID, out Text, 2048, out ChatEntryType); // Must be called from within OnGameConnectedFriendChatMsg
            Console.WriteLine(ret + " " + pCallback.m_steamIDUser + ": " + Text);
        }

        void OnFriendsGetFollowerCount(FriendsGetFollowerCount_t pCallback, bool bIOFailure)
        {
            Console.WriteLine("[" + FriendsGetFollowerCount_t.k_iCallback + " - FriendsGetFollowerCount] - " + pCallback.m_eResult + " -- " + pCallback.m_steamID + " -- " + pCallback.m_nCount);
        }

        void OnFriendsIsFollowing(FriendsIsFollowing_t pCallback, bool bIOFailure)
        {
            Console.WriteLine("[" + FriendsIsFollowing_t.k_iCallback + " - FriendsIsFollowing] - " + pCallback.m_eResult + " -- " + pCallback.m_steamID + " -- " + pCallback.m_bIsFollowing);
        }

        void OnFriendsEnumerateFollowingList(FriendsEnumerateFollowingList_t pCallback, bool bIOFailure)
        {
            Console.WriteLine("[" + FriendsEnumerateFollowingList_t.k_iCallback + " - FriendsEnumerateFollowingList] - " + pCallback.m_eResult + " -- " + pCallback.m_rgSteamID + " -- " + pCallback.m_nResultsReturned + " -- " + pCallback.m_nTotalResultCount);
        }

        void OnSetPersonaNameResponse(SetPersonaNameResponse_t pCallback, bool bIOFailure)
        {
            Console.WriteLine("[" + SetPersonaNameResponse_t.k_iCallback + " - SetPersonaNameResponse] - " + pCallback.m_bSuccess + " -- " + pCallback.m_bLocalSuccess + " -- " + pCallback.m_result);
        }
    }
