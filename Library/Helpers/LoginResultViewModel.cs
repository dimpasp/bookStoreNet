namespace Library.Helpers
{
    public class LoginResultViewModel
    {
        public bool UserOrPasswordWrong { get; set; }
        public bool EmailNotConfirmed { get; set; }
        public bool IsSuccess { get; set; } = true;
    }
}
