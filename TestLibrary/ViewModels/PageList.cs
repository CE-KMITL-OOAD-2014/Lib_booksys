using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestLibrary.ViewModels
{
    public class PageList<typeName>
    {
        private int CurrentPage;
        private int PageSize;
        private List<typeName> List;

        public PageList(List<typeName> list){
        this.List = list;
        CurrentPage = 0;
        PageSize = 0;
    }

        public int GetCurrentPage(){
            return CurrentPage;
        }
        public void SetCurrentPage(int page)
        {
            CurrentPage = page;
        }

        public void SetPageSize(int size)
        {
            PageSize = size;
        }

        public int GetPageSize()
        {
            return PageSize;
        }

        public List<typeName> GetList()
        {
            return List;
        }
    }
}