using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Game_Project_4.StateManagement;
using static System.TimeZoneInfo;
using Game_Project_4.Background;
using Game_Project_4.Sprites;
using System.Collections.Generic;
using System.Reflection.Metadata;
using Game_Project_4.Collisions;
using SharpDX.Direct2D1;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using System.DirectoryServices.ActiveDirectory;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify;
using Game_Project_4.ParticleManagement;
using Game_Project_4.Misc;
using System.Drawing.Text;
using System.IO;
using Game_Project_4.BotAI;

namespace Game_Project_4.Screens
{
    // This screen implements the actual game logic. It is just a
    // placeholder to get the idea across: you'll probably want to
    // put some more interesting gameplay in here!
    public class GameplayScreen : GameScreen
    {
        private ContentManager _content;
        /*        private SpriteFont _gameFont;

                private Vector2 _playerPosition = new Vector2(100, 100);
                private Vector2 _enemyPosition = new Vector2(100, 100);

                private readonly Random _random = new Random();

                private float _pauseAlpha;
                private readonly InputAction _pauseAction;

                private GraphicsDeviceManager graphics;*/

        private bool _hasBegun = false;
        private bool _lost = false;

        private CharacterSprite _mainCharacter;
        private SpriteFont spriteFont;
        private Enemy[] _enemy = new Enemy[6];
        private AlienEnemy _alienEnemy = new AlienEnemy(RandomHelper.Next(1, 3), 3.8f);
        private FinalBoss _finalBoss = new FinalBoss(1, 0);
        private StaminaBarSprite _staminaSprite;


        private bool _won = false;

        private TimeSpan _elapsedTime = new TimeSpan();

        private Color _timeColor;

        private float _shakeTime;

        private float _drownTime;

        private bool _isDrowning;

        private float _startTimer = 0;

        TimeSpan introProgress;
        Song _ingameSong;

        private GraphicsDeviceManager _graphics;
        private Map _map;
        private Vector2 _viewportPosition;
        private BasicTilemap _groundLayer;

        private BackgroundDetails _backgroundDetails;

        private int _level = 0;



        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            _hasBegun = true;
            /*
                        _pauseAction = new InputAction(
                            new[] { Buttons.Start, Buttons.Back },
                            new[] { Keys.Back }, true);*/
        }




        // Load graphics content for the game
        public override void Activate()
        {
            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");




            _map = Map.Load(Path.Combine(_content.RootDirectory, "Cursed_land.tmx"), _content);
            _groundLayer = _content.Load<BasicTilemap>("example2");

            _staminaSprite = new StaminaBarSprite();
            _staminaSprite.LoadContent(_content);

            _backgroundDetails = new BackgroundDetails();
            _backgroundDetails.LoadContent(_content);

            _mainCharacter = new CharacterSprite();

            _mainCharacter.LoadContent(_content);

            _alienEnemy.LoadContent(_content);

            _finalBoss.LoadContent(_content);

            for (int i = 0; i < 6; i++)
            {
                int level;
                float time;
                if (i <= 1)
                {
                    level = 1;
                    time = i * 2.45f;
                }
                else
                {
                    level = 2;
                    time = (i - 2) * 2.45f;
                }

                _enemy[i] = new Enemy(RandomHelper.Next(1, 3), time, level);
                _enemy[i].LoadContent(_content);
                _enemy[i].Player = _mainCharacter;
            }

            _alienEnemy.Player = _mainCharacter;
            _finalBoss.Player = _mainCharacter;


            _mainCharacter.MaxOffsetX = (ScreenManager.GraphicsDevice.Viewport.Width);

            _mainCharacter.MaxOffsetY = (ScreenManager.GraphicsDevice.Viewport.Height);

            _finalBoss.MaxOffsetX = (ScreenManager.GraphicsDevice.Viewport.Width);

            _finalBoss.MaxOffsetY = (ScreenManager.GraphicsDevice.Viewport.Height);


            spriteFont = _content.Load<SpriteFont>("retro");





            /*            // _gameFont = _content.Load<SpriteFont>("gamefont");

                        // A real game would probably have more content than this sample, so
                        // it would take longer to load. We simulate that by delaying for a
                        // while, giving you a chance to admire the beautiful loading screen.
                        Thread.Sleep(1000);

                        // once the load has finished, we use ResetElapsedTime to tell the game's
                        // timing mechanism that we have just finished a very long frame, and that
                        // it should not try to catch up.*/
            ScreenManager.Game.ResetElapsedTime();

            _ingameSong = _content.Load<Song>("IngameSong");



            MediaPlayer.Play(_ingameSong);
            MediaPlayer.IsRepeating = true;
        }

