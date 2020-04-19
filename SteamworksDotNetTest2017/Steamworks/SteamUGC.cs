#if STEAMCLIENT
using System.Collections;
using Steamworks;
using System;
using System.Collections.Generic;
using Yargis;


/// <summary>
/// Used for Workshop item upload and download. https://partner.steamgames.com/documentation/ugc
/// </summary>
public class SteamUGCTest
{
    private UGCQueryHandle_t m_UGCQueryHandle;
    public PublishedFileId_t m_PublishedFileId;
    public UGCUpdateHandle_t m_UGCUpdateHandle;

    protected Callback<ItemInstalled_t> m_ItemInstalled;

    private CallResult<SteamUGCQueryCompleted_t> OnSteamUGCQueryCompletedCallResult;
    private CallResult<SteamUGCRequestUGCDetailsResult_t> OnSteamUGCRequestUGCDetailsResultCallResult;
    private CallResult<CreateItemResult_t> OnCreateItemResultCallResult;
    private CallResult<SubmitItemUpdateResult_t> OnSubmitItemUpdateResultCallResult;

    private CallResult<RemoteStorageSubscribePublishedFileResult_t> OnRemoteStorageSubscribePublishedFileResultCallResult;
    private CallResult<RemoteStorageUnsubscribePublishedFileResult_t> OnRemoteStorageUnsubscribePublishedFileResultCallResult;

    //private CallResult<SteamUGCRequestUGCDetailsResult_t> onDownloadItemCallResult;

    public SteamUGCTest()
    {
        m_ItemInstalled = Callback<ItemInstalled_t>.Create(OnItemInstalled);

        OnSteamUGCQueryCompletedCallResult = CallResult<SteamUGCQueryCompleted_t>.Create(OnSteamUGCQueryCompleted);
        OnSteamUGCRequestUGCDetailsResultCallResult = CallResult<SteamUGCRequestUGCDetailsResult_t>.Create(OnSteamUGCRequestUGCDetailsResult);
        OnCreateItemResultCallResult = CallResult<CreateItemResult_t>.Create(OnCreateItemResult);
        OnSubmitItemUpdateResultCallResult = CallResult<SubmitItemUpdateResult_t>.Create(OnSubmitItemUpdateResult);

        // These come from ISteamRemoteStorage but they are used here as well...
        OnRemoteStorageSubscribePublishedFileResultCallResult = CallResult<RemoteStorageSubscribePublishedFileResult_t>.Create(OnRemoteStorageSubscribePublishedFileResult);
        OnRemoteStorageUnsubscribePublishedFileResultCallResult = CallResult<RemoteStorageUnsubscribePublishedFileResult_t>.Create(OnRemoteStorageUnsubscribePublishedFileResult);

        //onDownloadItemCallResult = CallResult<DownloadItemResult_t>.Create(OnDownloadItemResult);

        //PublishedFileId_t temp = getID(SteamUtils.GetAppID());
    }



    /// <summary>
    /// Not sure if this is needed. Accessing the ISteamUGC API. The API must be accessed through the pointer that is returned from SteamUGC(). 
    /// </summary>
    public SteamAPICall_t getID(PublishedFileId_t workshopItemID)
    {
        return  SteamUGC.RequestUGCDetails(workshopItemID, 60);
    }


    /// <summary>
    /// Create an item in the Steam Workshop (you must then Register it with registerFileInfo). Returns a SteamAPICall_t handle. m_bUserNeedsToAcceptWorkshopLegalAgreement The m_bUserNeedsToAcceptWorkshopLegalAgreement variable should also be checked and if true, the user should be redirected to accept the legal agreement. See the Workshop Legal Agreement section for more details. https://partner.steamgames.com/?goto=%2Fdocumentation%2Fugc#Legal
    /// </summary>
    /// <returns>Returns a SteamAPICall_t handle</returns>
    public SteamAPICall_t CreateWorkshopItem()
    {
        //1. Request a handle 
        SteamAPICall_t handle;
        handle = SteamUGC.CreateItem(SteamUtils.GetAppID(), EWorkshopFileType.k_EWorkshopFileTypeCommunity);
        OnCreateItemResultCallResult.Set(handle); //not sure if this allows more than 1 upload at a time.
        //2. CreateItemResult_t will be called via a callback.
        Console.WriteLine("CreateWorkshopItem (SteamAPICall_t): " + handle);
        return handle;
    }


