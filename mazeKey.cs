using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CollisionDetectionV3
{
    public class mazeKey
    {
        
        Random random = new Random();
        int col, row;
        public Rectangle Keyrect;
        public Color pigment;
        
        public void initialisekey(maze Maze, Color color)
        {
            col = random.Next(0, Maze.GridSize - 1);
            row = random.Next(0, Maze.GridSize - 1);
            pigment = color;
            Keyrect = new Rectangle(col * Maze.CellSize+(Maze.CellSize/2), row * Maze.CellSize + (Maze.CellSize / 2), 15, 15);
        }

        public void drawKey(Graphics g) //draws the key with the given color
        {
            using(Brush brush = new SolidBrush(pigment))
            {
                g.FillEllipse(brush , Keyrect);
            }
            
        }
      
    }
}
