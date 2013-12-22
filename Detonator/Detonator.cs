using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MarvinsArena.Robot;

namespace Detonator
{
    public class Detonator : EnhancedRobot, IRobot
    {

        #region Variaveis

        #region | Colisao |

        private bool blnColidindo;
        private bool blnVoltando;
        private bool blnVirando;
        private int intDistanciaSegura = 10;

        #endregion

        #region | Posicao |

        private bool blnPosicaoIdeal;
        private bool blnVirandoPosicaoIdeal;

        #endregion

        #region | Robos |

        private bool blnRoboEncontrado;
        private bool blnEncontrandoAngulo;

        #endregion

        #endregion

        #region Initialize

        public void Initialize()
        {




            blnColidindo = false;
            blnVirando = false;
            blnVoltando = false;

            intDistanciaSegura += Radius;

            HitBullet += new EventHandler<EventArgs>(Detonator_HitBullet);
            HitMissile += new EventHandler<EventArgs>(Detonator_HitMissile);
            HitRobot += new EventHandler<EventArgs>(Detonator_HitRobot);
            HitWall += new EventHandler<EventArgs>(Detonator_HitWall);
            ScannedRobot += new EventHandler<ScannedRobotEventArgs>(Detonator_ScannedRobot);

        }



        #endregion

        #region Run

        public void Run()
        {

            RotateRadarLeftDeg(1);

            //Print("rotacao atual: " + RotationDeg.ToString());

            if (!blnColidindo)
            {
                if (
                    PositionX < intDistanciaSegura
                    || PositionX > MapWidth - intDistanciaSegura
                    || PositionY < intDistanciaSegura
                    || PositionY > MapHeight - intDistanciaSegura
                    )
                {
                    Colidindo();
                }
            }






            //RotateToAngle(35);

            //EvitarColisao();





        }

        #endregion

        #region Metodos


        /// <summary>
        /// Para fins de cálculo e visualização geométrica, vamos representar o plano cartesiano pelos números do RELÓGIO. Temos quatro quadrantes.
        /// PRIMEIRO QUADRANTE = entre os números 12 e 3
        /// SEGUNDO QUADRANTE = entre os números 3 e 6
        /// TERCEIRO QUADRANTE = entre os números 6 e 9
        /// QUARTO QUADRANTE = entre os números 9 e 12
        /// </summary>
        /// <param name="vetOrigem"></param>
        /// <param name="vetDestino"></param>
        /// <returns></returns>
        private double FindAngle(Vetor2D vetOrigem, Vetor2D vetDestino)
        {


            if (!blnEncontrandoAngulo)
            {
                double dblFinalAngle = 0.0;

                blnEncontrandoAngulo = true;

                //Cria um vetor auxiliar para traçar um triângulo, possibilitando o uso de funções trigonométricas
                Vetor2D vetAuxiliar = new Vetor2D { X = vetOrigem.X, Y = vetDestino.Y };

                //Recupera um dos catetos (neste caso, será o cateto oposto, para calcular a tangente)
                double dblCateto1 = vetDestino.X - vetAuxiliar.X;

                //Recupera o cateto adjacente
                double dblCateto2 = vetAuxiliar.Y - vetOrigem.Y;

                //Recupera a variação dos catetos para calcular a hipotenusa
                double dblVariacaoCateto1 = Math.Pow(dblCateto1, 2);
                double dblVariacaoCateto2 = Math.Pow(dblCateto2, 2);

                //Cria o valor da hipotenusa (não será utilizado aqui, já que vamos trabalhar com tangente)
                double dblHipotenusa = Math.Sqrt(dblVariacaoCateto1 + dblVariacaoCateto2);

                //Calcula a tangente, dividindo o cateto oposto (cateto 1) pelo adjacente (cateto 2)
                double dblTangente = dblCateto1 / dblCateto2;

                //Recupera o ângulo em radianos
                double dblRadianos = Math.Atan(dblTangente);

                //Converte o ângulo em radianos para graus
                double dblGraus = dblRadianos * (180.0 / Math.PI);

                //Print("angulo encontrado inicial: " + dblGraus.ToString());

                //Quando o cálculo culminar nas extremidades (quando a tangente é zero)
                if (dblTangente == 0)
                {
                    //Extremidade de cima
                    if (dblCateto2 < 0)
                    {

                        dblGraus = -90;
                    }
                    //Extremidade de baixo
                    else if (dblCateto1 >= 0 && dblCateto2 >= 0)
                    {
                        dblGraus = 90;
                    }
                }

                //Se a tangente for menor que zero, o ponto destino se encontra no PRIMEIRO ou TERCEIRO quadrante
                if (dblTangente < 0)
                {

                    //TERCEIRO QUADRANTE
                    if (dblCateto1 < 0)
                    {

                        dblGraus = 90 - dblGraus;

                    }
                    //PRIMEIRO QUADRANTE
                    else
                    {
                        dblGraus += 90;
                        dblGraus *= -1;
                    }
                }

                    //SEGUNDO ou QUARTO quadrantes
                else if (dblTangente > 0)
                {
                    //QUARTO QUADRANTE
                    if (dblCateto1 < 0 && dblCateto2 < 0)
                    {
                        dblGraus = 90 + dblGraus;
                        dblGraus *= -1;
                    }
                        //SEGUNDO QUADRANTE
                    else
                        dblGraus = 90 - dblGraus;
                }

                dblFinalAngle = dblGraus;


                //Print(string.Format("Cateto 1 (oposto):  {0}", dblCateto1));
                //Print(string.Format("Cateto 2 (adjacente):  {0}", dblCateto2));
                //Print(string.Format("tangente :  {0}", dblTangente));
                //Print(string.Format("angulo (radianos):  {0}", dblRadianos));
                //Print(string.Format("angulo (graus):  {0}", dblGraus));

                return dblFinalAngle;

            }

            return 0.0;

        }

