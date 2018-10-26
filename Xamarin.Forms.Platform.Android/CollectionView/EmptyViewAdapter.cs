using System;
using Android.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Xamarin.Forms.Platform.Android
{
	public class EmptyViewAdapter : RecyclerView.Adapter
	{
		public object EmptyView { get; set; }

		public override int ItemCount => 1;

		public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
		{
			// TODO hartez 2018/10/25 10:12:47 If EmptyViewTemplate and EmptyView are both set, set binding context	
		}

		public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
		{
			var context = parent.Context;

			switch (EmptyView)
			{
				case View formsView:
				{
					var itemContentControl = new SizedItemContentControl(CreateRenderer(formsView, context), context,
						() => parent.Width, () => parent.Height);
					return new EmptyViewHolder(itemContentControl, formsView);
				}
				case string text:
				{
					return new EmptyViewHolder(CreateTextView(text, context), null);
				}
				case object obj:
					// TODO hartez 2018/10/25 10:08:44 Use template	
					break;
			}

			// If all else fails, create a TextView with the string value of EmptyView
			return new EmptyViewHolder(CreateTextView(EmptyView?.ToString(), context), null);
		}

		IVisualElementRenderer CreateRenderer(View view, Context context)
		{
			if (view == null)
			{
				throw new ArgumentNullException(nameof(view));
			}

			var renderer = Platform.CreateRenderer(view, context);
			Platform.SetRenderer(view, renderer);

			return renderer;
		}

		static TextView CreateTextView(string text, Context context)
		{
			var textView = new TextView(context) { Text = text };
			var layoutParams = new ViewGroup.LayoutParams(ViewGroup.LayoutParams.MatchParent,
				ViewGroup.LayoutParams.MatchParent);
			textView.LayoutParameters = layoutParams;
			textView.Gravity = GravityFlags.Center;
			return textView;
		}

		internal class EmptyViewHolder : RecyclerView.ViewHolder
		{
			public EmptyViewHolder(global::Android.Views.View itemView, View rootElement) : base(itemView)
			{
				View = rootElement;
			}

			public View View { get; }
		}
	}
}