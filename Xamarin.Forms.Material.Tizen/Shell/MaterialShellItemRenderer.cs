using Xamarin.Forms.Platform.Tizen;
using TForms = Xamarin.Forms.Platform.Tizen.Forms;

namespace Xamarin.Forms.Material.Tizen
{
	public class MaterialShellItemRenderer : ShellItemRenderer
	{
		public MaterialShellItemRenderer(IFlyoutController flyoutController, ShellItem item) : base(flyoutController, item)
		{
		}

		protected override IShellTabs CreateTabs()
		{
			return new MaterialShellTabs(TForms.NativeParent);
		}

		protected override ShellSectionNavigation CreateShellSectionNavigation(IFlyoutController flyoutController, ShellSection section)
		{
			return new MaterialShellSectionNavigation(flyoutController, section);
		}
	}
}
