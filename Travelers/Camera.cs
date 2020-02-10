using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travelers
{
    public class Camera
    {
        private Matrix transform;
        public Matrix Transform
        {
            get
            {
                return transform;
            }
        }

        public Vector2 center;
        public Viewport viewport;

        public float zoom = 1;

        public Camera(Viewport view)
        {
            this.viewport = view;
        }

        public void Update(Vector2 position)
        {
            center = new Vector2(position.X, position.Y);
            transform = Matrix.CreateTranslation(new Vector3(-center.X, -center.Y, 0)) *
                        Matrix.CreateScale(new Vector3(zoom, zoom, 0)) *
                        Matrix.CreateTranslation(new Vector3(viewport.Width / 2, viewport.Height / 2, 0));
        }
    }
}
