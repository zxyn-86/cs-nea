using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
//using System.Threading;
using System.Threading.Tasks;
//using System.Timers;
using System.Windows.Forms;

namespace CollisionDetectionV3
{
    public class PowerUps
    {

        public List<items> powerups;
        private Random random;
        private maze Maze;
        private pathfinding pathfind;
        private player Player;

        public bool isSpeedActive;
        public bool isGhostActive;
        public bool isFreezeActive;

        private int duration = 6000;

        private DateTime SpeedStart;
        private DateTime GhostStart;
        private DateTime FreezeStart;

        private Timer powerupTimer;
        
        public PowerUps(maze _Maze, player _Player)
        {
            Maze = _Maze;
            
            Player = _Player;
            random = new Random();
            isFreezeActive = false;
            isGhostActive = false;
            isSpeedActive = false;
            initialisePowerups();
        }

        public void initialisePowerTimer() //initialises a timer to time deactivation of powerups
        {
            powerupTimer = new Timer();
            powerupTimer.Interval = duration;
            powerupTimer.Tick += powerUp_Tick;
            powerupTimer.Start();
        }
        public void initialisePowerups() //picks a random cell and spawns a power of random type
        {
            powerups = new List<items>();
            Cell randomCell;
            PowerUpType type = (PowerUpType)random.Next(0,3); //figure out how to make it a random type from the enum
            int count = 3;
            for (int i = 0; i < count; i++)
            {
                randomCell = Maze.cells[random.Next(0, Maze.cells.Count)];
                int x = randomCell.Bounds.X + (Maze.CellSize / 2);
                int y = randomCell.Bounds.Y + (Maze.CellSize / 2);
                powerups.Add(new items((PowerUpType)random.Next(0, 3), x, y));
            }
        }

   
        public void CheckForPowerup() //loops through the list of powerups and checks if player collides
        {
            foreach(items item in powerups)
            {
                if (Player.PlayerRect.IntersectsWith(item.ItemRect))
                {
                    switch(item.type)
                    {
                        case(PowerUpType.speedBoost):
                            activateSpeed();
                            break;
                        case (PowerUpType.Freeze):
                            activateFreeze();
                            break;
                        case (PowerUpType.Ghost):
                            activateGhost(); 
                            break;
                    }

                    remove(item);
                    break;
                }
                
            }
        }
        private void powerUp_Tick(object sender, EventArgs e) 
        {
            //checks if the diff between when the item was collected and now is > than the 6s time
            if (isSpeedActive && (DateTime.Now - SpeedStart).TotalMilliseconds>duration) 
            {
                isSpeedActive = false;
                Player.speed /= 1.4; //returns speed to normal
            }
            if (isFreezeActive && (DateTime.Now - FreezeStart).TotalMilliseconds > duration)
            {
                isFreezeActive = false; //boolean val so i can handle the freeze in the actial main tick
            }
            if (isGhostActive && (DateTime.Now - GhostStart).TotalMilliseconds > duration)
            {
                isGhostActive = false;
            }
        }
        private void activateSpeed()
        {
            if(!isSpeedActive)
            {
                isSpeedActive = true;
                SpeedStart = DateTime.Now; //establishes start time so we can stop it after 6 seconds
                Player.speed *= 1.7;
                
            }

        }
        private void activateGhost()
        {
            if (!isGhostActive)
            {
                isGhostActive = true;
                GhostStart = DateTime.Now;  //establishes start time so we can stop it after 6 seconds
            }
        }
        private void activateFreeze()
        {
            if (!isFreezeActive)
            {
                isFreezeActive = true;
                FreezeStart = DateTime.Now;  //establishes start time so we can stop it after 6 seconds
            }
        }

        private void remove(items item)
        {
            powerups.Remove(item); //used to remove the item from the list so that it isnt drawn in the paint method
        }
    }
}
