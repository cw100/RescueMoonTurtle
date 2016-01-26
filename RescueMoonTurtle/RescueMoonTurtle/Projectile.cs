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
    public class Projectile : Animation
    {
        public Vector2 velocity;
        float speed;
        Vector2 vectorAngle;
        //Constructor creates new projectile and projectile animation via the base class constructor
        public Projectile(Texture2D projectileTex, Vector2 position, float speed, float angle)
            : base(projectileTex, position,angle,Color.White)
        
        {
            //Angle projectile is fired at
            this.angle = angle;
            //Projectile speed per millisecond
            this.speed = speed;
            //Turns angle into vector for projectile movement
            vectorAngle = AngleToVector(angle);
            //If the vector is 0, creates a default unit vector
            if (vectorAngle.X == 0 && vectorAngle.Y == 0)
            {
                vectorAngle = new Vector2(0, 1);
            }
            //Creates unit vector
            vectorAngle.Normalize();
        }
        //Turns angle into vector
        Vector2 AngleToVector(float angle)
        {
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }
        //Disables projectile after leaving game window
        public void CheckInsideWindow()
        {
            if(Position.X>Game1.windowWidth||Position.X<0||
                Position.Y>Game1.windowHeight||Position.Y<0)
            {
                active = false;
            }
        }
        //Projectile game update logic
        public override void Update(GameTime gameTime)
        {
            //Creates velocity, in the direction of the angle vector, with a length of projectiles speed for every millisecond since last frame
            velocity = vectorAngle * speed * gameTime.ElapsedGameTime.Milliseconds;

            //Adds velocity to projectile animation position 
            Position += velocity;
            //Disables projectile after leaving game window
            CheckInsideWindow();
            //Animation base class update
            base.Update(gameTime);
           
            
        }

        //Projectile draw logic
        public override void Draw(SpriteBatch sb)
        {
            //Animation base class draw
                base.Draw(sb);
        }
        
    }
}
