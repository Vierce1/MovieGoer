namespace IMDB_Cralwer.Models
{
    public class Movie
    {
        public string title { get; set; }
        public string titleId { get; set; }
        //public string ordering { get; set; }
        public string region { get; set;}
        public string language { get; set;}
        public string types { get; set; }
        public string attributes { get; set; }
        public string isOriginalTitle { get; set; }
        public string year { get; set; }
        public string worldEvent { get; set; }
        public string artist { get; set; }
        public string rating { get; set; }
        public string genre { get; set; }
        public int runtime { get; set; }
        public bool isWatched { get; set; }

        public string wikiLink { get; set; }
        public string imdbLink  { get; set; }

        //public List<Director> directors {get; set;}
        //public string releaseDate {get; set;} // should this actually be a string?
        //public int rating {get; set;}




        //public Movie(string[] movieRow)
        //{

        //    //string[] movieInfo = movieRow.Split("\t");

        //    titleId = movieRow[0];
        //    //ordering = movieInfo[1];
        //    title = movieRow[1];
        //    region = movieRow[2];
        //    language = movieRow[3];
        //    types = movieRow[4];
        //    attributes = movieRow[5];
        //    isOriginalTitle = movieRow[6];

        //}

        //public Movie()
        //{

        //}
    }
}