    /// <summary>
    /// Registers a file's info. (Must send file next with sendFileOrUpdate). This is also used when updating files.
    /// </summary>
    /// <param name="pubId">Obtained from CreateWorkshopItem</param>
    /// <param name="title">Limit the title length to 128 ASCII characters</param>
    /// <param name="description">Limit the description length to 8000 ASCII characters</param>
    /// <param name="visability">Valid visibility (ERemoteStoragePublishedFileVisibility) values include:        k_ERemoteStoragePublishedFileVisibilityPublic = 0        k_ERemoteStoragePublishedFileVisibilityFriendsOnly = 1        k_ERemoteStoragePublishedFileVisibilityPrivate = 2</param>
    /// <param name="tags">Utilizes a SteamParamStringArray_t which contains a pointer to an array of char * strings and a count of the number of strings in the array. </param>
    /// <param name="contentFolder">pszContentFolder is the absolute path to a local folder containing one or more files that represents the workshop item. For efficient upload and download, files should not be merged or compressed into single files (e.g. zip files). </param>
    /// <param name="imagePreviewFile">pszPreviewFile is the absolute path to a local preview image file for the workshop item. It must be under 1MB in size. The format should be one that both the web and the application (if necessary) can render. Suggested formats include JPG, PNG or GIF. </param>
    /// <returns></returns>
    public UGCUpdateHandle_t registerFileInfoOrUpdate(PublishedFileId_t pubId, string title, string description, ERemoteStoragePublishedFileVisibility visability, IList<string> tags, string contentFolder, string imagePreviewFile)
    {
        bool success=true;
        //2. An item update begins with a call to:
        UGCUpdateHandle_t handle;
        handle = SteamUGC.StartItemUpdate(SteamUtils.GetAppID(), pubId);

        //3. Using the UGCUpdateHandle_t that is returned from StartItemUpdate, calls can be made to update the Title, Description, Visibility, Tags, Item Content and Item Preview Image through the various SetItem methods. 
        success=SteamUGC.SetItemTitle(handle, title);
        Console.Write("Title: " + success);

        success = SteamUGC.SetItemDescription(handle, description);
        Console.Write(" -- Description: " + success);

        success = SteamUGC.SetItemVisibility(handle, visability);
        Console.Write(" -- Visability: " + success);

        success = SteamUGC.SetItemTags(handle, tags);
        Console.Write(" -- Tags: " + success);

        //TODO: We need to fix these paths>>>>>>>
        success = SteamUGC.SetItemContent(handle, contentFolder);
        Console.Write(" -- Content: " + success);

        success = SteamUGC.SetItemPreview(handle, imagePreviewFile);
        Console.WriteLine(" -- Preview: " + success);

        Console.WriteLine("registerFileInfo (UGCUpdateHandle_t): " + handle);

        m_UGCUpdateHandle = handle;

        return handle;
    }

    /// <summary>
    /// Sends or updates a file (must be registered first). 5. If desired, the progress of the upload can be tracked using EItemUpdateStatus GetItemUpdateProgress( UGCUpdateHandle_t handle, uint64 *punBytesProcessed, uint64* punBytesTotal). In the same way as Creating a Workshop Item, confirm the user has accepted the legal agreement. This is necessary in case where the user didn't initially create the item but is editing an existing item. 
    /// </summary>
    /// <param name="handle"></param>
    /// <param name="changeNote">Info about the change to the file for updates.</param>
    /// <returns></returns>
    public SteamAPICall_t sendFileOrUpdate(UGCUpdateHandle_t handle, string changeNote)
    {
        //4. Once the update calls have been completed, calling SteamAPICall_t SubmitItemUpdate( UGCUpdateHandle_t handle, const char *pchChangeNote ) will initiate the upload process to the Steam Workshop. 
        return SteamUGC.SubmitItemUpdate(handle, changeNote);
    }

    /// <summary>
    /// Gets the upload progress of a file.
    /// </summary>
    public EItemUpdateStatus getProgress(UGCUpdateHandle_t m_UGCUpdateHandle, out ulong BytesProcessed, out ulong BytesTotal)
    {
        EItemUpdateStatus ret = SteamUGC.GetItemUpdateProgress(m_UGCUpdateHandle, out BytesProcessed, out BytesTotal);
        Console.WriteLine("GetItemUpdateProgress (" +  m_UGCUpdateHandle + "): " + ret + " -- " + BytesProcessed + " -- " + BytesTotal);
        return ret;
    }

