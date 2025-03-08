using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollisionDetectionV3
{
    public class Gate
    {

        public Rectangle gateRect;
        public Color pigment;

        public void initialiseGate(maze Maze, Color color)
        {
            Cell pos = Maze.cells.Last();
            pigment = color;
            gateRect = new Rectangle(pos.Bounds.X + (Maze.CellSize / 2), pos.Bounds.Y + (Maze.CellSize / 2), 10, 10);
        }

        public void drawGate(Graphics g)//draws the gate with the given color
        {
            using (Brush brush = new SolidBrush(pigment))
            {
                g.FillEllipse(brush, gateRect);
            }

        }
    }
}
