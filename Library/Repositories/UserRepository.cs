using Library.Data;
using Library.Entities;
using Library.Interfaces;

namespace Library.Repositories
{
    public class UserRepository : IUserRepository
    {

        private readonly LibraryContext context;

        public UserRepository(LibraryContext context)
        {
            this.context = context;
        }
        public bool CheckIfUserExist(string name)
        {
            return context.Student.Any(s => s.Name == name);
        }

        public async Task CreateUser(Student student)
        {

            await context.AddAsync(student);
            await context.SaveChangesAsync();
        }
        public async Task<string> GetStudentId(Student student)
        {

            return context.Student.Where(x => x.Name == student.Name)
                                  .Select(x => x.Id)
                                  .FirstOrDefault().ToString();
        }
    }
}
