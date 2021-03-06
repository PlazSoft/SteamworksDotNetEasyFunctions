﻿#if STEAMCLIENT
using System.Collections;
using Steamworks;
using System;

/// <summary>
/// Best to use SteamUGC, but there are a couple of callbacks that come back to this old system.
/// </summary>
public class SteamRemoteStorageTest 
{

    const string MESSAGE_FILE_NAME = "message.dat";

    private string m_Message = "";
    private int m_FileCount;
    private int m_FileSize;
    private int m_TotalBytes;
    private int m_FileSizeInBytes;
    private UGCFileWriteStreamHandle_t m_FileStream;
    private UGCHandle_t m_UGCHandle;
    private PublishedFileId_t m_PublishedFileId;
    private PublishedFileUpdateHandle_t m_PublishedFileUpdateHandle;

    protected Callback<RemoteStorageAppSyncedClient_t> m_RemoteStorageAppSyncedClient;
    protected Callback<RemoteStorageAppSyncedServer_t> m_RemoteStorageAppSyncedServer;
    protected Callback<RemoteStorageAppSyncProgress_t> m_RemoteStorageAppSyncProgress;
    protected Callback<RemoteStorageAppSyncStatusCheck_t> m_RemoteStorageAppSyncStatusCheck;
    protected Callback<RemoteStorageConflictResolution_t> m_RemoteStorageConflictResolution;
    protected Callback<RemoteStoragePublishedFileSubscribed_t> m_RemoteStoragePublishedFileSubscribed;
    protected Callback<RemoteStoragePublishedFileUnsubscribed_t> m_RemoteStoragePublishedFileUnsubscribed;
    protected Callback<RemoteStoragePublishedFileDeleted_t> m_RemoteStoragePublishedFileDeleted;
    protected Callback<RemoteStoragePublishFileProgress_t> m_RemoteStoragePublishFileProgress;
    protected Callback<RemoteStoragePublishedFileUpdated_t> m_RemoteStoragePublishedFileUpdated;

    private CallResult<RemoteStorageFileShareResult_t> RemoteStorageFileShareResult;
    private CallResult<RemoteStoragePublishFileResult_t> RemoteStoragePublishFileResult;
    private CallResult<RemoteStorageDeletePublishedFileResult_t> RemoteStorageDeletePublishedFileResult;
    private CallResult<RemoteStorageEnumerateUserPublishedFilesResult_t> RemoteStorageEnumerateUserPublishedFilesResult;
    private CallResult<RemoteStorageSubscribePublishedFileResult_t> RemoteStorageSubscribePublishedFileResult;
    private CallResult<RemoteStorageEnumerateUserSubscribedFilesResult_t> RemoteStorageEnumerateUserSubscribedFilesResult;
    private CallResult<RemoteStorageUnsubscribePublishedFileResult_t> RemoteStorageUnsubscribePublishedFileResult;
    private CallResult<RemoteStorageUpdatePublishedFileResult_t> RemoteStorageUpdatePublishedFileResult;
    private CallResult<RemoteStorageDownloadUGCResult_t> RemoteStorageDownloadUGCResult;
    private CallResult<RemoteStorageGetPublishedFileDetailsResult_t> RemoteStorageGetPublishedFileDetailsResult;
    private CallResult<RemoteStorageEnumerateWorkshopFilesResult_t> RemoteStorageEnumerateWorkshopFilesResult;
    private CallResult<RemoteStorageGetPublishedItemVoteDetailsResult_t> RemoteStorageGetPublishedItemVoteDetailsResult;
    private CallResult<RemoteStorageUpdateUserPublishedItemVoteResult_t> RemoteStorageUpdateUserPublishedItemVoteResult;
    private CallResult<RemoteStorageUserVoteDetails_t> RemoteStorageUserVoteDetails;
    private CallResult<RemoteStorageEnumerateUserSharedWorkshopFilesResult_t> RemoteStorageEnumerateUserSharedWorkshopFilesResult;
    private CallResult<RemoteStorageSetUserPublishedFileActionResult_t> RemoteStorageSetUserPublishedFileActionResult;
    private CallResult<RemoteStorageEnumeratePublishedFilesByUserActionResult_t> RemoteStorageEnumeratePublishedFilesByUserActionResult;

    public delegate void CheckSubsribedItemsEventHandler();
    public event CheckSubsribedItemsEventHandler OnItemSubscribeCheck;

