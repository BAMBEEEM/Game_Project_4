using Game_Project_4.Collisions;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Game_Project_4.Enums;
using Microsoft.Xna.Framework.Audio;
using Game_Project_4.Misc;
using SharpDX.DirectWrite;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks.Dataflow;
using Game_Project_4.ParticleManagement;

namespace Game_Project_4.Sprites
{

    /// <summary>
    /// A class representing a ninja
    /// </summary>
    public class CharacterSprite
    {
        private GamePadState gamePadState;

        private KeyboardState keyboardState;

        private Texture2D _charTexture;

        public Vector2 CurrentPosition = new Vector2(600, 200) * 1.6f;

        private BoundingRectangle _characterBounds = new BoundingRectangle(new Vector2(600 - 54, 200 - 56) * 1.6f, 23 * 2.8f, 33f * 2.8f);


        private BoundingRectangle _weaponBounds = new BoundingRectangle(new Vector2(600 - 54, 200 - 56) * 1.6f, 368f, 8.3f);


        private bool flipped;

        private int _animationFrame;

        private double _flippingTimer;

        private double _animationTimer;

        private double _soundTimer;

        private double _laserTimer;

        private StepSound _stepSound = StepSound.Left;

        private SoundEffect[] _steps = new SoundEffect[3];

        private SoundEffect _dashSound;

        private SoundEffect _laserSound;
        private float _flippingSpeed = 0.35f;

        public bool Stopped = false;

        private bool _softStopped = false;

        private float _animationSpeed = 0.1f;

        private bool _standing = true;

        public bool Poisoned = false;

        public bool Slowed = false;

        public bool WinManeuver = false;

        public bool LossManeuver = false;

        private bool _shooting = false;

        private bool _lastflipped = false;

        /*        private Vector2 _lossDirection = RandomHelper.NextDirection();


                private Vector2 _lossAcceleration = new Vector2(RandomHelper.NextFloat(-0.3f, 0.3f), RandomHelper.NextFloat(-0.21f, 0.2f));

                private Vector2 _lossVelocity = new Vector2(0, RandomHelper.NextFloat(1,1.2f));

                private float _lossManeuverTime = 0;

                public Vector2 LossPosition;*/

        public float Stamina = 100;

        // they will be instantiated in Game Screen class using ScreenManager's Graphics Viewport
        public float MinOffsetX { get; set; }
        public float MaxOffsetX { get; set; }
        public float MinOffsetY { get; set; }
        public float MaxOffsetY { get; set; }



        public float CharBonusSpeed = 1f;

        private float _charSpeedLimiter;

        double ClickTimer;
        const double TimerDelay = 500;

        private bool _isCharging;
        private float _timeSinceDash;

        public float Health = 750;

        public bool Dead = false;

        public float AtackFirepower = 10;

        /// <summary>
        /// The bounding volume of the sprite
        /// </summary>
        public BoundingRectangle CharacterBounds => _characterBounds;

        /// <summary>
        /// The bounding volume of the sprite's attack
        /// </summary>
        public BoundingRectangle WeaponBounds => _weaponBounds;

        /// <summary>
        /// The color to blend with the ghost
        /// </summary>
        public Color Color { get; set; } = Color.White;

        public bool Damaging = false;

        /// <summary>
        /// Loads the sprite texture using the provided ContentManager
        /// </summary>
        /// <param name="content">The ContentManager to load with</param>
        public void LoadContent(ContentManager content)
        {
            _charTexture = content.Load<Texture2D>("ninja");


            _steps[0] = content.Load<SoundEffect>("step1");
            _steps[1] = content.Load<SoundEffect>("step2");
            _steps[2] = content.Load<SoundEffect>("step3");


            _dashSound = content.Load<SoundEffect>("dashsound");
            _laserSound = content.Load<SoundEffect>("Laser");
        }

        float _lastSpacePressTime = 0;
        KeyboardState _previousKeyboardState;
        GamePadState _previousGamePadState;

