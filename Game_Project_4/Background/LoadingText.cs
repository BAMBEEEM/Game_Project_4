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
        private Texture2D _buttonRT;
        private MenuWood _wood = new MenuWood();
        public ReturnButton ReturnButton = new ReturnButton();

        private double _animationTimer;

        private SpriteFont _smallFont;
        private SpriteFont _font;


        public void LoadContent(ContentManager content)
        {
            _smallFont = content.Load<SpriteFont>("retrosmall");
            _font = content.Load<SpriteFont>("retro");

            _spaceBarButton = content.Load<Texture2D>("spacebar");
            _buttonRT = content.Load<Texture2D>("RTButton");
            _wood.LoadContent(content);
            ReturnButton.LoadContent(content);

            DBModel.ReadLists();


        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
        {
            float firstLineYPosition = 300;
            int lineNumber = 0;
            _wood.Draw(spriteBatch);
            ReturnButton.Draw(spriteBatch);
            spriteBatch.DrawString(_font, "Scores", new Vector2(530, 155), Color.White);

            foreach (string line in DBModel.Scores)
            {
                spriteBatch.DrawString(_smallFont, line, new Vector2(320 , 207 + (lineNumber * 49)), Color.White);
                lineNumber++;
            }



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



            spriteBatch.DrawString(_font, "       Press             (or     ) to shoot!", new Vector2(98, 660), Color.White);
            spriteBatch.Draw(_spaceBarButton, new Vector2(391, 662), new Rectangle(0, 0, 202, 46), Color.White, 0, new Vector2(0, 0), (float)1, SpriteEffects.None, 0.17f);
            spriteBatch.Draw(_buttonRT, new Vector2(706, 660), new Rectangle(0, 0, 54, 53), Color.White, 0, new Vector2(0, 0), (float)1, SpriteEffects.None, 0.74f);




        }

    }
}
