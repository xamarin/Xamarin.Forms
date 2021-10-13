using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 13577,
		"[Bug][DatePicker][XF5] DatePicker empty format now invalid",
		PlatformAffected.iOS)]
	public partial class Issue13577 : TestContentPage
	{
		public Issue13577()
		{
#if APP
			InitializeComponent();
#endif
		}

		protected override void Init()
		{
			BindingContext = new Issue13577ViewModel();
		}
	}

	public class Issue13577ViewModel
	{
		public Issue13577ViewModel()
		{
			NullableDate = null;
		}

		public DateTime? NullableDate { get; set; }
	}

	public class NullableDatePicker : Xamarin.Forms.DatePicker
	{
		public static readonly BindableProperty NullableDateProperty = BindableProperty.Create("NullableDate", typeof(DateTime?), typeof(NullableDatePicker), null, BindingMode.TwoWay);

		public DateTime? NullableDate
		{
			get
			{
				return (DateTime?)GetValue(NullableDateProperty);
			}
			set
			{
				if (value != NullableDate)
				{
					SetValue(NullableDateProperty, value);
					UpdateDate();
				}
			}
		}

		public void CleanDate()
		{
			NullableDate = null;
			UpdateDate();
		}

		public void AssignValue()
		{
			NullableDate = Date;
			UpdateDate();
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();
			UpdateDate();
		}

		protected override void OnPropertyChanged(string propertyName = null)
		{
			base.OnPropertyChanged(propertyName);

			if (propertyName == IsFocusedProperty.PropertyName)
			{
				if (!IsFocused)
				{
					OnPropertyChanged(DateProperty.PropertyName);
				}
			}

			if (propertyName == DateProperty.PropertyName)
			{
				NullableDate = Date;
			}

			if (propertyName == NullableDateProperty.PropertyName)
			{
				if (NullableDate.HasValue)
				{
					Date = NullableDate.Value;
					Format = "dd-MM-yyyy";
				}
				else
				{
					Format = "  ";
				}
			}
		}

		private void UpdateDate()
		{
			if (NullableDate.HasValue)
			{
				Date = NullableDate.Value;
				Format = "dd-MM-yyyy";
			}
			else
			{
				Format = "  ";
			}
		}
	}
}