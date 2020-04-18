using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Steamworks;
using System.Net;
using System.IO;
//using Microsoft.Xna.Framework.Audio;

public class SteamAntiCheat
{

    //Docs https://partner.steamgames.com/documentation/vac

    //Testint tools: https://partner.steamgames.com/documentation/service_tester


    private byte[] m_Ticket;
    private uint m_pcbTicket;
    private HAuthTicket m_HAuthTicket;
    //private GameObject m_VoiceLoopback;

    protected Callback<SteamServersConnected_t> m_SteamServersConnected;
    protected Callback<SteamServerConnectFailure_t> m_SteamServerConnectFailure;
    protected Callback<SteamServersDisconnected_t> m_SteamServersDisconnected;
    protected Callback<ClientGameServerDeny_t> m_ClientGameServerDeny;
    protected Callback<IPCFailure_t> m_IPCFailure;
    protected Callback<LicensesUpdated_t> m_LicensesUpdated;
    protected Callback<ValidateAuthTicketResponse_t> m_ValidateAuthTicketResponse;
    protected Callback<MicroTxnAuthorizationResponse_t> m_MicroTxnAuthorizationResponse;
    protected Callback<GetAuthSessionTicketResponse_t> m_GetAuthSessionTicketResponse;
    protected Callback<GameWebCallback_t> m_GameWebCallback;

    private CallResult<EncryptedAppTicketResponse_t> OnEncryptedAppTicketResponseCallResult;
    private CallResult<StoreAuthURLResponse_t> OnStoreAuthURLResponseCallResult;

    public SteamAntiCheat()
    {
        m_SteamServersConnected = Callback<SteamServersConnected_t>.Create(OnSteamServersConnected);
        m_SteamServerConnectFailure = Callback<SteamServerConnectFailure_t>.Create(OnSteamServerConnectFailure);
        m_SteamServersDisconnected = Callback<SteamServersDisconnected_t>.Create(OnSteamServersDisconnected);
        m_ClientGameServerDeny = Callback<ClientGameServerDeny_t>.Create(OnClientGameServerDeny);
        m_IPCFailure = Callback<IPCFailure_t>.Create(OnIPCFailure);
        m_LicensesUpdated = Callback<LicensesUpdated_t>.Create(OnLicensesUpdated);
        m_ValidateAuthTicketResponse = Callback<ValidateAuthTicketResponse_t>.Create(OnValidateAuthTicketResponse);
        m_MicroTxnAuthorizationResponse = Callback<MicroTxnAuthorizationResponse_t>.Create(OnMicroTxnAuthorizationResponse);
        m_GetAuthSessionTicketResponse = Callback<GetAuthSessionTicketResponse_t>.Create(OnGetAuthSessionTicketResponse);
        m_GameWebCallback = Callback<GameWebCallback_t>.Create(OnGameWebCallback);

        OnEncryptedAppTicketResponseCallResult = CallResult<EncryptedAppTicketResponse_t>.Create(OnEncryptedAppTicketResponse);
        OnStoreAuthURLResponseCallResult = CallResult<StoreAuthURLResponse_t>.Create(OnStoreAuthURLResponse);
    }

    //https://partner.steamgames.com/documentation/auth
    /// <summary>
    /// Returns a one time use ticket. The ticket is normally transmitted to the server next, along with the client's Steam ID, and Steam Name. The client must revoke the ticket when done (CancelAuthTicket).
    /// </summary>
    /// <returns>A one time user ticket.</returns>
    public byte[] getTicket()
    {
        byte[] pTicket = new byte[1024];
        int cbMaxTicket = pTicket.Length;
        uint pcbTicketSize;  //looks like the number of bytes used?
        HAuthTicket tempTicket = SteamUser.GetAuthSessionTicket(pTicket, cbMaxTicket, out pcbTicketSize);
        return pTicket;
    }

