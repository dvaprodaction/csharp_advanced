using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsteroidGame.VisualObjects
{
    class Star : VisualObject
    {
        public Star(Point Position, Point Direction, int Size)
            :base(Position, Direction, new Size(Size, Size))
        {
            
        }

        public override void Draw(Graphics g)
        {

            int _render = 1;

            g.DrawLine(Pens.Gray, _Position.X + _render, _Position.Y + _render, _Position.X + _Size.Width - _render, _Position.Y + _Size.Height - _render);
            g.DrawLine(Pens.Gray, _Position.X + _Size.Width / 2, _Position.Y, _Position.X + _Size.Width / 2, _Position.Y + _Size.Height);
            g.DrawLine(Pens.Gray, _Position.X + _Size.Width - _render, _Position.Y + _render, _Position.X + _render, _Position.Y + _Size.Height - _render);
            g.DrawLine(Pens.Gray, _Position.X + _Size.Width, _Position.Y + _Size.Height / 2, _Position.X, _Position.Y + _Size.Height / 2);

            //g.DrawLine(Pens.Gray,
            //    _Position.X + _Size.Width, _Position.Y,
            //    _Position.X, _Position.Y + _Size.Height);
        }

        public override void Update()
        {
            _Position.X -= _Direction.X;
            if (_Position.X < 0)
                _Position.X = Game.Width + _Size.Width;
            
        }

    }
}
