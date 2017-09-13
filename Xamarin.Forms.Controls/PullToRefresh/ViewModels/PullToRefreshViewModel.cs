using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Xamarin.Forms.Controls.PullToRefresh.ViewModels
{
    public class PullToRefreshViewModel : BindableObject
    {
        private ObservableCollection<string> _items;
        private bool _isBusy;
        private int _count;

        private ICommand _loadCommand;

        public PullToRefreshViewModel()
        {
            _count = 1;
            Items = new ObservableCollection<string>();

            for (int i = 0; i < 10; i++)
            {
                Items.Insert(0, _count.ToString());
                _count++;
            }
        }

        public ObservableCollection<string> Items
        {
            get
            {
                return _items;
            }
            set
            {
                _items = value;
                OnPropertyChanged();
            }
        }

        public bool IsBusy
        {
            get { return _isBusy; }
            set
            {
                _isBusy = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoadCommand
        {
            get
            {
                if (_loadCommand == null)
                {
                    _loadCommand = new Command(async () => await LoadDataAsync());
                }

                return _loadCommand;
            }
        }

        private async Task LoadDataAsync()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            await Task.Delay(3000);

            for (int i = 0; i < 10; i++)
            {
                Items.Insert(0, _count.ToString());
                _count++;
            }

            IsBusy = false;
        }
    }
}