    public SteamRemoteStorageTest()
    {
        m_RemoteStorageAppSyncedClient = Callback<RemoteStorageAppSyncedClient_t>.Create(OnRemoteStorageAppSyncedClient);
        m_RemoteStorageAppSyncedServer = Callback<RemoteStorageAppSyncedServer_t>.Create(OnRemoteStorageAppSyncedServer);
        m_RemoteStorageAppSyncProgress = Callback<RemoteStorageAppSyncProgress_t>.Create(OnRemoteStorageAppSyncProgress);
        m_RemoteStorageAppSyncStatusCheck = Callback<RemoteStorageAppSyncStatusCheck_t>.Create(OnRemoteStorageAppSyncStatusCheck);
        m_RemoteStorageConflictResolution = Callback<RemoteStorageConflictResolution_t>.Create(OnRemoteStorageConflictResolution);
        m_RemoteStoragePublishedFileSubscribed = Callback<RemoteStoragePublishedFileSubscribed_t>.Create(OnRemoteStoragePublishedFileSubscribed);
        m_RemoteStoragePublishedFileUnsubscribed = Callback<RemoteStoragePublishedFileUnsubscribed_t>.Create(OnRemoteStoragePublishedFileUnsubscribed);
        m_RemoteStoragePublishedFileDeleted = Callback<RemoteStoragePublishedFileDeleted_t>.Create(OnRemoteStoragePublishedFileDeleted);
        m_RemoteStoragePublishFileProgress = Callback<RemoteStoragePublishFileProgress_t>.Create(OnRemoteStoragePublishFileProgress);
        m_RemoteStoragePublishedFileUpdated = Callback<RemoteStoragePublishedFileUpdated_t>.Create(OnRemoteStoragePublishedFileUpdated);

        RemoteStorageFileShareResult = CallResult<RemoteStorageFileShareResult_t>.Create(OnRemoteStorageFileShareResult);
        RemoteStoragePublishFileResult = CallResult<RemoteStoragePublishFileResult_t>.Create(OnRemoteStoragePublishFileResult);
        RemoteStorageDeletePublishedFileResult = CallResult<RemoteStorageDeletePublishedFileResult_t>.Create(OnRemoteStorageDeletePublishedFileResult);
        RemoteStorageEnumerateUserPublishedFilesResult = CallResult<RemoteStorageEnumerateUserPublishedFilesResult_t>.Create(OnRemoteStorageEnumerateUserPublishedFilesResult);
        RemoteStorageSubscribePublishedFileResult = CallResult<RemoteStorageSubscribePublishedFileResult_t>.Create(OnRemoteStorageSubscribePublishedFileResult);
        RemoteStorageEnumerateUserSubscribedFilesResult = CallResult<RemoteStorageEnumerateUserSubscribedFilesResult_t>.Create(OnRemoteStorageEnumerateUserSubscribedFilesResult);
        RemoteStorageUnsubscribePublishedFileResult = CallResult<RemoteStorageUnsubscribePublishedFileResult_t>.Create(OnRemoteStorageUnsubscribePublishedFileResult);
        RemoteStorageUpdatePublishedFileResult = CallResult<RemoteStorageUpdatePublishedFileResult_t>.Create(OnRemoteStorageUpdatePublishedFileResult);
        RemoteStorageDownloadUGCResult = CallResult<RemoteStorageDownloadUGCResult_t>.Create(OnRemoteStorageDownloadUGCResult);
        RemoteStorageGetPublishedFileDetailsResult = CallResult<RemoteStorageGetPublishedFileDetailsResult_t>.Create(OnRemoteStorageGetPublishedFileDetailsResult);
        RemoteStorageEnumerateWorkshopFilesResult = CallResult<RemoteStorageEnumerateWorkshopFilesResult_t>.Create(OnRemoteStorageEnumerateWorkshopFilesResult);
        RemoteStorageGetPublishedItemVoteDetailsResult = CallResult<RemoteStorageGetPublishedItemVoteDetailsResult_t>.Create(OnRemoteStorageGetPublishedItemVoteDetailsResult);
        RemoteStorageUpdateUserPublishedItemVoteResult = CallResult<RemoteStorageUpdateUserPublishedItemVoteResult_t>.Create(OnRemoteStorageUpdateUserPublishedItemVoteResult);
        RemoteStorageUserVoteDetails = CallResult<RemoteStorageUserVoteDetails_t>.Create(OnRemoteStorageUserVoteDetails);
        RemoteStorageEnumerateUserSharedWorkshopFilesResult = CallResult<RemoteStorageEnumerateUserSharedWorkshopFilesResult_t>.Create(OnRemoteStorageEnumerateUserSharedWorkshopFilesResult);
        RemoteStorageSetUserPublishedFileActionResult = CallResult<RemoteStorageSetUserPublishedFileActionResult_t>.Create(OnRemoteStorageSetUserPublishedFileActionResult);
        RemoteStorageEnumeratePublishedFilesByUserActionResult = CallResult<RemoteStorageEnumeratePublishedFilesByUserActionResult_t>.Create(OnRemoteStorageEnumeratePublishedFilesByUserActionResult);
    }

    //public void RenderOnGUI(SteamTest.EGUIState state)
    //{
    //    GUILayout.BeginArea(new Rect(Screen.width - 200, 0, 200, Screen.height));
    //    Console.WriteLine("Variables:");
    //    Console.WriteLine("m_Message: ");
    //    m_Message = GUILayout.TextField(m_Message, 40);
    //    Console.WriteLine("m_FileCount: " + m_FileCount);
    //    Console.WriteLine("m_FileSize: " + m_FileSize);
    //    Console.WriteLine("m_TotalBytes: " + m_TotalBytes);
    //    Console.WriteLine("m_FileSizeInBytes: " + m_FileSizeInBytes);
    //    Console.WriteLine("m_FileStream: " + m_FileStream);
    //    Console.WriteLine("m_UGCHandle: " + m_UGCHandle);
    //    Console.WriteLine("m_PublishedFileId: " + m_PublishedFileId);
    //    Console.WriteLine("m_PublishedFileUpdateHandle: " + m_PublishedFileUpdateHandle);
    //    GUILayout.EndArea();

    //    if (state == SteamTest.EGUIState.SteamRemoteStorage)
    //    {
    //        RenderPageOne();
    //    }
    //    else
    //    {
    //        RenderPageTwo();
    //    }
    //}

    //private void RenderPageOne()
    //{
    //    if (GUILayout.Button("FileWrite(MESSAGE_FILE_NAME, Data, Data.Length)"))
    //    {
    //        if (System.Text.Encoding.UTF8.GetByteCount(m_Message) > m_TotalBytes)
    //        {
    //            Console.WriteLine("Remote Storage: Quota Exceeded! - Bytes: " + System.Text.Encoding.UTF8.GetByteCount(m_Message) + " - Max: " + m_TotalBytes);
    //        }
    //        else
    //        {
    //            byte[] Data = new byte[System.Text.Encoding.UTF8.GetByteCount(m_Message)];
    //            System.Text.Encoding.UTF8.GetBytes(m_Message, 0, m_Message.Length, Data, 0);

    //            bool ret = SteamRemoteStorage.FileWrite(MESSAGE_FILE_NAME, Data, Data.Length);
    //            Console.WriteLine("FileWrite(" + MESSAGE_FILE_NAME + ", Data, " + Data.Length + ") - " + ret);
    //        }
    //    }

    //    if (GUILayout.Button("FileRead(MESSAGE_FILE_NAME, Data, Data.Length)"))
    //    {
    //        if (m_FileSize > 40)
    //        {
    //            byte[] c = { 0 };
    //            Console.WriteLine("RemoteStorage: File was larger than expected. . .");
    //            SteamRemoteStorage.FileWrite(MESSAGE_FILE_NAME, c, 1);
    //        }
    //        else
    //        {
    //            byte[] Data = new byte[40];
    //            int ret = SteamRemoteStorage.FileRead(MESSAGE_FILE_NAME, Data, Data.Length);
    //            m_Message = System.Text.Encoding.UTF8.GetString(Data, 0, ret);
    //            Console.WriteLine("FileRead(" + MESSAGE_FILE_NAME + ", Data, " + Data.Length + ") - " + ret);
    //        }
    //    }

