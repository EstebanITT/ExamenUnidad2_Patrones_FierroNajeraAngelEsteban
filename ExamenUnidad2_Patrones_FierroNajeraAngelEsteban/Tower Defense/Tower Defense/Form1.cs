using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TowerDefense
{
    public partial class Form1 : Form
    {
        private readonly Timer timer;
        private Button btnTurno;
        private Button btnReiniciar;
        private bool turnoBloqueado = false;

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.Text = "Tower Defense - 2 Jugadores";
            this.Width = 900;
            this.Height = 650;
            this.BackColor = Color.FromArgb(245, 245, 245);

            timer = new Timer();
            timer.Interval = 60;
            timer.Tick += (s, e) =>
            {
                GameController.Instancia.ActualizarEntidades();
                Invalidate();
            };
            timer.Start();

            // Botón pasar turno
            btnTurno = new Button
            {
                Text = "➡️ Pasar Turno",
                Location = new Point(this.Width / 2 - 160, 10),
                Size = new Size(120, 40),
                BackColor = Color.LightSkyBlue,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            btnTurno.Click += BtnTurno_Click;
            this.Controls.Add(btnTurno);

            // Botón reiniciar
            btnReiniciar = new Button
            {
                Text = "🔁 Reiniciar",
                Location = new Point(this.Width / 2 - 2, 10),
                Size = new Size(120, 40),
                BackColor = Color.LightGreen,
                Font = new Font("Arial", 10, FontStyle.Bold)
            };
            btnReiniciar.Click += (s, e) =>
            {
                GameController.Instancia.Reiniciar();
                turnoBloqueado = false;
                Invalidate();
            };
            this.Controls.Add(btnReiniciar);

            this.MouseClick += (s, e) =>
            {
                GameController.Instancia.IntentarColocarTorre(e.Location);
                Invalidate();
            };
        }

        private void BtnTurno_Click(object sender, EventArgs e)
        {
            if (turnoBloqueado) return;
            GameController.Instancia.SiguienteTurno();
            turnoBloqueado = true; // Bloquea para evitar múltiples clics rápidos
            Timer desbloqueo = new Timer { Interval = 100 }; // 0.1s de retardo
            desbloqueo.Tick += (s, ev) =>
            {
                turnoBloqueado = false;
                desbloqueo.Stop();
                desbloqueo.Dispose();
            };
            desbloqueo.Start();
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Dibujar zonas de los jugadores
            g.FillRectangle(new SolidBrush(Color.FromArgb(230, 240, 255)), 0, 100, 400, 450);
            g.FillRectangle(new SolidBrush(Color.FromArgb(255, 240, 230)), 500, 100, 400, 450);
            g.DrawString("ZONA JUGADOR 1", new Font("Arial", 12, FontStyle.Bold), Brushes.Blue, 120, 80);
            g.DrawString("ZONA JUGADOR 2", new Font("Arial", 12, FontStyle.Bold), Brushes.DarkRed, 640, 80);

            // Mostrar turno o ganador
            if (GameController.Instancia.JuegoTerminado)
            {
                g.DrawString("Ganador: " + GameController.Instancia.Ganador,
                    new Font("Arial", 18, FontStyle.Bold),
                    Brushes.DarkGreen,
                    340, 60);
            }
            else
            {
                string textoTurno = "Turno actual: " +
                    (GameController.Instancia.TurnoActual == GameController.JugadorTurno.Jugador1 ?
                    "Jugador 1" : "Jugador 2");
                g.DrawString(textoTurno, new Font("Arial", 14, FontStyle.Bold), Brushes.Black, 340, 60);
            }

            // Dibujar entidades
            GameController.Instancia.DibujarEntidades(g);
        }
    }
}