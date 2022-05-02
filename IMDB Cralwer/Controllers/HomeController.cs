using IMDB_Cralwer.Models;
using IMDB_Cralwer.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using Npgsql;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
namespace IMDB_Cralwer.Controllers
{
    public class HomeController : Controller
    {
        IMDBViewModel viewModel;
        DataProcessing dp;
        public List<Movie> watchedMovies { get; set; }

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        
        public IActionResult Index()
        {
            dp = new DataProcessing();
            return View();

        }

        public IActionResult Privacy()
        {
            return View();
        }


        [HttpGet]
        public IActionResult IMDB() //returns the view when you enter the page
        {

            //IMDBViewModel m = new IMDBViewModel();
            //if (viewModel != null)
            //{
            //    //m.movieList = viewModel.movieList;
            //    return View(viewModel);
            //}

            return View();
        }


        public async void ProcessKeyWord(string word, IMDBViewModel m)
        {
            Console.WriteLine(m.titleInput); //this returns the correct input word

            //int year = 0;
            //bool useYear = false;
            //if (int.TryParse(word, out year)) //change to another field
            //{
            //    if (Convert.ToInt32(word) <= 2022 && Convert.ToInt32(word) > 1950)
            //    {
            //        useYear = true;
            //        //treat as a year
            //        //use word now since it is a string
            //        //now query the year table instead
            //    }

            //}



            var connectionString = "Host=localhost;Username=postgres;Password=Apostria1;Database=imdb"; 
            //var connStringTrusted = "Server=localhost; Database = imdb; Trusted_Connection = True"; //use this if you have set the db to allow you to use windows creds
            await using var conn = new NpgsqlConnection(connectionString); //create the connection
            await conn.OpenAsync();

            /*word = "Clown";*/ //input from IMDB.cshtml on webpage

            string query = "SELECT * FROM mov_table WHERE title LIKE (@p1)"; //leaving out 'title' should select entire table


            query = "SELECT * " +
                "FROM(SELECT mov_table.title, movie_basics.rating, movie_basics.year, genre, runtime " +
                " FROM mov_table JOIN movie_basics ON mov_table.title = movie_basics.title " +
                "  WHERE mov_table.title LIKE (@p1) " +
                "OR  mov_table.title LIKE (@p2) " +
                "OR movie_basics.genre LIKE (@p3))" +
                " AS newTable JOIN world_events ON world_events.year = newTable.year " +
                "JOIN songs ON songs.year = newTable.year " +
                "ORDER BY rating DESC "+
                "LIMIT 150";

            if (m.shortMovie)
            {
                Console.WriteLine("Short movie picked");
                query  = "SELECT * " +
                "FROM(SELECT mov_table.title, movie_basics.rating, movie_basics.year, genre, runtime " +
                " FROM mov_table JOIN movie_basics ON mov_table.title = movie_basics.title " +
                "  WHERE mov_table.title LIKE (@p1) AND movie_basics.runtime < 100 AND movie_basics.runtime != 0 " +
                "OR  mov_table.title LIKE (@p2) AND movie_basics.runtime < 100 AND movie_basics.runtime != 0 " +
                "OR movie_basics.genre LIKE (@p3) AND movie_basics.runtime < 100 AND movie_basics.runtime != 0) " +
                " AS newTable JOIN world_events ON world_events.year = newTable.year " +
                "JOIN songs ON songs.year = newTable.year " +
                "ORDER BY rating DESC " +
                "LIMIT 150";
            }


            //if (useYear) 
            //{
            //    //query = "SELECT event FROM world_events WHERE year = word";
            //}


            await using var command = new NpgsqlCommand(query, conn)
            {
                Parameters =
                {
                    new("@p1", "%" +word+ "%"),
                    new("@p2", "%" +word+ " %"),
                    new("@p3", "%" +word+ "%")
                }
            };


            {
                //DataTable may work now that i figured out the Select * From

                //DataTable dt = new DataTable(); //should be able to infer the shcema from the data
                //dt.Columns.Add("titleId", typeof(string));
                //dt.Columns.Add("title", typeof(string));
                ////dt.Columns.Add("ordering", typeof(string));
                //dt.Columns.Add("region", typeof(string));
                //dt.Columns.Add("language", typeof(string));
                //dt.Columns.Add("types", typeof(string));
                //dt.Columns.Add("attributes", typeof(string));
                //dt.Columns.Add("isOriginalTitle", typeof(string));

                //for (int i = 0; i < 100; i++) //create 100 rows on the datatable. May need to use the actual number to avoid null
                //{
                //    dt.Rows.Add(new object[] { "titleId", "title", "region", "language", "types", "attributes", "isOriginalTitle" });
                //}
            }

            List<Movie> myMovies = new List<Movie> { };

            await using var reader = await command.ExecuteReaderAsync(); //now we have selected the objects (rows) that fit


            //bsed on the errors, reader does not have any columns

            while (await reader.ReadAsync())
            {

                //m.movieList.Add(reader.GetData(0));

                //string[] s = new string[7] { reader["title"].ToString(), reader["title"].ToString(), reader["title"].ToString(), reader["title"].ToString(), reader["title"].ToString(), reader["title"].ToString(), reader["title"].ToString() };
                string[] s = new string[6] { GetSafeString(reader, 0), GetSafeString(reader, 1), GetSafeString(reader, 2), GetSafeString(reader, 3),  GetSafeString(reader, 5), GetSafeString(reader, 8)};

                for (int i = 0; i < s.Length; i++)
                {
                    bool isWatched = false;
                    foreach (Movie mov in m.movieList) //check if the movie is already watched
                    {
                        if (mov.isWatched && mov.title == s[0])
                        {
                            isWatched = true;
                        }
                    }

                    myMovies.Add(
                    new Movie
                    {
                        title = s[0],
                        year = s[2],
                        titleId = "",
                        region = "",
                        language = "",
                        types = "",
                        attributes = "",
                        isOriginalTitle = "",
                        worldEvent = s[4],
                        rating = s[1],
                        genre = s[3],
                        runtime = reader.GetInt16(4),
                        isWatched = isWatched,
                        artist = s[5]


                    }) ;



                }

            }

            //IRepository sqlDb = new SQLDatabase(connectionString); //if use this, move the sql query into the irepos
            //List<Movie> movies = sqlDb.GetAllMovies();


            //need to close the connection at the end
            conn.Close();


            if (myMovies != null && myMovies.Count > 1)
            {

                m.movieList = myMovies.ToList();
            }


                //foreach(Movie mov in myMovies)
                //{
                //    m.returnedMovieTitles.Add( FetchStrings("Title", mov)); //get a list of the titles only to display
                //}
            

            //RedirectToAction("Create", "Home");
        }


