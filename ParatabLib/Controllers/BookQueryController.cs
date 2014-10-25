using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ParatabLib.Models;
using ParatabLib.DataAccess;
using Newtonsoft.Json.Linq;
using ParatabLib.Utilities;
namespace ParatabLib.Controllers
{
    public class BookQueryController : ApiController
    {
        LibraryRepository LibRepo = new LibraryRepository();
        // GET api/<controller>
        public IEnumerable<Book> GetBookQuery()
        {
            return LibRepo.BookRepo.List().Select(
                        book => new Book(){
                        BookID = book.BookID,
                        BookName = book.BookName,
                        Author = book.Author,
                        Publisher = book.Publisher,
                        Year = book.Year,
                        BookStatus = book.BookStatus,
                        Detail = book.Detail
                        });
       }

        public IHttpActionResult GetBookByName(string name)
        {
            if (name == null)
                return NotFound();
            var target = from book in LibRepo.BookRepo.List()
                          where book.BookName.Contains(name)
                          select new Book(){
                              BookID = book.BookID,
                              BookName = book.BookName,
                              Author = book.Author,
                              Detail = book.Detail,
                              Publisher = book.Publisher,
                              Year = book.Year,
                              BookStatus = book.BookStatus
                            };
            if (target == null)
                return NotFound();
            else if (target.ToList().Count > 0)
                return Ok(target);
            else
                return NotFound();
        }

        public IHttpActionResult GetBookByAuthor(string author)
        {
            if (author == null)
                return NotFound();
            var target = from book in LibRepo.BookRepo.List()
                         where StringUtil.IsContains(book.Author, author)
                         select new Book
                         {
                             BookID = book.BookID,
                             BookName = book.BookName,
                             Author = book.Author,
                             Detail = book.Detail,
                             Publisher = book.Publisher,
                             Year = book.Year,
                             BookStatus = book.BookStatus
                         };
            if (target == null)
                return NotFound();
            else if (target.ToList().Count > 0)
                return Ok(target);
            else
                return NotFound();
        }

        public IHttpActionResult GetBookByPublisher(string publisher)
        {
            if (publisher == null)
                return NotFound();
            var target = from book in LibRepo.BookRepo.List()
                         where StringUtil.IsContains(book.Publisher, publisher)
                         select new Book
                         {
                             BookID = book.BookID,
                             BookName = book.BookName,
                             Author = book.Author,
                             Detail = book.Detail,
                             Publisher = book.Publisher,
                             Year = book.Year,
                             BookStatus = book.BookStatus
                         };
            if (target == null)
                return NotFound();
            else if (target.ToList().Count > 0)
                return Ok(target);
            else
                return NotFound();
        }


        public IHttpActionResult GetBookByYear(int? year)
        {
            if (year == null)
                return NotFound();
            var target = from book in LibRepo.BookRepo.List()
                         where book.Year == year
                         select new Book
                         {
                             BookID = book.BookID,
                             BookName = book.BookName,
                             Author = book.Author,
                             Detail = book.Detail,
                             Publisher = book.Publisher,
                             Year = book.Year,
                             BookStatus = book.BookStatus
                         };
            if (target == null)
                return NotFound();
            else if (target.ToList().Count > 0)
                return Ok(target);
            else
                return NotFound();
        }




        //Get book by ID to view Detail.
        public IHttpActionResult GetBookById(int id)
        {
            Book target = (from book in LibRepo.BookRepo.List()
                          where book.BookID == id
                          select new Book(){
                              BookID = book.BookID,
                              BookName = book.BookName,
                              Author = book.Author,
                              Detail = book.Detail,
                              Publisher = book.Publisher,
                              Year = book.Year,
                              BookStatus = book.BookStatus
                          }).SingleOrDefault();
            if(target!=null)
                return Ok(target);
            return NotFound();
        }

        public IHttpActionResult PostBook([FromBody]JObject target)
        {
            Book bookToFind = new Book();
            bookToFind.BookName = target["BookName"].ToString();
            bookToFind.Author = target["Author"].ToString();
            bookToFind.Publisher = target["Publisher"].ToString();

            IEnumerable<Book> list;
            if (target["Year"].ToString() == "")
            {
                list = from book in LibRepo.BookRepo.List()
                           where StringUtil.IsContains(book.Author,bookToFind.Author) && book.BookName.Contains(bookToFind.BookName) &&
                                 StringUtil.IsContains(book.Publisher, bookToFind.Publisher)
                           select new Book()
                           {
                               BookID = book.BookID,
                               BookName = book.BookName,
                               Author = book.Author,
                               Detail = book.Detail,
                               Publisher = book.Publisher,
                               Year = book.Year,
                               BookStatus = book.BookStatus
                           };
            }
            else
            {
                try
                {
                bookToFind.Year = int.Parse(target["Year"].ToString());
                list = from book in LibRepo.BookRepo.List()
                       where StringUtil.IsContains(book.Author, bookToFind.Author) && book.BookName.Contains(bookToFind.BookName) &&
                                 StringUtil.IsContains(book.Publisher, bookToFind.Publisher) && book.Year == bookToFind.Year
                       select new Book()
                       {
                           BookID = book.BookID,
                           BookName = book.BookName,
                           Author = book.Author,
                           Detail = book.Detail,
                           Publisher = book.Publisher,
                           Year = book.Year,
                           BookStatus = book.BookStatus
                       };
                    }
                    catch(FormatException){   
                        return InternalServerError();
                    }
            }
            if (list == null)
                return NotFound();
           else if (list.ToList().Count > 0)
                return Ok(list);
            else
                return NotFound();
        }

    }
}