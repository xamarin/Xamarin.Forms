using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace Xamarin.Forms
{
	public class DragGestureRecognizer : GestureRecognizer
	{
		public static readonly BindableProperty CanDragProperty = BindableProperty.Create(nameof(CanDrag), typeof(bool), typeof(DragGestureRecognizer), false);

		public event EventHandler<DropCompletedEventArgs> DropCompleted;
		public event EventHandler<DragStartingEventArgs> DragStarting;

		public bool CanDrag
		{
			get { return (bool)GetValue(CanDragProperty); }
			set { SetValue(CanDragProperty, value); }
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SendDropCompleted(DropCompletedEventArgs args)
		{
			_ = args ?? throw new ArgumentNullException(nameof(args));
			DropCompleted?.Invoke(this, args);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SendDragStarting(DragStartingEventArgs args, VisualElement element)
		{
			_ = args ?? throw new ArgumentNullException(nameof(args));

			DragStarting?.Invoke(this, args);

			if (!args.Handled)
			{
				args.Data.PropertiesInternal.Add("DragSource", element);
			}
		}
	}
}
