using IMDB_Cralwer.Models;
using System.Security.Cryptography;


namespace IMDB_Cralwer
{
    public class DataProcessing
    {
        public List<Movie> savedMovies;

        public string AcceptPassword(string password)
        {
            byte[] salt = new byte[16];
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations: 5000);

            string hPassword = Convert.ToBase64String(pbkdf2.GetBytes(32));

            Console.WriteLine(hPassword);
            return hPassword; 



            

            //using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltSize, iterations))
            //{

            //    byte[] salt = new byte[16];
            //    using (var rngCsp = new RNGCryptoServiceProvider())
            //    {
            //        rngCsp.GetNonZeroBytes(salt);
            //    }
                //Console.WriteLine($"Salt: {Convert.ToBase64String(salt)}");

            //    string hashed = pbkdf2;
            //Console.WriteLine(hashed);
            //return hashed;

            //using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            //{
            //    // Fill the array with a random value.
            //    rngCsp.GetBytes(salt);
            //}


        }
    }
}
