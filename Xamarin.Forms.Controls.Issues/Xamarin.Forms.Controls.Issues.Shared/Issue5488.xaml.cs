using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 5488, "CarouselView on iOS in XamarinForms 3.6: System.InvalidCastException: Specified cast is not valid.", PlatformAffected.iOS)]
	public partial class Issue5488 : ContentPage
	{
		public Issue5488()
		{
			InitializeComponent();
		}
	}

	public class TestPageViewModel
	{
		public ObservableCollection<User> Users { get; set; }
		public TestPageViewModel()
		{
			Users = new ObservableCollection<User>(Enumerable.Range(1, 10).Select(_ =>
				new User
				{
					Id = _,
					Name = $"User_{_}"
				}
			));
		}
	}

	public class User
	{
		public int Id { get; set; }
		public string Name { get; set; }
	}
}

