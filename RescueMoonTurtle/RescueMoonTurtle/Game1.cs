#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

#endregion

namespace RescueMoonTurtle
{

    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Moon moon;
        
        Texture2D projectileTexture;
        Texture2D turtleTexture;
        List<Turtle> turtles;
        int maxTurtles,numberOfPlayers;
         List<Player> players;
        public static List<Projectile> projectiles;
        public static int windowWidth, windowHeight;
        public Game1()
            : base()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferHeight = 1050;

            graphics.PreferredBackBufferWidth = 1680;
            windowWidth = graphics.PreferredBackBufferWidth;
            windowHeight = graphics.PreferredBackBufferHeight;
        }

        protected override void Initialize()
        {
            numberOfPlayers = 4;
            maxTurtles = 10; 
            players = new List<Player>();
            projectiles = new List<Projectile>();
            turtles = new List<Turtle>();
            base.Initialize();
        }
        public static Texture2D LoadTexture(ContentManager theContentManager, string textureName)
        {

            return theContentManager.Load<Texture2D>(textureName);

        }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            projectileTexture = LoadTexture(this.Content, "projectile");
            LoadPlayers();
            turtleTexture = LoadTexture(this.Content, "turtle");
            moon = new Moon(LoadTexture(this.Content, "moon"),
                new Vector2(windowWidth / 2, windowHeight / 2), 100);
           

        }


        protected override void UnloadContent()
        {

        }
        TimeSpan previousSpawnTime;
        TimeSpan spawnTime = TimeSpan.FromSeconds(1);
        public void LoadPlayers() 
        { 
            for(int i =0;i<numberOfPlayers;i++)
            {
               Player player = new Player(LoadTexture(this.Content, "player"), projectileTexture, TimeSpan.FromSeconds(0.1), new Vector2(100*i, 100),
                new Vector2(windowWidth / 2, windowHeight / 2),
                150,
                (PlayerIndex)i, 100);
               players.Add(player);
            }
        }
        public void AddTurtle(GameTime gameTime)
        {
            if (turtles.Count < maxTurtles)
            {
                if (gameTime.TotalGameTime - previousSpawnTime > spawnTime)
                {
                    Random random = new Random();
                    Vector2 turtleSpawnPos;
                    switch (random.Next(1,5))
                    {
                        case 1:
                            turtleSpawnPos = new Vector2(0 - turtleTexture.Width, random.Next(windowHeight));
                            break;
                        case 2:
                            turtleSpawnPos = new Vector2(windowWidth + turtleTexture.Width, random.Next(windowHeight));
                            break;
                        case 3:
                            turtleSpawnPos = new Vector2(random.Next(windowWidth), 0 - turtleTexture.Height);
                            break;
                        case 4:
                            turtleSpawnPos = new Vector2(random.Next(windowWidth), windowHeight + turtleTexture.Height);
                            break;
                        default:

                            turtleSpawnPos = new Vector2(0 - turtleTexture.Width, random.Next(windowHeight));
                            break;
                    }

                    Turtle turtle = new Turtle(turtleTexture, turtleSpawnPos, moon.position, 0.001f, 5, 5, 10);
                    turtles.Add(turtle);
                    previousSpawnTime = gameTime.TotalGameTime;
                }
            }
        }
  
        
        public void TurtleMoonCollision()
        {
           
                for (int i = 0; i < turtles.Count; i++)
                {
                    if (turtles[i].hitBox.Intersects(moon.hitBox))
                    {
                        if (Collision.CollidesWith(turtles[i], moon, turtles[i].turtleTransformation, moon.moonTransformation))
                        {
                            turtles[i].active = false;
                            turtles.RemoveAt(i);
                        }
                    }
                }
            
        }
        public void MoonProjectileCollision()
        {
            for (int i = 0; i < projectiles.Count; i++)
            {
              
                    if (moon.hitBox.Intersects(projectiles[i].hitBox))
                    {
                        if (Collision.CollidesWith(moon, projectiles[i], moon.moonTransformation, projectiles[i].projectileTransformation))
                        {
                            
                            projectiles[i].active = false;
                        }
                    }
                
            }
        }
        public void TurtleProjectileCollision()
        {
            for (int i = 0; i < projectiles.Count; i++)
            {
                for (int j = 0; j < turtles.Count; j++)
                {
                    if (turtles[j].hitBox.Intersects(projectiles[i].hitBox))
                    {
                    if (Collision.CollidesWith(turtles[j], projectiles[i], turtles[j].turtleTransformation, projectiles[i].projectileTransformation))
                    {
                        turtles[j].velocity += projectiles[i].velocity/5;
                        
                        projectiles[i].active = false;
                    }
                    }
                }
            }
        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            for (int i = 0; i < projectiles.Count; i++)
            {
                projectiles[i].Update(gameTime);
                if (projectiles[i].active == false)
                {
                    projectiles.RemoveAt(i);
                }
            }
            AddTurtle(gameTime);
            for (int i = 0; i < turtles.Count; i++)
            {
                turtles[i].Update(gameTime);
                if (turtles[i].active == false)
                {
                    turtles.RemoveAt(i);
                }
            }
            moon.Update(gameTime);
            foreach (Player player in players)
            {
                player.Update(gameTime);
            }
            TurtleProjectileCollision();
            TurtleMoonCollision();
            MoonProjectileCollision();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();
            foreach (Projectile projectile in projectiles)
            {
                projectile.Draw(spriteBatch);
            }
            foreach (Turtle turtle in turtles)
            {
                turtle.Draw(spriteBatch);
            }
            moon.Draw(spriteBatch);
            foreach (Player player in players)
            {
                player.Draw(spriteBatch);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
