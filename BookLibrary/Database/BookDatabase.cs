using BookLibrary.Models;
using SQLite;
using System.Collections.Generic;

namespace BookLibrary.Database
{
    public class BookDatabase : IBookDatabase
    {
        static object locker = new object();
        static SQLiteConnection conn;
        static BookDatabase database;

        public static BookDatabase Database => database;

        public static void CreateDatabase(SQLiteConnection connection)
        {
            conn = connection;

            conn.CreateTable<BookItem>();
            database = new BookDatabase();
        }

        public IEnumerable<BookItem> GetItems()
        {
            lock (locker)
            {
                return conn.Table<BookItem>().ToList();
            }
        }

        public BookItem GetItem(int id)
        {
            lock (locker)
            {
                return conn.Table<BookItem>().FirstOrDefault(x => x.Id == id);
            }
        }

        public int SaveItem(BookItem item)
        {
            // Set a mutual-exclusive lock on our database, while 
            // saving/updating our book item.
            lock (locker)
            {
                if (item.Id != 0)
                {
                    conn.Update(item);
                    return item.Id;
                }
                else
                {
                    return conn.Insert(item);
                }
            }
        }

        public int DeleteItem(int id)
        {
            lock (locker)
            {
                return conn.Delete<BookItem>(id);
            }
        }
    }
}