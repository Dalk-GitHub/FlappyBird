﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlappyBird
{
    public class GameRenderComponent : Control
    {
        public int MaxFPS { get; set; } = 30;
        public bool Active { get; private set; } = false;
        public FlappyBirdApplication FlappyBird { get; set; } = new FlappyBirdApplication();

        public void StartPlaying()
        {
            Active = true;
            Thread t = new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    RunANewRender();
                    Thread.Sleep(1000 / MaxFPS);
                }
            }));
            t.Start();
        }

        private void RunANewRender()
        {
            if (Active)
                if (Height > 0)
                    if (Width > 0)
                        try
                        {
                            Thread render = null;
                            render = new Thread(new ThreadStart(() =>
                            {
                                try
                                {
                                    Bitmap image = new Bitmap(Width, Height);
                                    var g = Graphics.FromImage(image);
                                    g.Clear(BackColor);
                                    FlappyBird.PutOn(g, Width, Height);
                                    BeginInvoke(new Action(() =>
                                    {

                                    }));
                                    var cg = CreateGraphics();
                                    cg.InterpolationMode = InterpolationMode.NearestNeighbor;
                                    cg.DrawImage(image, 0, 0, Width, Height);
                                    cg.Dispose();
                                    g.Dispose();
                                    image.Dispose();
                                }
                                catch (Exception e)
                                {
                                    RenderError(e);
                                }
                                render.Abort();
                            }));
                            render.Start();
                        }
                        catch (Exception e)
                        {
                            RenderError(e);
                        }
                    else
                    {
                        NotActive();
                    }
                else
                {
                    NotActive();
                }
            else
            {
                NotActive();
            }
        }

        protected override bool IsInputKey(Keys keyData)
        {
            return true;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            FlappyBird.HandleKey(e);
        }

        private void RenderError(Exception e)
        {
            var g = CreateGraphics();
            var b = new SolidBrush(ForeColor);
            g.Clear(BackColor);
            g.DrawString(e.ToString(), Font, b, 0, 0);
            g.Dispose();
            b.Dispose();
        }

        private void NotActive()
        {
            var g = CreateGraphics();
            var b = new SolidBrush(ForeColor);
            g.Clear(BackColor);
            g.DrawString(Text + " not running", Font, b, 0, 0);
            g.Dispose();
            b.Dispose();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            RunANewRender();
        }
    }
}
