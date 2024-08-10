using Game_Project_4.Collisions;
using Game_Project_4.ParticleManagement;
using Game_Project_4.Sprites;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Project_4.BotAI
{

    public class FinalBoss
    {
        public Vector2 Position { get; set; }

        public Texture2D _attackTexture;
        /*        public Texture2D _runningTexture;
        */
        public CharacterSprite Player { get; set; }
        public float Distance { get; set; } = 250;

        public Vector2 Guard { get; set; } = new(350, 350);

        public float Speed { get; set; } = RandomHelper.NextFloat(225, 295);

        public bool isFighting { get; set; } = false;

        private bool flipped;

        private int _animationFrame;

        private double _flippingTimer;

        private double _animationTimer;

        private float _flippingSpeed = 0.35f;

        public bool Stopped = false;


        private float _animationSpeed = 0.1f;

        private bool _standing = false;

        public bool Dead = false;

        public Color Color = Color.White;

        public bool _approaching = true;

        public bool _charging = false;

        private bool _teleporting = false;

        public float _attackTimerLength;

        public bool _runAnimation = false;

        private float _chargeTimerLength = 670;

        public bool _attacking = false;

        public bool _running = false;

        Vector2 direction;

        public bool Damaging = false;

        private KeyboardState keyboardState;

        public float Health = 250;

        public float AttackDamage = 7;

        KeyboardState _previousKeyboardState;

        public float RespawnTime;

        private BoundingRectangle _attackBounds = new BoundingRectangle(new Vector2(600 - 54, 200 - 56) * 1.6f, 395, 210);

        public bool Initializing = false;

        private float _runningTimerLength;

        /// <summary>
        /// The bounding volume of the sprite's attack
        /// </summary>
        public BoundingRectangle AttackBounds => _attackBounds;

        private BoundingRectangle _characterBounds = new BoundingRectangle(new Vector2(600 - 54, 200 - 56) * 1.6f, 84, 108);

        /// <summary>
        /// The bounding volume of the sprite
        /// </summary>
        public BoundingRectangle CharacterBounds => _characterBounds;

        private bool _allAnimations = false;
        public FinalBoss(int position, float respawnTime)
        {
            RespawnTime = respawnTime;
            if (position == 1)
            {
                Position = new Vector2(300, 180);
            }
            else
            {
                Position = new(1270, RandomHelper.NextFloat(505, 600));
            }
        }

        public void LoadContent(ContentManager content)
        {
            _attackTexture = content.Load<Texture2D>("FinalBoss");
            /*            _runningTexture = content.Load<Texture2D>("run");
            */
        }

        private float _deadTime;
        public void Update(GameTime gameTime)
        {


            /*            if (Initializing & !Stopped)
                        {
                            if (Position.X < 170)
                                Position += new Vector2(0.79f, 0) * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                            else if (Position.X > 1070)
                                Position += new Vector2(-0.79f, 0) * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                            if (Position.Y > 510) Position += new Vector2(0, -0.79f) * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                            if ((Position.X >= 170 && Position.X <= 1070) || direction.Length() < 100)
                                Initializing = false;
                        }*/

            if (Dead)
            {
                _deadTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            #region Debugging Buttons
            _previousKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();
            /*            if (keyboardState.IsKeyDown(Keys.R))
                            Dead = true;
                        if (keyboardState.IsKeyDown(Keys.Q))
                        {
                            Running();
                        }
                        if (keyboardState.IsKeyDown(Keys.W))
                        {
                            Charging();
                        }
                        if (keyboardState.IsKeyDown(Keys.E))
                        {
                            Attacking();
                        }
                        if (keyboardState.IsKeyDown(Keys.F))
                        {
                            flipped = (flipped) ? flipped = false : flipped = true;
                        }*/

            if (keyboardState.IsKeyDown(Keys.X))
            {
                AllAnimations();
                _allAnimations = true;
            }


            #endregion


            if (Health <= 0)
            {
                Dead = true;
                Damaging = false;
            }

            if (Player is null) return;

            /*            if (!(_charging || _attacking)) // if not running, get direction.
                            direction = Player.CurrentPosition - Position;
                        else //else, reduce timer for dash attack
                        {
                            _attackTimer -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                        }*/

            if (_approaching) // if not running, get direction.
                direction = Player.CurrentPosition - Position;
            else if (_charging) //else, reduce timer for dash attack
            {
                direction = Player.CurrentPosition - Position;

                _chargeTimerLength -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }
            else if (_attacking)
                _attackTimerLength -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            else if (_running)
                _runningTimerLength -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;




            /*            if (!_attacking && !_charging) // if not attacking, get direction.
                        {*/
            float maximumDistance = 0;

            #region Flipping And Bounds Logic
            if (flipped)
            {
                maximumDistance = RandomHelper.NextFloat(25, 105);
                _attackBounds.X = Position.X - (139 + 120f); //1.6f
                _attackBounds.Y = Position.Y - 39f;    //1.6f
                _characterBounds.X = Position.X + 120f; //1.6f
                _characterBounds.Y = Position.Y + 31f;    //1.6f

            }
            else
            {
                maximumDistance = RandomHelper.NextFloat(105, 400);
                _attackBounds.X = Position.X + 72 + 81.2f; //1.6f
                _attackBounds.Y = Position.Y - 35;    //1.6f
                _characterBounds.X = Position.X + 81; //1.6f
                _characterBounds.Y = Position.Y + 31f;    //1.6f
            }


            if (direction.X > 0)
            {
                if (!_attacking && !(_animationFrame >= 6 && _animationFrame <= 19) && !Dead)
                    flipped = false;
            }

            else
            {
                if (!_attacking && !(_animationFrame >= 6 && _animationFrame <= 19) && !Dead)
                    flipped = true;
            }

            #endregion

            if (direction.Length() > 400 && !_running && !_charging && !_attacking && !Initializing && !Dead && !Stopped/* && _attackTimer > 750*/) // (if distance is big we follow, else we melee them)
            {   //do the run()
                Charging(); // after the charge, the run will be initiated.
            }
            else if (direction.Length() > 200 && !_running && !_charging && !_attacking && !Initializing && !Dead && !Stopped/* && _attackTimer > 750*/) // (if distance is big we follow, else we melee them)
            {
                Approaching(); // to move
                direction.Normalize();

                Position += direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (direction.Length() < 200 && !_running && !_charging && !_attacking)
            {
                Attacking(); // melee
            }

            if (_charging == true && _chargeTimerLength < -18)
            {
                Running();
            }

            if (_runningTimerLength > 0)
            {
                _running = true;
                _attacking = false;
                _approaching = false;
                _charging = false;
            }
            else if (_running)
            {
                Approaching();
            }

            if (_attackTimerLength > 0)
            {
                _attacking = true;
                _approaching = false;
                _charging = false;
                _running = false;
            }
            else if (_attacking)
            {
                Approaching();
            }




            //Update the bounds




            if (_animationFrame >= 0 && _animationFrame <= 6)
                Damaging = true;
            else
                Damaging = false;
        }







        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _animationTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (!Dead)
            {
                if (_animationTimer > _animationSpeed)
                {
                    _animationFrame++;
                    /*                    if (_standing == true)
                                            _animationFrame = 6;

                                        else*/

                    if (_approaching)
                    {

                        if (_animationFrame < 0 || _animationFrame > 5)
                            _animationFrame = 0;
                    }



                    if (_charging)
                    {

                        if (_animationFrame < 0 || _animationFrame > 5)
                            _animationFrame = 0;
                    }

                    if (_attacking)
                    {

                        if (_animationFrame < 6 || _animationFrame > 19)
                            _animationFrame = 6;
                    }

                    if (_teleporting)

                    {
                        if (_animationFrame < 21 || _animationFrame > 28)
                            _animationFrame = 21;
                    }


                    if (_allAnimations)
                    {
                        if (_animationFrame < 0 || _animationFrame > 36)
                            _animationFrame = 0;
                    }


                    /*                        if (_animationFrame == 26 || _animationFrame )
                                            {
                                                _animationFrame = 20;
                                            }*/


                    _animationTimer -= _animationSpeed;
                }
            }
            else // stop at animation frame #3 when dead (looks the best) or stay standing if wasn't moving
            {

                if (_animationTimer > _animationSpeed && (_deadTime < 3))
                {
                    if (_animationFrame == 37)
                        ; //do nothing

                    else if (_animationFrame < 31 || _animationFrame > 36)
                        _animationFrame = 31;

                    else
                    {
                        _animationFrame++;
                    }

                    _animationTimer -= _animationSpeed;
                }

/*                if (_deadTime > 3.21)
                    _animationFrame = -1;
                else if ((_deadTime > 3.16 && _deadTime < 3.21f) || (_deadTime > 3.06 && _deadTime < 3.11f) || (_deadTime > 2.90 && _deadTime <= 2.98f) || (_deadTime > 2.70 && _deadTime <= 2.80f) || (_deadTime > 2.40 && _deadTime <= 2.55f) || (_deadTime > 2 && _deadTime <= 2.25f))
                {
                    _animationFrame = -1;
                }
                else if (_animationFrame == -1)
                    _animationFrame = 19;*/

            }
            if (keyboardState.IsKeyDown(Keys.Q))
                _animationFrame = 19;
            if (keyboardState.IsKeyDown(Keys.W))
                _animationFrame = 20;
            if (keyboardState.IsKeyDown(Keys.E))
                _animationFrame = 21;
            if (keyboardState.IsKeyDown(Keys.R))
                _animationFrame = 22;
            if (keyboardState.IsKeyDown(Keys.T))
                _animationFrame = 23;
            if (keyboardState.IsKeyDown(Keys.Y))
                _animationFrame = 24;
            if (keyboardState.IsKeyDown(Keys.U))
                _animationFrame = 25;
            if (keyboardState.IsKeyDown(Keys.I))
                _animationFrame = 26;
            if (keyboardState.IsKeyDown(Keys.O))
                _animationFrame = 27;
            if (keyboardState.IsKeyDown(Keys.P))
                _animationFrame = 28;
            if (keyboardState.IsKeyDown(Keys.OemOpenBrackets))
                _animationFrame = 29;
            if (keyboardState.IsKeyDown(Keys.OemCloseBrackets))
                _animationFrame = 30;
            /*            if (_animationFrame == 23)
            */
            Rectangle source = new();
            Vector2 origin = (flipped) ? new Vector2(300, 0) : new Vector2(0, 0);
/*            if (_animationFrame == 17)
            {
                source = new Rectangle(0, (_animationFrame * 96), 200, 96);
                _animationSpeed = 0.06f;

            }*/

/*            else if (!(_animationFrame >= 24 && _animationFrame <= 29))*/
                source = new Rectangle(0, (_animationFrame * 176), 548, 176);
/*
            else if (_animationFrame == 24)
            {
                source = new Rectangle(0, 2312, 200, 70);
                origin.Y -= 8;
            }
            else if (_animationFrame == 25)
            {
                source = new Rectangle(0, 2384, 200, 70);
                origin.Y += 16;
            }
            else if (_animationFrame == 26)
            {
                source = new Rectangle(0, 2480, 200, 70);
                origin.Y += 16;
            }
            else if (_animationFrame == 27)
            {
                source = new Rectangle(0, 2577, 200, 69);
                origin.Y += 15;
            }
            else if (_animationFrame == 28)
            {
                _animationSpeed = 0.1f;
                source = new Rectangle(0, 2673, 200, 70);
                origin.Y += 15;
            }
            else if (_animationFrame == 29)
            {
                source = new Rectangle(0, 2755, 200, 83);
                origin.Y += 29;

            }*/
            /*           if (_animationFrame == 0) // melee
                            source = new Rectangle(0, 0, 505, 172);
                        else if (_animationFrame == 1)
                            source = new Rectangle(0, 178, 505, 176);
                        else if (_animationFrame == 2)
                            source = new Rectangle(0, 374, 505, 191);
                        else if (_animationFrame == 3)
                            source = new Rectangle(0, 572, 505, 174);
                        else if (_animationFrame == 4)
                            source = new Rectangle(0, 753, 505, 204);
                        else if (_animationFrame == 5)
                            source = new Rectangle(0, 965, 505, 196);
                        else if (_animationFrame == 6)
                            source = new Rectangle(0, 1162, 505, 195);
                        else if (_animationFrame == 7) // end of melee
                            source = new Rectangle(0, 1369, 505, 182);
                        else if (_animationFrame == 8) // charge
                            source = new Rectangle(0, 1581, 505, 150);
                        else if (_animationFrame == 9)
                            source = new Rectangle(0, 1731, 505, 150);
                        else if (_animationFrame == 10)
                            source = new Rectangle(0, 1881, 505, 150);
                        else if (_animationFrame == 11) //end of charge
                            source = new Rectangle(0, 2031, 505, 150);
                        else if (_animationFrame == 12) //death
                            source = new Rectangle(0, 2335, 505, 163);
                        else if (_animationFrame == 13)
                            source = new Rectangle(0, 2498, 505, 163);
                        else if (_animationFrame == 14)
                            source = new Rectangle(0, 2661, 505, 163);
                        else if (_animationFrame == 15)
                            source = new Rectangle(0, 2824, 505, 163);
                        else if (_animationFrame == 16)
                            source = new Rectangle(0, 2987, 505, 163);
                        else if (_animationFrame == 17)
                            source = new Rectangle(0, 3150, 505, 163);
                        else if (_animationFrame == 18)
                            source = new Rectangle(0, 3313, 505, 163);
                        else if (_animationFrame == 19) //end of death
                            source = new Rectangle(0, 3476, 505, 181);
                        else if (_animationFrame == 28) // white death sprite
                            source = new Rectangle(0, 3635, 505, 181);
                        else if (_animationFrame == 20) //walk
                            source = new Rectangle(0 * 216, 3895, 216, 180);
                        else if (_animationFrame == 21)
                            source = new Rectangle(1 * 216, 3895, 216, 180);
                        else if (_animationFrame == 22)
                            source = new Rectangle(2 * 216, 3895, 216, 180);
                        else if (_animationFrame == 23)
                            source = new Rectangle(3 * 216, 3895, 216, 180);
                        else if (_animationFrame == 24)
                            source = new Rectangle(4 * 216, 3895, 216, 180);
                        else if (_animationFrame == 25)
                            source = new Rectangle(5 * 216, 3895, 216, 180);
                        else if (_animationFrame == 26)
                            source = new Rectangle(6 * 216, 3895, 216, 180);
                        else if (_animationFrame == 27) //end of walk
                            source = new Rectangle(7 * 216, 3895, 216, 180);
            */
            /*            if (_animationFrame >= 20 && _animationFrame <= 27)
                            origin.Y += 30;

                        if (_animationFrame >= 0 && _animationFrame <= 19 && flipped)
                        {
                            origin.X += 290;
                        }*/



            SpriteEffects spriteEffects = (flipped) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(_attackTexture, Position, source, Color, 0, origin, 1.81f, spriteEffects, 0.2f);

        }

        public void Charging()
        {
            _charging = true;
            _approaching = false;
            _attacking = false;
            _chargeTimerLength = 670;
            _running = false;
        }

        public void Attacking()
        {
            _attacking = true;
            _approaching = false;
            _charging = false;
            _running = false;
            _attackTimerLength = 900;
        }

        public void Approaching()
        {
            _approaching = true;
            _attacking = false;
            _charging = false;
            _running = false;

        }

        public void Running()
        {
            _running = true;
            _approaching = false;
            _attacking = false;
            _charging = false;
            _runningTimerLength = 600;
        }

        public void AllAnimations()
        {
            _approaching = false;
            _attacking = false;
            _charging = false;
            _running = false;
        }

    }
}
