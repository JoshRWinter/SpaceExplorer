using System;
using System.Windows.Forms;
using System.Drawing;
using System.Collections.Generic;

namespace SpaceExplorer
{
    // represents the front-end for the game
    class Window : Form
    {
        Timer stepTimer;
        SpaceExplorer game;
        SpaceExplorer.Controls controls;
        Assets assets;

        Font font, fontBig;
        Brush brush = new SolidBrush(Color.Red);

        // keyboard keypresses
        bool btnUp;
        bool btnDown;
        bool btnRight;
        bool btnLeft;

        internal Window()
        {
            game = new SpaceExplorer();
            assets = new Assets();

            // window settings
            Text = "Welcome to Space Explorer!";
            Size = new Size(800, 600);
            DoubleBuffered = true;
            font = new Font("Courier New", 10, FontStyle.Regular);
            fontBig = new Font("Courier New", 30, FontStyle.Italic);

            // setup a timer to refresh the screen and process game entities
            stepTimer = new Timer();
            stepTimer.Tick += new EventHandler(Step);
            stepTimer.Interval = 16;
            stepTimer.Enabled = true;
            stepTimer.Start();

            // controls
            controls.MovementAngle = 0.0f;
            controls.LookAngle = 0.0f;
            controls.Intensity = 0.0f;
            controls.Firing = false;
            btnRight = false;
            btnLeft = false;
            btnDown = false;
            btnUp = false;
        }

        // called on a timer (16ms)
        private void Step(object sender, EventArgs args)
        {
            game.Step(ref controls); // allow the game to process its entities
            Refresh(); // trigger a repaint
        }

        // draw the game
        protected override void OnPaint(PaintEventArgs painter)
        {
            // collect some state from the game class
            Player player = game.Player1;
            List<Bullet> bullets = game.Bullets;
            List<Enemy> enemies = game.Enemies;

            // draw the player
            {
                Bitmap tex = assets.GetPlayer(player.Rot);
                float px = player.X;
                float py = player.Y;
                AdjustCoords(ref px, ref py, player);
                float x = (px + (Player.WIDTH / 2)) - (tex.Width / 2);
                float y = (py + (Player.HEIGHT / 2)) - (tex.Height / 2);
                painter.Graphics.DrawImage(tex, x, y, tex.Width, tex.Height);
            }

            // draw the bullets
            Pen pen = new Pen(Color.Black);
            foreach(Bullet bullet in bullets)
            {
                float x = bullet.X;
                float y = bullet.Y;
                AdjustCoords(ref x, ref y, player);
                painter.Graphics.DrawLine(pen, x, y, x - bullet.Xv, y - bullet.Yv);
            }

            // draw the enemies
            foreach(Enemy enemy in enemies)
            {
                Bitmap tex = assets.GetEnemy(enemy.Rot);
                float ex = enemy.X;
                float ey = enemy.Y;
                AdjustCoords(ref ex, ref ey, player);
                float x = (ex + (Enemy.WIDTH / 2)) - (tex.Width / 2);
                float y = (ey + (Enemy.HEIGHT / 2)) - (tex.Height / 2);
                painter.Graphics.DrawImage(tex, x, y, tex.Width, tex.Height);
            }

            // draw the HUD
            painter.Graphics.DrawString($"Health: {(player.Health < 0 ? 0 : player.Health)}", font, brush, (Width / 2) - 100, Height - 70);
            if (player.Health < 1)
                painter.Graphics.DrawString("You Died", fontBig, brush, (Width / 2) - 90, (Height / 2) - 100);
            painter.Graphics.DrawString($"Score: {player.Score}", font, brush, (Width / 2) - 100, Height - 90);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            Point p = e.Location;
            float px = Width / 2;
            float py = Height / 2;

            controls.LookAngle =(float)Math.Atan2(p.Y - (py + (Player.HEIGHT / 2)), p.X - (px + (Player.WIDTH / 2)));
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            controls.Firing = true;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            controls.Firing = false;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            ProcessKey(e.KeyCode, true);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            ProcessKey(e.KeyCode, false);
        }

