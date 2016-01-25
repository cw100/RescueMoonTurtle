using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace RescueMoonTurtle
{
    class Turtle : Animation
    {
        public int hp;
        public Vector2 velocity;
        public Vector2 gravityVector;
        public float gravity;
        public int mass;
        public int speed;
        public Vector2 gravityCenter;
        public Turtle(Texture2D texture, Vector2 position, Vector2 center, float gravity, int speed, int mass, int hp)
            : base(texture, 1, 1, position, 0f, Color.White)
        {
            
            this.gravityCenter = center;
            this.speed = speed;
            this.mass = mass;
            this.hp = hp;
            this.gravity = gravity;
            angle = (float)Math.Atan2(center.Y - position.Y, center.X - position.X);
        }
        public void CheckInsideWindow()
        {
            if (Position.X > Game1.windowWidth+100 || X < 0 -100||
                Position.Y > Game1.windowHeight + 100 || Y < 0-100)
            {
                active = false;
            }
        }
        Vector2 AngleToVector(float angle)
        {
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }
        public void ApplyGravity(GameTime gameTime)
        {
            gravityVector = gravityCenter - Position;
            gravityVector.Normalize();
            velocity += gravityVector * gravity * gameTime.ElapsedGameTime.Milliseconds;
        }
        public override void Update(GameTime gameTime)
        {
            ApplyGravity(gameTime);
            
            Position += velocity;
            angle = (float)Math.Atan2(gravityCenter.Y - Y, gravityCenter.X - X);
            hitBox.X = (int)X - frameWidth / 2;
            hitBox.Y = (int)Y - frameHeight / 2;
            CheckInsideWindow();
            base.Update(gameTime);
            
            
        }


        public override void Draw(SpriteBatch sb)
        {
            if (active)
            {
                base.Draw(sb);
            }
        }
    }
}
