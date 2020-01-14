# MusicStore (Teil B)

Dieser Teil ist die Fortführung von Teil A der mehrteiligen Entwicklungsserie 'MusicStore'. In diesem Abschnitt soll das System um asynchrone Methoden erweitert werden.  

## Teilprojekt: MusicStore.Contracts  

### Erweiterung der Schnittstelle 'IControllerAccess\<T\>'  

Die Schnittstelle 'IControllerAccess\<T\>' ist die einzige Möglichkeit wie die Kontroller-Objekte außerhalb vom Assembly kummunizieren. Aus diesem Grund muss diese Schnittstelle um den asynchronen Teil erweitert werden. Um die Übersichtlichkeit zu wahren, erfolgt die Definition der asynchronen Methoden in einer partiellen Klasse. Nähere Informationen zu partielle Klassen finden sich unter [Partielle Klassen und Methoden](https://docs.microsoft.com/de-de/dotnet/csharp/programming-guide/classes-and-structs/partial-classes-and-methods). Im Nachfolgendem ist die Erweiterung der Schnittstelle angegeben:

```csharp ({"Type": "FileRef", "File": "Contracts/Client/IControllerAccessAsync.cs", "StartTag": "//MdStart", "EndTag": "//MdEnd" })
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MusicStore.Contracts.Client
{
    /// <summary>
    /// This interface defines the basic properties and basic operations for accessing the controller.
    /// </summary>
    /// <typeparam name="T">Type, which the basic operations relate.</typeparam>
    public partial interface IControllerAccess<T> : IDisposable
        where T : Contracts.IIdentifiable
    {
        #region Async-Methods
        /// <summary>
        /// Gets the number of quantity in the collection.
        /// </summary>
        /// <returns>Number of entities in the collection.</returns>
        Task<int> CountAsync();
        /// <summary>
        /// Returns all interfaces of the entities in the collection.
        /// </summary>
        /// <returns>All interfaces of the entity collection.</returns>
        Task<IEnumerable<T>> GetAllAsync();
        /// <summary>
        /// Returns the element of type T with the identification of id.
        /// </summary>
        /// <param name="id">The identification.</param>
        /// <returns>The element of the type T with the corresponding identification.</returns>
        Task<T> GetByIdAsync(int id);
        /// <summary>
        /// Creates a new element of type T.
        /// </summary>
        /// <returns>The new element.</returns>
        Task<T> CreateAsync();
        /// <summary>
        /// The entity is being tracked by the context but does not yet exist in the repository. 
        /// </summary>
        /// <param name="entity">The entity which is to be inserted.</param>
        /// <returns>The inserted entity.</returns>
        Task<T> InsertAsync(T entity);
        /// <summary>
        /// The entity is being tracked by the context and exists in the repository, and some or all of its property values have been modified.
        /// </summary>
        /// <param name="entity">The entity which is to be updated.</param>
        Task UpdateAsync(T entity);
        /// <summary>
        /// Removes the entity from the repository with the appropriate identity.
        /// </summary>
        /// <param name="id">The identification.</param>
        Task DeleteAsync(int id);
        /// <summary>
        /// Saves any changes in the underlying persistence.
        /// </summary>
        Task SaveChangesAsync();
        #endregion Async-Methods
    }
}
```  

Als Konvention für asynchrone Methoden wird der Postfix 'Async' und der Rückgabetyp 'Task' empfohlen. Daran ist zu erkennen, dass es sich um eine asynchrone Methode handelt. Diese Konvention soll der Übersichtlichkeit in Programmen dienen und wird von Microsoft vorgegeben und auch praktiziert.  

Aufgrund der geänderten Schnittstellendefinition von 'IControllerAccess\<T\>' ergeben sich zahlreiche Änderungen in der Klassenbibliothek der Logik.  

**Hinweis:** Alle Änderungen, bezüglich der asynchronen Methoden, sind wegen der Übersichtlichkeit in partielle Klassen zusammen gefasst und der Dateiname ist mit dem Postfix 'Asnyc' erweitert.  

## Teilprojekt: MusicStore.Logic  

### Erweiterung der Klasse 'GenericController\<E, I\>'  

Zunächst muss die Klasse 'GenericController\<E, I\>', im Ordner 'Controllers', erweitert werden. Diese Erweiterung ist im nachfolgendem Programmcode dargestellt:

```csharp ({"Type": "FileRef", "File": "Logic/Controllers/GenericControllerAsync.cs", "StartTag": "//MdStart", "EndTag": "//MdEnd" })
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MusicStore.Logic.Controllers
{
    internal abstract partial class GenericController<E, I>
    {
        #region Async-Methods
        public Task<int> CountAsync()
        {
            return Context.CountAsync<I, E>();
        }
        public virtual Task<I> GetByIdAsync(int id)
        {
            return Task.Run<I>(() =>
            {
                var result = default(E);
                var item = Set.SingleOrDefault(i => i.Id == id);

                if (item != null)
                {
                    result = new E();
                    result.CopyProperties(item);
                }
                return result;
            });
        }
        public virtual Task<IEnumerable<I>> GetAllAsync()
        {
            return Task.Run<IEnumerable<I>>(() =>
                Set.Select(i =>
                    {
                        var result = new E();

                        result.CopyProperties(i);
                        return result;
                    }));
        }
        public virtual Task<I> CreateAsync()
        {
            return Task.Run<I>(() => new E());
        }

        protected virtual Task BeforeInsertingAsync(I entity)
        {
            return Task.FromResult(0);
        }
        public virtual async Task<I> InsertAsync(I entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await BeforeInsertingAsync(entity);
            var result = await Context.InsertAsync<I, E>(entity);
            await AfterInsertedAsync(result);
            return result;
        }
        protected virtual Task AfterInsertedAsync(E entity)
        {
            return Task.FromResult(0);
        }

        protected virtual Task BeforeUpdatingAsync(I entity)
        {
            return Task.FromResult(0);
        }
        public virtual async Task UpdateAsync(I entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            await BeforeUpdatingAsync(entity);
            var updateEntity = await Context.UpdateAsync<I, E>(entity);

            if (updateEntity != null)
            {
                await AfterUpdatedAsync(updateEntity);
            }
            else
            {
                throw new Exception("Entity can't find!");
            }
        }
        protected virtual Task AfterUpdatedAsync(E entity)
        {
            return Task.FromResult(0);
        }

        protected virtual Task BeforeDeletingAsync(int id)
        {
            return Task.FromResult(0);
        }
        public async Task DeleteAsync(int id)
        {
            await BeforeDeletingAsync(id);
            var item = await Context.DeleteAsync<I, E>(id);

            if (item != null)
            {
                await AfterDeletedAsync(item);
            }
        }
        protected virtual Task AfterDeletedAsync(E entity)
        {
            return Task.FromResult(0);
        }

        public Task SaveChangesAsync()
        {
            return Context.SaveAsync();
        }
        #endregion Async-Methods
    }
}
```  

Manche Methoden implementieren die asynchrone Methode mit der Task-Bibliothek, und andere Methoden leiten die asynchronen Anfragen an den Kontext weiter. Das bedeutet, dass der Kontext ebenfalls um die entsprechenden  asynchronen Methoden erweitert werden muss. Zuerst erfolgt die Erweiterung der Schnittstelle 'IContext'.

### Erweiterung der 'DataContext'-Komponenten

Zu Beginn muss die Schnittstelle 'IContext' um die asynchronen Metrhoden erweitert werden. Nachfolgend die Erweiterungen von 'IContext':

```csharp ({"Type": "FileRef", "File": "Logic/DataContext/IContextAsync.cs", "StartTag": "//MdStart", "EndTag": "//MdEnd" })
using System;
using System.Threading.Tasks;
using MusicStore.Contracts;
using MusicStore.Logic.Entities;

namespace MusicStore.Logic.DataContext
{
    internal partial interface IContext : IDisposable
    {
        #region Async-Methods
        Task<int> CountAsync<I, E>()
            where I : IIdentifiable
            where E : IdentityObject, I;

        Task<E> CreateAsync<I, E>()
            where I : IIdentifiable
            where E : IdentityObject, ICopyable<I>, I, new();

        Task<E> InsertAsync<I, E>(I entity)
            where I : IIdentifiable
            where E : IdentityObject, ICopyable<I>, I, new();

        Task<E> UpdateAsync<I, E>(I entity)
            where I : IIdentifiable
            where E : IdentityObject, ICopyable<I>, I, new();

        Task<E> DeleteAsync<I, E>(int id)
            where I : IIdentifiable
            where E : IdentityObject, I;

        Task SaveAsync();
        #endregion Async-Methods
    }
}
```  

Die Erweiterung von 'IContext' erfordert eine Erweiterung der Klasse 'ContextObject':

```csharp ({"Type": "FileRef", "File": "Logic/DataContext/ContextObjectAsync.cs", "StartTag": "//MdStart", "EndTag": "//MdEnd" })
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
        public abstract Task<E> InsertAsync<I, E>(I entity)
            where I : IIdentifiable
            where E : IdentityObject, ICopyable<I>, I, new();
        public abstract Task<E> UpdateAsync<I, E>(I entity)
            where I : IIdentifiable
            where E : IdentityObject, I, ICopyable<I>, new();
        public abstract Task<E> DeleteAsync<I, E>(int id)
            where I : IIdentifiable
            where E : IdentityObject, I;
        public abstract Task SaveAsync();
        #endregion Async-Methods
    }
}
```  


Im nächsten Teil (PartC) soll das System um eine Rest-Schnittstelle erweitert werden. Informationen zum Erstellen einer REST-Schnittstelle finden sich unter folgendem Link [Tutorial: Create a web API with ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/tutorials/first-web-api?view=aspnetcore-3.1&tabs=visual-studio).  

**Viel Spaß beim Erweitern!**
