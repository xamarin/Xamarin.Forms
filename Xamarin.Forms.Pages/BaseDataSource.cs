using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Xamarin.Forms.Pages
{
	public abstract class BaseDataSource : IDataSource, INotifyPropertyChanged
	{
		readonly DataSourceList _dataSourceList = new DataSourceList();
		bool _initialized;
		bool _isLoading;

		public IReadOnlyList<IDataItem> Data
		{
			get
			{
				Initialize();
				return _dataSourceList;
			}
		}

		public bool IsLoading
		{
			get { return _isLoading; }
			set
			{
				if (_isLoading == value)
					return;
				_isLoading = value;
				OnPropertyChanged();
			}
		}

		public object this[string key]
		{
			get
			{
				Initialize();
				return GetValue(key);
			}
			set
			{
				Initialize();
				if (SetValue(key, value))
					OnKeyChanged(key);
			}
		}

		IEnumerable<string> IDataSource.MaskedKeys => _dataSourceList.MaskedKeys;

		void IDataSource.MaskKey(string key)
		{
			Initialize();
			_dataSourceList.MaskKey(key);
		}

		void IDataSource.UnmaskKey(string key)
		{
			Initialize();
			_dataSourceList.UnmaskKey(key);
		}

		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add { PropertyChanged += value; }
			remove { PropertyChanged -= value; }
		}

		protected abstract Task<IList<IDataItem>> GetRawData();

		protected abstract object GetValue(string key);

		protected void OnPropertyChanged([CallerMemberName] string property = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
		}

		protected abstract bool SetValue(string key, object value);

		async void Initialize()
		{
			// Do this lazy because GetRawData is virtual and calling it in the ctor is therefor unfriendly
			if (_initialized)
				return;
			_initialized = true;
			IList<IDataItem> rawData = await GetRawData();
			if (!(rawData is INotifyCollectionChanged))
			{
				Log.Warning("Xamarin.Forms.Pages", "DataSource does not implement INotifyCollectionChanged, updates will not be reflected");
				rawData = rawData.ToList(); // Make a copy so we can be sure this list wont change out from under us
			}
			_dataSourceList.MainList = rawData;

			// Test if INPC("Item") is enough to trigger a full reset rather than triggering a new event for each key?
			foreach (IDataItem dataItem in rawData)
			{
				OnKeyChanged(dataItem.Name);
			}
		}

		void OnKeyChanged(string key)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs($"Item[{key}]"));
		}

		event PropertyChangedEventHandler PropertyChanged;
	}
}