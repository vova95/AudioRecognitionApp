using System.Collections.Generic;

namespace AudioRecognitionApp.DAL.Interfaces
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T Get(int id);
        //IEnumerable<T> Find(Func<T, bool> predicate);
        T Create(T item);
        T Update(T item);
        T Delete(int id);
    }
}
