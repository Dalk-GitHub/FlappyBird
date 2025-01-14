﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FlappyBird.Death;
using FlappyBird.Game;
using FlappyBird.Menu;
using Logging.Net;

namespace FlappyBird.Engine
{
    public enum ComponentActivityMode
    {
        Playing,
        Dead,
        Menu,
        Win
    }
    public class FlappyBirdApplication
    {
        public static List<IFlappyCompound> Compounds { get; set; } = new List<IFlappyCompound>();
        public static int MainMenuZ = 404;
        public static int MainMenuButtonZ = 500;
        public static int BirdZ = 7500;
        internal static int EventControllerZ = 100;
        static Dictionary<int, bool> Values { get; set; } = new Dictionary<int, bool>()
        {
            {
                0,
                false
            },
            {
                1,
                false
            }
        };
        public void PutOn(Graphics g, int width, int height)
        {
            Render(g, width, height);
        }
        public static Color BackColor { get; set; } = Color.FromArgb(0x00, 0x95, 0xDB);
        public static ComponentActivityMode Playing { get; set; } = ComponentActivityMode.Menu;
        static Point lastHov = new Point(0, 0);
        public static void Render(Graphics g, int width, int height)
        {
            if (!Values[0])
            {
                Values[0] = true;
                Setup();
            }
            g.Clear(BackColor);
            var cpsx = new IFlappyCompound[Compounds.Count];
            Compounds.CopyTo(cpsx);
            var cps = cpsx.ToList();
            cps.Sort((x, y) => x.GetZ().CompareTo(y.GetZ()));

            foreach (var cl in cps)
            {
                if (cl.IsActive)
                {
                    cl.DoPhysics();
                    cl.BeforeRender();
                    if (cl.GetRectangle().IntersectsWith(new Rectangle(lastHov, new System.Drawing.Size(8, 8))))
                        cl.Hover();
                    var render = cl.GetFrame();
                    var rect = cl.GetRectangle();
                    g.DrawImage(render, rect.X, rect.Y, rect.Width, rect.Height);
                    render.Dispose();
                }
            }
        }

        public void HandleKey(KeyEventArgs e)
        {
            WorkWithKey(e);
        }

        public static void WorkWithKey(KeyEventArgs e)
        {

        }

        public static void Shutdown()
        {
            Values[1] = true;
        }

        internal static readonly MainMenuPlayButton plb = new MainMenuPlayButton();
        internal static readonly MainMenuDisplay mmd = new MainMenuDisplay();
        internal static readonly Bird bird = new Bird();
        internal static readonly BirdController birdEventCollector = new BirdController();
        internal static readonly DeathDisplay deathDisplay = new DeathDisplay();
        internal static readonly BackgroundImage bg = new BackgroundImage();
        public static void Setup()
        {
            Compounds.Add(mmd);
            Compounds.Add(bird);
            Compounds.Add(birdEventCollector);
            Compounds.Add(plb);
            Compounds.Add(deathDisplay);
            Compounds.Add(bg);
            FlappyMapLoader.LoadOn(new ComponentAdding((e) =>
            {
                Compounds.Add(e);
            }));
            Thread upd = null;
            upd = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    if (Values[1])
                        break;
                    foreach (var c in Compounds)
                    {
                        c.Update();
                    }
                    Thread.Sleep(100);
                }
                upd.Abort();
            }));
            upd.Start();
        }

        public void Hovering(Point point)
        {
            lastHov = point;
        }

        public void Click(Point point)
        {
            PerformClick(point);
        }
        public static void PerformClick(Point p)
        {
            foreach (var cl in Compounds)
            {
                if (cl.GetRectangle().IntersectsWith(new Rectangle(p, new System.Drawing.Size(8, 8))))
                    cl.Click();
            }
        }
    }
}