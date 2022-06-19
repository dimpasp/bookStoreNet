#nullable disable
using AutoMapper;
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
    public class AuthorsController : Controller
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;

        public AuthorsController(IMapper mapper, IAuthorRepository authorRepository, IBookRepository bookRepository)
        {
            _mapper = mapper;
            _authorRepository = authorRepository;
            _bookRepository = bookRepository;
        }

        // GET: Authors
        public async Task<IActionResult> Index()
        {
            var authors = await _authorRepository.GetAll();
            var authorsViewModel = _mapper.Map<IEnumerable<AuthorViewModel>>(authors);
            return View(authorsViewModel);
         }

        // GET: Authors/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var author = await _authorRepository.DetailsOfAuthor(id);
            if (author == null) return NotFound();
            var authorViewModel = _mapper.Map<AuthorViewModel>(author);
            return View(authorViewModel);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var authorViewModel = new AuthorViewModel();
            var books = await _bookRepository.GetAllBooks();
            authorViewModel.AuthorBooks = _mapper.Map<List<BookViewModel>>(books);

            return View(authorViewModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AuthorViewModel authorViewModel)
        {
            var author = _mapper.Map<Author>(authorViewModel);

            try
            {
               await _authorRepository.AddAuthor(author);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {

                Console.WriteLine(e);
            }

            return View(author);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            if (id == null)return NotFound();
            var author = await _authorRepository.GetAuthorById(id);
            if (author == null) return NotFound();
            var authorViewModel = _mapper.Map<AuthorViewModel>(author);
            return View(authorViewModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AuthorViewModel authorViewModel)
        {
            var author = _mapper.Map<Author>(authorViewModel);
            if (authorViewModel.Id != author.Id) return NotFound();    
            try
            {
                await _authorRepository.UpdateAuthor(author);
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthorExists(author.Id)) return NotFound();
                else return View(author);      
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var author = await _authorRepository.GetAuthorById(id);
            if (author == null)
            {
                return NotFound();
            }

            var authorViewModel = _mapper.Map<AuthorViewModel>(author);
            return View(authorViewModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _authorRepository.DeleteAuthor(id);
            }
            catch (Exception)
            {

                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool AuthorExists(int id)
        {
            if(_authorRepository.GetAuthorById(id)!=null)
                return true;
            else return false;
        }
    }
}