    public void queryWorkshop(string tag)
    {
        // UGCQueryHandle_t CreateQueryAllUGCRequest( EUGCQuery eQueryType, EUGCMatchingUGCType eMatchingeMatchingUGCTypeFileType, AppId_t nCreatorAppID, AppId_t nConsumerAppID, uint32 unPage )          //Use this method when querying for all matching UGC.

        m_UGCQueryHandle = SteamUGC.CreateQueryUserUGCRequest(SteamUser.GetSteamID().GetAccountID(), EUserUGCList.k_EUserUGCList_Subscribed, EUGCMatchingUGCType.k_EUGCMatchingUGCType_UsableInGame, EUserUGCListSortOrder.k_EUserUGCListSortOrder_CreationOrderDesc, AppId_t.Invalid, SteamUtils.GetAppID(), 1);
        SteamUGC.AddRequiredTag(m_UGCQueryHandle, tag);
        SteamAPICall_t handle = SteamUGC.SendQueryUGCRequest(m_UGCQueryHandle);
        OnSteamUGCQueryCompletedCallResult.Set(handle);
        Console.WriteLine("SteamUGC.SendQueryUGCRequest(" + m_UGCQueryHandle + ") : " + handle);
    }

    public void GetSubscribedItems()
    {
        uint number = SteamUGC.GetNumSubscribedItems();
        PublishedFileId_t[] items = new PublishedFileId_t[number];
        SteamUGC.GetSubscribedItems(items, number);
        Console.WriteLine("items[" + (number-1) + "]: " + items[number-1]);
    }

