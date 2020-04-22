using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnUploadWorkshop_Click(object sender, EventArgs e)
        {

        }

        ///// <summary>
        ///// Transmits all of the stats to Steam. Do not call this function too often. Normally and the end of the round.
        ///// </summary>
        //public void storeStats(Yargis.Player tempPlayer, Yargis.GameSession tempSession)
        //{
        //    if (!SteamManager.Initialized)
        //        return;

        //    //Store stats in the Steam database if necessary
        //    //if (m_bStoreStats)
        //    {
        //        // already set any achievements in UnlockAchievement
        //        // set stats
        //        //tempSession

        //        SteamUserStats.SetStat("Money", (float)tempPlayer.Money);
        //        SteamUserStats.SetStat("GamesWon", tempPlayer.universalScores.RoundsWon);
        //        SteamUserStats.SetStat("GamesLost", tempPlayer.universalScores.RoundsLost);
        //        //SteamUserStats.SetStat("FeetTraveled", m_flTotalFeetTraveled);
        //        SteamUserStats.SetStat("Kills", tempPlayer.universalScores.Kills);
        //        SteamUserStats.SetStat("Deaths", tempPlayer.universalScores.Deaths);
        //        SteamUserStats.SetStat("K:D Ratio", tempPlayer.universalScores.KDRatio);
        //        SteamUserStats.SetStat("Experience", tempPlayer.ExpPoints);
        //        SteamUserStats.SetStat("Level", tempPlayer.Level);
        //        SteamUserStats.SetStat("Coins", tempPlayer.universalScores.Coins);
        //        SteamUserStats.SetStat("Cows Rescued", tempPlayer.universalScores.CowsRescued);
        //        SteamUserStats.SetStat("MultiplayerRounds", tempPlayer.universalScores.MultiPlayerRounds);
        //        SteamUserStats.SetStat("CTFRounds", tempPlayer.universalScores.CTFRounds);
        //        SteamUserStats.SetStat("CoOp Rounds", tempPlayer.universalScores.CoOpRounds);
        //        SteamUserStats.SetStat("Battle Rounds", tempPlayer.universalScores.BattleRounds);

        //        if (tempPlayer.CampaignData.IsLevelComplete(29))
        //        {
        //            SteamUserStats.SetAchievement("FinishSinglePlayer");
        //        }

        //        bool bSuccess = SteamUserStats.StoreStats();
        //        // If this failed, we never sent anything to the server, try
        //        // again later.
        //        //m_bStoreStats = !bSuccess;
        //    }


    }
}
