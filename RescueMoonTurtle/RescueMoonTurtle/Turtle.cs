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
        public Rectangle hitBox;
        public Vector2 velocity;
        public Vector2 gravityVector;
        public float gravity;
        public int mass;
        public Vector2 vectorAngle;
        public int speed;
        Random random;
        public Vector2 center;
        public Matrix turtleTransformation;
        public Turtle(Texture2D texture, Vector2 pos, Vector2 center, float gravity, int speed, int mass, int hp)
            : base(texture, 1, 1, pos, 0f, Color.White)
        {
            
            this.center = center;
            this.speed = speed;
            this.mass = mass;
            this.hp = hp;
            this.gravity = gravity;
            angle = (float)Math.Atan2(center.Y - pos.Y, center.X - pos.X);
            hitBox = new Rectangle((int)position.X - frameWidth / 2, (int)position.Y - frameHeight / 2, frameWidth, frameHeight);
        }
        public void CheckInsideWindow()
        {
            if (position.X > Game1.windowWidth+100 || position.X < 0 -100||
                position.Y > Game1.windowHeight + 100 || position.Y < 0-100)
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
            gravityVector = center - position;
            gravityVector.Normalize();
            velocity += gravityVector * gravity * gameTime.ElapsedGameTime.Milliseconds;
        }
        public void Update(GameTime gameTime)
        {
            ApplyGravity(gameTime);
            position += velocity;
            hitBox.X = (int)position.X - frameWidth / 2;
            hitBox.Y = (int)position.Y - frameHeight / 2;
            CheckInsideWindow();
            base.Update(gameTime);
            turtleTransformation =
             Matrix.CreateTranslation(new Vector3(-origin, 0.0f)) *
             Matrix.CreateScale(scale) *
             Matrix.CreateRotationZ(angle)*
             Matrix.CreateTranslation(new Vector3(position, 0.0f));
            Matrix.CreateTranslation(new Vector3(position, 0.0f));
            
        }


        public void Draw(SpriteBatch sb)
        {
            if (active)
            {
                base.Draw(sb);
            }
        }
    }
}
