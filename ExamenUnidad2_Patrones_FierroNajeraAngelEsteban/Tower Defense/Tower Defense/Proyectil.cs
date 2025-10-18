using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tower_Defense;


namespace TowerDefense
{
    public class Proyectil : EntidadDeJuego
    {
        private float velocidad;
        private bool activo;
        private int jugadorOrigen;

        public void Activar(float x, float y, int jugador)
        {
            X = x;
            Y = y;
            velocidad = jugador == 1 ? 12 : -12;
            jugadorOrigen = jugador;
            activo = true;
        }

        public override void Actualizar()
        {
            if (!activo) return;
            X += velocidad;

            foreach (var torre in GameController.Instancia.ObtenerTorresEnemigas(jugadorOrigen))
            {
                if (torre == null) continue;
                if (X + 10 > torre.X && X < torre.X + 30 && Y + 10 > torre.Y && Y < torre.Y + 30)
                {
                    torre.RecibirDaño();
                    activo = false;
                    GameController.Instancia.DevolverProyectil(this);
                    return;
                }
            }

            if (X < 0 || X > 900)
            {
                activo = false;
                GameController.Instancia.DevolverProyectil(this);
            }
        }

        public override void Dibujar(Graphics g)
        {
            if (activo)
                g.FillEllipse(Brushes.Gold, X, Y, 10, 10);
        }
    }
}
