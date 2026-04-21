using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CoffinGame
{
    public class Tower
    {
        public Rectangle Bounds;
        public int Health;

        public Tower(int x, int y)
        {
            
            Bounds = new Rectangle(x, y, 80, 200);
            Health = 100;
        }

        public void Draw(SpriteBatch sb, Texture2D pixel)
        {
            
            sb.Draw(pixel, Bounds, Color.Black);
            
           
            Rectangle healthBar = new Rectangle(Bounds.X, Bounds.Y - 20, (int)(Bounds.Width * (Health / 100f)), 10);
            sb.Draw(pixel, healthBar, Color.Red);
        }
    }
}