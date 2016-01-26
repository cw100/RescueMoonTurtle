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
        public float damage;
        float speed;
        Vector2 vectorAngle;
        public Projectile(Texture2D projectileTex, Vector2 position, float speed, float angle, float damage)
            : base(projectileTex, position,angle,Color.White)
        
        {
            this.angle = angle;
            this.speed = speed;
            this.damage = damage;
            vectorAngle = AngleToVector(angle);
            if (vectorAngle.X == 0 && vectorAngle.Y == 0)
            {
                vectorAngle = new Vector2(0, 1);
            }
            vectorAngle.Normalize();
        }
        Vector2 AngleToVector(float angle)
        {
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }
        public void CheckInsideWindow()
        {
            if(Position.X>Game1.windowWidth||Position.X<0||
                Position.Y>Game1.windowHeight||Position.Y<0)
            {
                active = false;
            }
        }
        public override void Update(GameTime gameTime)
        {
            prePosition = Position;
            velocity = new Vector2(vectorAngle.X,vectorAngle.Y) * speed * gameTime.ElapsedGameTime.Milliseconds;
            Position += velocity;
            hitBox.X = (int)Position.X - frameWidth / 2;
            hitBox.Y = (int)Position.Y - frameHeight / 2;
            
            base.Update(gameTime);
            projectileTransformation =
             Matrix.CreateTranslation(new Vector3(-origin, 0.0f)) *
             Matrix.CreateScale(scale) *
             Matrix.CreateTranslation(new Vector3(Position, 0.0f));
            Matrix.CreateTranslation(new Vector3(Position, 0.0f));
            CheckInsideWindow();
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
