using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Xml.Schema;
using System.Drawing;

namespace CollisionDetectionV3
{
    public class leaderboard
    {
        private Form1 GameForm;
        game NewGame;

        private Label title;
        private Button back;
        public Panel leaderboardPanel;



        public leaderboard(Form1 gameForm, game _NewGame)
        {
            leaderboardPanel = new Panel
            {
                Location = new System.Drawing.Point(0, 0),
                Size = new System.Drawing.Size(700, 700),
                BackColor = System.Drawing.Color.White,
                Visible = false
            };
            gameForm.Controls.Add(leaderboardPanel);
            NewGame = _NewGame;
            GameForm = gameForm;
            
        }

        //creates a csv file to store the scores if it doesnt exits and then writes the username, difficulty and score to the file
        public void SaveScore()
        {
            string filePath = "leaderboard.csv";

            if (!File.Exists(filePath))
            {
                using (StreamWriter sw = new StreamWriter(filePath, false))
                {
                    sw.WriteLine($"{NewGame.username},{NewGame.difficulty},{NewGame.score}");

                }
            }
            else
            {
                using (StreamWriter sw = new StreamWriter(filePath, true))
                {
                    sw.WriteLine($"{NewGame.username},{NewGame.difficulty},{NewGame.score}");
                }
            }
            
        }

        //dynamically generates the leaderboard from the csv file
        public void drawLeaderboard()
        {
            int yoffset = 120;
            int count = 1;
            List<string> scores = LoadScore();
            leaderboardPanel.Resize += LeaderboardPanel_Resize;
            leaderboardPanel.Dock = DockStyle.Fill;

            title = new Label
            {
                Text = "Leaderboard",
                Location = new System.Drawing.Point(20, 30),
                Font = new System.Drawing.Font("Consolas", 30),
                AutoSize = true
            };
            leaderboardPanel.Controls.Add(title);

            foreach (string score in scores)
            {
                Label scoreLabel = new Label
                {
                    Text = $"{count}) {score}",
                    Location = new System.Drawing.Point(20, yoffset),
                    AutoSize = true,
                   // AutoScrollOffset = new System.Drawing.Point(50, yoffset)
                };
                leaderboardPanel.Controls.Add(scoreLabel);
                yoffset += 40;
                count += 1;
            }

             back = new Button
            {
                Location = new Point(630, 10),
                Size = new Size(29, 29),
                BackColor = Color.Black,
                ForeColor = Color.White,
                Text = "❌"
            };
            
            back.Click += GameForm.Menu_Click;
            leaderboardPanel.Controls.Add(back);
        }

        private void LeaderboardPanel_Resize(object sender, EventArgs e)
        {
            int titlex = leaderboardPanel.Width / 2 - title.Width / 2;
            title.Location = new Point(titlex , title.Location.Y);

            int backx = leaderboardPanel.Width - 50;
            back.Location = new Point(backx, back.Location.Y);
        }

        public List<string> LoadScore()
        {
           // parts of the string are username, difficulty, time
           List<string> scores = new List<string>();
           string filePath = "leaderboard.csv";
           string[] parts;
           List<string> lines;
           if (File.Exists(filePath))
           {
               using (StreamReader sr = new StreamReader(filePath)) // using a stream reader to read all the lines spilt them and then add them to a list to be returned 
               {
                   lines = File.ReadAllLines(filePath).ToList();
                   lines = sortScores(lines);
                
                   for(int i = 1; i < lines.Count; i++)
                   {
                       parts = lines[i].Split(',');

                       scores.Add($"{parts[0]} : {parts[1]} => {parts[2]}");  // will be displayed as username : difficulty => time
                   }

               }
           }

           if (scores.Count > 10)
           {
                scores.RemoveRange(10, scores.Count - 10);
           }
            return scores;



        }

       
        private List<string> sortScores(List<string> scores)
       {
            // this will use quicksort to sort the list of scores 
            if(scores.Count <= 1)
            {
                return scores;
            }

            string pivot = scores[scores.Count - 1]; // set pivot to end
            int pivotScore = convertMilliseconds(pivot.Split(',')[2]); // convert the time score to one integer

            List<string> less = new List<string>(); //sublists 
            List<string> greater = new List<string>();
            List<string> sorted = new List<string>();



            foreach (string score in scores.Take(scores.Count - 1)) //all elements except the pivot
            {

                int currentScore = convertMilliseconds(score.Split(',')[2]);

                if(currentScore < pivotScore) 
                {
                    less.Add(score);
                }
                else
                {
                    greater.Add(score);
                }

            }

            sorted.AddRange(sortScores(less)); //recursively adds sorts the sub lists
            sorted.Add(pivot);
            sorted.AddRange(sortScores(greater));

            return sorted;
        }

        private int convertMilliseconds(string time)
        {
            string[] parts = time.Split(':');
            int minutes = int.Parse(parts[0]) * 60 * 1000;
            int seconds = int.Parse(parts[1]) * 1000;
            int milliseconds = int.Parse(parts[2]);

            int total = minutes + seconds + milliseconds;
            return total;



        }
    }
}
