﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

#if UITEST
using Xamarin.UITest;
using Xamarin.UITest.Queries;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[NUnit.Framework.Category(UITestCategories.SwipeView)]
#endif
#if APP
	[XamlCompilation(XamlCompilationOptions.Compile)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 11831, "[Bug] SwipeView removes Frame borders on Android", PlatformAffected.Android)]
	public partial class Issue11831 : TestContentPage
	{
		public Issue11831()
		{
#if APP
			Title = "Issue 11831";
			InitializeComponent();

			var random = new Random();
			for (var i = 0; i < 16; i++)
				Data.Add($"Entry #{i + 1} - {random.Next(0, 999999)}");

			DeleteCommand = new Command<string>(input => DisplayAlert("Delete entry", $"Delete: {input}", "OK"));

			BindingContext = this;
#endif
		}

		public ObservableCollection<string> Data { get; } = new ObservableCollection<string>();

		public ICommand DeleteCommand { get; }

		protected override void Init()
		{

		}
	}
}