using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Drawing
{
    public struct Size
    {
        public int Width, Height;
        public Size(int X, int Y)
        {
            this.Width = X;
            this.Height = Y;
        }
    }
    public struct Rectangle
    {
        public int X, Y, Width, Height;
        public Rectangle(int x,int y,int w,int h)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }
    }
    public struct Point
    {
        public int X, Y;
        public Point(int X,int Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }
    public struct PointF
    {
        public float X, Y;
        public PointF(float X, float Y)
        {
            this.X = X;
            this.Y = Y;
        }
    }
}
