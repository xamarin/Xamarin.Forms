﻿using Xamarin.Forms;

namespace Xamarin.Platform
{
	public interface IText : IFont
	{
		string Text { get; }

		Color TextColor { get; }

		Font Font { get; }

		TextTransform TextTransform { get; }

		double CharacterSpacing { get; }
	}
}