using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollisionDetectionV3
{
    public class maze
    {


        public List<Cell> cells; // List to store cells
        public  int CellSize; // Size of each cell
        public int GridSize; // Number of cells in a row and column
        private Random random = new Random();
        private Stack<Cell> stack = new Stack<Cell>(); // Stack for DFS traversal
        public List<Cell> neighbors;


        public maze(int _CellSize, int _GridSize)
        {
            CellSize = _CellSize;
            GridSize = _GridSize;
            
        }



        public void GenerateMaze()
        {
            //initialise();

            neighbors = new List<Cell>();       
            Cell startCell = cells[random.Next(cells.Count)];
            startCell.Visited = true;
            stack.Push(startCell);

            while (stack.Count > 0)
            {
                Cell currentCell = stack.Peek();
                List<Cell> unvisitedNeighbors = GetUnvisitedNeighbors(currentCell);

                if (unvisitedNeighbors.Count > 0)
                {
                    // Choose a random unvisited neighbor
                    Cell neighbor = unvisitedNeighbors[random.Next(unvisitedNeighbors.Count)];

                    // Remove the wall between the current cell and the neighbor
                    RemoveWallBetween(currentCell, neighbor);

                    // Mark the neighbor as visited and push it onto the stack
                    neighbor.Visited = true;
                    stack.Push(neighbor);
                }
                else
                {
                    // Backtrack by popping the stack
                    stack.Pop();
                }
            }
        }
        private List<Cell> GetUnvisitedNeighbors(Cell cell)
        {
            neighbors = new List<Cell>();

            int row = cell.Row;
            int col = cell.Col;

            // Check North, South, East, West neighbors
            if (row > 0 && !cells[(row - 1) * GridSize + col].Visited) // North
                neighbors.Add(cells[(row - 1) * GridSize + col]);

            if (row < GridSize - 1 && !cells[(row + 1) * GridSize + col].Visited) // South
                neighbors.Add(cells[(row + 1) * GridSize + col]);

            if (col > 0 && !cells[row * GridSize + (col - 1)].Visited) // West
                neighbors.Add(cells[row * GridSize + (col - 1)]);

            if (col < GridSize - 1 && !cells[row * GridSize + (col + 1)].Visited) // East
                neighbors.Add(cells[row * GridSize + (col + 1)]);

            return neighbors;
        }

        private void RemoveWallBetween(Cell cell1, Cell cell2)
        {
            if (cell1.Row == cell2.Row)
            {
                // Same row: Remove East/West wall
                if (cell1.Col < cell2.Col)
                {
                    cell1.EastWall = false;
                    cell2.WestWall = false;
                }
                else
                {
                    cell1.WestWall = false;
                    cell2.EastWall = false;
                }
            }
            else if (cell1.Col == cell2.Col)
            {
                // Same column: Remove North/South wall
                if (cell1.Row < cell2.Row)
                {
                    cell1.SouthWall = false;
                    cell2.NorthWall = false;
                }
                else
                {
                    cell1.NorthWall = false;
                    cell2.SouthWall = false;
                }
            }
        }

        public bool IsCollidingWithAnyCell(Rectangle playerRect)
        {
            foreach (Cell cell in cells)
            {
                if (IsCollidingWithCellWalls(playerRect, cell))
                {
                    return true;
                }
            }
            return false;
        }



        private bool IsCollidingWithCellWalls(Rectangle playerRect, Cell cell)
        {
            // Check each wall of the cell against the player rectangle
            if (cell.NorthWall && IsColliding(playerRect, new Rectangle(cell.Bounds.Left, cell.Bounds.Top, cell.Bounds.Width, 1)))
                return true;

            if (cell.EastWall && IsColliding(playerRect, new Rectangle(cell.Bounds.Right, cell.Bounds.Top, 1, cell.Bounds.Height)))
                return true;

            if (cell.SouthWall && IsColliding(playerRect, new Rectangle(cell.Bounds.Left, cell.Bounds.Bottom, cell.Bounds.Width, 1)))
                return true;

            if (cell.WestWall && IsColliding(playerRect, new Rectangle(cell.Bounds.Left, cell.Bounds.Top, 1, cell.Bounds.Height)))
                return true;

            return false;
        }

        private bool IsColliding(Rectangle rectOne, Rectangle rectTwo)
        {
            return rectOne.Right > rectTwo.Left &&
                   rectOne.Left < rectTwo.Right &&
                   rectOne.Bottom > rectTwo.Top &&
                   rectOne.Top < rectTwo.Bottom;
        }

        public void initialise()
        {
            cells = new List<Cell>();
            // Initialize cells
            for (int row = 0; row < GridSize; row++)
            {
                for (int col = 0; col < GridSize; col++)
                {
                    cells.Add(new Cell(row, col, CellSize));
                }
            }
        }

        public int index( int row, int col )
        {
            int index = row * GridSize + col;
            return index;
        }



    }
}
