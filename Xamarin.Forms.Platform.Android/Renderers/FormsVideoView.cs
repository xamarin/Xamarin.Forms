using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Media;
using System.Diagnostics;

namespace Xamarin.Forms.Platform.Android
{
    public sealed class FormsVideoView : VideoView
    {
        private int _videoHeight, _videoWidth;
        private TimeSpan _duration;

        public FormsVideoView(Context context) : base(context) { }

        public override void SetVideoPath(string path)
        {
            if (System.IO.File.Exists(path))
            {
                MediaMetadataRetriever retriever = new MediaMetadataRetriever();
                try
                {
                    retriever.SetDataSource(path);
                    ExtractMetadata(retriever);
                }
                catch { }
            }

            base.SetVideoPath(path);
        }

        private void ExtractMetadata(MediaMetadataRetriever retriever)
        {
            _duration = TimeSpan.Zero;
            _videoWidth = 0;
            int.TryParse(retriever.ExtractMetadata(MetadataKey.VideoWidth), out _videoWidth);
            _videoHeight = 0;
            int.TryParse(retriever.ExtractMetadata(MetadataKey.VideoHeight), out _videoHeight);

            string durationString = retriever.ExtractMetadata(MetadataKey.Duration);
            if (!string.IsNullOrEmpty(durationString))
            {
                long durationMS = long.Parse(durationString);
                _duration = TimeSpan.FromMilliseconds(durationMS);
            }
        }

        public override void SetVideoURI(global::Android.Net.Uri uri, IDictionary<string, string> headers)
        {
            GetMetaData(uri, headers);
            base.SetVideoURI(uri, headers);
        }

        public override void SetVideoURI(global::Android.Net.Uri uri)
        {
            GetMetaData(uri, new Dictionary<string, string>());
            base.SetVideoURI(uri);
        }

        private void GetMetaData(global::Android.Net.Uri uri, IDictionary<string, string> headers)
        {
            MediaMetadataRetriever retriever = new MediaMetadataRetriever();
            try
            {
                if (uri.Scheme != null && uri.Scheme.StartsWith("http") && headers != null)
                {
                    retriever.SetDataSource(uri.ToString(), headers);
                }
                else
                {
                    retriever.SetDataSource(Context, uri);
                }

                ExtractMetadata(retriever);
            }
            catch { }
        }

        public int VideoHeight
        {
            get { return _videoHeight; }
        }

        public int VideoWidth
        {
            get { return _videoWidth; }
        }

        public TimeSpan NaturalDuration
        {
            get { return _duration; }
        }
    }
}