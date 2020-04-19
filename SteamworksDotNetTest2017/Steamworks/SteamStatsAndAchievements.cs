#if STEAMCLIENT
using System.Collections;
using System.ComponentModel;
using Steamworks;
using System;
using Yargis;


// This is a port of StatsAndAchievements.cpp from SpaceWar, the official Steamworks Example.
public class SteamStatsAndAchievements  {
	private enum Achievement : int {
		ACH_WIN_ONE_GAME,
		ACH_WIN_100_GAMES,
		ACH_HEAVY_FIRE,
		ACH_TRAVEL_FAR_ACCUM,
		ACH_TRAVEL_FAR_SINGLE,
	    };

    

	private Achievement_t[] m_Achievements = new Achievement_t[] {
		new Achievement_t(Achievement.ACH_WIN_ONE_GAME, "Winner", ""),
		new Achievement_t(Achievement.ACH_WIN_100_GAMES, "Champion", ""),
		new Achievement_t(Achievement.ACH_TRAVEL_FAR_ACCUM, "Interstellar", ""),
		new Achievement_t(Achievement.ACH_TRAVEL_FAR_SINGLE, "Orbiter", "")
	    };
    
	// Our GameID
	private CGameID m_GameID;

	// Did we get the stats from Steam?
	private bool m_bRequestedStats;
	private bool m_bStatsValid;

	// Should we store stats this frame?
	private bool m_bStoreStats;

	// Current Stat details
	private float m_flGameFeetTraveled;
	private float m_ulTickCountGameStart;
	private double m_flGameDurationSeconds;

	// Persisted Stat details
	private int m_nTotalGamesPlayed;
	private int m_nTotalNumWins;
	private int m_nTotalNumLosses;
	private float m_flTotalFeetTraveled;
	private float m_flMaxFeetTraveled;
	private float m_flAverageSpeed;

	protected Callback<UserStatsReceived_t> m_UserStatsReceived;
	protected Callback<UserStatsStored_t> m_UserStatsStored;
	protected Callback<UserAchievementStored_t> m_UserAchievementStored;


    /// <summary>
    /// Default constructor, initializes callbacks...
    /// </summary>
    public SteamStatsAndAchievements()
    {
        //SteamManager.Init();
        if (!SteamManager.Initialized)
            return;

        // Cache the GameID for use in the Callbacks
        m_GameID = new CGameID(SteamUtils.GetAppID());//SteamUtils.GetAppID());

		m_UserStatsReceived = Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);
		m_UserStatsStored = Callback<UserStatsStored_t>.Create(OnUserStatsStored);
		m_UserAchievementStored = Callback<UserAchievementStored_t>.Create(OnAchievementStored);

