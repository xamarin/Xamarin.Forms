using Xamarin.Forms.Platform.Tizen;
using TForms = Xamarin.Forms.Platform.Tizen.Forms;

namespace Xamarin.Forms.Material.Tizen
{
	public class MaterialShellSectionRenderer : ShellSectionRenderer
	{
		public MaterialShellSectionRenderer(ShellSection section) : base(section)
		{
		}

		protected override IShellTabs CreateToolbar()
		{
			return new MaterialShellTabs(TForms.NativeParent);
		}
	}
}
