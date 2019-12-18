//@BaseCode
//MdStart
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
            return Task.Run(() => Count<I, E>());
        }
        public override Task<E> CreateAsync<I, E>()
        {
            return Task.Run(() => Create<I, E>());
        }
        public override Task<E> InsertAsync<I, E>(I entity)
        {
            entity.CheckArgument(nameof(entity));

			return Task.Run(() => Insert<I, E>(entity));
        }
        public override Task<E> UpdateAsync<I, E>(I entity)
        {
            entity.CheckArgument(nameof(entity));

			return Task.Run(() => Update<I, E>(entity));
        }
        public override Task<E> DeleteAsync<I, E>(int id)
        {
			return Task.Run(() => Delete<I, E>(id));
        }
        #endregion Async-Methods
    }
}
//MdEnd