		// These need to be reset to get the stats upon an Assembly reload in the Editor.
		m_bRequestedStats = false;
		m_bStatsValid = false;
        Console.WriteLine("SteamStatsAndAchievements Initialized GameID: " + m_GameID);
	}

    public void requestCurrentStats()
    {
        if (!SteamManager.Initialized)
			return;

        //Only requests the stats 1 time.
        if (!m_bRequestedStats)
        {
            // Is Steam Loaded? if no, can't get stats, done
            if (!SteamManager.Initialized)
            {
                m_bRequestedStats = true;
                return;
            }

            // If yes, request our stats
            bool bSuccess = SteamUserStats.RequestCurrentStats();
        }
    }

    /// <summary>
    /// Reads and write stats... Syncs stats and rewards with Steam.
    /// </summary>
	public void UpdateStatsAndRewards() {
		if (!SteamManager.Initialized)
			return;

        //Only requests the stats 1 time.
		if (!m_bRequestedStats) {
			// Is Steam Loaded? if no, can't get stats, done
			if (!SteamManager.Initialized) {
				m_bRequestedStats = true;
				return;
			}
			
			// If yes, request our stats
			bool bSuccess = SteamUserStats.RequestCurrentStats();

			// This function should only return false if we weren't logged in, and we already checked that.
			// But handle it being false again anyway, just ask again later.
			m_bRequestedStats = bSuccess;
            Console.WriteLine("Steam stats requested. Success (m_bRequestedStats): " + m_bRequestedStats);
		}

        Console.WriteLine("m_bStatsValid: " + m_bStatsValid);
		if (!m_bStatsValid)
			return;

		// Get info from sources

		// Evaluate achievements for completion
		foreach (Achievement_t achievement in m_Achievements) 
        {
			if (achievement.m_bAchieved)
				continue;

			switch (achievement.m_eAchievementID) 
            {
				case Achievement.ACH_WIN_ONE_GAME:
					if (m_nTotalNumWins != 0) {
						UnlockAchievement(achievement);
					}
					break;
				case Achievement.ACH_WIN_100_GAMES:
					if (m_nTotalNumWins >= 100) {
						UnlockAchievement(achievement);
					}
					break;
				case Achievement.ACH_TRAVEL_FAR_ACCUM:
					if (m_flTotalFeetTraveled >= 5280) {
						UnlockAchievement(achievement);
					}
					break;
				case Achievement.ACH_TRAVEL_FAR_SINGLE:
					if (m_flGameFeetTraveled >= 500) {
						UnlockAchievement(achievement);
					}
					break;
			}
		}
	}


    /// <summary>
    /// Do not call this function too often. Normally and the end of the round.
    /// </summary>
    public void sendStats(Yargis.Player tempPlayer, Yargis.GameSession tempSession)
    {
        m_bStoreStats = true;
        storeStats(tempPlayer, tempSession);
    }

    /// <summary>
    /// Transmits all of the stats to Steam.
    /// </summary>
    private void storeStats(Yargis.Player tempPlayer, Yargis.GameSession tempSession)
    {
        if (!SteamManager.Initialized)
            return;

        //Store stats in the Steam database if necessary
        if (m_bStoreStats)
        {
            // already set any achievements in UnlockAchievement
            // set stats
            //tempSession

            SteamUserStats.SetStat("Money", (float)tempPlayer.Money);
            SteamUserStats.SetStat("GamesWon", tempPlayer.universalScores.RoundsWon);
            SteamUserStats.SetStat("GamesLost", tempPlayer.universalScores.RoundsLost);
            //SteamUserStats.SetStat("FeetTraveled", m_flTotalFeetTraveled);
            SteamUserStats.SetStat("Kills", tempPlayer.universalScores.Kills);
            SteamUserStats.SetStat("Deaths", tempPlayer.universalScores.Deaths);
            SteamUserStats.SetStat("K:D Ratio", tempPlayer.universalScores.KDRatio);
            SteamUserStats.SetStat("Experience", tempPlayer.ExpPoints);
            SteamUserStats.SetStat("Level", tempPlayer.Level);
            SteamUserStats.SetStat("Coins", tempPlayer.universalScores.Coins);
            SteamUserStats.SetStat("Cows Rescued", tempPlayer.universalScores.CowsRescued);
            SteamUserStats.SetStat("MultiplayerRounds", tempPlayer.universalScores.MultiPlayerRounds);
            SteamUserStats.SetStat("CTFRounds", tempPlayer.universalScores.CTFRounds);
            SteamUserStats.SetStat("CoOp Rounds", tempPlayer.universalScores.CoOpRounds);
            SteamUserStats.SetStat("Battle Rounds", tempPlayer.universalScores.BattleRounds);

            if (tempPlayer.CampaignData.IsLevelComplete(29)) 
            {
                SteamUserStats.SetAchievement("FinishSinglePlayer");
            }

            bool bSuccess = SteamUserStats.StoreStats();
            // If this failed, we never sent anything to the server, try
            // again later.
            m_bStoreStats = !bSuccess;
        }
       
    }

	//-----------------------------------------------------------------------------
	// Purpose: Accumulate distance traveled
	//-----------------------------------------------------------------------------
	public void AddDistanceTraveled(float flDistance) {
		m_flGameFeetTraveled += flDistance;
        m_flTotalFeetTraveled += flDistance;
	}
	
    ////-----------------------------------------------------------------------------
    //// Purpose: Game state has changed
    ////-----------------------------------------------------------------------------
    //public void OnGameStateChange(EClientGameState eNewState) {
    //    if (!m_bStatsValid)
    //        return;

    //    if (eNewState == EClientGameState.k_EClientGameActive) {
    //        // Reset per-game stats
    //        m_flGameFeetTraveled = 0;
    //        m_ulTickCountGameStart = Time.time;
    //    }
    //    else if (eNewState == EClientGameState.k_EClientGameWinner || eNewState == EClientGameState.k_EClientGameLoser) {
    //        if (eNewState == EClientGameState.k_EClientGameWinner) {
    //            m_nTotalNumWins++;
    //        }
    //        else {
    //            m_nTotalNumLosses++;
    //        }

    //        // Tally games
    //        m_nTotalGamesPlayed++;

    //        // Accumulate distances
    //        m_flTotalFeetTraveled += m_flGameFeetTraveled;

    //        // New max?
    //        if (m_flGameFeetTraveled > m_flMaxFeetTraveled)
    //            m_flMaxFeetTraveled = m_flGameFeetTraveled;

    //        // Calc game duration
    //        m_flGameDurationSeconds = Time.time - m_ulTickCountGameStart;

    //        // We want to update stats the next frame.
    //        m_bStoreStats = true;
    //    }
    //}

	//-----------------------------------------------------------------------------
	// Purpose: Unlock this achievement
	//-----------------------------------------------------------------------------
	private void UnlockAchievement(Achievement_t achievement) {
		achievement.m_bAchieved = true;

		// the icon may change once it's unlocked
		//achievement.m_iIconImage = 0;

		// mark it down
		SteamUserStats.SetAchievement(achievement.m_eAchievementID.ToString());

		// Store stats end of frame
		m_bStoreStats = true;
	}

    /// <summary>
    /// FOR TESTING PURPOSES ONLY!
    /// </summary>
    public void resetAllStats()
    {
            SteamUserStats.ResetAllStats(true);
            SteamUserStats.RequestCurrentStats();        
    }


	//-----------------------------------------------------------------------------
	// Purpose: We have stats data from Steam. It is authoritative, so update
	//			our data with those results now.
	//-----------------------------------------------------------------------------
	private void OnUserStatsReceived(UserStatsReceived_t pCallback) 
    {
		if (!SteamManager.Initialized)
			return;

		// we may get callbacks for other games' stats arriving, ignore them
		if ((ulong)m_GameID == pCallback.m_nGameID) 
        {
			if (EResult.k_EResultOK == pCallback.m_eResult) 
            {
				Console.WriteLine("Received stats and achievements from Steam\n");

				m_bStatsValid = true;

				// load achievements
				foreach (Achievement_t ach in m_Achievements) 
                {
					bool ret = SteamUserStats.GetAchievement(ach.m_eAchievementID.ToString(), out ach.m_bAchieved);
					if (ret) 
                    {
						ach.m_strName = SteamUserStats.GetAchievementDisplayAttribute(ach.m_eAchievementID.ToString(), "name");
						ach.m_strDescription = SteamUserStats.GetAchievementDisplayAttribute(ach.m_eAchievementID.ToString(), "desc");
					}
					else 
                    {
						Console.WriteLine("SteamUserStats.GetAchievement failed for Achievement " + ach.m_eAchievementID + "\nIs it registered in the Steam Partner site?");
					}
                }

                #region Load Stats
                //float tempfloat;    // These temps are used because  SteamUserStats.GetStat
                //int tempint;        // does not want to use temp.currentValue as an out parameter
                //Player tempPlayer = Global.ActivePlayer;

                ////Rounds Won
                //var temp = tempPlayer.universalScores.Objectives.GetObjectiveByName("Rounds Won");
                //if (temp != null)
                //{ 
                //    SteamUserStats.GetStat("GamesWon", out tempint);
                //    temp.CurrentValue = tempint;
                //}
                ////FeetTravelled
                ////var temp = tempPlayer.universalScores.Objectives.GetObjectiveByName("Feet Travelled");
                ////if (temp != null)
                ////{
                ////    SteamUserStats.GetStat("FeetTravelled", out tempfloat);
                ////    temp.CurrentValue = tempfloat;
                ////}
                ////Kills
                //temp = tempPlayer.universalScores.Objectives.GetObjectiveByName("Kills");
                //if (temp != null)
                //{
                //    SteamUserStats.GetStat("Kills", out tempint);
                //    temp.CurrentValue = tempint;
                //}
                ////Deaths
                //temp = tempPlayer.universalScores.Objectives.GetObjectiveByName("Deaths");
                //if (temp != null)
                //{
                //    SteamUserStats.GetStat("Deaths", out tempint);
                //    temp.CurrentValue = tempint;
                //}
                ////Money
                //if(SteamUserStats.GetStat("Money", out tempfloat))
                //    tempPlayer.Money = (decimal)tempfloat;
                ////Experience
                //if(SteamUserStats.GetStat("Experience", out tempint))
                //    tempPlayer.PermRewards.xp = tempint;
                ////Level
                //SteamUserStats.GetStat("Level", out tempPlayer.Level);
                ////Coins
                //temp = tempPlayer.universalScores.Objectives.GetObjectiveByName("Coin");
                //if (temp != null)
                //{
                //    SteamUserStats.GetStat("Coins", out tempint);
                //    temp.CurrentValue = tempint;
                //}
                ////Cows Rescued
                //temp = tempPlayer.universalScores.Objectives.GetObjectiveByName("Cattle Rescued");
                //if (temp != null)
                //{
                //    SteamUserStats.GetStat("Cows Rescued", out tempint);
                //    temp.CurrentValue = tempint;
                //}
                ////Multiplayer Rounds
                //temp = tempPlayer.universalScores.Objectives.GetObjectiveByName("Total Multiplayer Rounds");
                //if (temp != null)
                //{
                //    SteamUserStats.GetStat("MultiplayerRounds", out tempint);
                //    temp.CurrentValue = tempint;
                //}
                ////CTF Rounds
                //temp = tempPlayer.universalScores.Objectives.GetObjectiveByName("CTF Rounds");
                //if (temp != null)
                //{
                //    SteamUserStats.GetStat("CTFRounds", out tempint);
                //    temp.CurrentValue = tempint;
                //}
                ////Co-Op Rounds
                //temp = tempPlayer.universalScores.Objectives.GetObjectiveByName("Co-Op Rounds");
                //if (temp != null)
                //{
                //    SteamUserStats.GetStat("CoOp Rounds", out tempint);
                //    temp.CurrentValue = tempint;
                //}
                ////Battle Rounds
                //temp = tempPlayer.universalScores.Objectives.GetObjectiveByName("Battle Rounds");
                //if (temp != null)
                //{
                //    SteamUserStats.GetStat("Battle Rounds", out tempint);
                //    temp.CurrentValue = tempint;
                //}
                #endregion
            }
			else 
            {
				Console.WriteLine("RequestStats - failed, " + pCallback.m_eResult);
			}
		}
	}

	//-----------------------------------------------------------------------------
	// Purpose: Our stats data was stored!
	//-----------------------------------------------------------------------------
	private void OnUserStatsStored(UserStatsStored_t pCallback) {
		// we may get callbacks for other games' stats arriving, ignore them
		if ((ulong)m_GameID == pCallback.m_nGameID) {
			if (EResult.k_EResultOK == pCallback.m_eResult) {
				Console.WriteLine("StoreStats - success");
			}
			else if (EResult.k_EResultInvalidParam == pCallback.m_eResult) {
				// One or more stats we set broke a constraint. They've been reverted,
				// and we should re-iterate the values now to keep in sync.
				Console.WriteLine("StoreStats - some failed to validate");
				// Fake up a callback here so that we re-load the values.
				UserStatsReceived_t callback = new UserStatsReceived_t();
				callback.m_eResult = EResult.k_EResultOK;
				callback.m_nGameID = (ulong)m_GameID;
				OnUserStatsReceived(callback);
			}
			else {
				Console.WriteLine("StoreStats - failed, " + pCallback.m_eResult);
			}
		}
	}

	//-----------------------------------------------------------------------------
	// Purpose: An achievement was stored
	//-----------------------------------------------------------------------------
	private void OnAchievementStored(UserAchievementStored_t pCallback) {
		// We may get callbacks for other games' stats arriving, ignore them
		if ((ulong)m_GameID == pCallback.m_nGameID) {
			if (0 == pCallback.m_nMaxProgress) {
				Console.WriteLine("Achievement '" + pCallback.m_rgchAchievementName + "' unlocked!");
			}
			else {
				Console.WriteLine("Achievement '" + pCallback.m_rgchAchievementName + "' progress callback, (" + pCallback.m_nCurProgress + "," + pCallback.m_nMaxProgress + ")");
			}
		}
	}

    /// <summary>
    /// Purpose: Display the user's stats and achievements
    /// </summary>
    public void DisplayStats() {
        if (!SteamManager.Initialized) {
            Console.WriteLine("Steamworks not Initialized");
            return;
        }

        Console.WriteLine("********STATS********");
        Console.WriteLine("m_ulTickCountGameStart: " + m_ulTickCountGameStart);
        Console.WriteLine("m_flGameDurationSeconds: " + m_flGameDurationSeconds);
        Console.WriteLine("m_flGameFeetTraveled: " + m_flGameFeetTraveled);
        //GUILayout.Space(10);
        Console.WriteLine("NumGames: " + m_nTotalGamesPlayed);
        Console.WriteLine("NumWins: " + m_nTotalNumWins);
        Console.WriteLine("NumLosses: " + m_nTotalNumLosses);
        Console.WriteLine("FeetTraveled: " + m_flTotalFeetTraveled);
        Console.WriteLine("MaxFeetTraveled: " + m_flMaxFeetTraveled);
        Console.WriteLine("AverageSpeed: " + m_flAverageSpeed);

        //GUILayout.BeginArea(new Rect(Screen.width - 300, 0, 300, 800));
        foreach(Achievement_t ach in m_Achievements) {
            Console.WriteLine(ach.m_eAchievementID.ToString());
            Console.WriteLine(ach.m_strName + " - " + ach.m_strDescription);
            Console.WriteLine("Achieved: " + ach.m_bAchieved);
           // GUILayout.Space(20);
        }

        // FOR TESTING PURPOSES ONLY!
        //if (GUILayout.Button("RESET STATS AND ACHIEVEMENTS")) {
        //    SteamUserStats.ResetAllStats(true);
        //    SteamUserStats.RequestCurrentStats();
        //    OnGameStateChange(EClientGameState.k_EClientGameActive);
        //}
        //GUILayout.EndArea();

        Console.WriteLine("****************");
    }
	
	private class Achievement_t {
		public Achievement m_eAchievementID;
		public string m_strName;
		public string m_strDescription;
		public bool m_bAchieved;

		/// <summary>
		/// Creates an Achievement. You must also mirror the data provided here in https://partner.steamgames.com/apps/achievements/yourappid
		/// </summary>
		/// <param name="achievement">The "API Name Progress Stat" used to uniquely identify the achievement.</param>
		/// <param name="name">The "Display Name" that will be shown to players in game and on the Steam Community.</param>
		/// <param name="desc">The "Description" that will be shown to players in game and on the Steam Community.</param>
		public Achievement_t(Achievement achievementID, string name, string desc) {
			m_eAchievementID = achievementID;
			m_strName = name;
			m_strDescription = desc;
			m_bAchieved = false;
		}
	}
}
#endif