using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TestLibrary.Models;
using TestLibrary.DataAccess;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
namespace TestLibrary.Controllers
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
                        Author = book.Author
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
                              Year = book.Year
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
                              Year = book.Year
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
                           where book.Author.Contains(bookToFind.Author) && book.BookName.Contains(bookToFind.BookName) &&
                                 book.Publisher.Contains(bookToFind.Publisher)
                           select new Book()
                           {
                               BookID = book.BookID,
                               BookName = book.BookName,
                               Author = book.Author,
                               Detail = book.Detail,
                               Publisher = book.Publisher,
                               Year = book.Year
                           };
            }
            else
            {
                try
                {
                bookToFind.Year = int.Parse(target["Year"].ToString());
                list = from book in LibRepo.BookRepo.List()
                       where book.Author.Contains(bookToFind.Author) && book.BookName.Contains(bookToFind.BookName) &&
                                 book.Publisher.Contains(bookToFind.Publisher) && book.Year == bookToFind.Year
                       select new Book()
                       {
                           BookID = book.BookID,
                           BookName = book.BookName,
                           Author = book.Author,
                           Detail = book.Detail,
                           Publisher = book.Publisher,
                           Year = book.Year
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