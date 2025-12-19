using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Library.Data
{
    public interface IRepository<T> where T : class
    {
        // Получить все записи
        IEnumerable<T> GetAll();

        // Получить по ID
        T GetById(int id);

        // Найти по условию
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);

        // Добавить
        void Add(T entity);

        // Удалить
        void Remove(T entity);

        // Обновить
        void Update(T entity);

        // Сохранить изменения
        int SaveChanges();
    }
}