using Gdk;
using Xamarin.Forms.Platform.GTK.Extensions;

namespace Xamarin.Forms.Platform.GTK.Controls
{
	public class RadioButton : Gtk.RadioButton
	{
		private Gtk.Alignment _container;
		private Gtk.Box _imageAndLabelContainer;

		private Gdk.Color _defaultBorderColor;
		private Gdk.Color _defaultBackgroundColor;
		private Gdk.Color? _borderColor;
		private Gdk.Color? _backgroundColor;

		private Gtk.Image _image;
		private Gtk.Label _label;
		private uint _imageSpacing = 0;
		private uint _borderWidth = 0;

		public RadioButton(Gtk.RadioButton radio_group_member, string label) : base(radio_group_member, label)
		{
			_defaultBackgroundColor = Style.Backgrounds[(int)Gtk.StateType.Normal];
			_defaultBorderColor = Style.BaseColors[(int)Gtk.StateType.Active];

			Relief = Gtk.ReliefStyle.None;

			_image = new Gtk.Image();
			_label = new Gtk.Label();
			_container = new Gtk.Alignment(0.5f, 0.5f, 0, 0);


			Add(_container);

			RecreateContainer();
		}

		public RadioButton(string label) : this(null, label) { }

		public RadioButton() : this(string.Empty) { }

		#region Properties

		public Gtk.Label LabelWidget => _label;

		public Gtk.Image ImageWidget => _image;

		public uint ImageSpacing
		{
			get
			{
				return _imageSpacing;
			}

			set
			{
				_imageSpacing = value;
				UpdateImageSpacing();
			}
		}

		#endregion Properties

		#region Public methods

		public void SetBackgroundColor(Gdk.Color? color)
		{
			_backgroundColor = color;
			QueueDraw();
		}

		public void ResetBackgroundColor()
		{
			_backgroundColor = _defaultBackgroundColor;
			QueueDraw();
		}

		public void SetForegroundColor(Gdk.Color color)
		{
			_label.ModifyFg(Gtk.StateType.Normal, color);
			_label.ModifyFg(Gtk.StateType.Prelight, color);
			_label.ModifyFg(Gtk.StateType.Active, color);
		}

		public void SetBorderWidth(uint width)
		{
			_borderWidth = width;
			QueueDraw();
		}

		public void SetBorderColor(Gdk.Color? color)
		{
			_borderColor = color;
			QueueDraw();
		}

		public void ResetBorderColor()
		{
			_borderColor = _defaultBorderColor;
			QueueDraw();
		}

		public void SetImagePosition(Gtk.PositionType position)
		{
			ImagePosition = position;
			RecreateContainer();
		}

		#endregion Public methods

		#region Gtk.RadioButton overrides

		public override void Destroy()
		{
			base.Destroy();

			_label = null;
			_image = null;
			_imageAndLabelContainer = null;
			_container = null;
		}

		#endregion Gtk.RadioButton overrides

		#region Gtk.Widget overrides

		protected override bool OnExposeEvent(EventExpose evnt)
		{
			double colorMaxValue = 65535;

			using (var cr = CairoHelper.Create(GdkWindow))
			{
				cr.Rectangle(Allocation.Left, Allocation.Top, Allocation.Width, Allocation.Height);

				// Draw BackgroundColor
				if (_backgroundColor.HasValue)
				{
					var color = _backgroundColor.Value;
					cr.SetSourceRGBA(color.Red / colorMaxValue, color.Green / colorMaxValue, color.Blue / colorMaxValue, 1.0);
					cr.FillPreserve();
				}

				// Draw BorderColor
				if (_borderColor.HasValue)
				{
					cr.LineWidth = _borderWidth;

					var color = _borderColor.Value;
					cr.SetSourceRGB(color.Red / colorMaxValue, color.Green / colorMaxValue, color.Blue / colorMaxValue);
					cr.Stroke();
				}
			}

			return base.OnExposeEvent(evnt);
		}

		#endregion Gtk.Widget overrides

		#region Private methods

		private void RecreateContainer()
		{
			if (_imageAndLabelContainer != null)
			{
				_imageAndLabelContainer.RemoveFromContainer(_image);
				_imageAndLabelContainer.RemoveFromContainer(_label);
				_container.RemoveFromContainer(_imageAndLabelContainer);
				_imageAndLabelContainer = null;
			}

			switch (ImagePosition)
			{
				case Gtk.PositionType.Left:
					_imageAndLabelContainer = new Gtk.HBox();
					_imageAndLabelContainer.PackStart(_image, false, false, _imageSpacing);
					_imageAndLabelContainer.PackStart(_label, false, false, 0);
					break;
				case Gtk.PositionType.Right:
					_imageAndLabelContainer = new Gtk.HBox();
					_imageAndLabelContainer.PackStart(_label, false, false, 0);
					_imageAndLabelContainer.PackStart(_image, false, false, _imageSpacing);
					break;
				case Gtk.PositionType.Top:
					_imageAndLabelContainer = new Gtk.VBox();
					_imageAndLabelContainer.PackStart(_image, false, false, _imageSpacing);
					_imageAndLabelContainer.PackStart(_label, false, false, 0);
					break;
				case Gtk.PositionType.Bottom:
					_imageAndLabelContainer = new Gtk.VBox();
					_imageAndLabelContainer.PackStart(_label, false, false, 0);
					_imageAndLabelContainer.PackStart(_image, false, false, _imageSpacing);
					break;
			}

			if (_imageAndLabelContainer != null)
			{
				_container.Add(_imageAndLabelContainer);
				_container.ShowAll();
			}
		}

		private void UpdateImageSpacing()
		{
			_imageAndLabelContainer.SetChildPacking(_image, false, false, _imageSpacing, Gtk.PackType.Start);
		}

		#endregion Private methods
	}
}