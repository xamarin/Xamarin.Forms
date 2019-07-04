using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Tizen;
using Xamarin.Forms.Material.Tizen;
using TForms = Xamarin.Forms.Platform.Tizen.Forms;
using Tizen.NET.MaterialComponents;

[assembly: ExportRenderer(typeof(Shell), typeof(MaterialShellRenderer), new[] { typeof(VisualMarker.MaterialVisual) })]
namespace Xamarin.Forms.Material.Tizen
{
	public class MaterialShellRenderer : ShellRenderer
	{
		protected override INavigationDrawer CreateNavigationDrawer()
		{
			return new MaterialNavigationDrawer(TForms.NativeParent);
		}

		protected override INavigationView CreateNavigationView()
		{
			return new MaterialNavigationView(TForms.NativeParent);
		}

		protected override ShellItemRenderer CreateShellItem(ShellItem item)
		{
			return new MaterialShellItemRenderer(this, item);
		}
	}
}
