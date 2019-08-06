using System.Collections.Generic;

namespace Xamarin.Forms.Controls.GalleryPages
{
	public partial class DataTemplatePageGallery : ContentPage
	{
		public DataTemplatePageGallery()
		{
			InitializeComponent();

			var list = new List<BaseTestClass>
			{
				new FirstTestClass { StringValue = "First" },
				new SecondTestClass { StringValue = "Second" },
				new ThirdTestClass { StringValue = "Third" },
				new FirstTestClass { StringValue = "First" },
				new SecondTestClass { StringValue = "Second" },
				new ThirdTestClass { StringValue = "Third" },
				new FirstTestClass { StringValue = "First" },
				new SecondTestClass { StringValue = "Second" },
				new ThirdTestClass { StringValue = "Third" },
				new FirstTestClass { StringValue = "First" },
				new SecondTestClass { StringValue = "Second" },
				new ThirdTestClass { StringValue = "Third" },
				new FirstTestClass { StringValue = "First" },
				new SecondTestClass { StringValue = "Second" },
				new ThirdTestClass { StringValue = "Third" },
				new FirstTestClass { StringValue = "First" },
				new SecondTestClass { StringValue = "Second" },
				new ThirdTestClass { StringValue = "Third" },
				new FirstTestClass { StringValue = "First" },
				new SecondTestClass { StringValue = "Second" },
				new ThirdTestClass { StringValue = "Third" }
			};

			ListViewWithTemplates.ItemsSource = list;
		}
	}

	public class BaseTestClass
	{
		public string StringValue { get; set; }
	}

	public class FirstTestClass : BaseTestClass
	{
	}

	public class SecondTestClass : BaseTestClass
	{
	}

	public class ThirdTestClass : BaseTestClass
	{
	}
}