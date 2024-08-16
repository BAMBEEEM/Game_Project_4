using Game_Project_4.StateManagement;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Game_Project_4.Background;
using Game_Project_4.GameButtons;
using SharpDX.Direct2D1;
using Microsoft.Xna.Framework.Audio;
using System.Reflection.Metadata;
using Game_Project_4.Misc;
using SharpDX.DirectWrite;
using System.IO;

namespace Game_Project_4.Screens
{
    public class MenuScreen : GameScreen
    {

        ContentManager _content;

        Texture2D _texture;
        VideoPlayer _player;
        bool _isPlaying = false;
        InputAction _skip;

        private MenuBackground __forestIntro;
        private LoadingText _loadingText;



        private SpriteFont _arial;
        private InputState _inputState;
        //private Song _introSong;

        private InputAction _menuUp;
        private InputAction _menuDown;
        private InputAction _menuSelect;

        private float _time;
        private bool _isTransitioning = false;
        private float _timeSinceTransition;

        private MenuWood _wood;
        private StartButton _startButton;
        private ControlsButton _controlsButton;
        private bool _scoreClicked = false;
        private bool _returnClicked = false;


        TimeSpan introProgress;
        Matrix cameraUpTransform;




        /*        public MenuScreen() 
                {
                    _player = new VideoPlayer();
                    _skip = new InputAction(new Buttons[] { Buttons.A }, new Keys[] { Keys.Enter, Keys.Space }, true);
                }*/

        /*        private void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
                {
                    ExitScreen();
                    var loadingScreen = new GameplayScreen();
                    ScreenManager.AddScreen(loadingScreen, PlayerIndex.One);

                } //here we have our own quit game etc.*/

        public override void Activate()
        {
            if (_content == null)
            {
                _content = new ContentManager(ScreenManager.Game.Services, "Content");
            }
            _inputState = new InputState();
            _wood = new MenuWood();
            _startButton = new StartButton();
            _controlsButton = new ControlsButton();
            __forestIntro = new MenuBackground();
            _loadingText = new LoadingText();

            _loadingText.LoadContent(_content);



            _wood.LoadContent(_content);
            __forestIntro.LoadContent(_content);
            _startButton.LoadContent(_content);
            _controlsButton.LoadContent(_content);

            /*
                        _menuSong = _content.Load<Song>("MenuSongWithLoop");
                        MediaPlayer.Play(_menuSong);*/

            //_introSong = _content.Load<Song>("IntroSong");

            _menuUp = new InputAction(
                new[] { Buttons.DPadUp, Buttons.LeftThumbstickUp },
                new[] { Keys.Up, Keys.W }, true);
            _menuDown = new InputAction(
                new[] { Buttons.DPadDown, Buttons.LeftThumbstickDown },
                new[] { Keys.Down, Keys.S }, true);
            _menuSelect = new InputAction(
                new[] { Buttons.A, Buttons.Start },
                new[] { Keys.Enter, Keys.Space }, true);
            MediaPlayer.IsRepeating = true;

            //MediaPlayer.Play(_introSong);

        }



