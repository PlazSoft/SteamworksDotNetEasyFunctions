//#if STEAMCLIENT
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Steamworks;
//using System.IO;

//namespace YargisEditor
//{
//    public class Steam
//    {
//        public static string editNote = "";


//        public static void uploadToWorkshop(string level, File file, PublishedFileId_t PublisherID, string Title, string Description, ERemoteStoragePublishedFileVisibility Visibility)
//        {

//            if (SteamManager.Initialized)
//            {
//                if (PublisherID == (Steamworks.PublishedFileId_t)0)
//                {
//                    SteamManager.SteamUGCworkshop.CreateWorkshopItem();
//                }
//                else
//                {
//                    string contentPath = Path.GetTempPath() + @"Yargis\" + PublisherID + @"\";
//                    if (!Directory.Exists(contentPath))
//                    {
//                        Directory.CreateDirectory(contentPath);
//                    }
//                    file.SaveLevelAs(contentPath + Path.GetFileName(file.currentLevelFileName));
//                    List<string> tags = new List<string>();
//                    tags.Add("Levels");
//                    FileStream stream = new FileStream(contentPath + @"LevelPreview.jpg", FileMode.Create);
//                    level.Preview.LevelPreview.SaveAsJpeg(stream, level.Preview.LevelPreview.Width, level.Preview.LevelPreview.Height);
//                    stream.Close();
//                    UGCUpdateHandle_t updateHandle = SteamManager.SteamUGCworkshop.registerFileInfoOrUpdate(PublisherID, Title, Description,
//                        Visibility, tags, contentPath, contentPath + "LevelPreview.jpg");
//                    Steam.sendFile(updateHandle);
//                }
//            }
//        }

//        public static void sendFile(UGCUpdateHandle_t UpdateHandle)
//        {
//            SteamAPICall_t SteamAPICall_t_handle = SteamManager.SteamUGCworkshop.sendFileOrUpdate(UpdateHandle, editNote);
//            Console.WriteLine("sendFileOrUpdate (SteamAPICall_t_handle): " + SteamAPICall_t_handle);
//        }

//        public static void openPublishedFile(string m_PublishedFileId)
//        {
//            System.Diagnostics.Process.Start("steam://url/CommunityFilePage/" + m_PublishedFileId);

//        }





//    }
//}
//#endif