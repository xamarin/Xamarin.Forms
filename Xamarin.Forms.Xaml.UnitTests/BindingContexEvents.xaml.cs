using NUnit.Framework;
using Xamarin.Forms.Core.UnitTests;

namespace Xamarin.Forms.Xaml.UnitTests
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class BindingContexEvents : ContentView
	{
		public BindingContexEvents()
		{
			InitializeComponent();
		}

		class Test: BaseTestFixture
		{
			[Test]
			public void ParentAndChildBindingContextChanged()
			{
				int countPropertyChanged = 0
					, countDataContexChanged = 0
					, countPropertyChanging = 0;


				var parent = new BindingContexEvents();
				var child = parent.Button;
				child.PropertyChanging += (sender, args) => { if (args.PropertyName == "BindingContext") ++countPropertyChanging; };
				child.PropertyChanged += (sender, args) => { if (args.PropertyName == "BindingContext") ++countPropertyChanged; };
				child.BindingContextChanged += (sender, args) => { ++countDataContexChanged; };

				parent.BindingContext = new object();
				Assert.AreEqual(1, countDataContexChanged);
				Assert.AreEqual(1, countPropertyChanging);
				Assert.AreEqual(1, countPropertyChanged);				
			}
		}
	}
}