using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Xamarin.Forms.Controls.GalleryPages
{
	public partial class DataTemplatePageGallery : ContentPage
	{
		public DataTemplatePageGallery()
		{
			InitializeComponent();

			var list = new List<BaseClass>
			{
				new First{ StringValue = "First"},
				new Second{ StringValue = "Second"},
				new Third{ StringValue = "Third"},
				new First{ StringValue = "First"},
				new Second{ StringValue = "Second"},
				new Third{ StringValue = "Third"},
				new First{ StringValue = "First"},
				new Second{ StringValue = "Second"},
				new Third{ StringValue = "Third"},
				new First{ StringValue = "First"},
				new Second{ StringValue = "Second"},
				new Third{ StringValue = "Third"},
				new First{ StringValue = "First"},
				new Second{ StringValue = "Second"},
				new Third{ StringValue = "Third"},
				new First{ StringValue = "First"},
				new Second{ StringValue = "Second"},
				new Third{ StringValue = "Third"},
				new First{ StringValue = "First"},
				new Second{ StringValue = "Second"},
				new Third{ StringValue = "Third"},
			};

			ListViewWithTemplates.ItemsSource = list;
		}



		class BaseClass
		{
			public string StringValue { get; set; }
		}


		class First : BaseClass
		{
			
		}

		class Second : BaseClass
		{
			
		}

		class Third : BaseClass
		{
			
		}

	}
}
