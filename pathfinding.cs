using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CollisionDetectionV3
{
    public class pathfinding
    {

        private List<Cell> cells;
        public List<Cell> path;
        private int GridSize;

        public pathfinding(List<Cell> cells, int GridSize)
        {
            this.cells = cells;
            this.GridSize = GridSize;

        }

        public List<Cell> FindPath(Cell start , Cell end)
        {
            //this function is the A* pathfinding algorithm that will be used to find the path the enemy will take to the player
            List<Cell> OpenCells = new List<Cell> { start };
            List<Cell> ClosedCells = new List<Cell>();
            Cell current;
            int PotentialGcost;

            foreach (Cell cell in cells)
            {
                cell.Gcost = int.MaxValue;  //as in pathfinding algos we set distance to infinity initaially
                cell.Hcost = 0;
                cell.parent = null; //this is because when a new path needs to be found we clear any parent data from previous paths so clean slate
            }

            start.Gcost = 0;
            start.Hcost = DistanceBetween(start, end);

            while (OpenCells.Count > 0)
            {
                current = OpenCells.OrderBy(cell => cell.Fcost).First(); //we set current to the next cell that has the lowest f value

                if(current == end)
                {
                    return EnemyPath(start, end); // if the path to the player has been found then return this path
                }

                OpenCells.Remove(current); // the current one has been visited so it is  now closed
                ClosedCells.Add(current);

                foreach(Cell neigbour in GetNeighbours(current))  //for the neigbours see if theyre walkable and open then find the cost of going to the neigbour and evaluate which one to go to
                {
                    if(ClosedCells.Contains(neigbour) || !CanEnemyWalk(current, neigbour))
                    {
                        continue;
                    }

                    PotentialGcost = current.Gcost + 1;  // here we see what the cost would be if we went to the next cell and compare it to neighbours

                    if (PotentialGcost < neigbour.Gcost)
                    {
                        neigbour.Gcost = PotentialGcost;
                        neigbour.Hcost= DistanceBetween(neigbour,end);
                        neigbour.parent = current;

                        if (!OpenCells.Contains(neigbour)) //adds neigbour to the open set so it can be explored later.
                        {
                            OpenCells.Add(neigbour);    
                        }
                    }

                }
            }

            return null;


        }



        private List<Cell> GetNeighbours(Cell MazeCell) // function to return the neighbours of the cell the enemy will be in
        {
            List<Cell> neighbours = new List<Cell>();

            if (MazeCell.Row > 0)
            {
                neighbours.Add(cells.First(c => c.Row == MazeCell.Row-1 && c.Col == MazeCell.Col)); //linq function used to add cells to neighbour based on if their in the bounds of the maze
            }
            if (MazeCell.Row < GridSize-1)
            {
                neighbours.Add(cells.First(c => c.Row == MazeCell.Row + 1 && c.Col == MazeCell.Col)); // if the cell is less than the last cell in the column then we can say to the right is valid
            }
            if (MazeCell.Col > 0)
            {
                neighbours.Add(cells.First(c => c.Row == MazeCell.Row && c.Col == MazeCell.Col-1));
            }
            if (MazeCell.Col < GridSize-1)
            {
                neighbours.Add(cells.First(c => c.Row == MazeCell.Row && c.Col == MazeCell.Col+1));
            }

            return neighbours;
        }

        private bool CanEnemyWalk(Cell current, Cell neighbour)  // func to see if enemy can walk to a certain cell returns false if there is a wall in the way
        {
            if (current.Row > neighbour.Row && current.NorthWall) return false;
            if (current.Row < neighbour.Row && neighbour.NorthWall) return false;
            if (current.Col > neighbour.Col && current.WestWall) return false;
            if (current.Col < neighbour.Col && neighbour.NorthWall) return false;
            return true;
        }

        public int DistanceBetween(Cell enemy, Cell player) // manhattan distance between player and enemy
        {
            Random rand = new Random();
            int distance = System.Math.Abs(enemy.Row - player.Row) + System.Math.Abs(enemy.Col - player.Col);
            
            return distance;
        }

        private List<Cell> EnemyPath(Cell start, Cell end)  // this just retraces and returns the path the enemy will take for the move enemy func
        {
            List<Cell> path = new List<Cell>();

            Cell current;

            current = end;

            while (current != start && current!= null)
            {
                path.Add(current);
                current = current.parent;
            }

            path.Reverse();
            return path;
        }

        public void PreciseTrack(Cell playerCell, enemy Enemy, float enemySpeed, maze Maze)
        {
            //this function moves the enemy towards the player using the actual a star path
            Cell nextCell;
            Random rand = new Random();
            path = FindPath(Enemy.currentCell, playerCell);

            if (path != null && path.Count > 0)
            {
                nextCell = path[0]; // we set it to the first item as each update a new path is found

                float magnitude;


                // vectors for moving the enemy in the direction of the next cell in the path
                PointF target = new PointF(nextCell.Bounds.X + nextCell.Bounds.Width / 2, nextCell.Bounds.Y + nextCell.Bounds.Height / 2); //the target is the centre of the next cells rectangle
                PointF current = new PointF(Enemy.currentPos.X + Enemy.currentPos.Width / 2, Enemy.currentPos.Y + Enemy.currentPos.Height / 2);
                PointF direction = new PointF(target.X - current.X, target.Y - current.Y);

                magnitude = (float)Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y); //finding magnitude

                if (magnitude > 0) // normalising the vector so its magnitude is one while keeping its direction so we can accelerate towards it
                {
                    direction.X /= magnitude;
                    direction.Y /= magnitude;

                }

                Enemy.velocity = new PointF(direction.X * enemySpeed, direction.Y * enemySpeed);
                int newposx = (int)(Enemy.currentPos.X + Enemy.velocity.X);
                int newposy = (int)(Enemy.currentPos.Y + Enemy.velocity.Y);

                Enemy.currentPos = new Rectangle(newposx, newposy, Enemy.currentPos.Width, Enemy.currentPos.Height);

                
            }
        }
        public void Patrol(maze Maze, enemy Enemy, float enemySpeed)
        {
            
            Random rand = new Random();
            Cell previous = Enemy.currentCell;
            List<Cell> neighbors = previous.GetNearbyCell(Maze);
            neighbors.Remove(previous);

            

            // If the enemy has no target or is close to its target, pick a new target
           if (Enemy.TargetCell == null ||
               (Math.Abs(Enemy.currentPos.X + Enemy.currentPos.Width / 2 - Enemy.TargetCell.Bounds.X - Enemy.TargetCell.Bounds.Width / 2) < enemySpeed &&
                Math.Abs(Enemy.currentPos.Y + Enemy.currentPos.Height / 2 - Enemy.TargetCell.Bounds.Y - Enemy.TargetCell.Bounds.Height / 2) < enemySpeed))
           {
               // Pick a new random neighboring cell
               Enemy.TargetCell = neighbors[rand.Next(neighbors.Count)];
                // Snap to the current target cell

           }

            // Calculate the target position
            PointF target = new PointF(
                Enemy.TargetCell.Bounds.X + Enemy.TargetCell.Bounds.Width / 2,
                Enemy.TargetCell.Bounds.Y + Enemy.TargetCell.Bounds.Height / 2
            );
            PointF current = new PointF(
                Enemy.currentPos.X + Enemy.currentPos.Width / 2,
                Enemy.currentPos.Y + Enemy.currentPos.Height / 2
            );

            // Calculate direction and normalize it
            PointF direction = new PointF(target.X - current.X, target.Y - current.Y);
            float magnitude = (float)Math.Sqrt(direction.X * direction.X + direction.Y * direction.Y);

            if (magnitude > 0)
            {
                direction.X /= magnitude;
                direction.Y /= magnitude;
            }

            // Apply velocity
            PointF enemyVelocity = new PointF(direction.X * enemySpeed, direction.Y * enemySpeed);
            int newposx = (int)(Enemy.currentPos.X + enemyVelocity.X);
            int newposy = (int)(Enemy.currentPos.Y + enemyVelocity.Y);

            // Update enemy position
            Enemy.currentPos = new Rectangle(newposx, newposy, Enemy.currentPos.Width, Enemy.currentPos.Height);

        }

      
       
    }
}
