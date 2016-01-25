#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using System.IO;
#endregion

namespace RescueMoonTurtle
{

    public class Game1 : Game
    {
        string timeString;
        string path = @"Score.txt";
        List<float> scores;
        SpriteFont timerFont;
        TimeSpan gameTimeScore;
        Texture2D whiteRectangle;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Moon moon;
        Texture2D explosionTexture;
        List<Animation> explosions;
        TimeSpan previousSpawnTime;
        TimeSpan spawnTime;
        Texture2D projectileTexture;
        Texture2D turtleTexture;
        Texture2D moonTexture;
        Texture2D healthTexture;
        Texture2D playButton, exitButton;
        Texture2D backgroundTexture;
        Texture2D titleTexture;
        List<Button> startMenuButtons;
        List<Turtle> turtles;
        int maxTurtles, numberOfPlayers;
        List<Player> players;
        public static List<Projectile> projectiles;
        public static int windowWidth, windowHeight;
        enum GameStates
        {
            StartMenu,
            GamePlaying
        }
        GameStates currentState = GameStates.StartMenu;
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
        public void InitializeGame()
        {
            explosions = new List<Animation>();

            numberOfPlayers = 0;
            for (int i = 0; i < 4; i++)
            {
                if (GamePad.GetState((PlayerIndex)i).IsConnected)
                {
                    numberOfPlayers++;
                }
            }
            spawnTime = TimeSpan.FromSeconds(1 / (numberOfPlayers));
            maxTurtles = 20;
            players = new List<Player>();
            projectiles = new List<Projectile>();
            turtles = new List<Turtle>();
            LoadPlayers();
            gameOver = false;
            moon = new Moon(moonTexture,
                new Vector2(windowWidth / 2, windowHeight / 2), 100);
            gameTimeScore = TimeSpan.FromSeconds(0);
            difficulityIncreaseTime = TimeSpan.FromSeconds(10);
            elapsedDifficulityIncreaseTime = TimeSpan.FromSeconds(0);
        }
        public void InitializeStartMenu()
        {
            startMenuButtons = new List<Button>();
            Button button = new Button(new Vector2(windowWidth / 2, windowHeight * 2 / 5), playButton, "play");
            startMenuButtons.Add(button);
            button = new Button(new Vector2(windowWidth / 2, (windowHeight * 3) / 5), exitButton, "exit");
            startMenuButtons.Add(button);
        }
        protected override void Initialize()
        {

            scores = new List<float>();
            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {

                }
            }

            using (StreamReader sr = File.OpenText(path))
            {

                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    try
                    {
                        scores.Add(float.Parse(line));
                    }
                    catch
                    {

                    }
                }

            }

