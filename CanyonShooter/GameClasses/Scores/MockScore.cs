using System;
using System.Collections.Generic;

namespace CanyonShooter.GameClasses.Scores
{
    class MockScore
    {
        public MockScore(ICanyonShooterGame game)
        {
            Highscores highscore = new Highscores(game);
            string[] names = { "SMartin" , "SSimon", "SPaul", "SGabi", "SPhhillip", "SDavid", "SSabrina", "SAnna", "SAnton", "SHarald","SGiesela","SJulia"};
            Random random = new Random();
            for (int diff=0; diff<3; diff++)
            {
                List<Score> list = new List<Score>();
                highscore.highscores.Add(list);
                for (int i = 0; i < 10; i++)
                {
                    int points = random.Next(10,1500);
                    Score score = new Score();
                    score.Highscore = points;
                    score.Playername = names[points % names.Length];
                    highscore.AddScore(score, game, diff, false);
                }
            }
            highscore.SaveToFile();
        }
    }
}
