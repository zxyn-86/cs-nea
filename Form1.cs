using NEA_zain_uddin.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CollisionDetectionV3
{
    public partial class Form1 : Form
    {
        private Random random = new Random();
        //game constants and variables
        private const int CellSize = 50; // Size of each cell
        private const int GridSize = 10; // Number of cells in a row and column
        private int count;

        //ui components---------------------------------------------------------------------------------------------
        private Panel menu, winner, loss, pausePanel;
        public TextBox username;

        //labels on the screen
        private Label KEY, _isSpeedOn, _isGhostOn, _isFreezeOn, timer;

        //powerups
        Rectangle isSpeedOn, isFreezeOn, isGhostOn;
        Color speedcolor = Color.Red;
        Color ghostcolor = Color.Red;
        Color freezecolor = Color.Red;

        //buttons to add to panels
        Button pause;
        Button resume;
        Button menuButton;
        private Button SaveUserName;

        settingsPage settingsPage;
        leaderboard leaderboard;
        //--------------------------------------------------------------------------------------------------------
        
        //game objects and variables------------------------------------------------------------------------------
        private game NewGame;
        private maze Maze;
        private pathfinding pathfind;
        private bool isColliding;
        private Cell playerCell;
        private float enemySpeed = 2;
        //--------------------------------------------------------------------------------------------------------

        //timers--------------------------------------------------------------------------------------------------
        private Timer gametimer;
        Stopwatch scoreWatch;


        public Form1()
        {
            InitializeComponent();
           /* this.BackColor = Color.White;
            
            NewGame = new game("normal", this);
            leaderboard = new leaderboard(this, NewGame);
            settingsPage = new settingsPage(this, NewGame);
            settingsPage.loadKeyBinds();

            //this.Size = new Size(NewGame.Maze.GridSize * NewGame.Maze.CellSize + 200, NewGame.Maze.GridSize * NewGame.Maze.CellSize + 50);
            this.Size = new Size(700, 700);
            this.Text = "Maze with DFS and Collision Detection";
            this.StartPosition = FormStartPosition.CenterScreen;


            NewGame.start();

            drawability(CreateGraphics());
            initialiseMenu();

            this.DoubleBuffered = true; // Enable double-buffering
            
            // Enable key events
            this.KeyDown += Form1_KeyDown;
            this.Resize += Form1_Resize;
            gametimer = new Timer();   // adding a timer for the game loop
            gametimer.Interval = 8;
            
            scoreWatch = new Stopwatch();

            //Task.Run(() => GameProcessing());


           */ 
            InitializeUI();
            InitializeGame();

        }
        private void InitializeUI()
        {
            this.BackColor = Color.White;
            this.Size = new Size(700, 700);
            this.Text = "Maze with DFS and Collision Detection";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.DoubleBuffered = true;
        }
        private void InitializeGame()
        {
            
            NewGame = new game("normal", this);
            leaderboard = new leaderboard(this, NewGame);
            settingsPage = new settingsPage(this, NewGame);
            settingsPage.loadKeyBinds();

            drawability(CreateGraphics());
            initialiseMenu();

            gametimer = new Timer { Interval = 8 };
            scoreWatch = new Stopwatch();

            this.KeyDown += Form1_KeyDown;
            this.Resize += Form1_Resize;

            NewGame.start();
        }


        //  general game  components and event handlers-------------------------------------------------------------------------------------------------------------------
       

        private void changeVisibility()
        {
            menu.Visible = false;
            _isFreezeOn.Visible = false;
            _isGhostOn.Visible = false;
            _isSpeedOn.Visible = false;
            KEY.Visible = false;
            pause.Visible = false;
            settingsPage.settingsPanel.Visible = false;
            leaderboard.leaderboardPanel.Visible = false;

            if (loss != null)
            {
                loss.Visible = false;
            }
            if (winner != null)
            {
                winner.Visible = false;
            }
            if (timer != null)
            {
                timer.Visible = false;
            }
            if (pausePanel != null)
            {
                pausePanel.Visible = false;
            }
        }




        //resize event so main form format stays consistent
        private void Form1_Resize(object sender, EventArgs e)
        {
            pause.Location = new Point(this.Width - 70, 10);
        }

        //game loop
        private void gameTick(object sender, EventArgs e)
        {
            //NewGame.changeDifficulty();
            TimeSpan elapsed = scoreWatch.Elapsed;
            timer.Text = string.Format("Time: {0:00}:{1:00}:{2:00}", elapsed.Minutes, elapsed.Seconds, elapsed.Milliseconds);

            Rectangle newPlayerPosition = NewGame.Player.PlayerRect;
            //pathfind.path = null;
            


            if (NewGame.UpKey)
            {
                
                newPlayerPosition.Y -= (int) NewGame.Player.speed;
            }
            if (NewGame.DownKey)
            {
                newPlayerPosition.Y += (int)NewGame.Player.speed;
            }
            if (NewGame.LeftKey)
            {
                newPlayerPosition.X -= (int)NewGame.Player.speed;
            }
            if (NewGame.RightKey)
            {
                newPlayerPosition.X += (int)NewGame.Player.speed;
            }
           

            isColliding = NewGame.Maze.IsCollidingWithAnyCell(newPlayerPosition);

            // Check for collisions with walls
             if (!isColliding)
             {
                 // If no collision, update the player's position
                 NewGame.Player.PlayerRect = newPlayerPosition;

                 // Invalidate only the area that needs to be redrawn (around the player)

                 this.Invalidate();
             }
            
           /* if (!isColliding)
            {
                
                NewGame.Player.PlayerRect = newPlayerPosition;
                Rectangle oldPosition = NewGame.previousPlayerPosition;
                // Calculate the area to invalidate (old and new player positions)
                Rectangle invalidateArea = Rectangle.Union(oldPosition, NewGame.Player.PlayerRect);
                this.Invalidate(invalidateArea);

                // Update the previous player position
                NewGame.previousPlayerPosition = newPlayerPosition;
            }
           */
            Task.Run(() => GameProcessing());
            
            /*
            foreach (enemy enemy in NewGame.Enemies.enemyList)
            {
                if (NewGame.powers.isFreezeActive) 
                {
                    continue;
                   
                }
                playerCell = NewGame.Maze.cells.First(c => c.Bounds.Contains(NewGame.Player.PlayerRect.X, NewGame.Player.PlayerRect.Y)); //finds current player cell
                enemy.currentCell = NewGame.Maze.cells.First(c => c.Bounds.Contains(enemy.currentPos.X, enemy.currentPos.Y)); //finds current enemy cell

                //if (pathfind.DistanceBetween(enemy.currentCell, playerCell) >= 5)
                //{
                //    pathfind.Patrol(NewGame.Maze,enemy,enemySpeed);
                //}

                if (pathfind.DistanceBetween(enemy.currentCell, playerCell) > 0) //temporary while i sort out enemy stuff
                {  

                    pathfind.PreciseTrack(playerCell,enemy,pathfind,enemySpeed, NewGame.Maze);
                }

                if (NewGame.Player.PlayerRect.IntersectsWith(enemy.currentPos) && !NewGame.powers.isGhostActive)
                {
                    NewGame.Caught = true;
                }
                else if((enemy.currentPos.IntersectsWith(NewGame.Player.PlayerRect) && !NewGame.powers.isGhostActive))
                {
                    NewGame.Caught = true;

                }
                else { NewGame.Caught = false; }
            }

            if (NewGame.Player.PlayerRect.IntersectsWith(NewGame.key.Keyrect))
            {
                NewGame.IsKeyCollected = true;
                NewGame.key.pigment = Color.White;

            }
            NewGame.powers.CheckForPowerup();
            abilityStatus();
            NewGame.handleGameOver();
            if(NewGame.gamestate.ToLower() == "won")
            {
                gametimer.Stop();

                initialiseWin();
                //NewGame = new game(Maze, pathfind, Enemies, this);

            }
            if (NewGame.gamestate.ToLower() == "lost")
            {
                gametimer.Stop();

                initialiseLoss();
                //NewGame = new game(Maze, pathfind, Enemies, this);


            }
            //else { NewGame.gamestate = "playing"; }
            */


        }

        private async Task GameProcessing()
        {
            await Task.Run(() =>
            {
                if (count % 2 == 0)
                {
                    foreach (enemy enemy in NewGame.Enemies.enemyList)
                    {
                        if (NewGame.powers.isFreezeActive)
                        {
                            continue;

                        }
                        playerCell = NewGame.Maze.cells.First(c => c.Bounds.Contains(NewGame.Player.PlayerRect.X, NewGame.Player.PlayerRect.Y)); //finds current player cell

                        enemy.currentCell = NewGame.Maze.cells.First(c => c.Bounds.Contains(enemy.currentPos.X, enemy.currentPos.Y)); //finds current enemy cell

                        if (NewGame.pathfind.DistanceBetween(enemy.currentCell, playerCell) >= 5)
                        {
                            NewGame.pathfind.Patrol(NewGame.Maze, enemy, enemySpeed);
                        }

                        else if (NewGame.pathfind.DistanceBetween(enemy.currentCell, playerCell) < 5) 
                        {

                            NewGame.pathfind.PreciseTrack(playerCell, enemy, NewGame.Enemies.enemySpeed, NewGame.Maze);
                        }

                        if (NewGame.Player.PlayerRect.IntersectsWith(enemy.currentPos) && !NewGame.powers.isGhostActive)
                        {
                            NewGame.Caught = true;
                        }
                        else if ((enemy.currentPos.IntersectsWith(NewGame.Player.PlayerRect) && !NewGame.powers.isGhostActive))
                        {
                            NewGame.Caught = true;

                        }
                        else { NewGame.Caught = false; }
                    }

                    if (NewGame.Player.PlayerRect.IntersectsWith(NewGame.key.Keyrect))
                    {
                        NewGame.IsKeyCollected = true;
                        NewGame.key.pigment = Color.White;

                    }


                    Invoke((Action)(() =>
                    {

                        NewGame.handleGameOver();
                        NewGame.powers.CheckForPowerup();
                        abilityStatus();

                        if (NewGame.gamestate.ToLower() == "won")
                        {

                            initialiseWin();

                            gametimer.Stop();

                        }
                        if (NewGame.gamestate.ToLower() == "lost")
                        {

                            initialiseLoss();
                            gametimer.Stop();

                        }
                    }));
                }
               count++;

            });
        }

        //key up and down events
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
           

            if (e.KeyCode == settingsPage.KeyMap["MoveUp"])
            {
                NewGame.UpKey = true;
                Trace.WriteLine("key pressed ");
            }
            if (e.KeyCode == settingsPage.KeyMap["MoveLeft"])
            {
                NewGame.LeftKey = true;
                Trace.WriteLine("key pressed ");

            }
            if (e.KeyCode == settingsPage.KeyMap["MoveDown"])
            {
                NewGame.DownKey = true;
                Trace.WriteLine("key pressed ");

            }
            if (e.KeyCode == settingsPage.KeyMap["MoveRight"])
            {
                NewGame.RightKey = true; 
                Trace.WriteLine("key pressed ");

            }
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == settingsPage.KeyMap["MoveUp"])
            {
                NewGame.UpKey = false;
            }
            if (e.KeyCode == settingsPage.KeyMap["MoveLeft"])
            {
                NewGame.LeftKey = false;
            }
            if (e.KeyCode == settingsPage.KeyMap["MoveDown"])
            {
                NewGame.DownKey = false;
            }
            if (e.KeyCode == settingsPage.KeyMap["MoveRight"])
            {
                NewGame.RightKey = false;
            }

        }

        // draws the ui labels such as the labels for powerup indicators
        private void drawability(Graphics g)
        {
            _isSpeedOn = new Label
            {
                Left = 530,
                Top = 200,
                Text = "Speed",
                AutoSize = true,
                Visible = false,


            };
            this.Controls.Add(_isSpeedOn);

            _isGhostOn = new Label
            {
                Left = 530,
                Top = 300,
                Text = "Ghost",
                AutoSize = true,
                Visible = false,

            };
            this.Controls.Add(_isGhostOn);

            _isFreezeOn = new Label
            {
                Left = 530,
                Top = 250,
                Text = "Freeze",
                AutoSize = true,
                Visible = false,

            };
            this.Controls.Add(_isFreezeOn);

            KEY = new Label
            {
                Left = 530,
                Top = 150,
                Text = "",
                AutoSize = true,
                Visible = false,

            };
            this.Controls.Add(KEY);

            isSpeedOn = new Rectangle
            {
                X = 600,
                Y = 200,
                Height = 10,
                Width = 10
            };

            isFreezeOn = new Rectangle
            {
                X = 600,
                Y = 250,
                Height = 10,
                Width = 10
            };

            isGhostOn = new Rectangle
            {
                X = 600,
                Y = 300,
                Height = 10,
                Width = 10
            };

            pause = new Button
            {
                Text = "⏸️",
                Size = new Size(29, 29),
                Location = new Point(630, 10),
                BackColor = Color.Black,
                ForeColor = Color.White,
                Visible = false,



            };

            this.Controls.Add(pause);
            pause.Click += Pause_Click;

        }

        //checks the status of the powerups and updates the ui
        private void abilityStatus()
        {
            if (NewGame.powers.isGhostActive)
            {
                ghostcolor = Color.Orchid;
            }
            else { ghostcolor = Color.Red; }

            if (NewGame.powers.isSpeedActive)
            {
                speedcolor = Color.Orchid;
            }
            else { speedcolor = Color.Red; }

            if (NewGame.powers.isFreezeActive)
            {
                freezecolor = Color.Orchid;
            }
            else { freezecolor = Color.Red; }

            if (NewGame.IsKeyCollected)
            {
                KEY.Text = "collected";
            }
            else { KEY.Text = "not collected"; }
        }

        //pause button click event
        private void Pause_Click(object sender, EventArgs e)
        {
            gametimer.Stop();
            pausePanel = new Panel
            {
                Size = new Size(300, 300),
                Location = new Point(this.Width / 2 - 150, 70),
                BackColor = Color.White,
                Visible = true,
                BorderStyle = BorderStyle.FixedSingle


            };

            resume = new Button
            {
                Text = "Resume",
                Size = new Size(200, 60),
                Location = new Point(50, 100),
            };
            pausePanel.Controls.Add(resume);
            resume.Click += Resume_Click;

            menuButton = new Button
            {
                Text = "Menu",
                Size = new Size(200, 60),
                Location = new Point(50, 200),
            };
            pausePanel.Controls.Add(menuButton);
            menuButton.Click += Menu_Click;
            this.Controls.Add(pausePanel);


        }

        //resume button click event
        private void Resume_Click(object sender, EventArgs e)
        {
            gametimer.Start();
            pausePanel.Visible = false;
        }

        //paint event
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            NewGame.Player.drawPlayer(g);

            // Draw each cell
            foreach (var cell in NewGame.Maze.cells)
            {
                cell.Draw(g);
            }

            // Clear the previous player position
            //g.FillRectangle(Brushes.White, previousPlayerPosition); // Clear the old player position

            // Draw player rectangle
            Brush playerBrush = Brushes.Black;
            Brush enemyBrush = Brushes.Red;
            //g.FillRectangle(playerBrush, NewGame.player);
            NewGame.key.drawKey(g);
            NewGame.gate.drawGate(g);
           



            //drawability(g); moved this to only run at the start
            g.FillEllipse(new SolidBrush(speedcolor), isSpeedOn);
            g.FillEllipse(new SolidBrush(freezecolor), isFreezeOn);
            g.FillEllipse(new SolidBrush(ghostcolor), isGhostOn);

            foreach ( enemy x in NewGame.Enemies.enemyList)
            {
                x.Draw(g);
            }
            foreach(items y in NewGame.powers.powerups)
            {
                y.drawItems(g);
            }
            // Update the previous player position to the new one
            NewGame.previousPlayerPosition = NewGame.Player.PlayerRect;
        }

        // MENU components and events-------------------------------------------------------------------------------------------------------------------

        //saves the username
        private void SaveUserClick(object sender, EventArgs e)
        {
            if (username.Text == "")
            {
                MessageBox.Show("Please enter a username");
                return;
            }
            else if (username.Text.Length > 10)
            {
                MessageBox.Show("Username must be: " +
                                 " less than 10 characters" +
                                 " greater than 0 characters" +
                                 " cannot contain the , character");
            }
            else if (username.Text == "UNKNOWN")
            {
                MessageBox.Show("Invalid Username");
            }
            else if (username.Text.Contains(","))
            {
                MessageBox.Show("Username must be: " +
                                " less than 10 characters" +
                                " greater than 0 characters" +
                                " cannot contain the , character");
            }
            else { NewGame.username = username.Text; }


        }

        //menu button click event
        public void Menu_Click(object sender, EventArgs e)
        {

            changeVisibility();
            menu.Visible = true;
        }

        //quit button click event
        private void QuitClick(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //start button click event 
        private void startClick(object sender, EventArgs e)
        {
            menu.Visible = false;

            if (NewGame.gamestate != "playing")
            {
                if (NewGame.gamestate == "lost")
                {
                    loss.Visible = false;

                }
                if (NewGame.gamestate == "won")
                {
                    winner.Visible = false;

                }
                resetGame();


            }

            _isFreezeOn.Visible = true;
            _isGhostOn.Visible = true;
            _isSpeedOn.Visible = true;
            KEY.Visible = true;
            pause.Visible = true;


            gametimer.Tick += gameTick;

            gametimer.Start();
            //startTime = DateTime.Now;
            scoreWatch.Start();

            timer = new Label
            {
                Left = 520,
                Top = 100,
                Text = "Time: ",
                Visible = true,
                AutoSize = true,

            };
            this.Controls.Add(timer);
            this.Focus();


            //shouldGameStart = true;

        }

        //initialises the menu panel adding buttons and labels
        private void initialiseMenu()
        {
             menu = new Panel
             {
                Size = new Size(700, 700),
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Location = new Point(0,0),

             };
            Label title = new Label
            {
                Text = "maze.exe",
                Location = new Point(250, 50),
                Font = new Font("Consolas", 30),
                AutoSize = true
            };
            menu.Controls.Add(title);
            menu.Resize += Menu_Resize;

            username = new TextBox
            {
                Size = new Size(200, 60),
                Location = new Point(250, 130),
                Text = " Enter a username",
            };
            menu.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;
            menu.Controls.Add(username);
            username.Click += (sender, e) => { username.Text = ""; }; // clears the textbox when clicked

            SaveUserName = new Button
            {
                Text = "Save",
                Size = new Size(60, 30),
                Location = new Point(470, 130),
            };
            menu.Controls.Add(SaveUserName);
            SaveUserName.Click += SaveUserClick;


            Button leaderboard = new Button
            {
                Text = "Leaderboard",
                Size = new Size(200, 60),
                Location = new Point(250, 400),
            };
            leaderboard.Click += Leaderboard_Click;
            

            Button settings = new Button
            {
                Text = "settings",
                Size = new Size( 200 , 60),
                Location = new Point(250, 340),

            };
            settings.Click += SettingsClick;

            Button start = new Button
            {
                Text = "Play",
                Size = new Size (200,60),
                Location = new Point(250,200),

            };
            start.Click += startClick;

            Button Quit = new Button
            {
                Text = "Quit",
                Size = new Size(200, 60),
                Location = new Point(250, 270),

            };
            Quit.Click += QuitClick;

            menu.Controls.Add(start);
            menu.Controls.Add(settings);
            menu.Controls.Add(leaderboard);
            menu.Controls.Add(Quit);

            this.Controls.Add(menu);
        }

        //keeps the menu formatting consistent
        private void Menu_Resize(object sender, EventArgs e)
        {
            int usernameButtonX = (menu.Width / 2) - 100;
            int usernameButtonY = 260;

            int buttonx = (menu.Width / 2) - 100;
            int buttony = 250;
            username.Location = new Point(usernameButtonX, usernameButtonY);


            foreach (Button button in menu.Controls.OfType<Button>())
            {
                button.Location = new Point(buttonx, buttony);
                buttony += 70;
            }
            SaveUserName.Location = new Point(usernameButtonX + 220, usernameButtonY);

            foreach(Label label in menu.Controls.OfType<Label>())
            {
                label.Location = new Point((menu.Width / 2) - 100, 100);
                label.Font = new Font("Consolas", 40);
            }
        }

        // game state handing components and events--------------------------------------------------------------------------------------------------------

        //shows the game win screen and resets the game
        private void initialiseWin()
        {
            winner = new Panel
            {
                Size = new Size(700, 700),
                Location = new Point(0, 0),
                BackColor = Color.White,


            };

            Label winning = new Label
            {
                Text = "You Win !!!!",
                Left = 250,
                Top = 100,

            };
            winner.Controls.Add(winning);

            TimeSpan elapsed = scoreWatch.Elapsed;
            NewGame.score = $"{elapsed.Minutes}:{elapsed.Seconds}:{elapsed.Milliseconds / 10}";
            leaderboard.SaveScore();

            Label time = new Label
            {
                Text = $" Time completed: {NewGame.score}",
                Top = 140,
                Left = 250,
                AutoSize = true
            };
            winner.Controls.Add(time);

            Button menu = new Button
            {
                Text = "menu",
                Size = new Size(200, 60),
                Location = new Point(250, 200),
            };
            menu.Click += Menu_Click;
            Button Quit = new Button
            {
                Text = "Quit",
                Size = new Size(200, 60),
                Location = new Point(250, 270),

            };

            Quit.Click += QuitClick;
            winner.Controls.Add(menu);
            winner.Controls.Add(Quit);

            _isFreezeOn.Visible = false;
            _isGhostOn.Visible = false;
            _isSpeedOn.Visible = false;
            KEY.Visible = false;
            timer.Visible = false;
            pause.Visible = false;
            this.Controls.Add(winner);
        }

        //shows the game loss screen and resets the game 
        private void initialiseLoss()
        {
            loss = new Panel
            {
                Size = new Size(700, 700),
                Location = new Point(0, 0),
                BackColor = Color.White
            };
            Label loser = new Label
            {
                Text = "You Lose!",
                Left = 270,
                Top = 100,

            };
            loss.Controls.Add(loser);

            Button Quit = new Button
            {
                Text = "Quit",
                Size = new Size(200, 60),
                Location = new Point(250, 270),

            };

            Button menu = new Button
            {
                Text = "menu",
                Size = new Size(200, 60),
                Location = new Point(250, 200),
            };

            menu.Click += Menu_Click;
            Quit.Click += QuitClick;

            loss.Controls.Add(menu);
            loss.Controls.Add(Quit);

            _isFreezeOn.Visible = false;
            _isGhostOn.Visible = false;
            _isSpeedOn.Visible = false;
            KEY.Visible = false;
            timer.Visible = false;
            pause.Visible = false;


            this.Controls.Add(loss);
        }

        //resets the game variables and objects
        private void resetGame()
        {
            Maze = new maze(CellSize, GridSize);
            Maze.initialise();

            pathfind = new pathfinding(Maze.cells, Maze.GridSize);
            //NewGame = new game(Maze, pathfind, Enemies, this);
            NewGame = new game("normal", this);


            NewGame.start();
        }

        //leaderboard components and events--------------------------------------------------------------------------------------------------------------
        private void Leaderboard_Click(object sender, EventArgs e)
        {
            changeVisibility();

            leaderboard.drawLeaderboard();
            leaderboard.leaderboardPanel.Visible = true;
            this.Controls.Add(leaderboard.leaderboardPanel);
           
        }


        //settings components and events--------------------------------------------------------------------------------------------------------------
        private void SettingsClick(object sender, EventArgs e)
        {
            changeVisibility();
            this.Controls.Add(settingsPage.settingsPanel);

            settingsPage.settingsPanel.Visible = true;
            leaderboard.leaderboardPanel.Visible = false;
            settingsPage.generateSettingsPage();


        }

        

       

       

     

        
        
        
       
       

       

       
    }
}
