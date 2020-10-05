using System.Collections.Generic;

namespace Xamarin.Platform
{
	public interface ILayout : IView
	{
		IReadOnlyList<IView> Children { get; }
	}

	public interface IStackLayout : ILayout
	{
		int Spacing { get; }
	}

	public enum Alignment
	{
		Start,
		Center,
		End,
		Fill
	}
}