    //    if (GUILayout.Button("FileForget(MESSAGE_FILE_NAME)"))
    //    {
    //        bool ret = SteamRemoteStorage.FileForget(MESSAGE_FILE_NAME);
    //        Console.WriteLine("FileForget(" + MESSAGE_FILE_NAME + ") - " + ret);
    //    }

    //    if (GUILayout.Button("FileDelete(MESSAGE_FILE_NAME)"))
    //    {
    //        bool ret = SteamRemoteStorage.FileDelete(MESSAGE_FILE_NAME);
    //        Console.WriteLine("FileDelete(" + MESSAGE_FILE_NAME + ") - " + ret);
    //    }

    //    if (GUILayout.Button("FileShare(MESSAGE_FILE_NAME)"))
    //    {
    //        SteamAPICall_t handle = SteamRemoteStorage.FileShare(MESSAGE_FILE_NAME);
    //        RemoteStorageFileShareResult.Set(handle);
    //        Console.WriteLine("FileShare(" + MESSAGE_FILE_NAME + ") - " + handle);
    //    }

    //    if (GUILayout.Button("SetSyncPlatforms(MESSAGE_FILE_NAME, k_ERemoteStoragePlatformAll)"))
    //    {
    //        bool ret = SteamRemoteStorage.SetSyncPlatforms(MESSAGE_FILE_NAME, ERemoteStoragePlatform.k_ERemoteStoragePlatformAll);
    //        Console.WriteLine("SetSyncPlatforms(" + MESSAGE_FILE_NAME + ", ERemoteStoragePlatform.k_ERemoteStoragePlatformAll) - " + ret);
    //    }

    //    if (GUILayout.Button("FileWriteStreamOpen(MESSAGE_FILE_NAME)"))
    //    {
    //        m_FileStream = SteamRemoteStorage.FileWriteStreamOpen(MESSAGE_FILE_NAME);
    //        Console.WriteLine("FileWriteStreamOpen(" + MESSAGE_FILE_NAME + ") - " + m_FileStream);
    //    }

    //    if (GUILayout.Button("FileWriteStreamWriteChunk(m_FileStream, Data, Data.Length)"))
    //    {
    //        if (System.Text.Encoding.UTF8.GetByteCount(m_Message) > m_TotalBytes)
    //        {
    //            Console.WriteLine("Remote Storage: Quota Exceeded! - Bytes: " + System.Text.Encoding.UTF8.GetByteCount(m_Message) + " - Max: " + m_TotalBytes);
    //        }
    //        else
    //        {
    //            byte[] Data = new byte[System.Text.Encoding.UTF8.GetByteCount(m_Message)];
    //            System.Text.Encoding.UTF8.GetBytes(m_Message, 0, m_Message.Length, Data, 0);

    //            bool ret = SteamRemoteStorage.FileWriteStreamWriteChunk(m_FileStream, Data, Data.Length);
    //            Console.WriteLine("FileWriteStreamWriteChunk(" + m_FileStream + ", Data, " + Data.Length + ") - " + ret);
    //        }
    //    }

    //    if (GUILayout.Button("FileWriteStreamClose(m_FileStream)"))
    //    {
    //        bool ret = SteamRemoteStorage.FileWriteStreamClose(m_FileStream);
    //        Console.WriteLine("FileWriteStreamClose(" + m_FileStream + ") - " + ret);
    //    }

    //    if (GUILayout.Button("FileWriteStreamCancel(m_FileStream)"))
    //    {
    //        bool ret = SteamRemoteStorage.FileWriteStreamCancel(m_FileStream);
    //        Console.WriteLine("FileWriteStreamCancel(" + m_FileStream + ") - " + ret);
    //    }

    //    Console.WriteLine("FileExists(MESSAGE_FILE_NAME) : " + SteamRemoteStorage.FileExists(MESSAGE_FILE_NAME));
    //    Console.WriteLine("FilePersisted(MESSAGE_FILE_NAME) : " + SteamRemoteStorage.FilePersisted(MESSAGE_FILE_NAME));
    //    Console.WriteLine("GetFileSize(MESSAGE_FILE_NAME) : " + SteamRemoteStorage.GetFileSize(MESSAGE_FILE_NAME));
    //    Console.WriteLine("GetFileTimestamp(MESSAGE_FILE_NAME) : " + SteamRemoteStorage.GetFileTimestamp(MESSAGE_FILE_NAME));
    //    Console.WriteLine("GetSyncPlatforms(MESSAGE_FILE_NAME) : " + SteamRemoteStorage.GetSyncPlatforms(MESSAGE_FILE_NAME));

    //    m_FileCount = SteamRemoteStorage.GetFileCount();
    //    Console.WriteLine("GetFileCount() : " + m_FileCount);
    //    for (int i = 0; i < m_FileCount; ++i)
    //    {
    //        string FileName = SteamRemoteStorage.GetFileNameAndSize(i, out m_FileSize);
    //        Console.WriteLine("GetFileNameAndSize(i, out FileSize) : " + FileName + " -- " + m_FileSize);
    //    }

    //    {
    //        int AvailableBytes;
    //        bool ret = SteamRemoteStorage.GetQuota(out m_TotalBytes, out AvailableBytes);
    //        Console.WriteLine("GetQuota(out m_TotalBytes, out AvailableBytes) : " + ret + " -- " + m_TotalBytes + " -- " + AvailableBytes);
    //    }

    //    Console.WriteLine("IsCloudEnabledForAccount() : " + SteamRemoteStorage.IsCloudEnabledForAccount());

    //    {
    //        bool CloudEnabled = SteamRemoteStorage.IsCloudEnabledForApp();
    //        Console.WriteLine("IsCloudEnabledForApp() : " + CloudEnabled);

    //        if (GUILayout.Button("SetCloudEnabledForApp(!CloudEnabled)"))
    //        {
    //            SteamRemoteStorage.SetCloudEnabledForApp(!CloudEnabled);
    //            Console.WriteLine("SetCloudEnabledForApp(!CloudEnabled)");
    //        }
    //    }
    //}

