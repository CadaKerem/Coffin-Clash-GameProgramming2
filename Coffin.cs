using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CoffinGame
{
    public class Coffin
    {
        public Vector2 Position;
        public float Health = 150f; //coffin hp
        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, 60, 100);

        public Coffin(Vector2 startPos) { Position = startPos; }

        public void Update(float velocity, float dt)
        {
            Position.X += velocity * dt;
            Position.X = MathHelper.Clamp(Position.X, 100, 1120);
        }

        public void Draw(SpriteBatch sb, Texture2D pixel)
        {
            sb.Draw(pixel, Bounds, Color.White);
            Rectangle bg = new Rectangle(Bounds.X, Bounds.Y - 20, Bounds.Width, 8);
            Rectangle fill = new Rectangle(Bounds.X, Bounds.Y - 20, (int)(Bounds.Width * (Health / 150f)), 8);
            sb.Draw(pixel, bg, Color.Black * 0.5f);
            sb.Draw(pixel, fill, Color.LimeGreen);
        }
    }
}