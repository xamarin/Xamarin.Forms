using System.Threading.Tasks;
using NUnit.Framework.Interfaces;

namespace Xamarin.Forms.Controls
{
	interface IPlatformTestRunner
	{
		Task<ITestResult> Run(ITestFilter testFilter = null);
	}
}
