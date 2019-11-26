using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.ControlGallery.Android;
using Xamarin.Forms.Controls.Tests;
using Xamarin.Forms.Internals;

[assembly: Dependency(typeof(PlatformAssemblyRetriever))]
namespace Xamarin.Forms.ControlGallery.Android
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