using WMatrix = Windows.UI.Xaml.Media.Matrix;
using WMatrixTransform = Windows.UI.Xaml.Media.MatrixTransform;

namespace Xamarin.Forms.Platform.UWP
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