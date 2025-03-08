using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollisionDetectionV3
{
    public class enemy
    {
        public Rectangle currentPos;
        public PointF velocity;

        public Cell currentCell;
        public Cell TargetCell;

        private int height = 15;
        private int width = 15;
        public float enemySpeed = 1.5f;

        public enemy(int x, int y)
        {
            currentPos = new Rectangle(x, y, width, height);
            velocity = new PointF(0,0);
            currentCell = null;
        }
        

        public void Draw(Graphics g)
        {
            Brush brush = new SolidBrush(Color.FromArgb(186, 148, 250));
            g.FillRectangle(brush, currentPos);
        }
    }
}
