using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
namespace CollisionDetectionV3
{

    
    public class settingsPage
    {

        public Panel settingsPanel;
        private Form1 GameForm;
        game NewGame;

        private Label settingsLabel, difficultyLabel;
        private Button back, save;
        private ComboBox difficulty;

        //constructor 
        public settingsPage(Form1 gameForm,game _NewGame)
        {
            settingsPanel = new Panel
            {
                Location = new System.Drawing.Point(0, 0),
                Size = new System.Drawing.Size(700, 700),
                BackColor = Color.White
            };
            NewGame = _NewGame;
            GameForm = gameForm;
        }

        //keymap for key binds
        public Dictionary<string, Keys> KeyMap = new Dictionary<string, Keys>
        {
             { "MoveUp", Keys.W },
             { "MoveDown", Keys.S },
             { "MoveLeft", Keys.A },
             { "MoveRight", Keys.D },
             

        };


        public void generateSettingsPage()
        {
            settingsPanel.Controls.Clear();
            settingsPanel.Dock = DockStyle.Fill;

            // dynamically generates buttons that match to whatever i put in the dictionary
            int Offset = 150;

            settingsLabel = new Label
            {
                Text = "Settings",
                Location = new System.Drawing.Point(20, 20),
                Font = new System.Drawing.Font("Consolas", 26),
                AutoSize = true
            };
            settingsPanel.Controls.Add(settingsLabel);

            foreach (KeyValuePair<string, Keys> action in KeyMap)
            {
                Label ActionLabel = new Label
                {
                    Text = action.Key,
                    Location = new System.Drawing.Point(10, Offset),
                    AutoSize = true
                };

                Button KeyButton = new Button
                {
                    Text = action.Value.ToString(),
                    Location = new System.Drawing.Point(settingsPanel.Width - 130, Offset),
                    Tag = action.Key,
                    Size = new System.Drawing.Size(100, 30)
                };

                KeyButton.Click += KeyButton_Click;

                settingsPanel.Controls.Add(ActionLabel);
                settingsPanel.Controls.Add(KeyButton);

                Offset += 60;

            }

            back = new Button
            {
                Location = new Point(630, 10),
                Size = new Size(29, 29),
                Text = "❌",
                ForeColor = Color.White,
                BackColor = Color.Black
            };
            back.Click += GameForm.Menu_Click;
            settingsPanel.Controls.Add(back);

             save = new Button
             {
                Location = new Point(settingsPanel.Width-130, 360),
                Size = new Size(100, 30),
                Text = "save"
             };
            save.Click += Save_Click;
            settingsPanel.Controls.Add(save);

            difficultyLabel = new Label
            {
                Text = "Difficulty",
                Location = new Point(10, 100),
                AutoSize = true,
            };
            settingsPanel.Controls.Add(difficultyLabel);

            difficulty = new ComboBox
            {
                Location = new Point(settingsPanel.Width - 130, 120),
                Size = new Size(100, 50),
                Text = "difficulty",
                Font = new System.Drawing.Font("Consolas",12, System.Drawing.FontStyle.Bold),
            };
            difficulty.Items.Add("easy");
            difficulty.Items.Add("normal");
            difficulty.Items.Add("hard");
            difficulty.SelectedIndexChanged += Difficulty_SelectedIndexChanged;
            settingsPanel.Controls.Add(difficulty);

            settingsPanel.Resize += SettingsPanel_Resize;
        }

        private void SettingsPanel_Resize(object sender, EventArgs e)
        {

            // dynamically resizes the buttons and labels when the panel is resized
            int labelx = settingsPanel.Width / 2 - settingsLabel.Width / 2;
            int saveX = settingsPanel.Width - 100;
            settingsLabel.Location = new System.Drawing.Point(labelx, 20);
            
            difficulty.Location = new Point(settingsPanel.Width - 130, 120);

            foreach (Button b in settingsPanel.Controls.OfType<Button>())
            {
                if (b.Text == "❌")
                {
                    b.Location = new Point(settingsPanel.Width - 70, 10);
                }
                else if(b.Text == "save")
                {
                    b.Location = new Point(settingsPanel.Width - 130, 360);
                }
                else
                {
                    b.Location = new Point(settingsPanel.Width - 130, b.Location.Y);
                }
            }   


        }

        private void Difficulty_SelectedIndexChanged(object sender, EventArgs e)
        {
            // takes combo box input and changes the difficulty of the game
            if (sender is ComboBox cb)
            {
                string difficulty = cb.SelectedItem.ToString();
                switch (difficulty)
                {
                    case "easy":
                        NewGame.difficulty = "easy";
                        NewGame.changeDifficulty("easy");
                        break;
                    case "normal":
                        NewGame.difficulty = "normal";
                        NewGame.changeDifficulty("normal");

                        break;
                    case "hard":
                        NewGame.difficulty = "hard";
                        NewGame.changeDifficulty("hard");

                        break;
                }
            }

        }

        private void Save_Click(object sender, EventArgs e)
        {
            SaveBinds();
            MessageBox.Show("Keybinds saved");

        }

        private void KeyButton_Click(object sender, EventArgs e)
        {
            //this will log whatever the player presses to the button 
            Button btn = sender as Button;

            string action = btn.Tag.ToString();
            Keys currentkey = KeyMap[action];

            btn.Text = "press any key";
            btn.KeyDown += (s, KeyEventArgs) => // lambda func for this purpose
            {
              
                Keys newKey = KeyEventArgs.KeyCode;
                if(KeyMap.ContainsValue(newKey))
                {
                    // if the key is already mapped to something, we need to find it and change it to the new key
                    MessageBox.Show("This key is already mapped to something");
                    KeyMap[action] = currentkey;
                    btn.Text = currentkey.ToString();
                    return;

                }

                KeyMap[action] = newKey;
                btn.Text = newKey.ToString();

            };

        }

        public void loadKeyBinds() 
        {
            string FilePath = "keybinds.csv";   

            if (!File.Exists(FilePath)) //if the file does not exist, create it and write the default keybinds
            {
               using (StreamWriter sw = File.CreateText(FilePath))
               {
                    sw.WriteLine("MoveUp,W");
                    sw.WriteLine("MoveDown,S");
                    sw.WriteLine("MoveLeft,A");
                    sw.WriteLine("MoveRight,D");
                    
               }

            }

            KeyMap.Clear(); //clears the dictionary to write the new keybinds
            foreach (var line in File.ReadAllLines(FilePath))
            {
                string[] parts = line.Split(',');// splits the line into two parts
                if (parts.Length == 2 && Enum.TryParse(parts[1], out Keys key)) // checks if we have exactly 2 parts
                {                                                              // second part converts the string representation to the keys enum equivalent 
                    KeyMap[parts[0]] = key; //stores the key in the dictionary
                }
            }

        }
        public void SaveBinds()
        {
            string FilePath = "keybinds.csv";
            using (StreamWriter sw = new StreamWriter(FilePath)) //writes the new keybinds to the csv file 
            {
                foreach (KeyValuePair<string, Keys> action in KeyMap)
                {
                    sw.WriteLine($"{action.Key},{action.Value}");
                }
            }

        }

    }
}
