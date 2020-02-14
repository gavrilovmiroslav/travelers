using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travelers
{
    public class Character
    {
        public ContentManager Content;

        public Character(ContentManager c)
        {
            Content = c;
        }

        public void Update() { }

        public void Draw(SpriteBatch spriteBatch)
        {

        }
    }
}
