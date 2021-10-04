using BookLibrary.Models;
using System.Collections.Generic;

namespace BookLibrary.Database
{
    public interface IBookDatabase
    {       
        IEnumerable<BookItem> GetItems();
        
        BookItem GetItem(int id);
        
        int SaveItem(BookItem item);
       
        int DeleteItem(int id);
    }
}