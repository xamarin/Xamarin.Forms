using System.Security.Cryptography;

namespace Xamarin.Forms.Platform.WPF
{
	internal abstract class MD5 : HashAlgorithm
	{
		public MD5()
		{
			HashSizeValue = 128;
		}
	}
}