    private void RenderPageTwo()
    {
//        if (GUILayout.Button("UGCDownload(m_UGCHandle, 0)"))
//        {
//            SteamAPICall_t handle = SteamRemoteStorage.UGCDownload(m_UGCHandle, 0);
//            RemoteStorageDownloadUGCResult.Set(handle);
//            Console.WriteLine("UGCDownload(" + m_UGCHandle + ", 0) - " + handle);
//        }

//        {
//            int BytesDownloaded;
//            int BytesExpected;
//            bool ret = SteamRemoteStorage.GetUGCDownloadProgress(m_UGCHandle, out BytesDownloaded, out BytesExpected);
//            Console.WriteLine("GetUGCDownloadProgress(m_UGCHandle, out BytesDownloaded, out BytesExpected) : " + ret + " -- " + BytesDownloaded + " -- " + BytesExpected);
//        }

//        // Spams warnings if called with an empty handle
//        if (m_UGCHandle != (UGCHandle_t)0)
//        {
//            AppId_t AppID;
//            string Name;
//            CSteamID SteamIDOwner;
//            bool ret = SteamRemoteStorage.GetUGCDetails(m_UGCHandle, out AppID, out Name, out m_FileSizeInBytes, out SteamIDOwner);
//            Console.WriteLine("GetUGCDetails(m_UGCHandle, out AppID, Name, out FileSizeInBytes, out SteamIDOwner) : " + ret + " -- " + AppID + " -- " + Name + " -- " + m_FileSizeInBytes + " -- " + SteamIDOwner);
//        }
//        else
//        {
//            Console.WriteLine("GetUGCDetails(m_UGCHandle, out AppID, Name, out FileSizeInBytes, out SteamIDOwner) : ");
//        }

//        if (GUILayout.Button("UGCRead(m_UGCHandle, Data, m_FileSizeInBytes, 0, EUGCReadAction.k_EUGCRead_Close)"))
//        {
//            byte[] Data = new byte[m_FileSizeInBytes];
//            int ret = SteamRemoteStorage.UGCRead(m_UGCHandle, Data, m_FileSizeInBytes, 0, EUGCReadAction.k_EUGCRead_Close);
//            Console.WriteLine("UGCRead(" + m_UGCHandle + ", Data, m_FileSizeInBytes, 0, EUGCReadAction.k_EUGCRead_Close) - " + ret + " -- " + System.Text.Encoding.UTF8.GetString(Data, 0, ret));
//        }

//        Console.WriteLine("GetCachedUGCCount() : " + SteamRemoteStorage.GetCachedUGCCount());
//        Console.WriteLine("GetCachedUGCHandle(0) : " + SteamRemoteStorage.GetCachedUGCHandle(0));

//#if _PS3 || _SERVER
//        //Console.WriteLine(" : " + SteamRemoteStorage.GetFileListFromServer());
//        //Console.WriteLine(" : " + SteamRemoteStorage.FileFetch(string pchFile));
//        //Console.WriteLine(" : " + SteamRemoteStorage.FilePersist(string pchFile));
//        //Console.WriteLine(" : " + SteamRemoteStorage.SynchronizeToClient());
//        //Console.WriteLine(" : " + SteamRemoteStorage.SynchronizeToServer());
//        //Console.WriteLine(" : " + SteamRemoteStorage.ResetFileRequestState());
//#endif

//        if (GUILayout.Button("PublishWorkshopFile([...])"))
//        {
//            string[] Tags = { "Test1", "Test2", "Test3" };
//            SteamAPICall_t handle = SteamRemoteStorage.PublishWorkshopFile(MESSAGE_FILE_NAME, null, SteamUtils.GetAppID(), "Title!", "Description!", ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPublic, Tags, EWorkshopFileType.k_EWorkshopFileTypeCommunity);
//            RemoteStoragePublishFileResult.Set(handle);
//            Console.WriteLine("PublishWorkshopFile(" + MESSAGE_FILE_NAME + ", null, " + SteamUtils.GetAppID() + ", \"Title!\", \"Description!\", k_ERemoteStoragePublishedFileVisibilityPublic, SteamParamStringArray(list), k_EWorkshopFileTypeCommunity)");
//        }

//        if (GUILayout.Button("CreatePublishedFileUpdateRequest(m_PublishedFileId)"))
//        {
//            m_PublishedFileUpdateHandle = SteamRemoteStorage.CreatePublishedFileUpdateRequest(m_PublishedFileId);
//            Console.WriteLine("CreatePublishedFileUpdateRequest(" + m_PublishedFileId + ") - " + m_PublishedFileUpdateHandle);
//        }

//        if (GUILayout.Button("UpdatePublishedFileFile(m_PublishedFileUpdateHandle, MESSAGE_FILE_NAME)"))
//        {
//            bool ret = SteamRemoteStorage.UpdatePublishedFileFile(m_PublishedFileUpdateHandle, MESSAGE_FILE_NAME);
//            Console.WriteLine("UpdatePublishedFileFile(" + m_PublishedFileUpdateHandle + ", " + MESSAGE_FILE_NAME + ") - " + ret);
//        }

//        if (GUILayout.Button("UpdatePublishedFilePreviewFile(m_PublishedFileUpdateHandle, null)"))
//        {
//            bool ret = SteamRemoteStorage.UpdatePublishedFilePreviewFile(m_PublishedFileUpdateHandle, null);
//            Console.WriteLine("UpdatePublishedFilePreviewFile(" + m_PublishedFileUpdateHandle + ", " + null + ") - " + ret);
//        }

//        if (GUILayout.Button("UpdatePublishedFileTitle(m_PublishedFileUpdateHandle, \"New Title\")"))
//        {
//            bool ret = SteamRemoteStorage.UpdatePublishedFileTitle(m_PublishedFileUpdateHandle, "New Title");
//            Console.WriteLine("UpdatePublishedFileTitle(" + m_PublishedFileUpdateHandle + ", \"New Title\") - " + ret);
//        }

//        if (GUILayout.Button("UpdatePublishedFileDescription(m_PublishedFileUpdateHandle, \"New Description\")"))
//        {
//            bool ret = SteamRemoteStorage.UpdatePublishedFileDescription(m_PublishedFileUpdateHandle, "New Description");
//            Console.WriteLine("UpdatePublishedFileDescription(" + m_PublishedFileUpdateHandle + ", \"New Description\") - " + ret);
//        }

//        if (GUILayout.Button("UpdatePublishedFileVisibility(m_PublishedFileUpdateHandle, k_ERemoteStoragePublishedFileVisibilityPublic)"))
//        {
//            bool ret = SteamRemoteStorage.UpdatePublishedFileVisibility(m_PublishedFileUpdateHandle, ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPublic);
//            Console.WriteLine("UpdatePublishedFileVisibility(" + m_PublishedFileUpdateHandle + ", " + ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPublic + ") - " + ret);
//        }

//        if (GUILayout.Button("UpdatePublishedFileTags(m_PublishedFileUpdateHandle, new string[] {\"First\", \"Second\", \"Third\"})"))
//        {
//            bool ret = SteamRemoteStorage.UpdatePublishedFileTags(m_PublishedFileUpdateHandle, new string[] { "First", "Second", "Third" });
//            Console.WriteLine("UpdatePublishedFileTags(" + m_PublishedFileUpdateHandle + ", " + new string[] { "First", "Second", "Third" } + ") - " + ret);
//        }

//        if (GUILayout.Button("CommitPublishedFileUpdate(m_PublishedFileUpdateHandle)"))
//        {
//            SteamAPICall_t handle = SteamRemoteStorage.CommitPublishedFileUpdate(m_PublishedFileUpdateHandle);
//            RemoteStorageUpdatePublishedFileResult.Set(handle);
//            Console.WriteLine("CommitPublishedFileUpdate(" + m_PublishedFileUpdateHandle + ") - " + handle);
//        }

//        if (GUILayout.Button("GetPublishedFileDetails(m_PublishedFileId, 0)"))
//        {
//            SteamAPICall_t handle = SteamRemoteStorage.GetPublishedFileDetails(m_PublishedFileId, 0);
//            RemoteStorageGetPublishedFileDetailsResult.Set(handle);
//            Console.WriteLine("GetPublishedFileDetails(" + m_UGCHandle + ", 0) - " + handle);
//        }

//        if (GUILayout.Button("DeletePublishedFile(m_PublishedFileId)"))
//        {
//            SteamAPICall_t handle = SteamRemoteStorage.DeletePublishedFile(m_PublishedFileId);
//            RemoteStorageDeletePublishedFileResult.Set(handle);
//            Console.WriteLine("DeletePublishedFile(" + m_PublishedFileId + ") - " + handle);
//        }

//        if (GUILayout.Button("EnumerateUserPublishedFiles(0)"))
//        {
//            SteamAPICall_t handle = SteamRemoteStorage.EnumerateUserPublishedFiles(0);
//            RemoteStorageEnumerateUserPublishedFilesResult.Set(handle);
//            Console.WriteLine("EnumerateUserPublishedFiles(0) - " + handle);
//        }

//        if (GUILayout.Button("SubscribePublishedFile(m_PublishedFileId)"))
//        {
//            SteamAPICall_t handle = SteamRemoteStorage.SubscribePublishedFile(m_PublishedFileId);
//            RemoteStorageSubscribePublishedFileResult.Set(handle);
//            Console.WriteLine("SubscribePublishedFile(" + m_PublishedFileId + ") - " + handle);
//        }

//        if (GUILayout.Button("EnumerateUserSubscribedFiles(0)"))
//        {
//            SteamAPICall_t handle = SteamRemoteStorage.EnumerateUserSubscribedFiles(0);
//            RemoteStorageEnumerateUserSubscribedFilesResult.Set(handle);
//            Console.WriteLine("EnumerateUserSubscribedFiles(0) - " + handle);
//        }

//        if (GUILayout.Button("UnsubscribePublishedFile(m_PublishedFileId)"))
//        {
//            SteamAPICall_t handle = SteamRemoteStorage.UnsubscribePublishedFile(m_PublishedFileId);
//            RemoteStorageUnsubscribePublishedFileResult.Set(handle);
//            Console.WriteLine("UnsubscribePublishedFile(" + m_PublishedFileId + ") - " + handle);
//        }

//        if (GUILayout.Button("UpdatePublishedFileSetChangeDescription(m_PublishedFileUpdateHandle, \"Changelog!\")"))
//        {
//            bool ret = SteamRemoteStorage.UpdatePublishedFileSetChangeDescription(m_PublishedFileUpdateHandle, "Changelog!");
//            Console.WriteLine("UpdatePublishedFileSetChangeDescription(" + m_PublishedFileUpdateHandle + ", \"Changelog!\") - " + ret);
//        }

//        if (GUILayout.Button("GetPublishedItemVoteDetails(m_PublishedFileId)"))
//        {
//            SteamAPICall_t handle = SteamRemoteStorage.GetPublishedItemVoteDetails(m_PublishedFileId);
//            RemoteStorageGetPublishedItemVoteDetailsResult.Set(handle);
//            Console.WriteLine("GetPublishedItemVoteDetails(" + m_PublishedFileId + ") - " + handle);
//        }

//        if (GUILayout.Button("UpdateUserPublishedItemVote(m_PublishedFileId, true)"))
//        {
//            SteamAPICall_t handle = SteamRemoteStorage.UpdateUserPublishedItemVote(m_PublishedFileId, true);
//            RemoteStorageUpdateUserPublishedItemVoteResult.Set(handle);
//            Console.WriteLine("UpdateUserPublishedItemVote(" + m_PublishedFileId + ") - " + handle);
//        }

//        if (GUILayout.Button("GetUserPublishedItemVoteDetails(m_PublishedFileId)"))
//        {
//            SteamAPICall_t handle = SteamRemoteStorage.GetUserPublishedItemVoteDetails(m_PublishedFileId);
//            RemoteStorageUserVoteDetails.Set(handle);
//            Console.WriteLine("GetUserPublishedItemVoteDetails(" + m_PublishedFileId + ") - " + handle);
//        }

//        if (GUILayout.Button("EnumerateUserSharedWorkshopFiles(SteamUser.GetSteamID(), 0, null, null)"))
//        {
//            SteamAPICall_t handle = SteamRemoteStorage.EnumerateUserSharedWorkshopFiles(SteamUser.GetSteamID(), 0, null, null);
//            RemoteStorageEnumerateUserSharedWorkshopFilesResult.Set(handle);
//            Console.WriteLine("EnumerateUserSharedWorkshopFiles(" + SteamUser.GetSteamID() + ", 0, System.IntPtr.Zero, System.IntPtr.Zero) - " + handle);
//        }

//        if (GUILayout.Button("PublishVideo([...])"))
//        {
//            SteamAPICall_t handle = SteamRemoteStorage.PublishVideo(EWorkshopVideoProvider.k_EWorkshopVideoProviderYoutube, "William Hunter", "Rmvb4Hktv7U", null, SteamUtils.GetAppID(), "Test Video", "Desc", ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPublic, null);
//            RemoteStoragePublishFileResult.Set(handle);
//            Console.WriteLine("PublishVideo(k_EWorkshopVideoProviderYoutube, \"William Hunter\", \"Rmvb4Hktv7U\", null, SteamUtils.GetAppID(), \"Test Video\", \"Desc\", k_ERemoteStoragePublishedFileVisibilityPublic, null) - " + handle);
//        }

//        if (GUILayout.Button("SetUserPublishedFileAction(m_PublishedFileId, k_EWorkshopFileActionPlayed)"))
//        {
//            SteamAPICall_t handle = SteamRemoteStorage.SetUserPublishedFileAction(m_PublishedFileId, EWorkshopFileAction.k_EWorkshopFileActionPlayed);
//            RemoteStorageSetUserPublishedFileActionResult.Set(handle);
//            Console.WriteLine("SetUserPublishedFileAction(" + m_PublishedFileId + ", " + EWorkshopFileAction.k_EWorkshopFileActionPlayed + ") - " + handle);
//        }

//        if (GUILayout.Button("EnumeratePublishedFilesByUserAction(k_EWorkshopFileActionPlayed, 0)"))
//        {
//            SteamAPICall_t handle = SteamRemoteStorage.EnumeratePublishedFilesByUserAction(EWorkshopFileAction.k_EWorkshopFileActionPlayed, 0);
//            RemoteStorageEnumeratePublishedFilesByUserActionResult.Set(handle);
//            Console.WriteLine("EnumeratePublishedFilesByUserAction(EWorkshopFileAction.k_EWorkshopFileActionPlayed, 0) - " + handle);
//        }

//        if (GUILayout.Button("EnumeratePublishedWorkshopFiles(k_EWorkshopEnumerationTypeRankedByVote, 0, 3, 0, IntPtr.Zero, IntPtr.Zero)"))
//        {
//            SteamAPICall_t handle = SteamRemoteStorage.EnumeratePublishedWorkshopFiles(EWorkshopEnumerationType.k_EWorkshopEnumerationTypeRankedByVote, 0, 3, 0, null, null);
//            RemoteStorageEnumerateWorkshopFilesResult.Set(handle);
//            Console.WriteLine("EnumeratePublishedWorkshopFiles(k_EWorkshopEnumerationTypeRankedByVote, 0, 3, 0, IntPtr.Zero, IntPtr.Zero) - " + handle);
//        }

        // There is absolutely no documentation on how to use this function, or what CallResult it gives you...
        // If you figure out how to use this then let me know!
        /*if (GUILayout.Button("UGCDownloadToLocation(m_UGCHandle, \"C:\\\", 0)")) {
            SteamAPICall_t handle = SteamRemoteStorage.UGCDownloadToLocation(m_UGCHandle, "C:\\", 0);

            Console.WriteLine("UGCDownloadToLocation(m_UGCHandle, \"C:\\\", 0)");
        }*/
    }

