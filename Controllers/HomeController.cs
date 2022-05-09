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


        public string connectionString = "postgres://ntzgackjldsuog:73596861aaa9efc5b6d46b5c814d1a904bab53a202b08a9d72779915e716a47c@ec2-44-195-169-163.compute-1.amazonaws.com:5432/de7mbvodl8cjk9";
        bool useridExists;
        public string userId;
        public string userPassword;

        public List<Movie> watchedMovies { get; set; }

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        
        public IActionResult Index()
        {
            
            return View();

        }




        //register / sign in buttons
        public IActionResult RegisterOnClick( IndexViewModel m)
        {
            Console.WriteLine("Clicked register");
            Console.WriteLine(m.userName);

            if (m.userName != null && m.userName.Length > 0 && m.userName.Length < 20)
            {
                //first check sql tale to see if username already exists.
                ProcessUserReg(m.userName, m);
            }
            else
            {
                m.usernameInvalid = true;
            }
            System.Threading.Thread.Sleep(2200);
            Console.WriteLine("Reloading");

            //return RedirectToAction("Index", m);
            return View("Index", m);
        }


        async void ProcessUserReg(string username, IndexViewModel m)
        {

            string connString = GetConnectionStringNPG(connectionString);
            //set up and open the connection
            await using var conn = new NpgsqlConnection(connString); //create the connection
            await conn.OpenAsync();

            string query = "SELECT * FROM user_reg WHERE userid =  (@p1)";


            await using var command = new NpgsqlCommand(query, conn)
            {
                Parameters =
                {
                    new("@p1", username)
                }
            };

            await using var reader = await command.ExecuteReaderAsync(); //now we have selected the objects (rows) that fit


            if (reader != null && reader.HasRows)
            {
                while (await reader.ReadAsync())
                {

                    string s = reader.GetString(0);
                    if (s == null)
                    {
                        useridExists = false;

                    }
                    else
                    {
                        if (s.Length < 6 || s.Length > 20)
                        {
                            m.usernameInvalid = true;
                        }
                        else
                        {
                            useridExists = true;
                            m.usernameTaken = true;
                        }
                    }
                }
            }
            else
            {
                useridExists = false;
            }
            conn.Close();

            Console.WriteLine("user id  exists = "  +  useridExists.ToString());

            AddUserReg(m);
        }

        async void AddUserReg(IndexViewModel m)
        {
            if (m.password != null && m.password.Length > 8 && m.password.Length < 20 && !useridExists)
            {
                string hashedPW = new DataProcessing().AcceptPassword(m.password);

                //sql command to add user to database
                string connString = GetConnectionStringNPG(connectionString);
                await using var conn = new NpgsqlConnection(connString); //create the connection
                await conn.OpenAsync();

                string query = "INSERT INTO user_reg VALUES ((@p1), (@p2))";
                await using var command = new NpgsqlCommand(query, conn)
                {
                    Parameters =
                    {
                        new("@p1", m.userName),
                        new ("@p2", hashedPW)
                    }
                };

                await using var reader = await command.ExecuteReaderAsync();

                conn.Close();


            }
            else if(useridExists)
            {
                m.passwordInvalid = true;
            }


            Console.WriteLine("Finsihed reg");
            
        }



        public IActionResult SignInOnClick(IndexViewModel m)
        {
            Console.WriteLine("Clicked Sign In");
            //now query the db to see if registration exists, then update the controller
            SignInUser(m);

            System.Threading.Thread.Sleep(2200);
            

            //return RedirectToAction("Index");
            return View("Index", m);
        }

        async void SignInUser(IndexViewModel m)
        {
            string hashedPW = new DataProcessing().AcceptPassword(m.password);

            string connString = GetConnectionStringNPG(connectionString);
            await using var conn = new NpgsqlConnection(connString); //create the connection
            await conn.OpenAsync();

            string query = "SELECT * FROM user_reg WHERE userid = (@p1) AND password = (@p2)";
            await using var command = new NpgsqlCommand(query, conn)
            {
                Parameters =
                    {
                        new("@p1", m.userName),
                        new ("@p2", hashedPW)
                    }
            };

            await using var reader = await command.ExecuteReaderAsync();

            if (reader != null && reader.HasRows)
            {
                while (await reader.ReadAsync())
                {
                    //if the query[0] matches username, and we found the correct password, sign in user
                    string s = reader.GetString(0);
                    if (s == m.userName)
                    {
                        m.signedIn = true;

                    }
                    else
                    {
                        m.couldNotSignIn = true;
                    }
                }
            }
            else
            {
                m.couldNotSignIn = true;
            }

            conn.Close();
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

            return View(new IMDBViewModel());
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
            Console.WriteLine("Genre =  " + m.chosenGenreId);

            m.clickedSubmit = true;

            //get the connection string from DATABASE_URL = this comes from environment var. On localhost, it will come from path defined in sys properties. When deploying online, will receive it from the database
            //var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");


            //convert conn string to proper format for Npgsql commands
            string connString = GetConnectionStringNPG(connectionString);
            
            //postgres database connection string
            //var connectionString = "postgres://ntzgackjldsuog:73596861aaa9efc5b6d46b5c814d1a904bab53a202b08a9d72779915e716a47c@ec2-44-195-169-163.compute-1.amazonaws.com:5432/de7mbvodl8cjk9";

            Console.WriteLine(connString);

            //set up and open the connection
            await using var conn = new NpgsqlConnection(connString); //create the connection
            await conn.OpenAsync();

            

            string query = "SELECT * FROM mov_table WHERE title LIKE (@p1)"; 



            string genre = " mbasic.genre LIKE ";
            if (m.shortMovie)
            {

                //choose genre if selected on page
                switch (m.chosenGenreId)
                {
                    case 1: genre = ""; break;
                    case 2:
                        genre += " 'Action' AND ";
                        break;
                    case 3:
                        genre += " 'Comedy' AND ";
                        break;
                    case 4:
                        genre += " 'Drama' AND ";
                        break;
                    case 5:
                        genre += " 'Sci-Fi' AND ";
                        break;
                    case 6:
                        genre += " 'Crime' AND ";
                        break;


                }

                //final query for short movie with genre injected
                query = "SELECT * " +
                "FROM (SELECT mov_table.title, mbasic.rating, mbasic.year, genre, runtime , mbasic.titleid " +
                " FROM mov_table JOIN mbasic ON mov_table.title = mbasic.title " +
                "WHERE " + genre + " mbasic.runtime < 100 AND mbasic.runtime != 0 " +
                " AND  (mov_table.title LIKE (@p2) " +
                " OR mov_table.title LIKE (@p1))) " +
                " AS newTable JOIN world_events ON world_events.year = newTable.year " +
                "JOIN songs ON songs.year = newTable.year ";


            }
            else //all movie lengths
            {
                //choose genre if selected on page
                switch (m.chosenGenreId)
                {
                    case 1: genre = ""; break;
                    case 2:
                        genre += " 'Action' AND ";
                        break;
                    case 3:
                        genre += " 'Comedy' AND ";
                        break;
                    case 4:
                        genre += " 'Drama' AND ";
                        break;
                    case 5:
                        genre += " 'Sci-F' AND ";
                        break;
                    case 6:
                        genre += " 'Crime' AND ";
                        break;
                }

                //final query for non-short movie with genre injected
                query = "SELECT * " +
                "FROM (SELECT mov_table.title, mbasic.rating, mbasic.year, genre, runtime , mbasic.titleid " +
                " FROM mov_table JOIN mbasic ON mov_table.title = mbasic.title " +
                "WHERE " + genre  +
                " (mov_table.title LIKE (@p2) " +
                " OR mov_table.title LIKE (@p1))) " +
                " AS newTable JOIN world_events ON world_events.year = newTable.year " +
                "JOIN songs ON songs.year = newTable.year ";
            }

            
            //sort by rating and limit to 150 rows
            query += "ORDER BY rating DESC " +
                "LIMIT 150";



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
                //DataTable - can convert sql reader into a class object

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



            while (await reader.ReadAsync())
            {


                //string[] s = new string[7] { reader["title"].ToString(), reader["title"].ToString(), reader["title"].ToString(), reader["title"].ToString(), reader["title"].ToString(), reader["title"].ToString(), reader["title"].ToString() };
                string[] s = new string[6] { GetSafeString(reader, 0), GetSafeString(reader, 1), GetSafeString(reader, 2), GetSafeString(reader, 3), GetSafeString(reader, 6), GetSafeString(reader, 9) };

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
                        year = TrimLastChar( s[2]),
                        titleId = GetSafeString(reader, 5),
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
                        artist = s[5],

                        wikiLink = "https://www.wikipedia.org/wiki/" + s[2],
                        imdbLink = "https://www.imdb.com/title/" + reader[5].ToString()


                    });;



                }

            }


            // close the connection
            conn.Close();


            //add all movies to the list to display
            if (myMovies != null && myMovies.Count > 1)
            {
                m.movieList = myMovies.ToList();
            }

        }


        [HttpPost]
        public  IActionResult IMDB(IMDBViewModel m)
        {


            if (m.titleInput != "")
            {
                ProcessKeyWord(m.titleInput, m);
            }
            System.Threading.Thread.Sleep(1200); //crucial - makes system pause while loading results from ProcessKeyWord()


            
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

        public string GetConnectionStringNPG(string uri)
        {
            var databaseUri = new Uri(uri);
            var userInfo = databaseUri.UserInfo.Split(':');

            var builder = new NpgsqlConnectionStringBuilder
            {
                Host = databaseUri.Host,
                Port = databaseUri.Port,
                Username = userInfo[0],
                Password = userInfo[1],
                Database = databaseUri.LocalPath.TrimStart('/')
            };

            return builder.ToString();
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

        string TrimFirstChar(string s)
        {

            s = s.Remove(0, 1); 
            return s;
        }
        string TrimLastChar(string s)
        {

            s = s.Remove(s.Length - 1 , 1);
            return s;
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}