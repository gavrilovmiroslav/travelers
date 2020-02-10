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
        private static Random rng = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

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

            Shuffle(next);

            Queue<Vector2> nextUp = new Queue<Vector2>(next);

            while(nextUp.Count > 0)
            {
                Vector2 f = nextUp.Dequeue();
                var bs = map.BiomesInNeighbors(f);
                Console.WriteLine(bs[r.Next(0, bs.Count)]);
            }
        }
    }
}
