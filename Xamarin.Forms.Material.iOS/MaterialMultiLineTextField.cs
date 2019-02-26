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
		internal bool AutoSizeWithChanges { get; set; } = false;
		CGSize _contentSize;


		public MaterialMultilineTextField(IMaterialEntryRenderer element, IFontElement fontElement)
		{
			VisualElement.VerifyVisualFlagEnabled();
			MaterialTextManager.Init(element, this, fontElement);
		}

		public override CGSize SizeThatFits(CGSize size)
		{
			bool expandTurnedBackOn = NumberOfLinesCheck();
			var result = base.SizeThatFits(size);

			if (nfloat.IsInfinity(result.Width))
				result = SystemLayoutSizeFittingSize(result, (float)UILayoutPriority.FittingSizeLevel, (float)UILayoutPriority.DefaultHigh);

			if (ExpandsOnOverflow)
				_contentSize = result;
			else
				_contentSize = TextView.ContentSize;

			if (!expandTurnedBackOn)
				UpdateExpandsOnOverflow();

			return result;
		}

		bool NumberOfLinesCheck()
		{
			if (!ExpandsOnOverflow &&
				!AutoSizeWithChanges &&
				!shouldRestrainSize())
			{
				ExpandsOnOverflow = true;
				return true;
			}

			return false;
		}

		bool shouldRestrainSize()
		{
			if (TextView?.Font == null)
				return false;

			return (((NumberOfLines + 1) * TextView.Font.LineHeight) > Frame.Height);
		}

		void UpdateExpandsOnOverflow()
		{
			if (!NumberOfLinesCheck() && !AutoSizeWithChanges && ExpandsOnOverflow && Frame.Height > 0 && TextView?.Font != null) 
			{
				if (shouldRestrainSize())
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

		int NumberOfLines
		{
			get
			{
				if (TextView?.ContentSize == null || TextView.Font == null || TextView.Font.LineHeight == 0)
					return 0;

				return (int)(_contentSize.Height / TextView.Font.LineHeight);
			}
		}

		internal void ApplyTypographyScheme(IFontElement fontElement) => MaterialTextManager.ApplyTypographyScheme(this, fontElement);

		internal void ApplyTheme(IMaterialEntryRenderer element) => MaterialTextManager.ApplyTheme(this, element);

		internal void UpdatePlaceholder(IMaterialEntryRenderer element) => MaterialTextManager.UpdatePlaceholder(this, element);

		internal void UpdateTextColor(IMaterialEntryRenderer element) => MaterialTextManager.UpdateTextColor(this, element);


	}
}
