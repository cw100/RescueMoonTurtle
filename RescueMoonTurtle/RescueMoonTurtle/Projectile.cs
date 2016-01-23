using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace RescueMoonTurtle
{
    public class Projectile : Animation
    {
        public Matrix projectileTransformation;
        public Vector2 prePosition;
        public Vector2 velocity;
        public Rectangle hitBox;
        public float damage;
        float speed;
        Vector2 vectorAngle;
        public Projectile(Texture2D projectileTex, Vector2 position, float speed, float angle, float damage)
            : base(projectileTex, 1, 1, position, 0f, Color.White)
        
        {
            active = true;
            this.position = position;
            this.angle = angle;
            this.speed = speed;
            this.damage = damage;
            vectorAngle = AngleToVector(angle);
            if (vectorAngle.X == 0 && vectorAngle.Y == 0)
            {
                vectorAngle = new Vector2(0, 1);
            }
            vectorAngle.Normalize();
            hitBox = new Rectangle((int)position.X - frameWidth / 2, (int)position.Y -frameHeight / 2, frameWidth, frameHeight);
        }
        Vector2 AngleToVector(float angle)
        {
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }
        public void CheckInsideWindow()
        {
            if(position.X>Game1.windowWidth||position.X<0||
                position.Y>Game1.windowHeight||position.Y<0)
            {
                active = false;
            }
        }
        public void Update(GameTime gameTime)
        {
            prePosition = position;
            velocity = new Vector2(vectorAngle.X,vectorAngle.Y) * speed * gameTime.ElapsedGameTime.Milliseconds;
            position += velocity;
            hitBox.X = (int)position.X - frameWidth / 2;
            hitBox.Y = (int)position.Y - frameHeight / 2;
            
            base.Update(gameTime);
            projectileTransformation =
             Matrix.CreateTranslation(new Vector3(-origin, 0.0f)) *
             Matrix.CreateScale(scale) *
             Matrix.CreateTranslation(new Vector3(position, 0.0f));
            Matrix.CreateTranslation(new Vector3(position, 0.0f));
            CheckInsideWindow();
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
