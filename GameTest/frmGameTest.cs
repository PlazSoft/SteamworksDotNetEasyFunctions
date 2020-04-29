using Steamworks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace GameTest
{
    public partial class frmGameTest : Form
    {
        public const string newLine = "\r\n";
        SteamAPICall_t workshopUploadHandle;

        public itemToUpload uploadItem = new itemToUpload();

        public frmGameTest()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

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

  
        }

        private void btnUploadWorkshop_Click(object sender, EventArgs e)
        {
            //Creating a new publisher ID every time will create a new file. To update you must populate the publisherID
            //PublishedFileId_t PublisherID = new PublishedFileId_t(); 

            List<string> tags = new List<string>();
            tags.Add("Levels");
            uploadItem.tags = tags;
            uploadItem.PublishedFileID = uploadItem.PublishedFileID;

            uploadItem.title = "Title Test";
            uploadItem.description = "Description Test";


            if (SteamManager.Initialized)
            {


                if (uploadItem.PublishedFileID == (Steamworks.PublishedFileId_t)0)
                {
                    workshopUploadHandle = SteamManager.SteamUGCworkshop.CreateWorkshopItem();
                    CheckPublishIDTimer.Enabled = true;
                   // PublisherID = workshopUploadHandle;
                }
                else
                {
                    //file.SaveLevelAs(contentPath + Path.GetFileName(file.currentLevelFileName));
                    itemToUpload.uploadToWorkshop(uploadItem.PublishedFileID, uploadItem.tags, uploadItem.contentPath, uploadItem.title, uploadItem.description, uploadItem.imagePreviewFile);

    //                UGCUpdateHandle_t updateHandle = SteamManager.SteamUGCworkshop.registerFileInfoOrUpdate(PublisherID, "Title Test", "Description Test",
    //ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPrivate, tags, contentPath, contentPath + "LevelPreview.jpg");
                }
            }
            ////Upload a file with F.   Not sure about this???: While uploading check the status with P. Then Q. 

            //    SteamAPICall_t handle = SteamManager.SteamUGCworkshop.CreateWorkshopItem();

            //// SteamAPICall_t handle = SteamUGC.CreateItem((AppId_t)480, EWorkshopFileType.k_EWorkshopFileTypeWebGuide);
            ////OnCreateItemResultCallResult.Set(handle);
            ////Console.WriteLine("SteamUGC.CreateItem((AppId_t)480, EWorkshopFileType.k_EWorkshopFileTypeWebGuide) : " + handle);

            //Program.debugString += "Upload file handle: " + handle + "\n" + newLine; //+ bSuccess
            txtDebug.Text = Program.debugString;

        }
        
        public static string GetFileNameFromPath(string Path)
        {
            try
            {
                return Path.Substring(Path.LastIndexOf(@"\") + 1);
            }
            catch (Exception ex)
            {
                return Path;
            }
        }

        static void CopyFiles(string source, string destination, string searchPattern,  bool overwrite)
        {
            foreach (string foundFile in System.IO.Directory.GetFiles(source, searchPattern))
                File.Copy(System.IO.Path.Combine(source, GetFileNameFromPath(foundFile)), System.IO.Path.Combine(destination, GetFileNameFromPath(foundFile)));
        }


        public void updateContentPaths()
        {
            string contentPath = Path.GetTempPath() + @"SteamUploads\" + uploadItem.PublishedFileID + @"\";
            if (!Directory.Exists(contentPath))
            {
                Directory.CreateDirectory(contentPath);
            }

            System.IO.Directory.CreateDirectory(Path.GetTempPath() + @"SteamUploads\");
           // DirectoryInfo src = new DirectoryInfo(@"TestUpload\*.*");
           // DirectoryInfo dst = new DirectoryInfo(contentPath);
            CopyFiles(@"TestUpload\", contentPath, "*.*", true);

            uploadItem.contentPath = contentPath; //TODO: We may need to update this in production <<<<<<<<<<<<<<<<<<<<<<<<<<
            uploadItem.imagePreviewFile = contentPath + "Christmas Party.jpg";
        }

        private void CheckPublishIDTimer_Tick(object sender, EventArgs e)
        {
            if (SteamManager.SteamUGCworkshop != null && SteamManager.SteamUGCworkshop.m_PublishedFileId != null && SteamManager.SteamUGCworkshop.m_PublishedFileId.m_PublishedFileId != null &&
                (int)SteamManager.SteamUGCworkshop.m_PublishedFileId.m_PublishedFileId != 0)
            {
                uploadItem.PublishedFileID = SteamManager.SteamUGCworkshop.m_PublishedFileId;
                updateContentPaths();
                itemToUpload.uploadToWorkshop(uploadItem.PublishedFileID, uploadItem.tags, uploadItem.contentPath, uploadItem.title, uploadItem.description, uploadItem.imagePreviewFile);
                CheckPublishIDTimer.Enabled = false;
                CheckUploadTimer.Enabled = true;
            }
        }

        private void CheckUploadTimer_Tick(object sender, EventArgs e)
        {
//#if STEAMCLIENT
            ulong BytesProcessed;
            ulong BytesTotal;

            if (SteamManager.SteamUGCworkshop.getProgress(SteamManager.SteamUGCworkshop.m_UGCUpdateHandle, out BytesProcessed, out BytesTotal) == EItemUpdateStatus.k_EItemUpdateStatusInvalid)
            {
                CheckUploadTimer.Enabled = false;
                UploadProgress.Visible = false;
                Console.WriteLine("steam://url/CommunityFilePage/" + uploadItem.PublishedFileID);
                System.Diagnostics.Process.Start("steam://url/CommunityFilePage/" + uploadItem.PublishedFileID);
                
            }
            else
            {
                UploadProgress.Visible = true;
                UploadProgress.Value = 0;
                UploadProgress.Maximum = (int)BytesTotal;
                UploadProgress.Value = (int)BytesProcessed;
            }
//#endif
        }

        //private void btnUploadWorkshopFile_Click(object sender, EventArgs e)
        //{
        //    //if (SteamManager.Initialized)
        //    //{
        //    //    string contentPath = Path.GetTempPath() + @"Yargis\" + uploadItem.PublisherID + @"\";
        //    //    if (!Directory.Exists(contentPath))
        //    //    {
        //    //        Directory.CreateDirectory(contentPath);
        //    //    }
        //    //    List<string> tags = new List<string>();
        //    //    tags.Add("Levels");
        //    //    uploadItem.tags = tags;
        //    //    uploadItem.PublisherID = uploadItem.PublisherID;
        //    //    uploadItem.contentPath = @"\TestUpload\"; //contentPath;
        //    //    uploadItem.title = "Title Test";
        //    //    uploadItem.description = "Description Test";
        //    //    uploadItem.imagePreviewFile =  @"\TestUpload\Christmas Party.jpg"; //contentPath + @"\TestUpload\Christmas Party.jpg";
        //    //    SteamAPICall_t handle = UploadToWorkshop(uploadItem.contentPath + "TestDoc.docx", uploadItem.title, uploadItem.description, uploadItem.tags, uploadItem.imagePreviewFile,
        //    //                ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPublic, EWorkshopFileType.k_EWorkshopFileTypeCommunity);
        //    //    Console.WriteLine("steam://url/CommunityFilePage/" + uploadItem.PublisherID);
        //    //    System.Diagnostics.Process.Start("steam://url/CommunityFilePage/" + uploadItem.PublisherID);
        //    //}


        //}

        //private SteamAPICall_t UploadToWorkshop(string fileName, string workshopTitle, string workshopDescription,  List<string> tags, string previewImage = null,
        //                     ERemoteStoragePublishedFileVisibility visability =ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPublic,
        //                     EWorkshopFileType workshopFileType= EWorkshopFileType.k_EWorkshopFileTypeCommunity)
        //{
        //    SteamAPICall_t handle = SteamRemoteStorage.PublishWorkshopFile(fileName, previewImage, SteamUtils.GetAppID(), workshopTitle, workshopDescription, visability, tags, workshopFileType);
        //    //RemoteStoragePublishFileResult.Set(handle);
        //    return handle;
        //}


        private void btnSendStats_Click(object sender, EventArgs e)
        {
            if (!SteamManager.Initialized)
                return;

            SteamUserStats.SetStat("Cows Rescued", 3);
            SteamUserStats.SetAchievement("FinishSinglePlayer");

            bool bSuccess = SteamUserStats.StoreStats();

            Program.debugString += "StoreStats Success : " + bSuccess + newLine;
            txtDebug.Text = Program.debugString;
        }



        private void button1_Click(object sender, EventArgs e)
        {
            Program.debugString += "DLC Ownership of 369040 : " +  SteamManager.ownsDLC(369040) + newLine;
            Program.debugString += "DLC Ownership of 269040 : " + SteamManager.ownsDLC(269040) + newLine;
            txtDebug.Text = Program.debugString;
        }

        private void btnVerifyAntiCheat_Click(object sender, EventArgs e)
        {
            byte[] tempTicket = SteamManager.SteamAntiCheatClass.getTicket();

            string reasult = SteamManager.SteamAntiCheatClass.verifyTicket(tempTicket, SteamUser.GetSteamID()).ToString();
            Program.debugString += "Anti Cheat Check : " + reasult + newLine;
            txtDebug.Text = Program.debugString;
        }

        private void button1_Click_1(object sender, EventArgs e)
        {

            SteamManager.SteamUGCworkshop.queryWorkshop("Levels");

            Program.debugString += "Query sent: " + newLine; //+ bSuccess
            txtDebug.Text = Program.debugString;
        }

        private void btnUploadProgress_Click(object sender, EventArgs e)
        {
            //getProgress

                ulong temp;
                ulong temp2;
                EItemUpdateStatus progress = SteamManager.SteamUGCworkshop.getProgress(SteamManager.SteamUGCworkshop.m_UGCUpdateHandle, out temp, out temp2);


            Program.debugString += "Upload progress: " + progress + "\n" + newLine; //+ bSuccess
            

            SteamManager.SteamUGCworkshop.m_UGCUpdateHandle = SteamUGC.StartItemUpdate((AppId_t)480, SteamManager.SteamUGCworkshop.m_PublishedFileId);
            Program.debugString += "SteamUGC.StartItemUpdate((AppId_t)480, " + SteamManager.SteamUGCworkshop.m_PublishedFileId + ") : " + SteamManager.SteamUGCworkshop.m_UGCUpdateHandle + newLine;

            txtDebug.Text = Program.debugString;
        }

        private void btnGetSubscribed_Click(object sender, EventArgs e)
        {
            SteamManager.SteamUGCworkshop.GetSubscribedItems();

            Program.debugString += "GetSubscribedItems done: "  + newLine; //+ bSuccess
            txtDebug.Text = Program.debugString;
        }

        private void CheckCallBacks_Tick(object sender, EventArgs e)
        {
            SteamAPI.RunCallbacks();
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