        /*        public override void Deactivate()
                {
                    base.Deactivate();
                }*/

        /*        public override void Unload()
                {
                    _content.Unload();
                }*/
        private float _mainTimer;
        private int _killed = 0;
        private bool _saved = false;

        private float _levelOneTimer = 5500;
        private float _levelTwoTimer = 3500;
        private float _levelThreeTimer = 5000;


        // This method checks the GameScreen.IsActive property, so the game will
        // stop updating when the pause menu is active, or if you tab away to a different application.
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            if (_finalBoss.Dead)
                _won = true;
            else if (_levelThreeTimer < 0 && !(_level == 3))

            {
                _level = 3;
                ScreenManager.Game.ResetElapsedTime();
                _mainTimer = 0;
            }
            else if (_levelTwoTimer < 0 && !(_level >= 2))

            {
                _level = 2;
                ScreenManager.Game.ResetElapsedTime();
                _mainTimer = 0;
            }
            else if (_levelOneTimer < 0 && !(_level >= 1))

            { 
                _level = 1;
                ScreenManager.Game.ResetElapsedTime();
                _mainTimer = 0;
            }

            if (_enemy[2].Dead && _enemy[3].Dead && _enemy[4].Dead && _enemy[5].Dead && _alienEnemy.Dead)
            {
                _levelThreeTimer -= (float) gameTime.ElapsedGameTime.TotalMilliseconds;
            }

            if (_enemy[0].Dead && _enemy[1].Dead )
                _levelTwoTimer -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (_level == 0)
                _levelOneTimer -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;


