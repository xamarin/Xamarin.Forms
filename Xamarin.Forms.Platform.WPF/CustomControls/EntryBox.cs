using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Xamarin.WPF.CustomControls
{
    public class EntryBox : UserControl
	{
		private PasswordBox passwordBox = new PasswordBox();
		private TextBox textBox = new TextBox();
		private Label label = new Label();
		
		public EntryBox()
		{
			var grid = new Grid();
			label.IsHitTestVisible = false;
			label.Foreground = new SolidColorBrush(Color.FromArgb(125, 0, 0, 0));
			grid.Children.Add(passwordBox);
			grid.Children.Add(textBox);
			grid.Children.Add(label);
			this.Content = grid;

			this.SetLiteBinding(textBox, "Text", TextProperty, BindingMode.TwoWay);

			BindPassword();

			this.SetLiteBinding(label, "Foreground", PlaceholderForegroundBrushProperty, BindingMode.TwoWay);

			var bindingSelectionStart = new Binding("SelectionStart");
			bindingSelectionStart.Source = textBox;
			bindingSelectionStart.Mode = BindingMode.TwoWay;
			this.SetBinding(SelectionStartProperty, bindingSelectionStart);

			this.SetLiteBinding(textBox, "TextAlignment", TextAlignmentProperty, BindingMode.TwoWay);
			this.SetLiteBinding(textBox, "TextAlignment", TextAlignmentProperty, BindingMode.TwoWay);
		}
		
		private void BindPassword()
		{
			passwordBox.PasswordChanged += PasswordBox_PasswordChanged;
		}

		private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
		{
			this.Text = passwordBox.Password;
		}

		void UpdateState()
		{
			var isPassword = (bool)this.GetValue(IsPasswordProperty);
			passwordBox.Visibility = isPassword ? Visibility.Visible : Visibility.Collapsed;
			textBox.Visibility = isPassword ? Visibility.Collapsed : Visibility.Visible;
			label.Content = this.GetValue(PlaceholderProperty);
			label.Visibility = string.IsNullOrEmpty((string)this.GetValue(TextProperty)) ? Visibility.Visible : Visibility.Collapsed;
		}

		public static readonly DependencyProperty IsPasswordProperty = DependencyProperty.Register("IsPassword", typeof(bool), typeof(EntryBox), new PropertyMetadata(default(bool), PasswordBoxChanged));
		private static void PasswordBoxChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var entryBox = d as EntryBox;
			entryBox?.UpdateState();
		}

		public bool IsPassword
		{
			get { return (bool)GetValue(IsPasswordProperty); }
			set { SetValue(IsPasswordProperty, value); }
		}
		
		public static readonly DependencyProperty PlaceholderProperty = DependencyProperty.Register("Placeholder", typeof(string), typeof(EntryBox), new PropertyMetadata(default(string), PlaceholderPropertyChanged));
		
		private static void PlaceholderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var entryBox = d as EntryBox;
			entryBox?.UpdateState();
		}

		public string Placeholder
		{
			get { return (string)GetValue(PlaceholderProperty); }
			set { SetValue(PlaceholderProperty, value); }
		}
		
		public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(EntryBox), new PropertyMetadata(default(string), TextPropertyChanged));
		
		private static void TextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{

			var entryBox = d as EntryBox;
			entryBox.OnTextChanged(null);
			if (entryBox.Text != entryBox.passwordBox.Password)
			{
				entryBox.passwordBox.Password = entryBox.Text;
			}


			entryBox?.UpdateState();
		}

		public string Text
		{
			get { return (string)GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}

		public static readonly DependencyProperty PlaceholderForegroundBrushProperty = DependencyProperty.Register("PlaceholderForegroundBrush", typeof(Brush), typeof(EntryBox), new PropertyMetadata(default(Brush)));


		public Brush PlaceholderForegroundBrush
		{
			get { return (Brush)GetValue(PlaceholderForegroundBrushProperty); }
			set { SetValue(PlaceholderForegroundBrushProperty, value); }
		}

		public static readonly DependencyProperty SelectionStartProperty = DependencyProperty.Register("SelectionStart", typeof(int), typeof(EntryBox), new PropertyMetadata(default(int)));


		public int SelectionStart
		{
			get { return (int)GetValue(SelectionStartProperty); }
			set { SetValue(SelectionStartProperty, value); }
		}


		public static readonly DependencyProperty TextAlignmentProperty = DependencyProperty.Register("TextAlignment", typeof(TextAlignment), typeof(EntryBox), new PropertyMetadata(default(TextAlignment), TextAlignmentPropertyChanged));

		private static void TextAlignmentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var entryBox = d as EntryBox;
			HorizontalAlignment ha = HorizontalAlignment.Left;
			switch (entryBox.TextAlignment)
			{
				case TextAlignment.Center:
					ha = HorizontalAlignment.Center;
					break;
				case TextAlignment.Justify:
					ha = HorizontalAlignment.Stretch;
					break;
				case TextAlignment.Left:
					ha = HorizontalAlignment.Left;
					break;
				case TextAlignment.Right:
					ha = HorizontalAlignment.Right;
					break;
			}
			entryBox.label.HorizontalContentAlignment = ha;
		}

		public TextAlignment TextAlignment
		{
			get { return (TextAlignment)GetValue(TextAlignmentProperty); }
			set { SetValue(TextAlignmentProperty, value); }
		}

		public event EventHandler<TextChangedEventArgs> TextChanged;

		protected virtual void OnTextChanged(TextChangedEventArgs e)
		{
			TextChanged?.Invoke(this, e);
		}

	    public void Select(int start, int length)
	    {
	        textBox.Select(start,length);
	    }
	}
}