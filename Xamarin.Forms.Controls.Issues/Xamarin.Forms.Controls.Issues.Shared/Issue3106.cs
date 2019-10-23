using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 3106, "Added LineBreakMode on Button")]
	public class Issue3106 : TestContentPage
	{
		int count;
		const string content = "Welcome to Xamarin.Forms! Welcome to Xamarin.Forms! Welcome to Xamarin.Forms! Welcome to Xamarin.Forms!";

		Button materialButton;
		Label label;
		Label lineBreakModeType;

		protected override void Init()
		{
			var mainButton = new Button
			{
				Text = content,
				LineBreakMode = LineBreakMode.WordWrap,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand
			};

			mainButton.Clicked += MainButton_Clicked;

			materialButton = new Button
			{
				Text = content,
				LineBreakMode = LineBreakMode.WordWrap,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand,
			};

			label = new Label
			{
				Text = content,
				HorizontalOptions = LayoutOptions.CenterAndExpand,
				VerticalOptions = LayoutOptions.CenterAndExpand
			};
			
			
			lineBreakModeType = new Label
			{
				Text = LineBreakMode.WordWrap.ToString(),
				VerticalOptions = LayoutOptions.EndAndExpand,
				LineBreakMode = LineBreakMode.WordWrap,
			};
			var layout = new StackLayout
			{
				Children =
				{
					mainButton,
					materialButton,
					label,
					lineBreakModeType
				}
			};

			Content = layout;
		}

		void MainButton_Clicked(object sender, EventArgs e)
		{
			var mainButton = (sender as Button);
			label.LineBreakMode = materialButton.LineBreakMode = mainButton.LineBreakMode = SelectLineBreakMode();
			lineBreakModeType.Text = label.LineBreakMode.ToString();
		}

		LineBreakMode SelectLineBreakMode()
		{
			count++;
			switch (count)
			{
				case 1:
					return LineBreakMode.CharacterWrap;
				case 2:
					return LineBreakMode.HeadTruncation;
				case 3:
					return LineBreakMode.MiddleTruncation;
				case 4:
					return LineBreakMode.NoWrap;
				case 5:
					return LineBreakMode.TailTruncation;
				default:
					count = 0;
					return LineBreakMode.WordWrap;
			}
		}
	}
}
