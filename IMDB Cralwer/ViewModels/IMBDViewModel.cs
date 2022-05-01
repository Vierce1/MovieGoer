using IMDB_Cralwer.Models;


namespace IMDB_Cralwer.ViewModels
{
    public class IMDBViewModel
    {
        public string? titleInput { get; set; }
        public List<Movie>? movieList { get ; set;}

        public  List<string> returnedMovieTitles { get; set; }
        public bool submitted { get; set; } = false;

        public List<Movie> userWatchedMovies { get; set; }


        public IMDBViewModel() //does this get called every time you go to this page?
        {
            titleInput = string.Empty;
            movieList = new List<Movie>();
            returnedMovieTitles = new List<string>();
            submitted = true;
            userWatchedMovies = new List<Movie>();
        }
    }

    

}
