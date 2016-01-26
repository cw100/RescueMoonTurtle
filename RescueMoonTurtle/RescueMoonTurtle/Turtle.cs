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
    class Turtle : Animation
    {
        public Vector2 velocity;
        public Vector2 gravityVector;
        public float gravity;
        public Vector2 gravityCenter;

        //Constructor creates new turtle and turtle animation via the base class constructor
        public Turtle(Texture2D texture, Vector2 position, Vector2 gravityCenter, float scale, float gravity)
            : base(texture, 1, 1, position, 0f, Color.White)
        {
            //Texture size
            this.scale = scale;
            //Center of gravity, which the turtle object is pulled toward
            this.gravityCenter = gravityCenter;
            //Force of gravity on turtle object
            this.gravity = gravity;
            //Points the turtle towards the center of gravity
            angle = (float)Math.Atan2(gravityCenter.Y - Y, gravityCenter.X - X);
        }
        //Checks if turtle is outside the game window plus a margin so the turtle isn't disabled immediately after being created
        public void CheckInsideWindow()
        {
            if (Position.X > Game1.windowWidth + 1+frameWidth * scale || X < 0 - 1-frameWidth * scale ||
                Position.Y > Game1.windowHeight + 1+frameHeight * scale || Y < 0 -1- frameHeight * scale)
            {
                //Disables turtle if pushed outside the game window
                active = false;
            }
        }
        
        //Moves turtle toward center of gravity, based on gravity value
        public void ApplyGravity(GameTime gameTime)
        {
            //Creates a unit vector pointed toward center of gravity
            gravityVector = gravityCenter - Position;
            gravityVector.Normalize();
            //Adds the force of gravity pointed toward center of gravity for each millisecond passed since the last frame to the velocity vector, creating a accelerating gravity effect
            velocity += gravityVector * gravity * gameTime.ElapsedGameTime.Milliseconds;
        }
        //Turtle game update logic
        public override void Update(GameTime gameTime)
        {
            //Adds gravity vector to velocity
            ApplyGravity(gameTime);
            //Adds velocity to turtle animation position 
            Position += velocity;
            //Keeps turtle angle facing center of gravity
            angle = (float)Math.Atan2(gravityCenter.Y - Y, gravityCenter.X - X);
            //Checks if turtle has been pushed out of game window
            CheckInsideWindow();
            //Animation base class update
            base.Update(gameTime);
            
            
        }

        //Turtle draw logic
        public override void Draw(SpriteBatch sb)
        {

                //Animation base class draw
                base.Draw(sb);
            
        }
    }
}
