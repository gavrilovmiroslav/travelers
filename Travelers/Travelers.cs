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

            var rivers = new PathClass(Content) { FamilyName = "rivers" };
            rivers.Add(Compass.C, Compass.E, "overlay_river", 2);
            rivers.Add(Compass.C, Compass.W, "overlay_river", 2);
            rivers.Add(Compass.C, Compass.NE, "overlay_river", 3);
            rivers.Add(Compass.C, Compass.NW, "overlay_river", 3);
            rivers.Add(Compass.C, Compass.SE, "overlay_river", 3);
            rivers.Add(Compass.C, Compass.SW, "overlay_river", 3);
            rivers.Add(Compass.E, Compass.SE, "overlay_river", 2);
            rivers.Add(Compass.E, Compass.SW, "overlay_river", 2);
            rivers.Add(Compass.E, Compass.W, "overlay_river", 3);
            rivers.Add(Compass.NE, Compass.E, "overlay_river", 3);
            rivers.Add(Compass.NE, Compass.SE, "overlay_river", 3);
            rivers.Add(Compass.NE, Compass.SW, "overlay_river", 3);
            rivers.Add(Compass.NE, Compass.W, "overlay_river", 3);
            rivers.Add(Compass.NW, Compass.E, "overlay_river", 3);
            rivers.Add(Compass.NW, Compass.NE, "overlay_river", 2);
            rivers.Add(Compass.NW, Compass.SE, "overlay_river", 3);
            rivers.Add(Compass.NW, Compass.SW, "overlay_river", 3);
            rivers.Add(Compass.NW, Compass.W, "overlay_river", 3);
            rivers.Add(Compass.SE, Compass.SW, "overlay_river", 2);
            rivers.Add(Compass.SE, Compass.W, "overlay_river", 2);
            rivers.Add(Compass.SW, Compass.W, "overlay_river", 2);
            map.AddPathClass("rivers", rivers);

            var paths = new PathClass(Content) { FamilyName = "paths" };
            paths.Add(Compass.C, Compass.E, "overlay_path", 2);
            paths.Add(Compass.C, Compass.W, "overlay_path", 2);
            paths.Add(Compass.C, Compass.NE, "overlay_path", 3);
            paths.Add(Compass.C, Compass.NW, "overlay_path", 3);
            paths.Add(Compass.C, Compass.SE, "overlay_path", 3);
            paths.Add(Compass.C, Compass.SW, "overlay_path", 3);
            paths.Add(Compass.E, Compass.SE, "overlay_path", 2);
            paths.Add(Compass.E, Compass.SW, "overlay_path", 2);
            paths.Add(Compass.E, Compass.W, "overlay_path", 3);
            paths.Add(Compass.NE, Compass.E, "overlay_path", 3);
            paths.Add(Compass.NE, Compass.SE, "overlay_path", 3);
            paths.Add(Compass.NE, Compass.SW, "overlay_path", 3);
            paths.Add(Compass.NE, Compass.W, "overlay_path", 3);
            paths.Add(Compass.NW, Compass.E, "overlay_path", 3);
            paths.Add(Compass.NW, Compass.NE, "overlay_path", 2);
            paths.Add(Compass.NW, Compass.SE, "overlay_path", 3);
            paths.Add(Compass.NW, Compass.SW, "overlay_path", 3);
            paths.Add(Compass.NW, Compass.W, "overlay_path", 3);
            paths.Add(Compass.SE, Compass.SW, "overlay_path", 2);
            paths.Add(Compass.SE, Compass.W, "overlay_path", 2);
            paths.Add(Compass.SW, Compass.W, "overlay_path", 2);
            map.AddPathClass("paths", paths);

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
                if (camera.zoom > 1f) camera.zoom = 1f;
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
