using System;

namespace SpaceExplorer
{
    // the game class
    // encapsulates and provides methods to operate on game state
    class SpaceExplorer
    {
        // processing multiplier to compensate for fast or slow game loop
        internal static double Delta { get; private set; } = 1.0;

        // game state
        internal Player Player1 { get; private set; }

        internal SpaceExplorer()
        {
            Player1 = new Player();
        }

        // the "tick" function, processes all game entities
        internal void Step(ref Controls controls)
        {
            Player1.Step(ref controls);
        }

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
