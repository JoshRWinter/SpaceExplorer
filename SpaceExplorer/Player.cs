using System;

namespace SpaceExplorer
{
    class Player : Entity
    {
        internal const int WIDTH = 40;
        internal const int HEIGHT = 40;
        internal const int MAX_SPEED = 5;

        int Health { get; set; }
        double TimerFire { get; set; }

        internal Player() : base(100.0f, 100.0f, WIDTH, HEIGHT)
        {
            Health = 100;
            TimerFire = 0.0;
        }

        // process the player entity
        internal void Step(ref SpaceExplorer.Controls controls)
        {
            Xv = (float)Math.Cos(controls.MovementAngle) * controls.Intensity * MAX_SPEED;
            Yv = (float)Math.Sin(controls.MovementAngle) * controls.Intensity * MAX_SPEED;

            // update player position
            X += Xv;
            Y += Yv;

            // update look angle
            Rot = controls.LookAngle + (float)Math.PI;
        }
    }
}
