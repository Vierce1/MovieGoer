using IMDB_Cralwer.Models;
using System.Web;

namespace IMDB_Cralwer.ViewModels
{
    public class IMDBViewModel
    {
        public string? titleInput { get; set; }
        public List<Movie>? movieList { get ; set;}

        public  List<string> returnedMovieTitles { get; set; }
        public bool submitted { get; set; } = false;

        public List<Movie> userWatchedMovies { get; set; }

        public bool shortMovie { get; set; } = false;

        public IMDBViewModel() //does this get called every time you go to this page?
        {
            titleInput = string.Empty;


            if (movieList == null)
            {                
                movieList = new List<Movie>();
            }


            returnedMovieTitles = new List<string>();
            submitted = true;

            if (userWatchedMovies == null)
            { userWatchedMovies = new List<Movie>(); }
        }


    }

    

}