    void OnRemoteStorageAppSyncedClient(RemoteStorageAppSyncedClient_t pCallback)
    {
        Console.WriteLine("[" + RemoteStorageAppSyncedClient_t.k_iCallback + " - RemoteStorageAppSyncedClient] - " + pCallback.m_nAppID + " -- " + pCallback.m_eResult + " -- " + pCallback.m_unNumDownloads);
    }

    void OnRemoteStorageAppSyncedServer(RemoteStorageAppSyncedServer_t pCallback)
    {
        Console.WriteLine("[" + RemoteStorageAppSyncedServer_t.k_iCallback + " - RemoteStorageAppSyncedServer] - " + pCallback.m_nAppID + " -- " + pCallback.m_eResult + " -- " + pCallback.m_unNumUploads);
    }

    void OnRemoteStorageAppSyncProgress(RemoteStorageAppSyncProgress_t pCallback)
    {
        Console.WriteLine("[" + RemoteStorageAppSyncProgress_t.k_iCallback + " - RemoteStorageAppSyncProgress] - " + pCallback.m_rgchCurrentFile + " -- " + pCallback.m_nAppID + " -- " + pCallback.m_uBytesTransferredThisChunk + " -- " + pCallback.m_dAppPercentComplete + " -- " + pCallback.m_bUploading);
    }

