using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Audio;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Media;
using Steamworks;
//using System.Net;


namespace SteamworksDotNetTest2017
{
    static class Program
    {
        //GraphicsDeviceManager graphics;
        //SpriteBatch spriteBatch;

        //KeyboardState oldState = Keyboard.GetState();

        //public static Color backgroundColor = Color.CornflowerBlue;



        public static string debugString = "";

        //SpriteFont font;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (Initialize())
            {
                    finalize();
                    Application.Run(new Form1());
            }
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        private static bool Initialize()
        {
            // TODO: Add your initialization logic here
            SteamManager.Init();

#if  DEBUG
            return true;
#endif
            if (SteamManager.RestartNeeded(369040))  //this
            {
                //this.Exit();
                return false;
            }


            if (SteamManager.isValidApp(369040)) //TODO: ADD YOUR APP ID HERE<<<<<<<<<<<<<<<<<<
            {
                Console.WriteLine("isValidApp = true");
            }
            else
            {
                Console.WriteLine("isValidApp = false");
                Console.WriteLine("Please make sure Steam is running or Re-install Steam / Yargis. http://store.steampowered.com/about/");
                //DialogResult dialogResult = MessageBox.Show("Error with Steam. Try re-installing Steam / Yargis. Do you want to download now?", "Some Title", MessageBoxButtons.YesNo);
                //if (dialogResult == DialogResult.Yes)
                //{
                //    http://store.steampowered.com/about/
                //}
                
                //this.Exit();
                return false;
            }

           

            return true;
           // base.Initialize();
        }

        private static void finalize()
        {
            SteamManager.FinalInit();
            //Test
            //SteamAPI.InitSafe();
            if (SteamManager.Initialized)
            {
                Console.WriteLine("SteamUser: " + SteamFriends.GetPersonaName());
                Console.WriteLine("SteamUser: " + SteamUser.GetSteamID());
            }

            SteamManager.statsAndAchievements.UpdateStatsAndRewards();

           
        }
    }
}


