using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
namespace TestLibrary.DataAccess
{
    interface IRepository<typeName> where typeName:class
    {
        List<typeName> List();
        List<typeName> ListWhere(Func<typeName, bool> condition);
        void Add(typeName item);
        typeName Find(int index);
        void Remove(typeName item);
        void Remove(List<typeName> removeList);
        void Update(typeName item);
    }
}
