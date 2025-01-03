using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Vibora
{
    public partial class Form1 : Form
    {
        private List<Point> vibora = new List<Point>(); //Lista de puntos para crear la vibora
        private Timer temporizador; //Temporizador para iniciar el juego
        private int direc = 0; //La utilizo para saber que tecla direccional presiono
        private const int tam_seg = 30; //Me permite definir el tamaño de los segmentos
        private int contador = 0; //La utilizo para mostrar el puntaje

        /*******************/
        //Parallel la comida
        Random aleatorio = new Random(); //Esta variable me permite crear las coordenadas aleatoriamente
        Point comida; //La utilizo cada vez que se genera la comida

        public Form1()
        {
            InitializeComponent();
            lblContador.BackColor = Color.Transparent;
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();

            this.KeyDown += Form1_KeyDown;
            vibora.Clear();
            vibora.Add(new Point(5, 5));
            vibora.Add(new Point(4, 5));
            vibora.Add(new Point(3, 5));
            generarComida();
            comenzar();
        }

        /********************************************************************************************/
        //Este metodo permite dibujar la vibora y la comida
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            for (int i = 0; i < vibora.Count;i++ ) //Recorro los segmentos de la vibora
            {
                var segmento = vibora[i]; //Obtengo cada segmento de la vibora
                g.FillEllipse(Brushes.Green, segmento.X * tam_seg, segmento.Y * tam_seg, tam_seg, tam_seg); //Dibujo cada segmento de la vibora

            }

            if(comida != Point.Empty)
            {
                g.FillEllipse(Brushes.Red, comida.X * tam_seg, comida.Y * tam_seg, tam_seg, tam_seg); //Dibujo la comida
            }
        }

        /********************************************************************************************/
        private void comenzar()
        {
            temporizador = new Timer //Creo un temporizador para controlar el juego en tiempo real
            {
                Interval = 200 //Declaro mi intervalo con 200 milisegundos
            };
            //temporizador.Tick += (s, e) => actualizarJuego();
            temporizador.Tick += Temporizador_Tick; //Llamo al metodo temporizador_Tick (Lo realizara cada 200 milisegundos)

            temporizador.Start(); //Inicio el temporizador

        }

        /*********************************************************************************************/

        private void Temporizador_Tick(object ob, EventArgs e)
        {
            actualizarJuego(); //Actualiza el juego cada 200 milisegundos
        }

        /*******************************************************************************************/
        private void actualizarJuego()
        {
            moverVivora(); //Llama al método que permite mover la vibora

            if(vibora[0] == comida) //Valido si la cabeza de la vibora tiene la misma posicion de la comida
            {
                vibora.Add(new Point(-1, -1)); // Si eso es verdadero, se incrementa la vibora
                contador += 20; //Utilizo esta variable para el contador en la pantalla principal
                lblContador.Text = ""+contador+" puntos"; //Muestro el puntaje en la etiqueta Label
                generarComida(); //Genero nuevamente la comida
            }

            this.Invalidate(); //Este metodo le indica al SO que hay una area que necesita ser repintada y llama automaticamente a OnPaint
        }

        /*******************************************************************************************/
        private void moverVivora()
        {
            for(int i = vibora.Count - 1; i > 0; i--) //Recorro todos los segmentos de la vibora
            {
                vibora[i] = vibora[i - 1]; //A cada elemento lo desplazo una posición
            }

            Point cabeza = vibora[0]; //Obtengo la cabeza de la vibora, es el primer elemento

            switch (direc)
            {
                case 0: cabeza.X += 1; break; //Se mueve hacia la derecha
                case 1: cabeza.Y += 1; break; //Se mueve hacia abajo
                case 2: cabeza.X -= 1; break; //Se mueve hacia la izquierda
                case 3: cabeza.Y -= 1; break; //Se mueve hacia arriba
            }


            /*********************************************************************************************/
            //Verificar limites
            int maxX = this.ClientSize.Width / tam_seg;
            int maxY = this.ClientSize.Height / tam_seg;

            if (cabeza.X < 0 || cabeza.Y < 1.5 || cabeza.X >= maxX || cabeza.Y >= (maxY-1)) //Validamos que no choque con los limites de la ventana
            {
                temporizador.Stop(); //Si choca con las paredes de la ventana, paramos el temporizador
                continuarSiONo("La vibora se salió del área de juego, ¿Quieres juegar de nuevo?"); //Preguntamos si quiere jugar nuevamente
            }

            //if (cabeza.X < 0) cabeza.X = maxX - 1;


            //Validar si la vibora choca consigo misma
            for (int i = 0; i < vibora.Count; i++ ) //Este for nos permite validar si choca consigo misma, es decir si la posicion de la cabeza es igual a alguno de sus nodos
            {
                if(vibora[i]==cabeza) //validamos si la cabeza es igual a uno de sus nodos
                {
                    temporizador.Stop(); //Paramos el temporizador
                    continuarSiONo("Te chocaste, ¿Quieres juegar de nuevo?"); //Preguntamos si desea jugar nuevamente
                }

            }
                vibora[0] = cabeza; //asignamos la nueva posicion a la cabeza de la vibora
        }

        /*********************************************************************************************/
        //Me permite los desplazamientos en funcion de las teclas direccionales
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Right: 
                    if( direc != 2)  direc = 0; break; // Se mueve hacia la derecha y evitar ir hacia atrás
                case Keys.Down:
                    if (direc != 3) direc = 1; break; // Se mueve hacia abajo y evitar ir hacia atrás
                case Keys.Left:
                    if (direc != 0) direc = 2; break; // Se mueve hacia la izquierda y evitar ir hacia atrás
                case Keys.Up:
                    if (direc != 1) direc = 3; break; // Se mueve hacia arriba y evitar ir hacia atrás
            }
        }

        /***********************************************************************************************/
        //Esta función me permite generar la comida en una ubicacion aleatoria
        private void generarComida()
        {
            int maxX = this.ClientSize.Width / tam_seg; //Obtengo el maximo de ubicaciones en "X", es decir 1185 / 30
            int maxY = this.ClientSize.Height / tam_seg; //Obtengo el maximo de ubicaciones en "Y" es decir 800 / 30
            Point nuevaComida; //Declaro la nueva comida como un objeto Poit, recordar que la vibora es una lista de Point

            do
            {
                int x = aleatorio.Next(0, maxX); //Obtengo la posición en "X" aleatoriamente
                int y = aleatorio.Next(2, maxY - 1); //Obtengo la posición en "Y" aleatoriamente
                nuevaComida = new Point(x, y); //A la nueva comida le asigno el nuevo punto
            } while (vibora.Contains(nuevaComida)); //comprueba que la posicion de la nueva comida no este ocupada por un segmento de la vibora

            comida = nuevaComida; //A la variable global comida, le asigno la nueva comida
        }

        /***********************************************************************************************/
        private void comenzarDeNuevo()
        {
            vibora.Clear(); //Borramos todos los nodos de la vibora
            vibora.Add(new Point(8, 7)); //colocamos los nodos, este primer nodo es la cabeza
            vibora.Add(new Point(7, 7)); //colocamos los nodos
            vibora.Add(new Point(6, 7)); //colocamos los nodos

            if(direc == 0) direc = 2; //si se choco por el lado derecho, va a aparecer por el lado izquierdo
            else if (direc == 2) direc = 0; //si se choco por el lado izquierdo, va a aparecer por el lado derecho
            else if (direc == 1) direc = 3; //si se choco por el lado inferior, va a aparecer por el lado superior
            else direc = 1; //si se choco por el lado superior, va a aparecer por el lado inferior

            lblContador.Text = ""; //inicializamos el contador de la puntuación

            generarComida(); //generamos la primera comida aleatoriamente

            comenzar(); //Esta función permite inicializar el temporizador
        }


        /**********************************************************************************************/
        //Si un jugador pierde, este metodo le permite iniciar de nuevo
        private void continuarSiONo(String mensaje)
        {
            DialogResult resultado = MessageBox.Show(mensaje, "Confirmacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question); //este metodo me permite mostrar un mensaje de confirmación

            if (resultado == DialogResult.Yes) //Si el usuario presiona "YES" el juego se reinicia
            {
                comenzarDeNuevo();
            }
            else
            {
                Application.Exit(); //Si el usuario presiona "NO" el juego termina
            }
        }
    }
}