        private void RotateToAngle(double dblAngle)
        {
            RotateToAngle(Options.CAR, dblAngle);
        }

        /// <summary>
        /// Rotaciona o carro para o angulo passado por parametro, considerando a posição atual.
        /// <remarks>Este método é diferente do rotate, que rotaciona uma quantidade
        /// definida de graus, não considerando a posição atual</remarks>
        /// </summary>
        /// <param name="dblAngle"></param>
        private void RotateToAngle(Options option, double dblAngle)
        {

            if (!blnPosicaoIdeal)
            {
                //Define o novo angulo considerando a posicao atual
                double dblNovoAngulo = dblAngle - RotationDeg;

                //Inicia a rotação, se já não está fazendo.
                if (!blnVirandoPosicaoIdeal)
                {
                    blnVirandoPosicaoIdeal = true;

                    switch (option)
                    {

                        case Options.GUN:
                            RotateGunRightDeg(dblNovoAngulo);
                            break;

                        case Options.RADAR:
                            RotateRadarRightDeg(dblNovoAngulo);
                            break;

                        default:
                        case Options.CAR:
                            RotateRightDeg(dblNovoAngulo);
                            break;
                    }

                }

                //Se terminuo a rotação, reinicializa as variaveis
                if (RemainingRotation == 0)
                {
                    blnPosicaoIdeal = true;
                    blnVirandoPosicaoIdeal = false;
                }
            }
        }

        private void Colidindo()
        {
            blnColidindo = true;
            blnVirando = false;
            blnVoltando = false;
        }

        private void EvitarColisao()
        {
            if (blnColidindo)
            {

                if (!blnVoltando)
                {
                    blnVoltando = true;
                    MoveBackward(10);


                }

                if (RemainingDistance == 0)
                {

                    if (!blnVirando)
                    {
                        blnVirando = true;
                        RotateLeftDeg(20);
                    }
                }

                if (RemainingRotation == 0 && blnVirando)
                {
                    blnColidindo = false;
                    blnVirando = false;
                    blnVoltando = false;
                }

                //if (RemainingDistance == 0)
                //{

                //    blnVoltando = false;

                //    if (!blnVirando)
                //    {
                //        blnVirando = true;
                //        RotateLeftDeg(20);
                //    }

                //    if (RemainingRotation == 0)
                //    {
                //        blnColidindo = false;

                //    }
                //}
            }
            else
                MoveForward(10);
        }

        #endregion

        #region Eventos


        void Detonator_HitBullet(object sender, EventArgs e)
        {

        }
        void Detonator_HitWall(object sender, EventArgs e)
        {
            //Colidindo();
            //StopMove();
        }

        void Detonator_HitRobot(object sender, EventArgs e)
        {
            //Colidindo();





        }

        void Detonator_HitMissile(object sender, EventArgs e)
        {

        }

        void Detonator_ScannedRobot(object sender, ScannedRobotEventArgs e)
        {

            //if (PositionX < intDistanciaSegura
            //    || PositionX > e.PositionX + 10
            //    || PositionY < intDistanciaSegura
            //    || PositionY > e.PositionY + 10
            //    )
            //{
            //    Colidindo();
            //}


            //StopRotateRadar();


            if (!blnRoboEncontrado)
            {
                blnRoboEncontrado = true;


                Vetor2D vetThis = new Vetor2D { X = PositionX, Y = PositionY };
                Vetor2D vetScannedRobot = new Vetor2D { X = e.PositionX, Y = e.PositionY };

                double dblPosicaoRobo = FindAngle(vetThis, vetScannedRobot);

                RotateToAngle(Options.GUN, dblPosicaoRobo);

            }

            if (RemainingRotationGun == 0)
            {
                FireBullet(2);
            }






        }

        #endregion

        #region Opcoes

        private enum Options
        {
            GUN,
            CAR,
            RADAR
        }

        #endregion



    }

    #region Triangulo

    public struct Vetor2D
    {
        private double dblX;
        private double dblY;

        public double X
        {
            get
            {
                return dblX;
            }
            set
            {
                dblX = value;

            }
        }

        public double Y
        {
            get
            {
                return dblY;
            }
            set
            {
                dblY = value;

            }
        }
    }
    public struct Triangulo
    {
        private Vetor2D A;
        private Vetor2D B;
        private Vetor2D C;


        public double Cateto1
        {
            get
            {
                return B.X - A.X;
            }
        }




    }

    #endregion

}
