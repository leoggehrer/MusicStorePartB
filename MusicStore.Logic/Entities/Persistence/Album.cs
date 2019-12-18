//@CodeCopy
//MdStart
using System;
using System.Collections.Generic;

namespace MusicStore.Logic.Entities.Persistence
{
    /// <summary>
    /// Implements the properties and methods of album model.
    /// </summary>
    [Serializable]
    partial class Album : IdentityObject, Contracts.Persistence.IAlbum
    {
        public int ArtistId { get; set; }
        public string Title { get; set; }

        public void CopyProperties(Contracts.Persistence.IAlbum other)
        {
            if (other == null)
                throw new ArgumentNullException(nameof(other));

            Id = other.Id;
            ArtistId = other.ArtistId;
            Title = other.Title;
        }
		public Artist Artist { get; set; }
		public IEnumerable<Track> Tracks { get; set; }
    }
}
//MdEnd
