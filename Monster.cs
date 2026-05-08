using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CoffinGame
{
    public class Monster
    {
        public Vector2 Position;
        public int Health, MaxHealth, Damage, Size;
        public int TargetSide; 
        public Color MonsterColor;
        private float speed = 110f; 
        private float attackTimer = 0f;

        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, Size, Size);

        public Monster(Vector2 startPos, int level, int targetSide)
        {
            this.TargetSide = targetSide;

            // monster info 
            if (level == 1) { MaxHealth = 18; Damage = 5; Size = 45; MonsterColor = Color.LightGreen; }
            else if (level == 2) { MaxHealth = 35; Damage = 10; Size = 60; MonsterColor = Color.DarkGreen; }
            else { MaxHealth = 55; Damage = 20; Size = 90; MonsterColor = Color.Purple; } 

            Position = new Vector2(startPos.X, 550 - Size); 
            Health = MaxHealth;
        }

        public void Update(Vector2 targetPos, float dt, Rectangle targetTowerBounds)
        {
            if (!this.Bounds.Intersects(targetTowerBounds))
            {
                if (Position.X < targetPos.X) Position.X += speed * dt;
                else if (Position.X > targetPos.X) Position.X -= speed * dt;
            }

            if (attackTimer > 0) attackTimer -= dt;
        }

        public bool CanAttack() { if (attackTimer <= 0) { attackTimer = 1.3f; return true; } return false; }

        public void Draw(SpriteBatch sb, Texture2D pixel)
        {
            sb.Draw(pixel, Bounds, MonsterColor);
            Rectangle bg = new Rectangle(Bounds.X, Bounds.Y - 12, Bounds.Width, 6);
            Rectangle fill = new Rectangle(Bounds.X, Bounds.Y - 12, (int)(Bounds.Width * ((float)Health / MaxHealth)), 6);
            sb.Draw(pixel, bg, Color.Black * 0.5f);
            sb.Draw(pixel, fill, Color.Red);
        }
    }
}