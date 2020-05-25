using WMatrix = System.Windows.Media.Matrix;
using WMatrixTransform = System.Windows.Media.MatrixTransform;

namespace Xamarin.Forms.Platform.WPF.Extensions
{
	public static class TransformExtensions
	{
		public static WMatrixTransform ToWindows(this Transform transform)
		{
			Matrix matrix = transform.Value;

			return new WMatrixTransform
			{
				Matrix = new WMatrix(
					matrix.M11,
					matrix.M12,
					matrix.M21,
					matrix.M22,
					matrix.OffsetX,
					matrix.OffsetY)
			};
		}
	}
}