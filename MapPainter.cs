using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrokerageGather
{
    public class MapPainter
    {
        public class MapPainterEventArgs : EventArgs
        {
            public Bitmap Map { get; set; }
        }

        public decimal Left { get; set; }
        public decimal Top { get; set; }
        public decimal Bottom { get; set; }
        public decimal Right { get; set; }
        public Bitmap Map { get; set; }
        public int MapHeight { get; set; }
        public int MapWidth { get; set; }
        public decimal GPSWidth { get; set; }
        public decimal GPSHeight { get; set; }
        public float PointSize { get; set; }
        public int BorderWidth { get; set; }
        public event EventHandler<MapPainterEventArgs> PaintEvent;
        public MapPainter(decimal left, decimal top, decimal bottom, decimal right)
        {
            MapHeight = 1000;
            MapWidth = 1000;
            PointSize = 6;
            BorderWidth = 10;

            GPSWidth = right - left;
            GPSHeight = top - bottom;

            Left = left;
            Right = right;
            Top = top;
            Bottom = bottom;

            Map = new Bitmap(MapWidth, MapHeight);
        }

        public MapPosition GetMapPosition(decimal lat, decimal lng)
        {
            MapPosition pos = new MapPosition();
            decimal left = 0;
            decimal top = 0;

            left = lng - Left;
            top = Top - lat;

            pos.Left = (int)Math.Round(left / GPSWidth * (MapWidth - 2 * BorderWidth) + BorderWidth, 0);
            pos.Top = (int)Math.Round(top / GPSHeight * (MapHeight - 2 * BorderWidth) + BorderWidth, 0);

            return pos;
        }

        public void Draw(decimal lat, decimal lng, Color color)
        {
            Graphics g = Graphics.FromImage(Map);
            var position = GetMapPosition(lat, lng);
            Pen pen = new Pen(color, PointSize);
            //g.DrawRectangle(pen, new Rectangle(
            //    new Point(position.Left - (int)PointSize / 2, position.Top - (int)PointSize / 2),
            //    new Size((int)PointSize, (int)PointSize))
            //    );
            g.DrawEllipse(pen, new Rectangle(
                new Point(position.Left - (int)PointSize / 2, position.Top - (int)PointSize / 2),
                new Size((int)PointSize, (int)PointSize))
                );
            if (PaintEvent != null)
                PaintEvent.Invoke(this, new MapPainterEventArgs { Map = Map.Clone() as Bitmap });
        }

        public class MapPosition
        {
            public int Left { get; set; }
            public int Top { get; set; }
        }
    }
}