    void OnRemoteStorageAppSyncStatusCheck(RemoteStorageAppSyncStatusCheck_t pCallback)
    {
        Console.WriteLine("[" + RemoteStorageAppSyncStatusCheck_t.k_iCallback + " - RemoteStorageAppSyncStatusCheck] - " + pCallback.m_nAppID + " -- " + pCallback.m_eResult);
    }

    void OnRemoteStorageConflictResolution(RemoteStorageConflictResolution_t pCallback)
    {
        Console.WriteLine("[" + RemoteStorageConflictResolution_t.k_iCallback + " - RemoteStorageConflictResolution] - " + pCallback.m_nAppID + " -- " + pCallback.m_eResult);
    }

    void OnRemoteStorageFileShareResult(RemoteStorageFileShareResult_t pCallback, bool bIOFailure)
    {
        Console.WriteLine("[" + RemoteStorageFileShareResult_t.k_iCallback + " - RemoteStorageFileShareResult] - " + pCallback.m_eResult + " -- " + pCallback.m_hFile);
        if (pCallback.m_eResult == EResult.k_EResultOK)
        {
            m_UGCHandle = pCallback.m_hFile;
        }
    }

    void OnRemoteStoragePublishFileResult(RemoteStoragePublishFileResult_t pCallback, bool bIOFailure)
    {
        Console.WriteLine("[" + RemoteStoragePublishFileResult_t.k_iCallback + " - RemoteStoragePublishFileResult] - " + pCallback.m_eResult + " -- " + pCallback.m_nPublishedFileId + " -- " + pCallback.m_bUserNeedsToAcceptWorkshopLegalAgreement);
        if (pCallback.m_eResult == EResult.k_EResultOK)
        {
            m_PublishedFileId = pCallback.m_nPublishedFileId;
        }
    }

