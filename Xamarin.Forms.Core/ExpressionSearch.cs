using System;

namespace Xamarin.Forms
{
	internal abstract class ExpressionSearch
	{
		internal static Lazy<IExpressionSearch> Default { get; set; }
	}
}