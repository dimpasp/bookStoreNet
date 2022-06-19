using AutoMapper;
using Library.Data;
using Library.Entities;
using Library.Interfaces;
using Library.ViewModels.Book;
using Library.ViewModels.Store;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library.Controllers
{
    [Authorize(Roles = "Admin,Student")]
    public class StoreController : Controller
    {
        private readonly LibraryContext _context;
        private readonly IStoreRepository _storeRepository;
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;

        public StoreController(LibraryContext context, IMapper mapper,IStoreRepository storeRepository, IBookRepository bookRepository)
        {
            _mapper = mapper;
            _context = context;
            _storeRepository = storeRepository;
            _bookRepository = bookRepository;
        }
        public async Task<IActionResult> Index(string sortName)
        {
            var stores = await _storeRepository.GetAll();
            var storeViewModel = _mapper.Map<IEnumerable<StoreViewModel>>(stores);
            ViewData["CitySortTable"] = String.IsNullOrWhiteSpace(sortName) ? "City" : "";
            switch (sortName)
            {
                case "City":
                    storeViewModel = storeViewModel.OrderByDescending(x => x.City);
                    break;
                default:
                    storeViewModel = storeViewModel.OrderBy(x => x.City);
                    break;
            }
            return View(storeViewModel);
        }
        public async Task<IActionResult> Details(int id)
        {
            var store = await _storeRepository.GetStoreById(id);
            if (store == null) return NotFound();          
            var storeViewModel = _mapper.Map<StoreViewModel>(store);
            var books = store.StoreBooks?.Select(x => x.Book).ToList();
            storeViewModel.StoreBooks = _mapper.Map<List<BookViewModel>>(books);
            return View(storeViewModel);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            var storeViewModel = new StoreViewModel();
            var books = await _bookRepository.GetAllBooks();
            storeViewModel.StoreBooks = _mapper.Map<List<BookViewModel>>(books);

            return View(storeViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(StoreViewModel storeViewModel)
        {
            var store = _mapper.Map<Store>(storeViewModel);

            if (ModelState.IsValid)
            {
                await _storeRepository.AddStore(store);
                return RedirectToAction(nameof(Index));
            }
            return View(storeViewModel);
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var store = await _storeRepository.GetStoreById(id);

            var storeViewModel = _mapper.Map<StoreViewModel>(store);


            var books = await _bookRepository.GetAllBooks();
            var allBooksViewModel = _mapper.Map<List<BookViewModel>>(books);
            if (storeViewModel.StoreBooks != null)
            {
                foreach (var book in storeViewModel.StoreBooks)
                {
                    var existBook = allBooksViewModel.FirstOrDefault(x => x.Id == book.Id);
                    if (existBook != null)
                    {
                        existBook.IsSelected = true;
                    }
                }
            }

            storeViewModel.StoreBooks = allBooksViewModel;
            return View(storeViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(StoreViewModel storeViewModel)
        {
            var old = await _storeRepository.GetStoreById(storeViewModel.Id);

            _mapper.Map(storeViewModel, old);

            try
            {
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StoreExists(storeViewModel.Id))
                {
                    return NotFound();
                }


                return View(storeViewModel);
            }
        }
        public bool StoreExists(int id)
        {
            if (_storeRepository.GetStoreById(id) != null) return true;
            else return false;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var store = await _storeRepository.GetStoreById(id);
            if (store == null)
            {
                return NotFound();
            }
          
            var storeViewModel = _mapper.Map<StoreViewModel>(store);
            return View(storeViewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var store = await _storeRepository.GetStoreById(id);
            if (store!=null) await _storeRepository.DeleteStore(store);
            return RedirectToAction(nameof(Index));
        }
    }
}
