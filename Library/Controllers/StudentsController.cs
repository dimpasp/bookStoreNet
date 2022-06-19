#nullable disable
using AutoMapper;
using Library.Data;
using Library.Entities;
using Library.Interfaces;
using Library.ViewModels.Book;
using Library.ViewModels.Student;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.Controllers
{
    [Authorize]
    public class StudentsController : Controller
    {
        private readonly LibraryContext _context;
        private readonly IMapper _mapper;
        private readonly IStudentRepository _studentRepository;
        private readonly IBookRepository _bookRepository;
        private readonly UserManager<ApplicationUser> userManager;
        public StudentsController(LibraryContext context,IMapper mapper,IStudentRepository studentRepository,IBookRepository bookRepository, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _bookRepository = bookRepository;
            _studentRepository = studentRepository;
            this.userManager = userManager;
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
            var students = await _studentRepository.GetAllStudents();

            var studentsViewModel = _mapper.Map<IEnumerable<StudentViewModel>>(students);
            return View(studentsViewModel);
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _studentRepository.GetById(id);   

            var studentsViewModel = _mapper.Map<StudentViewModel>(student);
            var books = student.StudentBooks.Select(x => x.Book).ToList();
            studentsViewModel.StudentBooks = _mapper.Map<List<BookViewModel>>(books);
            return View(studentsViewModel);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var studentViewModel = new StudentViewModel();
            var books = await _bookRepository.GetAllBooks();
            studentViewModel.StudentBooks = _mapper.Map<List<BookViewModel>>(books);

            return View(studentViewModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StudentViewModel studentViewModel)
        {
            try
            {
                var student = _mapper.Map<Student>(studentViewModel);
                student.CreatedDate=DateTime.Now;
                student.Gender = studentViewModel.studentGender.ToString();
                await _studentRepository.CreateStudent(student);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return View(studentViewModel);
            }
        }

        //Get
        [Authorize("MustBeOwner")]
        public async Task<IActionResult> Edit(int id)
        {
            var student = await _studentRepository.GetById(id);

            var studentsViewModel = _mapper.Map<StudentViewModel>(student);


            var books = await _bookRepository.GetAllBooks();
            var allBooksViewModel = _mapper.Map<List<BookViewModel>>(books);

            foreach (var book in studentsViewModel.StudentBooks)
            {
                var existBook = allBooksViewModel.FirstOrDefault(x => x.Id == book.BookId);
                if (existBook != null)
                {
                    existBook.IsSelected = true;
                }
            }

            List<int> availableBooks = new List<int>();
            foreach (var book in books)
            {
                var HowManyTimesExist = _bookRepository.HowManyTimesBookExist(book.Id);
                var availablebook = book.Amount - HowManyTimesExist;

                availableBooks.Add(availablebook);
            }
            studentsViewModel.availableBooks = availableBooks;

            studentsViewModel.StudentBooks = allBooksViewModel;
            return View(studentsViewModel);
        }



        // POST: Students/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StudentViewModel studentViewModel)
        {
            var old = await _studentRepository.GetById(studentViewModel.Id);
            
            _mapper.Map(studentViewModel, old);

            //if uncheck button then remove book from user
            var books = await _bookRepository.GetAllBooks();
            var allBooksViewModel = _mapper.Map<List<BookViewModel>>(books);

            foreach (var book in studentViewModel.StudentBooks)
            {
                var existBook = allBooksViewModel.FirstOrDefault(x => x.Id == book.BookId);
                if (existBook != null)
                {
                    if (book.IsSelected != true)
                    {
                        studentViewModel.StudentBooks.Remove(book);
                    }
                }
            }

            try
            {
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(studentViewModel.Id))
                {
                    return NotFound();
                }
                return View(studentViewModel);
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _studentRepository.GetById(id);
            if (student == null)
            {
                return NotFound();
            }

            await _studentRepository.RemoveBook(id);
            

            var studentViewModel = _mapper.Map<StudentViewModel>(student);
            return View(studentViewModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
           await _studentRepository.DeleteStudent(id);
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            var Check = _studentRepository.CheckIfStudentExist(id);
            return Check;
        }
    }
}
