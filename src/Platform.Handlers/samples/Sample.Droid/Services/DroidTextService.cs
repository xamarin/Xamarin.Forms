using Sample.Services;

namespace Sample.Droid.Services
{
	public class DroidTextService : ITextService
	{
		public string GetText() => "Hello From Android";
	}
}