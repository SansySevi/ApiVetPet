using System.Security.Cryptography;
using System.Text;

namespace ApiVetPet.Helpers
{
    public class HelperCryptography
    {
        //Metodo para generar un SALT
        public static string GenerateSalt()
        {
            Random random = new Random();
            string salt = "";
            for (int i = 1; i <= 50; i++)
            {
                int aleat = random.Next(0, 255);
                char letra = Convert.ToChar(aleat);
                salt += letra;
            }
            return salt;
        }

        //Metodo para comparar salt
        public static bool CompareArrays(byte[] a, byte[] b)
        {
            bool iguales = true;
            if (a.Length != b.Length)
            {
                iguales = false;
            }
            else
            {
                for (int i = 0; i < a.Length; i++)
                {
                    if (a[i].Equals(b[i]) == false)
                    {
                        iguales = false;
                        break;
                    }
                }
            }
            return iguales;
        }

        //Metodo para cifrar un PASSWORD
        public static byte[]
            EncryptPassword(string password, string salt)
        {
            string contenido = password + salt;
            SHA512 sHA = SHA512.Create();
            byte[] salida = Encoding.UTF8.GetBytes(contenido);
            for (int i = 1; i <= 107; i++)
            {
                salida = sHA.ComputeHash(salida);
            }
            sHA.Clear();
            return salida;
        }
    }

}
