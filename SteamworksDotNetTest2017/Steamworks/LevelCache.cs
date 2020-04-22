//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.IO;
//using PlazSharp.Serialization;
//using Newtonsoft.Json;

//namespace Yargis
//{
//    /*
//     * Cache prefers these locations in order:
//     * 1. Program Files
//     * 2. Cache (from network games)
//     * 3. Saved Games
//     * 
//     * If a level of a same name exists at a lower level of that list
//     * (so Saved Games overrides Cache) the game will use that.
//     * If the game needs to pull a specific version (by SHA)
//     * it should pull from Cache if necessary.
//     * 
//     * TODO * If the same level exists in two places, the newer one will be
//     * used (except for Cache which is only used if no other copy
//     * is available.)
//     * */
//    public abstract class LevelList : IGameSerializable
//    {
//        public List<CachedLoadableFile> Levels = new List<CachedLoadableFile>();

//        public abstract List<LoadableFile> GetLevelListFromDisk();
        
//        public void Reset()
//        {
//            Levels.Clear();
//            Update();
//        }

//		public virtual void Update()
//		{
//			// Remove nonexistent. Do this before adding new
//			// so that if a player deleted some levels in SavedGames
//			// we see the ones in Content again.
//			for (int i = 0; i < Levels.Count; i++)
//			{
//				if (!Levels[i].Exists)
//				{
//					Levels.RemoveAt(i);
//					i--;
//				}
//			}

//			// Add new levels
//			var list = GetLevelListFromDisk();
//			for (int i = 0; i < list.Count; i++)
//			{
//				bool exists = false;
//				var file = list[i];
//				for (int j = 0; j < Levels.Count; j++)
//				{
//					if (file.DisplayName == Levels[j].DisplayName)
//					{
//						// Update the cache
//						var fi = new FileInfo(file.RawFileName);
//						if (fi.Exists && fi.GetSafeLastWriteTimeUtc() != Levels[j].LastModified)
//						{
//							try
//							{
//								try
//								{
//									Level level = null;
//									using (var levelData = file.GetDataStream())
//									{
//#if WINDOWS
//										level = PlazSharp.Serialization.JSON.DeserializeStream<Level>(levelData);
//#endif

//										levelData.Seek(0, SeekOrigin.Begin);

//										UpdateCacheEntry(j, levelData, level);
//									}
//								}
//								catch
//								{
//									Levels.RemoveAt(j);
//									throw;
//								}
//							}
//							catch (Newtonsoft.Json.JsonException)
//							{
//								// invalid file: skip it.
//							}
//							catch (System.IO.IOException)
//							{
//								// read error
//							}
//							catch (System.Security.SecurityException)
//							{
//								// access denied
//							}
//						}

//						exists = true;
//						break;
//					}
//				}

//				if (!exists && list[i].Exists)
//				{
//					try
//					{
//						var cached = GetCacheEntry(list[i]);
//						if (cached != null)
//							Levels.Add(cached);
//					}
//					catch (Newtonsoft.Json.JsonException)
//					{
//						// invalid file: skip it.
//					}
//					catch (System.IO.IOException)
//					{
//						// read error
//					}
//					catch (System.Security.SecurityException)
//					{
//						// access denied
//					}
//				}
//			}

//			Levels.Sort();
//		}

//		/// <summary>
//		/// (Prefer the overload accepting a stream instead.)
//		/// </summary>
//		/// <param name="idx"></param>
//		/// <param name="levelData"></param>
//		/// <param name="level"></param>
//		public virtual void UpdateCacheEntry(int idx, string levelData, Level level)
//		{
//			Levels[idx].SHASum = levelData.SHA1();
//#if WINDOWS
//			Levels[idx].Preview = new GamePreview(level.Preview);
//#endif
//			Levels[idx].UpdateLastModified();
//		}

//		public virtual void UpdateCacheEntry(int idx, Stream levelData, Level level)
//		{
//			Levels[idx].SHASum = levelData.SHA1();
//#if WINDOWS
//			Levels[idx].Preview = new GamePreview(level.Preview);
//#endif
//			Levels[idx].UpdateLastModified();
//		}

//        protected static CachedLoadableFile GetCacheEntry(LoadableFile file)
//        {
//#if WINDOWS
//            Level theLevel = Level.Load(file);
//            var preview = new GamePreview(theLevel.Preview);
//#endif

//            return new CachedLoadableFile()
//            {
//                FileName = file.FileName,
//                DisplayName = file.DisplayName,
//                SHASum = file.CalculateSHA(),
//                Location = file.Location,

//#if WINDOWS
//                Preview = preview,
//#endif
//                LastModified = new FileInfo(file.RawFileName).GetSafeLastWriteTimeUtc()
//            };
//        }

//        protected static List<LoadableFile> GetCompleteLevelList()
//        {
//            int substr = "World/".Length;

//            List<LoadableFile> ret = new List<LoadableFile>();

