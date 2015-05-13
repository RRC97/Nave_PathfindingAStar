using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace PathfindingAStar
{
    public partial class Form1 : Form
    {
        Thread thread;
        Space[] spaces;
        Space a, b;
        int rows, columns;
        public Form1()
        {
            InitializeComponent();

            Paint += new PaintEventHandler(onDraw);

            rows = 10;
            columns = 15;

            DoubleBuffered = true;

            spaces = new Space[rows * columns];

            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    int index = i * rows + j;
                    spaces[index] = new Space(i * 40, j * 40);
                }
            }

            Size = new Size(columns * 40 + 2, rows * 40 + 2);

            thread = new Thread(new ThreadStart(onUpdate));
            thread.Start();
        }

        public void onDraw(Object sender, PaintEventArgs e)
        {
            foreach (Space space in spaces)
                space.onDraw(e.Graphics);
        }

        public void onUpdate()
        {
            while (true)
            {
                Invalidate();
            }
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            float x = e.X;
            float y = e.Y;
            if (a == null)
            {
                foreach (Space space in spaces)
                {
                    if (space.getType() == SpaceType.None && space.contains(x, y))
                    {
                        a = space;
                        a.setType(SpaceType.A);
                        break;
                    }
                }
            }
            else if (b == null)
            {
                foreach (Space space in spaces)
                {
                    if (space.getType() == SpaceType.None && space.contains(x, y))
                    {
                        b = space;
                        b.setType(SpaceType.B);
                        break;
                    }
                }
            }
            else
            {
                foreach (Space space in spaces)
                {
                    if (space.getType() == SpaceType.None && space.contains(x, y))
                    {
                        space.setType(SpaceType.Wall);
                        break;
                    }
                }
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {

            }
            if (e.KeyCode == Keys.Back)
            {
                a = b = null;
                foreach (Space space in spaces)
                {
                    space.setType(SpaceType.None);
                }
            }
        }
    }
}
