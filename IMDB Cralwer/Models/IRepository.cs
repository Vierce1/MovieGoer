using System.Text.Json.Serialization;
using System.Text.Json;


namespace IMDB_Cralwer.Models
{


    public  interface IRepository
    {
        //List<Movie> GetAllMovies();
        //List<Movie> GetMoviesByDirector(string directorName);
        //List<Movie> GetMoviesInYear(string year);
        //List<string> ListMoviesWithRatings();

        public void UserName(string userid);
        public string GetUserName();
        public void StoreWatchedMovies(string title);
        public List<string> GetWatchedMovies();

    }


    public class UserRepository : IRepository
    {
        public string username;
        public List<string> watchedMovies = new List<string>();

        public UserRepository(string userName)
        {
            username = userName;
        }
        void IRepository.UserName(string userid)
        {
            username = userid;
        }
        string IRepository.GetUserName()
        {
            return username;
        }
        void IRepository.StoreWatchedMovies(string title)
        {
            watchedMovies.Add(title);
        }
        List<string> IRepository.GetWatchedMovies()
        {
            return watchedMovies;
        }

    }



    //class JsonDatabase : IRepository
    //{

    //    public string jsonP;

    //    public JsonDatabase(string jsonPath)
    //    {
    //        jsonP = jsonPath;
    //    }

    //    List<Movie> IRepository.GetAllMovies()
    //    {
    //        string serialized = System.IO.File.ReadAllText(jsonP);

    //        return JsonSerializer.Deserialize<List<Movie>>(serialized);
    //    }

    //    List<Movie> IRepository.GetMoviesByDirector(string directorName)
    //    {
    //        //directorName 
    //        return new List<Movie>();
    //    }

    //    List<Movie> IRepository.GetMoviesInYear(string year)
    //    {
    //        //do something
    //        return new List<Movie>();
    //    }




    //}




    //class SQLDatabase : IRepository
    //{

    //    public string conString;

    //    public SQLDatabase(string connectionString)
    //    {
    //        conString = connectionString;
    //    }

    //    List<Movie> IRepository.GetAllMovies()
    //    {
    //        return new List<Movie>();
    //    }

    //    List<Movie> IRepository.GetMoviesByDirector(string directorName)
    //    {
    //        //directorName 
    //        return new List<Movie>();
    //    }

    //    List<Movie> IRepository.GetMoviesInYear(string year)
    //    {
    //        //do something
    //        return new List<Movie>();
    //    }




    //}
}
