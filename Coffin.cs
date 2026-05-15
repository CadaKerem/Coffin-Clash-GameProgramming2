using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CoffinGame
{
    public class Coffin
    {
        public Vector2 Position;
        public float Health = 150f; //coffin hp
        
        public Rectangle Bounds => new Rectangle((int)Position.X + 40, (int)Position.Y, 144, 203); // hitbox
        
        private Texture2D _texture;
        private int frameWidth = 320;
        private int frameHeight = 290;
        private int currentFrame;
        private float timer;
        private float interval = 80f; // for time based anim 
        private int totalFrames = 8;
        private float scale = 0.7f; // coffin scale

        public Coffin(Vector2 startPos, Texture2D texture)
        {
            Position = new Vector2(startPos.X, 850 - (frameHeight * scale));
            _texture = texture;
        }

        public void Update(float velocity, float dt, GameTime gameTime) // frame by frame anim
        {
            Position.X += velocity * dt;
            Position.X = MathHelper.Clamp(Position.X, 0, 1920 - (frameWidth * scale));

            if (velocity > 15f || velocity < -15f)
            {
                timer += dt * 1000;
                if (timer > interval)
                {
                    currentFrame++;
                    if(currentFrame >= totalFrames)
                    {
                        currentFrame = 0;
                    }
                    timer = 0;
                }
            }
            else
            {
                currentFrame = 0;
            }
        }

        public void Draw(SpriteBatch sb, Texture2D pixel)
        {
            Rectangle sourceRect = new Rectangle(currentFrame * frameWidth, 0, frameWidth, frameHeight); // frame by frame draw
            
            sb.Draw(_texture, Position, sourceRect, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            
            Rectangle bg = new Rectangle((int)Position.X + 40, (int)Position.Y - 25, 144, 8);
            Rectangle fill = new Rectangle((int)Position.X + 40, (int)Position.Y - 25, (int)(144 * (Health / 150f)), 8);
            sb.Draw(pixel, bg, Color.Black * 0.5f);
            sb.Draw(pixel, fill, Color.LimeGreen);
        }
    }
}