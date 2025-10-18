using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefense
{
    public interface IActualizable
    {
        void Actualizar();
        void Dibujar(Graphics g);
    }
}
