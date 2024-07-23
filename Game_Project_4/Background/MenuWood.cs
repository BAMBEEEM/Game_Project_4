using Game_Project_4.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Project_4.Background
{
    /// <summary>
    /// This class represents the wooden background menu. 
    /// </summary>
    public class MenuWood
    {
        private Texture2D _texture;
        private Vector2 _position = new Vector2(90, 48) * 1.52f;
        private BoundingRectangle bounds = new BoundingRectangle(new Vector2(133, 52) * 1.52f, 518 * 1.52f, 373 * 1.52f);
        public BoundingRectangle Bounds => bounds;
        public Color Shade = Color.White;
        public void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>("menuwood");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _position, null, Shade, 0, new Vector2(8, 0), 1f * 1.52f, SpriteEffects.None, 1);
        }
    }
}