            endGameDelay = TimeSpan.FromSeconds(1.5);
            base.Initialize();
            InitializeGame();
            InitializeStartMenu();
        }
        public List<float> SortScores(List<float> scores)
        {
            bool sorted = false;
            while (!sorted)
            {

                sorted = true;
                for (int i = 0; i < scores.Count - 1; i++)
                {

                    float holder = scores[i];
                    if (scores[i] < scores[i + 1])
                    {
                        scores[i] = scores[i + 1];
                        scores[i + 1] = holder;
                        sorted = false;
                    }
                }
            }
            return scores;
        }
        public static Texture2D LoadTexture(ContentManager theContentManager, string textureName)
        {

            return theContentManager.Load<Texture2D>(textureName);

        }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            timerFont = this.Content.Load<SpriteFont>("timerFont");
            playButton = LoadTexture(this.Content, "playButton");
            exitButton = LoadTexture(this.Content, "exitButton");
            projectileTexture = LoadTexture(this.Content, "projectile");
            explosionTexture = LoadTexture(this.Content, "explosion");
            turtleTexture = LoadTexture(this.Content, "turtle");
            moonTexture = LoadTexture(this.Content, "moon");
            healthTexture = LoadTexture(this.Content, "healthbar");
            backgroundTexture = LoadTexture(this.Content, "background");
            titleTexture = LoadTexture(this.Content, "logo");
            whiteRectangle = new Texture2D(GraphicsDevice, 1, 1);
            whiteRectangle.SetData(new[] { Color.White });
        }


        protected override void UnloadContent()
        {

        }

        public void LoadPlayers()
        {
            for (int i = 0; i < numberOfPlayers; i++)
            {
                Player player = new Player(LoadTexture(this.Content, "player"), projectileTexture, TimeSpan.FromSeconds(0.1), new Vector2(0, 0),
                 new Vector2(windowWidth / 2, windowHeight / 2),
                 175, (PlayerIndex)i, 100);
                switch (i)
                {
                    case 0:
                        player.color = Color.Green;
                        player.rotation = 0;

                        player.X = player.gravityCenter.X + (float)Math.Sin(player.rotation) * (float)player.distanceToCenter;
                        player.Y = player.gravityCenter.Y - (float)Math.Cos(player.rotation) * (float)player.distanceToCenter;
                        break;
                    case 1:
                        player.color = Color.Yellow;
                        player.rotation = (float)Math.PI / 2;

                        player.X = player.gravityCenter.X + (float)Math.Sin(player.rotation) * (float)player.distanceToCenter;
                        player.Y = player.gravityCenter.Y - (float)Math.Cos(player.rotation) * (float)player.distanceToCenter;
                        break;
                    case 2:
                        player.color = Color.Red;
                        player.rotation = (float)Math.PI;

                        player.X = player.gravityCenter.X + (float)Math.Sin(player.rotation) * (float)player.distanceToCenter;
                        player.Y = player.gravityCenter.Y - (float)Math.Cos(player.rotation) * (float)player.distanceToCenter;
                        break;
                    case 3:
                        player.color = Color.Blue;
                        player.rotation = -(float)Math.PI / 2;

                        player.X = player.gravityCenter.X + (float)Math.Sin(player.rotation) * (float)player.distanceToCenter;
                        player.Y = player.gravityCenter.Y - (float)Math.Cos(player.rotation) * (float)player.distanceToCenter;
                        break;
                }
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
                    switch (random.Next(1, 5))
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

                    Turtle turtle = new Turtle(turtleTexture, turtleSpawnPos, moon.Position, 0.001f, 5, 5, 10);

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
                    if (Collision.CollidesWith(turtles[i], moon))
                    {
                        AddExplosion(turtles[i].Position, 1f);
                        turtles[i].active = false;
                        turtles.RemoveAt(i);
                        moon.hp -= 5;

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
                    if (Collision.CollidesWith(moon, projectiles[i]))
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
                        if (Collision.CollidesWith(turtles[j], projectiles[i]))
                        {
                            turtles[j].velocity += projectiles[i].velocity / 5;
                            AddExplosion(projectiles[i].Position, 0.3f);
                            projectiles[i].active = false;
                        }
                    }
                }
            }
        }

        public void UpdateStartMenu(GameTime gameTime)
        {
            int currentSelected = MenuSelect(gameTime, PlayerIndex.One, startMenuButtons);
            startMenuButtons[currentSelected].selected = true;
            foreach (Button button in startMenuButtons)
            {
                button.Update(gameTime);
                if (button.CheckForClick())
                {

                    if (button.buttonName == "play")
                    {
                        InitializeGame();
                        currentState = GameStates.GamePlaying;

                    }
                    if (button.buttonName == "exit")
                    {
                        Exit();
                    }
                }
            }
            startMenuButtons[currentSelected].selected = false;

        }
        int currentMenuItem = 0;
        int elapsedTime = 0;
        int menuTime = 100;
        public int MenuSelect(GameTime gameTime, PlayerIndex num, List<Button> menuButtons)
        {
            if (0 <= currentMenuItem && currentMenuItem < menuButtons.Count)
            {
                elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
                if (elapsedTime > menuTime)
                {
                    if ((GamePad.GetState(num).ThumbSticks.Left.Y) >= 0.25)
                    {
                        currentMenuItem -= 1;

                    }
                    if ((GamePad.GetState(num).ThumbSticks.Left.Y) <= -0.25)
                    {
                        currentMenuItem += 1;

                    }
                    elapsedTime = 0;
                }

            }
            if (0 > currentMenuItem)
            {
                currentMenuItem = 0;
            }
            if (currentMenuItem > menuButtons.Count - 1)
            {
                currentMenuItem = menuButtons.Count - 1;
            }
            return currentMenuItem;
        }

        public void AddExplosion(Vector2 position, float scale)
        {
            Animation explosion = new Animation(explosionTexture, 10, 0.25f, position, 0f, Color.White);

            explosion.scale = scale;
            explosion.runOnce = true;

            explosions.Add(explosion);
        }
        public void UpdateExplosions(GameTime gameTime)
        {
            for (int i = 0; i < explosions.Count; i++)
            {
                explosions[i].Update(gameTime);
                if (!explosions[i].active)
                {
                    explosions.RemoveAt(i);
                }
            }
        }
        bool gameOver = false;
        public void UpdateGame(GameTime gameTime)
        {
            if (!gameOver)
            {

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

                if (moon.hp <= 0)
                {

                    projectiles = new List<Projectile>();
                    elapsedEndGameTime = TimeSpan.FromSeconds(0);
                    AddExplosion(moon.Position, 2);
                    gameOver = true;
                    moon.active = false;
                    foreach (Turtle turtle in turtles)
                    {
                        AddExplosion(turtle.Position, 1);

                        turtle.active = false;
                    }
                    foreach (Player player in players)
                    {
                        AddExplosion(player.Position, 1);

                        player.active = false;
                    }
                }

                gameTimeScore += TimeSpan.FromSeconds(gameTime.ElapsedGameTime.TotalSeconds);
                elapsedDifficulityIncreaseTime += TimeSpan.FromSeconds(gameTime.ElapsedGameTime.TotalSeconds);
                if (elapsedDifficulityIncreaseTime > difficulityIncreaseTime)
                {
                    if (spawnTime > TimeSpan.FromSeconds(0.05) - TimeSpan.FromSeconds(0.1))
                    {
                        spawnTime -= TimeSpan.FromSeconds(0.1);
                        elapsedDifficulityIncreaseTime = TimeSpan.FromSeconds(0);
                    }
                    else
                    {
                        spawnTime = TimeSpan.FromSeconds(0.05);
                    }
                }

            }
            UpdateExplosions(gameTime);
            if (gameOver)
            {
                elapsedEndGameTime += TimeSpan.FromSeconds(gameTime.ElapsedGameTime.TotalSeconds);
                if (elapsedEndGameTime > endGameDelay)
                {
                    if (GamePad.GetState(PlayerIndex.One).Buttons.Start == ButtonState.Pressed)
                    {
                        try
                        {
                            scores.Add(float.Parse(timeString));
                        }
                        catch
                        {

                        }
                        scores = SortScores(scores);
                        string text = "";
                        foreach (float score in scores)
                        {
                            text += score.ToString() + "\n";
                        }
                        System.IO.File.WriteAllText(path, text);
                        currentState = GameStates.StartMenu;
                    }
                }
            }
        }
        TimeSpan endGameDelay;
        TimeSpan elapsedEndGameTime;
        TimeSpan difficulityIncreaseTime;
        TimeSpan elapsedDifficulityIncreaseTime;
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (currentState == GameStates.StartMenu)
            {
                UpdateStartMenu(gameTime);
            }
            if (currentState == GameStates.GamePlaying)
            {
                UpdateGame(gameTime);
            }
            base.Update(gameTime);
        }
        public void DrawStartMenu(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(titleTexture, new Vector2((windowWidth - titleTexture.Width) / 2, 100), Color.White);
            foreach (Button button in startMenuButtons)
            {
                button.Draw(spriteBatch);
            }
            string highScoreTitle = "HighScores:";

            Vector2 FontOrigin = timerFont.MeasureString(highScoreTitle);
            spriteBatch.DrawString(timerFont, highScoreTitle, new Vector2(timerFont.MeasureString(highScoreTitle).X + 10, timerFont.MeasureString(highScoreTitle).Y + 10), Color.White,
                          0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);

            for (int i = 0; i < scores.Count; i++)
            {
                if (i < 10)
                {
                    string scoreText = (i + 1) + ". " + scores[i].ToString("00.00");

                    FontOrigin = timerFont.MeasureString(scoreText);
                    spriteBatch.DrawString(timerFont, scoreText,
                        new Vector2(timerFont.MeasureString(scoreText).X + 10,
                        timerFont.MeasureString(highScoreTitle).Y + 60 + i * ((float)timerFont.MeasureString(scoreText).Y * 1.5f)), Color.White,
                                  0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
                }
            }

        }

        public void DrawGame(SpriteBatch spriteBatch)
        {
            foreach (Projectile projectile in projectiles)
            {
                projectile.Draw(spriteBatch);
            }
            foreach (Turtle turtle in turtles)
            {
                turtle.Draw(spriteBatch);
            }
            moon.Draw(spriteBatch);
            DrawExplosions(spriteBatch);
            foreach (Player player in players)
            {
                player.Draw(spriteBatch);
            }
            if (elapsedEndGameTime < endGameDelay)
            {


                DrawHealth(spriteBatch);

                timeString = gameTimeScore.ToString(@"mm\.ss");
                Vector2 FontOrigin = timerFont.MeasureString(timeString) / 2;
                spriteBatch.DrawString(timerFont, timeString, new Vector2(1500, 30), Color.White,
                              0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
            }
            else
            {
                string endGameString = "Game Over!\nYour Time Was:\n" +
                    gameTimeScore.ToString(@"mm\.ss") +
                    "\nPress Start To Return To The Main Menu";
                Vector2 FontOrigin = timerFont.MeasureString(endGameString) / 2;
                spriteBatch.DrawString(timerFont, endGameString, new Vector2(windowWidth / 2, windowHeight / 2), Color.White,
                              0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
            }

        }
        public void DrawExplosions(SpriteBatch spriteBatch)
        {
            foreach (Animation explosion in explosions)
            {
                explosion.Draw(spriteBatch);
            }
        }
        public void DrawHealth(SpriteBatch spriteBatch)
        {
            float healthPercentage = (float)moon.hp / 100; ;
            float visibleWidth = (float)healthTexture.Width * healthPercentage;

            Rectangle healthRectangle = new Rectangle(((int)windowWidth - healthTexture.Width) / 2 + 10,
                                           (int)windowHeight - 90,
                                           (int)(visibleWidth) - 20,
                                           healthTexture.Height - 20);

            spriteBatch.Draw(healthTexture, new Vector2((windowWidth - healthTexture.Width) / 2, windowHeight - 100), Color.White);
            spriteBatch.Draw(whiteRectangle, healthRectangle, Color.Green);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            spriteBatch.Draw(backgroundTexture, new Vector2(0, 0), Color.White);
            if (currentState == GameStates.StartMenu)
            {
                DrawStartMenu(spriteBatch);
            }
            if (currentState == GameStates.GamePlaying)
            {
                DrawGame(spriteBatch);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
