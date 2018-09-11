using System;
using System.Collections.Generic;

namespace SpaceExplorer
{
    // the game class
    // encapsulates and provides methods to operate on game state
    class SpaceExplorer
    {
        const int RESET_TIMER = 100;

        // processing multiplier to compensate for fast or slow game loop
        internal static float Delta { get; private set; } = 1.0f;
        private static Random random = new Random();

        // game state
        internal Player Player1 { get; private set; }
        internal List<Bullet> Bullets { get; private set; }
        internal List<Enemy> Enemies { get; private set; }

        private float resetTimer;

        internal SpaceExplorer()
        {
            Player1 = new Player();
            Bullets = new List<Bullet>();
            Enemies = new List<Enemy>();
            resetTimer = RESET_TIMER;
        }

        // the "tick" function, processes all game entities
        internal void Step(ref Controls controls)
        {
            // process player
            Player1.Step(ref controls, Bullets);

            // process bullets
            Bullet.Step(Bullets, Enemies, Player1);

            // process enemies
            Enemy.Step(Enemies, Bullets, Player1);

            // check for gameover
            if (Player1.Health < 1)
            {
                if(resetTimer <= 0.0f)
                Reset();
                else
                    resetTimer -= Delta;
            }
        }

        internal static int RandomInt(int min, int max)
        {
            return random.Next(min, max);
        }
        
        // when the player dies
        internal void Reset()
        {
            Player1 = new Player();
            Bullets.Clear();
            Enemies.Clear();
            resetTimer = RESET_TIMER;
        }

        // input controls
        internal struct Controls
        {
            private float intensity;

            internal float LookAngle { get; set; }
            internal bool Firing { get; set; }
            internal float MovementAngle { get; set; }
            internal float Intensity
            {
                get { return intensity; }
                set
                {
                    if (value > 1.0)
                        intensity = 1.0f;
                    else
                        intensity = value;
                }
            }
        }
    }
}