        float dashtime = 0;
        private float _staminaChargeTimer = 0;
        public void Update(GameTime gameTime)
        {

            if (Health <= 0)
            {
                Dead = true;
                Stopped = true;
            }

            float initSpeed = CurrentPosition.X; //debug
/*            if (keyboardState.IsKeyDown(Keys.K))
                Dead = true;*/

            if (WinManeuver == true)
            {
                CurrentPosition += new Vector2(2, 0) * 1.6f * CharBonusSpeed;
                if (CurrentPosition.Y < 380)
                    CurrentPosition += new Vector2(0, 2) * 1.6f * CharBonusSpeed;
                else if (CurrentPosition.Y > 385) CurrentPosition += new Vector2(0, -2) * 1.6f * CharBonusSpeed;
            }

            if (LossManeuver == true)
            {
                /*                if (_lossManeuverTime < 600)
                                {
                                    _lossVelocity += _lossAcceleration;
                                    CurrentPosition += _lossVelocity;
                                    _lossManeuverTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                                }*/
                /*                CurrentPosition += new Vector2(2, 0) * 1.6f * CharBonusSpeed;
                                if (CurrentPosition.Y < LossPosition.Y+5)
                                    CurrentPosition += new Vector2(0, 2) * 1.6f * CharBonusSpeed;
                                else if (CurrentPosition.Y > LossPosition.Y+5) CurrentPosition += new Vector2(0, -2) * 1.6f * CharBonusSpeed;

                                if (CurrentPosition.X < LossPosition.X)
                                    CurrentPosition += new Vector2(2, 0) * 1.6f * CharBonusSpeed;
                                else if (CurrentPosition.X > LossPosition.X) CurrentPosition += new Vector2(-2, 0) * 1.6f * CharBonusSpeed;*/
            }


            _timeSinceDash += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            /*            if (_timeSinceDash > DifficultySettings.StaminaDelay)
                        {
                            _staminaChargeTimer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                            if (Stamina < 100)
                            {
                                if (_staminaChargeTimer > DifficultySettings.StaminaRate && !Stopped)
                                {
                                    Stamina += 2.5f;
                                    _staminaChargeTimer = 0;
                                }
                            }
                        }*/



            _previousKeyboardState = keyboardState;
            _previousGamePadState = gamePadState;
            keyboardState = Keyboard.GetState();
            gamePadState = GamePad.GetState(0);
            float currentTime = (float)gameTime.TotalGameTime.TotalMilliseconds;


            if (dashtime > 0)
                dashtime -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            else 
            {
                _shooting = false;

            }
            Rectangle source = new Rectangle(0, 0, 224, 160);

            // Check for double press of space within 500ms
            if ((keyboardState.IsKeyDown(Keys.Space) && _previousKeyboardState.IsKeyUp(Keys.Space))
                || (gamePadState.IsButtonDown(Buttons.RightTrigger) && _previousGamePadState.IsButtonUp(Buttons.RightTrigger)))
            {
                _shooting = true;
                dashtime = 450;
                _lastflipped = flipped;
                /*float timeSinceLastPress = currentTime - _lastSpacePressTime;
                                if (timeSinceLastPress < 500 && Stamina >= DifficultySettings.StaminaUsePerDash && !Stopped)
                                {
                                    _timeSinceDash = 0;
                                    // Perform dash action
                                    Stamina -= DifficultySettings.StaminaUsePerDash;
                                    dashtime = 250;
                                    // Reset the timer
                                    _lastSpacePressTime = 0;
                                    _dashSound.Play();
                                }
                                else
                                {
                                    _lastSpacePressTime = currentTime;
                                }*/
            }
            //dashtime = 250; //for debug

            float footstepSoundTimer = 300;
            /*            if (!Slowed)
                        {
                            CharBonusSpeed = DifficultySettings.CharSpeedWithoutSlow + (6.1f * (dashtime / 250));
                            footstepSoundTimer = 300;
                        }
                        else
                        {
                            CharBonusSpeed = DifficultySettings.CharSpeedWithSlow + (4.5f * (dashtime / 250));
                            footstepSoundTimer = 500;
                        }*/
            float laserSoundTimer = 1000;





            #region GamePad Input

            if (!Stopped && !_softStopped)
            {
                CurrentPosition += gamePadState.ThumbSticks.Left * new Vector2(2, -2) * 1.6f * CharBonusSpeed;

                if (gamePadState.ThumbSticks.Left.Y != 0) //up or down
                {
                    _flippingTimer += gameTime.ElapsedGameTime.TotalSeconds;

                    if (_flippingTimer > _flippingSpeed)
                    {
                        if (flipped)
                            flipped = false;
                        else
                            flipped = true;

                        _flippingTimer -= _flippingSpeed;
                    }
                    int MathSignofLeftThumbStickX = Math.Sign(gamePadState.ThumbSticks.Left.X);
                    CurrentPosition.X += (MathSignofLeftThumbStickX - gamePadState.ThumbSticks.Left.X) * 2 * 1.6f * CharBonusSpeed;


                }
                else if (Math.Sign(gamePadState.ThumbSticks.Left.X) >0)
                    flipped = false;
                else flipped = true;
            }
            /*            else
                        {
                            flipped = false;
                        }*/
            #endregion

            #region Keyboard Input

            if (!Stopped && !_softStopped)
            {
                // Apply keyboard movement
                if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
                {
                    //if (position.Y > 170)
                    CurrentPosition += new Vector2(0, -2) * 1.6f * CharBonusSpeed;

                    _flippingTimer += gameTime.ElapsedGameTime.TotalSeconds;

                    if (_flippingTimer > _flippingSpeed)
                    {
                        if (flipped)
                            flipped = false;
                        else
                            flipped = true;

                        _flippingTimer -= _flippingSpeed;
                    }
                }
                if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
                {
                    //if (position.Y < 453)
                    CurrentPosition += new Vector2(0, 2) * 1.6f * CharBonusSpeed;

                    _flippingTimer += gameTime.ElapsedGameTime.TotalSeconds;

                    if (_flippingTimer > _flippingSpeed)
                    {
                        if (flipped)
                            flipped = false;
                        else
                            flipped = true;

                        _flippingTimer -= _flippingSpeed;
                    }

                }
                if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
                {
                    //if (position.X > 20)

                    CurrentPosition += new Vector2(-2, 0) * 1.6f * CharBonusSpeed;
                    if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W)
                        || keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
                    {

                        if (_flippingTimer > _flippingSpeed)
                        {
                            if (flipped)
                                flipped = false;
                            else
                                flipped = true;

                            _flippingTimer -= _flippingSpeed;
                        }
                    }
                    else
                        flipped = true;
                }
                if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
                {
                    //if (position.X < 780)
                    CurrentPosition += new Vector2(2, 0) * 1.6f * CharBonusSpeed;
                    if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W)
                        || keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
                    {

                        if (_flippingTimer > _flippingSpeed)
                        {
                            if (flipped)
                                flipped = false;
                            else
                                flipped = true;

                            _flippingTimer -= _flippingSpeed;
                        }
                    }
                    else
                        flipped = false;
                }


