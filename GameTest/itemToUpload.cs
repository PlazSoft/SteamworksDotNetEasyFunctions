using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameTest
{
    public class itemToUpload
    {
        public PublishedFileId_t PublishedFileID;
        public List<string> tags;
        public string contentPath;
        public string title;
        public string description;
        public string imagePreviewFile;

        public static void uploadToWorkshop(PublishedFileId_t PublisherID, List<string> tags, string contentPath, string title, string description, string imagePreviewFile)
        {
            //FileStream stream = new FileStream(contentPath + @"LevelPreview.jpg", FileMode.Create);
            //level.Preview.LevelPreview.SaveAsJpeg(stream, level.Preview.LevelPreview.Width, level.Preview.LevelPreview.Height);
            //stream.Close();
            UGCUpdateHandle_t updateHandle = SteamManager.SteamUGCworkshop.registerFileInfoOrUpdate(PublisherID, title, description,
                ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPrivate, tags, contentPath, imagePreviewFile);

            SteamAPICall_t SteamAPICall_t_handle = SteamManager.SteamUGCworkshop.sendFileOrUpdate(updateHandle, "Update NOtes are helpful... " + PublisherID);
            Console.WriteLine("sendFileOrUpdate (SteamAPICall_t_handle): " + SteamAPICall_t_handle);

        }
    }
}
