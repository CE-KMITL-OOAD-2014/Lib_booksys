using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace ParatabLib.ViewModels
{
    //Declare enumuration of PageListResult
    public enum PageListResult
    {
        Ok,Empty,Error
    }

    //This class use to manage very large list to display into paged list display
    public class PageList<typeName>
    {
        private int CurrentPage;
        private int ListPerPage;
        private bool IsCategorized;
        private List<typeName> List;
        private int PageSize;
        
        /* The constructor of this class is receive 3 parameter:list for desired list to set as paged list,
         * curPage for page that want to view and sizePerPage is number of list per page.Instantiate it with
         * these 3 value.Also set IsCatgorized to false.
         */ 
        public PageList(List<typeName> list,int curPage,int sizePerPage){
        this.List = list;
        CurrentPage = curPage;
        ListPerPage = sizePerPage;
        IsCategorized = false;
    }
        /* This method use to categorized form very large list to paged list.
         * First calculate pageSize by rounding result of List.Count/ListPerPage
         * Second find start index that will use in getRange
         * Third try to get new list from desire index and listPerPage
         * Track result and return PageListResult in final.
         */ 
        public PageListResult Categorized()
        {
            /* Check that whether pagelist object is already categorzied or not 
             * if not do statement in condition.
             */ 
            if (!IsCategorized)
            {
                //Set IsCategorized to true to prevent repeat using.
                IsCategorized = true;
                PageSize = (int)Math.Ceiling((double)List.Count / ListPerPage);
                int index = (CurrentPage - 1) * ListPerPage;

                /* If list size if more than index and size of list is more than index + ListPerPage
                 * (mean that it is not last page).Use getRange to receive new list that categorized.
                 */ 
                if (index < List.Count && ((index + ListPerPage) <= List.Count))
                {
                    List = List.GetRange(index, ListPerPage);
                }
                /* If list size if more than index but size of list is less than index + ListPerPage
                 * (mean that it is last page).Use getRange to receive new list that categorized but 
                 * the second parameter of getRange is come from rational of list.Count divide ListPerPage.
                 */ 
                else if (index < List.Count)
                {
                    List = List.GetRange(index, List.Count % ListPerPage);
                }
                // If list size is equal to 0 return PageListResult.Empty enum value
                else if (List.Count == 0)
                {
                    return PageListResult.Empty;
                }
                /* If index is more than List.Count and index+listPerPage is more than size of list
                 * return PageListResult.Error enum value
                 */ 
                else
                {
                    return PageListResult.Error;
                }
            }
            //Return PageListResult.Ok in success case.
            return PageListResult.Ok;
        }

        public int GetCurrentPage(){
            return CurrentPage;
        }
        public void SetCurrentPage(int page)
        {
            CurrentPage = page;
            IsCategorized = false;
        }

        public void SetListPerPage(int size)
        {
            PageSize = size;
            IsCategorized = false;
        }
        public int GetListPerPage()
        {
            return ListPerPage;
        }
        public int GetPageSize()
        {
            return PageSize;
        }
        public void SetList(List<typeName> list)
        {
            this.List = list;
            IsCategorized = false;
        }
        public List<typeName> GetList()
        {
            return List;
        }

        
    }
}