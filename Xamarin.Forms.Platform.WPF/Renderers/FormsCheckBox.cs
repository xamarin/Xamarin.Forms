using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFCheckBox = System.Windows.Controls.CheckBox;
using WControl = System.Windows.Controls.Control;
using System.Windows;
using System.Windows.Media;

namespace Xamarin.Forms.Platform.WPF
{
	public class FormsCheckBox : WPFCheckBox
	{

		public static readonly DependencyProperty TintBrushProperty =
			DependencyProperty.Register(nameof(TintBrush), typeof(Brush), typeof(FormsCheckBox),
				new PropertyMetadata(default(Brush)));

		public FormsCheckBox()
		{
			
		}

		public Brush TintBrush
		{
			get { return (Brush)GetValue(TintBrushProperty); }
			set { SetValue(TintBrushProperty, value); }
		}
	}
}
