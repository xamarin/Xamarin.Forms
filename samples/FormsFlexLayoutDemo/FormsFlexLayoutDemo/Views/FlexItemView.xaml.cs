using System;
using Xamarin.Forms;

namespace FormsFlexLayoutDemo
{
    public partial class FlexItemView : ContentView
    {
        public event EventHandler RemoveClicked;
        public FlexItemView()
        {
            InitializeComponent();
            BindingContext = new FlexItemViewModel();
        }
        void ClearClicked(object sender, EventArgs e)
        {
            RemoveClicked?.Invoke(this, e);
        }
    }
}
