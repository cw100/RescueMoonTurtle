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

        public static bool CollidesWith(Animation objectA, Animation objectB, Matrix aTransform, Matrix bTransform)
        {
            return CollidesWith(objectA, objectB, aTransform, bTransform, true);
        }
        public static bool CollidesWith(Animation objectA, Animation objectB, Matrix aTransform, Matrix bTransform, Point lineorigin, Point lineend)
        {
            return CollidesWith(objectA, objectB, aTransform, bTransform, true);
        }

        public static bool CollidesWith(Animation objectA, Animation objectB, Matrix aTransform, Matrix bTransform, bool calcPerPixel)
        {


            Rectangle objectARectangle = CalculateBoundingRectangle(
                     new Rectangle(0, 0, objectA.frameWidth, objectA.frameHeight),
                     aTransform);


            Rectangle objectBRectangle = CalculateBoundingRectangle(
                    new Rectangle(0, 0, objectB.frameWidth, objectB.frameHeight),
                    bTransform);


            if (objectBRectangle.Intersects(objectARectangle))
            {
                objectA.collisionData();
                objectB.collisionData();
                if (IntersectPixels(aTransform, objectA.frameWidth,
                                    objectA.frameHeight, objectA.colorData,
                                    bTransform, objectB.frameWidth,
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
        public static bool LineIntersectsRect(Point p2, Point p1, Rectangle r)
        {
            return LineIntersectsLine(p1, p2, new Point(r.X, r.Y), new Point(r.X + r.Width, r.Y)) ||
                   LineIntersectsLine(p1, p2, new Point(r.X + r.Width, r.Y), new Point(r.X + r.Width, r.Y + r.Height)) ||
                   LineIntersectsLine(p1, p2, new Point(r.X + r.Width, r.Y + r.Height), new Point(r.X, r.Y + r.Height)) ||
                   LineIntersectsLine(p1, p2, new Point(r.X, r.Y + r.Height), new Point(r.X, r.Y)) ||
                   r.Contains(p1) || r.Contains(p2);
        }

        private static bool LineIntersectsLine(Point l1p1, Point l1p2, Point l2p1, Point l2p2)
        {
            float q = (l1p1.Y - l2p1.Y) * (l2p2.X - l2p1.X) - (l1p1.X - l2p1.X) * (l2p2.Y - l2p1.Y);
            float d = (l1p2.X - l1p1.X) * (l2p2.Y - l2p1.Y) - (l1p2.Y - l1p1.Y) * (l2p2.X - l2p1.X);

            if (d == 0)
            {
                return false;
            }

            float r = q / d;

            q = (l1p1.Y - l2p1.Y) * (l1p2.X - l1p1.X) - (l1p1.X - l2p1.X) * (l1p2.Y - l1p1.Y);
            float s = q / d;

            if (r < 0 || r > 1 || s < 0 || s > 1)
            {
                return false;
            }

            return true;
        }
        public static Vector2 returnCollision(int x, int y, Matrix transformB)
        {
            collisionVector = Vector2.Transform(new Vector2(x, y), transformB);
            return collisionVector;
        }


        public static bool IsAboveAC(Rectangle collsionHitBox, Vector2 playervector)
        {
            return IsOnUpperSideOfLine(GetBottomRightCorner(collsionHitBox), GetTopLeftCorner(collsionHitBox), playervector);
        }
        public static bool IsAboveDB(Rectangle collsionHitBox, Vector2 playervector)
        {
            return IsOnUpperSideOfLine(GetTopRightCorner(collsionHitBox), GetBottomLeftCorner(collsionHitBox), playervector);
        }

        public static bool RectangleCollisionTop(Rectangle playerHitBox, Rectangle collsionHitBox, Vector2 velocity)
        {


            if (playerHitBox.Left < collsionHitBox.Right && playerHitBox.Right > collsionHitBox.Left &&
                playerHitBox.Bottom + velocity.Y > collsionHitBox.Top && playerHitBox.Top < collsionHitBox.Top &&
                        IsAboveAC(collsionHitBox, GetBottomRightCorner(playerHitBox)) &&
                        IsAboveDB(collsionHitBox, GetBottomLeftCorner(playerHitBox)))
            {
                return true;
            }
            return false;
        }

        public static bool RectangleCollisionBottom(Rectangle playerHitBox, Rectangle collsionHitBox, Vector2 velocity)
        {

            if (playerHitBox.Left < collsionHitBox.Right && playerHitBox.Right > collsionHitBox.Left &&
                playerHitBox.Top + velocity.Y < collsionHitBox.Bottom && playerHitBox.Bottom > collsionHitBox.Bottom &&
             !IsAboveAC(collsionHitBox, GetTopLeftCorner(playerHitBox)) &&
                        !IsAboveDB(collsionHitBox, GetTopRightCorner(playerHitBox)))
            {
                return true;
            }
            return false;
        }
        public static bool RectangleCollisionLeft(Rectangle playerHitBox, Rectangle collsionHitBox, Vector2 velocity)
        {


            if (playerHitBox.Right + velocity.X > collsionHitBox.Left && playerHitBox.Left + velocity.X < collsionHitBox.Left &&
                   IsAboveDB(collsionHitBox, GetTopRightCorner(playerHitBox)) &&
                        !IsAboveAC(collsionHitBox, GetBottomRightCorner(playerHitBox)))
            {
                return true;
            }
            return false;
        }
        public static bool RectangleCollisionRight(Rectangle playerHitBox, Rectangle collsionHitBox, Vector2 velocity)
        {
            if (playerHitBox.Left + velocity.X < collsionHitBox.Right && playerHitBox.Right + velocity.X > collsionHitBox.Right &&
                IsAboveAC(collsionHitBox, GetTopLeftCorner(playerHitBox)) &&
                        !IsAboveDB(collsionHitBox, GetBottomLeftCorner(playerHitBox)))
            {
                return true;
            }
            return false;
        }
        public static Vector2 GetCenter(Rectangle rect)
        {
            return new Vector2(rect.Center.X, rect.Center.Y);
        }


        public static Vector2 GetTopLeftCorner(Rectangle rect)
        {
            return new Vector2(rect.X, rect.Y);
        }
        public static Vector2 GetTopRightCorner(Rectangle rect)
        {
            return new Vector2(rect.X + rect.Width, rect.Y);
        }
        public static Vector2 GetBottomRightCorner(Rectangle rect)
        {
            return new Vector2(rect.X + rect.Width, rect.Y + rect.Height);
        }
        public static Vector2 GetBottomLeftCorner(Rectangle rect)
        {
            return new Vector2(rect.X, rect.Y + rect.Height);
        }
        public static bool IsOnUpperSideOfLine(Vector2 corner1, Vector2 oppositeCorner, Vector2 playerHitBoxCenter)
        {
            return ((oppositeCorner.X - corner1.X) * (playerHitBoxCenter.Y - corner1.Y) - (oppositeCorner.Y - corner1.Y) * (playerHitBoxCenter.X - corner1.X)) > 0;
        }
    }
}
