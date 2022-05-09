namespace IMDB_Cralwer.ViewModels
{
    public class IndexViewModel
    {

        public string userName { get; set; }
        public string password { get; set; }

        public bool usernameInvalid { get; set; }
        public bool usernameTaken { get; set; }
        public bool passwordInvalid { get; set; }
        public bool couldNotSignIn { get; set; }
        public bool signedIn { get; set; }
    }
}
