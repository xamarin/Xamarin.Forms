namespace Microsoft.Maui
{
	public interface ILabel : IView, IText
	{
		int MaxLines { get; }

		LineBreakMode LineBreakMode { get; }
	}
}