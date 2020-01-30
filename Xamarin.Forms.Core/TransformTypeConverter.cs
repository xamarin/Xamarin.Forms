namespace Xamarin.Forms
{
    public class TransformTypeConverter : TypeConverter
    {
        public override object ConvertFromInvariantString(string value)
        {
            return new MatrixTransform
            {
                Matrix = MatrixTypeConverter.CreateMatrix(value)
            };
        }
    }
}