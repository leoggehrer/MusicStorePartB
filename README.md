# MusicStore (Teil B)
Ausgangspunkt stellt das Projekt [MusicStore-Teil A](https://github.com/leoggehrer/MusicStorePartA) dar.
### Erweiterung Teil B
In diesen Abschnitt wird der Zugriff auf das System um asynchrone Methoden erweitert. Die asynchronen Methoden haben die gleiche 
Semantik wie die gleichnamigen synchronen Methoden - sie sind eben asynchron!  
Aus dieser Forderung, dass der Zugriff auch asynchron erfolgen sollen, wird die entsprechende Schnittstelle um die Definition der 
entsprechend asynchronen Methoden erweitert. Zu diesem Zweck wird die Schnittstelle 'IControllerAccess' um eine Teilklasse mit dem 
Dateinamen 'IControllerAccessAsync.cs' ergänzt. Es gilt folgende Regel für die Bilung von asynchronen Methoden: Name der Methode mit 
der Endung Async.

```csharp
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


Viel Spaß beim Testen!
