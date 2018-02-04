using Gtk;

namespace Xamarin.Forms.Platform.GTK.Controls
{
    public class ScrolledTextView : ScrolledWindow
    {
        private TextView _textView;
        private int _maxLength;

        public ScrolledTextView()
        {
            ShadowType = ShadowType.In;
            HscrollbarPolicy = PolicyType.Never;
            VscrollbarPolicy = PolicyType.Always;

            _textView = new TextView
            {
                AcceptsTab = false,
                WrapMode = WrapMode.WordChar
            };

            _textView.Buffer.InsertText += InsertText;
            Add(_textView);
        }

        public TextView TextView => _textView;

        public void SetMaxLength(int maxLength)
        {
            
        }

        protected override void OnFocusGrabbed()
        {
            _textView?.GrabFocus();
        }

        void InsertText(object o, InsertTextArgs args)
        {
            args.RetVal = args.Length <= _maxLength;
        }
    }
}