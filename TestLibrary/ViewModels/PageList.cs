using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace TestLibrary.ViewModels
{
    public enum PageListResult
    {
        Ok,Empty,Error
    }
    public class PageList<typeName>
    {
        private int CurrentPage;
        private int ListPerPage;
        private bool IsCategorized;
        private List<typeName> List;
        private int PageSize;
        public PageList(List<typeName> list,int curPage,int size){
        this.List = list;
        CurrentPage = curPage;
        ListPerPage = size;
        IsCategorized = false;
    }
        public PageListResult Categorized()
        {
            if (!IsCategorized)
            {
                IsCategorized = true;
                PageSize = (int)Math.Ceiling((double)List.Count / ListPerPage);
                int index = (CurrentPage - 1) * ListPerPage;
                if (index < List.Count && ((index + ListPerPage) <= List.Count))
                {
                    List = List.GetRange((CurrentPage - 1) * ListPerPage, ListPerPage);
                }
                else if (index < List.Count)
                {
                    List = List.GetRange((CurrentPage - 1) * ListPerPage, List.Count % ListPerPage);
                }
                else if (List.Count == 0)
                {
                    return PageListResult.Empty;
                }
                else
                {
                    return PageListResult.Error;
                }
            }
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