    public void RenderOnGUI()
    {
        //GUILayout.BeginArea(new Rect(Screen.width - 120, 0, 120, Screen.height));
        Console.WriteLine("Variables:");
        Console.WriteLine("m_UGCQueryHandle: " + m_UGCQueryHandle);
        Console.WriteLine("m_PublishedFileId: " + m_PublishedFileId);
        Console.WriteLine("m_UGCUpdateHandle: " + m_UGCUpdateHandle);
        //GUILayout.EndArea();

        //if (GUILayout.Button("CreateQueryUserUGCRequest(SteamUser.GetSteamID().GetAccountID(), EUserUGCList.k_EUserUGCList_Published, EUGCMatchingUGCType.k_EUGCMatchingUGCType_Screenshots, EUserUGCListSortOrder.k_EUserUGCListSortOrder_CreationOrderDesc, AppId_t.Invalid, SteamUtils.GetAppID(), 1)"))
        {
            m_UGCQueryHandle = SteamUGC.CreateQueryUserUGCRequest(SteamUser.GetSteamID().GetAccountID(), EUserUGCList.k_EUserUGCList_Published, EUGCMatchingUGCType.k_EUGCMatchingUGCType_Screenshots, EUserUGCListSortOrder.k_EUserUGCListSortOrder_CreationOrderDesc, AppId_t.Invalid, SteamUtils.GetAppID(), 1);
            Console.WriteLine("SteamUGC.CreateQueryUserUGCRequest(" + SteamUser.GetSteamID().GetAccountID() + ", EUserUGCList.k_EUserUGCList_Published, EUGCMatchingUGCType.k_EUGCMatchingUGCType_Screenshots, EUserUGCListSortOrder.k_EUserUGCListSortOrder_CreationOrderDesc, " + AppId_t.Invalid + ", " + SteamUtils.GetAppID() + ", 1) : " + m_UGCQueryHandle);
        }

        //if (GUILayout.Button("CreateQueryAllUGCRequest(EUGCQuery.k_EUGCQuery_RankedByPublicationDate, EUGCMatchingUGCType.k_EUGCMatchingUGCType_AllGuides, AppId_t.Invalid, SteamUtils.GetAppID(), 1)"))
        {
            m_UGCQueryHandle = SteamUGC.CreateQueryAllUGCRequest(EUGCQuery.k_EUGCQuery_RankedByPublicationDate, EUGCMatchingUGCType.k_EUGCMatchingUGCType_AllGuides, AppId_t.Invalid, SteamUtils.GetAppID(), 1);
            Console.WriteLine("SteamUGC.CreateQueryAllUGCRequest(EUGCQuery.k_EUGCQuery_RankedByPublicationDate, EUGCMatchingUGCType.k_EUGCMatchingUGCType_AllGuides, " + AppId_t.Invalid + ", " + SteamUtils.GetAppID() + ", 1) : " + m_UGCQueryHandle);
        }

        //if (GUILayout.Button("SendQueryUGCRequest(m_UGCQueryHandle)"))
        {
            SteamAPICall_t handle = SteamUGC.SendQueryUGCRequest(m_UGCQueryHandle);
            OnSteamUGCQueryCompletedCallResult.Set(handle);
            Console.WriteLine("SteamUGC.SendQueryUGCRequest(" + m_UGCQueryHandle + ") : " + handle);
        }

        //if (GUILayout.Button("GetQueryUGCResult(m_UGCQueryHandle, 0, out Details)"))
        {
            SteamUGCDetails_t Details;
            bool ret = SteamUGC.GetQueryUGCResult(m_UGCQueryHandle, 0, out Details);
            Console.WriteLine("SteamUGC.GetQueryUGCResult(" + m_UGCQueryHandle + ", 0, out Details) : " + ret);
            Console.WriteLine(Details.m_nPublishedFileId + " -- " + Details.m_eResult + " -- " + Details.m_eFileType + " -- " + Details.m_nCreatorAppID + " -- " + Details.m_nConsumerAppID + " -- " + Details.m_rgchTitle + " -- " + Details.m_rgchDescription + " -- " + Details.m_ulSteamIDOwner + " -- " + Details.m_rtimeCreated + " -- " + Details.m_rtimeUpdated + " -- " + Details.m_rtimeAddedToUserList + " -- " + Details.m_eVisibility + " -- " + Details.m_bBanned + " -- " + Details.m_bAcceptedForUse + " -- " + Details.m_bTagsTruncated + " -- " + Details.m_rgchTags + " -- " + Details.m_hFile + " -- " + Details.m_hPreviewFile + " -- " + Details.m_pchFileName + " -- " + Details.m_nFileSize + " -- " + Details.m_nPreviewFileSize + " -- " + Details.m_rgchURL + " -- " + Details.m_unVotesUp + " -- " + Details.m_unVotesDown + " -- " + Details.m_flScore + " -- " + Details.m_unNumChildren);
        }

        //if (GUILayout.Button("ReleaseQueryUGCRequest(m_UGCQueryHandle)"))
        {
            Console.WriteLine("SteamUGC.ReleaseQueryUGCRequest(" + m_UGCQueryHandle + ") : " + SteamUGC.ReleaseQueryUGCRequest(m_UGCQueryHandle));
        }

        //if (GUILayout.Button("AddRequiredTag(m_UGCQueryHandle, \"Co-op\")"))
        {
            Console.WriteLine("SteamUGC.AddRequiredTag(" + m_UGCQueryHandle + ", \"Co-op\") : " + SteamUGC.AddRequiredTag(m_UGCQueryHandle, "Co-op"));
        }

        //if (GUILayout.Button("AddExcludedTag(m_UGCQueryHandle, \"Co-op\")"))
        {
            Console.WriteLine("SteamUGC.AddExcludedTag(" + m_UGCQueryHandle + ", \"Co-op\") : " + SteamUGC.AddExcludedTag(m_UGCQueryHandle, "Co-op"));
        }

        //if (GUILayout.Button("SetReturnLongDescription(m_UGCQueryHandle, true)"))
        {
            Console.WriteLine("SteamUGC.SetReturnLongDescription(" + m_UGCQueryHandle + ", true) : " + SteamUGC.SetReturnLongDescription(m_UGCQueryHandle, true));
        }

       // if (GUILayout.Button("SetReturnTotalOnly(m_UGCQueryHandle, true)"))
        {
            Console.WriteLine("SteamUGC.SetReturnTotalOnly(" + m_UGCQueryHandle + ", true) : " + SteamUGC.SetReturnTotalOnly(m_UGCQueryHandle, true));
        }

        //if (GUILayout.Button("SetAllowCachedResponse(m_UGCQueryHandle, 5)"))
        {
            Console.WriteLine("SteamUGC.SetAllowCachedResponse(" + m_UGCQueryHandle + ", 5) : " + SteamUGC.SetAllowCachedResponse(m_UGCQueryHandle, 5));
        }

        //if (GUILayout.Button("SetCloudFileNameFilter(m_UGCQueryHandle, \"\")"))
        {
            Console.WriteLine("SteamUGC.SetCloudFileNameFilter(" + m_UGCQueryHandle + ", \"\") : " + SteamUGC.SetCloudFileNameFilter(m_UGCQueryHandle, ""));
        }

        //if (GUILayout.Button("SetMatchAnyTag(m_UGCQueryHandle, true)"))
        {
            Console.WriteLine("SteamUGC.SetMatchAnyTag(" + m_UGCQueryHandle + ", true) : " + SteamUGC.SetMatchAnyTag(m_UGCQueryHandle, true));
        }

        //if (GUILayout.Button("SetSearchText(m_UGCQueryHandle, \"AciD\")"))
        {
            Console.WriteLine("SteamUGC.SetSearchText(" + m_UGCQueryHandle + ", \"AciD\") : " + SteamUGC.SetSearchText(m_UGCQueryHandle, "AciD"));
        }

        //if (GUILayout.Button("SetRankedByTrendDays(m_UGCQueryHandle, 7)"))
        {
            Console.WriteLine("SteamUGC.SetRankedByTrendDays(" + m_UGCQueryHandle + ", 7) : " + SteamUGC.SetRankedByTrendDays(m_UGCQueryHandle, 7));
        }

        //if (GUILayout.Button("RequestUGCDetails(m_PublishedFileId, 5)"))
        {
            SteamAPICall_t handle = SteamUGC.RequestUGCDetails(m_PublishedFileId, 5);
            OnSteamUGCRequestUGCDetailsResultCallResult.Set(handle);
            Console.WriteLine("SteamUGC.RequestUGCDetails(m_PublishedFileId, 5) : " + handle);
        }

        //if (GUILayout.Button("CreateItem((AppId_t)480, EWorkshopFileType.k_EWorkshopFileTypeWebGuide)"))
        {
            SteamAPICall_t handle = SteamUGC.CreateItem((AppId_t)480, EWorkshopFileType.k_EWorkshopFileTypeWebGuide);
            OnCreateItemResultCallResult.Set(handle);
            Console.WriteLine("SteamUGC.CreateItem((AppId_t)480, EWorkshopFileType.k_EWorkshopFileTypeWebGuide) : " + handle);
        }

        //if (GUILayout.Button("StartItemUpdate((AppId_t)480, m_PublishedFileId)"))
        {
            m_UGCUpdateHandle = SteamUGC.StartItemUpdate((AppId_t)480, m_PublishedFileId);
            Console.WriteLine("SteamUGC.StartItemUpdate((AppId_t)480, " + m_PublishedFileId + ") : " + m_UGCUpdateHandle);
        }

       // if (GUILayout.Button("SetItemTitle(m_UGCUpdateHandle, \"This is a Test\")"))
        {
            bool ret = SteamUGC.SetItemTitle(m_UGCUpdateHandle, "This is a Test");
            Console.WriteLine("SteamUGC.SetItemTitle(" + m_UGCUpdateHandle + ", \"This is a Test\") : " + ret);
        }

        //if (GUILayout.Button("SetItemDescription(m_UGCUpdateHandle, \"This is the test description.\")"))
        {
            bool ret = SteamUGC.SetItemDescription(m_UGCUpdateHandle, "This is the test description.");
            Console.WriteLine("SteamUGC.SetItemDescription(" + m_UGCUpdateHandle + ", \"This is the test description.\") : " + ret);
        }

        //if (GUILayout.Button("SetItemVisibility(m_UGCUpdateHandle, ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPrivate)"))
        {
            bool ret = SteamUGC.SetItemVisibility(m_UGCUpdateHandle, ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPrivate);
            Console.WriteLine("SteamUGC.SetItemVisibility(" + m_UGCUpdateHandle + ", ERemoteStoragePublishedFileVisibility.k_ERemoteStoragePublishedFileVisibilityPrivate) : " + ret);
        }

        //if (GUILayout.Button("SetItemTags(m_UGCUpdateHandle, new string[] {\"Tag One\", \"Tag Two\", \"Test Tags\", \"Sorry\"})"))
        {
            bool ret = SteamUGC.SetItemTags(m_UGCUpdateHandle, new string[] { "Tag One", "Tag Two", "Test Tags", "Sorry" });
            Console.WriteLine("SteamUGC.SetItemTags(" + m_UGCUpdateHandle + ", new string[] {\"Tag One\", \"Tag Two\", \"Test Tags\", \"Sorry\"}) : " + ret);
        }

        //if (GUILayout.Button("SetItemContent(m_UGCUpdateHandle, Application.dataPath + \"/Scenes\")"))
        {
            bool ret = SteamUGC.SetItemContent(m_UGCUpdateHandle, AppDomain.CurrentDomain.BaseDirectory + "/Scenes"); // Should point to a folder containing the UGC Item
            Console.WriteLine("SteamUGC.SetItemContent(" + m_UGCUpdateHandle + ", Application.dataPath + \"/Scenes\") : " + ret);
        }

        //if (GUILayout.Button("SetItemPreview(m_UGCUpdateHandle, Application.dataPath + \"/controller.vdf\")"))
        {
            bool ret = SteamUGC.SetItemPreview(m_UGCUpdateHandle, AppDomain.CurrentDomain.BaseDirectory + "/controller.vdf"); // Should actually be a PNG/JPG Screenshot.
            Console.WriteLine("SteamUGC.SetItemPreview(" + m_UGCUpdateHandle + ", Application.dataPath + \"/controller.vdf\") : " + ret);
        }

        //if (GUILayout.Button("SubmitItemUpdate(m_UGCUpdateHandle, \"Test Changenote\")"))
        {
            SteamAPICall_t handle = SteamUGC.SubmitItemUpdate(m_UGCUpdateHandle, "Test Changenote");
            OnSubmitItemUpdateResultCallResult.Set(handle);
            Console.WriteLine("SteamUGC.SubmitItemUpdate(" + m_UGCUpdateHandle + ", \"Test Changenote\") : " + handle);
        }

        {
            ulong BytesProcessed;
            ulong BytesTotal;
            EItemUpdateStatus ret = SteamUGC.GetItemUpdateProgress(m_UGCUpdateHandle, out BytesProcessed, out BytesTotal);
            Console.WriteLine("GetItemUpdateProgress(m_UGCUpdateHandle, out BytesProcessed, out BytesTotal) : " + ret + " -- " + BytesProcessed + " -- " + BytesTotal);
        }

        //if (GUILayout.Button("SubscribeItem((PublishedFileId_t)113142309)"))
        {
            SteamAPICall_t handle = SteamUGC.SubscribeItem((PublishedFileId_t)113142309); // http://steamcommunity.com/sharedfiles/filedetails/?id=113142309
            OnRemoteStorageSubscribePublishedFileResultCallResult.Set(handle);
            Console.WriteLine("SteamUGC. : " + handle);
        }

        //if (GUILayout.Button("UnsubscribeItem((PublishedFileId_t)113142309)"))
        {
            SteamAPICall_t handle = SteamUGC.UnsubscribeItem((PublishedFileId_t)113142309); // http://steamcommunity.com/sharedfiles/filedetails/?id=113142309
            OnRemoteStorageUnsubscribePublishedFileResultCallResult.Set(handle);
            Console.WriteLine("SteamUGC. : " + handle);
        }

        Console.WriteLine("GetNumSubscribedItems() : " + SteamUGC.GetNumSubscribedItems());

        //if (GUILayout.Button("GetSubscribedItems(PublishedFileID, (uint)PublishedFileID.Length)"))
        {
            PublishedFileId_t[] PublishedFileID = new PublishedFileId_t[1];
            uint ret = SteamUGC.GetSubscribedItems(PublishedFileID, (uint)PublishedFileID.Length);
            m_PublishedFileId = PublishedFileID[0];
            Console.WriteLine("SteamUGC.GetSubscribedItems(" + PublishedFileID + ", (uint)PublishedFileID.Length) : " + ret + " -- " + PublishedFileID[0]);
        }

        {
            ulong SizeOnDisk;
            string Folder;
            uint LegacyItem; //bool LegacyItem;
            bool ret = SteamUGC.GetItemInstallInfo(m_PublishedFileId, out SizeOnDisk, out Folder, 1024, out LegacyItem);
            Console.WriteLine("GetItemInstallInfo(m_PublishedFileId, out SizeOnDisk, out Folder, 1024) : " + ret + " -- " + SizeOnDisk + " -- " + Folder + " -- " + LegacyItem);
        }

        {
            bool NeedsUpdate;
            bool IsDownloading;
            ulong BytesDownloaded;
            ulong BytesTotal;  //bool ret = SteamUGC.GetItemUpdateInfo(m_PublishedFileId, out NeedsUpdate, out IsDownloading, out BytesDownloaded, out BytesTotal);
            bool ret = SteamUGC.GetItemDownloadInfo(m_PublishedFileId, out BytesDownloaded, out BytesTotal); //, out BytesDownloaded, out BytesTotal);
            Console.WriteLine("GetItemUpdateInfo(m_PublishedFileId, out NeedsUpdate, out IsDownloading, out BytesDownloaded, out BytesTotal) : " + ret + " -- " + " -- " + BytesDownloaded + " -- " + BytesTotal);
            //Console.WriteLine("GetItemUpdateInfo(m_PublishedFileId, out NeedsUpdate, out IsDownloading, out BytesDownloaded, out BytesTotal) : " + ret + " -- " + NeedsUpdate + " -- " + IsDownloading + " -- " + BytesDownloaded + " -- " + BytesTotal);
        }
    }

