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
    public class PaintedTile
    {
        public string biome;
        public string family;
        public Texture2D texture;
        public float tick;
        public float timer;
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

        public Dictionary<string, Dictionary<string, int>> biomeTies = new Dictionary<string, Dictionary<string, int>>();
        
        public void TieBiomes(string b1, string b2, int prob)
        {
            if (!biomeTies.ContainsKey(b1))
                biomeTies.Add(b1, new Dictionary<string, int>());

            if (!biomeTies[b1].ContainsKey(b2))
                biomeTies[b1].Add(b2, 0);

            biomeTies[b1][b2] += prob;
        }

        public string RandomBiomeAround(string b)
        {
            if (b == null) b = "town";
            List<string> choice = new List<string>();
            if (!biomeTies.ContainsKey(b)) return b;
            foreach(var v in biomeTies[b])
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

                    if(tile.Value.biome == "water" || tile.Value.biome == "ocean")
                    {
                        textures[i].tick = r.Next(60, 120);
                        textures[i].timer = 0;
                    }
                    else
                    {
                        textures[i].tick = 360;
                        textures[i].timer = r.Next(0, 360);
                    }
                }
                biomes.Add(tile.Key, textures);
            }

            textures = biomes;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (int i = 0; i < MapWidth; i++)
                for (int j = 0; j < MapHeight; j++)
                {
                    PaintedTile tex = fields[i, j];
                    if (fields[i, j] == null) tex = textures["_"][0];

                    if (tex.timer >= tex.tick)
                    {
                        tex.timer = 0;
                    }

                    if (tex.biome == "water" || tex.biome == "ocean")
                    {
                        tex.timer++;

                        if (tex.timer == 1)
                        {
                            tex.tick = r.Next(60, 120);
                            Put(i, j, tex.biome);
                        }
                    }

                    var scale = Vector2.One;
                    if (tex.biome != "water" && tex.biome != "ocean")
                    {
                        tex.timer += 0.25f;
                        float s = 1.0f + (float)Math.Sin(tex.timer * Math.PI / 180.0f) * 0.01f;
                        scale *= s;
                    }

                    var origin = new Vector2(tex.texture.Width / 2, tex.texture.Height / 2);
                    if (j % 2 == 0)
                    {
                        var xy = new Vector2(i * TileWidth, j * TileHeight);
                        spriteBatch.Draw(tex.texture, xy, null, Color.White, 0, origin, scale, SpriteEffects.None, 0);
                    }
                    else
                    {
                        var xy = new Vector2(TileX + i * TileWidth, TileY + j * TileHeight);
                        spriteBatch.Draw(tex.texture, xy, null, Color.White, 0, origin, scale, SpriteEffects.None, 0);
                    }
                }
        }

        public List<Vector2> Neighbors(Vector2 f) => Neighbors((int)f.X, (int)f.Y);

        public List<Vector2> Neighbors(int i, int j)
        {
            var list = new List<Vector2>() {
                new Vector2(i - 1, j),
                new Vector2(i + 1, j),
                new Vector2(i - 1, j - 1),
                new Vector2(i - 1, j + 1),
                new Vector2(i, j - 1),
                new Vector2(i, j + 1),
            };

            return list.FindAll(t => t.X >= 0 && t.X < this.MapWidth && t.Y >= 0 && t.Y < this.MapHeight);
        }

        public List<Vector2> EmptyNeighbors(Vector2 f) => EmptyNeighbors((int)f.X, (int)f.Y);

        public List<Vector2> EmptyNeighbors(int i, int j)
        {
            var ns = Neighbors(i, j);
            var es = ns.FindAll(t => this.fields[(int)t.X, (int)t.Y].biome == null);

            return es;
        }

        public List<string> AllBiomesInNeighbors(Vector2 f) => AllBiomesInNeighbors((int)f.X, (int)f.Y);

        public List<string> AllBiomesInNeighbors(int i, int j)
        {
            return Neighbors(i, j).Select(t => this.fields[(int)t.X, (int)t.Y].biome).ToList();
        }

        public List<string> BiomesInNeighbors(Vector2 f) => BiomesInNeighbors((int)f.X, (int)f.Y);

        public List<string> BiomesInNeighbors(int i, int j)
        {
            return Neighbors(i, j).Select(t => this.fields[(int)t.X, (int)t.Y].biome).Distinct().ToList();
        }

        internal void Put(Vector2 n, string reg) => Put((int)n.X, (int)n.Y, reg);

        internal void Put(int i, int j, string reg)
        {
            if (i < 0 || j < 0 || i >= MapWidth || j >= MapHeight) return;

            var biomes = BiomesPerRegion[reg];
            var v = biomes[r.Next(0, biomes.Count)];
            var ts = textures[v];
            fields[i, j] = ts[r.Next(0, ts.Length)];
        }

        internal List<Vector2> GetAll(string reg)
        {
            List<Vector2> result = new List<Vector2>();
            for (int i = 0; i < MapWidth; i++)
                for (int j = 0; j < MapHeight; j++)
                    if (fields[i, j].biome == reg) result.Add(new Vector2(i, j));

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
