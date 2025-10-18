using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tower_Defense;


namespace TowerDefense
{
    public class Torre : EntidadDeJuego
    {
        private ObjectPool<Proyectil> poolProyectiles;
        private bool activo = true;
        public int Jugador { get; set; } = 1;
        private int vida = 3;

        public void AsignarPool(ObjectPool<Proyectil> pool)
        {
            poolProyectiles = pool;
        }

        public void Activar(float x, float y)
        {
            X = x;
            Y = y;
            vida = 3;
            activo = true;
        }

        public void RecibirDaño()
        {
            vida--;
            if (vida <= 0)
            {
                activo = false;
                GameController.Instancia.DevolverTorre(this);
            }
        }

        public void DispararProyectil()
        {
            if (!activo) return;
            Proyectil p = poolProyectiles.ObtenerObjeto();
            if (p == null) return; // No hay proyectiles disponibles

            p.Activar(X + (Jugador == 1 ? 30 : -10), Y + 10, Jugador);
            GameController.Instancia.RegistrarProyectil(p);
        }

        public override void Actualizar() { }

        public override void Dibujar(Graphics g)
        {
            if (!activo) return;
            Brush color = Jugador == 1 ? Brushes.Blue : Brushes.DarkRed;
            g.FillRectangle(color, X, Y, 30, 30);
            g.DrawRectangle(Pens.Black, X, Y, 30, 30);
            g.DrawString(vida.ToString(), new Font("Arial", 9, FontStyle.Bold),
                Brushes.White, X + 8, Y + 8);
        }
    }
}