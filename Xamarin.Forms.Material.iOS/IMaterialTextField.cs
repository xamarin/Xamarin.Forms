using MaterialComponents;
using MTextInputControllerBase = MaterialComponents.TextInputControllerBase;

namespace Xamarin.Forms.Material.iOS
{
	public interface IMaterialTextField
	{
		SemanticColorScheme ColorScheme { get; set; }
		TypographyScheme TypographyScheme { get; set; }
		MTextInputControllerBase ActiveTextInputController { get; set; }
		ITextInput TextInput { get; }
	}
}