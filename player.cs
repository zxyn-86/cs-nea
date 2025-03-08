using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollisionDetectionV3
{
    public class player
    {
        public Rectangle PlayerRect;
        public int height, width;
        public double speed;
        private maze Maze;
        public player(maze _Maze, double _speed)
        {
            Maze = _Maze;
            speed = _speed; 
            initailisePlayer();
        }

        public void initailisePlayer()
        {
            Cell position = Maze.cells[0];
            width = 10;
            height = 10;
            PlayerRect = new Rectangle(position.Bounds.X + (Maze.CellSize / 2), position.Bounds.Y + (Maze.CellSize / 2), width, height);
        }

        public void drawPlayer(Graphics g)
        {
            Pen pen = new Pen(Color.Black);
            Brush brush = new SolidBrush(Color.FromArgb(97, 110, 159));
            g.FillRectangle(brush,PlayerRect);
        }
    }
}
