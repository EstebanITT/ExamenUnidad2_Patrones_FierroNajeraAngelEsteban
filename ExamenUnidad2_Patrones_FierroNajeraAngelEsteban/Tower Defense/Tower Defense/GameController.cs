using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TowerDefense
{
    public class GameController
    {
        private static GameController instancia;
        public static GameController Instancia
        {
            get
            {
                if (instancia == null)
                    instancia = new GameController();
                return instancia;
            }
        }

        private readonly ObjectPool<Torre> poolTorres;
        private readonly ObjectPool<Proyectil> poolProyectiles;
        private readonly List<EntidadDeJuego> entidades = new List<EntidadDeJuego>();
        private readonly List<Torre> torresJugador1 = new List<Torre>();
        private readonly List<Torre> torresJugador2 = new List<Torre>();

        private const int MAX_TORRES_POR_JUGADOR = 5;
        private const int DISTANCIA_MINIMA = 40;

        public enum JugadorTurno { Jugador1, Jugador2 }
        public JugadorTurno TurnoActual { get; private set; }

        public bool JuegoTerminado { get; private set; }
        public string Ganador { get; private set; }

        private GameController()
        {
            TurnoActual = JugadorTurno.Jugador1;
            JuegoTerminado = false;

            // Precrear torres
            var torresIniciales = new List<Torre>();
            for (int i = 0; i < 10; i++) torresIniciales.Add(new Torre());
            poolTorres = new ObjectPool<Torre>(torresIniciales);

            // Precrear proyectiles
            var proyectilesIniciales = new List<Proyectil>();
            for (int i = 0; i < 20; i++) proyectilesIniciales.Add(new Proyectil());
            poolProyectiles = new ObjectPool<Proyectil>(proyectilesIniciales);
        }

        public void IntentarColocarTorre(Point click)
        {
            if (JuegoTerminado) return;

            if (TurnoActual == JugadorTurno.Jugador1)
                ColocarTorreJugador(click, 1, new Rectangle(0, 100, 400, 450), torresJugador1);
            else
                ColocarTorreJugador(click, 2, new Rectangle(500, 100, 400, 450), torresJugador2);
        }

        private void ColocarTorreJugador(Point click, int jugador, Rectangle zona, List<Torre> lista)
        {
            if (!zona.Contains(click) || lista.Count >= MAX_TORRES_POR_JUGADOR)
                return;

            if (lista.Any(torreExistente =>
                Distancia(torreExistente.X, torreExistente.Y, click.X, click.Y) < DISTANCIA_MINIMA))
                return;

            Torre torreNueva = poolTorres.ObtenerObjeto();
            if (torreNueva == null) return; // No hay torres disponibles

            torreNueva.AsignarPool(poolProyectiles);
            torreNueva.Activar(click.X - 15, click.Y - 15);
            torreNueva.Jugador = jugador;

            lista.Add(torreNueva);
            entidades.Add(torreNueva);
        }

        private double Distancia(float x1, float y1, float x2, float y2)
        {
            return Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));
        }

        public void RegistrarProyectil(Proyectil p) => entidades.Add(p);
        public void DevolverProyectil(Proyectil p) => poolProyectiles.DevolverObjeto(p);

        public void DevolverTorre(Torre torre)
        {
            entidades.Remove(torre);
            if (torre.Jugador == 1) torresJugador1.Remove(torre);
            else torresJugador2.Remove(torre);
            poolTorres.DevolverObjeto(torre);

            RevisarVictoria();
        }

        public List<Torre> ObtenerTorresEnemigas(int jugador) =>
            jugador == 1 ? torresJugador2 : torresJugador1;

        public void ActualizarEntidades()
        {
            if (JuegoTerminado) return;
            foreach (var entidad in entidades.ToArray())
                entidad.Actualizar();
        }

        public void DibujarEntidades(Graphics g)
        {
            foreach (var entidad in entidades)
                entidad.Dibujar(g);
        }

        public void SiguienteTurno()
        {
            if (JuegoTerminado) return;

            TurnoActual = TurnoActual == JugadorTurno.Jugador1 ?
                JugadorTurno.Jugador2 : JugadorTurno.Jugador1;

            var torres = TurnoActual == JugadorTurno.Jugador1 ? torresJugador1 : torresJugador2;
            foreach (var torre in torres)
                torre.DispararProyectil();
        }

        private void RevisarVictoria()
        {
            if (torresJugador1.Count == 0 && torresJugador2.Count == 0)
            {
                JuegoTerminado = true;
                Ganador = "Empate";
            }
            else if (torresJugador1.Count == 0)
            {
                JuegoTerminado = true;
                Ganador = "Jugador 2";
            }
            else if (torresJugador2.Count == 0)
            {
                JuegoTerminado = true;
                Ganador = "Jugador 1";
            }
        }

        public void Reiniciar()
        {
            // Devolver torres activas al pool
            foreach (var torre in torresJugador1.ToArray())
                DevolverTorre(torre);
            foreach (var torre in torresJugador2.ToArray())
                DevolverTorre(torre);

            // Devolver proyectiles activos al pool
            var proyectilesActivos = entidades.OfType<Proyectil>().ToArray();
            foreach (var p in proyectilesActivos)
                DevolverProyectil(p);

            entidades.Clear();
            torresJugador1.Clear();
            torresJugador2.Clear();
            JuegoTerminado = false;
            Ganador = "";
            TurnoActual = JugadorTurno.Jugador1;
        }
    }
}