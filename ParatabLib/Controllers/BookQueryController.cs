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
using System.Web.Http.Cors;
namespace ParatabLib.Controllers
{
    //This class is all about handle book search API
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class BookQueryController : ApiController
    {
        LibraryRepository LibRepo = new LibraryRepository();
        
        /* Handle method of /api/BookQuery
         * Find all book in database and return booklist as result in HTTPresponse 
         */ 
        public IEnumerable<Book> GetBookQuery()
        {
            return LibRepo.BookRepo.List().Select(
                        book => new Book(){
                        BookID = book.BookID,
                        CallNumber = book.CallNumber,
                        BookName = book.BookName,
                        Author = book.Author,
                        Publisher = book.Publisher,
                        Year = book.Year,
                        BookStatus = book.BookStatus,
                        Detail = book.Detail
                        });
       }

        /* Handle method of /api/BookQuery?name={name}
         * this method use to find related-book by querystring name
         * and return result as HTTPresponse whether it found or not.
         */
        public IHttpActionResult GetBookByName(string name)
        {
            if (name == null)
                return NotFound();
            var target = from book in LibRepo.BookRepo.List()
                          where book.BookName.Contains(name)
                          select new Book(){
                              BookID = book.BookID,
                              CallNumber = book.CallNumber,
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

        /* Handle method of /api/BookQuery?author={author}
         * this method use to find related-book by querystring author
         * and return result as HTTPresponse whether it found or not.
         */
        public IHttpActionResult GetBookByAuthor(string author)
        {
            if (author == null)
                return NotFound();
            var target = from book in LibRepo.BookRepo.List()
                         where StringUtil.IsContains(book.Author, author)
                         select new Book
                         {
                             BookID = book.BookID,
                             CallNumber = book.CallNumber,
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

        /* Handle method of /api/BookQuery?publisher={publisher}
         * this method use to find related-book by querystring publisher
         * and return result as HTTPresponse whether it found or not.
         */
        public IHttpActionResult GetBookByPublisher(string publisher)
        {
            if (publisher == null)
                return NotFound();
            var target = from book in LibRepo.BookRepo.List()
                         where StringUtil.IsContains(book.Publisher, publisher)
                         select new Book
                         {
                             BookID = book.BookID,
                             CallNumber = book.CallNumber,
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

        /* Handle method of /api/BookQuery?year={year}
         * this method use to find related-book by querystring year
         * since year is integer so if querystring is not numeric (e.g. 234Ab56) then it will pass null value instead
         * then this will result in notfound otherwise return normal find result as HTTPresponse 
         * whether it found or not.
         */
        public IHttpActionResult GetBookByYear(int? year)
        {
            if (year == null)
                return NotFound();
            var target = from book in LibRepo.BookRepo.List()
                         where book.Year == year
                         select new Book
                         {
                             BookID = book.BookID,
                             CallNumber = book.CallNumber,
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

        /* Handle method of /api/BookQuery/id
         * this method use to get individual book data by id
         * and return result as HTTPresponse whether it found or not.
         */
        public IHttpActionResult GetBookById(int id)
        {
            Book target = (from book in LibRepo.BookRepo.List()
                          where book.BookID == id
                          select new Book(){
                              BookID = book.BookID,
                              CallNumber = book.CallNumber,
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

        /* Handle method of /api/BookQuery?callno={callno}
         * this method use to get related-book data by call number 
         * and return result as HTTPresponse whether it found or not.
         */
        public IHttpActionResult GetBookByCallNumber(string callno)
        {
            if (callno == null)
                return NotFound();
            var target = from book in LibRepo.BookRepo.List()
                         where book.CallNumber.ToLower().Contains(callno.ToLower())
                         select new Book()
                         {
                             BookID = book.BookID,
                             CallNumber = book.CallNumber,
                             BookName = book.BookName,
                             Author = book.Author,
                             Detail = book.Detail,
                             Publisher = book.Publisher,
                             Year = book.Year,
                             BookStatus = book.BookStatus
                         };

            if (target == null)
                return NotFound();
            else if (target.ToList().Count != 0)
                return Ok(target);
            else
                return NotFound();
        }

        /* Handle method of /api/BookQuery (HTTPPOST)
         * this method use to get related-book data by target data as JSON object
         * in this object it contain data of specifed book that user want to find.
         * Finally return result as HTTPresponse whether it found or not.
         */
        public IHttpActionResult PostBook([FromBody]JObject target)
        {
            Book bookToFind = new Book();
            //Recall individual data on JSON object by use indexing of target parameter
            bookToFind.BookName = target["BookName"].ToString();
            bookToFind.Author = target["Author"].ToString();
            bookToFind.Publisher = target["Publisher"].ToString();
            bookToFind.CallNumber = target["CallNumber"].ToString().ToLower();
            IEnumerable<Book> list;
            //If year is not specfied find book exclude year data since year is numeric type
            if (target["Year"].ToString() == "")
            {
                list = from book in LibRepo.BookRepo.List()
                           where StringUtil.IsContains(book.Author,bookToFind.Author) && book.BookName.Contains(bookToFind.BookName) &&
                                 StringUtil.IsContains(book.Publisher, bookToFind.Publisher) && book.CallNumber.ToLower().Contains(bookToFind.CallNumber)
                           select new Book()
                           {
                               BookID = book.BookID,
                               CallNumber = book.CallNumber,
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
                /* If year is specfied find book include year data
                 * if error occured while parse string to integer return error result(not found)
                 * otherwise return find result as HTTPresponse.
                 */
                  
                bookToFind.Year = int.Parse(target["Year"].ToString());
                list = from book in LibRepo.BookRepo.List()
                       where StringUtil.IsContains(book.Author, bookToFind.Author) && book.BookName.Contains(bookToFind.BookName) &&
                                 StringUtil.IsContains(book.Publisher, bookToFind.Publisher) && book.Year == bookToFind.Year
                                 && book.CallNumber.ToLower().Contains(bookToFind.CallNumber)
                       select new Book()
                       {
                           BookID = book.BookID,
                           CallNumber = book.CallNumber,
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