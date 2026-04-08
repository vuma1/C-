using System;

namespace QuanLyQuanCafe.Models
{
    public class Feedback
    {
        public int Id { get; set; }
        public int StarRating { get; set; }
        public string ProductRating { get; set; }
        public string ServiceRating { get; set; }
        public string OtherComment { get; set; }
        public DateTime CreatedAt { get; set; }

        public Feedback(int id, int starRating, string productRating, string serviceRating, string otherComment, DateTime createdAt)
        {
            Id = id;
            StarRating = starRating;
            ProductRating = productRating;
            ServiceRating = serviceRating;
            OtherComment = otherComment;
            CreatedAt = createdAt;
        }
    }
}
