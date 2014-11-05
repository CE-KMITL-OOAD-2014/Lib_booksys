using System;
using System.Collections.Generic;
using System.Linq;
namespace ParatabLib.DataAccess
{
    public interface IGenericRepository<typeName> where typeName:class
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
