using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CoffinGame
{
    public class Coffin
    {
        public Vector2 Position;
        
        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, 60, 100);
        public float Speed = 300f;

        public Coffin(Vector2 startPos)
        {
            Position = startPos;
        }

        public void Update(float direction, float deltaTime)
        {
            
            Position.X += direction * Speed * deltaTime;

            
            Position.X = MathHelper.Clamp(Position.X, 100, 1120);
        }

        public void Draw(SpriteBatch sb, Texture2D pixel)
        {
            
            sb.Draw(pixel, Bounds, Color.White);
        }
    }
}