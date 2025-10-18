using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TowerDefense
{
    public abstract class EntidadDeJuego : IActualizable
    {
        public float X { get; set; }
        public float Y { get; set; }

        public abstract void Actualizar();
        public abstract void Dibujar(Graphics g);
    }
}