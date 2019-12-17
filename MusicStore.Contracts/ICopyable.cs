//@CodeCopy
//MdStart
namespace MusicStore.Contracts
{
	public partial interface ICopyable<T>
	{
		void CopyProperties(T other);
	}
}
//MdEnd
