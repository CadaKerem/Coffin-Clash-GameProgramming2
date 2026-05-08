using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace CoffinGame
{

    public class Bullet {
        public Vector2 Position;
        public Vector2 Velocity;
        public bool FromPlayer1; 
        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, 20, 5);
        public void Update(float dt) { Position += Velocity * dt; }
        public void Draw(SpriteBatch sb, Texture2D pixel) { sb.Draw(pixel, Bounds, Color.Yellow); }
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _pixel;
        private SpriteFont _font; 

        enum GameState { Menu, Combat, Shop, Invasion, GameOver } //states
        GameState _currentState = GameState.Menu;
        string winnerText = "";

        Tower player1, player2;
        Coffin mainCoffin;
        float roundTimer = 20f;
        int p1Gold = 0, p2Gold = 0;
        float p1PushPower = 1f, p2PushPower = 1f;
        float coffinVelocity = 0f;

        List<Monster> monsters = new List<Monster>();
        List<Bullet> bullets = new List<Bullet>();
        int waveCount = 0;
        KeyboardState previousState;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
        }

        protected override void Initialize() { ResetGame(); base.Initialize(); }

        private void ResetGame()
        {
            player1 = new Tower(50, 400); 
            player2 = new Tower(1150, 400); 
            mainCoffin = new Coffin(new Vector2(610, 450));
            mainCoffin.Health = 150f; // coffin hp
            p1Gold = 0; p2Gold = 0; p1PushPower = 1f; p2PushPower = 1f;
            roundTimer = 20f; coffinVelocity = 0;
            monsters.Clear(); bullets.Clear(); waveCount = 0;
            winnerText = "";
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
            _font = Content.Load<SpriteFont>("Font"); 
        }

        protected override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState ks = Keyboard.GetState();

            // menu keys
            if (_currentState == GameState.Menu)
            {
                if (ks.IsKeyDown(Keys.Escape)) Exit();

                if (ks.IsKeyDown(Keys.Space) && previousState.IsKeyUp(Keys.Space)) { ResetGame(); _currentState = GameState.Combat; }
            }
            
            // combat keys
            else if (_currentState == GameState.Combat)
            {
                roundTimer -= dt;
                if (ks.IsKeyDown(Keys.D) && previousState.IsKeyUp(Keys.D)) FireBullet(true);
                if (ks.IsKeyDown(Keys.Left) && previousState.IsKeyUp(Keys.Left)) FireBullet(false);

                coffinVelocity *= 0.94f; // velocity = sürtünme
                mainCoffin.Update(coffinVelocity, dt);
                UpdateBullets(dt);

                if (mainCoffin.Health <= 0 || roundTimer <= 0) 
                { 
                    bullets.Clear(); // bullet fix
                    _currentState = GameState.Shop; 
                }
            }

            // shop keys
            else if (_currentState == GameState.Shop)
            {
                if (ks.IsKeyDown(Keys.W) && previousState.IsKeyUp(Keys.W) && p1Gold >= 500) { p1Gold -= 500; p1PushPower += 0.2f; }
                if (ks.IsKeyDown(Keys.Up) && previousState.IsKeyUp(Keys.Up) && p2Gold >= 500) { p2Gold -= 500; p2PushPower += 0.2f; }
                
                if (ks.IsKeyDown(Keys.Space) && previousState.IsKeyUp(Keys.Space)) 
                { 
                    bullets.Clear(); // bullet fix
                    waveCount++; 
                    SpawnMonsters(); 
                    _currentState = GameState.Invasion; 
                }
            }

            // invasion keys
            else if (_currentState == GameState.Invasion)
            {
                foreach (var m in monsters)
                {
                    Tower target = m.TargetSide == 1 ? player1 : player2;
                    m.Update(new Vector2(target.Bounds.X, target.Bounds.Y), dt, target.Bounds);

                    if (m.Bounds.Intersects(target.Bounds) && m.CanAttack()) target.Health -= m.Damage;

                    if (target == player1 && ks.IsKeyDown(Keys.D) && previousState.IsKeyUp(Keys.D)) FireBullet(true);
                    if (target == player2 && ks.IsKeyDown(Keys.Left) && previousState.IsKeyUp(Keys.Left)) FireBullet(false);
                }

                UpdateBullets(dt);

                // check win state
                if (player1.Health <= 0) { winnerText = "PLAYER 2 WINS!"; _currentState = GameState.GameOver; }
                else if (player2.Health <= 0) { winnerText = "PLAYER 1 WINS!"; _currentState = GameState.GameOver; }

                // round reset
                if (monsters.Count == 0 && bullets.Count == 0) 
                { 
                    bullets.Clear();
                    roundTimer = 20f; 
                    mainCoffin.Health = 150f;
                    mainCoffin.Position.X = 610; 
                    coffinVelocity = 0; 
                    _currentState = GameState.Combat; 
                }
            }

            // game over keys
            else if (_currentState == GameState.GameOver)
            {
                if (ks.IsKeyDown(Keys.Space) && previousState.IsKeyUp(Keys.Space)) _currentState = GameState.Menu;
                if (ks.IsKeyDown(Keys.Escape)) Exit();
            }

            previousState = ks;
            base.Update(gameTime);
        }

        private void FireBullet(bool fromP1)
        {
            Vector2 start = fromP1 ? new Vector2(player1.Bounds.Right, 510) : new Vector2(player2.Bounds.Left, 510);
            Vector2 velocity = fromP1 ? new Vector2(1600f, 0) : new Vector2(-1600f, 0);
            bullets.Add(new Bullet { Position = start, Velocity = velocity, FromPlayer1 = fromP1 });
        }

        private void UpdateBullets(float dt)
        {
            for (int i = bullets.Count - 1; i >= 0; i--)
            {
                bullets[i].Update(dt);
                bool hit = false;

                // coffin hits
                if (_currentState == GameState.Combat && bullets[i].Bounds.Intersects(mainCoffin.Bounds))
                {
                    mainCoffin.Health -= 1.2f; 
                    coffinVelocity += bullets[i].FromPlayer1 ? (175f * p1PushPower) : (-175f * p2PushPower);
                    if (bullets[i].FromPlayer1) p1Gold += 5; else p2Gold += 5;
                    hit = true;
                }
                // monsters hits
                else if (_currentState == GameState.Invasion)
                {
                    for (int j = monsters.Count - 1; j >= 0; j--)
                    {
                        if (bullets[i].Bounds.Intersects(monsters[j].Bounds))
                        {
                            monsters[j].Health--;
                            
                            if (monsters[j].Health <= 0)
                                monsters.RemoveAt(j);
                            hit = true;    
                            break;
                        }
                    }
                }

                if (hit || bullets[i].Position.X < 0 || bullets[i].Position.X > 1280) bullets.RemoveAt(i);
            }
        }

        private void SpawnMonsters() {
            Vector2 pos = mainCoffin.Position;
            int targetSide = (pos.X < 640) ? 1 : 2; // movement of monsters

            if (waveCount == 1) 
                for(int i=0; i<3; i++) monsters.Add(new Monster(new Vector2(pos.X + (i*70), pos.Y), 1, targetSide));
            else if (waveCount == 2) { 
                for(int i=0; i<3; i++) monsters.Add(new Monster(new Vector2(pos.X + (i*70), pos.Y), 1, targetSide)); 
                for(int i=0; i<2; i++) monsters.Add(new Monster(new Vector2(pos.X + (i*90), pos.Y), 2, targetSide)); 
            }
            else monsters.Add(new Monster(pos, 3, targetSide)); // Boss
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DimGray); 
            _spriteBatch.Begin();

            if (_currentState == GameState.Menu) // menu screen
            {
                _spriteBatch.DrawString(_font, "COFFIN GAME", new Vector2(560, 240), Color.White);
                _spriteBatch.DrawString(_font, "PRESS [SPACE] TO START", new Vector2(480, 310), Color.Yellow);
                _spriteBatch.DrawString(_font, "PRESS [ESC] TO QUIT", new Vector2(510, 380), Color.Red);
            }
            else if (_currentState == GameState.GameOver) // game over screen
            {
                _spriteBatch.DrawString(_font, winnerText, new Vector2(540, 300), Color.Red);
                _spriteBatch.DrawString(_font, "PRESS [SPACE] FOR MENU", new Vector2(480, 370), Color.Yellow);
            }
            else
            {
                player1.Draw(_spriteBatch, _pixel);
                player2.Draw(_spriteBatch, _pixel);
                mainCoffin.Draw(_spriteBatch, _pixel);
                foreach (var m in monsters) m.Draw(_spriteBatch, _pixel);
                foreach (var b in bullets) b.Draw(_spriteBatch, _pixel);

                _spriteBatch.DrawString(_font, "GOLD: " + p1Gold, new Vector2(50, 20), Color.Gold);
                _spriteBatch.DrawString(_font, "GOLD: " + p2Gold, new Vector2(1050, 20), Color.Gold);

                if (_currentState == GameState.Combat) // combat  screen
                    _spriteBatch.Draw(_pixel, new Rectangle(0, 0, (int)(1280 * (roundTimer / 20f)), 15), Color.Yellow * 0.7f);

                if (_currentState == GameState.Shop) // shop screen
                {
                    _spriteBatch.Draw(_pixel, new Rectangle(340, 160, 600, 400), Color.Black * 0.85f);
                    _spriteBatch.DrawString(_font, "--- SHOP ---", new Vector2(560, 190), Color.White);
                    _spriteBatch.DrawString(_font, "P1 Push Power [W]: 500 Gold", new Vector2(380, 280), Color.Cyan);
                    _spriteBatch.DrawString(_font, "P2 Push Power [UP]: 500 Gold", new Vector2(380, 340), Color.Tomato);
                    _spriteBatch.DrawString(_font, "PRESS [SPACE] TO SUMMON MONSTERS", new Vector2(420, 500), Color.Yellow);
                }
            }
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}