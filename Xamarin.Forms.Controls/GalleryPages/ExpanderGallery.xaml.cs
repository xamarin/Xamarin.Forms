using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Xamarin.Forms.Controls
{
	public partial class ExpanderGallery : ContentPage
	{
		public ExpanderGallery()
		{
			InitializeComponent();
		}

		public string[] Items { get; } = new string[]
		{
			"The First",
			"The Second",
			"The Third",
			"The Fourth",
			"The Fifth"
		};
	}
}
