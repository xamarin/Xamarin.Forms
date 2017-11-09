using System;
using Xamarin.Forms;

namespace FormsFlexLayoutDemo
{
	public class FlexLayoutViewModel : BaseViewModel
	{
		public FlexLayoutViewModel()
		{
			//set demo default values
			SelectedAlignItems = Align.Stretch;
            SelectedWrap = Wrap.NoWrap;
            SelectedAlignContent = Align.Stretch;
			SelectedFlexDirection = FlexDirection.Row;
			SelectedJustifyContent = Justify.FlexStart;
            ItemWidth = 100;
			//MessagingCenter.Subscribe<Xamarin.Forms.FlexLayout, string>(this, "LayoutTime", (arg1, arg2) => LayoutTime = arg2);
		}
	
		public object AlignOptions => Enum.GetValues(typeof(Align));

		public object FlexDirectionOptions => Enum.GetValues(typeof(FlexDirection));

		public object WrapOptions => Enum.GetValues(typeof(Wrap));

		public object JustifyOptions => Enum.GetValues(typeof(Justify));

		public object PositionOptions => Enum.GetValues(typeof(FlexPosition));

		public object OverflowOptions => Enum.GetValues(typeof(Overflow));

		Align _selectedAlignItems;
		public Align SelectedAlignItems
		{
			get { return _selectedAlignItems; }
			set
			{
				if (_selectedAlignItems == value)
					return;
				_selectedAlignItems = value;
				OnPropertyChanged();
			}
		}

		Align _selectedAlignContent;
		public Align SelectedAlignContent
		{
			get { return _selectedAlignContent; }
			set
			{
				if (_selectedAlignContent == value)
					return;
				_selectedAlignContent = value;
				OnPropertyChanged();
			}
		}

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

		FlexDirection _selectedFlexDirection;
		public FlexDirection SelectedFlexDirection
		{
			get { return _selectedFlexDirection; }
			set
			{
				if (_selectedFlexDirection == value)
					return;
				_selectedFlexDirection = value;
				OnPropertyChanged();
			}
		}

		Wrap _selectedWrap;
		public Wrap SelectedWrap
		{
			get { return _selectedWrap; }
			set
			{
				if (_selectedWrap == value)
					return;
				_selectedWrap = value;
				OnPropertyChanged();
			}
		}

		Justify _selectedJustifyContent;
		public Justify SelectedJustifyContent
		{
			get { return _selectedJustifyContent; }
			set
			{
				if (_selectedJustifyContent == value)
					return;
				_selectedJustifyContent = value;
				OnPropertyChanged();
			}
		}

		FlexPosition _selectedPositionType;
		public FlexPosition SelectedPositionType
		{
			get { return _selectedPositionType; }
			set
			{
				if (_selectedPositionType == value)
					return;
				_selectedPositionType = value;
				OnPropertyChanged();
			}
		}

		string _layoutTime;
		public string LayoutTime
		{
			get { return _layoutTime; }
			set
			{
				if (_layoutTime == value)
					return;
				_layoutTime = value;
				OnPropertyChanged();
			}
		}

		Overflow _selectedOverflow;
		public Overflow SelectedOverflow
		{
			get { return _selectedOverflow; }
			set
			{
				if (_selectedOverflow == value)
					return;
				_selectedOverflow = value;
				OnPropertyChanged();
			}
		}

        int _itemWidth = 1;
        public int ItemWidth
        {
            get { return _itemWidth; }
            set
            {
                if (_itemWidth == value)
                    return;
                _itemWidth = value;
                OnPropertyChanged();
            }
        }
    }
}