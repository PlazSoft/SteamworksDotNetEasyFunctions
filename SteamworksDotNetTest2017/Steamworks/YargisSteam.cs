
#if STEAMCLIENT
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Steamworks;
using System.IO;
using System.Net;
//using Yargis;  //This may cause a circular refrence.

namespace Steamworks
{
    public class YargisSteam
    {
        public static string Log = "***LOG START***\n";

        public static void CheckSubscribedItems(Yargis.LevelCache _multiLevelCache, Yargis.LevelCache _singleLevelCache)
        {
            if (!SteamManager.Initialized)
                return;

            uint number = SteamUGC.GetNumSubscribedItems();
            PublishedFileId_t[] items = new PublishedFileId_t[number];
            SteamUGC.GetSubscribedItems(items, number);
            long itemCount = (long)number;
            for (long x = 0; x <= itemCount - 1; x++)
            {
                //if (items[x].ToString().Contains("468449740"))
                //{                }
                //if (!isItemInstalledOnYargis(items[x], _multiLevelCache, _singleLevelCache))  //We can use SteamManager.SteamUGCworkshop.getID to verify if it is still valid.
                //{ 
                    ulong punSizeOnDisk;
                    string pchFolder;
                    uint cchFolderSize = 260;
                    uint punTimeStamp;
                    bool inReadyOnSteam = SteamUGC.GetItemInstallInfo(items[x], out punSizeOnDisk, out pchFolder,  cchFolderSize, out punTimeStamp);
                    if (inReadyOnSteam)
                        installFiles(pchFolder, _multiLevelCache);
                //}
                Console.Write("[" + x + "]: " + items[x] + ", ");
            }
            Console.WriteLine("");
        }

        private static void installFiles(string steamTempFolder, Yargis.LevelCache _levelCache)
        {
            //Check multiplayer levels
            installToCache(steamTempFolder, _levelCache, "*.ylv");

            //Check singleplayer levels
            installToCache(steamTempFolder, _levelCache, "*.yslv");
        }

        private static void installToCache(string steamTempFolder, Yargis.LevelCache _levelCache, string fileCards)
        {
            if (steamTempFolder.Contains("530999920"))
             {       }
            
            string[] files = null;
            try
            {
                if (Directory.Exists(steamTempFolder))
                    files = Directory.GetFiles(steamTempFolder, fileCards);
            }
            catch
            {
                Console.WriteLine("Invalid steamTempFolder!! " + steamTempFolder); 
            }

            if (files != null)
            {
                foreach (string file in files)
                {
                    try
                    {
                        FileInfo file_info = new FileInfo(file);
                        MoveWithReplace(steamTempFolder + @"\" + file_info.Name, Yargis.FileNames.StorageContainerPath + @"\" + file_info.Name);
                    }
                    catch
                    {
                        Console.WriteLine("Steam copy fail!! " + steamTempFolder);
                    }
                }

                _levelCache.Update(); 

                //Delete folder when done:
                try
                {
                    if (Directory.Exists(steamTempFolder))
                        Directory.Delete(steamTempFolder, true);
                }
                catch
                { }
            }
        }


        /// <summary>
        /// Checks publisher ID in Steam. Copies the file and adds to cache.
        /// </summary>
        /// <returns></returns>
        private static bool isItemInstalledOnYargis(PublishedFileId_t publishedFileID, Yargis.LevelCache _multiLevelCache, Yargis.LevelCache _singleLevelCache)
        {
            if (!_multiLevelCache.FindCachedLevelByPublishedID(publishedFileID) && !_singleLevelCache.FindCachedLevelByPublishedID(publishedFileID))
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        public static void MoveWithReplace(string sourceFileName, string destFileName)
        {

            //first, delete target file if exists, as File.Move() does not support overwrite
            if (File.Exists(destFileName))
            {
                File.Delete(destFileName);
            }

            File.Move(sourceFileName, destFileName);

        }


        internal static void AdvertiseServer(CSteamID cSteamID, System.Net.IPAddress iPAddress, ushort port)
        {
            SteamManager.SteamAntiCheatClass.AdvertiseGame(cSteamID, iPAddress, port);
        }

        internal static void AdvertiseServer(string iPAddress, string port)
        {
            SteamManager.SteamAntiCheatClass.AdvertiseGame(CSteamID.NonSteamGS, IPAddress.Parse(iPAddress), (ushort)Convert.ToInt32(port));

#if STEAMCLIENT || STEAMDEDICATED
            SteamManager.SteamMatchmakingClass.createLobbyGameServer(ELobbyType.k_ELobbyTypePublic);
            Yargis.GUI.Screens.ConnectionsScreen.currentSteamServer.IpAddress = iPAddress;
            Yargis.GUI.Screens.ConnectionsScreen.currentSteamServer.port = port;
            Yargis.GUI.Screens.ConnectionsScreen.currentSteamServer.password = "";

#endif
            //SteamLobby.createLobby(ELobbyType.k_ELobbyTypePublic, 512);
        }



        /// <summary>
        /// Host (IP), Port, and Password (you can leave blank "")
        /// </summary>
        /// <param name="host">Host (Server IP)</param>
        /// <param name="port">Port number</param>
        /// <param name="pwd">Password (you can leave blank "")</param>
        internal static void ChangeServer(string host, string port, string pwd)
        {
            Yargis.GUI.Screens.ConnectionsScreen.Connect(host, port, pwd);
        }

        /// <summary>
        /// Host (IP), Port, and Password (you can leave blank "")
        /// </summary>
        /// <param name="host">Host (Server IP)</param>
        /// <param name="port"></param>
        /// <param name="pwd">Password (you can leave blank "")</param>
        internal static void ChangeServer(uint host, uint port, string pwd)
        {

            Yargis.GUI.Screens.ConnectionsScreen.Connect(SteamAntiCheat.ToAddr(host), port.ToString(), pwd);
        }

        internal static List<uint> CheckDlcList()
        {
            List<uint> retunList = new List<uint>();
            uint tempCheck = 0;

            tempCheck = 383610; //Music, artwork, and basic ship pack
            if (SteamManager.isValidApp(tempCheck))
                retunList.Add(tempCheck);
            tempCheck = 402950; //Extra ships
            if (SteamManager.isValidApp(tempCheck))
                retunList.Add(tempCheck);

            return retunList;
        }

        public static SteamAPICall_t JoinLobbyGame(string lobby)
        {
            try
            {
                //public static SteamAPICall_t joinLobby(CSteamID steamIDLobby)   //{
                ulong vOut = Convert.ToUInt64(lobby);
               // CSteamID tempLobbyID = new CSteamID((ulong)lobby);
                return SteamMatchmaking.JoinLobby((CSteamID)vOut);
            }
            catch
            {
                return new SteamAPICall_t();
            }
        }



    }
    
}
#endif