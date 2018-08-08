using Android.Content;
using Android.Media;
using Android.Widget;
using System;
using System.Collections.Generic;

namespace Xamarin.Forms.Platform.Android
{
	public sealed class FormsVideoView : VideoView
	{
		int _videoHeight, _videoWidth;

		public FormsVideoView(Context context) : base(context) { }

		public override void SetVideoPath(string path)
		{
			base.SetVideoPath(path);

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
		}

		void ExtractMetadata(MediaMetadataRetriever retriever)
		{
			_videoWidth = 0;
			int.TryParse(retriever.ExtractMetadata(MetadataKey.VideoWidth), out _videoWidth);
			_videoHeight = 0;
			int.TryParse(retriever.ExtractMetadata(MetadataKey.VideoHeight), out _videoHeight);

			string durationString = retriever.ExtractMetadata(MetadataKey.Duration);
			if (!string.IsNullOrEmpty(durationString))
			{
				long durationMS = long.Parse(durationString);
				NaturalDuration = TimeSpan.FromMilliseconds(durationMS);
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

		void GetMetaData(global::Android.Net.Uri uri, IDictionary<string, string> headers)
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

		public TimeSpan? NaturalDuration { get; private set; }
	}
}