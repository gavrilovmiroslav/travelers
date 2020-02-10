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

            List<Vector2> used = new List<Vector2>();
            List<Vector2> next = new List<Vector2>();

            for(int i = 0; i < 4; i++)
            {
                int di = ci + r.Next(-map.MapWidth / 2, map.MapWidth / 2);
                int dj = cj + r.Next(-map.MapHeight / 2, map.MapHeight / 2);

                foreach (var n in map.EmptyNeighbors(di, dj))
                {
                    var chanceToFail = HexMap.Distance(n, center) * 4;

                    if (r.Next(0, 100) > 20)
                        map.Put(n, "town");
                    else
                        next.Add(n);

                    if (r.Next(0, 100) > chanceToFail)
                        next.AddRange(map.EmptyNeighbors(n));
                }

                map.Put(di, dj, "city");
            }

            Shuffle(next);

            Queue<Vector2> nextUp = new Queue<Vector2>(next);

            while(nextUp.Count > 0)
            {
                Vector2 f = nextUp.Dequeue();
                if (map.fields[(int)f.X, (int)f.Y].biome != null) continue;

                var bs = map.BiomesInNeighbors(f);
                var viable = bs[r.Next(0, bs.Count)];
                var v = map.RandomBiomeAround(viable);
                map.Put(f, v);
                used.Add(f);

                var neighbors = map.EmptyNeighbors(f);
                foreach (var n in neighbors)
                {                    
                    var chanceToFail = HexMap.Distance(n, center) * 8;
                    if (r.Next(0, 100) > chanceToFail)
                    {
                        nextUp.Enqueue(n);
                    }
                }
            }

            foreach (var field in used)
            {
                var mf = map.fields[(int)field.X, (int)field.Y];
                if (mf.biome == "water" && 
                    map.BiomesInNeighbors(field).Contains("valley"))
                {
                    if(r.Next(0, 100) > 50)
                        map.Put(field, "moors");
                }
            }

            foreach (var field in used)
            {
                foreach (var n in map.Neighbors(field))
                {
                    if (map.fields[(int)n.X, (int)n.Y].biome == null)
                    {
                        var prob = r.Next(0, 10);
                        if (prob < 5) map.Put(n, "water");
                        else if (prob < 2) map.Put(n, "ocean");
                    }
                }
            }
        }
    }
}