    /// <summary>
    /// Gets the Steam ID.
    /// </summary>
    public CSteamID getSteamID()
    {
        return SteamUser.GetSteamID();
    }

    /// <summary>
    /// Verify's a Steam ticket matches the userId and is valid. Once the OnValidateAuthTicketResponse is received, the ticket can be deleted and a verified flag set? The Steam ID must be saved and CancelAuthTicket called when the player leaves.
    /// </summary>
    /// <param name="theTicket"></param>
    /// <param name="steamUserID"></param>
    /// <returns></returns>
    public EBeginAuthSessionResult verifyTicket(byte[] theTicket, CSteamID steamUserID)
    {
        return SteamUser.BeginAuthSession(theTicket, theTicket.Length, steamUserID);
    }

    /// <summary>
    /// When the multiplayer session terminates: The client communicates that he left the session. Client A must pass the handle initially returned from GetAuthSessionTicket() to SteamUser()->CancelAuthTicket().
    /// </summary>
    public void CancelAuthTicket(HAuthTicket HAuthTicket)
    {
        SteamUser.CancelAuthTicket(HAuthTicket);
    }


    /// <summary>
    /// When the multiplayer session terminates: The server communicates that the session is over. Client B (the server) must pass the SteamID of client A  to SteamUser()->EndAuthSession().
    /// </summary>
    public void EndAuthSession(CSteamID CSteamID)
    {
        SteamUser.EndAuthSession(CSteamID);
    }

    /// <summary>
    /// Advertise that you are joining a server. Hint: IPAddress.Parse("192.168.1.44")
    /// </summary>
    /// <param name="serverType">The servers IP</param>
    /// <param name="ipAddress">IPAddress tempIp = IPAddress.Parse("192.168.1.44"); </param>
    /// <param name="port">The servers port</param>
    public void AdvertiseGame(CSteamID serverType, System.Net.IPAddress ipAddress, ushort port)
    {
        //int intAddress = BitConverter.ToInt32(IPAddress.Parse(ipAddress.ToString).GetAddressBytes(), 0);
        //string tempIp = new IPAddress(BitConverter.GetBytes(intAddress)).ToString();
        uint tempIp = ToUInt(ipAddress.ToString());
        SteamUser.AdvertiseGame(serverType, tempIp, port);
    }

    public static uint ToUInt(string addr)
    {
        // careful of sign extension: convert to uint first;
        // unsigned NetworkToHostOrder ought to be provided.
        return (uint)IPAddress.NetworkToHostOrder((int)IPAddress.Parse(addr).Address);
    }

    public static string ToAddr(long address)
    {
        return IPAddress.Parse(address.ToString()).ToString();
        // This also works:
        // return new IPAddress((uint) IPAddress.HostToNetworkOrder(
        //    (int) address)).ToString();
    }


    public void startRecordingVoice()
    {
        SteamUser.StartVoiceRecording();
    }

    public void stopRecordingVoice()
    {
        SteamUser.StopVoiceRecording();
    }


    byte[] DestBuffer = new byte[1024];
    byte[] UncompressedDestBuffer = new byte[1024];
    byte[] DestBuffer2 = new byte[11025 * 2];
    float[] test = new float[11025];
    EVoiceResult ret;

    //public void playVoiceIfAvailable()
    //{
    //    uint Compressed;
    //    uint Uncompressed;
    //    ret = SteamUser.GetAvailableVoice(out Compressed, out Uncompressed, 11025);


    //    if (ret == EVoiceResult.k_EVoiceResultOK && Compressed > 0)
    //    {
    //        //Console.WriteLine("GetAvailableVoice(out Compressed, out Uncompressed, 11025) : " + ret + " -- " + Compressed + " -- " + Uncompressed);
    //        uint BytesWritten;
    //        uint UncompressedBytesWritten;
    //        ret = SteamUser.GetVoice(true, DestBuffer, 1024, out BytesWritten, true, UncompressedDestBuffer, (uint)DestBuffer.Length, out UncompressedBytesWritten, 11025);
    //        //Console.WriteLine("SteamUser.GetVoice(true, DestBuffer, 1024, out BytesWritten, true, UncompressedDestBuffer, (uint)DestBuffer.Length, out UncompressedBytesWritten, 11025) : " + ret + " -- " + BytesWritten + " -- " + UncompressedBytesWritten);

