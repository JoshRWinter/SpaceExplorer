using System;
using System.Collections.Generic;

namespace SpaceExplorer
{
    class Enemy : Entity
    {
        internal const int WIDTH = 40;
        internal const int HEIGHT = 40;
        internal const int SPEED = 4;
        internal const int SCORE = 10;

        internal const int AGGRO_TIMER = 100; // how long they are aggressive for, decremented once per frame
        internal const int FIRE_TIMER = 20; // cool-down between firing lasers

        float waitTimer; // how long they stay in one spot until moving to a different spot
        float aggroTimer; // timer until enemy forgets about player
        float fireTimer;
        float health = 100;

        Entity destination; // screen coordinates they are heading to, null if stationary

        internal Enemy(float x, float y) : base(x, y, WIDTH, HEIGHT)
        {
            Rot = SpaceExplorer.RandomInt(0, 360) * ((float)Math.PI / 180.0f);
            fireTimer = 0.0f;
            aggroTimer = 0.0f;
            waitTimer = 0.0f;
            destination = null;
            health = 100;
        }

        // process the enemy entities
        internal static void Step(List<Enemy> enemyList, List<Bullet> bulletList, Player player)
        {
            if(enemyList.Count < 5)
            {
                const int spawnBox = 600;
                float newx = SpaceExplorer.RandomInt((int)player.X - spawnBox, (int)player.Y + spawnBox);
                float newy = SpaceExplorer.RandomInt((int)player.Y - spawnBox, (int)player.Y + spawnBox);
                enemyList.Add(new Enemy(newx, newy));
            }

            for(int i = 0; i < enemyList.Count;)
            {
                Enemy enemy = enemyList[i];

                // kill the enemy
                if(enemy.health < 1)
                {
                    enemyList.RemoveAt(i);
                    player.Score += Enemy.SCORE;
                    continue;
                }

                // update the enemy's position
                enemy.X += enemy.Xv * SpaceExplorer.Delta;
                enemy.Y += enemy.Yv * SpaceExplorer.Delta;

                // reduce timers
                if (enemy.fireTimer > 0.0f)
                    enemy.fireTimer -= SpaceExplorer.Delta;
                if (enemy.aggroTimer > 0.0f)
                    enemy.aggroTimer -= SpaceExplorer.Delta;
                if (enemy.fireTimer > 0.0f)
                    enemy.fireTimer -= SpaceExplorer.Delta;
                if (enemy.waitTimer > 0.0f)
                    enemy.waitTimer -= SpaceExplorer.Delta;

                // shoot at player, or wander around
                if(enemy.aggroTimer > 0.0f)
                {
                    // point self at player
                    enemy.Rot = (float)Math.Atan2((enemy.Y + (HEIGHT / 2)) - (player.Y + (Player.HEIGHT / 2)), (enemy.X + (WIDTH / 2)) - (player.X + (Player.WIDTH / 2)));

                    // shoot
                    if(enemy.fireTimer <= 0.0)
                    {
                        bulletList.Add(new Bullet(enemy));
                        enemy.fireTimer = FIRE_TIMER;
                    }
                }
                else
                {
                    // wander around
                    if(enemy.waitTimer <= 0.0 && enemy.destination == null)
                    {
                        const int box = 300;
                        float destx = SpaceExplorer.RandomInt((int)player.X - box, (int)player.X + box);
                        float desty = SpaceExplorer.RandomInt((int)player.Y - box, (int)player.Y + box);

                        enemy.destination = new Entity(destx, desty, 90, 90);
                    }
                    else if(enemy.destination != null)
                    {
                        // point at destination
                        enemy.Rot = (float)Math.Atan2((enemy.Y + (HEIGHT / 2)) - enemy.destination.Y,(enemy.X + (WIDTH / 2)) - enemy.destination.X);
                        enemy.Xv = -(float)Math.Cos(enemy.Rot) * SPEED;
                        enemy.Yv = -(float)Math.Sin(enemy.Rot) * SPEED;

                        if (enemy.Collide(enemy.destination))
                        {
                            enemy.destination = null;
                            enemy.Xv = 0.0f;
                            enemy.Yv = 0.0f;
                            enemy.waitTimer = SpaceExplorer.RandomInt(50, 70);
                        }
                    }
                }

                ++i;
            }

        }

        // this enemy was hit by a laser
        internal void Hit()
        {
            aggroTimer = AGGRO_TIMER;
            Xv = 0.0f;
            Yv = 0.0f;
            health -= Bullet.DAMAGE;
        }
    }
}
