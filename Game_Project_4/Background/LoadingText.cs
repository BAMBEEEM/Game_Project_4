using Game_Project_4.Collisions;
using Game_Project_4.GameButtons;
using Game_Project_4.Misc;
using Game_Project_4.StateManagement;
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
    /// A class representing all texts in the MenuScreen
    /// </summary>
    public class LoadingText
    {
        private Texture2D _spaceBarButton;
        private MenuWood _wood = new MenuWood();
        public ReturnButton ReturnButton = new ReturnButton();

        private double _animationTimer;

        private SpriteFont _smallFont;
        private SpriteFont _font;

        private Texture2D WoodTexture;

        public void LoadContent(ContentManager content)
        {
            _smallFont = content.Load<SpriteFont>("retrosmall");
            _font = content.Load<SpriteFont>("retro");
            WoodTexture = content.Load<Texture2D>("WoodGUI");
            _spaceBarButton = content.Load<Texture2D>("spacebar");
            _wood.LoadContent(content);
            ReturnButton.LoadContent(content);

/*            DBModel.ReadLists();
*/

        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            float firstLineYPosition = 300;
            int lineNumber = 0;
            _wood.Draw(spriteBatch);
            ReturnButton.Draw(spriteBatch);
            spriteBatch.DrawString(_font, "Controls", new Vector2(506, 146), Color.White);
            spriteBatch.DrawString(_smallFont, "    Shoot Up,    Shot Down", new Vector2(320, 189+ 10), Color.White);
            spriteBatch.DrawString(_smallFont, "    Shoot Left,    Shot Right", new Vector2(320, 189+51 +10), Color.White);
            spriteBatch.DrawString(_smallFont, "    Move Up,    Move Down", new Vector2(320, 189 + 51+ 51 + 10), Color.White);
            spriteBatch.DrawString(_smallFont, "    Move Left,    Move Right", new Vector2(320, 189 + 51 + 51 + 51 + 10), Color.White);
            spriteBatch.DrawString(_smallFont, "To Shoot, Press ", new Vector2(320, 189 + 51 + 51 + 51 + 51 + 10), Color.White);
            spriteBatch.Draw(_spaceBarButton, new Vector2(691, 189 + 51 + 51 + 51 + 51 + 10), new Rectangle(0, 0, 202, 46), Color.White, 0, new Vector2(0, 0), (float)1, SpriteEffects.None, 0.17f);


            spriteBatch.Draw(WoodTexture, new Vector2(323, 189 + 14), new Rectangle(0, 1301, 19, 19), Color.White, 0, new Vector2(0, 0), (float)2f, SpriteEffects.None, 0.17f); spriteBatch.Draw(WoodTexture, new Vector2(323, 189 + 12), new Rectangle(0, 1301, 18, 19), Color.White, 0, new Vector2(0, 0), (float)2f, SpriteEffects.None, 0.17f);
            spriteBatch.Draw(WoodTexture, new Vector2(600, 189 + 14), new Rectangle(0, 1282, 19, 19), Color.White, 0, new Vector2(0, 0), (float)2f, SpriteEffects.None, 0.17f);

            spriteBatch.Draw(WoodTexture, new Vector2(320, 189 + 51 + 13), new Rectangle(0, 1242, 18, 20), Color.White, 0, new Vector2(0, 0), (float)2f, SpriteEffects.None, 0.17f);
            spriteBatch.Draw(WoodTexture, new Vector2(651, 189 + 51 + 13), new Rectangle(0, 1261, 18, 20), Color.White, 0, new Vector2(0, 0), (float)2f, SpriteEffects.None, 0.17f);

            spriteBatch.Draw(WoodTexture, new Vector2(330, 189 + 51 + 51 + 14), new Rectangle(0, 1355, 12, 18), Color.White, 0, new Vector2(0, 0), (float)2.3f, SpriteEffects.None, 0.17f); spriteBatch.Draw(WoodTexture, new Vector2(323, 189 + 12), new Rectangle(0, 1301, 18, 19), Color.White, 0, new Vector2(0, 0), (float)2f, SpriteEffects.None, 0.17f);
            spriteBatch.Draw(WoodTexture, new Vector2(582, 189 + 51 + 51 + 14), new Rectangle(0, 1393, 12, 18), Color.White, 0, new Vector2(0, 0), (float)2.3f, SpriteEffects.None, 0.17f);

            spriteBatch.Draw(WoodTexture, new Vector2(330, 189 + 51 + 51 + 51 + 13), new Rectangle(0, 1374, 12, 18), Color.White, 0, new Vector2(0, 0), (float)2.3f, SpriteEffects.None, 0.17f);
            spriteBatch.Draw(WoodTexture, new Vector2(630, 189 + 51 + 51 + 51 + 13), new Rectangle(0, 1412, 12, 18), Color.White, 0, new Vector2(0, 0), (float)2.3f, SpriteEffects.None, 0.17f);




            //spriteBatch.DrawString(_font, "Loading.", new Vector2(513, 310), Color.White);

            /*            _animationTimer += gameTime.ElapsedGameTime.TotalSeconds;

                        if (_animationTimer >= 0f)
                        {
                            spriteBatch.DrawString(_font, "Loading.", new Vector2(513, 310), Color.White);
                        }
                        if (_animationTimer >= 0.44f)
                        {
                            spriteBatch.DrawString(_font, "Loading..", new Vector2(513, 310), Color.White);
                        }
                        if (_animationTimer >= 0.88f)
                        {
                            spriteBatch.DrawString(_font, "Loading...", new Vector2(513, 310), Color.White);
                        }
                        if (_animationTimer >= 1.22f) _animationTimer = 0;*/



            spriteBatch.DrawString(_font, "Open Controls For Instructions!", new Vector2(211, 660), Color.White);




        }

    }
}
