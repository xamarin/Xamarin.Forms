using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls.GalleryPages.DragAndDropGalleries
{
	[Preserve(AllMembers = true)]
	public class DragAndDropGallery : Shell
	{
		public DragAndDropGallery()
		{
			Items.Add(new EnablingAndDisablingGestureTests());
			Items.Add(new VariousDragAndDropPermutations());
		}
	}
}
