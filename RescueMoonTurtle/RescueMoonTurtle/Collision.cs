using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace RescueMoonTurtle
{
    class Collision
    {

        static public Vector2 collisionVector;

        //Checks for collision between two animations, by pixel alpha values (EXAMPLE OF POLYMOPHISM)
        public static bool CollidesWith(Animation objectA, Animation objectB)
        {
            return CollidesWith(objectA, objectB, true);
        }
        //Checks for collision between two animations, either by hitbox or pixel alpha values (EXAMPLE OF POLYMOPHISM)
        public static bool CollidesWith(Animation objectA, Animation objectB, bool calcPerPixel)
        {

            //Calculates current smallest rectangle around animation, based on animations transformation matrix
            Rectangle objectARectangle = CalculateBoundingRectangle(
                     new Rectangle(0, 0, objectA.frameWidth, objectA.frameHeight),
                     objectA.transformation);

            //Calculates current smallest rectangle around animation, based on animations transformation matrix
            Rectangle objectBRectangle = CalculateBoundingRectangle(
                    new Rectangle(0, 0, objectB.frameWidth, objectB.frameHeight),
                    objectB.transformation);

            //Checks hitbox collision of animations
            if (objectBRectangle.Intersects(objectARectangle))
            {
                //Gets current animation frames color data array
                objectA.collisionData();
                objectB.collisionData();
                //Checks if any animation pixels overlap, based on alpha values
                if (IntersectPixels(objectA.transformation, objectA.frameWidth,
                                    objectA.frameHeight, objectA.colorData,
                                    objectB.transformation, objectB.frameWidth,
                                    objectB.frameHeight, objectB.colorData))
                {
                    return true;
                }
            }


            return false;

        }
        //Calculates current smallest rectangle around animation, based on animations transformation matrix
        public static Rectangle CalculateBoundingRectangle(Rectangle rectangle, Matrix transform)
        {
            //Calculates vectors of rectangle corners 
            Vector2 leftTop = new Vector2(rectangle.Left, rectangle.Top);
            Vector2 rightTop = new Vector2(rectangle.Right, rectangle.Top);
            Vector2 leftBottom = new Vector2(rectangle.Left, rectangle.Bottom);
            Vector2 rightBottom = new Vector2(rectangle.Right, rectangle.Bottom);
            //Transforms corners by current animation transformation matrix
            Vector2.Transform(ref leftTop, ref transform, out leftTop);
            Vector2.Transform(ref rightTop, ref transform, out rightTop);
            Vector2.Transform(ref leftBottom, ref transform, out leftBottom);
            Vector2.Transform(ref rightBottom, ref transform, out rightBottom);
            //Calculates highest point,closest to left point
            Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop),
                                      Vector2.Min(leftBottom, rightBottom));
            //Calculates lowest, furthest from left point
            Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop),
                                      Vector2.Max(leftBottom, rightBottom));
            //Creates smallest rectangle that fits the animation
            return new Rectangle((int)min.X, (int)min.Y,
                                 (int)(max.X - min.X), (int)(max.Y - min.Y));
        }



        //Checks if any animation pixels overlap, based on alpha values
        public static bool IntersectPixels(Matrix transformA, int widthA, int heightA, Color[] dataA,
                                           Matrix transformB, int widthB, int heightB, Color[] dataB)
        {
            //Creates transformation matrix from both animation transformations
            Matrix transformAToB = transformA * Matrix.Invert(transformB);
            //Creates transformation normals for x and y
            Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, transformAToB);
            Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, transformAToB);
           
            Vector2 yPositionB = Vector2.Transform(Vector2.Zero, transformAToB);
            //Converts transformed color arrays into normailzed color arrays
            for (int yA = 0; yA < heightA; yA++)
            {
                Vector2 positionB = yPositionB;

                for (int xA = 0; xA < widthA; xA++)
                {
                    int xB = (int)Math.Round(positionB.X);
                    int yB = (int)Math.Round(positionB.Y);
                    //Checks if pixel is inside animation
                    if (0 <= xB && xB < widthB &&
                        0 <= yB && yB < heightB)
                    {
                        //Creates new normailzed color arrays
                        Color colorA = dataA[xA + yA * widthA];
                        Color colorB = dataB[xB + yB * widthB];
                        //Checks if alpha values are both none-zone
                        if (colorA.A != 0 && colorB.A != 0)
                        {
                            //Returns location of collision
                            collisionVector = returnCollision(xB, yB, transformB);
                            return true;
                        }
                    }
                    //Moves along transformed normal
                    positionB += stepX;
                }
                //Moves along transformed normal
                yPositionB += stepY;
            }

            return false;
        }
        //Calculates location of collision by transforming point back from normalized array
        public static Vector2 returnCollision(int x, int y, Matrix transformB)
        {
            collisionVector = Vector2.Transform(new Vector2(x, y), transformB);
            return collisionVector;
        }


       
    }
}
