using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace PathfindingAStar
{
    public enum SpaceType
    {
        None, Wall, A, B
    }
    class Space
    {
        float x, y;
        int valueA, valueB;
        int refX, refY;
        bool active;
        SpaceType type;

        public Space(float x, float y, int refX, int refY, SpaceType type = SpaceType.None)
        {
            this.refX = refX;
            this.refY = refY;
            this.x = x;
            this.y = y;
            this.type = type;
        }

        public void onDraw(Graphics g)
        {
            switch(type)
            {
                case SpaceType.Wall:
                    g.FillRectangle(Brushes.Blue, x, y, 40, 40);
                    break;
                case SpaceType.A:
                    g.FillRectangle(Brushes.Green, x, y, 40, 40);
                    break;
                case SpaceType.B:
                    g.FillRectangle(Brushes.Red, x, y, 40, 40);
                    break;
            }
            g.DrawRectangle(Pens.Blue, x, y, 40, 40);

            if (active)
            {


                Font font = new Font(new FontFamily("Arial"), 6);
                g.DrawString((valueA + valueB).ToString(), font, Brushes.White, x + 4, y + 4);
                g.DrawString(valueA.ToString(), font, Brushes.White, x + 4, y + 30);
                g.DrawString(valueB.ToString(), font, Brushes.White, x + 20, y + 30);
            }
        }

        public void setActive(bool active)
        {
            this.active = active;
        }

        public int getRefX() { return refX; }
        public int getRefY() { return refY; }

        public bool referedIn(int x, int y)
        {
            if (refX == x && refY == y)
                return true;

            return false;
        }

        public void setValue(int a, int b)
        {
            this.valueA = a;
            this.valueB = b;
        }

        public int getValueTotal()
        {
            if(this.type != SpaceType.Wall)
                return (this.valueA + this.valueB);

            return -1;
        }

        public void setType(SpaceType type)
        {
            this.type = type;
        }

        public SpaceType getType()
        {
            return type;
        }

        public bool contains(float x, float y)
        {
            if(x > this.x && x < this.x + 40
            && y > this.y && y < this.y + 40)
                return true;

            return false;
        }
    }
}
