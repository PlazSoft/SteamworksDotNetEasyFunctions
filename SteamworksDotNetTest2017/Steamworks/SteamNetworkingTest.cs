////using UnityEngine;
//using System.Collections;
//using Steamworks;
//using System;

//public class SteamNetworkingTest  {
//    // You'd typically get this from a Lobby. Hardcoding it so that we don't need to integrate the whole lobby system with the networking.
//    CSteamID m_RemoteSteamId = new CSteamID(0);

//    protected Callback<P2PSessionRequest_t> m_P2PSessionRequest;
//    protected Callback<P2PSessionConnectFail_t> m_P2PSessionConnectFail;
//    protected Callback<SocketStatusCallback_t> m_SocketStatusCallback_t;
	
//    void OnEnable() {
//        m_P2PSessionRequest = Callback<P2PSessionRequest_t>.Create(OnP2PSessionRequest);
//        m_P2PSessionConnectFail = Callback<P2PSessionConnectFail_t>.Create(OnP2PSessionConnectFail);
//        m_SocketStatusCallback_t = Callback<SocketStatusCallback_t>.Create(OnSocketStatusCallback);
//    }

//    void OnDisable() {
//        // Just incase we have it open when we close/assemblies get reloaded.
//        if (!m_RemoteSteamId.IsValid()) {
//            SteamNetworking.CloseP2PSessionWithUser(m_RemoteSteamId);
//        }
//    }

//    public void RenderOnGUI() {
//        GUILayout.BeginArea(new Rect(Screen.width - 200, 0, 200, Screen.height));
//        Console.WriteLine("Variables:");
//        Console.WriteLine("m_RemoteSteamId: " + m_RemoteSteamId);
//        GUILayout.EndArea();

//        if (!m_RemoteSteamId.IsValid()) {
//            Console.WriteLine("Please fill m_RemoteSteamId with a valid 64bit SteamId to use SteamNetworkingTest.");
//            Console.WriteLine("Alternatively it will be filled automatically when a session request is recieved.");
//            return;
//        }

//        // Session-less connection functions

//        if (GUILayout.Button("SendP2PPacket(m_RemoteSteamId, bytes, (uint)bytes.Length, EP2PSend.k_EP2PSendReliable)")) {
//            byte[] bytes = new byte[4];
//            using (System.IO.MemoryStream ms = new System.IO.MemoryStream(bytes))
//            using (System.IO.BinaryWriter b = new System.IO.BinaryWriter(ms)) {
//                b.Write((uint)MsgType.Ping);
//            }
//            bool ret = SteamNetworking.SendP2PPacket(m_RemoteSteamId, bytes, (uint)bytes.Length, EP2PSend.k_EP2PSendReliable);
//            print("SteamNetworking.SendP2PPacket(" + m_RemoteSteamId + ", bytes, " + bytes.Length + ", EP2PSend.k_EP2PSendReliable) - " + ret);
//        }

//        {
//            uint MsgSize;
//            bool ret = SteamNetworking.IsP2PPacketAvailable(out MsgSize);
//            Console.WriteLine("IsP2PPacketAvailable(out MsgSize) : " + ret + " -- " + MsgSize);

//            GUI.enabled = ret;

//            if (GUILayout.Button("ReadP2PPacket(bytes, MsgSize, out newMsgSize, out SteamIdRemote)")) {
//                byte[] bytes = new byte[MsgSize];
//                uint newMsgSize;
//                CSteamID SteamIdRemote;
//                ret = SteamNetworking.ReadP2PPacket(bytes, MsgSize, out newMsgSize, out SteamIdRemote);

//                using (System.IO.MemoryStream ms = new System.IO.MemoryStream(bytes))
//                using (System.IO.BinaryReader b = new System.IO.BinaryReader(ms)) {
//                    MsgType msgtype = (MsgType)b.ReadUInt32();
//                    // switch statement here depending on the msgtype
//                    print("SteamNetworking.ReadP2PPacket(bytes, " + MsgSize + ", out newMsgSize, out SteamIdRemote) - " + ret + " -- " + newMsgSize + " -- " + SteamIdRemote + " -- " + msgtype);
//                }

//            }

//            GUI.enabled = true;
//        }

//        // bool AcceptP2PSessionWithUser(CSteamID steamIDRemote) // Only called from within P2PSessionRequest Callback!

//        if (GUILayout.Button("CloseP2PSessionWithUser(m_RemoteSteamId)")) {
//            bool ret = SteamNetworking.CloseP2PSessionWithUser(m_RemoteSteamId);
//            print("SteamNetworking.CloseP2PSessionWithUser(" + m_RemoteSteamId + ") - " + ret);
//        }

//        if (GUILayout.Button("CloseP2PChannelWithUser(m_RemoteSteamId, 0)")) {
//            bool ret = SteamNetworking.CloseP2PChannelWithUser(m_RemoteSteamId, 0);
//            print("SteamNetworking.CloseP2PChannelWithUser(" + m_RemoteSteamId + ", 0) - " + ret);
//        }

//        {
//            P2PSessionState_t ConnectionState;
//            bool ret = SteamNetworking.GetP2PSessionState(m_RemoteSteamId, out ConnectionState);
//            Console.WriteLine("GetP2PSessionState(m_RemoteSteamId, out ConnectionState) : " + ret + " -- " + ConnectionState.m_bConnectionActive + " -- " + ConnectionState.m_bConnecting + " -- " + ConnectionState.m_eP2PSessionError + " -- " + ConnectionState.m_bUsingRelay + " -- " + ConnectionState.m_nBytesQueuedForSend + " -- " + ConnectionState.m_nPacketsQueuedForSend + " -- " + ConnectionState.m_nRemoteIP + " -- " + ConnectionState.m_nRemotePort);
//        }

//        if (GUILayout.Button("AllowP2PPacketRelay(true)")) {
//            bool ret = SteamNetworking.AllowP2PPacketRelay(true);
//            print("SteamNetworking.AllowP2PPacketRelay(true) - " + ret);
//        }

//        // LISTEN / CONNECT style interface functions


//    }

//    void OnP2PSessionRequest(P2PSessionRequest_t pCallback) {
//        Console.WriteLine("[" + P2PSessionRequest_t.k_iCallback + " - P2PSessionRequest] - " + pCallback.m_steamIDRemote);

//        bool ret = SteamNetworking.AcceptP2PSessionWithUser(pCallback.m_steamIDRemote);
//        Console.WriteLine("SteamNetworking.AcceptP2PSessionWithUser(" + pCallback.m_steamIDRemote + ") - " + ret);

//        m_RemoteSteamId = pCallback.m_steamIDRemote;
//    }

//    void OnP2PSessionConnectFail(P2PSessionConnectFail_t pCallback) {
//        Console.WriteLine("[" + P2PSessionConnectFail_t.k_iCallback + " - P2PSessionConnectFail] - " + pCallback.m_steamIDRemote + " -- " + pCallback.m_eP2PSessionError);
//    }

//    void OnSocketStatusCallback(SocketStatusCallback_t pCallback) {
//        Console.WriteLine("[" + SocketStatusCallback_t.k_iCallback + " - SocketStatusCallback] - " + pCallback.m_hSocket + " -- " + pCallback.m_hListenSocket + " -- " + pCallback.m_steamIDRemote + " -- " + pCallback.m_eSNetSocketState);
//    }

//    enum MsgType : uint {
//        Ping,
//        Ack,
//    }
//}

