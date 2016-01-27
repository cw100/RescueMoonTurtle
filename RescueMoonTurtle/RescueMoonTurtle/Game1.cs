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

        bool gameOver = false;
        bool noControllers;
        MouseState mouseState;
        string timeString;
        string path;
        List<float> scores;
        SpriteFont timerFont;
        TimeSpan gameTimeScore;
        TimeSpan endGameDelay;
        TimeSpan elapsedEndGameTime;
        TimeSpan difficulityIncreaseTime;
        TimeSpan elapsedDifficulityIncreaseTime;
        TimeSpan bigTurtleSpawnTime;
        TimeSpan elapsedBigTurtleSpawnTime;
        int currentMenuItem = 0;
        TimeSpan elapsedMenuTime;
        TimeSpan menuTime;
        Texture2D cursor;
        Texture2D whiteRectangle;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Moon moon;
        Texture2D explosionTexture;
        List<Animation> explosions;
        TimeSpan elapsedSpawnTime;
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
            //Sets game window dimensions
            graphics.PreferredBackBufferHeight = 900;
            graphics.PreferredBackBufferWidth = 1500;

            windowWidth = graphics.PreferredBackBufferWidth;
            windowHeight = graphics.PreferredBackBufferHeight;
        }
        //Resets values needed for gameplay
        public void InitializeGame()
        {
            //Creates new list for explosions
            explosions = new List<Animation>();
            numberOfPlayers = 0;
            //Checks number of controllers connected
            for (int i = 0; i < 4; i++)
            {
                if (GamePad.GetState((PlayerIndex)i, GamePadDeadZone.None).IsConnected)
                {
                    numberOfPlayers++;
                }
            }
            //If no controllers connected, add one player for keyboard control
            if (numberOfPlayers == 0)
            {
                numberOfPlayers = 1;
            }
            //Changes spawn time of turtles based on number of players
            spawnTime = TimeSpan.FromSeconds(1.5 / (double)numberOfPlayers);
            //Set max amount of turtles 
            maxTurtles = 20;
            //Creates new list for Players, Projectiles, and turtles
            players = new List<Player>();
            projectiles = new List<Projectile>();
            turtles = new List<Turtle>();
            //Load players based on number of players and control method
            LoadPlayers();
            //Reset game
            gameOver = false;
            //Spawn moon in middle of screen, with 100 health
            moon = new Moon(moonTexture,
                new Vector2(windowWidth / 2, windowHeight / 2), 100);
            //Reset all game timers
            gameTimeScore = TimeSpan.FromSeconds(0);
            elapsedBigTurtleSpawnTime = TimeSpan.FromSeconds(0);
            elapsedEndGameTime = TimeSpan.FromSeconds(0);
            elapsedDifficulityIncreaseTime = TimeSpan.FromSeconds(0);
        }
        //Create start menu
        public void InitializeStartMenu()
        {
            //Create new button list for start menu, and add buttons
            startMenuButtons = new List<Button>();
            Button button = new Button(new Vector2(windowWidth / 2, windowHeight * 2 / 5), playButton, "play");
            startMenuButtons.Add(button);
            button = new Button(new Vector2(windowWidth / 2, (windowHeight * 3) / 5), exitButton, "exit");
            startMenuButtons.Add(button);
        }

        protected override void Initialize()
        {
            //Creates list for scores   
            scores = new List<float>();
            //Create Appdata path
            path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)+"\\MoonTurtleRescue";
            //Creates new blank text file for scores if one doesn't exist;
            bool exists = System.IO.Directory.Exists(path);

            if (!exists)
                System.IO.Directory.CreateDirectory(path);
            path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\MoonTurtleRescue\\Scores.txt";
            if (!File.Exists(path))
            {
                using (StreamWriter sw = File.CreateText(path))
                {

                }
            }
            //Open scores text file
            using (StreamReader sr = File.OpenText(path))
            {
                String line;
                //Load each scores line into float array
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
            //Set game timers
            bigTurtleSpawnTime = TimeSpan.FromSeconds(10);
            difficulityIncreaseTime = TimeSpan.FromSeconds(10);
            endGameDelay = TimeSpan.FromSeconds(1);
            menuTime = TimeSpan.FromSeconds(0.1);
            elapsedMenuTime = TimeSpan.FromSeconds(0);
            base.Initialize();
            InitializeGame();
            InitializeStartMenu();
        }
        //Bubble sort for scores
        public List<float> SortScores(List<float> scores)
        {
            bool sorted = false;
            while (!sorted)
            {
                sorted = true;
                //Check each score in score array until sorted in descending order
                for (int i = 0; i < scores.Count - 1; i++)
                {
                    float holder = scores[i];
                    //If next score is higher than current score, flip indexs
                    if (scores[i] < scores[i + 1])
                    {
                        scores[i] = scores[i + 1];
                        scores[i + 1] = holder;
                        sorted = false;
                    }
                }
            }
            //Returns sorted array
            return scores;
        }
        //Texture loader
        public static Texture2D LoadTexture(ContentManager theContentManager, string textureName)
        {
            return theContentManager.Load<Texture2D>(textureName);
        }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            //Loads all textures and font
            timerFont = this.Content.Load<SpriteFont>("timerFont");
            cursor = LoadTexture(this.Content, "particle");
            playButton = LoadTexture(this.Content, "playButton");
            exitButton = LoadTexture(this.Content, "exitButton");
            projectileTexture = LoadTexture(this.Content, "projectile");
            explosionTexture = LoadTexture(this.Content, "explosion");
            turtleTexture = LoadTexture(this.Content, "turtle");
            moonTexture = LoadTexture(this.Content, "moon");
            healthTexture = LoadTexture(this.Content, "healthbar");
            backgroundTexture = LoadTexture(this.Content, "background");
            titleTexture = LoadTexture(this.Content, "logo");
            //Loads color texture for hp
            whiteRectangle = new Texture2D(GraphicsDevice, 1, 1);
            whiteRectangle.SetData(new[] { Color.White });
        }


        protected override void UnloadContent()
        {

        }
        //Loads players based on number of players
        public void LoadPlayers()
        {
            //Keyboard control player
            if (noControllers)
            {
                //Creates keyboard control player
                Player player = new Player(LoadTexture(this.Content, "player"), projectileTexture, TimeSpan.FromSeconds(0.2), new Vector2(0, 0),
                new Vector2(windowWidth / 2, windowHeight / 2), 175);
                //Sets player color and spawn location
                player.color = Color.Green;
                player.rotation = 0;
                player.X = player.gravityCenter.X + (float)Math.Sin(player.rotation) * (float)player.distanceToCenter;
                player.Y = player.gravityCenter.Y - (float)Math.Cos(player.rotation) * (float)player.distanceToCenter;
                players.Add(player);
            }
            else
            {
                //Creates Xbox control player until the correct number of players are created
                for (int i = 0; i < numberOfPlayers; i++)
                {
                    Player player = new Player(LoadTexture(this.Content, "player"), projectileTexture, TimeSpan.FromSeconds(0.2), new Vector2(0, 0),
                     new Vector2(windowWidth / 2, windowHeight / 2),
                     175, (PlayerIndex)i);
                    //Choses color and spawn location based on player number
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
        }
        //Create new turtle if able
        public void AddTurtle(GameTime gameTime)
        {
            //Checks if too many turtles are spawned
            if (turtles.Count < maxTurtles)
            {
                //Check last turtle spawn time
                if (elapsedSpawnTime > spawnTime)
                {
                    //Randomisation for spawn location
                    Random random = new Random();
                    Vector2 turtleSpawnPos;

                    int turtleScale = 1;
                    //Creates larger turtle based on big turtle spawn time
                    if (elapsedBigTurtleSpawnTime > bigTurtleSpawnTime)
                    {
                        turtleScale = 2;
                        elapsedBigTurtleSpawnTime = TimeSpan.FromSeconds(0);
                    }
                    //Chose turtle spawn side, and location
                    switch (random.Next(1, 5))
                    {
                        case 1:
                            turtleSpawnPos = new Vector2(0 - turtleTexture.Width * turtleScale, random.Next(windowHeight));
                            break;
                        case 2:
                            turtleSpawnPos = new Vector2(windowWidth + turtleTexture.Width * turtleScale, random.Next(windowHeight));
                            break;
                        case 3:
                            turtleSpawnPos = new Vector2(random.Next(windowWidth), 0 - turtleTexture.Height * turtleScale);
                            break;
                        case 4:
                            turtleSpawnPos = new Vector2(random.Next(windowWidth), windowHeight + turtleTexture.Height * turtleScale);
                            break;
                        default:

                            turtleSpawnPos = new Vector2(0 - turtleTexture.Width * turtleScale, random.Next(windowHeight));
                            break;
                    }
                    //Creates new turtle with correct spawn location and size
                    Turtle turtle = new Turtle(turtleTexture, turtleSpawnPos, moon.Position, turtleScale, 0.001f);
                    turtles.Add(turtle);
                    //Resets timer
                    elapsedSpawnTime = TimeSpan.FromSeconds(0);
                }
            }
        }
        //Collision between turtles and moon
        public void TurtleMoonCollision()
        {
            //Checks each turtle
            for (int i = 0; i < turtles.Count; i++)
            {
                //Checks pixel perfect collision with turtle and moon
                if (Collision.CollidesWith(turtles[i], moon))
                {
                    //Reduces moon hp by 5 times the size of the turtle
                    moon.hp -= (int)(5 * turtles[i].scale);
                    //Creates a explosion at turtle location
                    AddExplosion(turtles[i].Position, 1f);
                    //Disables turtle
                    turtles[i].active = false;
                   
                }
            }

        }
        //Collision between projectiles and moon
        public void MoonProjectileCollision()
        {
            //Checks each projectile
            for (int i = 0; i < projectiles.Count; i++)
            {
                //Checks pixel perfect collision with projectiles and moon
                if (Collision.CollidesWith(moon, projectiles[i]))
                {
                    //Disables projectile
                    projectiles[i].active = false;
                }
            }
        }
        //Collision between projectiles and turtles
        public void TurtleProjectileCollision()
        {
            //Checks each projectile
            for (int i = 0; i < projectiles.Count; i++)
            {
                //Checks each turtle
                for (int j = 0; j < turtles.Count; j++)
                {
                    //Checks pixel perfect collision with projectiles and turtles
                    if (Collision.CollidesWith(turtles[j], projectiles[i]))
                    {
                        //Knocks back turtle by projectile velocity, with reduced effect for larger turtles
                        turtles[j].velocity += projectiles[i].velocity / (5 * turtles[j].scale * turtles[j].scale);
                        //Creates a explosion at projectile location
                        AddExplosion(projectiles[i].Position, 0.3f);
                        //Disables projectile
                        projectiles[i].active = false;
                    }

                }
            }
        }
        //Updates start menu buttons
        public void UpdateStartMenu(GameTime gameTime)
        {
            //Checks for connected Xbox controllers
            noControllers = true;
            for (int i = 0; i < 4; i++)
            {
                if (GamePad.GetState((PlayerIndex)i, GamePadDeadZone.None).IsConnected)
                {
                    noControllers = false;
                }
            }
            int currentSelected = 1;
            if (!noControllers)
            {
                //Selects button based on controller input
                currentSelected = MenuSelect(gameTime, PlayerIndex.One, startMenuButtons);
                startMenuButtons[currentSelected].selected = true;
            }
            //Updates each button
            foreach (Button button in startMenuButtons)
            {
                //Enables mouse control if no controllers connected
                if (noControllers)
                {
                    //Checks if mouse is inside button and selects button if so
                    if (button.hitBox.Contains(mouseState.Position))
                    {
                        button.selected = true;
                    }
                    else
                    {
                        button.selected = false;
                    }
                }
                //updates button
                button.Update(gameTime);
                //Checks for player input
                if (button.CheckForClick())
                {
                    //If play button clicked, start game
                    if (button.buttonName == "play")
                    {
                        //Resets game values
                        InitializeGame();
                        //Sets current state to game start 
                        currentState = GameStates.GamePlaying;

                    }
                    //If play button clicked, exit game
                    if (button.buttonName == "exit")
                    {
                        Exit();
                    }
                }
            }
            //Refreshes selected button state
            if (!noControllers)
            {
                startMenuButtons[currentSelected].selected = false;
            }

        }
        //Selects button based on controller input
        public int MenuSelect(GameTime gameTime, PlayerIndex num, List<Button> menuButtons)
        {

            //Menu delay
            elapsedMenuTime += TimeSpan.FromSeconds(gameTime.ElapsedGameTime.TotalSeconds);
            if (elapsedMenuTime > menuTime)
            {
                //Moves selected button up if stick is up
                if ((GamePad.GetState(num, GamePadDeadZone.None).ThumbSticks.Left.Y) >= 0.25)
                {
                    currentMenuItem -= 1;

                }
                //Moves selected button down if stick is down
                if ((GamePad.GetState(num, GamePadDeadZone.None).ThumbSticks.Left.Y) <= -0.25)
                {
                    currentMenuItem += 1;

                }
                //Resets timer
                elapsedMenuTime = TimeSpan.FromSeconds(0);
            }
            //Keeps value in button list
            if (0 > currentMenuItem)
            {
                currentMenuItem = 0;
            }
            if (currentMenuItem > menuButtons.Count - 1)
            {
                currentMenuItem = menuButtons.Count - 1;
            }
            //Return button index
            return currentMenuItem;
        }
        //Creates explosion at correct position and size
        public void AddExplosion(Vector2 position, float scale)
        {
            //Creates explosion animation
            Animation explosion = new Animation(explosionTexture, 10, 0.25f, position, 0f, Color.White);
            explosion.scale = scale;
            //Disables explosion after one play
            explosion.runOnce = true;

            explosions.Add(explosion);
        }
        //Updates all explosions
        public void UpdateExplosions(GameTime gameTime)
        {
            for (int i = 0; i < explosions.Count; i++)
            {
                explosions[i].Update(gameTime);
                if (explosions[i].isPaused)
                {
                    //Removes explosions that are finished
                    explosions.RemoveAt(i);
                }
            }
        }
        //Updates game logic
        public void UpdateGame(GameTime gameTime)
        {
            //Checks if game is over
            if (!gameOver)
            {
                //Updates all projectiles
                for (int i = 0; i < projectiles.Count; i++)
                {
                    projectiles[i].Update(gameTime);
                    if (projectiles[i].active == false)
                    {
                        //Removes diabled projectiles
                        projectiles.RemoveAt(i);
                    }
                }
                AddTurtle(gameTime);
                //Updates all turtles
                for (int i = 0; i < turtles.Count; i++)
                {
                    turtles[i].Update(gameTime);
                    if (turtles[i].active == false)
                    {
                        //Removes diabled turtles
                        turtles.RemoveAt(i);
                    }
                }
                //Update moon
                moon.Update(gameTime);
                //Update all players
                foreach (Player player in players)
                {
                    player.Update(gameTime);
                }
                //Check collisions
                TurtleProjectileCollision();
                TurtleMoonCollision();
                MoonProjectileCollision();
                //Check for game over
                if (moon.hp <= 0)
                {
                    //Removes projectiles
                    projectiles = new List<Projectile>();
                    //Restarts end game pause timer
                    elapsedEndGameTime = TimeSpan.FromSeconds(0);
                    //Explodes moon
                    AddExplosion(moon.Position, 2);
                    gameOver = true;
                    //Removes moon
                    moon.active = false;
                    //Explodes turtles and players
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
                //Increase timers
                gameTimeScore += TimeSpan.FromSeconds(gameTime.ElapsedGameTime.TotalSeconds);
                elapsedDifficulityIncreaseTime += TimeSpan.FromSeconds(gameTime.ElapsedGameTime.TotalSeconds);
                elapsedSpawnTime += TimeSpan.FromSeconds(gameTime.ElapsedGameTime.TotalSeconds);
                elapsedBigTurtleSpawnTime += TimeSpan.FromSeconds(gameTime.ElapsedGameTime.TotalSeconds);
                //Increase difficulity based on timer
                if (elapsedDifficulityIncreaseTime > difficulityIncreaseTime)
                {
                    //Reduce spawn time by 0.1 seconds, to minimum of 0.005 seconds
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
            //Update explosions
            UpdateExplosions(gameTime);
            //Display score and end game
            if (gameOver)
            {
                //Check pause timer
                elapsedEndGameTime += TimeSpan.FromSeconds(gameTime.ElapsedGameTime.TotalSeconds);
                //Pause timer allows for explosions to finish before showing score
                if (elapsedEndGameTime > endGameDelay)
                {
                    if (GamePad.GetState(PlayerIndex.One, GamePadDeadZone.None).Buttons.Start == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Space))
                    {
                        //Adds time to score list
                        try
                        {
                            scores.Add(float.Parse(timeString));
                        }
                        catch
                        {

                        }
                        //Sorts score
                        scores = SortScores(scores);
                        string text = "";
                        //Format scores into string
                        foreach (float score in scores)
                        {
                            text += score.ToString() + "\n";
                        }
                        //Save score
                        System.IO.File.WriteAllText(path, text);
                        //Restart to start menu
                        currentState = GameStates.StartMenu;
                    }
                }
            }
        }
        //Main update method
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One, GamePadDeadZone.None).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            //Check mouse values
            mouseState = Mouse.GetState();
            //Update based on game state
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
        //Draw main menu sprites
        public void DrawStartMenu(SpriteBatch spriteBatch)
        {
            //Draw title
            spriteBatch.Draw(titleTexture, new Vector2((windowWidth - titleTexture.Width) / 2, 100), Color.White);
            //Draw all buttons
            foreach (Button button in startMenuButtons)
            {
                button.Draw(spriteBatch);
            }
            //Draw highscore title
            string highScoreTitle = "HighScores:";
            Vector2 FontOrigin = timerFont.MeasureString(highScoreTitle);
            spriteBatch.DrawString(timerFont, highScoreTitle, 
                new Vector2(timerFont.MeasureString(highScoreTitle).X + 10,
                timerFont.MeasureString(highScoreTitle).Y + 10), Color.White,
                          0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
            //Draw each score, to max of 10
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
        //Draw game sprites
        public void DrawGame(SpriteBatch spriteBatch)
        {
            //Draw all projectiles
            foreach (Projectile projectile in projectiles)
            {
                projectile.Draw(spriteBatch);
            }
            //Draw all turtles
            foreach (Turtle turtle in turtles)
            {
                turtle.Draw(spriteBatch);
            }
            //Draw moon
            moon.Draw(spriteBatch);
            //Draw explosions
            DrawExplosions(spriteBatch);
            //Draw all players
            foreach (Player player in players)
            {
                player.Draw(spriteBatch);
            }
           //Draw current time
            if (elapsedEndGameTime < endGameDelay)
            {
                DrawHealth(spriteBatch);
                timeString = gameTimeScore.ToString(@"mm\.ss");
                Vector2 FontOrigin = timerFont.MeasureString(timeString) / 2;
                spriteBatch.DrawString(timerFont, timeString, new Vector2(50, 30), Color.White,
                              0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
            }
            //Wait for game to end, then draw last score
            else
            {
                string endGameString = "Game Over!\nYour Time Was:\n" +
                    gameTimeScore.ToString(@"mm\.ss") +
                    "\nPress Start Or Space To Return To The Main Menu";
                Vector2 FontOrigin = timerFont.MeasureString(endGameString) / 2;
                spriteBatch.DrawString(timerFont, endGameString, new Vector2(windowWidth / 2, windowHeight / 2), Color.White,
                              0, FontOrigin, 1.0f, SpriteEffects.None, 0.5f);
            }

        }
        //Draw explosions
        public void DrawExplosions(SpriteBatch spriteBatch)
        {
            foreach (Animation explosion in explosions)
            {
                explosion.Draw(spriteBatch);
            }
        }
        //Draw current moon health
        public void DrawHealth(SpriteBatch spriteBatch)
        {
            //Current health percentage 
            float healthPercentage = (float)moon.hp / 100; ;
            //Width in pixels of health bar
            float visibleWidth = (float)healthTexture.Width * healthPercentage;
            //Create rectangle for current health, with margin to fit in empty bar
            Rectangle healthRectangle = new Rectangle(((int)windowWidth - healthTexture.Width) / 2 + 10,
                                           (int)windowHeight - 90,
                                           (int)(visibleWidth) - 20,
                                           healthTexture.Height - 20);
            //Draw empty health bar
            spriteBatch.Draw(healthTexture, new Vector2((windowWidth - healthTexture.Width) / 2, windowHeight - 100), Color.White);
            //Draw current health
            spriteBatch.Draw(whiteRectangle, healthRectangle, Color.Green);
        }
        //Main draw method
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            //Draw background
            spriteBatch.Draw(backgroundTexture, new Vector2(0, 0), Color.White);
            //Draw sprites based on current state
            if (currentState == GameStates.StartMenu)
            {
                DrawStartMenu(spriteBatch);
            }
            if (currentState == GameStates.GamePlaying)
            {
                DrawGame(spriteBatch);
            }
            //Draw mouse icon if no controllers
            if (noControllers)
            {
                spriteBatch.Draw(cursor, mouseState.Position.ToVector2(), null, Color.White, 0f,
                  new Vector2(cursor.Width / 2, cursor.Height / 2), 2, SpriteEffects.None, 0f);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
