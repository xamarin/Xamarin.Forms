namespace Xamarin.Forms.Platform.WPF
{
    public class VisualElementChangedEventArgs : ElementChangedEventArgs<VisualElement>
    {
        public VisualElementChangedEventArgs(VisualElement oldElement, VisualElement newElement) : base(oldElement, newElement)
        {
        }
    }
}