            _alienEnemy.Update(gameTime);
            _finalBoss.Update(gameTime);
            _startTimer -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            _mainTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_startTimer > 18)
            {
                _mainCharacter.Stopped = true;
            }
            else if (_startTimer > -18) _mainCharacter.Stopped = false;

            _staminaSprite.Stamina = _mainCharacter.Stamina;



            _mainCharacter.Color = Color.White; // default color
            foreach (Enemy e in _enemy)
            {
                if (e.RespawnTime < _mainTimer && e.Level == _level)
                    e.Stopped = false;
                else e.Stopped = true;

                if (e.Dead)
                    _killed++;
                e.Update(gameTime);
            }

            if (_alienEnemy.RespawnTime < _mainTimer && _level == 2)
                _alienEnemy.Stopped = false;
            else _alienEnemy.Stopped = true;

            if (_level == 3)
                _finalBoss.Stopped = false;
            else _finalBoss.Stopped = true;

            _mainCharacter.Update(gameTime);

            if (_mainCharacter.Dead)
                _lost = true;
            // very good for testing: 
            if (CollisionHelper.Collides(new BoundingRectangle(Mouse.GetState().Position.X, Mouse.GetState().Position.Y, 1, 1), _finalBoss.AttackBounds) && _finalBoss.Damaging)
                _finalBoss.Color = Color.Red;
            else _finalBoss.Color = Color.White;



            float respawnTime = 0;
            foreach (Enemy enemy in _enemy)
            {

                if (_mainCharacter.WeaponBounds.CollidesWith(enemy.CharacterBounds) && _mainCharacter.Damaging && !enemy.Dead)
                {
                    enemy.Health -= _mainCharacter.AtackFirepower;
                    enemy.Color = Color.Red;
                }
                else enemy.Color = Color.White;

                if (!_mainCharacter.Invincible)
                    if (enemy.AttackBounds.CollidesWith(_mainCharacter.CharacterBounds) && enemy.Damaging && !_mainCharacter.Dead)
                    {
                        _mainCharacter.Health -= enemy.AttackDamage;
                        _mainCharacter.Color = Color.Red;
                    }
            }

            if (_mainCharacter.WeaponBounds.CollidesWith(_alienEnemy.CharacterBounds) && _mainCharacter.Damaging && !_alienEnemy.Dead)
            {
                _alienEnemy.Health -= _mainCharacter.AtackFirepower;
                _alienEnemy.Color = Color.Red;
            }
            else _alienEnemy.Color = Color.White;

            if (!_mainCharacter.Invincible)

                if (_alienEnemy.AttackBounds.CollidesWith(_mainCharacter.CharacterBounds) && _alienEnemy.Damaging && !_mainCharacter.Dead && !_alienEnemy.Dead)
                {
                    _mainCharacter.Health -= _alienEnemy.AttackDamage;
                    _mainCharacter.Color = Color.Red;
                }

            if (_mainCharacter.WeaponBounds.CollidesWith(_finalBoss.CharacterBounds) && _mainCharacter.Damaging && !_finalBoss.Dead)
            {
                _finalBoss.Health -= _mainCharacter.AtackFirepower;
                _finalBoss.Color = Color.Red;
            }
            else _finalBoss.Color = Color.White;

            if (!_mainCharacter.Invincible)

                if (_finalBoss.AttackBounds.CollidesWith(_mainCharacter.CharacterBounds) && _finalBoss.Damaging && !_mainCharacter.Dead)
                {
                    _mainCharacter.Health -= _alienEnemy.AttackDamage;
                    _mainCharacter.Color = Color.Red;
                }




            /*            introProgress += gameTime.ElapsedGameTime;
                        TimeSpan songDurationLeft = TimeSpan.Zero;
                        if (introProgress >= _ingameSong.Duration - TimeSpan.FromMilliseconds(85))
                        {
                            songDurationLeft.Add(TimeSpan.FromMilliseconds(introProgress.TotalMilliseconds - _ingameSong.Duration.TotalMilliseconds));
                        }

                        if (introProgress.TotalMilliseconds >= _ingameSong.Duration.TotalMilliseconds - 18)
                        {
                            introProgress = TimeSpan.Zero;
                            MediaPlayer.Stop();
                            MediaPlayer.Play(_ingameSong);
                        }
                        songDurationLeft = TimeSpan.Zero;*/


            // responsible for time limit
            while (!_hasBegun) gameTime.ElapsedGameTime = new TimeSpan();
            _elapsedTime += gameTime.ElapsedGameTime;

            // responsible for time limit
            if (_won)
            {
                _mainCharacter.Stopped = true; // charecter stops when game ends.
                _mainCharacter.WinManeuver = true;
            }

            else if (_lost)
            {
                _mainCharacter.Stopped = true; // charecter stops when game ends.

            }




            /*
                        // Gradually fade in or out depending on whether we are covered by the pause screen.
                        if (coveredByOtherScreen)
                            _pauseAlpha = Math.Min(_pauseAlpha + 1f / 32, 1);
                        else
                            _pauseAlpha = Math.Max(_pauseAlpha - 1f / 32, 0);

                        if (IsActive)
                        {
                            // Apply some random jitter to make the enemy move around.
                            const float randomization = 10;

                            _enemyPosition.X += (float)(_random.NextDouble() - 0.5) * randomization;
                            _enemyPosition.Y += (float)(_random.NextDouble() - 0.5) * randomization;

                            // Apply a stabilizing force to stop the enemy moving off the screen.
                            var targetPosition = new Vector2(
                                ScreenManager.GraphicsDevice.Viewport.Width / 2 - _gameFont.MeasureString("Insert Gameplay Here").X / 2,
                                200);

                            _enemyPosition = Vector2.Lerp(_enemyPosition, targetPosition, 0.05f);

                            // This game isn't very fun! You could probably improve
                            // it by inserting something more interesting in this space :-)
                        }*/

            base.Update(gameTime, otherScreenHasFocus, false);
        }

        // Unlike the Update method, this will only be called when the gameplay screen is active.
        public override void HandleInput(GameTime gameTime, InputState input)
        {
            /*            if (input == null)
                            throw new ArgumentNullException(nameof(input));

                        // Look up inputs for the active player profile.
                        int playerIndex = (int)ControllingPlayer.Value;

                        var keyboardState = input.CurrentKeyboardStates[playerIndex];
                        var gamePadState = input.CurrentGamePadStates[playerIndex];

                        // The game pauses either if the user presses the pause button, or if
                        // they unplug the active gamepad. This requires us to keep track of
                        // whether a gamepad was ever plugged in, because we don't want to pause
                        // on PC if they are playing with a keyboard and have no gamepad at all!
                        bool gamePadDisconnected = !gamePadState.IsConnected && input.GamePadWasConnected[playerIndex];

                        PlayerIndex player;
                        if (_pauseAction.Occurred(input, ControllingPlayer, out player) || gamePadDisconnected)
                        {
                           // ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
                        }
                        else
                        {
                            // Otherwise move the player position.
                            var movement = Vector2.Zero;

                            if (keyboardState.IsKeyDown(Keys.Left))
                                movement.X--;

                            if (keyboardState.IsKeyDown(Keys.Right))
                                movement.X++;

                            if (keyboardState.IsKeyDown(Keys.Up))
                                movement.Y--;

                            if (keyboardState.IsKeyDown(Keys.Down))
                                movement.Y++;

                            var thumbstick = gamePadState.ThumbSticks.Left;

                            movement.X += thumbstick.X;
                            movement.Y -= thumbstick.Y;

                            if (movement.Length() > 1)
                                movement.Normalize();

                            _playerPosition += movement * 8f;
                        }*/
        }

        public override void Draw(GameTime gameTime)
        {
            _shakeTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            var spriteBatch = ScreenManager.SpriteBatch;

            //GraphicsDevice.Clear(Color.Transparent);

            //Calculate our offset vector
            float playerX = MathHelper.Clamp(_mainCharacter.CurrentPosition.X, 630, 13300);
            float offsetX = 630 - playerX;


            /*            Matrix zoomTranslation = Matrix.CreateTranslation(-1280 / 2f, -720 / 2f, 0);
                        Matrix zoomTransform = zoomTranslation * Matrix.CreateScale(0.85f) * Matrix.Invert(zoomTranslation);
            */
            // Background

            spriteBatch.Begin();
            _groundLayer.Draw(gameTime, spriteBatch);
            _map.Draw(spriteBatch, new Rectangle(0, 0, ScreenManager.GraphicsDevice.Viewport.Width, ScreenManager.GraphicsDevice.Viewport.Height), _viewportPosition);

            _mainCharacter.Draw(gameTime, spriteBatch);
            foreach (Enemy enemy in _enemy)
                enemy.Draw(gameTime, spriteBatch);
            _alienEnemy.Draw(gameTime, spriteBatch);
            _finalBoss.Draw(gameTime, spriteBatch);
            _backgroundDetails.Draw(spriteBatch);
            /*            _mainCharacter.Draw(gameTime, spriteBatch);
            */
            spriteBatch.End();

            spriteBatch.Begin();
            _staminaSprite.Draw(spriteBatch);

            spriteBatch.End();
            CameraSettings.transform = Matrix.CreateTranslation(offsetX, 0, 0) * CameraSettings.WaveShakeEffect(_shakeTime) * CameraSettings.DrownShakeEffect(_shakeTime, _isDrowning);


            spriteBatch.Begin(transformMatrix: Matrix.CreateTranslation(offsetX, 0, 0) * CameraSettings.DrownShakeEffect(_shakeTime, _isDrowning));



            spriteBatch.End();


            spriteBatch.Begin(transformMatrix: Matrix.CreateTranslation(offsetX, 0, 0) * CameraSettings.WaveShakeEffect(_shakeTime) * CameraSettings.DrownShakeEffect(_shakeTime, _isDrowning));
            /*            _wave.Draw(spriteBatch);
                        _outerWaveEffectOne.Draw(spriteBatch);
                        _outerWaveEffectTwo.Draw(spriteBatch);*/
            spriteBatch.End();

            spriteBatch.Begin(blendState: BlendState.Additive, transformMatrix: Matrix.CreateTranslation(offsetX, 0, 0) * CameraSettings.WaveShakeEffect(_shakeTime) * CameraSettings.DrownShakeEffect(_shakeTime, _isDrowning));
            /*
                        _innerWaveEffectOne.Draw(spriteBatch);
                        _innerWaveEffectTwo.Draw(spriteBatch);
            */
            spriteBatch.End();

            spriteBatch.Begin(transformMatrix: Matrix.CreateTranslation(offsetX, 0, 0) * CameraSettings.DrownShakeEffect(_shakeTime, _isDrowning));

            /*            _forestFrontLayer.Draw(spriteBatch);
            */

            


            spriteBatch.End();


            // TODO: Add your drawing code here
            spriteBatch.Begin();
            /*            _staminaSprite.Draw(spriteBatch);
            */
            /*            if (_startTimer > 0)
                        {
                            spriteBatch.DrawString(spriteFont, "RUN! -->", new Vector2(355, 75) * 1.6f, Color.White);
                        }

                        if (_won)
                        {
                            spriteBatch.DrawString(spriteFont, "You win!", new Vector2(335, 195) * 1.6f, Color.White);
                        }*/

            int score = _killed * 500;
            if (_lost || _won)
            {
                if (_lost)
                    spriteBatch.DrawString(spriteFont, "You're Dead!", new Vector2(295, 204) * 1.6f, Color.White);
                else
                    spriteBatch.DrawString(spriteFont, "You Won!", new Vector2(328, 201) * 1.6f, Color.White);

                /*                spriteBatch.DrawString(spriteFont, $"Score: {score}", new Vector2(302, 228) * 1.6f, Color.White);
                */
/*                if (!_saved)
                {
                    DBModel.SaveList(score);
                    _saved = true;
                }*/
            }
            /*            else spriteBatch.DrawString(spriteFont, $"Score: {score}", new Vector2(310, 30) * 1.6f, Color.White);
            */             if (_levelThreeTimer >= 450 && _levelThreeTimer <= 2450)
                spriteBatch.DrawString(spriteFont, $"Here Comes the boss!", new Vector2(222, 205) * 1.6f, Color.White);

             if (_levelThreeTimer >= 2950 && _levelThreeTimer <= 4950)
                spriteBatch.DrawString(spriteFont, $"Final Round", new Vector2(296, 205) * 1.6f, Color.White);


            else if (_levelTwoTimer >= 1550 && _levelTwoTimer <= 3450)
                spriteBatch.DrawString(spriteFont, $"Round 2", new Vector2(326, 204) * 1.6f, Color.White);
            else if (_levelOneTimer >= 3450 && _levelOneTimer <= 5450)

                spriteBatch.DrawString(spriteFont, $"Defeat All Enemies!", new Vector2(234, 206) * 1.6f, Color.White);

            else if (_levelOneTimer >= 950 && _levelOneTimer <= 2950)
            {
                spriteBatch.DrawString(spriteFont, $"Round 1", new Vector2(327, 204) * 1.6f, Color.White);

            }
            spriteBatch.End();





            base.Draw(gameTime);
            /*
                // This game has a blue background. Why? Because!
                ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.CornflowerBlue, 0, 0);

                // Our player and enemy are both actually just text strings.
                var spriteBatch = ScreenManager.SpriteBatch;

                spriteBatch.Begin();

    *//*            spriteBatch.DrawString(_gameFont, "// TODO", _playerPosition, Color.Green);
                spriteBatch.DrawString(_gameFont, "Insert Gameplay Here",
                                       _enemyPosition, Color.DarkRed);*//*

                spriteBatch.End();

                // If the game is transitioning on or off, fade it out to black.
                if (TransitionPosition > 0 || _pauseAlpha > 0)
                {
                    float alpha = MathHelper.Lerp(1f - TransitionAlpha, 1f, _pauseAlpha / 2);

                    ScreenManager.FadeBackBufferToBlack(alpha);
                }*/
        }
    }
}
