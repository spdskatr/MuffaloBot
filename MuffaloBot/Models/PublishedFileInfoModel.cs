using System.Collections.Generic;

namespace MuffaloBot.Models
{
    public class PublishedFileInfoModel
    {
        public class TagModel
        {
            public string Tag { get; set; }
        }

        public class Publishedfiledetail
        {
            public string PublishedFileId { get; set; }
            public int Result { get; set; }
            public string Creator { get; set; }
            public int Creator_App_Id { get; set; }
            public int Consumer_App_Id { get; set; }
            public string Filename { get; set; }
            public int File_Size { get; set; }
            public string File_Url { get; set; }
            public string HContent_File { get; set; }
            public string Preview_Url { get; set; }
            public string HContent_Preview { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public int Time_Created { get; set; }
            public int Time_Updated { get; set; }
            public int Visibility { get; set; }
            public int Banned { get; set; }
            public string Ban_Reason { get; set; }
            public int Subscriptions { get; set; }
            public int Favorited { get; set; }
            public int Lifetime_Subscriptions { get; set; }
            public int Lifetime_Favorited { get; set; }
            public int Views { get; set; }

            public List<TagModel> Tags { get; set; }
        }

        public class Response
        {
            public int Result { get; set; }
            public int ResultCount { get; set; }
            public List<Publishedfiledetail> PublishedFileDetails { get; set; }
        }

        public Response response { get; set; }
    }
}