        public override void HandleInput(GameTime gameTime, InputState input)
        {
            if (_inputState.PriorMouseState.Position != _inputState.CurrentMouseState.Position)
            {
                _startButton.IsSelected = false;
                _startButton.Shade = Color.White;
                _controlsButton.IsSelected = false;
                _controlsButton.Shade = Color.White;
                _loadingText.ReturnButton.Shade = Color.White;
                _loadingText.ReturnButton.IsSelected = false;

            }
            else
            {
                if (_startButton.IsSelected || _controlsButton.IsSelected || _loadingText.ReturnButton.IsSelected)
                {

                    ScreenManager.Game.IsMouseVisible = false;
                }
                else ScreenManager.Game.IsMouseVisible = true;

            }



            if (_controlsButton.Bounds.CollidesWith(_inputState.Cursor) && _inputState.Clicked)
                _controlsButton.InitialClick = true;

            if (_controlsButton.Bounds.CollidesWith(_inputState.Cursor) && _controlsButton.InitialClick)
            {
                if (_inputState.Clicking)
                {
                    _controlsButton.Shade = Color.DarkGray;

                }
                else
                {
                    _controlsButton.Shade = Color.White;
                }
                if (Mouse.GetState().LeftButton == ButtonState.Released)
                {
                    _controlsButton.Shade = Color.DarkGray;
                    _controlsButton.InitialClick = false;
                    _scoreClicked = true;
                }
            }
            else
            {
                if (_inputState.CurrentMouseState.LeftButton == ButtonState.Released)
                {
                    _controlsButton.InitialClick = false;
                }
                _controlsButton.Shade = Color.White;
            }

            if (_startButton.Bounds.CollidesWith(_inputState.Cursor) && _inputState.Clicked)
                _startButton.InitialClick = true;

            if (_startButton.Bounds.CollidesWith(_inputState.Cursor) && _startButton.InitialClick)
            {
                if (_inputState.Clicking)
                {
                    _startButton.Shade = Color.DarkGray;

                }
                else
                {
                    _startButton.Shade = Color.White;
                }
                if (_inputState.CurrentMouseState.LeftButton == ButtonState.Released)
                {
                    _startButton.InitialClick = false;
                    LoadTransition(gameTime);
                }

            }
            else
            {
                if (_inputState.CurrentMouseState.LeftButton == ButtonState.Released)
                {
                    _startButton.InitialClick = false;
                }
                _startButton.Shade = Color.White;
            }



            if (_loadingText.ReturnButton.Bounds.CollidesWith(_inputState.Cursor) && _inputState.Clicked)
                _loadingText.ReturnButton.InitialClick = true;

            if (_loadingText.ReturnButton.Bounds.CollidesWith(_inputState.Cursor) && _loadingText.ReturnButton.InitialClick)
            {
                if (_inputState.Clicking)
                {
                    _loadingText.ReturnButton.Shade = Color.DarkGray;

                }
                else
                {
                    _loadingText.ReturnButton.Shade = Color.White;
                }
                if (_inputState.CurrentMouseState.LeftButton == ButtonState.Released)
                {
                    _loadingText.ReturnButton.InitialClick = false;
                    _returnClicked = true;

                }

            }
            else
            {
                if (_inputState.CurrentMouseState.LeftButton == ButtonState.Released)
                {
                    _loadingText.ReturnButton.InitialClick = false;
                }
                _loadingText.ReturnButton.Shade = Color.White;
            }

            PlayerIndex playerIndex;

            if (cameraUpTransform == Matrix.Identity)
            {
                if (_menuUp.Occurred(input, ControllingPlayer, out playerIndex))
                {
                    if (_startButton.IsSelected)
                    {
                        _startButton.IsSelected = false;
                        _controlsButton.IsSelected = true;
                    }
                    else if (_controlsButton.IsSelected)
                    {
                        _startButton.IsSelected = true;
                        _controlsButton.IsSelected = false;
                    }
                    else
                    {
                        _startButton.IsSelected = true;
                        _controlsButton.IsSelected = false;
                    }

                }

                if (_menuDown.Occurred(input, ControllingPlayer, out playerIndex))
                {

                    if (_startButton.IsSelected)
                    {
                        _startButton.IsSelected = false;
                        _controlsButton.IsSelected = true;
                    }
                    else if (_controlsButton.IsSelected)
                    {
                        _startButton.IsSelected = true;
                        _controlsButton.IsSelected = false;
                    }
                    else
                    {
                        _startButton.IsSelected = false;
                        _controlsButton.IsSelected = true;
                    }


                }

                if (_menuSelect.Occurred(input, ControllingPlayer, out playerIndex))
                {
                    if (_startButton.IsSelected)
                    {
                        _startButton.Shade = Color.DarkGray;
                        LoadTransition(gameTime);
                    }
                    else
                    {
                        _controlsButton.Shade = Color.DarkGray;
                        /*                    _difficultyButton.NextDifficulty();
                        */
                    }

                }
            }
            else if (_menuSelect.Occurred(input, ControllingPlayer, out playerIndex))
            {
                if (_loadingText.ReturnButton.IsSelected)
                {
                    _loadingText.ReturnButton.Shade = Color.DarkGray;
                    cameraUpTransform = Matrix.Identity;
                }


            }
        }

        void StartGame()
        {
            /*            DifficultySettings.InitializeDifficulty();
            */
            ScreenManager.Game.ResetElapsedTime();
            var gameplayScreen = new GameplayScreen();
            ScreenManager.AddScreen(gameplayScreen, PlayerIndex.One);
            ExitScreen();
        }

        void LoadTransition(GameTime gameTime)
        {
            _time = 0;
            _isTransitioning = true;
            _timeSinceTransition = 0;
            ScreenManager.Game.IsMouseVisible = false;
            StartGame();
        }

        /*        void QuitGame()
                {
                    ScreenManager.Game.Exit();
                }*/

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {


            _timeSinceTransition += gameTime.ElapsedGameTime.Milliseconds;

            introProgress += gameTime.ElapsedGameTime;

            /*            if (_isTransitioning && introProgress.TotalMilliseconds >= _introSong.Duration.TotalMilliseconds-1650)
                        {
                            StartGame();
                        }

                        if (introProgress.TotalMilliseconds >= _introSong.Duration.TotalMilliseconds - 1650)
                        {
                            introProgress = TimeSpan.Zero - TimeSpan.FromMilliseconds(1600);
                        }*/

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);





            _inputState.Update();

            /*if (_inputState.Exit == true)
            { Exit(); }*/
            // TODO: Add your update logic here

            //balls[2].Position += _inputManager.Direction;

            // TODO: Add your update logic here


        }

        public override void Deactivate()
        {
            /*            _player.Pause();
                        _isPlaying = false;*/
        }

        public override void Draw(GameTime gameTime)
        {

            var _spriteBatch = ScreenManager.SpriteBatch;
            _spriteBatch.Begin();

            __forestIntro.Draw(_spriteBatch);
            _loadingText.Draw(_spriteBatch, gameTime);


            _spriteBatch.End();


            cameraUpTransform = Matrix.Identity;

            if (_scoreClicked)
            {
                _time += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                Matrix cameraUpTranslation = Matrix.CreateTranslation(0, -0.6f * _time, 0);
                cameraUpTransform = cameraUpTranslation;
            }

            if (_returnClicked)
            {
                
                _scoreClicked = false;
                _time = 0;
                cameraUpTransform = Matrix.Identity;
                _returnClicked = false;
            }

            _spriteBatch.Begin(transformMatrix: cameraUpTransform);

            _wood.Draw(_spriteBatch);

            _startButton.Draw(_spriteBatch);

            _controlsButton.Draw(_spriteBatch);
            _spriteBatch.End();




        }
    }
}
