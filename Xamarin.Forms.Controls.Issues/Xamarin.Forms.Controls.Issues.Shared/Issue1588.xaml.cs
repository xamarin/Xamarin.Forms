using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms.Controls.Issues
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 1588, "[WPF] Stacklayout WidthRequest adds unwanted margin", PlatformAffected.WPF)]
	public partial class Issue1588 : TestContentPage
	{
		public Issue1588 ()
		{
			InitializeComponent ();
		}

		protected override void Init()
		{
		}
	}

	public enum EntryOrientation
	{
		Vertical,

		Horizontal
	}

	class LabledEntry : ContentView
	{
		StackLayout stk;

		Label label;

		Entry entry;

		public static readonly BindableProperty FontSizeProperty = BindableProperty.Create(
			"FontSize",
			typeof(double),
			typeof(LabledEntry),
			Device.GetNamedSize(NamedSize.Small, typeof(Entry)),
			BindingMode.TwoWay,
			propertyChanged: (bindable, oldvalue, newvalue) =>
			{
				var labledEntry = ((LabledEntry)bindable);
				var fontSize = (double)newvalue;
				labledEntry.entry.FontSize = fontSize;
				labledEntry.label.FontSize = fontSize;
			});

		public double FontSize
		{
			set { SetValue(FontSizeProperty, value); }
			get { return (double)GetValue(FontSizeProperty); }
		}

		public static readonly BindableProperty OrientationProperty = BindableProperty.Create(
			"Orientation",
			typeof(EntryOrientation),
			typeof(LabledEntry),
			EntryOrientation.Vertical,
			BindingMode.TwoWay,
			propertyChanged: (bindable, oldvalue, newvalue) =>
			{
				var labledEntry = ((LabledEntry)bindable);
				var orientation = (EntryOrientation)newvalue;
				labledEntry.stk.Orientation = orientation == EntryOrientation.Vertical ? StackOrientation.Vertical : StackOrientation.Horizontal;
				labledEntry.Orientation = orientation;
			});

		public EntryOrientation Orientation
		{
			set { SetValue(OrientationProperty, value); }
			get { return (EntryOrientation)GetValue(OrientationProperty); }
		}


		public static readonly BindableProperty KeyboardProperty = BindableProperty.Create(
			"Keyboard",
			typeof(Keyboard),
			typeof(LabledEntry),
			Keyboard.Default,
			BindingMode.TwoWay,
			propertyChanged: (bindable, oldvalue, newvalue) =>
			{
				var labledEntry = ((LabledEntry)bindable);
				var keyboard = (Keyboard)newvalue;
				labledEntry.entry.Keyboard = keyboard;
				labledEntry.Keyboard = keyboard;
			});

		public Keyboard Keyboard
		{
			set { SetValue(KeyboardProperty, value); }
			get { return (Keyboard)GetValue(KeyboardProperty); }
		}

		public static readonly BindableProperty LabelTextProperty = BindableProperty.Create(
			"LabelText",
			typeof(string),
			typeof(LabledEntry),
			null,
			BindingMode.TwoWay,
			propertyChanged: (bindable, oldvalue, newvalue) =>
			{
				var labledEntry = ((LabledEntry)bindable);
				labledEntry.label.Text = (string)newvalue;
				labledEntry.LabelText = (string)newvalue;
			});

		public string LabelText
		{
			set { SetValue(LabelTextProperty, value); }
			get { return (string)GetValue(LabelTextProperty); }
		}

		public static readonly BindableProperty EntryTextProperty = BindableProperty.Create(
			"EntryText",
			typeof(string),
			typeof(LabledEntry),
			null,
			BindingMode.TwoWay,
			propertyChanged: (bindable, oldvalue, newvalue) =>
			{
				var labledEntry = ((LabledEntry)bindable);
				labledEntry.entry.Text = (string)newvalue;
				labledEntry.EntryText = (string)newvalue;
			});

		public string EntryText
		{
			set { SetValue(EntryTextProperty, value); }
			get { return (string)GetValue(EntryTextProperty); }
		}

		public static readonly BindableProperty PlaceholderProperty = BindableProperty.Create(
			"Placeholder",
			typeof(string),
			typeof(LabledEntry),
			null,
			BindingMode.TwoWay,
			propertyChanged: (bindable, oldvalue, newvalue) =>
			{
				var labledEntry = ((LabledEntry)bindable);
				labledEntry.entry.Placeholder = (string)newvalue;
				labledEntry.Placeholder = (string)newvalue;
			});

		public string Placeholder
		{
			set { SetValue(PlaceholderProperty, value); }
			get { return (string)GetValue(PlaceholderProperty); }
		}


		public static readonly BindableProperty LabelTextColorProperty = BindableProperty.Create(
			"LabelTextColor",
			typeof(Color),
			typeof(LabledEntry),
			Color.Default,
			BindingMode.TwoWay,
			propertyChanged: (bindable, oldvalue, newvalue) =>
			{
				var labledEntry = ((LabledEntry)bindable);
				labledEntry.label.TextColor = (Color)newvalue;
				labledEntry.LabelTextColor = (Color)newvalue;
			});

		public Color LabelTextColor
		{
			set { SetValue(LabelTextColorProperty, value); }
			get { return (Color)GetValue(LabelTextColorProperty); }
		}

		public static readonly BindableProperty EntryTextColorProperty = BindableProperty.Create(
			"EntryTextColor",
			typeof(Color),
			typeof(LabledEntry),
			Color.Default,
			BindingMode.TwoWay,
			propertyChanged: (bindable, oldvalue, newvalue) =>
			{
				var labledEntry = ((LabledEntry)bindable);
				labledEntry.entry.TextColor = (Color)newvalue;
				labledEntry.EntryTextColor = (Color)newvalue;
			});

		public Color EntryTextColor
		{
			set { SetValue(EntryTextColorProperty, value); }
			get { return (Color)GetValue(EntryTextColorProperty); }
		}


		public static readonly BindableProperty PlaceholderColorProperty = BindableProperty.Create(
			"PlaceholderColor",
			typeof(Color),
			typeof(LabledEntry),
			Color.Default,
			BindingMode.TwoWay,
			propertyChanged: (bindable, oldvalue, newvalue) =>
			{
				var labledEntry = ((LabledEntry)bindable);
				labledEntry.entry.PlaceholderColor = (Color)newvalue;
				labledEntry.PlaceholderColor = (Color)newvalue;
			});

		public Color PlaceholderColor
		{
			set { SetValue(PlaceholderColorProperty, value); }
			get { return (Color)GetValue(PlaceholderColorProperty); }
		}


		public static readonly BindableProperty MaxLengthProperty = BindableProperty.Create(
			"MaxLength",
			typeof(int),
			typeof(LabledEntry),
			30,
			BindingMode.TwoWay,
			propertyChanged: (bindable, oldvalue, newvalue) =>
			{
				var labledEntry = ((LabledEntry)bindable);

			});

		public int MaxLength
		{
			set { SetValue(MaxLengthProperty, value); }
			get { return (int)GetValue(MaxLengthProperty); }
		}

		public LabledEntry()
		{
			stk = new StackLayout();
			entry = new Entry();
			entry.FontSize = Device.GetNamedSize(NamedSize.Small, entry);
			label = new Label();
			entry.FontSize = Device.GetNamedSize(NamedSize.Small, label);
			entry.HorizontalOptions = LayoutOptions.FillAndExpand;
			label.VerticalOptions = LayoutOptions.Center;
			stk.Children.Add(label);
			stk.Children.Add(entry);
			Content = stk;

		}
	}

	public enum DividerOrientation
	{
		Vertical,

		Horizontal
	}
	class Divider : ContentView
	{
		BoxView line;

		public static readonly BindableProperty OrientationProperty = BindableProperty.Create(
			"Orientation",
			typeof(DividerOrientation),
			typeof(Divider),
			DividerOrientation.Vertical,
			BindingMode.TwoWay,
			propertyChanged: (bindable, oldvalue, newvalue) =>
			{
				var divider = ((Divider)bindable);
				var orientation = (DividerOrientation)newvalue;
				if (orientation == DividerOrientation.Vertical)
				{
					divider.line.WidthRequest = divider.Thickness;
					divider.line.HeightRequest = -1;
				}
				else
				{
					divider.line.HeightRequest = divider.Thickness;
					divider.line.WidthRequest = -1;
				}
				divider.Orientation = orientation;
			});

		public DividerOrientation Orientation
		{
			set { SetValue(OrientationProperty, value); }
			get { return (DividerOrientation)GetValue(OrientationProperty); }
		}

		public static readonly BindableProperty ColorProperty = BindableProperty.Create(
			"Color",
			typeof(Color),
			typeof(Divider),
			Color.Black,
			BindingMode.TwoWay,
			propertyChanged: (bindable, oldvalue, newvalue) =>
			{
				var divider = ((Divider)bindable);
				divider.line.Color = (Color)newvalue;
				divider.Color = (Color)newvalue;
			});

		public Color Color
		{
			set { SetValue(ColorProperty, value); }
			get { return (Color)GetValue(ColorProperty); }
		}


		public static readonly BindableProperty ThicknessProperty = BindableProperty.Create(
		   "Thickness",
		   typeof(double),
		   typeof(Divider),
		   1.0,
		   BindingMode.TwoWay,
		   propertyChanged: (bindable, oldvalue, newvalue) =>
		   {
			   var divider = ((Divider)bindable);
			   var thickness = (double)newvalue;
			   if (divider.Orientation == DividerOrientation.Vertical)
			   {
				   divider.line.WidthRequest = thickness;
			   }
			   else
			   {
				   divider.line.HeightRequest = thickness;
			   }
			   divider.Thickness = thickness;
		   });

		public double Thickness
		{
			set { SetValue(ThicknessProperty, value); }
			get { return (double)GetValue(ThicknessProperty); }
		}

		public Divider()
		{
			line = new BoxView();
			Content = line;

		}
	}
}