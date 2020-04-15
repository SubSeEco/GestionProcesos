using System;
using System.Net.Mail;

namespace App.Infrastructure.Extensions
{
    public static class ExtensionesString
    {
        public static bool IsBoolean(this string texto)
        {
            bool valor;
            return bool.TryParse(texto, out valor);
        }
        public static bool IsInt(this string texto)
        {
            int valor;
            return int.TryParse(texto, out valor);
        }
        public static bool IsDecimal(this string texto)
        {
            decimal valor;
            return decimal.TryParse(texto, out valor);
        }
        public static bool IsDouble(this string texto)
        {
            double valor;
            return double.TryParse(texto, out valor);
        }
        public static bool IsFloat(this string texto)
        {
            float valor;
            return float.TryParse(texto, out valor);
        }
        public static bool IsDateTime(this string texto)
        {
            DateTime valor;
            return DateTime.TryParse(texto, out valor);
        }
        public static bool IsHour(this string texto)
        {
            var arregloHora = texto.Split(':');
            if (arregloHora.Length != 2)
            {
                return false;
            }

            int valorInt;
            if (!int.TryParse(arregloHora[0], out valorInt))
            {
                return false;
            }
            if (!int.TryParse(arregloHora[1], out valorInt))
            {
                return false;
            }

            DateTime valor;
            return DateTime.TryParse(texto, out valor);
        }
        public static bool IsRut(this string texto)
        {
            if (texto == null)
                return false;

            int parteNumeral;
            var arregloRut = texto.Split('-');
            texto = texto.Insert(texto.Length - 1, "-");
            //var arregloRut = texto.Split('-');
            if (arregloRut.Length != 2)
                return false;

            if (!int.TryParse(arregloRut[0], out parteNumeral))
                return false;

            var digitoVerificador = arregloRut[1].ToUpper();

            var contador = 2;
            var acumulador = 0;
            while (parteNumeral != 0)
            {
                var multiplo = parteNumeral % 10 * contador;
                acumulador = acumulador + multiplo;
                parteNumeral = parteNumeral / 10;
                contador = contador + 1;
                if (contador == 8)
                    contador = 2;
            }
            var digito = 11 - acumulador % 11;
            var rutDigito = digito.ToString().Trim();
            if (digito == 10)
                rutDigito = "K";
            if (digito == 11)
                rutDigito = "0";

            return rutDigito == digitoVerificador;
        }
        public static bool IsEmail(this string email)
        {
            try
            {
                var mailAddress = new MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }
        public static bool IsUrl(this string texto)
        {
            Uri uri;
            return Uri.TryCreate(texto, UriKind.Absolute, out uri);
        }
        public static bool IsNullOrWhiteSpace(this string texto)
        {
            return string.IsNullOrWhiteSpace(texto);
        }
        public static bool IsGuId(this string texto)
        {
            Guid valor;
            return Guid.TryParse(texto, out valor);
        }
        public static int ToInt(this string texto)
        {
            int valor;
            int.TryParse(texto, out valor);
            return valor;
        }
        public static string enletras(string num)
        {

            string res, dec = "";

            Int64 entero;

            int decimales;

            double nro;

            try
            {

                nro = Convert.ToDouble(num);

            }

            catch
            {

                return "";

            }

            entero = Convert.ToInt64(Math.Truncate(nro));

            decimales = Convert.ToInt32(Math.Round((nro - entero) * 100, 2));

            if (decimales > 0)
            {

                dec = " CON " + decimales.ToString() + "PESOS";

            }

            res = toText(Convert.ToDouble(entero)) + dec;

            return res;

        }
        private static string toText(double value)
        {

            string Num2Text = "";

            value = Math.Truncate(value);

            if (value == 0) Num2Text = "CERO";

            else if (value == 1) Num2Text = "UNO";

            else if (value == 2) Num2Text = "DOS";

            else if (value == 3) Num2Text = "TRES";

            else if (value == 4) Num2Text = "CUATRO";

            else if (value == 5) Num2Text = "CINCO";

            else if (value == 6) Num2Text = "SEIS";

            else if (value == 7) Num2Text = "SIETE";

            else if (value == 8) Num2Text = "OCHO";

            else if (value == 9) Num2Text = "NUEVE";

            else if (value == 10) Num2Text = "DIEZ";

            else if (value == 11) Num2Text = "ONCE";

            else if (value == 12) Num2Text = "DOCE";

            else if (value == 13) Num2Text = "TRECE";

            else if (value == 14) Num2Text = "CATORCE";

            else if (value == 15) Num2Text = "QUINCE";

            else if (value < 20) Num2Text = "DIECI" + toText(value - 10);

            else if (value == 20) Num2Text = "VEINTE";

            else if (value < 30) Num2Text = "VEINTI" + toText(value - 20);

            else if (value == 30) Num2Text = "TREINTA";

            else if (value == 40) Num2Text = "CUARENTA";

            else if (value == 50) Num2Text = "CINCUENTA";

            else if (value == 60) Num2Text = "SESENTA";

            else if (value == 70) Num2Text = "SETENTA";

            else if (value == 80) Num2Text = "OCHENTA";

            else if (value == 90) Num2Text = "NOVENTA";

            else if (value < 100) Num2Text = toText(Math.Truncate(value / 10) * 10) + " Y " + toText(value % 10);

            else if (value == 100) Num2Text = "CIEN";

            else if (value < 200) Num2Text = "CIENTO " + toText(value - 100);

            else if ((value == 200) || (value == 300) || (value == 400) || (value == 600) || (value == 800)) Num2Text = toText(Math.Truncate(value / 100)) + "CIENTOS";

            else if (value == 500) Num2Text = "QUINIENTOS";

            else if (value == 700) Num2Text = "SETECIENTOS";

            else if (value == 900) Num2Text = "NOVECIENTOS";

            else if (value < 1000) Num2Text = toText(Math.Truncate(value / 100) * 100) + " " + toText(value % 100);

            else if (value == 1000) Num2Text = "MIL";

            else if (value < 2000) Num2Text = "MIL " + toText(value % 1000);

            else if (value < 1000000)
            {

                Num2Text = toText(Math.Truncate(value / 1000)) + " MIL";

                if ((value % 1000) > 0) Num2Text = Num2Text + " " + toText(value % 1000);

            }

            else if (value == 1000000) Num2Text = "UN MILLON";

            else if (value < 2000000) Num2Text = "UN MILLON " + toText(value % 1000000);

            else if (value < 1000000000000)
            {

                Num2Text = toText(Math.Truncate(value / 1000000)) + " MILLONES ";

                if ((value - Math.Truncate(value / 1000000) * 1000000) > 0) Num2Text = Num2Text + " " + toText(value - Math.Truncate(value / 1000000) * 1000000);

            }

            else if (value == 1000000000000) Num2Text = "UN BILLON";

            else if (value < 2000000000000) Num2Text = "UN BILLON " + toText(value - Math.Truncate(value / 1000000000000) * 1000000000000);

            else
            {

                Num2Text = toText(Math.Truncate(value / 1000000000000)) + " BILLONES";

                if ((value - Math.Truncate(value / 1000000000000) * 1000000000000) > 0) Num2Text = Num2Text + " " + toText(value - Math.Truncate(value / 1000000000000) * 1000000000000);

            }

            return Num2Text;

        }

    }
}