    //        if (ret == EVoiceResult.k_EVoiceResultOK && BytesWritten > 0)
    //        {

    //            uint BytesWritten2;
    //            ret = SteamUser.DecompressVoice(DestBuffer, BytesWritten, DestBuffer2, (uint)DestBuffer2.Length, out BytesWritten2, 11025);
    //            //Console.WriteLine("SteamUser.DecompressVoice(DestBuffer, BytesWritten, DestBuffer2, (uint)DestBuffer2.Length, out BytesWritten2, 11025) - " + ret + " -- " + BytesWritten2);

    //            if (ret == EVoiceResult.k_EVoiceResultOK && BytesWritten2 > 0)
    //            {

    //                //converts to doubles?
    //                //for (int i = 0; i < test.Length; ++i)
    //                //{
    //                //    test[i] = (short)(DestBuffer2[i * 2] | DestBuffer2[i * 2 + 1] << 8) / 32768.0f;
    //                //}
    //                //source.clip.SetData(test, 0);

    //                MemoryStream stream = new MemoryStream(DestBuffer2);

    //                SoundEffect sound = new SoundEffect(stream.ToArray(), 11025, AudioChannels.Mono);
    //                sound.Play();

    //                stream.Dispose();
    //                //sound.Dispose();

    //                ////AudioSource source;
    //                //if (!m_VoiceLoopback)
    //                //{
    //                //    m_VoiceLoopback = new GameObject("Voice Loopback");
    //                //    source = m_VoiceLoopback.AddComponent<AudioSource>();
    //                //    source.clip = AudioClip.Create("Testing!", 11025, 1, 11025, false, false);
    //                //}
    //                //else
    //                //{
    //                //    source = m_VoiceLoopback.GetComponent<AudioSource>();
    //                //}


    //                //source.Play();
    //            }
    //        }
    //    }
    //}




    //Client A must retrieve a session ticket by calling SteamUser()->GetAuthSessionTicket().
    //Client A must send its session ticket to client B.
    //Client B must pass client A's ticket to SteamUser()->BeginAuthSession(), which will perform a quick validity check. If the ticket is valid, BeginAuthSession() will forward the ticket to then the Steam backend to verify that the ticket has not been reused and was issued by the account owner of client A. The result of this verification will be returned in a ValidateAuthTicketResponse_t callback.
    //When the multiplayer session terminates:
    //    Client A must pass the handle initially returned from GetAuthSessionTicket() to SteamUser()->CancelAuthTicket().
    //    Client B must pass the SteamID of client A to SteamUser()->EndAuthSession().


