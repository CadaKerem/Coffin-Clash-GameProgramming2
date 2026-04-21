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

        
        enum GameState { Combat, Shop }
        GameState _currentState = GameState.Combat;

        
        Tower player1;
        Tower player2;
        Coffin mainCoffin;

        float roundTimer = 20f;
        int p1Gold = 0;
        int p2Gold = 0;

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
            player1 = new Tower(50, 400); 
            player2 = new Tower(1150, 400); 
            mainCoffin = new Coffin(new Vector2(610, 450)); 

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            
            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }

        protected override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState ks = Keyboard.GetState();

            if (_currentState == GameState.Combat)
            {
                roundTimer -= dt;
                float moveDir = 0;

                //player1 control
                if (ks.IsKeyDown(Keys.D)) { moveDir += 1; p1Gold++; }
                //player2 control
                if (ks.IsKeyDown(Keys.Left)) { moveDir -= 1; p2Gold++; }

                mainCoffin.Update(moveDir, dt);

                
                if (roundTimer <= 0)
                {
                    if (mainCoffin.Position.X < 640) player1.Health -= 20;
                    else player2.Health -= 20;

                    _currentState = GameState.Shop;
                }
            }
            else 
            {
                
                if (ks.IsKeyDown(Keys.Space))
                {
                    roundTimer = 20f;
                    mainCoffin.Position.X = 610;
                    _currentState = GameState.Combat;
                }
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DimGray); 
            _spriteBatch.Begin();

           
            player1.Draw(_spriteBatch, _pixel);
            player2.Draw(_spriteBatch, _pixel);
            mainCoffin.Draw(_spriteBatch, _pixel);

           
            _spriteBatch.Draw(_pixel, new Rectangle(0, 0, (int)(1280 * (roundTimer / 20f)), 20), Color.Yellow * 0.5f);

            
            if (_currentState == GameState.Shop)
            {
                _spriteBatch.Draw(_pixel, new Rectangle(340, 160, 600, 400), Color.Black * 0.7f);
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}