        [HttpPost]
        public  IActionResult IMDB(IMDBViewModel m)
        {
            //if (viewModel == null)
            //{
            //    viewModel = m;
            //}
            //else
            //{
            //    m = viewModel;
            //    Console.WriteLine("View model not null");
            //}

            if (m.titleInput != "")
            {
                ProcessKeyWord(m.titleInput, m);
            }
            System.Threading.Thread.Sleep(500); //crucial - makes system pause while loading results from ProcessKeyWord()

            //if (m.movieList != null && m.movieList.Count > 0)
            //{
            //    Console.WriteLine("MovieList not null");  
            //    SaveWatchedMovies(m); 
            //    System.Threading.Thread.Sleep(500);
            //}
            //else 
            //{ 
            //    Console.WriteLine("Null"); 
            //}


            
            int k = 0;
            for(int i = 0; i < m.movieList.Count; i++)
            {
                if (m.movieList[i].isWatched)
                {
                    k++;
                }
            }
            //Console.WriteLine(k + "  watched");



            return View(m);
        }


        public void SaveWatchedMovies(IMDBViewModel m)
        {
            foreach (Movie movie in m.movieList)
            {
                //Console.WriteLine(movie.title);
                if (movie.isWatched)
                {
                    watchedMovies.Add(movie);
                    Console.WriteLine("Added " + movie.title);
                }
            }
        }

        public void SaveWatchedMovieChecked(Movie mov)
        {
            watchedMovies.Add(mov);
            Console.WriteLine(mov.title + " Added");
        }


        public string FetchStrings(string requestedMovieProperty, Movie mov)
        {
            switch (requestedMovieProperty)
            {
                case "Title":
                    return mov.title;
            }
            return "";
        }



        private string GetSafeString(NpgsqlDataReader reader, int colindex)
        {
            if (reader.IsDBNull(colindex))
            {
                return string.Empty;
            }
            else
            {
                return reader.GetString(colindex);
            }
        }

        public void OnCheckChange()
        {
            Console.WriteLine("Ran OnCheckChagne");
            //Response.Write(Request.RawUrl.ToString());
        }


        //[HttpPost]
        //public  IActionResult IMDB(IMDBViewModel m)
        //{

        //    IRepository jsonDatabase = new JsonDatabase(@"H:\C# Projects\JSONDatabase\IMDBMovieTitleInfo");
        //    List<Movie> moviesList = jsonDatabase.GetAllMovies();

        //    moviesList = moviesList.Where(movie => movie.title.Contains(m.titleInput) && movie.region == "US").ToList();

        //    m.movieList = moviesList;
        //    ProcessDB(m.titleInput.ToString(), m);

        //    return View(m);
        //}




        //Original functionality using json serialization
        //public IActionResult BuildDatabase(IMDBViewModel m)  DID THIS IN CONSOLE APP. This was to convert the data to a serialized json file / read it out
        //{
        //    var reader = new StreamReader(@"H:\C# Projects\IMDB Cralwer\data.tsv");
        //    List<Movie> moviesList = new List<Movie> { };
        //    string movie;
        //    while ((movie = reader.ReadLine()) != null)
        //    {
        //        Movie mov = new Movie(movie);

        //    }

        //    m.movieList = moviesList;

        //    string fileName = @"H:\C# Projects\JSONDatabase\IMDBMovieTitleInfo";
        //    string jsonString = JsonSerializer.Serialize(moviesList);
        //    System.IO.File.WriteAllText(fileName, jsonString);

        //    m.builtDatabase = true;
        //    return View(m);
        //}




        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}