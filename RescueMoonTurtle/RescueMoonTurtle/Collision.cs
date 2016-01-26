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

        public static bool CollidesWith(Animation objectA, Animation objectB)
        {
            return CollidesWith(objectA, objectB, true);
        }
      
        public static bool CollidesWith(Animation objectA, Animation objectB, bool calcPerPixel)
        {


            Rectangle objectARectangle = CalculateBoundingRectangle(
                     new Rectangle(0, 0, objectA.frameWidth, objectA.frameHeight),
                     objectA.transformation);

            
            Rectangle objectBRectangle = CalculateBoundingRectangle(
                    new Rectangle(0, 0, objectB.frameWidth, objectB.frameHeight),
                    objectB.transformation);


            if (objectBRectangle.Intersects(objectARectangle))
            {
                objectA.collisionData();
                objectB.collisionData();
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

        public static Rectangle CalculateBoundingRectangle(Rectangle rectangle, Matrix transform)
        {

            Vector2 leftTop = new Vector2(rectangle.Left, rectangle.Top);
            Vector2 rightTop = new Vector2(rectangle.Right, rectangle.Top);
            Vector2 leftBottom = new Vector2(rectangle.Left, rectangle.Bottom);
            Vector2 rightBottom = new Vector2(rectangle.Right, rectangle.Bottom);
            Vector2.Transform(ref leftTop, ref transform, out leftTop);
            Vector2.Transform(ref rightTop, ref transform, out rightTop);
            Vector2.Transform(ref leftBottom, ref transform, out leftBottom);
            Vector2.Transform(ref rightBottom, ref transform, out rightBottom);

            Vector2 min = Vector2.Min(Vector2.Min(leftTop, rightTop),
                                      Vector2.Min(leftBottom, rightBottom));
            Vector2 max = Vector2.Max(Vector2.Max(leftTop, rightTop),
                                      Vector2.Max(leftBottom, rightBottom));

            return new Rectangle((int)min.X, (int)min.Y,
                                 (int)(max.X - min.X), (int)(max.Y - min.Y));
        }



        public static bool IntersectPixels(Matrix transformA, int widthA, int heightA, Color[] dataA,
                                           Matrix transformB, int widthB, int heightB, Color[] dataB)
        {
            Matrix transformAToB = transformA * Matrix.Invert(transformB);

            Vector2 stepX = Vector2.TransformNormal(Vector2.UnitX, transformAToB);
            Vector2 stepY = Vector2.TransformNormal(Vector2.UnitY, transformAToB);

            Vector2 yPositionB = Vector2.Transform(Vector2.Zero, transformAToB);

            for (int yA = 0; yA < heightA; yA++)
            {
                Vector2 positionB = yPositionB;

                for (int xA = 0; xA < widthA; xA++)
                {
                    int xB = (int)Math.Round(positionB.X);
                    int yB = (int)Math.Round(positionB.Y);

                    if (0 <= xB && xB < widthB &&
                        0 <= yB && yB < heightB)
                    {
                        Color colorA = dataA[xA + yA * widthA];
                        Color colorB = dataB[xB + yB * widthB];

                        if (colorA.A != 0 && colorB.A != 0)
                        {
                            collisionVector = returnCollision(xB, yB, transformB);
                            return true;
                        }
                    }
                    positionB += stepX;
                }

                yPositionB += stepY;
            }

            return false;
        }
      
        public static Vector2 returnCollision(int x, int y, Matrix transformB)
        {
            collisionVector = Vector2.Transform(new Vector2(x, y), transformB);
            return collisionVector;
        }


       
    }
}
