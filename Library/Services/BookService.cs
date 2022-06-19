using AutoMapper;
using Library.Data;
using Library.Interfaces;
using Library.ViewModels.Author;
using Library.ViewModels.Book;

namespace Library.Services
{
    public class BookService: IBookService
    {
        private IBookRepository _bookRepository;
        private IAuthorRepository _authorRepository;
        private IMapper _mapper;
        public BookService(IBookRepository bookRepository, IMapper mapper, IAuthorRepository authorRepository)
        {
            _bookRepository = bookRepository;
            _authorRepository = authorRepository;
            _mapper = mapper;
        }

        public async Task<BookViewModel> GetBookById(int id)
        {
            var existBook = await _bookRepository.GetById(id);
            var bookViewModel = _mapper.Map<BookViewModel>(existBook);

            return bookViewModel;
        }

        public async Task<BookViewModel> GetEditBookView(int id)
        {
            var bookViewModel = await GetBookById(id);
            var authors = await _authorRepository.GetAll();
            var allAuthorsViewModel = _mapper.Map<List<AuthorViewModel>>(authors);

            foreach (var author in bookViewModel.AuthorBooks)
            {
                var existAuthor = allAuthorsViewModel.FirstOrDefault(x => x.Id == author.Id);
                if (existAuthor != null)
                {
                    existAuthor.IsSelected = true;
                }
            }

            bookViewModel.AuthorBooks = allAuthorsViewModel;

            return bookViewModel;
        }
        public List<RatingPairs> AddStudentBookRating(int id)
        {
            var ratingPares = _bookRepository.GetStudentBooksRatings(id);
            var list = new List<RatingPairs>();

            foreach (var rating in ratingPares)
            {
                RatingPairs ratingPairs = new RatingPairs()
                {
                    Rating = rating.Rating,
                    Comment = rating.Comment
                };
                list.Add(ratingPairs);
            }
            return list;
        }
    }
}
