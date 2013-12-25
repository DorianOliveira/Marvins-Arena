using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MarvinsArena.Robot;


namespace Detonator
{
    public class Detonator : EnhancedRobot, IRobot
    {

        #region | Variaveis |

        #region - Colisao -

        private bool blnColidindo;
        private bool blnVoltando;
        private bool blnVirando;
        private int intDistanciaSegura = 10;

        #endregion

        #region - Posicao -

        private bool blnPosicaoIdeal;
        private bool blnVirandoPosicaoIdeal;

        #endregion

        #region - Robos -

        private bool blnRoboEncontrado;
        private bool blnEncontrandoAngulo;
        private bool blnIndoPara;
        private bool blnRotatingGun;
        private bool blnRotatedGun;
        private bool blnRotated;


        private double dblEnemyX;
        private double dblEnemyY;

        private double dblPosicaoRobo;

        #endregion

        #endregion

        #region | Initialize |

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

        #region | Run |

        public void Run()
        {

            //Mantém o radar girando
            RotateRadarLeftDeg(1);

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

            EvitarColisao();

            //Se encontrou um robo (no evento de scanner)
            if (blnRoboEncontrado)
            {

                //Crias posicoes
                Trigonometry.Vetor2D vetThis = new Trigonometry.Vetor2D { X = PositionX, Y = PositionY };
                Trigonometry.Vetor2D vetScannedRobot = new Trigonometry.Vetor2D { X = dblEnemyX, Y = dblEnemyY };
                

                //Se nao esta procurando angulo
                if (!blnEncontrandoAngulo)
                {
                    
                    //Encontra o angulo a ser rotacionado para se voltar para o robo escaneado
                    dblPosicaoRobo = Trigonometry.FindAngle(vetThis, vetScannedRobot);

                    //Rotaciona o carro para o robo
                    RotateToAngle(Options.CAR, dblPosicaoRobo);

                    //Evita entrar novamente aqui enquanto procura o angulo
                    blnEncontrandoAngulo = true;
                }

                //Se está procurando o angulo e ja encontrou
                if (blnEncontrandoAngulo && RemainingRotation == 0)
                {
                    //Define que a rotação está completa
                    blnRotated = true;
                }

                //Se já rotacionou o carro mas a arma ainda nao
                if (blnRotated && !blnRotatingGun)
                {
                    
                    //Rotaciona a arma
                    RotateToAngle(Options.GUN, dblPosicaoRobo);

                    //Evita entrar aqui novamente, pois a arma está rotacionando
                    blnRotatingGun = true;
                }

                //Se a arma está rotacionando e já encontrou a posição desejada
                if (blnRotatingGun && RemainingRotationGun == 0)
                {
                    //Define que a arma está na posição desejada
                    blnRotatedGun = true;
                    
                }

                //Se a arma está na posicao desejada
                if (blnRotatedGun)
                {
                    //Atira
                    FireBullet(10);

                    //Reinicia as posicoes. Isso permite encontrar outro robo e reiniciar o processo
                    ReiniciarPosicoes();
                }
            }
        }

        #endregion

        #region | Metodos |

        /// <summary>
        /// Reinicia todas as posicoes relacionadas a busca de robos
        /// </summary>
        private void ReiniciarPosicoes()
        {
            blnRoboEncontrado = false;
            blnEncontrandoAngulo = false;
            blnRotatedGun = false;
            blnRotatingGun = false;
            blnRotated = false;

            dblPosicaoRobo = 0;

            dblEnemyX = 0;
            dblEnemyY = 0;
        }
        
        /// <summary>
        /// Overload do método que rotaciona o carro
        /// </summary>
        /// <param name="dblAngle"></param>
        private void RotateToAngle(double dblAngle)
        {
            RotateToAngle(Options.CAR, dblAngle);
        }

