using Library.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Library.Interfaces
{
    public interface IStudentRepository
    {        
        bool IsStudent(int studentId,int user);
        Task<Student?> GetStudent(string name);
        Task CreateStudent(Student student);
        Task<Student?> GetById(int? id);
        Task<List<Student>> GetAllStudents();
        Task<bool> RemoveBook(int id);
        Task DeleteStudent(int id);
        List<StudentBook> GetStudentBooksById(int id);
        bool CheckIfStudentExist(int id);
    }
}