                if (!(keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W)
                        || keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S)
                        || keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D)
                        || keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
                        && gamePadState.ThumbSticks.Left == new Vector2(0, 0)
                        && !(WinManeuver || LossManeuver))
                {

                    _standing = true;
                }
                else _standing = false;
            }
            /*            else
                        {
                            flipped = false;
                        }*/
            #endregion

            //to limit the sprite from getting out of map
            #region Position Offset


            if (CurrentPosition.X < 278)
                CurrentPosition.X += 278 - CurrentPosition.X;

            if (CurrentPosition.X > (MaxOffsetX -207))
                CurrentPosition.X -= CurrentPosition.X - (MaxOffsetX -207);

            if (CurrentPosition.Y < 170)
                CurrentPosition.Y += 170 - CurrentPosition.Y;

            if (CurrentPosition.Y > (MaxOffsetY - 250))
                CurrentPosition.Y -= CurrentPosition.Y - (MaxOffsetY - 250);

            #endregion

            //Update the bounds
            _characterBounds.X = CurrentPosition.X; //1.6f
            _characterBounds.Y = CurrentPosition.Y + 20;    //1.6f

            bool runningOrWeapon = ((_animationFrame >= 0 && _animationFrame <= 5) || (_animationFrame >= 16 && _animationFrame <= 20)) ? true : false;
            if (!flipped)
            {
                if (runningOrWeapon) _characterBounds.X += 29;

                _weaponBounds.X = CurrentPosition.X + 102.2f; //1.6f
                _weaponBounds.Y = CurrentPosition.Y + 65f;    //1.6f

            }
            else
            {
                if (runningOrWeapon) _characterBounds.X -= 16;

                _weaponBounds.X = CurrentPosition.X - 390.2f; //1.6f
                _weaponBounds.Y = CurrentPosition.Y + 65f;    //1.6f
            }


            #region Step Sound
            _soundTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            _laserTimer += gameTime.ElapsedGameTime.TotalMilliseconds;
            _charSpeedLimiter = (CharBonusSpeed - 1) / 4.5f;
            if (_soundTimer * (1 + _charSpeedLimiter) > footstepSoundTimer)
            {
                if (!_standing && !Stopped && !_softStopped)
                {
                    _steps[(int)_stepSound].Play();
                    //if (Slowed) _steps[(int)_stepSound + 2].Play();
                    if (_stepSound == StepSound.Left)
                        _stepSound = StepSound.LightRight;
                    else
                        _stepSound = StepSound.Left;

                }
                _soundTimer = 0;
            }

            if (_laserTimer > 472)
            {
                if (_softStopped && ! Dead)
                {
                    _laserSound.Play();
                }
                _laserTimer = 0;
            }
            #endregion
            float finalspeed = CurrentPosition.X - initSpeed;

            if (_animationFrame == 19|| _animationFrame == 20)
                Damaging = true;
            else 
                Damaging = false;
        }



        /// <summary>
        /// Draws the sprite using the supplied SpriteBatch
        /// </summary>
        /// <param name="gameTime">The game time</param>
        /// <param name="spriteBatch">The spritebatch to render with</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _charSpeedLimiter = (CharBonusSpeed - 1) / 16;

            //if (Slowed) Color = Color.SandyBrown;



            _animationTimer += gameTime.ElapsedGameTime.TotalSeconds * (1 + _charSpeedLimiter);
            if (!Dead)
            {
                if (_animationTimer > _animationSpeed)
                {
                    _animationFrame++;
                    if (_standing == true && !_shooting)
                        _animationFrame = 6;

                    else if (_shooting)
                    {
                        _softStopped = true;
                        if (_animationFrame < 16 || _animationFrame > 20)

                        {
                            _animationFrame = 16;
                        }

                        if ((keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A) || gamePadState.ThumbSticks.Left.X < 0) && _animationFrame <= 17)

                        {
                            flipped = true;
                        }
                        else if ((keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D) || gamePadState.ThumbSticks.Left.X > 0) && _animationFrame <= 17)
                        {
                            flipped = false;
                        }
                    }

                    else if (_animationFrame > 5) // walking
                    {
                        _animationFrame = 0;
                    }

                    if (!_shooting) _softStopped = false;




                    _animationTimer -= _animationSpeed;
                }

            }
            else // stop at animation frame #3 when dead (looks the best) or stay standing if wasn't moving
            {
                //Color = Color.SkyBlue;

                if (_animationTimer > _animationSpeed)
                {
                    if (_animationFrame == 15)
                        ; //do nothing

                    else if (_animationFrame < 6 || _animationFrame >= 16)
                        _animationFrame = 7;

                    else
                    {
                        _animationFrame++;
                    }




                    _animationTimer -= _animationSpeed;
                }
            }

            Rectangle source = new Rectangle();
            if (_animationFrame < 16)
                source = new Rectangle(_animationFrame * 46, 0, 46, 40);
            else
                source = new Rectangle(736 + ((_animationFrame - 16) * 165), 0, 165, 40);

            
            Vector2 origin = (flipped) ? new Vector2(20, 0) : new Vector2(0, 0);

            if (_animationFrame >= 16 && _animationFrame  <=  20 && flipped)
                origin.X += 117;

            SpriteEffects spriteEffects = (flipped) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(_charTexture, CurrentPosition, source, Color, 0, origin, 2.8f, spriteEffects, 0.2f);
        }
    }
}
