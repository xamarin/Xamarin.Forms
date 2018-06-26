using Android.Runtime;

[assembly: ResourceDesigner("Xamarin.Forms.Platform.Android.Resource", IsApplication = false)]

namespace Xamarin.Forms.Platform.Android
{
	public class Resource
	{
		static Resource()
		{
			ResourceIdManager.UpdateIdValues();
		}

		public class Attribute
		{
			// aapt resource value: 0x7f0100a5
			// ReSharper disable once InconsistentNaming
			// Android is pretty insistent about this casing
			public static int actionBarSize = 2130772133;

			// aapt resource value: 0x7f010148
			public static int vpiCirclePageIndicatorStyle = 2130772296;


			static Attribute()
			{
				ResourceIdManager.UpdateIdValues();
			}

			Attribute()
			{
			}
		}

		public partial class Color

		{

			// aapt resource value: 0x7f090052
			public static int default_circle_indicator_fill_color = 2131296338;

			// aapt resource value: 0x7f090053
			public static int default_circle_indicator_page_color = 2131296339;

			// aapt resource value: 0x7f090054
			public static int default_circle_indicator_stroke_color = 2131296340;

			static Color()
			{
				ResourceIdManager.UpdateIdValues();
			}

			Color()
			{
			}
		}

		public partial class Drawable
		{
			public static int Next = 2130837637;

			public static int Prev = 2130837651;

			public static int Down = 2130837624;

			public static int Up = 2130837661;

			static Drawable()
			{
				ResourceIdManager.UpdateIdValues();
			}

			Drawable()
			{
			}
		}


		public partial class Id
		{

			// aapt resource value: 0x7f080000
			public static int horizontal = 2131230720;

			// aapt resource value: 0x7f080003
			public static int indicator = 2131230723;

			// aapt resource value: 0x7f080002
			public static int pager = 2131230722;

			// aapt resource value: 0x7f080001
			public static int vertical = 2131230721;

			// aapt resource value: 0x7f080004
			public static int pageIndicator = 2131230724;

			// aapt resource value: 0x7f080005
			public static int prev = 2131230725;

			// aapt resource value: 0x7f080006
			public static int next = 2131230726;

			static Id()
			{
				ResourceIdManager.UpdateIdValues();
			}

			Id()
			{
			}
		}

		public partial class Layout
		{

			// aapt resource value: 0x7f020000
			public static int horizontal_viewpager = 2130837504;

			// aapt resource value: 0x7f020001
			public static int vertical_viewpager = 2130837505;

			static Layout()
			{
				ResourceIdManager.UpdateIdValues();
			}

			Layout()
			{
			}
		}

		public partial class Styleable
		{

			public static int[] CirclePageIndicator = new int[]
			{
					2130771969,
					2130771970,
					2130771971,
					2130771972,
					2130771973,
					2130771974,
					2130771975,
					2130771976
			};

			// aapt resource value: 0
			public static int CirclePageIndicator_vpiCentered = 0;

			// aapt resource value: 5
			public static int CirclePageIndicator_vpiFillColor = 5;

			// aapt resource value: 2
			public static int CirclePageIndicator_vpiOrientation = 2;

			// aapt resource value: 1
			public static int CirclePageIndicator_vpiPageColor = 1;

			// aapt resource value: 4
			public static int CirclePageIndicator_vpiRadius = 4;

			// aapt resource value: 3
			public static int CirclePageIndicator_vpiSnap = 3;

			// aapt resource value: 6
			public static int CirclePageIndicator_vpiStrokeColor = 6;

			// aapt resource value: 7
			public static int CirclePageIndicator_vpiStrokeWidth = 7;

			public static int[] ViewPagerIndicator = new int[]
			{
					2130771968
			};

			// aapt resource value: 0
			public static int ViewPagerIndicator_vpiCirclePageIndicatorStyle = 0;

			static Styleable()
			{
				global::Android.Runtime.ResourceIdManager.UpdateIdValues();
			}

			private Styleable()
			{
			}
		}

		public partial class Style
		{
			// aapt resource value: 0x7f080180
			public static int Widget_CirclePageIndicator = 2131231104;

			static Style()
			{
				ResourceIdManager.UpdateIdValues();
			}

			Style()
			{
			}
		}

		public partial class Integer
		{
			public static int default_circle_indicator_orientation = 2131492873;

			static Integer()
			{
				ResourceIdManager.UpdateIdValues();
			}

			Integer()
			{
			}
		}

		public partial class Dimension
		{
			// aapt resource value: 0x7f06008e
			public static int default_circle_indicator_radius = 2131099790;

			// aapt resource value: 0x7f06008f
			public static int default_circle_indicator_stroke_width = 2131099791;

			static Dimension()
			{
				global::Android.Runtime.ResourceIdManager.UpdateIdValues();
			}

			Dimension()
			{
			}
		}

		public partial class Boolean
		{
			// aapt resource value: 0x7f0b0005
			public static int default_circle_indicator_centered = 2131427333;

			// aapt resource value: 0x7f0b0006
			public static int default_circle_indicator_snap = 2131427334;

			static Boolean()
			{
				ResourceIdManager.UpdateIdValues();
			}

			Boolean()
			{
			}
		}
	}
}