using System;

namespace SpaceExplorer
{
    // the Entity base class for all in game entities (players, enemies, bullets, etc)
    class Entity
    {
        internal Entity(float x, float y, int width, int height, float rot = 0.0f, float xv = 0.0f, float yv = 0.0f)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Rot = rot;
            Xv = xv;
            Yv = yv;
        }

        // bounding box collision detection
        // positive tolerance to allow entities to slightly collide
        // negative tolerance will return true even if enemies are not quite colliding
        internal bool Collide(Entity other, double tolerance = 0.0)
        {
            return X + Width > other.X + tolerance && X < (other.X + other.Width) - tolerance && Y + Height > other.Y + tolerance && Y < (other.Y + other.Height) - tolerance; // sufficiently cryptic
        }

        // on screen coordinates
        internal float X { get; set; }
        internal float Y { get; set; }

        internal int Width { get; set; }
        internal int Height { get; set; }

        internal float Xv { get; set; } // x axis velocity
        internal float Yv { get; set; } // y axis velocity

        internal float Rot { get; set; } // rotation angle (radians)
    }
}
