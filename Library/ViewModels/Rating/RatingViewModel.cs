namespace Library.ViewModels.Rating
{
    public class RatingViewModel
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public int BookId { get; set; }
        public float? Rating { get; set; }
        public string? Comment { get; set; }

        public List<RatingViewModel>? RatingStudentBooks { get; set; }

    }
}
