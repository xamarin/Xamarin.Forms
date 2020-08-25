using ElmSharp;
using System;
using EBox = ElmSharp.Box;
using EImage = ElmSharp.Image;

namespace Xamarin.Forms.Platform.Tizen
{
	public class ShellMoreToolbar : GenList
	{
		const int _iconPadding = 15;
		const int _minIconPaddingInPixel = 30;

		const int _iconSize = 30;
		const int _minIconSizeInPixel = 60;

		const int _cellHeight = _iconPadding * 2 + _iconSize;
		const int _minCellHeightInPixel = _minIconPaddingInPixel * 2 + _minIconSizeInPixel;

		GenItemClass _defaultClass = null;

		public ShellMoreToolbar(EvasObject parent) : base(parent)
		{
			SetAlignment(-1, -1);
			SetWeight(1, 1);
			Homogeneous = true;
			SelectionMode = GenItemSelectionMode.Always;
			BackgroundColor = ShellRenderer.DefaultBackgroundColor.ToNative();
			_defaultClass = new GenItemClass(ThemeConstants.GenItemClass.Styles.Full)
			{
				GetContentHandler = GetContent,
			};
		}

		public void AddItem(ShellSection section)
		{
			Append(_defaultClass, section);
		}

		public int HeightRequest
		{
			get
			{
				return Math.Max(Forms.ConvertToScaledPixel(_cellHeight), _minCellHeightInPixel) * Count;
			}
		}

		EvasObject GetContent(object data, string part)
		{
			ShellSection section = data as ShellSection;

			var box = new EBox(Forms.NativeParent);
			box.Show();

			EImage icon = null;
			if (section.Icon != null)
			{
				icon = new EImage(Forms.NativeParent);
				icon.Show();
				box.PackEnd(icon);
				_ = icon.LoadFromImageSourceAsync(section.Icon);
			}

			var title = new Native.Label(Forms.NativeParent)
			{
				Text = section.Title,
				FontSize = Forms.ConvertToEflFontPoint(14),
				HorizontalTextAlignment = Native.TextAlignment.Start,
				VerticalTextAlignment = Native.TextAlignment.Center,
			};
			title.Show();
			box.PackEnd(title);

			box.SetLayoutCallback(() =>
			{
				var bound = box.Geometry;
				int iconPadding = Math.Max(Forms.ConvertToScaledPixel(_iconPadding), _minIconPaddingInPixel);
				int iconSize = Math.Max(Forms.ConvertToScaledPixel(_iconSize), _minIconSizeInPixel);
				int leftMargin = iconPadding;

				if (icon != null)
				{
					var iconBound = bound;
					iconBound.X += iconPadding;
					iconBound.Y += iconPadding;
					iconBound.Width = iconSize;
					iconBound.Height = iconSize;
					icon.Geometry = iconBound;
					leftMargin = (2 * iconPadding + iconSize);
				}
				
				bound.X += leftMargin;
				bound.Width -= leftMargin;
				title.Geometry = bound;
			});
			box.MinimumHeight = Math.Max(Forms.ConvertToScaledPixel(_cellHeight), _minCellHeightInPixel);
			return box;
		}
	}
}
