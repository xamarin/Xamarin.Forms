using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 10101, "Changing text on a Label on macOS resets font color to Black", PlatformAffected.macOS)]
	public partial class Issue10101 : TestContentPage
	{
		class ColorItem
		{
			public Color Color { get; set; } = Color.Red;
			public string Name { get; set; }
		}

		readonly Random _random = new Random();

		static List<ColorItem> GetSystemColorList() =>
			new List<ColorItem>
			{
				new ColorItem() {Color = Color.Accent, Name = "Accent"},
				new ColorItem() {Color = Color.AliceBlue, Name = "AliceBlue"},
				new ColorItem() {Color = Color.MintCream, Name = "MintCream"},
				new ColorItem() {Color = Color.MistyRose, Name = "MistyRose"},
				new ColorItem() {Color = Color.Moccasin, Name = "Moccasin"},
				new ColorItem() {Color = Color.NavajoWhite, Name = "NavajoWhite"},
				new ColorItem() {Color = Color.Navy, Name = "Navy"},
				new ColorItem() {Color = Color.OldLace, Name = "OldLace"},
				new ColorItem() {Color = Color.MidnightBlue, Name = "MidnightBlue"},
				new ColorItem() {Color = Color.Olive, Name = "Olive"},
				new ColorItem() {Color = Color.Orange, Name = "Orange"},
				new ColorItem() {Color = Color.OrangeRed, Name = "OrangeRed"},
				new ColorItem() {Color = Color.Orchid, Name = "Orchid"},
				new ColorItem() {Color = Color.PaleGoldenrod, Name = "PaleGoldenrod"},
				new ColorItem() {Color = Color.PaleGreen, Name = "PaleGreen"},
				new ColorItem() {Color = Color.PaleTurquoise, Name = "PaleTurquoise"},
				new ColorItem() {Color = Color.OliveDrab, Name = "OliveDrab"}
			};

		protected override void Init()
		{

		}

#if APP
		public Issue10101()
		{
			InitializeComponent();
			OnTapped(null, null);
		}

		ColorItem RandomColor()
		{
			var colors = GetSystemColorList();
			return colors[_random.Next(0, colors.Count - 1)];
		}

		void OnTapped(object sender, EventArgs e)
		{
			var color = RandomColor();

			labelText.TextColor = color.Color;
			labelColor.TextColor = color.Color;

			labelText.Text = $"TextColor should be {color.Name}";
			labelColor.Text = $"TextColor should be {color.Name}";
		}

		void button_Clicked(System.Object sender, System.EventArgs e)
		{
			label1.Text = "This text should still be red";
		}
#endif

	}
}