//@BaseCode
//MdStart
using System.Linq;
using System.Threading.Tasks;
using CommonBase.Extensions;

namespace MusicStore.Logic.DataContext
{
    abstract partial class MusicStoreFileContext : FileContext, IMusicStoreContext
    {
        #region Async-Methods
        // Falls die synchronen Methoden entfernt werden soll,
        // dann werden diese private spezifiziert und aus dem 
        // Interface entfernt.
        public override Task<int> CountAsync<I, E>()
        {
            return Task.Run(() => Set<I, E>().Count);
        }
        public override Task<E> CreateAsync<I, E>()
        {
            return Task.Run(() => new E());
        }
        public override Task<E> InsertAsync<I, E>(E entity)
        {
            entity.CheckArgument(nameof(entity));

            return Task.Run(() =>
            {
                Set<I, E>().Add(entity);
                return entity;
            });
        }
        public override Task<E> UpdateAsync<I, E>(E entity)
        {
            entity.CheckArgument(nameof(entity));

            return Task.Run(() =>
            {
                E result = Set<I, E>().SingleOrDefault(i => i.Id == entity.Id);

                if (result != null)
                {
                    result.CopyProperties(entity);
                }
                return result;
            });
        }
        public override Task<E> DeleteAsync<I, E>(int id)
        {
            return Task.Run(() =>
            {
                E result = Set<I, E>().SingleOrDefault(i => i.Id == id);

                if (result != null)
                {
                    Set<I, E>().Remove(result);
                }
                return result;
            });
        }
        #endregion Async-Methods
    }
}
//MdEnd