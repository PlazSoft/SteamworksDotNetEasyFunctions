using Steamworks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SteamworksDotNetTest2017
{
    public partial class Form1 : Form
    {

        public static CSteamID lobbyHandle;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            byte[] tempTicket = SteamManager.SteamAntiCheatClass.getTicket();

            SteamManager.SteamAntiCheatClass.verifyTicket(tempTicket, SteamUser.GetSteamID());
            //IPAddress tempIp = new IPAddress(SteamManager.SteamAntiCheatClass.ToInt("192.168.1.44"));



            //IPAddress tempIp = IPAddress.Parse("192.168.1.44");  //192.168.1.44 Converts to: 3232235820
            //SteamManager.SteamAntiCheatClass.AdvertiseGame(CSteamID.NonSteamGS, tempIp, 4444);


            SteamAPICall_t handle = SteamManager.SteamMatchmakingClass.createLobbyGameServer(ELobbyType.k_ELobbyTypePublic);
            //displayHandle = handle;

            //SteamManager.Update();  RUN THIS IN UPDATE<<<<<<<<<<<<



            //SteamManager.statsAndAchievements.UpdateStatsAndRewards();

            //SteamManager.statsAndAchievements.DisplayStats();
            //SteamManager.statsAndAchievements.AddDistanceTraveled(20f);



            //SteamManager.SteamUGCworkshop = new SteamUGCTest();

            //SteamManager.SteamRemoteStorageTest = new SteamRemoteStorageTest();

            SteamManager.ownsDLC(369040);
        }
    }
}


///// <summary>
///// Allows the game to run logic such as updating the world,
///// checking for collisions, gathering input, and playing audio.
///// </summary>
///// <param name="gameTime">Provides a snapshot of timing values.</param>
//protected override void Update(GameTime gameTime)
//{
//    System.Threading.Thread.Sleep(100);            //Give the CPU a break:

//    KeyboardState newState = Keyboard.GetState();

//    // Allows the game to exit
//    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
//        this.Exit();


//    if (newState.IsKeyDown(Keys.U) && !oldState.IsKeyDown(Keys.U))
//    {
//        SteamManager.statsAndAchievements.sendStats();
//        SteamManager.statsAndAchievements.UpdateStatsAndRewards();
//    }

//    if (newState.IsKeyDown(Keys.A) && !oldState.IsKeyDown(Keys.A))
//    {
//        SteamManager.statsAndAchievements.AddDistanceTraveled(25f);
//        //SteamManager.statsAndAchievements.UpdateStatsAndRewards();
//    }

//    //Upload a file with F.   Not sure about this???: While uploading check the status with P. Then Q. 
//    if (newState.IsKeyDown(Keys.F) && !oldState.IsKeyDown(Keys.F))
//    {
//        SteamAPICall_t handle = SteamManager.SteamUGCworkshop.CreateWorkshopItem();
//        debugString = "Upload file handle: " + handle + "\n" + debugString;

//        // SteamAPICall_t handle = SteamUGC.CreateItem((AppId_t)480, EWorkshopFileType.k_EWorkshopFileTypeWebGuide);
//        //OnCreateItemResultCallResult.Set(handle);
//        //Console.WriteLine("SteamUGC.CreateItem((AppId_t)480, EWorkshopFileType.k_EWorkshopFileTypeWebGuide) : " + handle);
//    }

//    //getProgress
//    if (newState.IsKeyDown(Keys.P) && !oldState.IsKeyDown(Keys.P))
//    {
//        ulong temp;
//        ulong temp2;
//        EItemUpdateStatus progress = SteamManager.SteamUGCworkshop.getProgress(SteamManager.SteamUGCworkshop.m_UGCUpdateHandle, out temp, out temp2);
//        debugString = "Upload progress: " + progress + "\n" + debugString;
//    }

//    //queryWorkshop
//    if (newState.IsKeyDown(Keys.Q) && !oldState.IsKeyDown(Keys.Q))
//    {
//        SteamManager.SteamUGCworkshop.queryWorkshop("Levels");
//    }

//    //GetSubscribedItems
//    if (newState.IsKeyDown(Keys.S) && !oldState.IsKeyDown(Keys.S))
//    {
//        SteamManager.SteamUGCworkshop.GetSubscribedItems();
//    }


//    if (newState.IsKeyDown(Keys.LeftControl) && !oldState.IsKeyDown(Keys.LeftControl))
//    {
//        SteamManager.SteamAntiCheatClass.startRecordingVoice();
//    }


//    if (!newState.IsKeyDown(Keys.LeftControl) && oldState.IsKeyDown(Keys.LeftControl))
//    {
//        SteamManager.SteamAntiCheatClass.stopRecordingVoice();
//    }

//    if (newState.IsKeyDown(Keys.L) && !oldState.IsKeyDown(Keys.L))
//    {
//        SteamMatchmakingTest.gameServer tempServer2 = SteamManager.SteamMatchmakingClass.getLobbyInfo(lobbyHandle);  //(CSteamID)pCallback.m_steamIDLobby.m_SteamID   //pCallback.m_steamIDLobby

//    }



//    SteamManager.Update();

//    if (SteamManager.Initialized)
//        SteamManager.SteamAntiCheatClass.playVoiceIfAvailable();


//    oldState = newState;

//    base.Update(gameTime);
//}