using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CollisionDetectionV3
{
    public class enemies
    {

        public List<enemy> enemyList;
        private maze Maze;

        public float enemySpeed;

        public enemies(maze _Maze, float _speed)
        {
            Maze = _Maze;
            enemySpeed = _speed;
            initialiseEnemies();
        }
        
       
        public void initialiseEnemies()
        {
            enemyList = new List<enemy>();
            // initialises enemies in the corners of the maze
            Cell enemy1 = Maze.cells[Maze.index(0,Maze.GridSize-1)];
            int x = enemy1.Bounds.X + (Maze.CellSize / 2);
            int y = enemy1.Bounds.Y + (Maze.CellSize / 2);
            enemyList.Add(new enemy(x, y));

            Cell enemy2 = Maze.cells[Maze.index( Maze.GridSize - 1, Maze.GridSize - 1)];
            int x2 = enemy2.Bounds.X + (Maze.CellSize / 2);
            int y2 = enemy2.Bounds.Y + (Maze.CellSize / 2);
            enemyList.Add(new enemy(x2, y2));

            Cell enemy3 = Maze.cells[Maze.index(Maze.GridSize - 1, 0)];
            int x3 = enemy3.Bounds.X + (Maze.CellSize / 2);
            int y3 = enemy3.Bounds.Y + (Maze.CellSize / 2);
            enemyList.Add(new enemy(x3, y3));



        }

    }
}
