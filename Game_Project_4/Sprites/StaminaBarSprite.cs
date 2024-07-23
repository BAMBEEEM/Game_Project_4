using Game_Project_4.Collisions;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Project_4.Sprites
{
    /// <summary>
    /// A class representing the Stamina Bar overlay
    /// </summary>
    public class StaminaBarSprite
    {
        private Texture2D texture;

        private Vector2 _overlayPosition = new(20, 20);

        private Vector2 _chargePosition;



        public Color color = Color.White;

        /// <summary>
        /// Stamina Capacity
        /// </summary>
        public float Stamina { private get; set; } = 100f;

        public void LoadContent(ContentManager content)
        {
            texture = content.Load<Texture2D>("staminabar");
        }

        public void Update(GameTime gameTime)
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // stamina bar overlay
            spriteBatch.Draw(texture, _overlayPosition, new Rectangle(64, 0, 115, 38), Color.White, 0, new Vector2(0, 0), (float)3, SpriteEffects.None, 0.1f);


            // stamina bar charges
            int charged = (int)(Stamina / 12.5f);
            for (int i = 0; i < 8; i++)
            {
                if (i < charged) color = Color.White;
                else color = Color.DarkGray;
                Rectangle source = new Rectangle(8 * i, 0, 8, 8);
                _chargePosition = new Vector2(_overlayPosition.X+116.5f, _overlayPosition.Y+45);
                spriteBatch.Draw(texture, new Vector2(_chargePosition.X+ ((i * 9) * 3), _chargePosition.Y), source, color, 0, new Vector2(0, 0), (float)3, SpriteEffects.None, 0.4f);
            }



        }
    }
}