    void OnRemoteStorageDeletePublishedFileResult(RemoteStorageDeletePublishedFileResult_t pCallback, bool bIOFailure)
    {
        Console.WriteLine("[" + RemoteStorageDeletePublishedFileResult_t.k_iCallback + " - RemoteStorageDeletePublishedFileResult] - " + pCallback.m_eResult + " -- " + pCallback.m_nPublishedFileId);
    }

    void OnRemoteStorageEnumerateUserPublishedFilesResult(RemoteStorageEnumerateUserPublishedFilesResult_t pCallback, bool bIOFailure)
    {
        Console.WriteLine("[" + RemoteStorageEnumerateUserPublishedFilesResult_t.k_iCallback + " - RemoteStorageEnumerateUserPublishedFilesResult] - " + pCallback.m_eResult + " -- " + pCallback.m_nResultsReturned + " -- " + pCallback.m_nTotalResultCount + " -- " + pCallback.m_rgPublishedFileId);
    }

    void OnRemoteStorageSubscribePublishedFileResult(RemoteStorageSubscribePublishedFileResult_t pCallback, bool bIOFailure)
    {
        Console.WriteLine("[" + RemoteStorageSubscribePublishedFileResult_t.k_iCallback + " - RemoteStorageSubscribePublishedFileResult] - " + pCallback.m_eResult + " -- " + pCallback.m_nPublishedFileId);
    }

    void OnRemoteStorageEnumerateUserSubscribedFilesResult(RemoteStorageEnumerateUserSubscribedFilesResult_t pCallback, bool bIOFailure)
    {
        Console.WriteLine("[" + RemoteStorageEnumerateUserSubscribedFilesResult_t.k_iCallback + " - RemoteStorageEnumerateUserSubscribedFilesResult] - " + pCallback.m_eResult + " -- " + pCallback.m_nResultsReturned + " -- " + pCallback.m_nTotalResultCount + " -- " + pCallback.m_rgPublishedFileId + " -- " + pCallback.m_rgRTimeSubscribed);
    }

    void OnRemoteStorageUnsubscribePublishedFileResult(RemoteStorageUnsubscribePublishedFileResult_t pCallback, bool bIOFailure)
    {
        Console.WriteLine("[" + RemoteStorageUnsubscribePublishedFileResult_t.k_iCallback + " - RemoteStorageUnsubscribePublishedFileResult] - " + pCallback.m_eResult + " -- " + pCallback.m_nPublishedFileId);
    }

    void OnRemoteStorageUpdatePublishedFileResult(RemoteStorageUpdatePublishedFileResult_t pCallback, bool bIOFailure)
    {
        Console.WriteLine("[" + RemoteStorageUpdatePublishedFileResult_t.k_iCallback + " - RemoteStorageUpdatePublishedFileResult] - " + pCallback.m_eResult + " -- " + pCallback.m_nPublishedFileId + " -- " + pCallback.m_bUserNeedsToAcceptWorkshopLegalAgreement);
    }

    void OnRemoteStorageDownloadUGCResult(RemoteStorageDownloadUGCResult_t pCallback, bool bIOFailure)
    {
        Console.WriteLine("[" + RemoteStorageDownloadUGCResult_t.k_iCallback + " - RemoteStorageDownloadUGCResult] - " + pCallback.m_eResult + " -- " + pCallback.m_hFile + " -- " + pCallback.m_nAppID + " -- " + pCallback.m_nSizeInBytes + " -- " + pCallback.m_pchFileName + " -- " + pCallback.m_ulSteamIDOwner);
    }

    void OnRemoteStorageGetPublishedFileDetailsResult(RemoteStorageGetPublishedFileDetailsResult_t pCallback, bool bIOFailure)
    {
        Console.WriteLine("[" + RemoteStorageGetPublishedFileDetailsResult_t.k_iCallback + " - RemoteStorageGetPublishedFileDetailsResult] - " + pCallback.m_eResult + " -- " + pCallback.m_nPublishedFileId + " -- " + pCallback.m_nCreatorAppID + " -- " + pCallback.m_nConsumerAppID + " -- " + pCallback.m_rgchTitle + " -- " + pCallback.m_rgchDescription + " -- " + pCallback.m_hFile + " -- " + pCallback.m_hPreviewFile + " -- " + pCallback.m_ulSteamIDOwner + " -- " + pCallback.m_rtimeCreated + " -- " + pCallback.m_rtimeUpdated + " -- " + pCallback.m_eVisibility + " -- " + pCallback.m_bBanned + " -- " + pCallback.m_rgchTags + " -- " + pCallback.m_bTagsTruncated + " -- " + pCallback.m_pchFileName + " -- " + pCallback.m_nFileSize + " -- " + pCallback.m_nPreviewFileSize + " -- " + pCallback.m_rgchURL + " -- " + pCallback.m_eFileType + " -- " + pCallback.m_bAcceptedForUse);
        if (pCallback.m_eResult == EResult.k_EResultOK)
        {
            m_UGCHandle = pCallback.m_hFile;
        }
    }

    void OnRemoteStorageEnumerateWorkshopFilesResult(RemoteStorageEnumerateWorkshopFilesResult_t pCallback, bool bIOFailure)
    {
        Console.WriteLine("[" + RemoteStorageEnumerateWorkshopFilesResult_t.k_iCallback + " - RemoteStorageEnumerateWorkshopFilesResult] - " + pCallback.m_eResult + " -- " + pCallback.m_nResultsReturned + " -- " + pCallback.m_nTotalResultCount + " -- " + pCallback.m_rgPublishedFileId + " -- " + pCallback.m_rgScore + " -- " + pCallback.m_nAppId + " -- " + pCallback.m_unStartIndex);
        for (int i = 0; i < pCallback.m_nResultsReturned; ++i)
        {
            Console.WriteLine(i + ": " + pCallback.m_rgPublishedFileId[i]);
        }

        if (pCallback.m_nResultsReturned >= 1)
        {
            m_PublishedFileId = pCallback.m_rgPublishedFileId[0];
        }
    }