//            List<string> content = new List<string>(XEL.Xel.Assets.Content.Load<List<string>>("world"));
//            for (int i = 0; i < content.Count; i++)
//            {
//                ret.Add(new LoadableFile(content[i].Substring(substr)));
//            }

//            //TODO: Add from the content directory, even if not in world manifest
//            // Isaac <ibrodsky@plazsoft.com> 8/1/2014

//            AddDirectory(ret, FileNames.LevelCachePath, DataLocation.Cache);
//            AddDirectory(ret, FileNames.StorageContainerPath, DataLocation.Storage);

//            return ret;
//        }

//        private static void AddDirectory(List<LoadableFile> ret, string path, DataLocation location)
//        {
//            DirectoryInfo storage = new DirectoryInfo(path);
//            if (storage.Exists)
//            {
//                foreach (var f in storage.GetFiles())
//                {
//                    if (f.Name.EndsWith(FileNames.extLV, StringComparison.OrdinalIgnoreCase)
//                        || f.Name.EndsWith(FileNames.extStory, StringComparison.OrdinalIgnoreCase))
//                    {
//                        var storageLevel = new LoadableFile(f.Name, location);
//                        for (int i = 0; i < ret.Count; i++)
//                        {
//                            if (ret[i].DisplayName == storageLevel.DisplayName)
//                            {
//                                ret.RemoveAt(i);
//                            }
//                        }
//                        ret.Add(storageLevel);
//                    }
//                }
//            }
//        }

//        public void OnSerialize()
//        {
//            for (int i = 0; i < Levels.Count; i++)
//            {
//                Levels[i].OnSerialize();
//            }
//        }

//        public void OnDeserialize()
//        {
//            for (int i = 0; i < Levels.Count; i++)
//            {
//                Levels[i].OnDeserialize();
//            }
//        }
//    }

//    public class SinglePlayerLevelList : LevelList
//    {
//        public override List<LoadableFile> GetLevelListFromDisk()
//        {
//            var list = GetCompleteLevelList();
//            list.RemoveAll(l => !l.FileName.EndsWith(FileNames.extStory, StringComparison.OrdinalIgnoreCase));
//            return list;
//        }
        
//		public override void UpdateCacheEntry(int idx, string levelData, Level level)
//        {
//            Levels[idx].UpdateLastModified();
//        }

//        public override void UpdateCacheEntry(int idx, Stream levelData, Level level)
//        {
//            Levels[idx].UpdateLastModified();
//        }
//    }

//    public class LevelCache : LevelList
//    {
//        public override List<LoadableFile> GetLevelListFromDisk()
//        {
//            var list = GetCompleteLevelList();
//            list.RemoveAll(l => !l.FileName.EndsWith(FileNames.extLV, StringComparison.OrdinalIgnoreCase));
//            return list;
//        }

//        public bool FindCachedLevel(ref LoadableLevel load)
//        {
//            try
//            {
//                DirectoryInfo cacheDir = new DirectoryInfo(FileNames.LevelCachePath);
//                if (!cacheDir.Exists)
//                    cacheDir.Create();

//                LoadableFile cached = new LoadableFile(load.DisplayName + "." + load.SHASum + FileNames.extLV, DataLocation.Cache);
				
//                if (cached.Exists && cached.Equals(load))
//                {
//                    load = cached;
//                }
//                return cached.Exists;
//            }
//            catch
//            {

//            }

//            return false;
//        }

//        public bool ReceiveLevel(ref LoadableLevel load, string levelData)
//        {
//            bool done = false;
//            if (Global.Settings.NetCacheLevels)
//            {
//                try
//                {
//                    DirectoryInfo cacheDir = new DirectoryInfo(FileNames.LevelCachePath);
//                    if (!cacheDir.Exists)
//                        cacheDir.Create();

//                    LoadableFile cached = new LoadableFile(load.DisplayName + "." + load.SHASum + FileNames.extLV, DataLocation.Cache);
//                    //If it already exists, we would have selected it and loaded it
//                    //rather than requesting the level from the server.
//                    if (!cached.Exists)
//                    {
//                        File.WriteAllText(cached.RawFileName, levelData);
//						// contains same level data, so keep the SHASum
//						cached.SHASum = load.SHASum;
//                        bool foundLevel = false;
//                        for (int i = 0; i < Levels.Count; i++)
//                        {
//                            if (Levels[i].DisplayName == cached.DisplayName)
//                            {
//                                foundLevel = true;
//                                break;
//                            }
//                        }
//                        if (!foundLevel)
//                            Levels.Add(GetCacheEntry(cached));
//                        load = cached;
//                        done = true;
//                    }
//                }
//                catch
//                {
//                    done = false;
//                }
//            }

//            if (!done)
//            {
//                load = new LoadableMemory(load.FileName, load.SHASum, levelData);
//            }
//            return true;
//        }

//        public bool FindCachedLevelByPublishedID(Steamworks.PublishedFileId_t ID)
//        {
//            return Levels.Exists(x => x.Preview.PublisherID == ID);
//        }
//    }
//}
