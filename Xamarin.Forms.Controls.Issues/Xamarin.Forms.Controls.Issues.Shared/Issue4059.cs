using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
using Xamarin.Forms.Core.UITests;
#endif

namespace Xamarin.Forms.Controls.Issues
{
#if UITEST
	[Category(UITestCategories.ListView)]
#endif
	[Preserve (AllMembers = true)]
	[Issue (IssueTracker.Github, 4059, "Bad performance on UWP ListView if ItemsSource is not an ObservableCollection", PlatformAffected.UWP)]
	public class Issue4059 : TestContentPage
	{
		[Preserve(AllMembers = true)]
		class NonObservableCustomCollection : IList, INotifyCollectionChanged, IEnumerator
		{
			List<object> _hash;
			int _index;

			public NonObservableCustomCollection(int count)
			{
				Count = count;
				_hash = Enumerable.Range(0, count).Select(x => (object)null).ToList();
				_index = 0;
			}

			public int LoadedItems { get; private set; }

			object SimulateAsyncGetItem(int i)
			{
				if (_hash[i] != null)
					return _hash[i];
				_hash[i] = $"item {i}";
				LoadedItems++;
				ItemLoaded?.Invoke(this, null);
				return _hash[i];
			}

			public object Current => SimulateAsyncGetItem(_index - 1);

			public bool MoveNext() => _index < Count ? ++_index > 0 : false;

			public void Reset() => throw new NotImplementedException();

			public int Count { get; private set; }

			public bool IsReadOnly => true;

			public bool IsFixedSize => true;

			public bool IsSynchronized => true;

			public object SyncRoot => null;

			object IList.this[int i]
			{
				get => SimulateAsyncGetItem(i);
				set => _hash[i] = value;
			}

			public void Clear() => throw new NotImplementedException();

			public bool Contains(object value) => _hash.Contains(value);

			public int IndexOf(object value) => _hash.IndexOf(value);

			public void CopyTo(Array array, int index) => throw new NotImplementedException();

			public IEnumerator GetEnumerator() => this;

			public int Add(object value) => throw new NotImplementedException();

			public void Insert(int index, object value) => throw new NotImplementedException();

			public void Remove(object value) => CollectionChanged?.Invoke(this, null);

			public void RemoveAt(int index) => throw new NotImplementedException();

			public event EventHandler ItemLoaded;

			public event NotifyCollectionChangedEventHandler CollectionChanged;
		}

		protected override void Init ()
		{
			var statusLabel = new Label();
			var successLabel = new Label();
			var asyncLoadedItems = new NonObservableCustomCollection(1000);
			asyncLoadedItems.ItemLoaded += (_, __) =>
			{
				statusLabel.Text = $"Loaded: [{asyncLoadedItems.LoadedItems}]";
				successLabel.Text = asyncLoadedItems.LoadedItems < 500 ? "Success" : "Fail";
			};
			var listView = new ListView
			{
				ItemsSource = asyncLoadedItems,
			};
			Content = new StackLayout
			{
				Children = {
					statusLabel,
					new Label() { Text = "Sumulate collection with latency. It should load part of 1000 elements if necessary" },
					listView,
					successLabel
				}
			};
		}

#if UITEST
		[Test]
		public void Issue4059Test()
		{
			RunningApp.WaitForElement("Success");
		}
#endif
	}
}
