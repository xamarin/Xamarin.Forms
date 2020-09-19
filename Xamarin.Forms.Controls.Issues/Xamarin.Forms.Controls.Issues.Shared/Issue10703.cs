using System;
using System.Collections.Generic;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;

namespace Xamarin.Forms.Controls.Issues
{
	[Issue(IssueTracker.Github, 10703, "Android keyboard disappears on losing focus", PlatformAffected.Android)]
	public class Issue10703 : TestContentPage
	{
		protected override void Init()
		{
			//This same issue is also reported in #4298
			//https://github.com/xamarin/Xamarin.Forms/issues/4298
			var editor = new Editor() { Text = "This is an editor" };
			var entry = new Entry { Text = "This is an entry" };
			var secondEntry = new Entry { Text = "This Entry will lose focus in Android." };
			var button = new Button { Text = "Tap this button, editor and entry shouldn't lose focus" };

			//This issue was reported in Issue#2136 and got fixed for iOS with PR #8740.
			//https://github.com/xamarin/Xamarin.Forms/issues/2136
			button.On<PlatformConfiguration.iOS>().SetCanBecomeFirstResponder(true);

			//Commenting below two lines will show existing behavior. 
			entry.On<PlatformConfiguration.Android>().SetDismissKeyboardOnOutsideTap(false);
			editor.On<PlatformConfiguration.Android>().SetDismissKeyboardOnOutsideTap(false);

			//With above feature, developer is getting a chance to disable automatic dismissing of Keyboard
			//when Editor/Entry loses focus. Helpful in developing chat application. Keyboard will not
			//dismiss on tapping Send Button. Developer can then maybe dismiss keyboard with a TapGestureRecognizer
			//on ContentPage.
			Content = new StackLayout()
			{
				Children = { editor, entry, secondEntry, button }
			};

			TapGestureRecognizer gestureRecognizer = new TapGestureRecognizer
			{
				Command = new Command(() =>
				{
					editor.Unfocus();

				})
			};
			Content.GestureRecognizers.Add(gestureRecognizer);
		}
	}
}
