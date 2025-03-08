using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace CollisionDetectionV3
{
    public enum WallType
    {
        North,
        East,
        South,
        West
    }
    public class Cell
    {
        public int Row { get; }
        public int Col { get; }
        public int Size { get; }
        public bool NorthWall { get; set; }
        public bool EastWall { get; set; }
        public bool SouthWall { get; set; }
        public bool WestWall { get; set; }
        public bool Visited { get; set; } // To track if the cell has been visited
        public Rectangle Bounds { get; }

        public int Gcost;

        public int Hcost;
        public int Fcost => Gcost + Hcost;

        public Cell parent;

        public List<Cell> neighbors;
        public Cell(int row, int col, int size)
        {
            Row = row;
            Col = col;
            Size = size;
            Bounds = new Rectangle(col * size, row * size, size, size);

            // Initially, all walls are present
            NorthWall = true;
            EastWall = true;
            SouthWall = true;
            WestWall = true;

            
        }

        public void RemoveWall(WallType wall)
        {
            switch (wall)
            {
                case WallType.North:
                    NorthWall = false;
                    break;
                case WallType.East:
                    EastWall = false;
                    break;
                case WallType.South:
                    SouthWall = false;
                    break;
                case WallType.West:
                    WestWall = false;
                    break;
            }
        }

        public void Draw(Graphics g)
        {
            Pen pen = Pens.Black;

            if (NorthWall)
                g.DrawLine(pen, Bounds.Left, Bounds.Top, Bounds.Right, Bounds.Top);

            if (EastWall)
                g.DrawLine(pen, Bounds.Right, Bounds.Top, Bounds.Right, Bounds.Bottom);

            if (SouthWall)
                g.DrawLine(pen, Bounds.Left, Bounds.Bottom, Bounds.Right, Bounds.Bottom);

            if (WestWall)
                g.DrawLine(pen, Bounds.Left, Bounds.Top, Bounds.Left, Bounds.Bottom);
        }


        public List<Cell> GetNearbyCell(maze Maze)
        {
            List<Cell> neighbors = new List<Cell>();
            Random rand = new Random();
            int row = this.Row;
            int col = this.Col;

            // Check North, South, East, West neighbors
            if (!this.NorthWall && IsCellValid(row - 1, col, Maze)) // north
                neighbors.Add(Maze.cells[(row - 1) * Maze.GridSize + col]);

            if (!this.SouthWall && IsCellValid(row + 1, col, Maze)) // South
                neighbors.Add(Maze.cells[(row + 1) * Maze.GridSize + col]);

            if (!this.WestWall && IsCellValid(row, col - 1, Maze)) // West
                neighbors.Add(Maze.cells[row * Maze.GridSize + (col - 1)]);

            if (!this.EastWall && IsCellValid(row, col + 1 , Maze)) // East
                neighbors.Add(Maze.cells[row * Maze.GridSize + (col + 1)]);

            return neighbors;
        }

        private bool IsCellValid(int row, int col, maze Maze)
        {
            return row>=0 && row <= Maze.GridSize-1 && col >=0 && col <= Maze.GridSize-1;
        }
    }
}
