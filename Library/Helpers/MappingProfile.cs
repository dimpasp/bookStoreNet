using AutoMapper;
using Library.Entities;
using Library.ViewModels;
using Library.ViewModels.Author;
using Library.ViewModels.Book;
using Library.ViewModels.Identity;
using Library.ViewModels.Store;
using Library.ViewModels.Student;

namespace Library.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            AuthorMapping();
            BookMapping();
            StudentMapping();
            StoreMapping();
        }
        private void AuthorMapping()
        {
            this.CreateMap<Author, AuthorViewModel>().ReverseMap();

            this.CreateMap<AuthorViewModel, Author>()
                   .ForMember(author => author.AuthorBooks, authorviewmodels => authorviewmodels.MapFrom((authorviewmodels, author,i, context) =>
                   {
                       var selected = authorviewmodels.AuthorBooks?.Where(x => x.IsSelected).Select(x => new AuthorBook()
                       {
                           BookId = x.Id
                       });
                       return selected;
                   })).ReverseMap();

            this.CreateMap<AuthorBook, BookViewModel>()
                .ForMember(bookViewModel => bookViewModel.Id, authorBook => authorBook.MapFrom((authorBook, bookViewModel,i, context) =>
                {
                    return authorBook.Book?.Id;
                }))
                .ForMember(bookViewModel => bookViewModel.Title, authorBook => authorBook.MapFrom((authorBook, bookViewModel, i, context) =>
                {
                    return authorBook.Book?.Title;
                }))
                .ForMember(bookViewModel => bookViewModel.Pages, authorBook => authorBook.MapFrom((authorBook, bookViewModel, i, context) =>
                {
                    return authorBook.Book?.Pages;
                }));


            this.CreateMap<AuthorViewModel, AuthorBook>()
                .ForMember(authorBook => authorBook.AuthorId, authorViewModel => authorViewModel.MapFrom((authorViewModel, authorBook, i, context) =>
                {
                    return authorViewModel.Id;
                }))
                .ForMember(authorBook => authorBook.Id, authorViewModel => authorViewModel.Ignore())
                .ForMember(authorBook => authorBook.BookId, authorViewModel => authorViewModel.Ignore());

            this.CreateMap<AuthorBook, AuthorViewModel>()
                .ForMember(authorViewModel => authorViewModel.Id, authorBook => authorBook.MapFrom((authorBook, authorViewModel, i, context) =>
                {
                    return authorBook.Author?.Id;
                }))
                .ForMember(authorViewModel => authorViewModel.Name, authorBook => authorBook.MapFrom((authorBook, authorViewModel, i, context) =>
                {
                    return authorBook.Author?.Name;
                }))
                .ForMember(authorViewModel => authorViewModel.IsSelected, authorViewModel => authorViewModel.Ignore());
        }

        private void BookMapping()
        {

            this.CreateMap<Book, BookViewModel>();

            this.CreateMap<BookViewModel, Book>()
                        .ForMember(book => book.AuthorBooks, bookViewModel => bookViewModel.MapFrom((bookViewModel, book, i, context) =>
                        {
                            var selected = bookViewModel.AuthorBooks?.Where(x => x.IsSelected).Select(x => new AuthorBook()
                            {
                                AuthorId = x.Id
                            });
                            return selected;
                        }));
        }
        private void StudentMapping()
        {

            this.CreateMap<Student, StudentViewModel>();

            this.CreateMap<StudentViewModel, Student>()
                        .ForMember(student => student.StudentBooks, studentViewModel => studentViewModel.MapFrom((studentViewModel, student, i, context) =>
                        {
                            var selected = studentViewModel.StudentBooks?.Where(x => x.IsSelected).Select(x => new StudentBook()
                            {
                               BookId = x.Id
                            });
                            return selected;
                        }))
                        .ReverseMap();

            this.CreateMap<StudentBook, BookViewModel>()
                .ForMember(bookViewModel => bookViewModel.Id, studentBook => studentBook.MapFrom((studentBook, bookViewModel, i, context) =>
                {
                    return studentBook.Id;
                }))
                .ForMember(bookViewModel => bookViewModel.Title, studentBook => studentBook.MapFrom((studentBook, bookViewModel, i, context) =>
                {
                    return studentBook.Book?.Title;
                }))
                .ForMember(bookViewModel => bookViewModel.Pages, studentBook => studentBook.MapFrom((studentBook, bookViewModel, i, context) =>
                {
                    return studentBook.Book?.Pages;
                }))
                .ReverseMap();


            this.CreateMap<StudentViewModel, StudentBook>()
                .ForMember(studentBook => studentBook.BookId, studentViewModel => studentViewModel.MapFrom((studentViewModel, studentBook, i, context) =>
                {
                    return studentViewModel.Id;
                }))
                .ForMember(studentBook => studentBook.Id, studentViewModel => studentViewModel.Ignore())
                .ForMember(studentBook => studentBook.BookId, studentViewModel => studentViewModel.Ignore());

            this.CreateMap<StudentBook, StudentViewModel>()
                .ForMember(studentViewModel => studentViewModel.Id, studentBook => studentBook.MapFrom((studentBook, studentViewModel, i, context) =>
                {
                    return studentBook.Book?.Id;
                }))
                .ForMember(studentViewModel => studentViewModel.Name, studentBook => studentBook.MapFrom((studentBook, studentViewModel, i, context) =>
                {
                    return studentBook.Book?.Title;
                }))
                .ForMember(studentViewModel => studentViewModel.IsSelected, studentViewModel => studentViewModel.Ignore());

        }
        private void StoreMapping()
        {
            this.CreateMap<Store, StoreViewModel>();

            this.CreateMap<StoreViewModel, Store>()
                        .ForMember(store => store.StoreBooks, storeViewModel => storeViewModel.MapFrom((storeViewModel, store, i, context) =>
                        {
                            var selected = storeViewModel.StoreBooks?.Where(x => x.IsSelected).Select(x => new StoreBook()
                            {
                                BookId = x.Id
                            });
                            return selected;
                        })).ReverseMap();


            this.CreateMap<StoreViewModel, StoreBook>()
                        .ForMember(storebook => storebook.BookId, storeViewModel => storeViewModel.MapFrom((storeViewModel, storebook, i, context) =>
                        {
                            return storeViewModel.Id;
                        }))
                        .ForMember(storebook => storebook.Id, storeViewModel => storeViewModel.Ignore())
                        .ForMember(storebook => storebook.BookId, storeViewModel => storeViewModel.Ignore());


            this.CreateMap<StoreBook, StoreViewModel>()
            .ForMember(storeViewModel => storeViewModel.Id, storebook => storebook.MapFrom((storebook, storeViewModel, i, context) =>
            {
                return storebook.Book?.Id;
            }))
            .ForMember(storeViewModel => storeViewModel.Name, storebook => storebook.MapFrom((storebook, storeViewModel, i, context) =>
            {
                return storebook.Book?.Title;
            }))
            .ForMember(storeViewModel => storeViewModel.IsSelected, storeViewModel => storeViewModel.Ignore());


            this.CreateMap<StoreBook, BookViewModel>()
            .ForMember(bookViewModel => bookViewModel.Id, storebook => storebook.MapFrom((storebook, bookViewModel, i, context) =>
            {
                return storebook.Book?.Id;
            }))
            .ForMember(bookViewModel => bookViewModel.Title, storebook => storebook.MapFrom((storebook, bookViewModel, i, context) =>
            {
                return storebook.Book?.Title;
            }))
            .ForMember(bookViewModel => bookViewModel.Pages, storebook => storebook.MapFrom((storebook, bookViewModel, i, context) =>
            {
                return storebook.Book?.Pages;
            }))
            .ReverseMap();
        }
    }
}
