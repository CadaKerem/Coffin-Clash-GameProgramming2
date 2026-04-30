using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CoffinGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _pixel;
        private SpriteFont _font; 

        enum GameState { Menu, Combat, Shop }
        GameState _currentState = GameState.Menu;

        Tower player1;
        Tower player2;
        Coffin mainCoffin;

        float roundTimer = 20f;
        int p1Gold = 0;
        int p2Gold = 0;

        float p1PushPower = 1f;
        float p2PushPower = 1f;

        KeyboardState previousState;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
        }

        protected override void Initialize()
        {
            ResetGame();
            base.Initialize();
        }

        private void ResetGame()
        {
            player1 = new Tower(50, 400); 
            player2 = new Tower(1150, 400); 
            mainCoffin = new Coffin(new Vector2(610, 450));
            p1Gold = 0;
            p2Gold = 0;
            p1PushPower = 1f;
            p2PushPower = 1f;
            roundTimer = 20f;
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

            if (_currentState == GameState.Menu) //Menu
            {
                if (ks.IsKeyDown(Keys.Space) && previousState.IsKeyUp(Keys.Space))
                {
                    ResetGame();
                    _currentState = GameState.Combat;
                }
            }
            else if (_currentState == GameState.Combat) //Combat
            {
                roundTimer -= dt;
                float moveDir = 0;

                if (ks.IsKeyDown(Keys.D)) 
                { 
                    moveDir += 1 * p1PushPower; //p1 control key
                    p1Gold++; 
                }
                
                if (ks.IsKeyDown(Keys.Left))  //p2 control key
                { 
                    moveDir -= 1 * p2PushPower; 
                    p2Gold++; 
                }

                mainCoffin.Update(moveDir, dt); 

                if (roundTimer <= 0)
                {
                    if (mainCoffin.Position.X < 640) player1.Health -= 20;
                    else player2.Health -= 20;

                    if (player1.Health <= 0 || player2.Health <= 0)
                        _currentState = GameState.Menu;
                    else
                        _currentState = GameState.Shop;
                }
            }
            else if (_currentState == GameState.Shop) //Shop
            {
                if (ks.IsKeyDown(Keys.W) && previousState.IsKeyUp(Keys.W) && p1Gold >= 500)
                {
                    p1Gold -= 500;
                    p1PushPower += 0.2f;
                }

                if (ks.IsKeyDown(Keys.Up) && previousState.IsKeyUp(Keys.Up) && p2Gold >= 500)
                {
                    p2Gold -= 500;
                    p2PushPower += 0.2f;
                }

                if (ks.IsKeyDown(Keys.Space) && previousState.IsKeyUp(Keys.Space))
                {
                    roundTimer = 20f;
                    mainCoffin.Position.X = 610; 
                    _currentState = GameState.Combat;
                }
            }

            previousState = ks;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DimGray); 
            _spriteBatch.Begin();

            if (_currentState == GameState.Menu) //Menu
            {
                _spriteBatch.DrawString(_font, "COFFIN GAME", new Vector2(560, 280), Color.White);
                _spriteBatch.DrawString(_font, "PRESS [SPACE] TO START GAME", new Vector2(480, 350), Color.Yellow);
            }
            else if (_currentState == GameState.Combat || _currentState == GameState.Shop)
            {
                player1.Draw(_spriteBatch, _pixel);
                player2.Draw(_spriteBatch, _pixel);
                mainCoffin.Draw(_spriteBatch, _pixel);

                _spriteBatch.DrawString(_font, "GOLD: " + p1Gold, new Vector2(50, 20), Color.Gold);
                _spriteBatch.DrawString(_font, "GOLD: " + p2Gold, new Vector2(1050, 20), Color.Gold);

                _spriteBatch.Draw(_pixel, new Rectangle(0, 0, (int)(1280 * (roundTimer / 20f)), 15), Color.Yellow * 0.5f);

                if (_currentState == GameState.Shop) //Shop
                {
                    _spriteBatch.Draw(_pixel, new Rectangle(340, 160, 600, 400), Color.Black * 0.85f);
                    
                    _spriteBatch.DrawString(_font, "--- MARKET ---", new Vector2(560, 190), Color.White);
                    _spriteBatch.DrawString(_font, "P1 Power Up [W]: 500 Gold", new Vector2(380, 280), Color.Cyan);
                    _spriteBatch.DrawString(_font, "P2 Power Up [UP]: 500 Gold", new Vector2(380, 340), Color.Tomato);
                    _spriteBatch.DrawString(_font, "PRESS [SPACE] TO CONTINUE", new Vector2(470, 500), Color.Yellow);
                }
            }


            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}