namespace TwitterWebApp1.Models.Profile
{
    public class ProfileViewModel
    {
        //public image
        public string UserName { get; set; }

        public string Name { get; set; }

        public string Bio { get; set; }

        public string Location { get; set; }

        public string JoinedDate { get; set; }

        public IFormFile? ProfileImageFile { get; set; }

        public string ProfileImage { get; set; }

        public bool IsCurrentUser { get; set; }
    }
}

