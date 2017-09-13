
namespace GMap.NET
{
   /// <summary>
   /// map zooming type
   /// </summary>
   public enum MouseWheelZoomType
   {
      /// <summary>
      /// zooms map to current mouse position and makes it map center
      /// </summary>
      MousePositionAndCenter,

      /// <summary>
      /// zooms to current mouse position, but doesn't make it map center,
      /// google/bing style ;}
      /// </summary>
      MousePositionWithoutCenter,

      /// <summary>
      /// zooms map to current view center
      /// </summary>
      ViewCenter,        
   }
}
