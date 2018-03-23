using System;
using System.Collections.Generic;

namespace SpaceExplorer
{
    class Bullet : Entity
    {
        internal const int WIDTH = 10;
        internal const int HEIGHT = 10;
        internal const int SPEED = 10;
        internal const int DAMAGE = 12;

        float ttl; // time to live

        internal Entity Parent { get; private set; }

        internal Bullet(Entity parent) : base(parent.X + (parent.Width / 2), parent.Y + (parent.Height / 2), WIDTH, HEIGHT, parent.Rot)
        {
            Xv = -(float)Math.Cos(Rot) * SPEED;
            Yv = -(float)Math.Sin(Rot) * SPEED;
            ttl = 100;
            Parent = parent;
        }

        // process the bullet entities
        internal static void Step(List<Bullet> bulletList, List<Enemy> enemyList, Player player)
        {
            for(int i = 0; i < bulletList.Count;)
            {
                Bullet bullet = bulletList[i];

                // update the bullet's position
                bullet.X += bullet.Xv * SpaceExplorer.Delta;
                bullet.Y += bullet.Yv * SpaceExplorer.Delta;

                // update ttl
                bullet.ttl -= SpaceExplorer.Delta;
                if(bullet.ttl <= 0.0f)
                {
                    bulletList.RemoveAt(i);
                    continue;
                }

                // check for collisions with enemies
                foreach (Enemy enemy in enemyList)
                {
                    if(bullet.Collide(enemy) && bullet.Parent == player)
                    {
                        enemy.Hit();
                        bulletList.RemoveAt(i);
                        continue;
                    }
                }

                // check for collision with player
                if (bullet.Collide(player) && bullet.Parent != player)
                {
                    player.Hit();
                    bulletList.RemoveAt(i);
                    continue;
                }

                ++i;
            }
        }
    }
}
