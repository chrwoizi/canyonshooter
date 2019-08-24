using System;
using System.Collections.Generic;
using System.Text;
using System.Collections.Specialized;
using System.Net;

namespace CanyonShooter.GameClasses.Scores
{
    public class OnlineScore
    {
        /// <summary>
        /// Post the score online.
        /// </summary>
        /// <param name="score">The game score object.</param>
        public static void PostScore(object game)
        {
            if (((ICanyonShooterGame)game).GameStates.Profil.CurrentProfil.OnlineHighscore != true) return;
            NameValueCollection postParam = new NameValueCollection();
            WebClient webClient = new WebClient();
            Byte[] webClientResponse;

            webClient.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 5.5; Windows NT 5.0)");
            webClient.Headers.Add("Cache-Control", "no-cache");

            postParam.Add("Nick", ((ICanyonShooterGame)game).GameStates.Profil.CurrentProfil.Playername.ToString());
            postParam.Add("Pass", "testpass");
            postParam.Add("Mail", "test@test.de");
            postParam.Add("scr8956tghzuw", ((ICanyonShooterGame)game).GameStates.Score.Highscore.ToString());

            try
            {
                webClientResponse = webClient.UploadValues("http://www.cellrays.de/csscore/add.php", "POST", postParam);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }
    }
}
