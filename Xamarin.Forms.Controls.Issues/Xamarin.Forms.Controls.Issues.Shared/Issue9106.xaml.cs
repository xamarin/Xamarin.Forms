using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Windows.Input;
#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 9106, "[Bug] Checked CheckBox does not update VisualState properly")]
	public partial class Issue9106 : TestContentPage
	{
		public Issue9106()
		{
			InitializeComponent();
		}


		protected override void Init()
		{
			BindingContext = new Issue9106ViewModel();
		}
	}

	[Preserve(AllMembers = true)]
	public class Issue9106ViewModel : BindableObject
	{
		public bool CheckboxIsEnabled { get; set; } 
		public bool CheckboxIsChecked { get; set; } 

		public Issue9106ViewModel()
		{
			CheckboxIsEnabled = false;
			CheckboxIsChecked = true;
		}

		public ICommand ToggleCheckboxIsEnabledCommand => new Command(ToggleCheckboxIsEnabled);

		void ToggleCheckboxIsEnabled()
		{
			CheckboxIsEnabled = !CheckboxIsEnabled;
			OnPropertyChanged("CheckboxIsEnabled");
		}
	}
}