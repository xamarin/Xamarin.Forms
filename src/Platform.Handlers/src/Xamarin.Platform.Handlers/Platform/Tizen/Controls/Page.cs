using System;
using System.Collections.Generic;
using ElmSharp;

namespace Xamarin.Platform.Tizen
{
	public class Page : Background, IContainable<EvasObject>
	{
		public new IList<EvasObject> Children => _canvas.Children;

		internal Canvas _canvas;

		public Page(EvasObject parent) : base(parent)
		{
			_canvas = new Canvas(this);
			this.SetOverlayPart(_canvas);
		}

		public event EventHandler<LayoutEventArgs> LayoutUpdated
		{
			add
			{
				_canvas.LayoutUpdated += value;
			}
			remove
			{
				_canvas.LayoutUpdated -= value;
			}
		}

		protected override void OnUnrealize()
		{
			_canvas.Unrealize();
			base.OnUnrealize();
		}
	}
}