    void OnSteamUGCQueryCompleted(SteamUGCQueryCompleted_t pCallback, bool bIOFailure)
    {
        Console.WriteLine("[" + SteamUGCQueryCompleted_t.k_iCallback + " - SteamUGCQueryCompleted_t] - " + pCallback.m_handle + " -- " + pCallback.m_eResult + " -- " + pCallback.m_unNumResultsReturned + " -- " + pCallback.m_unTotalMatchingResults + " -- " + pCallback.m_bCachedData);
        SteamUGCDetails_t pDetails;
        SteamUGC.GetQueryUGCResult(pCallback.m_handle, 17, out pDetails);
        Console.WriteLine("pDetails.m_pchFileName (" + pDetails.m_nPublishedFileId + "): " + pDetails.m_rgchTitle + " -- " + pDetails.m_rgchDescription);
        //Console.WriteLine("URL: " +pDetails.m_rgchURL);
    }

    void OnSteamUGCRequestUGCDetailsResult(SteamUGCRequestUGCDetailsResult_t pCallback, bool bIOFailure)
    {
        Console.WriteLine("[" + SteamUGCRequestUGCDetailsResult_t.k_iCallback + " - SteamUGCRequestUGCDetailsResult_t] - " + pCallback.m_details + " -- " + pCallback.m_bCachedData);
        Console.WriteLine(pCallback.m_details.m_nPublishedFileId + " -- " + pCallback.m_details.m_eResult + " -- " + pCallback.m_details.m_eFileType + " -- " + pCallback.m_details.m_nCreatorAppID + " -- " + pCallback.m_details.m_nConsumerAppID + " -- " + pCallback.m_details.m_rgchTitle + " -- " + pCallback.m_details.m_rgchDescription + " -- " + pCallback.m_details.m_ulSteamIDOwner + " -- " + pCallback.m_details.m_rtimeCreated + " -- " + pCallback.m_details.m_rtimeUpdated + " -- " + pCallback.m_details.m_rtimeAddedToUserList + " -- " + pCallback.m_details.m_eVisibility + " -- " + pCallback.m_details.m_bBanned + " -- " + pCallback.m_details.m_bAcceptedForUse + " -- " + pCallback.m_details.m_bTagsTruncated + " -- " + pCallback.m_details.m_rgchTags + " -- " + pCallback.m_details.m_hFile + " -- " + pCallback.m_details.m_hPreviewFile + " -- " + pCallback.m_details.m_pchFileName + " -- " + pCallback.m_details.m_nFileSize + " -- " + pCallback.m_details.m_nPreviewFileSize + " -- " + pCallback.m_details.m_rgchURL + " -- " + pCallback.m_details.m_unVotesUp + " -- " + pCallback.m_details.m_unVotesDown + " -- " + pCallback.m_details.m_flScore + " -- " + pCallback.m_details.m_unNumChildren);
    }

