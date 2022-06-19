using Library.Entities;

namespace Library.Interfaces
{
    public interface IUserRepository
    {
        bool CheckIfUserExist(string name);
        Task CreateUser(Student student);
        Task<string> GetStudentId(Student student);
    }
}
