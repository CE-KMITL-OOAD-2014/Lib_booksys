using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ParatabLib.Models;
namespace ParatabLib.ViewModels
{
    public class MemberTransactionViewer
    {
        private List<BorrowEntry> BorrowEntryViews = new List<BorrowEntry>();
        private List<RequestEntry> RequestEntryViews = new List<RequestEntry>();

        public void SetBorrowEntryViews(List<BorrowEntry> viewerEntry){
            try{
            this.BorrowEntryViews = viewerEntry;
            }
            catch(Exception ex){
                throw ex;
            }
        }

        public List<BorrowEntry> GetBorrowEntryViews()
        {
            return this.BorrowEntryViews;
        }

        public void SetRequestEntryViews(List<RequestEntry> viewerEntry){
            try{
                this.RequestEntryViews = viewerEntry;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<RequestEntry> GetRequestEntryViews()
        {
            return this.RequestEntryViews;
        }
    }
}