        /// <summary>
        /// Rotaciona o carro para o angulo passado por parametro, considerando a posição atual.
        /// <remarks>Este método é diferente do rotate, que rotaciona uma quantidade
        /// definida de graus, não considerando a posição atual
        /// OBSERVACAO: Este método nao é perfeito. O controle de quantas vezes ele é chamado deve ser feito externamente. NÃO FUNCIONA CORRETAMENTE SE FOR
        /// CHAMADO DUAS OU MAIS VEZES SEGUIDAS
        /// </remarks>
        /// </summary>
        /// <param name="dblAngle"></param>
        /// <param name="option">Define o que será rotacionado. Pode ser Options.CAR, Options.GUN, Options.RADAR</param>
        private void RotateToAngle(Options option, double dblAngle)
        {
            //Reinicia as posicoes
            blnPosicaoIdeal = false;
            blnVirandoPosicaoIdeal = false;

            //Se nao encontrou a posicao ideal
            if (!blnPosicaoIdeal)
            {
                blnPosicaoIdeal = true;

                //Define o novo angulo considerando a posicao atual
                double dblNovoAngulo = 0;

                //Inicia a rotação, se já não está fazendo.
                if (!blnVirandoPosicaoIdeal)
                {
                    blnVirandoPosicaoIdeal = true;

                    //Recupera o angulo atual e faz a rotação de acordo com a opcao selecionada
                    switch (option)
                    {
                        case Options.GUN:

                            dblNovoAngulo = dblAngle - RotationGunDeg;
                            RotateGunRightDeg(dblNovoAngulo);
                            
                            break;

                        case Options.RADAR:

                            dblNovoAngulo = dblAngle - RotationRadarDeg;
                            RotateRadarRightDeg(dblNovoAngulo);
                            
                            break;

                        default:
                        case Options.CAR:

                            dblNovoAngulo = dblAngle - RotationDeg;
                            RotateRightDeg(dblNovoAngulo);
                            
                            break;
                    }

                }
            }
        }

        /// <summary>
        /// Define que está prestes a colidir
        /// </summary>
        private void Colidindo()
        {
            blnColidindo = true;
            blnVirando = false;
            blnVoltando = false;
        }

        /// <summary>
        /// Determina que a posicao do robo é segura e ele pode seguir em frente, sem bater
        /// </summary>
        private void EmSeguranca()
        {
            blnColidindo = false;
            blnVirando = false;
            blnVoltando = false;
        }

        /// <summary>
        /// Metodo responsavel por evitar a colisão
        /// </summary>
        private void EvitarColisao()
        {
            //Se está colidindo
            if (blnColidindo)
            {
                //Se já nao esta voltando (dando ré), inicia a volta para trás
                if (!blnVoltando)
                {
                    blnVoltando = true;
                    MoveBackward(10);
                }

                //Se terminou a ré
                if (blnVoltando && RemainingDistance == 0)
                {
                    //Se nao está virando
                    if (!blnVirando)
                    {
                        //Inicia a rotação
                        blnVirando = true;
                        RotateLeftDeg(20);
                    }
                }

                //Se esta virando e encontrou a posicao ideal
                if (RemainingRotation == 0 && blnVirando)
                    EmSeguranca();

            }
            else
                MoveForward(10);
        }

        #endregion

        #region | Eventos |


        void Detonator_HitBullet(object sender, EventArgs e)
        {

        }

        void Detonator_HitWall(object sender, EventArgs e)
        {

        }

        void Detonator_HitRobot(object sender, EventArgs e)
        {


        }

        void Detonator_HitMissile(object sender, EventArgs e)
        {

        }

        void Detonator_ScannedRobot(object sender, ScannedRobotEventArgs e)
        {

            //Se nao foi encontrado nenhum robo, recupera as posicoes do robo escaneado
            if (!blnRoboEncontrado)
            {
                dblEnemyX = e.PositionX;
                dblEnemyY = e.PositionY;

                //Define que foi encontrado um robo. Evita entrar indesejadamente aqui
                blnRoboEncontrado = true;

            }


        }

        #endregion

        #region | Opcoes |
        
        /// <summary>
        /// Enum utilizado para definir com que parte do robo está lidando (carro, arma ou radar)
        /// </summary>
        private enum Options
        {
            GUN,
            CAR,
            RADAR
        }

        #endregion

    }





}
