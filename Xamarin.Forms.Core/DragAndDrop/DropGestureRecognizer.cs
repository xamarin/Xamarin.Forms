using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Xamarin.Forms
{
	public class DropGestureRecognizer : GestureRecognizer
	{
		public static readonly BindableProperty AllowDropProperty = BindableProperty.Create(nameof(AllowDrop), typeof(bool), typeof(DropGestureRecognizer), false);
		public event EventHandler<DragEventArgs> DragOver;
		public event EventHandler<DropEventArgs> Drop;

		public bool AllowDrop
		{
			get { return (bool)GetValue(AllowDropProperty); }
			set { SetValue(AllowDropProperty, value); }
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SendDragOver(DragEventArgs args)
		{
			DragOver?.Invoke(this, args);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public async void SendDrop(DropEventArgs args, VisualElement element)
		{
			Drop?.Invoke(this, args);

			if(!args.Handled)
			{
				var dataView = args.Data;
				var internalProperties = dataView.PropertiesInternal;
				VisualElement dragSource = null;
				ImageSource sourceTarget = await dataView.GetImageAsync();
				string text = await dataView.GetTextAsync();

				// TODO: Shane Generalize the retrieval of "values" from elements to provide the text for
				if (internalProperties.ContainsKey("DragSource"))
				{
					dragSource = (VisualElement)internalProperties["DragSource"];
					if (sourceTarget == null && dragSource is IImageElement imageElement)
						sourceTarget = imageElement.Source;

					if (String.IsNullOrWhiteSpace(text))
					{
						if (dragSource is Label label)
							text = label.Text;
						else if (dragSource is Entry entry)
							text = entry.Text;
						else if (dragSource is Editor editor)
							text = editor.Text;
						else if (dragSource is TimePicker tp)
							text = tp.Time.ToString();
						else if (dragSource is DatePicker dp)
							text = dp.Date.ToString();
					}
				}

				if (Parent is IImageElement && sourceTarget == null)
					sourceTarget = text;

				if (Parent is Image image)
					image.Source = sourceTarget;
				else if (Parent is ImageButton ib)
					ib.Source = sourceTarget;
				else if (Parent is Label label)
					label.Text = text;
				else if (Parent is Entry entry)
					entry.Text = text;
				else if (Parent is Editor editor)
					editor.Text = text;
			}
		}
	}
}