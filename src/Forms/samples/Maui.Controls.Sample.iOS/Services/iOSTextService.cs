using Sample.Services;

namespace Sample.iOS.Services
{
	class iOSTextService : ITextService
	{
		public string GetText() => "Hello From iOS";
	}
}