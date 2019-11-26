using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.ControlGallery.iOS.Tests;
using Xamarin.Forms.Controls.Tests;
using Xamarin.Forms.Internals;

[assembly: Dependency(typeof(PlatformAssemblyRetriever))]
namespace Xamarin.Forms.ControlGallery.iOS.Tests
{
	[Preserve(AllMembers = true)]
	public class PlatformAssemblyRetriever : IAssemblyRetriever
	{
		public Assembly GetAssembly()
		{
			return Assembly.GetExecutingAssembly();
		}
	}
}