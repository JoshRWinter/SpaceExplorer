using System;
using System.Collections.Generic;

namespace SpaceExplorer
{
    class Player : Entity
    {
        internal const int WIDTH = 40;
        internal const int HEIGHT = 40;
        internal const int MAX_SPEED = 5;
        internal const int TIMER_FIRE = 5;

        internal int Health { get; private set; }
        float TimerFire { get; set; }

        internal Player() : base(100.0f, 100.0f, WIDTH, HEIGHT)
        {
            Health = 100;
            TimerFire = 0.0f;
        }

        // process the player entity
        internal void Step(ref SpaceExplorer.Controls controls, List<Bullet> bulletList)
        {
            Xv = (float)Math.Cos(controls.MovementAngle) * controls.Intensity * MAX_SPEED;
            Yv = (float)Math.Sin(controls.MovementAngle) * controls.Intensity * MAX_SPEED;

            // update player position
            X += Xv;
            Y += Yv;

            // update look angle
            Rot = controls.LookAngle + (float)Math.PI;

            // shoot
            if (controls.Firing && TimerFire <= 0.0f)
            {
                bulletList.Add(new Bullet(this));
                TimerFire = TIMER_FIRE;
            }

            if(TimerFire > 0.0)
                TimerFire -= SpaceExplorer.Delta;
        }

        internal void Hit()
        {
            Health -= (int)(Bullet.DAMAGE / 1.5);
        }
    }
}