        private void ProcessKey(Keys key, bool press)
        {
            switch(key)
            {
                case Keys.Up:
                case Keys.W:
                case Keys.Oemcomma:
                    btnUp = press;
                    break;
                case Keys.A:
                case Keys.Left:
                    btnLeft = press;
                    break;
                case Keys.Down:
                case Keys.S:
                case Keys.O:
                    btnDown = press;
                    break;
                case Keys.Right:
                case Keys.E:
                case Keys.D:
                    btnRight = press;
                    break;
            }

            // recalculate controls
            int xaxis = 0;
            int yaxis = 0;

            if (btnLeft)
                xaxis = -1;
            if (btnRight)
                xaxis = 1;
            if (btnDown)
                yaxis = 1;
            if (btnUp)
                yaxis = -1;

            controls.MovementAngle = (float)Math.Atan2(yaxis, xaxis);
            if (xaxis == 0 && yaxis == 0)
                controls.Intensity = 0.0f;
            else
                controls.Intensity = 1.0f;
        }

        private void AdjustCoords(ref float x, ref float y, Player player)
        {
            x = x - (player.X + (Player.WIDTH / 2)) + (Width / 2);
            y = y - (player.Y + (Player.HEIGHT / 2)) + (Height / 2);
        }
    }

    // hold bitmap assets (textures)
    internal class Assets
    {
        private Bitmap[] player;
        private Bitmap[] enemy;

        internal Assets()
        {
            try
            {
                Load();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        // precompute 360 degrees of rotation for each bitmap
        private void Load()
        {
            player = new Bitmap[360];
            enemy = new Bitmap[360];
            
            Bitmap playerBmp = new Bitmap(@"C:\Users\Josh\Desktop\SpaceExplorer\SpaceExplorer\assets\player.png");
            Bitmap enemyBmp = new Bitmap(@"C:\Users\Josh\Desktop\SpaceExplorer\SpaceExplorer\assets\enemy.png");

            for(int i = 0; i < 360; ++i)
            {
                player[i] = Rotate(playerBmp, i);
                enemy[i] = Rotate(enemyBmp, i);
            }
        }

        /*
        private Bitmap Rotate(Bitmap orig, float degrees)
        {
            float cos = (float)Math.Abs(Math.Cos(ToRads(degrees)));
            float sin = (float)Math.Abs(Math.Sin(ToRads(degrees)));
            
            int newWidth = (int)(orig.Width * cos + orig.Height * sin);
            int newHeight = (int)(orig.Width * sin + orig.Height * cos);
            Bitmap resized = new Bitmap(newWidth, newHeight);

            using (Graphics g = Graphics.FromImage(resized))
            {
                g.DrawImageUnscaled(orig, 0, 0, resized.Width, resized.Height);
            }

            Bitmap rotated = new Bitmap(resized.Width, resized.Height);

            using (Graphics g = Graphics.FromImage(rotated))
            {
                g.TranslateTransform(resized.Width / 2, resized.Height / 2);
                g.RotateTransform(degrees);
                g.TranslateTransform(-resized.Width / 2, -resized.Height / 2);
                g.DrawImage(orig, 0, 0);
            }

            return rotated;
        }
        */

        // rotate a bitmap
        public static Bitmap Rotate(Bitmap image, float angle)
        {
            //create a new empty bitmap to hold rotated image
            Bitmap rotatedBmp = new Bitmap(image.Width, image.Height);

            //make a graphics object from the empty bitmap
            Graphics g = Graphics.FromImage(rotatedBmp);

            //Put the rotation point in the center of the image
            g.TranslateTransform(image.Width / 2, image.Height / 2);

            //rotate the image
            g.RotateTransform(angle);

            //move the image back
            g.TranslateTransform(-image.Width / 2, -image.Height / 2);

            //draw passed in image onto graphics object
            g.DrawImage(image, new PointF(0, 0));

            g.Dispose();

            return rotatedBmp;
        }

        private static int ToDegrees(float rads)
        {
            int degrees = (int)(rads * (180.0f / (float)Math.PI));
            // normalize
            while (degrees < 0)
                degrees += 360;
            while (degrees >= 360)
                degrees -= 360;

            return degrees;
        }

        private static float ToRads(float deg)
        {
            float rads = deg * ((float)Math.PI / 180.0f);
            // normalize
            while (rads < 0.0)
                rads += (float)Math.PI * 2.0f;
            while (rads >= Math.PI * 2.0)
                rads -= (float)Math.PI * 2.0f;

            return rads;
        }

        internal Bitmap GetPlayer(float radians)
        {
            return player[ToDegrees(radians)];
        }
        
        internal Bitmap GetEnemy(float radians)
        {
            return enemy[ToDegrees(radians)];
        }
    }
}
