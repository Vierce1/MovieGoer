namespace IMDB_Cralwer
{
    public class Helper
    {

        public static string ConnectionString(string name)
        {
            //"Data Source=PostgreSQL Database (.NET Framework Data Provider for PostgreSQL); Database = imdb; User Id = postgres; Password = Apostria1";
            return System.Configuration.ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }

    }
}
