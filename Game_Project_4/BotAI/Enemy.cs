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

    public class Enemy
    {
        public Vector2 Position { get; set; }

        public Texture2D _attackTexture;
/*        public Texture2D _runningTexture;
*/        public CharacterSprite Player { get; set; }
        public float Distance { get; set; } = 250;

        public Vector2 Guard { get; set; } = new(350, 350);

        public float Speed { get; set; } = RandomHelper.NextFloat(225,295);

        public bool isFighting { get; set; } = false;

        private bool flipped;

        private int _animationFrame;

        private double _flippingTimer;

        private double _animationTimer;

        private float _flippingSpeed = 0.35f;

        public bool Stopped = true;

        private float _animationSpeed = 0.1f;

        private bool _standing = false;

        public bool Dead = false;

        public Color Color = Color.White;

        public bool _running = true;

        public bool _charging = false;

        public float _attackTimerLength;

        public bool _runAnimation = false;

        private float _chargeTimerLength = 800;

        public bool _attacking = false;

        Vector2 direction;

        public bool Damaging = false;

        private KeyboardState keyboardState;

        public float Health = 250;

        public float AttackDamage = 7;

        KeyboardState _previousKeyboardState;

        public float RespawnTime;

        private BoundingRectangle _attackBounds = new BoundingRectangle(new Vector2(600 - 54, 200 - 56) * 1.6f, 160, 65);

        public bool Initializing = true;

        /// <summary>
        /// The bounding volume of the sprite's attack
        /// </summary>
        public BoundingRectangle AttackBounds => _attackBounds;

        private BoundingRectangle _characterBounds = new BoundingRectangle(new Vector2(600 - 54, 200 - 56) * 1.6f, 80, 75);

        /// <summary>
        /// The bounding volume of the sprite
        /// </summary>
        public BoundingRectangle CharacterBounds => _characterBounds;


        public Enemy(int position, float respawnTime)
        {
            RespawnTime = respawnTime;
            if (position == 1)
            {
                Position = new(-140, RandomHelper.NextFloat(505, 600));
            }
            else
            {
                Position = new(1270, RandomHelper.NextFloat(505, 600));
            }
        }

        public void LoadContent(ContentManager content)
        {
            _attackTexture = content.Load<Texture2D>("enemy");
/*            _runningTexture = content.Load<Texture2D>("run");
*/        }

        private float _deadTime;
        public void Update(GameTime gameTime)
        {


            if (Initializing &!Stopped)
            {
                if (Position.X < 170)
                    Position += new Vector2(0.79f, 0) * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                else if (Position.X > 1070)
                    Position += new Vector2(-0.79f, 0) * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (Position.Y > 510) Position += new Vector2(0, -0.79f) * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

                if ((Position.X >= 170 && Position.X <= 1070) || direction.Length() < 100)
                    Initializing = false;
            }

            if (Dead)
            {
                _deadTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

/*            #region Debugging Buttons
            _previousKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.R))
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
            }

            #endregion
*/

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

            if (_running) // if not running, get direction.
                direction = Player.CurrentPosition - Position;
            else if (_charging) //else, reduce timer for dash attack
            {
                /*                direction = Player.CurrentPosition - Position;
                */
                _chargeTimerLength -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }
            else if (_attacking)
                _attackTimerLength -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;




            /*            if (!_attacking && !_charging) // if not attacking, get direction.
                        {*/
            float maximumDistance = 0;

            #region Flipping And Bounds Logic
            if (flipped)
            {
                maximumDistance = RandomHelper.NextFloat(25, 105);
                _attackBounds.X = Position.X - 111f; //1.6f
                _attackBounds.Y = Position.Y + 31f;    //1.6f
                _characterBounds.X = Position.X + 40f; //1.6f
                _characterBounds.Y = Position.Y + 5f;    //1.6f

            }
            else
            {
                maximumDistance = RandomHelper.NextFloat(40, 190);
                _attackBounds.X = Position.X + 118.2f; //1.6f
                _attackBounds.Y = Position.Y + 31f;    //1.6f
                _characterBounds.X = Position.X + 40f; //1.6f
                _characterBounds.Y = Position.Y + 5f;    //1.6f
            }


            if (direction.X > 0)
            {
                if (!_attacking && _animationFrame > 7 && !Dead)
                    flipped = false;
            }

            else
            {
                if (!_attacking && _animationFrame > 7 && !Dead)
                    flipped = true;
            }

            #endregion

            if (direction.Length() > maximumDistance && !_charging && !_attacking && !Initializing && !Dead && !Stopped/*&& _attackTimer > 750*/) // (if distance is big we follow, else we melee them)
            {   //do the run()
                Running();
                direction.Normalize();

                Position += direction * Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (direction.Length() < maximumDistance && !_charging && !_attacking)
            {
                Charging();
            }
            // else {do the meleeSmash()}
            /*            }*/



            /*            if (_charging == true && _chargeTimer > 18) // if we initiated attack and timer is not ready, we stand still.
                        {
            *//*                _charging = true;
                            _attacking = false;
                            _running = false;*//*
                        }
                        else*/
            if (_charging == true && _chargeTimerLength < -18)
            {
                Attacking();
            }

            if (_attackTimerLength > 0)
            {
                _attacking = true;
                _running = false;
                _charging = false;
            }
            else if (_attacking)
            {
                Running();
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

                    if (_running)
                    {

                        if (_animationFrame < 20 || _animationFrame > 27)
                            _animationFrame = 20;
                    }

                    if (_charging)
                    {

                        if (_animationFrame < 8 || _animationFrame > 11)
                            _animationFrame = 8;
                    }

                    if (_attacking)
                    {

                        if (_animationFrame < 0 || _animationFrame > 7)
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
                    if (_animationFrame == 19)
                        ; //do nothing

                    else if (_animationFrame < 12 || _animationFrame > 19)
                        _animationFrame = 12;

                    else
                    {
                        _animationFrame++;
                    }

                    _animationTimer -= _animationSpeed;
                }

                if (_deadTime > 3.21)
                    _animationFrame = -1;
                else if ((_deadTime > 3.16 && _deadTime < 3.21f) || (_deadTime > 3.06 && _deadTime < 3.11f) || (_deadTime > 2.90 && _deadTime <= 2.98f) || (_deadTime > 2.70 && _deadTime <= 2.80f) || (_deadTime > 2.40 && _deadTime <= 2.55f) || (_deadTime > 2 && _deadTime <= 2.25f))
                {
                    _animationFrame = -1;
                }
                else if (_animationFrame == -1)
                    _animationFrame = 19;

            }
            Rectangle source = new();
            if (_animationFrame == 0) // melee
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

            Vector2 origin = (flipped) ? new Vector2(-72, 0) : new Vector2(0, 0);
            if (_animationFrame >= 20 && _animationFrame <= 27)
                origin.Y += 30;

            if (_animationFrame >= 0 && _animationFrame <= 19 && flipped)
            {
                origin.X += 290;
            }



            SpriteEffects spriteEffects = (flipped) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(_attackTexture, Position, source, Color, 0, origin, 0.6f, spriteEffects, 0.2f);

        }

        public void Charging()
        {
            _charging = true;
            _running = false;
            _attacking = false;
            _chargeTimerLength = 800;

        }

        public void Attacking()
        {
            _attacking = true;
            _running = false;
            _charging = false;
            _attackTimerLength = 600;
        }

        public void Running()
        {
            _running = true;
            _attacking = false;
            _charging = false;
        }

    }
}
