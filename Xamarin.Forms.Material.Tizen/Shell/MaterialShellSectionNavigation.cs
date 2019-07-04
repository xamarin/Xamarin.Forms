using Xamarin.Forms.Platform.Tizen;
using TForms = Xamarin.Forms.Platform.Tizen.Forms;

namespace Xamarin.Forms.Material.Tizen
{
	public class MaterialShellSectionNavigation : ShellSectionNavigation
	{
		public MaterialShellSectionNavigation(IFlyoutController flyoutController, ShellSection section) : base(flyoutController, section)
		{
		}

		protected override ShellSectionRenderer CreateShellSection(ShellSection section)
		{
			return new MaterialShellSectionRenderer(section);
		}
	}
}
