using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travelers
{
    public enum Compass
    {
        C, NE, E, SE, SW, W, NW
    }

    public static class CompassExtension
    {
        public static Compass Direction(this Vector2 a, Vector2 b)
        {
            if (a == b) return Compass.C;

            if (b.X == a.X + 1 && a.Y == b.Y) return Compass.E;
            else if (b.X == a.X - 1 && a.Y == b.Y) return Compass.W;
            else
            {
                if (b.Y == a.Y - 1)
                {
                    if (b.X == a.Y % 2 + a.X) return Compass.NE;
                    else if (b.X == a.Y % 2 + a.X - 1) return Compass.NW;
                }
                else if (b.Y == a.Y + 1)
                {
                    if (b.X == a.Y % 2 + a.X) return Compass.SE;
                    else if (b.X == a.Y % 2 + a.X - 1) return Compass.SW;
                }
            }

            throw new Exception("Compass wrong.");
        }

        public static Compass[] Neighbors(this Compass c)
        {
            switch (c)
            {
                case Compass.E: return new Compass[] { Compass.SE, Compass.NE };
                case Compass.W: return new Compass[] { Compass.SW, Compass.NW };
                case Compass.NW: return new Compass[] { Compass.NE, Compass.W };
                case Compass.NE: return new Compass[] { Compass.NW, Compass.E };
                case Compass.SW: return new Compass[] { Compass.SE, Compass.W };
                case Compass.SE: return new Compass[] { Compass.SW, Compass.E };
                case Compass.C: return new Compass[] { };
                default: throw new Exception("Compass behaving eratically.");
            }
        }

        public static Vector2 Of(this Compass c, Vector2 v) => Of(c, (int)v.X, (int)v.Y);

        public static Vector2 Of(this Compass c, int i, int j)
        {
            switch (c)
            {
                case Compass.E: return new Vector2(i + 1, j);
                case Compass.W: return new Vector2(i - 1, j);
                case Compass.NE: return new Vector2(j % 2 + i, j - 1);
                case Compass.NW: return new Vector2(j % 2 + i - 1, j - 1);
                case Compass.SW: return new Vector2(j % 2 + i - 1, j + 1);
                case Compass.SE: return new Vector2(j % 2 + i, j + 1);
                case Compass.C: return new Vector2(i, j);
                default: throw new Exception("Compass behaving eratically.");
            }
        }

        public static Compass Inverse(this Compass c)
        {
            switch (c)
            {
                case Compass.E: return Compass.W;
                case Compass.W: return Compass.E;
                case Compass.NW: return Compass.SE;
                case Compass.NE: return Compass.SW;
                case Compass.SW: return Compass.NE;
                case Compass.SE: return Compass.NW;
                case Compass.C: return Compass.C;
                default: throw new Exception("Compass behaving eratically.");
            }
        }
    }
}
