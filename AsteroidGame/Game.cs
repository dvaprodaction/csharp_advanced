using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using AsteroidGame.VisualObjects;
using AsteroidGame.VisualObjects.Interfaces;

namespace AsteroidGame
{
    static class Game
    {
        private const int __FrameTimeout = 40;
        public static int GamePoints = 0;

        private static BufferedGraphicsContext __Context;
        private static BufferedGraphics __Buffer;
        private static Timer __Timer;

        private static VisualObject[] __GameObjects;
        private static List<Bullet> __Bullets = new List<Bullet>();
        private static List<Asteroid> __Asteroids = new List<Asteroid>();
        private static SpaceShip __SpaceShip;

        private static int asteroids_count = 15;
        private const int asteroid_size = 25;
        private const int asteroid_max_speed = 20;

        public static int Width { get; set; }
        public static int Height { get; set; }
        
        public static void Initialize(Form form)
        {
            Width = form.Width;
            Height = form.Height;

            Graphics g = form.CreateGraphics();
            __Context = BufferedGraphicsManager.Current;
            __Buffer = __Context.Allocate(g, new Rectangle(0, 0, Width, Height));

            var timer = new Timer { Interval = __FrameTimeout };
            timer.Tick += OnTimerTick;
            timer.Start();

            __Timer = timer;

            form.KeyDown += OnFormKeyDown;
        }

        private static void OnFormKeyDown(object Sender, KeyEventArgs E)
        {
            switch (E.KeyCode)
            {
                case Keys.ControlKey:
                    __Bullets.Add(new Bullet(__SpaceShip.Position.Y));
                    break;

                case Keys.Up:
                    __SpaceShip.MoveUp();
                    break;

                case Keys.Down:
                    __SpaceShip.MoveDown();
                    break;
            }
        }

        private static void OnTimerTick(object sender, EventArgs e)
        {
            Update();
            Draw();
        }

        public static void Load()
        {
            var game_objects = new List<VisualObject>();
            var rnd = new Random();

            const int stars_count = 150;
            const int star_size = 6;
            const int star_max_speed = 20;
            for (var i = 0; i < stars_count; i++)
                game_objects.Add(new Star(
                    new Point(rnd.Next(0, Width), rnd.Next(0, Height)), 
                    new Point(rnd.Next(0, star_max_speed), 0), 
                    star_size));

            //const int asteroids_count = 15;
            //const int asteroid_size = 25;
            //const int asteroid_max_speed = 20;
            for (var i = 0; i < asteroids_count; i++)
                __Asteroids.Add(new Asteroid(
                    new Point(rnd.Next(0, Width), rnd.Next(0, Height)),
                    new Point(-rnd.Next(0, asteroid_max_speed), 0),
                    asteroid_size));

            __GameObjects = game_objects.ToArray();

            __SpaceShip = new SpaceShip(new Point(10, 400), new Point(5, 5), new Size(20, 20));
            __SpaceShip.ShipDestroyed += OnShipDestroyed;
        }

        private static void OnShipDestroyed(object Sender, EventArgs E)
        {
            __Timer.Stop();
            __Buffer.Graphics.Clear(Color.DarkBlue);
            __Buffer.Graphics.DrawString("Game over!!!", new Font(FontFamily.GenericSerif, 60, FontStyle.Bold), Brushes.Red, 200, 200);
            __Buffer.Render();
        }

        public static void Draw()
        {
            if (__SpaceShip.Energy <= 0) return;

            var g = __Buffer.Graphics;
            g.Clear(Color.Black);

            __SpaceShip.Draw(g);

            foreach (var bullet in __Bullets)
                bullet.Draw(g);

            foreach (var asteroid in __Asteroids.ToArray())
                asteroid?.Draw(g);

            foreach (var visual_object in __GameObjects)
                visual_object?.Draw(g);

            g.DrawString($"Energy: {__SpaceShip.Energy}", new Font(FontFamily.GenericSansSerif, 14, FontStyle.Italic), Brushes.White, 10, 10);
            g.DrawString($"Game points: {GamePoints}", new Font(FontFamily.GenericSansSerif, 14, FontStyle.Italic), Brushes.White, 10, 30);

            __Buffer.Render();
        }

        public static void Update()
        {
            foreach (var visual_object in __GameObjects)
                visual_object?.Update();

            foreach (var asteroid in __Asteroids)
                asteroid?.Update();

            var bullets_to_remove = new List<Bullet>();
            foreach (var bullet in __Bullets)
            {
                bullet.Update();
                if (bullet.Position.X > Width)
                    bullets_to_remove.Add(bullet);
            }

            var asteroids_to_remove = new List<Asteroid>();

            for (var i = 0; i < __Asteroids.Count; i++)
            {
                var obj = __Asteroids[i];
                
                if (obj is ICollision) // Применить "сопоставление с образцом"!
                {
                    var collision_object = (ICollision)obj;
                    __SpaceShip.CheckCollision(collision_object);

                    foreach (var bullet in __Bullets.ToArray())
                        if (bullet.CheckCollision(collision_object))
                        {
                            bullets_to_remove.Add(bullet);
                            asteroids_to_remove.Add(__Asteroids[i]);
                            GamePoints++;
                        }
                    //patch
                }
            }

            foreach (var bullet in bullets_to_remove)
                __Bullets.Remove(bullet);

            foreach (var asteroid in asteroids_to_remove)
                __Asteroids.Remove(asteroid);

            if (__Asteroids.Count == 0)
            {
                asteroids_count++;
                var rnd = new Random();
                for (var j = 0; j < asteroids_count; j++)
                    __Asteroids.Add(new Asteroid(
                        new Point(rnd.Next(0, Width), rnd.Next(0, Height)),
                        new Point(-rnd.Next(0, asteroid_max_speed), 0),
                        asteroid_size));
            }

        }
    }
}
