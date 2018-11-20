using System;

namespace Xamarin.Forms
{
	public static class VisualMarker
	{
		public static IVisual MatchParent { get; } = new VisualRendererMarker.MatchParent();
		public static IVisual Default { get; } = new VisualRendererMarker.Default();
		public static IVisual Material { get; } = new VisualRendererMarker.Material();
	}

	public static class VisualRendererMarker
	{
		public sealed class Material : IVisual { }
		public sealed class Default : IVisual { }
		internal sealed class MatchParent : IVisual { }
	}

	[TypeConverter(typeof(VisualTypeConverter))]
	public interface IVisual
	{

	}

	[Xaml.TypeConversion(typeof(IVisual))]
	public class VisualTypeConverter : TypeConverter
	{
		public override object ConvertFromInvariantString(string value)
		{
			if (value != null)
			{
				switch (value.Trim().ToLowerInvariant())
				{
					case "matchparent": return VisualMarker.MatchParent;
					case "material": return VisualMarker.Material;
					case "default":
					default: return VisualMarker.Default;
				}
			}
			throw new InvalidOperationException($"Cannot convert \"{value}\" into {typeof(IVisual)}");
		}
	}
}