//@BaseCode
//MdStart
using MusicStore.Contracts;
using MusicStore.Logic.Entities;
using System.Threading.Tasks;

namespace MusicStore.Logic.DataContext
{
    internal abstract partial class ContextObject
    {
        #region Async-Methods
        public abstract Task<int> CountAsync<I, E>()
            where I : IIdentifiable
            where E : IdentityObject, I;
        public abstract Task<E> CreateAsync<I, E>()
            where I : IIdentifiable
            where E : IdentityObject, I, ICopyable<I>, new();
        public abstract Task<E> InsertAsync<I, E>(E entity)
            where I : IIdentifiable
            where E : IdentityObject, ICopyable<I>, I, new();
        public abstract Task<E> UpdateAsync<I, E>(E entity)
            where I : IIdentifiable
            where E : IdentityObject, I, ICopyable<I>, new();
        public abstract Task<E> DeleteAsync<I, E>(int id)
            where I : IIdentifiable
            where E : IdentityObject, I;
        public abstract Task SaveAsync();
        #endregion Async-Methods
    }
}
//MdEnd