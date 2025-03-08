using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CollisionDetectionV3
{
    public enum PowerUpType { speedBoost, Ghost, Freeze }
    public class items
    {
       
        public PowerUpType type;
        public Rectangle ItemRect;
        private int height = 15;
        private int width = 15;
        private int duration;
        public items(PowerUpType _type,int x, int y)
        {
            type = _type;
            ItemRect = new Rectangle(x, y, height, width);
            duration = 3;
        }

        public void drawItems(Graphics g)
        {
          
            Color color = Color.Black;

            switch(type)
            {
                case PowerUpType.speedBoost:
                    color = Color.Red;
                     break;

                case PowerUpType.Ghost:
                    color = Color.DarkSeaGreen;
                    break;

                case PowerUpType.Freeze:
                    color = Color.RoyalBlue;
                    break;


            }
            Point[] lightningBolt = new Point[]
            {
                 new Point(ItemRect.X + ItemRect.Width / 2, ItemRect.Y),               // Top point
                 new Point(ItemRect.X + ItemRect.Width * 3 / 4, ItemRect.Y + ItemRect.Height / 2), // Right middle point
                 new Point(ItemRect.X + ItemRect.Width / 2, ItemRect.Y + ItemRect.Height / 2),     // Center point
                 new Point(ItemRect.X + ItemRect.Width * 3 / 4, ItemRect.Y + ItemRect.Height),     // Bottom right
                 new Point(ItemRect.X + ItemRect.Width / 4, ItemRect.Y + ItemRect.Height / 2),     // Bottom left
                 new Point(ItemRect.X + ItemRect.Width / 2, ItemRect.Y + ItemRect.Height / 2),     // Back to center
            };
            Pen pen = new Pen(Color.Black);
            g.FillPolygon(new SolidBrush(color), lightningBolt);
            g.DrawPolygon(pen , lightningBolt);
            
        }

       

    }

}
