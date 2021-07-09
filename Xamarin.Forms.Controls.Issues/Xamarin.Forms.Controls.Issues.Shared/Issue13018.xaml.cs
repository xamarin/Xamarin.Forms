using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Collections.Generic;
using System.Windows.Input;

#if UITEST
using Xamarin.Forms.Core.UITests;
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.CollectionView)]
#endif
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 13018, "[Bug] [iOS] CollectionView EmptyView causes the application to crash",
		PlatformAffected.iOS)]
	public partial class Issue13018 : TestContentPage
	{


		public Issue13018()
		{
#if APP
			InitializeComponent();

            BindingContext = this;
            
            SelectColorCommand = new Command<ColorViewModel>(SelectColor);

            LoadColors();
#endif
		}

		public ICommand SelectColorCommand { get; }

		public List<ColorViewModel> Colors { get; set; }

		protected override void Init()
		{

		}

		void LoadColors()
		{
			Colors = new List<ColorViewModel>
			{
				new ColorViewModel { Color = Issues.Colors.FrontSetColor0 },
				new ColorViewModel { Color = Issues.Colors.FrontSetColor1 },
				new ColorViewModel { Color = Issues.Colors.FrontSetColor2 },
				new ColorViewModel { Color = Issues.Colors.FrontSetColor3 },
				new ColorViewModel { Color = Issues.Colors.FrontSetColor4 },
				new ColorViewModel { Color = Issues.Colors.FrontSetColor5 },
				new ColorViewModel { Color = Issues.Colors.FrontSetColor6 },
				new ColorViewModel { Color = Issues.Colors.FrontSetColor7 },
				new ColorViewModel { Color = Issues.Colors.FrontSetColor8 }
			};

			OnPropertyChanged(nameof(Colors));
		}

		void SelectColor(ColorViewModel selectedColor)
		{
			if (selectedColor.IsSelected)
			{
				return;
			}

			DeselectAllColors();

			selectedColor.IsSelected = true;
		}

		void DeselectAllColors() =>
			Colors.ForEach(c =>
			{
				if (c.IsSelected)
				{
					c.IsSelected = false;
				}
			});
	}

	[Preserve(AllMembers = true)]
	public static class Colors
	{
		public static readonly Color FrontSetColor0 = Color.White;
		public static readonly Color FrontSetColor1 = Color.FromRgb(255, 220, 0);
		public static readonly Color FrontSetColor2 = Color.FromRgb(255, 128, 0);
		public static readonly Color FrontSetColor3 = Color.FromRgb(255, 0, 0);
		public static readonly Color FrontSetColor4 = Color.FromRgb(255, 0, 107);
		public static readonly Color FrontSetColor5 = Color.FromRgb(255, 0, 255);
		public static readonly Color FrontSetColor6 = Color.FromRgb(0, 0, 255);
		public static readonly Color FrontSetColor7 = Color.FromRgb(0, 255, 255);
		public static readonly Color FrontSetColor8 = Color.FromRgb(0, 255, 21);
	}

	[Preserve(AllMembers = true)]
	public class ColorViewModel : BindableObject
	{
		private Color _color;
		private bool _isSelected;

		public Color Color
		{
			get => _color;
			set
			{
				_color = value;
				OnPropertyChanged();
			}
		}

		public bool IsSelected
		{
			get => _isSelected;
			set
			{
				_isSelected = value;
				OnPropertyChanged();
			}
		}
	}
}