//@BaseCode
//MdStart
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MusicStore.Contracts;
using MusicStore.Logic.Entities;

namespace MusicStore.Logic.DataContext.Db
{
    partial class DbMusicStoreContext
    {
        #region IContext
        #region Async-Methods
        public Task<int> CountAsync<I, E>()
            where I : IIdentifiable
            where E : IdentityObject, I
        {
            return Set<E>().CountAsync();
        }
        public Task<E> CreateAsync<I, E>()
            where I : IIdentifiable
            where E : IdentityObject, ICopyable<I>, I, new()
        {
            return Task.Run(() => new E());
        }

        public Task<E> InsertAsync<I, E>(I entity)
            where I : IIdentifiable
            where E : IdentityObject, ICopyable<I>, I, new()
        {
            return Task.Run(() =>
            {
                E newEntity = new E();

                newEntity.CopyProperties(entity);
                newEntity.Id = 0;
                try
                {
                    if (Entry(newEntity).State == EntityState.Detached)
                    {
                        Entry(newEntity).State = EntityState.Added;
                    }
                }
                catch (Exception ex)
                {
                    Entry(newEntity).State = EntityState.Detached;
                    throw ex;
                }
                return newEntity;
            });
        }
        public Task<E> UpdateAsync<I, E>(I entity)
            where I : IIdentifiable
            where E : IdentityObject, ICopyable<I>, I, new()
        {
            return Task.Run(() =>
            {
                var updEntity = new E();

                updEntity.CopyProperties(entity);

                var omEntity = Entry(updEntity);

                if (omEntity.State == EntityState.Detached)
                {
                    E attachedEntity = Set<E>().Local.SingleOrDefault(e => e.Id == entity.Id);

                    if (attachedEntity != null)
                    {
                        Entry(attachedEntity).CurrentValues.SetValues(entity);
                        Entry(attachedEntity).State = EntityState.Modified;
                    }
                    else
                    {
                        omEntity.State = EntityState.Modified;
                    }
                }
                else
                {
                    EntityState saveState = omEntity.State;

                    try
                    {
                        Entry(entity).State = EntityState.Modified;
                    }
                    catch
                    {
                        Entry(entity).State = saveState;
                        throw;
                    }
                }
                return omEntity.Entity;
            });
        }
        public Task<E> DeleteAsync<I, E>(int id)
            where I : IIdentifiable
            where E : IdentityObject, I
        {
            return Task.Run(() =>
            {
                E result = Set<E>().SingleOrDefault(i => i.Id == id);

                if (result != null)
                {
                    Set<E>().Remove(result);
                }
                return result;
            });
        }

        public Task SaveAsync()
        {
            return SaveChangesAsync();
        }
        #endregion Async-Methods
        #endregion IContext
    }
}
//MdEnd