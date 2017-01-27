using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;

namespace Xamarin.Forms.Controls
{
	[Preserve(AllMembers = true)]
	[Issue(IssueTracker.Github, 1762,
		"Binding issue with SwitchCell - System.ArgumentException:'jobject' must not be IntPtr.Zero",
		PlatformAffected.Android)]
	public class Issue1762 : ContentPage
	{
		public static ObservableGroupMyObjCollection Objs = new ObservableGroupMyObjCollection();

		public Issue1762()
		{
			var stack = new StackLayout
				{ };
			stack.Children.Add(new ListView
			{
				ItemsSource = Objs,
				ItemTemplate = new DataTemplate(() =>
				{
					var cell = new SwitchCell();
					cell.SetBinding(SwitchCell.TextProperty, "DisplayText");
					cell.SetBinding(SwitchCell.OnProperty, "IsSelected");
					return cell;
				}),
				IsGroupingEnabled = true,
				GroupDisplayBinding = new Binding("Key")
			});
			var b = new Button
			{
				Text = "add"
			};
			b.Clicked += (sender, e) =>
			{
				var r = new Random();
				Objs.Add(new MyObj
				{
					DisplayText = r.Next().ToString(),
					IsSelected = r.Next() % 2 == 0
				});
			};
			stack.Children.Add(b);

			Content = stack;
		}
	}

	public class Grouping<K, T> : ObservableCollection<T>, IGroupingCollection<K, T> where T : INotifyPropertyChanged
	{
		public Grouping(K key, IEnumerable<T> items)
		{
			Key = key;
			foreach (T item in items)
				Items.Add(item);
		}

		public Grouping(K key, T item)
		{
			Key = key;
			Items.Add(item);
		}

		public K Key { get; private set; }
	}

	public static class Extensions
	{
		public static IEnumerable<T> Enumerate<T>(this IEnumerable<IEnumerable<T>> listOfList)
		{
			foreach (IEnumerable<T> list in listOfList)
			{
				foreach (T item in list)
					yield return item;
			}
		}
	}

	public interface IGroupingCollection<K, T> : ICollection<T>, IGrouping<K, T>
	{
	}

	public interface ISortingKey<T>
	{
		T SortingKey { get; }
	}

	public class MyObj : ObservableObject, ISortingKey<bool>
	{
		string _displayText;

		bool _isSelected;

		public MyObj()
		{
		}

		public string DisplayText
		{
			get { return _displayText; }
			set { SetProperty(ref _displayText, value); }
		}

		public bool IsSelected
		{
			get { return _isSelected; }
			set
			{
				if (SetProperty(ref _isSelected, value))
					NotifyPropertyChanged(() => SortingKey);
			}
		}

		#region ISortingKey implementation

		public bool SortingKey
		{
			get { return IsSelected; }
		}

		#endregion
	}

	public abstract class ObservableGroupCollection<K, T> : ObservableCollection<IGroupingCollection<K, T>>
		where T : class, INotifyPropertyChanged, ISortingKey<K>
	{
		Func<K, K, bool> _equalityComparer;

		public ObservableGroupCollection(IEnumerable<IGroupingCollection<K, T>> items, Func<K, K, bool> equalityComparer)
			: base(items)
		{
			_equalityComparer = equalityComparer;
			if (items != null)
			{
				foreach (T propChangeItem in items.Enumerate())
					SetupPropertyChanged(propChangeItem, equalityComparer);
			}
		}

		public ObservableGroupCollection(IGroupingCollection<K, T> item, Func<K, K, bool> equalityComparer)
		{
			_equalityComparer = equalityComparer;
			if (item != null)
			{
				foreach (T t in item)
					SetupPropertyChanged(t, equalityComparer);
			}
		}

		public void Add(T item)
		{
			SetupPropertyChanged(item, _equalityComparer);
			foreach (IGroupingCollection<K, T> group in Items)
			{
				if (_equalityComparer(group.Key, item.SortingKey))
				{
					group.Add(item);
					return;
				}
			}
			var newGroup = new Grouping<K, T>(item.SortingKey, item);
			Items.Add(newGroup);
			OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newGroup));
		}

		void SetupPropertyChanged(T propChangeItem, Func<K, K, bool> equalityComparer)
		{
			propChangeItem.PropertyChanged += (sender, e) =>
			{
				if (e.PropertyName == "SortingKey")
				{
					//using (BlockReentrancy())
					{
						var changedItem = (T)sender;
						IGroupingCollection<K, T> oldGroup = null, newGroup = null;
						foreach (IGroupingCollection<K, T> group in Items) //go through all groups to find item
						{
							if (oldGroup == null /* || newGroup == null*/)
							{
								foreach (T item2 in group)
								{
									if (oldGroup == null && item2 == changedItem)
										oldGroup = group;
								}
							}
							if (newGroup == null && equalityComparer(group.Key, changedItem.SortingKey))
								newGroup = group;
						}
						if (oldGroup != null)
						{
							oldGroup.Remove(changedItem);
							if (oldGroup.Count == 0)
							{
								OnCollectionChanged(
									new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldGroup));
								Items.Remove(oldGroup);
							}
						}
#if DEBUG
						else
							throw new Exception("oldGroup == null");
#endif
						if (newGroup == null)
						{
							Items.Add(newGroup = new Grouping<K, T>(changedItem.SortingKey, changedItem));
							OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add,
								newGroup));
						}
						else
						{
							foreach (T item in newGroup)
							{
								if (item == changedItem)
									return;
							}
							newGroup.Add(changedItem);
						}
					}
				}
			};
		}

		/*protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			base.OnCollectionChanged(e);
		}*/
	}

	public class ObservableGroupMyObjCollection : ObservableGroupCollection<bool, MyObj>
	{
		public ObservableGroupMyObjCollection()
			: base((IGroupingCollection<bool, MyObj>)null, (k1, k2) => k1 == k2)
		{
		}
	}

	public abstract class ObservableObject : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
		{
			OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
		}

		protected virtual void NotifyPropertyChanged<T>(Expression<Func<T>> propertyExpression)
		{
			string propertyName = GetPropertyName(propertyExpression);
			OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
		}

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			PropertyChangedEventHandler eventHandler = PropertyChanged;
			if (eventHandler != null)
			{
				try
				{
					eventHandler(this, e); //crashes here!
				}
				catch (Exception ex)
				{
					System.Diagnostics.Debug.WriteLine(ex);
				}
			}
		}

		protected bool SetProperty<T>(ref T storage, T value, Expression<Func<T>> propertyExpression)
		{
			string propertyName = GetPropertyName(propertyExpression);
			return SetProperty<T>(ref storage, value, propertyName);
		}

		protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
		{
			if (EqualityComparer<T>.Default.Equals(storage, value))
				return false;

			storage = value;
			NotifyPropertyChanged(propertyName);
			return true;
		}

		string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
		{
			if (propertyExpression == null)
				throw new ArgumentNullException("propertyExpression");

			if (propertyExpression.Body.NodeType != ExpressionType.MemberAccess)
				throw new ArgumentException("Should be a member access lambda expression", "propertyExpression");

			var memberExpression = (MemberExpression)propertyExpression.Body;
			return memberExpression.Member.Name;
		}
	}
}