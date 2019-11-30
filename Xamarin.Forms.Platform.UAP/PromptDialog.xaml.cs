using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Xamarin.Forms.Platform.UAP
{
	public sealed partial class PromptDialog : ContentDialog
	{
		public PromptDialog()
		{
			this.InitializeComponent();
		}

		public string Message
		{
			get => TextBlockMessage.Text;
			set => TextBlockMessage.Text = value;
		}

		public string Input
		{
			get => TextBoxInput.Text;
			set => TextBoxInput.Text = value;
		}

		public string Placeholder
		{
			get => TextBoxInput.PlaceholderText;
			set => TextBoxInput.PlaceholderText = value;
		}

		public int MaxLength
		{
			get => TextBoxInput.MaxLength;
			set => TextBoxInput.MaxLength = value;
		}

		public InputScope InputScope
		{
			get => TextBoxInput.InputScope;
			set => TextBoxInput.InputScope = value;
		}

		//private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		//{
		//}

		//private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		//{
		//}
	}
}
