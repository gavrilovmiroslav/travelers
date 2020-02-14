using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Travelers
{
    public class Travelers : Game
    {
        SpriteFont font;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        HexMap map;

        Camera camera;
        Vector2 center = new Vector2(0, 0);

        private readonly int x = 80, y = 0, w = 160, h = 140;
        
        public Travelers()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();

            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 800;
            graphics.ApplyChanges();

            this.IsMouseVisible = true;
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            font = Content.Load<SpriteFont>("Samble");

            camera = new Camera(graphics.GraphicsDevice.Viewport);

            map = new HexMap(20, 20);
            map.Dimensions(80, 0, 160, 140);
            map.Blank("tile_ocean_plain_dark_");
            map.DefineBiomes("town", "city", "forest", "hills", "moors", "mountains", "valley", "water");

            map.TieBiomes("city", "town", 4);
            map.TieBiomes("city", "forest", 5);
            map.TieBiomes("city", "valley", 5);

            map.TieBiomes("moors", "moors", 5);
            map.TieBiomes("moors", "water", 5);

            map.TieBiomes("town", "valley", 3);
            map.TieBiomes("town", "town", 2);
            map.TieBiomes("town", "hills", 3);
            map.TieBiomes("town", "water", 2);
            map.TieBiomes("town", "forest", 3);

            map.TieBiomes("forest", "town", 2);
            map.TieBiomes("forest", "forest", 3);
            map.TieBiomes("forest", "hills", 3);
            map.TieBiomes("forest", "mountains", 3);
            map.TieBiomes("forest", "valley", 1);

            map.TieBiomes("hills", "valley", 5);
            map.TieBiomes("hills", "forest", 2);
            map.TieBiomes("hills", "mountains", 2);
            map.TieBiomes("hills", "town", 1);

            map.TieBiomes("mountains", "hills", 5);
            map.TieBiomes("mountains", "forest", 5);

            map.TieBiomes("valley", "valley", 2);
            map.TieBiomes("valley", "hills", 2);
            map.TieBiomes("valley", "mountains", 2);
            map.TieBiomes("valley", "forest", 4);
            map.TieBiomes("valley", "town", 2);

            map.TieBiomes("water", "water", 5);
            map.TieBiomes("water", "mountains", 4);
            map.TieBiomes("water", "valley", 1);

            map.Add("city_clear", "town", "tile_city_clear_green_", 0, 9);
            map.Add("city_covered", "city", "tile_city_covered_green_", 0, 9);
            map.Add("forest_conifer_dense_clear", "forest", "tile_forest_conifer_dense_clear_green_", 0, 9);
            map.Add("forest_conifer_dense_covered", "forest", "tile_forest_conifer_dense_covered_green_", 0, 9);
            map.Add("forest_conifer_sparse_clear", "forest", "tile_forest_conifer_sparse_clear_green_", 0, 9);
            map.Add("forest_conifer_sparse_covered", "forest", "tile_forest_conifer_sparse_covered_green_", 0, 9);
            map.Add("forest_deciduous_dense_clear", "forest", "tile_forest_deciduous_dense_clear_green_", 0, 9);
            map.Add("forest_deciduous_dense_covered", "forest", "tile_forest_deciduous_dense_covered_green_", 0, 9);
            map.Add("forest_deciduous_sparse_clear", "forest", "tile_forest_deciduous_sparse_clear_green_", 0, 9);
            map.Add("forest_deciduous_sparse_covered", "forest", "tile_forest_deciduous_sparse_covered_green_", 0, 9);
            map.Add("hills_dense_clear", "hills", "tile_hills_dense_clear_green_", 0, 9);
            map.Add("hills_sparse_covered", "hills", "tile_hills_sparse_covered_green_", 0, 9);
            map.Add("mountain_peak_forest_conifer", "mountains", "tile_mountain_peak_forest_conifer_green_", 0, 9);
            map.Add("mountain_peak_forest_deciduous", "mountains", "tile_mountain_peak_forest_deciduous_green_", 0, 9);
            map.Add("mountain_peak_grassland_green", "mountains", "tile_mountain_peak_grassland_green_", 0, 9);
            map.Add("valley_dense_clear", "valley", "tile_valley_dense_clear_green_", 0, 9);
            map.Add("valley_sparse_clear", "valley", "tile_valley_sparse_clear_green_", 0, 9);
            map.Add("ocean_waves_small_dark", "water", "tile_ocean_waves_small_dark_", 0, 9);
            map.Add("ocean_waves_big_dark", "water", "tile_ocean_waves_big_dark_", 0, 9);
            map.Add("moor_sparse_covered", "moors", "tile_moor_sparse_covered_blue_", 0, 9);
            map.Add("moor_dense_clear", "moors", "tile_moor_dense_clear_blue_", 0, 9);

            map.AddLocations("overlay_location_framed_uncut_standard", "battlefield", "bridge", "camp", "campfire", "castle", "cave", "church", "city", "city_house", "dungeon",
                "farm", "graveyard", "house", "inn", "lighthouse", "military", "mine", "nest", "outpost", "quarry", "ruin", "sanctuary", "ship", "shipwreck", "spring", "tent",
                "tower", "village", "windmill", "witch_hut");

            map.AddCharacters("strategy_figure_180x180_framed_standard", "adventurer", "adventurer_with_horse", "archer", "armored_horse", "balloon", "bandit", "bandits", "barbarian",
                "bat", "bear", "boar", "bull", "carriage", "cart", "cat", "chicken", "cow", "crow", "cyclops", "dog", "donkey", "dragon", "druid", "elder", "farmer", "fen_fire", "frog",
                "ghost", "goblin", "goblins", "hero", "hero_with_horse", "horse", "knight", "knight_with_horse", "kraken", "livestock", "mage", "miner", "mummy", "orc", "orcs", "paladin",
                "paladin_with_horse", "party", "people", "pig", "pikeman", "priest", "rabbit", "rat", "rogue", "saddled_donkey", "saddled_horse", "sheep", "skeleton", "skeleton_archer",
                "skeleton_warrior", "skeletons", "slime", "snake", "soldiers", "spider", "stag", "trader", "trader_with_donkey", "trader_with_wagon", "troll", "turkey", "villager", "wagon",
                "warrior", "werewolf", "witch", "wolf", "worker", "workers", "yeti", "zombie", "zombies");

            map.player = new Character(Content);
            map.Load(Content);

            Creator.Island(ref map);

            center.X = 1600;
            center.Y = 1400;
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.A)) center.X -= 5 / camera.zoom;
            if (Keyboard.GetState().IsKeyDown(Keys.D)) center.X += 5 / camera.zoom;
            if (Keyboard.GetState().IsKeyDown(Keys.W)) center.Y -= 5 / camera.zoom;
            if (Keyboard.GetState().IsKeyDown(Keys.S)) center.Y += 5 / camera.zoom;
            
            if (Keyboard.GetState().IsKeyDown(Keys.Q))
            {
                camera.zoom -= 0.01f;
                if (camera.zoom < 0.25f) camera.zoom = 0.25f;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.E))
            {
                camera.zoom += 0.01f;
                if (camera.zoom > 2f) camera.zoom = 2f;
            }

            base.Update(gameTime);
            camera.Update(center);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(new Color(105, 105, 108));

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, camera.Transform);            
            map.Draw(spriteBatch, font);
            spriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}
