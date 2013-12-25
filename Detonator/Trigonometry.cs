using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Detonator
{
    public class Trigonometry
    {

        #region | Vetor2D |

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

        #endregion

        #region | FindDistance |  
        
        /// <summary>
        /// Encontra a distancia entre dois pontos
        /// </summary>
        /// <param name="vetOrigem"></param>
        /// <param name="vetDestino"></param>
        /// <returns></returns>
        public static double FindDistance(Vetor2D vetOrigem, Vetor2D vetDestino)
        {

            
            double dblVariacaoX = Math.Pow(vetDestino.X - vetOrigem.X, 2);
            double dblVariacaoY = Math.Pow(vetDestino.Y - vetOrigem.Y, 2);

            //Cria o valor da hipotenusa (não será utilizado aqui, já que vamos trabalhar com tangente)
            double dblDistancia = Math.Sqrt(dblVariacaoY + dblVariacaoX);

            return dblDistancia;
        }

        #endregion

        #region | Find Angle |

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
        public static double FindAngle(Vetor2D vetOrigem, Vetor2D vetDestino)
        {
            double dblFinalAngle = 0.0;

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
        #endregion
    }



}
