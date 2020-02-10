using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travelers
{
    public static class Creator
    {
        public static void Island(ref HexMap map)
        {
            Random r = new Random();
            map.Fill("_");

            int ci = map.MapWidth / 2; 
            int cj = map.MapHeight / 2;
            Vector2 center = new Vector2(ci, cj);

            List<Vector2> next = new List<Vector2>();

            for(int i = 0; i < 4; i++)
            {
                int di = ci + r.Next(-map.MapWidth / 2, map.MapWidth / 2);
                int dj = cj + r.Next(-map.MapHeight / 2, map.MapHeight / 2);

                foreach (var n in map.EmptyNeighbors(di, dj))
                {
                    var chanceToFail = HexMap.Distance(n, center) * 3;
                    Console.WriteLine(chanceToFail);
                    map.Put(n, "town");

                    if (r.Next(0, 100) > chanceToFail)
                        next.Add(n);

                }

                map.Put(di, dj, "city");
            }
        }
    }
}
