using Game_Project_4.Collisions;
using Game_Project_4.Misc;
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
    /// <summary>
    /// A class that represents the menu buttons.
    /// </summary>
    public class ControlsButton
    {
        private BoundingRectangle bounds = new BoundingRectangle(new Vector2(281.5F + 16 - 19.0625f, 246) * 1.52f, 112 * 2.3f * 1.52f, 31 * 2.3f * 1.52f);
        private Texture2D _texture;
        private Vector2 _position = new Vector2(281.5F + 16, 246) * 1.52f;

        public bool IsSelected = false;
        /// <summary>
        /// The bounding rectangle of the QuitButton
        /// </summary>
        public BoundingRectangle Bounds => bounds;
        public Color Shade = Color.White;
        public bool InitialClick = false;


        public void LoadContent(ContentManager content)
        {
            _texture = content.Load<Texture2D>("woodgui");
        }

/*        public void NextDifficulty()
        {
            if (DifficultySettings.SetDifficulty == Enums.Difficulty.Easy)
                DifficultySettings.SetDifficulty = Enums.Difficulty.Normal;
             
            else if (DifficultySettings.SetDifficulty == Enums.Difficulty.Normal)
                DifficultySettings.SetDifficulty = Enums.Difficulty.Hard;

            else DifficultySettings.SetDifficulty = Enums.Difficulty.Easy;
        }*/

        public void Draw(SpriteBatch spriteBatch)
        {
            if (IsSelected == true)
                Shade = Color.Thistle;

            Rectangle source;
/*            if (DifficultySettings.SetDifficulty == Enums.Difficulty.Easy)
                source = new Rectangle(0, 1192, 92, 32);
            else if (DifficultySettings.SetDifficulty == Enums.Difficulty.Normal)
                source = new Rectangle(93, 1192, 92, 32);

            else*/ source = new Rectangle(0, 1320, 112, 31);


            spriteBatch.Draw(_texture, _position, source, Shade, 0, new Vector2(8, 0), 2.3f * 1.52f, SpriteEffects.None, 1);
        }
    }
}
