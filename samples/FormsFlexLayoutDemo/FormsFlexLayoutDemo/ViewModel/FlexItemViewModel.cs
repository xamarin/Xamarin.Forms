using System;
using Xamarin.Forms;

namespace FormsFlexLayoutDemo
{
    public class FlexItemViewModel : BaseViewModel
    {
        public object AlignOptions => Enum.GetValues(typeof(Align));

        public object FlexBasisOptions => new string[] { "", "none", "0", "1" };

        Align _selectedAlignSelf;
        public Align SelectedAlignSelf
        {
            get { return _selectedAlignSelf; }
            set
            {
                if (_selectedAlignSelf == value)
                    return;
                _selectedAlignSelf = value;
                OnPropertyChanged();
            }
        }

        string _selectedFlexBasis;
        public string SelectedFlexBasis
        {
            get { return _selectedFlexBasis; }
            set
            {
                if (_selectedFlexBasis == value)
                    return;
                _selectedFlexBasis = value;
                OnPropertyChanged();
            }
        }

        int _index;
        public int Index
        {
            get { return _index; }
            set
            {
                if (_index == value)
                    return;
                _index = value;
                OnPropertyChanged();
            }
        }

        int _order;
        public int Order
        {
            get { return _order; }
            set
            {
                if (_order == value)
                    return;
                _order = value;
                OnPropertyChanged();
            }
        }

        float _flexshrink;
        public float FlexShrink
        {
            get { return _flexshrink; }
            set
            {
                if (_flexshrink == value)
                    return;
                _flexshrink = value;
                OnPropertyChanged();
            }
        }

        float _flexGrow;
        public float FlexGrow
        {
            get { return _flexGrow; }
            set
            {
                if (_flexGrow == value)
                    return;
                _flexGrow = value;
                OnPropertyChanged();
            }
        }
    }
}
