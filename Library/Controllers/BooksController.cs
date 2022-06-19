#nullable disable
using AutoMapper;
using Library.Data;
using Library.Entities;
using Library.Interfaces;
using Library.ViewModels.Author;
using Library.ViewModels.Book;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.Controllers
{
    [Authorize]
    public class BooksController : Controller
    {
        private readonly LibraryContext _context;
        private readonly IMapper _mapper;
        private readonly IBookRepository _bookRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IBookService _bookService;
        private readonly IAuthorRepository _authorRepository;

        [BindProperty(SupportsGet = true)]
        public string SearchTerm { get; set; }
        public BooksController(LibraryContext context, IMapper mapper, IAuthorRepository authorRepository, IBookRepository bookRepository, IStudentRepository studentRepository, IBookService bookService)
        {
            _context = context;
            _mapper = mapper;
            _bookRepository = bookRepository;
            _studentRepository = studentRepository;
            _bookService = bookService;
            _authorRepository = authorRepository;
        }

        // GET: Books
        public async Task<IActionResult> Index(string sortName)
        {
            var books = _bookRepository.Search(SearchTerm);
            var booksViewModel = _mapper.Map<IEnumerable<BookViewModel>>(books);


            var student = await _studentRepository.GetStudent(User.Identity.Name);
            foreach (var item in booksViewModel)
            {

                if (student.StudentBooks.FirstOrDefault(x => x.BookId == item.Id) is StudentBook)
                {
                    item.BooksExists = true;
                }

                var HowManyTimesExist = _bookRepository.HowManyTimesBookExist(item.Id);
                item.availableBooks = item.Amount - HowManyTimesExist;
                item.Rating = await _bookRepository.GetRatingOfBook(item.Id);
                if (item.Rating > 0)
                {
                    item.Rating = (float)(Math.Round((double)item.Rating, 2));
                }
            }
            ViewData["TitleSortTable"] = String.IsNullOrWhiteSpace(sortName) ? "nameDesc" : "";
            ViewData["PriceSortTable"] = sortName == "Price" ? "Price_desc" : "Price";
            ViewData["RatingSortTable"] = sortName == "Rating" ? "Rating_desc" : "Rating";

            switch (sortName)
            {
                case "nameDesc":
                    booksViewModel = booksViewModel.OrderByDescending(x => x.Title);
                    break;
                case "Price":
                    booksViewModel = booksViewModel.OrderBy(x => x.Price);
                    break;
                case "Price_desc":
                    booksViewModel = booksViewModel.OrderByDescending(x => x.Price);
                    break;
                case "Rating":
                    booksViewModel = booksViewModel.OrderBy(x => x.Rating);
                    break;
                case "Rating_desc":
                    booksViewModel = booksViewModel.OrderByDescending(x => x.Rating);
                    break;
                default:
                    booksViewModel = booksViewModel.OrderBy(x => x.Title);
                    break;
            }
            return View(booksViewModel);
        }
        public async Task<IActionResult> AddBookToStudent(int id)
        {

            var student = await _studentRepository.GetStudent(User.Identity.Name);
            var result = await _bookRepository.AddBook(id, student);
            if (result == true) return RedirectToAction(nameof(Index));
            else return BadRequest();

        }
        public async Task<IActionResult> DeleteBookFromStudent(int id)
        {
            var student = await _studentRepository.GetStudent(User.Identity.Name);
            var result = await _bookRepository.RemoveBook(id, student);
            if (result == true) return RedirectToAction(nameof(Index));
            else return BadRequest();

        }
        // GET: Books/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var book = await _bookRepository.GetById(id);

            var bookViewModel = _mapper.Map<BookViewModel>(book);
            var authors = book.AuthorBooks.Select(x => x.Author).ToList();
            bookViewModel.AuthorBooks = _mapper.Map<List<AuthorViewModel>>(authors);
            return View(bookViewModel);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var bookViewModel = new BookViewModel();
            var authors = _authorRepository.GetAll();
            bookViewModel.AuthorBooks = _mapper.Map<List<AuthorViewModel>>(authors);

            return View(bookViewModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookViewModel bookViewModel)
        {
            try
            {
                var book = _mapper.Map<Book>(bookViewModel);
                await _bookRepository.CreateBook(book);
                return RedirectToAction(nameof(Index));

            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return View(bookViewModel);
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var bookViewModel = await _bookService.GetEditBookView(id);
            return View(bookViewModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BookViewModel bookViewModel)
        {
            var old = await _bookRepository.GetById(bookViewModel.Id);

            _mapper.Map(bookViewModel, old);

            try
            {
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthorExists(bookViewModel.Id))
                {
                    return NotFound();
                }
                return View(bookViewModel);
            }
        }

        [BindProperty]
        public List<int> AreChecked { get; set; }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var book = await _bookRepository.GetById(id);

            var bookViewModel = _mapper.Map<BookViewModel>(book);

            return View(bookViewModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _bookRepository.DeleteBook(id);
            if (result == true) return RedirectToAction(nameof(Index));
            else return BadRequest();

        }

        private bool AuthorExists(int id)
        {
            if (_authorRepository.GetAuthorById(id) != null)
                return true;
            else return false;
        }

        //Get
        public async Task<IActionResult> AddRating(int id)
        {
            var book = await _bookRepository.GetById(id);
            var bookViewModel = _mapper.Map<BookViewModel>(book);
            ViewBag.RatingStudentBook = _bookService.AddStudentBookRating(id);
            return View(bookViewModel);
        }

        //post
        [HttpPost]
        public async Task<IActionResult> AddRating(BookViewModel bookViewModel)
        {
            var student = await _studentRepository.GetStudent(User.Identity.Name);
            if (student == null)
                return BadRequest($"Can't find student with Name: {User.Identity.Name}");

            switch (bookViewModel.ratingSystemValues.ToString())
            {
                case "Penniless":
                    bookViewModel.Rating = 1;
                    break;

                case "Poor":
                    bookViewModel.Rating = 2;
                    break;

                case "Fair":
                    bookViewModel.Rating = 3;
                    break;

                case "Good":
                    bookViewModel.Rating = 4;
                    break;

                case "Excellent":
                    bookViewModel.Rating = 5;
                    break;

                default:
                    break;
            }
            try
            {
                var ratingStudentBook = new RatingStudentBook { BookId = bookViewModel.Id, StudentId = student.Id, Comment = bookViewModel.Comment, Rating = bookViewModel.Rating };

                var checkIfUserHaveRate = await _bookRepository.checkIfUserHaveRate(student.Id, bookViewModel.Id);
                if (checkIfUserHaveRate == null) await _bookRepository.CreateRatingStudentBook(ratingStudentBook);
                else
                {
                    checkIfUserHaveRate.Rating = bookViewModel.Rating;
                    checkIfUserHaveRate.Comment = bookViewModel.Comment;
                    await _bookRepository.UpdateRatingStudentBook(checkIfUserHaveRate);
                }
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                return BadRequest("Invalid operation");
            }

        }

    }
}
