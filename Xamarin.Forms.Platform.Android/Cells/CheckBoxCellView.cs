using Android.Content;
using Android.Widget;

namespace Xamarin.Forms.Platform.Android
{
	public class CheckBoxCellView : BaseCellView, CompoundButton.IOnCheckedChangeListener
	{
		public CheckBoxCellView(Context context, Cell cell) : base(context, cell)
		{
			var checkBox = new global::Android.Widget.CheckBox(context);
			checkBox.SetOnCheckedChangeListener(this);

			SetAccessoryView(checkBox);

			SetImageVisible(false);
		}

		public CheckBoxCell Cell { get; set; }

		public void OnCheckedChanged(CompoundButton buttonView, bool isChecked)
		{
			Cell.On = isChecked;
		}
	}
}