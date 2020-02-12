using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travelers
{
    public static class ArrayExtensions
    {
        public static T FromArray<T>(this Random rng, IList<T> arr)
        {
            return arr[rng.Next(0, arr.Count())];
        }
    }

    public static class Creator
    {
        private static string[] Names =
        {
            "Elfacre",
            "Brightmill",
            "Bypine",
            "Starryfox",
            "Barrowmeadow",
            "Ashbridge",
            "Swynpond",
            "Wintermill",
            "Eribourne",
            "Bridgebeach",
            "Roselyn",
            "Summerwinter",
            "Fairviolet",
            "Ashvale",
            "Dordale",
            "Osthaven",
            "Deephaven",
            "Whiteflower",
            "Welledge",
            "Snowbeach",
            "Marblenesse",
            "Witchnesse",
            "Bluewell",
            "Shorelake",
            "Coldfalcon",
            "Strongbush",
            "Freyholt",
            "Oldtown",
        };

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

        static int Ord(Vector2 v1, Vector2 v2)
        {
            if (v1.X >= 0)
            {
                if (v2.X < 0)
                {
                    return -1;
                }
                return -Comparer<float>.Default.Compare(v1.Y, v2.Y);
            }
            else
            {
                if (v2.X >= 0)
                {
                    return 1;
                }
                return Comparer<float>.Default.Compare(v1.Y, v2.Y);
            }
        }

        public static void Island(ref HexMap map)
        {
            map.Fill("_");

            int ci = map.MapWidth / 2;
            int cj = map.MapHeight / 2;
            Vector2 center = new Vector2(ci, cj);

            List<Vector2> used = new List<Vector2>();
            List<Vector2> next = new List<Vector2>();

            for (int i = 0; i < 4; i++)
            {
                int di = ci + rng.Next(-map.MapWidth / 2, map.MapWidth / 2);
                int dj = cj + rng.Next(-map.MapHeight / 2, map.MapHeight / 2);

                foreach (var n in map.EmptyNeighbors(di, dj))
                {
                    var chanceToFail = HexMap.Distance(n, center) * 4;

                    if (rng.Next(0, 100) > 20)
                        map.Put(n, "town");
                    else
                        next.Add(n);

                    if (rng.Next(0, 100) > chanceToFail)
                        next.AddRange(map.EmptyNeighbors(n));
                }

                map.Put(di, dj, "city");
            }

            Shuffle(next);

            Queue<Vector2> nextUp = new Queue<Vector2>(next);

            while (nextUp.Count > 0)
            {
                Vector2 f = nextUp.Dequeue();
                if (map[f].biome != null) continue;

                var bs = map.BiomesInNeighbors(f);
                var viable = bs[rng.Next(0, bs.Count)];
                var v = map.RandomBiomeAround(viable);
                map.Put(f, v);
                used.Add(f);

                var neighbors = map.EmptyNeighbors(f);
                foreach (var n in neighbors)
                {
                    var chanceToFail = HexMap.Distance(n, center) * 8;
                    if (rng.Next(0, 100) > chanceToFail)
                    {
                        nextUp.Enqueue(n);
                    }
                }
            }

            // moors and waters

            foreach (var field in used)
            {
                var mf = map[field];
                if ((mf.biome == "water") &&
                    map.BiomesInNeighbors(field).Contains("valley"))
                {
                    if (rng.Next(0, 100) > 50)
                        map.Put(field, "moors");
                }
            }

            foreach (var field in used)
            {
                foreach (var n in map.Neighbors(field))
                {
                    if (map[n].biome == null)
                    {
                        var prob = rng.Next(0, 10);
                        if (prob < 5) map.Put(n, "water");
                    }
                }
            }

            // cities

            char sym = '@';

            foreach (var t in map.Towns)
                map[t].symbol = ++sym;

            var processing = true;
            while (processing)
            {
                processing = false;
                foreach (var t in map.Towns)
                {
                    var tf = map[t];

                    foreach (var n in map.Neighbors(t))
                    {
                        var nf = map[n];
                        if (nf.biome == "town" || nf.biome == "city")
                            if (tf.symbol < nf.symbol)
                            {
                                nf.symbol = tf.symbol;
                                processing = true;
                            }
                    }
                }
            }

            foreach (var t in map.Towns)
            {
                var tf = map[t];
                if (!map.TownFields.ContainsKey(tf.symbol))
                    map.TownFields.Add(tf.symbol, new List<Vector2>());

                map.TownFields[tf.symbol].Add(t);
            }

            Dictionary<char, List<Vector2>> tmpMap = new Dictionary<char, List<Vector2>>();

            {
                char i = '@';
                foreach (var p in map.TownFields)
                {
                    i++;
                    if (!tmpMap.ContainsKey(i))
                        tmpMap.Add(i, new List<Vector2>());

                    foreach (var f in p.Value)
                    {
                        tmpMap[i].Add(f);
                        map[f].symbol = i;
                    }
                }

                map.TownFields = tmpMap;
            }

            foreach (var p in map.TownFields)
            {
                var index = p.Key - 65;
            }

            // rivers

            var start = map.GetAllNot("water");

            Dictionary<Vector2, bool> usedRiver = new Dictionary<Vector2, bool>();

            map.Paths["rivers"] = new List<Path>();

            while (map.Paths["rivers"].Count < 4)
            { 
                Path river = new Path(map.PathClasses["rivers"], map);

                var m = start[rng.Next(0, start.Count)];

                usedRiver[m] = true;

                Compass? c = Compass.C;

                int attempt = 0;
                while(attempt < 10)
                {
                    attempt++;
                    KeyValuePair<Compass, Texture2D> to;
                    do
                    {
                        to = map.PathClasses["rivers"].GetRandom(c);
                    } while (to.Key == c);

                    river.Add(m, to.Value);
                    var n = to.Key.Of(m);

                    if (usedRiver.ContainsKey(n)) continue;

                    if (to.Key == Compass.C)
                        if (rng.Next(0, 50) < 25)
                            continue;
                        else break;

                    if (n.X >= map.MapWidth || n.Y >= map.MapHeight || n.X < 0 || n.Y < 0)
                        break;

                    if (map[n].biome == "water" || map[n].biome == "ocean" || map[n].biome == "town" || map[n].biome == "city")
                        break;

                    if (map[n].biome == "_" || map[n].biome == null)
                        break;

                    if (m == n)
                        break;

                    c = to.Key;
                    usedRiver[n] = true;
                    m = n;
                }

                if (river.pieces.Count > 4)
                    map.AddPath("rivers", river);
            }

            // roads

            const double max = 10000;
            {
                var traveler = new Character(map);
                var towns = map.TownFields.Keys.ToArray();
                char starting = rng.FromArray(towns);
                char ending;
                do
                {
                    ending = rng.FromArray(towns);
                } while (starting == ending);

                var xys = rng.FromArray(map.TownFields[starting]);
                var xyd = rng.FromArray(map.TownFields[ending]);

                map[xys].color = Color.Red;
                map[xyd].color = Color.Green;

                Dictionary<Vector2, double> optDist = new Dictionary<Vector2, double>();
                Queue<Vector2> opts = new Queue<Vector2>();
                opts.Enqueue(xys);

                while(opts.Count > 0)
                {
                    var o = opts.Dequeue();
                    var bo = map[o].biome;

                    if (optDist.ContainsKey(o)) continue;

                    optDist[o] = HexMap.Distance(o, xyd);
                    if (bo == null || bo == "_" || bo == "water" || bo == "ocean")
                        optDist[o] = max;
                    else if (bo == "moors" || bo == "mountains")
                        optDist[o] *= 2;
                    else if (bo == "hills" || bo == "forest" || bo == "city")
                        optDist[o] *= 1.15;

                    foreach (var n in map.Neighbors(o))
                    {
                        var b = map[n].biome;
                        if (b != null && b != "_" && b != "water" && b != "ocean")
                            opts.Enqueue(n);
                    }

                    if (optDist.ContainsKey(xyd)) break;
                }

                for (int i = 0; i < map.MapWidth; i++)
                    for (int j = 0; j < map.MapHeight; j++)
                    {
                        var no = new Vector2(i, j);
                        if (optDist.ContainsKey(no)) map.diag[i, j] = optDist[no];
                        else map.diag[i, j] = max;
                    }

                var path = new Path(map.PathClasses["paths"], map);
                var current = xys;
                var dir = Compass.C;

                while (current != xyd)
                {
                    double min = max;

                    var cand = current;
                    foreach(var nx in map.Neighbors(current))
                    {
                        if (!optDist.ContainsKey(nx))
                            optDist[nx] = 1000;

                        var onx = optDist[nx];
                        if (onx < min)
                        {
                            cand = nx;
                            min = onx;
                            optDist[nx] *= 2;
                        }
                    }

                    if (cand == current) break;
                    
                    map[cand].color = Color.Purple;
                    Console.WriteLine(current.Direction(cand));

                    var cat = map.PathClasses["paths"].PathPieces[dir][current.Direction(cand)];
                    path.Add(current, rng.FromArray(cat));

                    current = cand;
                    dir = current.Direction(cand).Inverse();
                }

                map.AddPath("paths", path);
            }
        }
    }
}