    /// <summary>
    /// Invoked when a new workshop item is created.
    /// </summary>
    void OnCreateItemResult(CreateItemResult_t pCallback, bool bIOFailure)
    {
        //3. When the CallResult handler is executed, read the PublishedFileId_t value and store for future updates to the workshop item (e.g. in a project file associated with the creation tool). 
        //4. The m_bUserNeedsToAcceptWorkshopLegalAgreement variable should also be checked and if true, the user should be redirected to accept the legal agreement. See the Workshop Legal Agreement section for more details.
        //ADD TO FORM:::: By submitting this item, you agree to the workshop terms of service (include link https://partner.steamgames.com/documentation/ugc#Legal)    

        //CreateItemResult_t.m_eResult may return any of the defined EResult's, however the following should be handled:
        //k_EResultInsufficientPrivilege - The user creating the item is currently banned in the community.
        //k_EResultTimeout - The operation took longer than expected, have the user retry the create process.
        // k_EResultNotLoggedOn - The user is not currently logged into Steam.

        Console.WriteLine("[" + CreateItemResult_t.k_iCallback + " - CreateItemResult_t] - " + pCallback.m_eResult + " -- " + pCallback.m_nPublishedFileId + " -- " + pCallback.m_bUserNeedsToAcceptWorkshopLegalAgreement);

        m_PublishedFileId = pCallback.m_nPublishedFileId;

    }

