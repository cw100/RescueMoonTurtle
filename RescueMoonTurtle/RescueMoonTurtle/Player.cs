using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace RescueMoonTurtle
{
    class Player : Animation
    {
        public PlayerIndex playerNumber;
        public Vector2 gravityCenter;
        public int hp;
        public float rotation;
        GamePadState gamePadState;
        Vector2 stickInputRight, stickInputLeft;
        public int distanceToCenter;
        Texture2D projectileTexture;

        TimeSpan fireRate ;
        TimeSpan previousFireTime;
        public Player(Texture2D texture, Texture2D projectileTexture, TimeSpan fireRate, Vector2 position, Vector2 center, int distanceToCenter, PlayerIndex playerNumber, int hp)
            : base(texture, 1, 1, position, 0f, Color.White)
        {
            this.fireRate = fireRate;
            this.projectileTexture = projectileTexture;
            this.distanceToCenter = distanceToCenter;
            this.gravityCenter = center;
            this.playerNumber = playerNumber;
            this.hp = hp;
            X = center.X + (float)Math.Sin(rotation) * (float)distanceToCenter;
            Y = center.Y - (float)Math.Cos(rotation) * (float)distanceToCenter;
        }
      
        public void Shoot(GameTime gameTime)
        {

            if (gameTime.TotalGameTime - previousFireTime > fireRate)
            {
                if (GamePad.GetState(playerNumber).Triggers.Right >= 0.5f)
                {
                    Vector2 shootOffset = new Vector2(0, frameHeight/2);


                    Vector2 shootPosition = Position + 
                        new Vector2(shootOffset.Length() * (float)Math.Sin((double)angle), -shootOffset.Length() * (float)Math.Cos((double)angle));
                 
                    Projectile projectile = new Projectile(projectileTexture, shootPosition, 1f, angle-(float)Math.PI/2, 10f);
                    previousFireTime = gameTime.TotalGameTime;
                    Game1.projectiles.Add(projectile);
                }
            }
        }
        public void GetInputRight()
        {

            float deadzone = 0.25f;
            stickInputRight = new Vector2(gamePadState.ThumbSticks.Right.X, gamePadState.ThumbSticks.Right.Y);
            if (stickInputRight.Length() < deadzone)
            {
                stickInputRight = Vector2.Zero;
            }
            else
            {
                Vector2 normalizedInput = stickInputRight;
                normalizedInput.Normalize();
                stickInputRight = normalizedInput * ((stickInputRight.Length() - deadzone) / (1 - deadzone));
            }
        }
        public void GetInputLeft()
        {

            float deadzone = 0.25f;
            stickInputLeft = new Vector2(gamePadState.ThumbSticks.Left.X, gamePadState.ThumbSticks.Left.Y);
            if (stickInputLeft.Length() < deadzone)
            {
                stickInputLeft = Vector2.Zero;
            }
            else
            {
                Vector2 normalizedInput = stickInputLeft;
                normalizedInput.Normalize();
                stickInputLeft = normalizedInput * ((stickInputLeft.Length() - deadzone) / (1 - deadzone));
            }
        }
        public static float Lerp(float targetAngle,float currentAngle, float turnSpeed) 
            {

                float difference = WrapAngle(targetAngle - currentAngle); 

                difference = MathHelper.Clamp(difference, -turnSpeed, turnSpeed); 

                return WrapAngle(currentAngle + difference); 
            } 
 
        public static float WrapAngle(float radians) 
                { 
                    while (radians < -MathHelper.Pi) 
                    { 
                        radians += MathHelper.TwoPi; 
                    } 
                    while (radians > MathHelper.Pi) 
                    { 
                        radians -= MathHelper.TwoPi; 
                    } 
                    return radians; 
                } 
        
        public void ControllerMove(GameTime gameTime)
        {

            if (stickInputLeft.X != 0 || stickInputLeft.Y != 0)
            {
                float currentRotation = rotation;

                rotation =  (float)(Math.Atan2(stickInputLeft.X, stickInputLeft.Y));

                rotation = Lerp(rotation, currentRotation, 0.05f);
                
                X = gravityCenter.X + (float)Math.Sin(rotation) * (float)distanceToCenter;
                Y = gravityCenter.Y - (float)Math.Cos(rotation) * (float)distanceToCenter;
            }
        }
        public void GetAngle()
        {
            if (stickInputRight.X != 0 || stickInputRight.Y != 0)
            {
                angle = (float)(Math.Atan2(stickInputRight.X, stickInputRight.Y) );

            }
   
        }

        public override void Update(GameTime gameTime)
        {
            if (hp <= 0)
            {
                active = false;
            }
            if (active)
            {
                gamePadState = GamePad.GetState(playerNumber, GamePadDeadZone.None);
                GetInputLeft();
                GetInputRight();
               
                ControllerMove(gameTime);
                GetAngle();
                Shoot(gameTime);
                base.Update(gameTime);
            }

        }
        public override void Draw(SpriteBatch sb)
        {

            base.Draw(sb);
        }
    }
}
