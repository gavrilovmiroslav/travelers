using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travelers
{
    public class PathClass
    {
        private Random r = new Random();

        public readonly Dictionary<Compass, Dictionary<Compass, List<Texture2D>>> PathPieces
            = new Dictionary<Compass, Dictionary<Compass, List<Texture2D>>>();

        private ContentManager Content;

        public string FamilyName;

        public PathClass(ContentManager c)
        {
            Content = c;
        }

        public void Add(Compass s, Compass d, string filename, int max)
        {
            if (!PathPieces.ContainsKey(s))
                PathPieces[s] = new Dictionary<Compass, List<Texture2D>>();

            if (!PathPieces.ContainsKey(d))
                PathPieces[d] = new Dictionary<Compass, List<Texture2D>>();

            if (!PathPieces[s].ContainsKey(d))
                PathPieces[s][d] = new List<Texture2D>();

            if (!PathPieces[d].ContainsKey(s))
                PathPieces[d][s] = new List<Texture2D>();

            for (int i = 0; i < max; i++)
            {
                var n = i > 0 ? $"_{i + 1}" : "";
                var t = Content.Load<Texture2D>($"{filename}_{s.ToString().ToLower()}_{d.ToString().ToLower()}{n}");

                PathPieces[s][d].Add(t);
                PathPieces[d][s].Add(t);
            }
        }

        public KeyValuePair<Compass, Texture2D> GetRandom(Compass? fromn)
        {
            Compass from = Compass.C;

            if (fromn.HasValue)
                from = fromn.Value;
            else
            {
                var ens = Enum.GetValues(typeof(Compass));
                from = (Compass)ens.GetValue(new Random().Next(ens.Length));
            }

            var inv = from.Inverse();

            if (from == Compass.C)
            {
                var tos = PathPieces[inv];
                var keys = tos.Keys.ToArray();

                var k = keys[r.Next(0, keys.Length)];
                var paths = tos[k];
                return new KeyValuePair<Compass, Texture2D>(k, paths[r.Next(0, paths.Count)]);
            }
            else
            {
                var flow = r.Next(0, 11);
                if(flow < 3)
                {
                    var paths = PathPieces[inv][from];
                    return new KeyValuePair<Compass, Texture2D>(inv, paths[r.Next(0, paths.Count)]);
                }
                else
                {
                    var neighbors = from.Neighbors();
                    var newflow = neighbors[r.Next(0, neighbors.Length)];
                    var paths = PathPieces[inv][newflow];
                    return new KeyValuePair<Compass, Texture2D>(newflow, paths[r.Next(0, paths.Count)]);
                }
            }
        }
    }

    public class Path
    {
        PathClass pathClass;
        HexMap map;

        public Dictionary<Vector2, Texture2D> pieces = new Dictionary<Vector2, Texture2D>();

        public Path(PathClass klass, HexMap map)
        {
            pathClass = klass;
            this.map = map;
        }

        public void Add(Vector2 v, Texture2D t) => pieces[v] = t;

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach(var kv in pieces)
            {
                var xy = kv.Key;
                var tex = kv.Value;

                var origin = new Vector2(tex.Width / 2, tex.Height / 2);
                spriteBatch.Draw(tex, map.XY((int)xy.X, (int)xy.Y), null, Color.White, 0, origin, new Vector2(1, 1), SpriteEffects.None, 0);
            }
        }
    }
}