    public void RenderOnGUI()
    {
        //GUILayout.BeginArea(new Rect(Screen.width - 200, 0, 200, Screen.height));
        //GUILayout.Label("m_HAuthTicket: " + m_HAuthTicket);
        //GUILayout.Label("m_pcbTicket: " + m_pcbTicket);
        //GUILayout.EndArea();

        //GUILayout.Label("GetHSteamUser() : " + SteamUser.GetHSteamUser());
        //GUILayout.Label("BLoggedOn() : " + SteamUser.BLoggedOn());
        //GUILayout.Label("GetSteamID() : " + SteamUser.GetSteamID());

        //GUILayout.Label("InitiateGameConnection() : " + SteamUser.InitiateGameConnection()); // N/A - Too Hard to test like this.
        //GUILayout.Label("TerminateGameConnection() : " + SteamUser.TerminateGameConnection()); // ^
        //GUILayout.Label("TrackAppUsageEvent() : " + SteamUser.TrackAppUsageEvent()); // Legacy function with no documentation

        {
            string Buffer;
            bool ret = SteamUser.GetUserDataFolder(out Buffer, 260);
            Console.WriteLine("GetUserDataFolder(out Buffer, 260) : " + ret + " -- " + Buffer);
        }

        //if (GUILayout.Button("StartVoiceRecording()"))
        {
            SteamUser.StartVoiceRecording();
            Console.WriteLine("SteamUser.StartVoiceRecording()");
        }

        //if (GUILayout.Button("StopVoiceRecording()"))
        {
            SteamUser.StopVoiceRecording();
            Console.WriteLine("SteamUser.StopVoiceRecording()");
        }

        {


            //GUILayout.Label("GetVoiceOptimalSampleRate() : " + SteamUser.GetVoiceOptimalSampleRate());

            {
                //if (GUILayout.Button("GetAuthSessionTicket(Ticket, 1024, out pcbTicket)"))
                {
                    m_Ticket = new byte[1024];
                    m_HAuthTicket = SteamUser.GetAuthSessionTicket(m_Ticket, 1024, out m_pcbTicket);
                    Console.WriteLine("SteamUser.GetAuthSessionTicket(Ticket, 1024, out pcbTicket) - " + m_HAuthTicket + " -- " + m_pcbTicket);
                }

                //if (GUILayout.Button("BeginAuthSession(m_Ticket, (int)m_pcbTicket, SteamUser.GetSteamID())"))
                {
                    if (m_HAuthTicket != HAuthTicket.Invalid && m_pcbTicket != 0)
                    {
                        EBeginAuthSessionResult ret = SteamUser.BeginAuthSession(m_Ticket, (int)m_pcbTicket, SteamUser.GetSteamID());
                        Console.WriteLine("SteamUser.BeginAuthSession(m_Ticket, " + (int)m_pcbTicket + ", " + SteamUser.GetSteamID() + ") - " + ret);
                    }
                    else
                    {
                        Console.WriteLine("Call GetAuthSessionTicket first!");
                    }
                }
            }

            //if (GUILayout.Button("EndAuthSession(SteamUser.GetSteamID())"))
            {
                SteamUser.EndAuthSession(SteamUser.GetSteamID());
                Console.WriteLine("SteamUser.EndAuthSession(" + SteamUser.GetSteamID() + ")");
            }

            //if (GUILayout.Button("CancelAuthTicket(m_HAuthTicket)"))
            {
                SteamUser.CancelAuthTicket(m_HAuthTicket);
                Console.WriteLine("SteamUser.CancelAuthTicket(" + m_HAuthTicket + ")");
            }

            Console.WriteLine("UserHasLicenseForApp(SteamUser.GetSteamID(), SteamUtils.GetAppID()) : " + SteamUser.UserHasLicenseForApp(SteamUser.GetSteamID(), SteamUtils.GetAppID()));
            Console.WriteLine("BIsBehindNAT() : " + SteamUser.BIsBehindNAT());

            //if (GUILayout.Button("AdvertiseGame(2, 127.0.0.1, 27015)"))
            {
                SteamUser.AdvertiseGame(CSteamID.NonSteamGS, 2130706433, 27015);
                Console.WriteLine("SteamUser.AdvertiseGame(2, 2130706433, 27015)");
            }

            //if (GUILayout.Button("RequestEncryptedAppTicket()"))
            {
                byte[] k_unSecretData = System.BitConverter.GetBytes(0x5444);
                SteamAPICall_t handle = SteamUser.RequestEncryptedAppTicket(k_unSecretData, sizeof(uint));
                OnEncryptedAppTicketResponseCallResult.Set(handle);
                Console.WriteLine("SteamUser.RequestEncryptedAppTicket(k_unSecretData, " + sizeof(uint) + ") - " + handle);
            }

            //if (GUILayout.Button("GetEncryptedAppTicket()"))
            {
                byte[] rgubTicket = new byte[1024];
                uint cubTicket;
                bool ret = SteamUser.GetEncryptedAppTicket(rgubTicket, 1024, out cubTicket);
                Console.WriteLine("SteamUser.GetEncryptedAppTicket() - " + ret + " -- " + cubTicket);
            }

            //GUILayout.Label("GetGameBadgeLevel(1, false) : " + SteamUser.GetGameBadgeLevel(1, false)); // SpaceWar does not have trading cards, so this function will only ever return 0 and produce an annoying warning.
            Console.WriteLine("GetPlayerSteamLevel() : " + SteamUser.GetPlayerSteamLevel());

            //if (GUILayout.Button("RequestStoreAuthURL(\"https://steampowered.com\")"))
            {
                SteamAPICall_t handle = SteamUser.RequestStoreAuthURL("https://steampowered.com");
                OnStoreAuthURLResponseCallResult.Set(handle);
                Console.WriteLine("SteamUser.RequestStoreAuthURL(\"https://steampowered.com\") - " + handle);
            }

#if _PS3
		//GUILayout.Label("LogOn() : " + SteamUser.LogOn());
		//GUILayout.Label("LogOnAndLinkSteamAccountToPSN : " + SteamUser.LogOnAndLinkSteamAccountToPSN());
		//GUILayout.Label("LogOnAndCreateNewSteamAccountIfNeeded : " + SteamUser.LogOnAndCreateNewSteamAccountIfNeeded());
		//GUILayout.Label("GetConsoleSteamID : " + SteamUser.GetConsoleSteamID());
#endif
        }
    }

