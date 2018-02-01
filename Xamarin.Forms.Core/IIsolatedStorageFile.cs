using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using FileMode = Xamarin.Forms.Internals.FileMode;
using FileAccess = Xamarin.Forms.Internals.FileAccess;
using FileShare = Xamarin.Forms.Internals.FileShare;

namespace Xamarin.Forms.Internals
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public interface IIsolatedStorageFile
	{
		Task CreateDirectoryAsync(string path);
		Task<bool> GetDirectoryExistsAsync(string path);
		Task<bool> GetFileExistsAsync(string path);

		Task<DateTimeOffset> GetLastWriteTimeAsync(string path);

		Task<Stream> OpenFileAsync(string path, FileMode mode, FileAccess access);
		Task<Stream> OpenFileAsync(string path, FileMode mode, FileAccess access, FileShare share);
	}
}