    void OnSubmitItemUpdateResult(SubmitItemUpdateResult_t pCallback, bool bIOFailure)
    {
        Console.WriteLine("[" + SubmitItemUpdateResult_t.k_iCallback + " - SubmitItemUpdateResult_t] - " + pCallback.m_eResult + " -- " + pCallback.m_bUserNeedsToAcceptWorkshopLegalAgreement);
    }

    /// <summary>
    /// Notification when a Workshop Item is Installed or Updated. If the return value is true then register and wait for the Callback DownloadItemResult_t before calling GetItemInstallInfo() or accessing the workshop item on disk.
    /// </summary>
    void OnItemInstalled(ItemInstalled_t pCallback)
    {
        Console.WriteLine("[" + ItemInstalled_t.k_iCallback + " - ItemInstalled_t] - " + pCallback.m_unAppID + " -- " + pCallback.m_nPublishedFileId);

        if (pCallback.m_unAppID == SteamUtils.GetAppID())
        {
            ulong sizeOnDisk;
            string folder;
            uint folderSize = 260;
            uint timeStamp;
            SteamUGC.GetItemInstallInfo(pCallback.m_nPublishedFileId, out sizeOnDisk, out folder, folderSize, out timeStamp);
            Console.WriteLine("GetItemInstallInfo: " + sizeOnDisk + " -- " + folder + " -- " + folderSize + " -- " + new DateTime(timeStamp));
            //NOT IMPLEMENTED OR NEEDED RIGHT NOW: SteamUGC.DownloadItem(pCallback.m_nPublishedFileId, true);

            YargisSteam.CheckSubscribedItems(YargisGame.Instance.MultiplayerLevelList, YargisGame.Instance.MultiplayerLevelList);
        }
    }

    //void OnDownloadItemResult(DownloadItemResult_t DownloadItemResult_t)
    //{

    //}

    // These are from ISteamRemoteStorage, but also used here.
    void OnRemoteStorageSubscribePublishedFileResult(RemoteStorageSubscribePublishedFileResult_t pCallback, bool bIOFailure)
    {
        Console.WriteLine("[" + RemoteStorageSubscribePublishedFileResult_t.k_iCallback + " - RemoteStorageSubscribePublishedFileResult] - " + pCallback.m_eResult + " -- " + pCallback.m_nPublishedFileId);
        m_PublishedFileId = pCallback.m_nPublishedFileId;
    }

    void OnRemoteStorageUnsubscribePublishedFileResult(RemoteStorageUnsubscribePublishedFileResult_t pCallback, bool bIOFailure)
    {
        Console.WriteLine("[" + RemoteStorageUnsubscribePublishedFileResult_t.k_iCallback + " - RemoteStorageUnsubscribePublishedFileResult] - " + pCallback.m_eResult + " -- " + pCallback.m_nPublishedFileId);
        m_PublishedFileId = pCallback.m_nPublishedFileId;
    }
}
#endif