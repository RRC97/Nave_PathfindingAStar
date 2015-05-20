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
    public enum ComprassCard
    {
        North, Northeast, East, Southeast, South, Southwest, West, Northwest
    }
    public partial class Form1 : Form
    {
        Thread thread;
        Space[] spaces;
        List<Space> pass;
        Space[] comprass;
        Space a, b;
        int rows, columns;
        public Form1()
        {
            InitializeComponent();

            Paint += new PaintEventHandler(onDraw);

            rows = 10;
            columns = 15;

            DoubleBuffered = true;

            pass = new List<Space>(rows * columns);
            spaces = new Space[rows * columns];
            comprass = new Space[3 * 3];

            for (int i = 0; i < columns; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    int index = i * rows + j;
                    spaces[index] = new Space(i * 40, j * 40, i, j);
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
                        a.setValue(-1, -1);
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

        Space CalcQuad(ComprassCard cc, bool clean = false)
        {
            int index = (int)cc;
            int refX = a.getRefX();
            int refY = a.getRefY();

            if (cc == ComprassCard.Northeast
            ||  cc == ComprassCard.East
            ||  cc == ComprassCard.Southeast)
                refX += 1;

            if (cc == ComprassCard.Northwest
            ||  cc == ComprassCard.West
            ||  cc == ComprassCard.Southwest)
                refX -= 1;

            if (cc == ComprassCard.Northwest
            ||  cc == ComprassCard.North
            ||  cc == ComprassCard.Northeast)
                refY -= 1;

            if (cc == ComprassCard.Southwest
            ||  cc == ComprassCard.South
            ||  cc == ComprassCard.Southeast)
                refY += 1;

            if (refX < 0 || refY < 0)
                return null;

            foreach (Space space in spaces)
            {
                if (space.referedIn(refX, refY))
                {
                    if (clean)
                    {
                        space.setValue(0, 0);
                        space.setActive(false);
                    }
                    else
                    {
                        if (space.getValueTotal() == 0)
                        {
                            int difRefABX = b.getRefX() - refX;
                            int difRefABY = b.getRefY() - refY;
                            difRefABX = difRefABX < 0 ? -difRefABX : difRefABX;
                            difRefABY = difRefABY < 0 ? -difRefABY : difRefABY;

                            int valueA = 0;
                            int valueB = (difRefABX + difRefABY) * 10;

                            if (cc == ComprassCard.North
                            || cc == ComprassCard.South
                            || cc == ComprassCard.West
                            || cc == ComprassCard.East)
                                valueA = 10;

                            if (cc == ComprassCard.Northeast
                            || cc == ComprassCard.Northwest
                            || cc == ComprassCard.Southeast
                            || cc == ComprassCard.Southwest)
                                valueA = 14;

                            space.setValue(valueA, valueB);
                            space.setActive(true);

                            return space;
                        }
                        else if (space.getValueTotal() > 0 && space.getType() != SpaceType.Pass)
                        {
                            return space;
                        }
                    }
                }
            }
            return null;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Space)
            {
                int count = Enum.GetNames(typeof(ComprassCard)).Length;
                comprass = new Space[count];
                for (int i = 0; i < count; i++)
                {
                    comprass[i] = CalcQuad((ComprassCard)i);
                }
                bool blocked = true;

                foreach (Space space in comprass)
                {
                    if (space != null)
                    {
                        blocked = false;
                        break;
                    }
                }

                if (blocked)
                {
                    if (pass.Count > 0)
                    {
                        a.setType(SpaceType.Impossible);
                        a = pass[pass.Count - 1];
                        pass.Remove(a);
                        a.setType(SpaceType.A);

                        for (int i = 0; i < count; i++)
                        {
                            CalcQuad((ComprassCard)i, true);
                        }
                    }
                }
                else
                {
                    int index = 0;
                    for (int i = 0; i < comprass.Length; i++)
                    {
                        if (comprass[index] == null)
                            index = i;
                        else if (comprass[index].getType() == SpaceType.Wall)
                            index = i;
                        else if (comprass[i] != null && comprass[i].getType() != SpaceType.Pass)
                        {
                            int valueI = comprass[i].getValueTotal();
                            int valueIndex = comprass[index].getValueTotal();
                            if (valueI >= 0 && valueIndex > valueI)
                                index = i;
                        }
                    }

                    if (comprass[index].getType() != SpaceType.Wall
                    && comprass[index].getType() != SpaceType.Impossible
                    && comprass[index].getType() != SpaceType.Pass)
                    {
                        comprass[index].setType(SpaceType.A);
                        a.setType(SpaceType.Pass);
                        pass.Add(a);
                        a = comprass[index];
                    }
                    else if(comprass[index].getType() == SpaceType.Pass)
                    {
                        comprass[index].setType(SpaceType.A);
                        a.setType(SpaceType.Impossible);
                        a = comprass[index];
                    }
                }
            }
            if (e.KeyCode == Keys.Back)
            {
                a = b = null;
                foreach (Space space in spaces)
                {
                    space.setType(SpaceType.None);
                    space.setActive(false);
                    space.setValue(0, 0);
                }
            }
        }
    }
}
