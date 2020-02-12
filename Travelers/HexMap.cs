using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Travelers
{
    public class PaintedTile : ICloneable
    {
        public string biome;
        public string family;
        public Texture2D texture;
        public float tick;
        public float timer;
        public char symbol = ' ';        
        public Color color = Color.White;

        public object Clone()
        {
            return new PaintedTile()
            {
                biome = biome,
                family = family,
                texture = texture,
                tick = tick,
                timer = timer,
                symbol = symbol,                
                color = color
            };
        }
    }

    public struct Tile
    {
        public int min;
        public int max;
        public string filename;
        public string biome;
    }

    public class HexMap
    {
        public PaintedTile[,] fields;
        public int cx, cy;
        Dictionary<string, PaintedTile[]> textures;

        public HexMap() {}

        public int TileX { get; private set; }
        public int TileY { get; private set; }
        public int TileWidth { get; private set; }
        public int TileHeight { get; private set; }
        public int MapWidth { get; private set; }
        public int MapHeight { get; private set; }

        public List<string> Region = new List<string>();
        public Dictionary<string, List<string>> BiomesPerRegion = new Dictionary<string, List<string>>();
        public Dictionary<string, Tile> TilePerBiome = new Dictionary<string, Tile>();

        public List<Vector2> Towns = new List<Vector2>();
        public Dictionary<char, List<Vector2>> TownFields = new Dictionary<char, List<Vector2>>();
        public Dictionary<string, Dictionary<string, int>> BiomeTies = new Dictionary<string, Dictionary<string, int>>();
        public Dictionary<string, PathClass> PathClasses = new Dictionary<string, PathClass>();
        public Dictionary<string, List<Path>> Paths = new Dictionary<string, List<Path>>();
        
        public void TieBiomes(string b1, string b2, int prob)
        {
            if (!BiomeTies.ContainsKey(b1))
                BiomeTies.Add(b1, new Dictionary<string, int>());

            if (!BiomeTies[b1].ContainsKey(b2))
                BiomeTies[b1].Add(b2, 0);

            BiomeTies[b1][b2] += prob;
        }

        public string RandomBiomeAround(string b)
        {
            if (b == null) b = "town";
            List<string> choice = new List<string>();
            if (!BiomeTies.ContainsKey(b)) return b;
            foreach(var v in BiomeTies[b])
            {
                for (int i = 0; i < v.Value; i++) choice.Add(v.Key);
            }

            return choice[r.Next(0, choice.Count)];
        }

        public Random r = new Random();

        public HexMap(int w, int h)
        {
            MapWidth = w;
            MapHeight = h;
            fields = new PaintedTile[w, h];
        }

        public void Dimensions(int x, int y, int w, int h)
        {
            TileX = x;
            TileY = y;
            TileWidth = w;
            TileHeight = h;
        }

        public Vector2 XY(int i, int j) => new Vector2(j % 2 * TileX + i * TileWidth, TileY + j * TileHeight);

        public PaintedTile this[Vector2 v]
        {
            get
            {
                return this.fields[(int)v.X, (int)v.Y];
            }
        }

        public PaintedTile this[float x, float y]
        {
            get
            {
                return this.fields[(int)x, (int)y];
            }
        }

        public void Blank(string filename)
        {
            TilePerBiome.Add("_", new Tile() { filename = filename, min = 0, max = 1, biome = null });            
        }

        public void Add(string familyName, string biomeName, string fileName, int minRange, int maxRange)
        {
            TilePerBiome.Add(familyName, new Tile() { filename = fileName, min = minRange, max = maxRange, biome = biomeName });

            if(!BiomesPerRegion.ContainsKey(biomeName))
                BiomesPerRegion.Add(biomeName, new List<string>());

            BiomesPerRegion[biomeName].Add(familyName);
        }

        public void DefineBiomes(params string[] biomes)
        {
            this.Region.AddRange(biomes);            
        }

        public void AddPathClass(string pathName, PathClass path)
        {
            this.PathClasses.Add(pathName, path);
        }

        public void AddPath(string pathName, Path path)
        {
            if (!this.Paths.ContainsKey(pathName))
                this.Paths[pathName] = new List<Path>();

            this.Paths[pathName].Add(path);
        }

        public void Load(ContentManager Content)
        {
            Random r = new Random();
            Dictionary<string, PaintedTile[]> biomes = new Dictionary<string, PaintedTile[]>();
            foreach (var tile in TilePerBiome)
            {
                var textures = new PaintedTile[tile.Value.max - tile.Value.min];
                for (int i = tile.Value.min; i < tile.Value.max; i++)
                {
                    textures[i] = new PaintedTile() {
                        texture = Content.Load<Texture2D>($"{tile.Value.filename}{i}"),
                        biome = tile.Value.biome,
                        family = tile.Key
                    };

                    textures[i].tick = 360;
                    textures[i].timer = r.Next(0, 360);   
                }

                biomes.Add(tile.Key, textures);
            }

            textures = biomes;
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            // draw map
            for (int i = 0; i < MapWidth; i++)
                for (int j = 0; j < MapHeight; j++)
                {
                    PaintedTile tex = fields[i, j];
                    if (fields[i, j] == null) tex = textures["_"][0];

                    if (tex.timer >= tex.tick)
                    {
                        tex.timer = 0;
                    }

                    var scale = Vector2.One;

                    var dt = 0.0f;
                    var st = 0.0f;

                    if (tex.biome == "water")
                    {
                        dt = 1f; st = 0.03f;

                        if (tex.timer == 1)
                        {
                            Put(i, j, tex.biome);
                        }
                    }
                    else if (tex.biome == "moors")
                    {
                        dt = 0.33f; st = 0.05f;
                    }
                    else if (tex.biome == "forest")
                    {
                        dt = 0.33f; st = 0.02f;
                    }

                    tex.timer += dt;
                    float s = 1.0f + (float)Math.Sin(tex.timer * Math.PI / 180.0f) * st;
                    scale *= s;

                    var origin = new Vector2(tex.texture.Width / 2, tex.texture.Height / 2);
                    spriteBatch.Draw(tex.texture, XY(i, j), null, tex.color, 0, origin, scale, SpriteEffects.None, 0);

                    spriteBatch.DrawString(font, $"{tex.symbol}", XY(i, j), Color.White, 0, font.MeasureString($"{tex.symbol}") / 2, 1, SpriteEffects.None, 1);
                }

            // draw things

            foreach(var ps in Paths)
                foreach (var p in ps.Value)
                    p.Draw(spriteBatch);
        }

        public Vector2? HexAt(Vector2 tv)
        {
            int j = (int) ((tv.Y - TileY) / TileHeight + 0.5f);
            int i = (int) ((tv.X - (j % 2 * TileX)) / TileWidth + 0.5f);

            if (i < 0 || j < 0 || i >= MapWidth || j >= MapHeight)
                return null;

            return new Vector2(i, j);
        }

        public List<Vector2> NeighborsThatAre(Vector2 f, string name) => NeighborsThatAre((int)f.X, (int)f.Y, name);

        public List<Vector2> NeighborsThatAre(int i, int j, string name)
        {
            return Neighbors(i, j).FindAll(t => this[t].biome == name);
        }

        public List<Vector2> NeighborsExcept(Vector2 f, string name) => NeighborsExcept((int)f.X, (int)f.Y, name);

        public List<Vector2> NeighborsExcept(int i, int j, string name)
        {
            return Neighbors(i, j).FindAll(t => this[t].biome != name);
        }

        public List<Vector2> Neighbors(Vector2 f) => Neighbors((int)f.X, (int)f.Y);

        public List<Vector2> Neighbors(int i, int j)
        {
            var list = new List<Vector2>() {
                new Vector2(i - 1, j),
                new Vector2(i + 1, j),
                new Vector2(j % 2 + i - 1, j - 1),
                new Vector2(j % 2 + i - 1, j + 1),
                new Vector2(j % 2 + i, j - 1),
                new Vector2(j % 2 + i, j + 1),
            };

            return list.FindAll(t => t.X >= 0 && t.X < this.MapWidth && t.Y >= 0 && t.Y < this.MapHeight);
        }

        public List<Vector2> EmptyNeighbors(Vector2 f) => EmptyNeighbors((int)f.X, (int)f.Y);

        public List<Vector2> EmptyNeighbors(int i, int j)
        {
            var ns = Neighbors(i, j);
            var es = ns.FindAll(t => this[t].biome == null);

            return es;
        }

        public List<string> AllBiomesInNeighbors(Vector2 f) => AllBiomesInNeighbors((int)f.X, (int)f.Y);

        public List<string> AllBiomesInNeighbors(int i, int j)
        {
            return Neighbors(i, j).Select(t => this[t].biome).ToList();
        }

        public List<string> BiomesInNeighbors(Vector2 f) => BiomesInNeighbors((int)f.X, (int)f.Y);

        public List<string> BiomesInNeighbors(int i, int j)
        {
            return Neighbors(i, j).Select(t => this[t].biome).Distinct().ToList();
        }

        internal void Put(Vector2 n, string reg) => Put((int)n.X, (int)n.Y, reg);

        internal void Put(int i, int j, string reg)
        {
            if (i < 0 || j < 0 || i >= MapWidth || j >= MapHeight) return;

            if (reg == "_")
            {
                fields[i, j] = textures["_"][0];
            }
            else
            {
                var biomes = BiomesPerRegion[reg];
                var v = biomes[r.Next(0, biomes.Count)];
                var ts = textures[v];
                fields[i, j] = (PaintedTile)ts[r.Next(0, ts.Length)].Clone();

                if (reg == "town" || reg == "city")
                {
                    Towns.Add(new Vector2(i, j));
                }
            }
        }

        internal List<Vector2> GetAll(string reg)
        {
            List<Vector2> result = new List<Vector2>();
            for (int i = 0; i < MapWidth; i++)
                for (int j = 0; j < MapHeight; j++)
                    if (fields[i, j].biome == reg) result.Add(new Vector2(i, j));

            return result;
        }


        internal List<Vector2> GetAllNot(string reg)
        {
            List<Vector2> result = new List<Vector2>();
            for (int i = 0; i < MapWidth; i++)
                for (int j = 0; j < MapHeight; j++)
                    if (fields[i, j].biome != reg && fields[i, j].biome != null) result.Add(new Vector2(i, j));

            return result;
        }

        internal void Fill(string v)
        {
            var ts = textures[v];
            for (int i = 0; i < MapWidth; i++)
                for (int j = 0; j < MapHeight; j++)
                {
                    fields[i, j] = ts[r.Next(0, ts.Length)];
                }
        }

        public static double Distance(Vector2 a, Vector2 b)
        {
            var dx = (b.X - a.X);
            var dy = (b.Y - a.Y);
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
