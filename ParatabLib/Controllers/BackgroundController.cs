using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ParatabLib.Models;
using ParatabLib.DataAccess;
namespace ParatabLib.Controllers
{
    /* This class use for daily up-to-date database for RequestEntry.
     * if entry has expired that request should be deleted and update
     * status of related book to available.To prevent anonymous user from
     * hack use querystring as secret password for calling method in this class.
     */

    public class BackgroundController : Controller
    {
        LibraryRepository libRepo = new LibraryRepository();

        
        public ActionResult UpdateDatabase(string secretPass)
        {
            if (secretPass == "Dream1357")
            {
                List<RequestEntry> entryToRemove = libRepo.RequestEntryRepo.ListWhere(target => target.ExpireDate != null);
                entryToRemove = entryToRemove.Where(target => target.ExpireDate.Value.Date < DateTime.Now.Date).ToList();
                foreach (RequestEntry entry in entryToRemove)
                {
                    Book requestBook = entry.GetRequestBook(ref libRepo);
                    requestBook.BookStatus = Status.Available;
                    libRepo.BookRepo.Update(requestBook);
                }
                libRepo.RequestEntryRepo.Remove(entryToRemove);
                libRepo.Save();
                return View();
            }
            return HttpNotFound();
        }
    }
}
