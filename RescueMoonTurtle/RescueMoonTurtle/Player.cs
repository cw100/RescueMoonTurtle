using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace RescueMoonTurtle
{
    //Inherits from animation class (EXAMPLE OF INHERITANCE)
    class Player : Animation
    {
        bool keyboardControl;
        public PlayerIndex playerNumber;
        public Vector2 gravityCenter;
        public int hp;
        public float rotation;
        GamePadState gamePadState;
        Vector2 stickInputRight, stickInputLeft;
        public int distanceToCenter;
        Texture2D projectileTexture;
        KeyboardState keyboardState;
        MouseState mouseState;
        TimeSpan fireRate;
        TimeSpan previousFireTime;
        //Constructor creates new player and player animation via the base class constructor, for Xbox control (EXAMPLE OF POLYMOPHISM)
        public Player(Texture2D texture, Texture2D projectileTexture, TimeSpan fireRate, Vector2 position, Vector2 gravityCenter,
            int distanceToCenter, PlayerIndex playerNumber)
            : base(texture, position)
        {
            //Disables keyboard control
            this.keyboardControl = false;
            //Time in milliseconds to fire new projectile
            this.fireRate = fireRate;
            //Texture for projectiles
            this.projectileTexture = projectileTexture;
            //Radius of player orbit
            this.distanceToCenter = distanceToCenter;
            //Center of player orbit
            this.gravityCenter = gravityCenter;
            //Xbox controller player index number (1-4)
            this.playerNumber = playerNumber;
            //Creates player orbit position centered on the gravityCenter, with radius distanceToCenter
            //Position in orbit is based on the rotation, 0 being the top of the orbit, -PI or PI being the bottom
            X = gravityCenter.X + (float)Math.Sin(rotation) * (float)distanceToCenter;
            Y = gravityCenter.Y - (float)Math.Cos(rotation) * (float)distanceToCenter;
        }
        //Constructor creates new player and player animation via the base class constructor, for keyboard control (EXAMPLE OF POLYMOPHISM)
        public Player(Texture2D texture, Texture2D projectileTexture, TimeSpan fireRate, Vector2 position, Vector2 gravityCenter, int distanceToCenter)
            : base(texture, position)
        {
            //Enables keyboard control
            this.keyboardControl = true;
            //Time in milliseconds to fire new projectile
            this.fireRate = fireRate;
            //Texture for projectiles
            this.projectileTexture = projectileTexture;
            //Radius of player orbit
            this.distanceToCenter = distanceToCenter;
            //Center of player orbit
            this.gravityCenter = gravityCenter;
            //Creates player orbit position centered on the gravityCenter, with radius distanceToCenter
            //Position in orbit is based on the rotation, 0 being the top of the orbit, -PI or PI being the bottom
            X = gravityCenter.X + (float)Math.Sin(rotation) * (float)distanceToCenter;
            Y = gravityCenter.Y - (float)Math.Cos(rotation) * (float)distanceToCenter;
        }
        //Creates a new projectile
        public void Shoot(GameTime gameTime)
        {
            //Checks to see if enough time has passed since last projectile
            if (gameTime.TotalGameTime - previousFireTime > fireRate)
            {
                //Creates offset from player position to start the projectile in the correct position inside the gun
                Vector2 shootOffset = new Vector2(frameWidth / 2, frameHeight/2);
                //Creates the vector for the start position of the projectile, based on the length of the offset vectors and the angle to the aimed postion
                Vector2 shootPosition = Position +
                    new Vector2(shootOffset.X * (float)Math.Sin((double)angle), -shootOffset.Y * (float)Math.Cos((double)angle));
                //Creates a projectile aimed in the correct direction, starting at the top on the player gun
                Projectile projectile = new Projectile(projectileTexture, shootPosition, 1f, angle - (float)Math.PI / 2);
                //Adds new projectile to main game list
                Game1.projectiles.Add(projectile);
                //Resets fire time
                previousFireTime = gameTime.TotalGameTime;

            }
        }
        //Gets right Xbox stick input with corrected deadzone
        public void GetInputRight()
        {
            //Deadzone sensitivity 
            float deadzone = 0.25f;
            //Gets current stick position
            stickInputRight = new Vector2(gamePadState.ThumbSticks.Right.X, gamePadState.ThumbSticks.Right.Y);
            //Ignores input if lower than deadzone value
            if (stickInputRight.Length() < deadzone)
            {
                stickInputRight = Vector2.Zero;
            }
            else
            {
                //Creates unit vector from stick input
                Vector2 normalizedInput = stickInputRight;
                normalizedInput.Normalize();
                //Creates smoothed stick input based on deadzone value
                stickInputRight = normalizedInput * ((stickInputRight.Length() - deadzone) / (1 - deadzone));
            }
        }

        //Gets left Xbox stick input with corrected deadzone
        public void GetInputLeft()
        {

            //Deadzone sensitivity 
            float deadzone = 0.25f;
            //Gets current stick position
            stickInputLeft = new Vector2(gamePadState.ThumbSticks.Left.X, gamePadState.ThumbSticks.Left.Y);
            //Ignores input if lower than deadzone value
            if (stickInputLeft.Length() < deadzone)
            {
                stickInputLeft = Vector2.Zero;
            }
            else
            {
                //Creates unit vector from stick input
                Vector2 normalizedInput = stickInputLeft;
                normalizedInput.Normalize();
                //Creates smoothed stick input based on deadzone value
                stickInputLeft = normalizedInput * ((stickInputLeft.Length() - deadzone) / (1 - deadzone));
            }
        }
        //Interpolation for angles
        public static float Lerp(float targetAngle, float currentAngle, float turnSpeed)
        {
            //Calculates difference between current and target angles
            float difference = WrapAngle(targetAngle - currentAngle);
            //Prevents angle change above turn speed in either direction
            difference = MathHelper.Clamp(difference, -turnSpeed, turnSpeed);
            //Returns corrected angle
            return WrapAngle(currentAngle + difference);
        }
        //Allows player to rotate 360 degrees
        public static float WrapAngle(float radians)
        {
            //Wraps negative angles to positive
            while (radians < -MathHelper.Pi)
            {
                radians += MathHelper.TwoPi;
            }
            //Wraps positive angles to negative
            while (radians > MathHelper.Pi)
            {
                radians -= MathHelper.TwoPi;
            }
            return radians;
        }
        //Player movement for Xbox controllers
        public void ControllerMove(GameTime gameTime)
        {
            //Ignores no input to prevent player reseting when no stick input is given
            if (stickInputLeft.X != 0 || stickInputLeft.Y != 0)
            {  
                //Interpolates current angle toward target angle, to limit turn speed
                rotation = Lerp((float)(Math.Atan2(stickInputLeft.X, stickInputLeft.Y)), rotation, 0.05f);
                //Creates player orbit position centered on the gravityCenter, with radius distanceToCenter
                //Position in orbit is based on the rotation, 0 being the top of the orbit, -PI or PI being the bottom
                X = gravityCenter.X + (float)Math.Sin(rotation) * (float)distanceToCenter;
                Y = gravityCenter.Y - (float)Math.Cos(rotation) * (float)distanceToCenter;
            }
        }
        //Player movement for keyboard
        public void KeyboardMove(GameTime gameTime)
        {
            //Moves anti-clockwise by set angle per millisecond
            if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
            {
                rotation -= 0.003f * gameTime.ElapsedGameTime.Milliseconds;
            }
            //Moves clockwise by set angle per millisecond
            if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
            {
                rotation += 0.003f * gameTime.ElapsedGameTime.Milliseconds;
            }
            //Creates player orbit position centered on the gravityCenter, with radius distanceToCenter
            //Position in orbit is based on the rotation, 0 being the top of the orbit, -PI or PI being the bottom
            X = gravityCenter.X + (float)Math.Sin(rotation) * (float)distanceToCenter;
            Y = gravityCenter.Y - (float)Math.Cos(rotation) * (float)distanceToCenter;
        }
        //Gets player projectile aim angle based on mouse or stick input
        public void GetAngle()
        {
            //Ignores no input from right stick, to allow player to keep last shoot angle on no input
            if (stickInputRight.X != 0 || stickInputRight.Y != 0)
            {
                //Gets right stick angle
                angle = (float)(Math.Atan2(stickInputRight.X, stickInputRight.Y));

            }
            //If keyboard control enabled, allow for mouse aim
            if (keyboardControl)
            {
                //Angle toward mouse from player center
                angle = (float)(Math.Atan2(-(X - mouseState.X), Y - mouseState.Y));
            }
        }

        //Player game update logic
        public override void Update(GameTime gameTime)
        {
                //Gets current Xbox controller values based on player index, with no default deadzone
                gamePadState = GamePad.GetState(playerNumber, GamePadDeadZone.None);
                //Add corrected deadzone to inputs
                GetInputLeft();
                GetInputRight();
                //Move player based on controller input
                ControllerMove(gameTime);
                if (keyboardControl)
                {
                    //Gets currect keyboard and mouse values
                    keyboardState = Keyboard.GetState();
                    mouseState = Mouse.GetState();
                    //Move player based on keyboard input
                    KeyboardMove(gameTime);
                }
                //Get current player aim angle
                GetAngle();
                if (keyboardControl)
                {
                    //Tries to shoot on left mouse click
                    if (mouseState.LeftButton == ButtonState.Pressed)
                    {
                        Shoot(gameTime);
                    }
                }
                else
                {
                    //Tries to shoot on right trigger press
                    if (gamePadState.Triggers.Right >= 0.5f)
                    {
                        Shoot(gameTime);
                    }
                }

                //Animation base class update
                base.Update(gameTime);
            

        }

        //Player draw logic
        public override void Draw(SpriteBatch sb)
        {

            //Animation base class draw
            base.Draw(sb);
        }
    }
}
