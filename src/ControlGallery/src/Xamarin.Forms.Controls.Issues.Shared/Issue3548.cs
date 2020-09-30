﻿using System.Threading.Tasks;
using Xamarin.Forms.Controls.Effects;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[NUnit.Framework.Category(Core.UITests.UITestCategories.Github5000)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 3548, "Cannot attach effect to Frame", PlatformAffected.Android)]
	public class Issue3548 : TestContentPage
	{
		private const string SuccessMessage = "EFFECT IS ATTACHED!";

		private Frame _statusFrame;
		private AttachedStateEffect _effect;

		protected override void Init()
		{
			var statusLabel = new Label
			{
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				FontSize = 40,
				Text = "EFFECT IS NOT ATTACHED"
			};

			_effect = new AttachedStateEffect();

			_statusFrame = new Frame
			{
				BackgroundColor = Color.Red,
				Padding = 15,
				VerticalOptions = LayoutOptions.CenterAndExpand,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				Content = statusLabel
			};

			_effect.StateChanged += (sender, e) =>
			{
				statusLabel.Text = _effect.State == AttachedStateEffect.AttachedState.Attached
					? SuccessMessage
					: "EFFECT IS DEATTACHED";

				_statusFrame.BackgroundColor = Color.LightGreen;
			};

			Content = new StackLayout
			{
				Padding = 50,
				Children = {
					_statusFrame
				}
			};
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			_statusFrame.Effects.Add(_effect);
		}

		protected override void OnDisappearing()
		{
			base.OnDisappearing();
			_statusFrame.Effects.Remove(_effect);
		}

#if UITEST
		[Test]
		public void CheckIsEffectAttached()
		{
			RunningApp.WaitForElement(q => q.Marked(SuccessMessage));
		}
#endif
	}
}

