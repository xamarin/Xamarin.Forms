using System;
using CoreGraphics;
using MaterialComponents;
using UIKit;
using MMultilineTextField = MaterialComponents.MultilineTextField;
using MTextInputControllerBase = MaterialComponents.TextInputControllerBase;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Platform.iOS.Material
{
	public class MaterialMultilineTextField : MMultilineTextField, IMaterialTextField
	{
		public SemanticColorScheme ColorScheme { get; set; }
		public TypographyScheme TypographyScheme { get; set; }
		public MTextInputControllerBase ActiveTextInputController { get; set; }
		public ITextInput TextInput => this;
		CGSize ContainerSize { get; set; }
		internal bool AutoSizeWithChanges { get; set; } = false;

		public MaterialMultilineTextField(IMaterialEntryRenderer element, IFontElement fontElement)
		{
			VisualElement.VerifyVisualFlagEnabled();
			MaterialTextManager.Init(element, this, fontElement);
		}

		public override CGSize SizeThatFits(CGSize size)
		{
			var result = base.SizeThatFits(size);

			if (nfloat.IsInfinity(result.Width))
				result = SystemLayoutSizeFittingSize(result, (float)UILayoutPriority.FittingSizeLevel, (float)UILayoutPriority.DefaultHigh);

			ContainerSize = result;
			UpdateExpandsOnOverflow();

			return result;
		}

		void UpdateExpandsOnOverflow()
		{
			if (!AutoSizeWithChanges && ExpandsOnOverflow && Frame.Height > 0) 
			{
				if ((ContainerSize.Height + TextView.Font.LineHeight) > Frame.Height)
				{
					ExpandsOnOverflow = false;
				}
			}
		}

		public override CGRect Frame
		{
			get => base.Frame; set
			{
				base.Frame = value;
				UpdateExpandsOnOverflow();
			}
		}


		internal void ApplyTypographyScheme(IFontElement fontElement) => MaterialTextManager.ApplyTypographyScheme(this, fontElement);

		internal void ApplyTheme(IMaterialEntryRenderer element) => MaterialTextManager.ApplyTheme(this, element);

		internal void UpdatePlaceholder(IMaterialEntryRenderer element) => MaterialTextManager.UpdatePlaceholder(this, element);

		internal void UpdateTextColor(IMaterialEntryRenderer element) => MaterialTextManager.UpdateTextColor(this, element);


	}
}