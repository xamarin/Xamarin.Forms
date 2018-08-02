namespace Xamarin.Forms
{
    /// <summary>
    /// Defines the potential states of a MediaElement object. 
    /// </summary>
    public enum MediaElementState
    {
        /// <summary>
        /// The <see cref="MediaElement"/> contains no media.
        /// The <see cref="MediaElement"/> displays a transparent frame.
        /// </summary>
        Closed = 0,

        /// <summary>
        /// The <see cref="MediaElement"/> is validating and attempting to load the specified source.
        /// </summary>
        Opening = 1,

        /// <summary>
        /// The <see cref="MediaElement"/> is loading the media for playback.
        /// Its <see cref="MediaElement.Position"/> does not advance during this state.
        /// If the <see cref="MediaElement"/> was already playing video, it continues to display the last displayed frame.
        /// </summary>
        Buffering = 2,

        /// <summary>
        /// The <see cref="MediaElement"/> is playing the current media source.
        /// </summary>
        Playing = 3,

        /// <summary>
        /// The <see cref="MediaElement"/> does not advance its <see cref="MediaElement.Position"/>.
        /// If the <see cref="MediaElement"/> was playing video, it continues to display the current frame.
        /// </summary>
        Paused = 4,

        /// <summary>
        /// The <see cref="MediaElement"/> contains media but is not playing or paused.
        /// Its <see cref="MediaElement.Position"/> is 0 and does not advance.
        /// If the loaded media is video, the <see cref="MediaElement"/> displays the first frame.
        /// </summary>
        Stopped = 5,
    }
}