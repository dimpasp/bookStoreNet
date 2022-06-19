using Library.Data;
using Library.Entities;
using Library.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.Repositories
{
    public class StudentRepository: IStudentRepository
    {
        private readonly LibraryContext context;

        public StudentRepository(LibraryContext context)
        {
            this.context = context;
        }

        public bool IsStudent(int studentId,int user)
        {
            try
            {
                return context.Student.Any(i => i.Id == studentId && studentId==user);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
        public async Task<Student?> GetStudent(string name)
        {
            return await context.Student
                       .Include(s => s.StudentBooks)
                       .ThenInclude(b => b.Book)
                       .FirstOrDefaultAsync(m => m.Name == name);
           
        }
        public async Task CreateStudent(Student student)
        {
            context.Add(student);
            await context.SaveChangesAsync();
        }

        public async Task<Student?> GetById(int? id)
        {

            return await context.Student
                .Include(s => s.StudentBooks)
                .ThenInclude(b => b.Book)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<List<Student>> GetAllStudents()
        {
            return await context.Student.ToListAsync();
        }

        public List<StudentBook> GetStudentBooksById(int id)
        {
            return context.StudentBook.Where(m => m.StudentId == id).ToList();
        }

        public async Task<bool> RemoveBook(int id)
        {
            try
            {
                var StudentBooks = GetStudentBooksById(id);

                if (StudentBooks != null)
                {
                    foreach (var item in StudentBooks)
                    {
                        context.StudentBook.Remove(item);

                    }
                }

                var result = await context.SaveChangesAsync();

                return true;


            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task DeleteStudent(int id)
        {
            var student = await context.Student
               .Include(s => s.StudentBooks)
               .ThenInclude(b => b.Book)
               .FirstOrDefaultAsync(m => m.Id == id);

            context.Student.Remove(student);
            await context.SaveChangesAsync();
        }

        public bool CheckIfStudentExist(int id)
        {
            return context.Student.Any(e => e.Id == id);
        }

    }
}