    void OnSteamServersConnected(SteamServersConnected_t pCallback)
    {
        Console.WriteLine("[" + SteamServersConnected_t.k_iCallback + " - SteamServersConnected]");
    }

    void OnSteamServerConnectFailure(SteamServerConnectFailure_t pCallback)
    {
        Console.WriteLine("[" + SteamServerConnectFailure_t.k_iCallback + " - SteamServerConnectFailure] - " + pCallback.m_eResult);
    }

    void OnSteamServersDisconnected(SteamServersDisconnected_t pCallback)
    {
        Console.WriteLine("[" + SteamServersDisconnected_t.k_iCallback + " - SteamServersDisconnected] - " + pCallback.m_eResult);
    }

    void OnClientGameServerDeny(ClientGameServerDeny_t pCallback)
    {
        Console.WriteLine("[" + ClientGameServerDeny_t.k_iCallback + " - ClientGameServerDeny] - " + pCallback.m_uAppID + " -- " + pCallback.m_unGameServerIP + " -- " + pCallback.m_usGameServerPort + " -- " + pCallback.m_bSecure + " -- " + pCallback.m_uReason);
    }

    void OnIPCFailure(IPCFailure_t pCallback)
    {
        Console.WriteLine("[" + IPCFailure_t.k_iCallback + " - IPCFailure] - " + pCallback.m_eFailureType);
    }

    void OnLicensesUpdated(LicensesUpdated_t pCallback)
    {
        Console.WriteLine("[" + LicensesUpdated_t.k_iCallback + " - LicensesUpdated_t]");
    }

    void OnValidateAuthTicketResponse(ValidateAuthTicketResponse_t pCallback)
    {
        Console.WriteLine("[" + ValidateAuthTicketResponse_t.k_iCallback + " - ValidateAuthTicketResponse] - " + pCallback.m_SteamID + " -- " + pCallback.m_eAuthSessionResponse + " -- " + pCallback.m_OwnerSteamID);
        //TODO: Update data in Yargis here<<<<<<<<<
    }

    void OnMicroTxnAuthorizationResponse(MicroTxnAuthorizationResponse_t pCallback)
    {
        Console.WriteLine("[" + MicroTxnAuthorizationResponse_t.k_iCallback + " - MicroTxnAuthorizationResponse] - " + pCallback.m_unAppID + " -- " + pCallback.m_ulOrderID + " -- " + pCallback.m_bAuthorized);
    }

