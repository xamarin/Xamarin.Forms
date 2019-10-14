﻿using Xamarin.Forms;
using Xamarin.Forms.Platform.Tizen;
using Xamarin.Forms.Material.Tizen;
using Tizen.NET.MaterialComponents;

[assembly: ExportRenderer(typeof(CheckBox), typeof(MaterialCheckBoxRenderer), new[] { typeof(VisualMarker.MaterialVisual) }, Priority = short.MinValue)]
namespace Xamarin.Forms.Material.Tizen
{
	public class MaterialCheckBoxRenderer : CheckBoxRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<CheckBox> e)
		{
			if (Control == null)
			{
				SetNativeControl(new MCheckBox(Forms.NativeParent));
			}
			base.OnElementChanged(e);
		}
	}
}
