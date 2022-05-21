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
        public int chosenGenreId { get; set; } = 1;
        public IEnumerable<Genre> Genres { get; set; }
        public bool actionGenre { get; set; } = false;
        public bool comedyGenre { get; set; } = false;
        public bool scifiGenre { get; set; } = false;
        public bool crimeGenre { get; set; } = false;
        public bool dramaGenre { get; set; } = false;
        public bool clickedSubmit { get; set; } = false;
        public IMDBViewModel() //does this get called every time you go to this page?
        {
            titleInput = string.Empty;


            if (movieList == null)
            {                
                movieList = new List<Movie>();
            }

            Genres = new List<Genre>()
            {
                new Genre{ GenreId = 1, GenreName = "All Genres"},
                 new Genre{ GenreId = 2, GenreName = "Action" },
                  new Genre{ GenreId = 3, GenreName = "Comedy" },
                   new Genre{ GenreId = 4, GenreName = "Drama" },
                    new Genre{ GenreId = 5, GenreName = "Sci-Fi" },
                     new Genre{ GenreId = 6, GenreName = "Crime" }
                     //new Genre{ GenreId = 7, GenreName = "Western"},
                     //new Genre{ GenreId = 8, GenreName = "Documentary"},
                     };

            returnedMovieTitles = new List<string>();
            submitted = true;


        }


    }

    

}