    void OnRemoteStorageGetPublishedItemVoteDetailsResult(RemoteStorageGetPublishedItemVoteDetailsResult_t pCallback, bool bIOFailure)
    {
        Console.WriteLine("[" + RemoteStorageGetPublishedItemVoteDetailsResult_t.k_iCallback + " - RemoteStorageGetPublishedItemVoteDetailsResult_t] - " + pCallback.m_eResult + " -- " + pCallback.m_unPublishedFileId + " -- " + pCallback.m_nVotesFor + " -- " + pCallback.m_nVotesAgainst + " -- " + pCallback.m_nReports + " -- " + pCallback.m_fScore);
    }

    void OnRemoteStoragePublishedFileSubscribed(RemoteStoragePublishedFileSubscribed_t pCallback)
    {
        Console.WriteLine("[" + RemoteStoragePublishedFileSubscribed_t.k_iCallback + " - RemoteStoragePublishedFileSubscribed] - " + pCallback.m_nPublishedFileId + " -- " + pCallback.m_nAppID);
        if (pCallback.m_nAppID == SteamUtils.GetAppID())
        {
            //PublishedFileId_t[] tempArray = new PublishedFileId_t[1];
            //tempArray[0]=pCallback.m_nPublishedFileId;
            //SteamUGC.GetSubscribedItems(tempArray, (uint)tempArray.Length);

            OnItemSubscribeCheck?.Invoke();//YargisSteam.CheckSubscribedItems();  //Yargis.YargisGame.Instance.MultiplayerLevelList, Yargis.YargisGame.Instance.MultiplayerLevelList
        }
    }

    void OnRemoteStoragePublishedFileUnsubscribed(RemoteStoragePublishedFileUnsubscribed_t pCallback)
    {
        Console.WriteLine("[" + RemoteStoragePublishedFileUnsubscribed_t.k_iCallback + " - RemoteStoragePublishedFileUnsubscribed] - " + pCallback.m_nPublishedFileId + " -- " + pCallback.m_nAppID);
    }

    void OnRemoteStoragePublishedFileDeleted(RemoteStoragePublishedFileDeleted_t pCallback)
    {
        Console.WriteLine("[" + RemoteStoragePublishedFileDeleted_t.k_iCallback + " - RemoteStoragePublishedFileDeleted] - " + pCallback.m_nPublishedFileId + " -- " + pCallback.m_nAppID);
    }

    void OnRemoteStorageUpdateUserPublishedItemVoteResult(RemoteStorageUpdateUserPublishedItemVoteResult_t pCallback, bool bIOFailure)
    {
        Console.WriteLine("[" + RemoteStorageUpdateUserPublishedItemVoteResult_t.k_iCallback + " - RemoteStorageUpdateUserPublishedItemVoteResult] - " + pCallback.m_eResult + " -- " + pCallback.m_nPublishedFileId);
    }

    void OnRemoteStorageUserVoteDetails(RemoteStorageUserVoteDetails_t pCallback, bool bIOFailure)
    {
        Console.WriteLine("[" + RemoteStorageUserVoteDetails_t.k_iCallback + " - RemoteStorageUserVoteDetails] - " + pCallback.m_eResult + " -- " + pCallback.m_nPublishedFileId + " -- " + pCallback.m_eVote);
    }

    void OnRemoteStorageEnumerateUserSharedWorkshopFilesResult(RemoteStorageEnumerateUserSharedWorkshopFilesResult_t pCallback, bool bIOFailure)
    {
        Console.WriteLine("[" + RemoteStorageEnumerateUserSharedWorkshopFilesResult_t.k_iCallback + " - RemoteStorageEnumerateUserSharedWorkshopFilesResult] - " + pCallback.m_eResult + " -- " + pCallback.m_nResultsReturned + " -- " + pCallback.m_nTotalResultCount + " -- " + pCallback.m_rgPublishedFileId);
    }

    void OnRemoteStorageSetUserPublishedFileActionResult(RemoteStorageSetUserPublishedFileActionResult_t pCallback, bool bIOFailure)
    {
        Console.WriteLine("[" + RemoteStorageSetUserPublishedFileActionResult_t.k_iCallback + " - RemoteStorageSetUserPublishedFileActionResult] - " + pCallback.m_eResult + " -- " + pCallback.m_nPublishedFileId + " -- " + pCallback.m_eAction);
    }

    void OnRemoteStorageEnumeratePublishedFilesByUserActionResult(RemoteStorageEnumeratePublishedFilesByUserActionResult_t pCallback, bool bIOFailure)
    {
        Console.WriteLine("[" + RemoteStorageEnumeratePublishedFilesByUserActionResult_t.k_iCallback + " - RemoteStorageEnumeratePublishedFilesByUserActionResult] - " + pCallback.m_eResult + " -- " + pCallback.m_eAction + " -- " + pCallback.m_nResultsReturned + " -- " + pCallback.m_nTotalResultCount + " -- " + pCallback.m_rgPublishedFileId + " -- " + pCallback.m_rgRTimeUpdated);
    }

    void OnRemoteStoragePublishFileProgress(RemoteStoragePublishFileProgress_t pCallback)
    {
        Console.WriteLine("[" + RemoteStoragePublishFileProgress_t.k_iCallback + " - RemoteStoragePublishFileProgress] - " + pCallback.m_dPercentFile + " -- " + pCallback.m_bPreview);
    }

    void OnRemoteStoragePublishedFileUpdated(RemoteStoragePublishedFileUpdated_t pCallback)
    {
        Console.WriteLine("[" + RemoteStoragePublishedFileUpdated_t.k_iCallback + " - RemoteStoragePublishedFileUpdated] - " + pCallback.m_nPublishedFileId + " -- " + pCallback.m_nAppID + " -- " + pCallback.m_hFile);
    }
}
#endif