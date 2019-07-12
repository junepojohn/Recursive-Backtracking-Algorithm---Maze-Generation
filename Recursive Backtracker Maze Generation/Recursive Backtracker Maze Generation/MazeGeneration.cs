using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Recursive_Backtracker_Maze_Generation
{
    public partial class MazeGeneration : Form
    {
        public MazeGeneration()
        {
            InitializeComponent();
        }

        int arrSize = 15;
        int width = 800;
        int size;
        Cell[,] grid;
        Cell current;
        int cx = 0;
        int cy = 0;
        Random rand = new Random();
        int totalVisited = 0;
        //Stack hold previous locations.  Can the stack overflow? Most likely
        List<Cell> stack = new List<Cell>();

        private void Form1_Load(object sender, EventArgs e)
        {
            size = width / arrSize;
            this.DoubleBuffered = true;
            grid = new Cell[arrSize, arrSize];
            CreateGrid();
        }

        void CreateGrid()
        {
            for (int x = 0; x < arrSize; x++)
            {
                for (int y = 0; y < arrSize; y++)
                {
                    grid[x, y] = new Cell(x, y, size);
                }
            }

            current = grid[cx, cy];
            current.visited = true;
            current.isCurrent = true;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            for (int x = 0; x < arrSize; x++)
            {
                for (int y = 0; y < arrSize; y++)
                {
                    grid[x, y].FillCellWithColor(e.Graphics);
                    grid[x, y].DrawWalls(e.Graphics);
                }
            }
        }

        public void CheckNeighbors()
        {
            List<Cell> neighbors = new List<Cell>();
            //if (cx - 1 < 0 || cy - 1 < 0
            //   || cx + 1 >= arrSize || cy + 1 >= arrSize)
            //    return;
            current.isCurrent = false;

            //Above
            if (cy - 1 >= 0)
                if (!grid[cx, cy - 1].visited)
                {
                    neighbors.Add(grid[cx, cy - 1]);
                }

            //Right
            if (cx + 1 < arrSize)
                if (!grid[cx + 1, cy].visited)
                {
                    neighbors.Add(grid[cx + 1, cy]);
                }

            //Below
            if (cy + 1 < arrSize)
                if (!grid[cx, cy + 1].visited)
                {
                    neighbors.Add(grid[cx, cy + 1]);
                }

            //Left
            if (cx - 1 >= 0)
                if (!grid[cx - 1, cy].visited)
                {
                    neighbors.Add(grid[cx - 1, cy]);
                }
            Cell next;
            if (neighbors.Count > 0)
            {
                next = neighbors[rand.Next(neighbors.Count)];
                cx = next.posX / size;
                cy = next.posY / size;
                next.visited = true;
                totalVisited++;
                next.RemoveWalls(current, next);
                stack.Insert(0, current);
                current.inStack = true;
                current = next;
                current.isCurrent = true;
            }
            else
            {
                //Backtrack
                if (stack.Count > 0)
                {
                    next = stack[0];
                    next.inStack = false;
                    stack.RemoveAt(0);
                    cx = next.posX / size;
                    cy = next.posY / size;
                    current = next;
                    current.isCurrent = true;
                }


                //Randomized backtracking = useless
                //List<int> pos = new List<int>();
                //next = null;

                //for (int i = 0; i < 4; i++)
                //{
                //    if (!current.walls[i])
                //    {
                //        pos.Add(i);
                //    }
                //}

                //int posRand = pos[rand.Next(pos.Count)];

                ////Top
                //if(posRand == 0)
                //{
                //    next = grid[cx, cy - 1];
                //}
                ////Right
                //if (posRand == 1)
                //{
                //    next = grid[cx + 1, cy];
                //}
                ////Bottom
                //if (posRand == 2)
                //{
                //    next = grid[cx, cy + 1];
                //}
                ////Left
                //if (posRand == 3)
                //{
                //    next = grid[cx - 1, cy];
                //}

                //if (next.visited)
                //{
                //    current.isCurrent = false;
                //    cx = next.posX / size;
                //    cy = next.posY / size;
                //    next.RemoveWalls(current, next);
                //    next.visited = true;
                //    current = next;
                //    current.isCurrent = true;
                //}
            }
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Space)
            {
                while(stack.Count > 0 || totalVisited < Math.Pow(arrSize, 2) - 1)
                {
                    CheckNeighbors();

                    this.Invalidate();
                    this.Update();
                    this.Refresh();

                    //System.Threading.Thread.Sleep(50);
                }
            }
        }
    }

    public class Cell
    {
        public bool isCurrent = false;
        public bool visited = false;
        public bool inStack = false;
        public int posX, posY;
        int w;
        int thickness = 2;
        public bool[] walls = new bool[] { true, true, true, true };

        public void DrawWalls(Graphics g)
        {
            //Top
            if (walls[0])
            {
                g.FillRectangle(Brushes.Black, new Rectangle(posX, posY, w, thickness));
            }
            //Right
            if (walls[1])
            {
                g.FillRectangle(Brushes.Black, new Rectangle(posX + w, posY, thickness, w));
            }
            //Bottom
            if (walls[2])
            {
                g.FillRectangle(Brushes.Black, new Rectangle(posX, posY + w, w, thickness));
            }
            //Left
            if (walls[3])
            {
                g.FillRectangle(Brushes.Black, new Rectangle(posX, posY, thickness, w));
            }
        }

        public void RemoveWalls(Cell c, Cell n)
        {
            int cx = c.posX / w;
            int cy = c.posY / w;

            int nx = n.posX / w;
            int ny = n.posY / w;

            int offsetX = cx - nx;
            if (offsetX == 1)
            {
                c.walls[3] = false;
                n.walls[1] = false;
            }
            else if (offsetX == -1)
            {
                c.walls[1] = false;
                n.walls[3] = false;
            }

            int offsetY = cy - ny;
            if (offsetY == 1)
            {
                c.walls[0] = false;
                n.walls[2] = false;
            }
            else if (offsetY == -1)
            {
                c.walls[2] = false;
                n.walls[0] = false;
            }
        }

        public void FillCellWithColor(Graphics gr)
        {
            if (!visited)
                gr.FillRectangle(Brushes.LightGray, new Rectangle(posX, posY, w, w));
            else
                gr.FillRectangle(Brushes.Purple, new Rectangle(posX, posY, w, w));

            if (isCurrent)
                gr.FillRectangle(Brushes.Blue, new Rectangle(posX, posY, w, w));

            if(inStack)
                gr.FillRectangle(Brushes.Yellow, new Rectangle(posX, posY, w, w));
        }

        public Cell(int x, int y, int size)
        {
            w = size;
            posX = x * size;
            posY = y * size;
        }
    }
}
