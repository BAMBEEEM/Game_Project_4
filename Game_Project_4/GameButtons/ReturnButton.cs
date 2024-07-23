using Game_Project_4.Collisions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Project_4.GameButtons
{
    public class ReturnButton
    {

        private Texture2D _texture;

        private BoundingRectangle bounds => new BoundingRectangle(new Vector2(267 + 16 - 18.3125f, 300) * 1.52f, 124 * 2.3f * 1.52f, 31 * 2.3f * 1.52f);
        private Vector2 _position => new Vector2(267 + 16, 300) * 1.52f;
        public Color Shade = Color.White;
        public bool InitialClick = false;

        public bool IsSelected = true;

        /// <summary>
        /// The bounding rectangle of the StartButton
        /// </summary>
        public BoundingRectangle Bounds => bounds;

        public void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>("woodgui");
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsSelected == true)
                Shade = Color.Thistle;


            spriteBatch.Draw(_texture, _position, new Rectangle(286, 1265, 124, 31), Shade, 0, new Vector2(8, 0), 2.3f * 1.52f, SpriteEffects.None, 0);

        }
    }
}
