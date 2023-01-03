using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NerdStore.Core.DomainObjects
{
    public class Validacoes //AssertConcern
    {
        public static void ValidarSeIgual(object object1, object object2, string messagem)
        {
            if (object1.Equals(object2))
            {
                throw new DomainException(messagem);
            }
        }

        public static void ValidarSeDiferente(object object1, object object2, string messagem)
        {
            if (!object1.Equals(object2))
            {
                throw new DomainException(messagem);
            }
        }

        public static void ValidarCaracteres(string valor, int maximo, string messagem)
        {
            var length = valor.Trim().Length;
            if (length > maximo)
            {
                throw new DomainException(messagem);
            }
        }

        public static void ValidarCaracteres(string valor, int minimo, int maximo, string messagem)
        {
            var length = valor.Trim().Length;
            if (length < minimo || length > maximo)
            {
                throw new DomainException(messagem);
            }
        }

        public static void ValidarSeVazio(string valor, string messagem)
        {
            if (valor == null || valor.Trim().Length == 0)
            {
                throw new DomainException(messagem);
            }
        }

        public static void ValidarSeNulo(object object1, string messagem)
        {
            if (object1 == null)
            {
                throw new DomainException(messagem);
            }
        }

        public static void ValidarMinimoMaximo(double valor, double minimo, double maximo, string messagem)
        {
            if (valor < minimo || valor > maximo)
            {
                throw new DomainException(messagem);
            }
        }

        public static void ValidarSeMenorQue(decimal valor, decimal minimo, string messagem)
        {
            if (valor <= minimo)
            {
                throw new DomainException(messagem);
            }
        }

        public static void ValidarSeFalso(bool boolValor, string messagem)
        {
            if (!boolValor)
            {
                throw new DomainException(messagem);
            }
        }

        public static void ValidarSeVerdadeiro(bool boolValor, string messagem)
        {
            if (boolValor)
            {
                throw new DomainException(messagem);
            }
        }

    }
}
