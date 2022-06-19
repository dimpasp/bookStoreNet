namespace Library.Helpers
{
    public class RegisterResultViewModel
    {
        public bool InvalidPassword { get; set; }
        public bool InvalidEmail { get; set; }
        public bool DuplicateUser { get; set; }
        public bool DuplicateExternalUser { get; set; }
        public bool InvalidUserName { get; set; }
        public string? GeneralError { get; set; }
        public bool IsSuccess { get; set; } = true;
    }
}
