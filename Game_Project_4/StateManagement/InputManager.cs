using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game_Project_4.Collisions;

namespace Game_Project_4.StateManagement
{
    public class InputManager
    {

        KeyboardState currentKeyboardState;
        KeyboardState priorKeyboardState;
        MouseState currentMouseState;
        MouseState priorMouseState;
        GamePadState currentGamePadState;
        GamePadState priorGamePadState;
        BoundingRectangle cursor;

        public MouseState CurrentMouseState => currentMouseState;
        public BoundingRectangle Cursor => cursor;


        /// <summary>
        /// If the Clicked/Clicking funcctionality has been requested from InputExampleGame (kinda like a delegeate??)
        /// </summary>
        public bool Clicked { get; private set; } = false;

        public bool Clicking { get; private set; } = false;


        /// <summary>
        /// Request game exit
        /// </summary>
        public bool Exit { get; private set; } = false;


        public void Update(GameTime gameTime)
        {
            #region Key Input
            priorKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            priorMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
            cursor = new(currentMouseState.Position.X, currentMouseState.Position.Y, 1, 1);


            priorGamePadState = currentGamePadState;
            currentGamePadState = GamePad.GetState(0);

            #endregion



            #region Clicked/CLicking Input
            Clicked = false;

            /*            if (currentKeyboardState.IsKeyDown(Keys.Space) &&
                            priorKeyboardState.IsKeyUp(Keys.Space))
                        { Clicked = true; }*/

            if (currentMouseState.LeftButton == ButtonState.Pressed &&
                priorMouseState.LeftButton == ButtonState.Pressed)
            {
                Clicking = true;
            }
            else
            {
                Clicking = false;
            }

            if (currentMouseState.LeftButton == ButtonState.Pressed &&
                priorMouseState.LeftButton == ButtonState.Released)
            {
                Clicked = true;
            }
            /*
                        if (currentMouseState.LeftButton == ButtonState.Released &&
                priorMouseState.LeftButton == ButtonState.Pressed)
                        {
                            Clicked = true;
                        }*/



            /*            if (currentGamePadState.IsButtonDown(Buttons.A) &&
                            priorGamePadState.IsButtonUp(Buttons.A))
                        {
                            Clicked = true;
                        }*/

            #endregion

            #region Exit Input

            if (currentGamePadState.Buttons.Back == ButtonState.Pressed || currentKeyboardState.IsKeyDown(Keys.Escape))
                Exit = true;

            #endregion

        }

    }
}

