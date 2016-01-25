using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace RescueMoonTurtle
{
    public class Animation
    {
        public Rectangle hitBox;
        public bool reversed =false;
        public float scale = 1f;
        public bool active = true;
        public SpriteEffects flip;
        public Texture2D spriteSheet;
        float time;
        float frameTime=1;
        public int frameIndex;
        int totalFrames=1;
        public int frameHeight;
        public int frameWidth;
        private Vector2 position;

        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        public float X
        {
            get { return position.X; }
            set { position.X = value; }
        }
        public float Y
        {
            get { return position.Y; }
            set { position.Y = value; }
        }
        public Vector2 origin;
        Rectangle source;
        public float angle;
        public Color color;
        public bool isPaused = false;
        public float offset=1;
        public Color[] colorData;
        public bool runOnce = false;
        public Matrix transformation;
        public void collisionData()
        {
            colorData = new Color[frameWidth * frameHeight];
            spriteSheet.GetData<Color>(0, source, colorData, 0, frameWidth * frameHeight);
        }
       

        public Animation(Texture2D spritesheet, int totalframes, float animationlength, Vector2 startposition, float startangle, Color startcolor)
        {
            active = true;
            spriteSheet = spritesheet;
            frameHeight = spriteSheet.Height;
            frameWidth = spriteSheet.Width / totalFrames;
            collisionData();

            totalFrames = totalframes;
            frameTime = animationlength / totalframes;
            position = startposition;
            angle = startangle;
            color = startcolor;

            hitBox = new Rectangle((int)X - frameWidth / 2, (int)Y - frameHeight / 2, frameWidth, frameHeight);
        }
      
  

        public virtual void Update(GameTime gameTime)
        {
            if (active)
            {
                frameHeight = spriteSheet.Height;
                frameWidth = spriteSheet.Width / totalFrames;
                time += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (!isPaused)
                {
                    while (time > frameTime)
                    {
                        if (!reversed)
                        {
                            frameIndex++;
                        }
                        else
                        {
                            frameIndex--;
                        }

                        if (frameIndex == totalFrames)
                        {
                            if (!runOnce)
                            {
                                if (!reversed)
                                {
                                    frameIndex = 0;
                                }
                                else
                                {
                                    frameIndex = totalFrames - 1;
                                }

                                
                            }
                            else
                            {
                                frameIndex = totalFrames-1;
                                isPaused = true;
                                active = false;
                            }

                        }
                        time = 0f;
                    }
                }
                if (!reversed)
                {
                    if (frameIndex > totalFrames)
                    {
                        frameIndex = 1;
                    }
                }
                else
                {
                    if (frameIndex < 1)
                    {
                        frameIndex = totalFrames -1;
                    }
                }
                source = new Rectangle(frameIndex * frameWidth, 0, frameWidth, frameHeight);
                origin = new Vector2((frameWidth / 2.0f), (frameHeight / 2.0f));
                
                transformation =
             Matrix.CreateTranslation(new Vector3(-origin, 0.0f)) *
             Matrix.CreateScale(scale) *
             Matrix.CreateRotationZ(angle) *
             Matrix.CreateTranslation(new Vector3(position, 0.0f));
                
            
            }
            else
            {
                frameIndex = 0;
                isPaused = false;
            }
            
        }
     

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (active)
            {

                spriteBatch.Draw(spriteSheet, position, source, color, angle,
                  origin, scale, flip, 0f);
            }
        }
    }
}
