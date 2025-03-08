using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CollisionDetectionV3
{
    public class game
    {
        //game objects------------------------------------------------------------------------------------------

        public maze Maze;
        public mazeKey key;
        public Gate gate;
        public PowerUps powers;
        public pathfinding pathfind;
        public enemies Enemies;
        private Form Form1;
        public player Player;
        public Rectangle player;
        public Rectangle previousPlayerPosition;

        //game flags----------------------------------------------------------------------------------------
        public bool gameover;
        public bool IsKeyCollected;
        public bool Caught;
        public bool isColliding;
        public bool UpKey;
        public bool DownKey;
        public bool RightKey;
        public bool LeftKey;

        //other game variables-------------------------------------------------------------------------------
       
        public int playerSpeed = 7; // Movement speed of the player
        public string difficulty;
        public string gamestate;
        public string username;
        public string score;
        //-----------------------------------------------------------------------------------------------------


        //constructor
        public game(string _difficulty, Form _Form1)
        {
            
            Form1 = _Form1;
            changeDifficulty(_difficulty);
        }

        /* constructor
         *  public game(maze _Maze, pathfinding _pathfind, enemies _Enemies, Form _Form1)
        {
            
           pathfind = _pathfind;
           
           Maze = _Maze;
           Enemies = _Enemies; 
           Form1 = _Form1;

        }
         */

        // function to start the game initialising the game objects
        public void start()
        {

            Maze.GenerateMaze();

            key = new mazeKey();
            gate = new Gate();
            key.initialisekey(Maze, Color.Gold);
            gate.initialiseGate(Maze, Color.Black);

            powers = new PowerUps(Maze, Player);           
            powers.initialisePowerTimer();

            previousPlayerPosition = Player.PlayerRect;

            gamestate = "playing";
            username = "UNKNOWN";
            score = "0:0:0";
            difficulty = "normal";

            IsKeyCollected = false;
            Caught = false;
        }

        //functions to handle detecting if the game is over by changing a flag
        public void handleGameOver()
        {
            if (Caught)
            {
                //endGame(); //takes player to lose screen
                gamestate = "lost";
            }
            else if(IsKeyCollected)
            {
                OpenGate(); // takes player to win screen
            }
            else
            {
                gamestate = "playing";
            }
        }

        public void OpenGate()
        {
            //checks the condition for completing the maze
            if (Player.PlayerRect.IntersectsWith(gate.gateRect))
            {
                gamestate = "won"; 
            }
        }

        public void changeDifficulty( string _difficulty)
        {
            difficulty = _difficulty.ToLower();
            int GridSize = 10;
            int CellSize = 30;
            float enemyspeed = 1.5f;
            double playerspeed = 5;
            Graphics g = Form1.CreateGraphics();

            if (difficulty == "easy")
            {
                GridSize = 7;
                playerspeed = 7;
                enemyspeed = 1;
              
            }
            else if (difficulty == "normal")
            {
                GridSize = 9;
                playerspeed = 5;
                enemyspeed = 1.5f;
              
            }
            else if (difficulty == "hard")
            {
                GridSize = 15;
                playerspeed = 5;
                enemyspeed = 2;
                
            }

            CellSize = 500/GridSize;  // keeps the size of the maze constant 500x500 pixels

            Maze = new maze(CellSize, GridSize);
            Maze.initialise();
            Maze.GenerateMaze();
            pathfind = new pathfinding(Maze.cells, GridSize); //re initialising the game elements
            Enemies = new enemies(Maze, enemyspeed);
            Player = new player(Maze, playerspeed);

            powers = new PowerUps(Maze, Player); //reinitialising the powerups and items
            // powers.initialisePowerTimer();
            key = new mazeKey();
            gate = new Gate();
            key.initialisekey(Maze, Color.Gold);
            gate.initialiseGate(Maze, Color.Black);


        }
     

    }

    
}