    void OnEncryptedAppTicketResponse(EncryptedAppTicketResponse_t pCallback, bool bIOFailure)
    {
        Console.WriteLine("[" + EncryptedAppTicketResponse_t.k_iCallback + " - EncryptedAppTicketResponse] - " + pCallback.m_eResult);

        // This code is taken directly from SteamworksExample/SpaceWar
        if (pCallback.m_eResult == EResult.k_EResultOK)
        {
            byte[] rgubTicket = new byte[1024];
            uint cubTicket;
            SteamUser.GetEncryptedAppTicket(rgubTicket, 1024, out cubTicket);

            // normally at this point you transmit the encrypted ticket to the service that knows the decryption key
            // this code is just to demonstrate the ticket cracking library

            // included is the "secret" key for spacewar. normally this is secret
            byte[] rgubKey = new byte[32] { 0xed, 0x93, 0x86, 0x07, 0x36, 0x47, 0xce, 0xa5, 0x8b, 0x77, 0x21, 0x49, 0x0d, 0x59, 0xed, 0x44, 0x57, 0x23, 0xf0, 0xf6, 0x6e, 0x74, 0x14, 0xe1, 0x53, 0x3b, 0xa3, 0x3c, 0xd8, 0x03, 0xbd, 0xbd };

            byte[] rgubDecrypted = new byte[1024];
            uint cubDecrypted = 1024;
            if (!SteamEncryptedAppTicket.BDecryptTicket(rgubTicket, cubTicket, rgubDecrypted, ref cubDecrypted, rgubKey, rgubKey.Length))
            {
                Console.WriteLine("Ticket failed to decrypt");
                return;
            }

            if (!SteamEncryptedAppTicket.BIsTicketForApp(rgubDecrypted, cubDecrypted, SteamUtils.GetAppID()))
            {
                Console.WriteLine("Ticket for wrong app id");
            }

            CSteamID steamIDFromTicket;
            SteamEncryptedAppTicket.GetTicketSteamID(rgubDecrypted, cubDecrypted, out steamIDFromTicket);
            if (steamIDFromTicket != SteamUser.GetSteamID())
            {
                Console.WriteLine("Ticket for wrong user");
            }

            uint cubData;
            byte[] punSecretData = SteamEncryptedAppTicket.GetUserVariableData(rgubDecrypted, cubDecrypted, out cubData);
            if (cubData != sizeof(uint))
            {
                Console.WriteLine("Secret data size is wrong.");
            }
            Console.WriteLine(punSecretData.Length);
            Console.WriteLine(System.BitConverter.ToUInt32(punSecretData, 0));
            if (System.BitConverter.ToUInt32(punSecretData, 0) != 0x5444)
            {
                Console.WriteLine("Failed to retrieve secret data");
                return;
            }

            Console.WriteLine("Successfully retrieved Encrypted App Ticket");
        }
    }

    void OnGetAuthSessionTicketResponse(GetAuthSessionTicketResponse_t pCallback)
    {
        Console.WriteLine("[" + GetAuthSessionTicketResponse_t.k_iCallback + " - GetAuthSessionTicketResponse] - " + pCallback.m_hAuthTicket + " -- " + pCallback.m_eResult);
    }

    void OnGameWebCallback(GameWebCallback_t pCallback)
    {
        Console.WriteLine("[" + GameWebCallback_t.k_iCallback + " - GameWebCallback] - " + pCallback.m_szURL);
    }

    void OnStoreAuthURLResponse(StoreAuthURLResponse_t pCallback, bool bIOFailure)
    {
        Console.WriteLine("[" + StoreAuthURLResponse_t.k_iCallback + " - StoreAuthURLResponse] - " + pCallback.m_szURL);
    }



}
