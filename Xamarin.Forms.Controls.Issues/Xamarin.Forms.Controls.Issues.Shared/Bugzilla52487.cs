using Xamarin.Forms.CustomAttributes;
using Xamarin.Forms.Internals;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Concurrent;
using System.Threading;

#if UITEST
using Xamarin.UITest;
using NUnit.Framework;
#endif

namespace Xamarin.Forms.Controls.Issues
{
	[Preserve(AllMembers = true)]
	[Issue(
		IssueTracker.Bugzilla,
		52487,
		"ListView with Recycle + HasUnevenRows generates lots (and lots!) of content view",
		// https://bugzilla.xamarin.com/show_bug.cgi?id=52487
		PlatformAffected.iOS
	)]
	public class Bugzilla52487 : TestContentPage
	{
		const int CountFontSize = 12;
		const int CellFontSize = 12;
		const int MinScrollDelta = 2;
		const int ItemsCount = 1000;
		const int GroupCount = 100;
		const int DefaultItemHeight = 300 / 4;
		const int MinimumItemHeight = 40;

		// generate alternate item type when % item id is zero
		const int ItemTypeModulous = 7;

		// render half height when % item id is zero (RecycleElement or RecycleElementAndDataTemplate)
		const int HalfHeightModulous = 5;

		// select alternate cell type when % item id is zero (RecycleElement)
		const int DataTemplateModulous = 3;

		static Tuple<int, int, int> Mix = new Tuple<int, int, int>(255, 255, 100);
		static Tuple<int, int, int> AltMix = new Tuple<int, int, int>(100, 100, 255);
		static IEnumerable<Color> ColorGenerator(Tuple<int, int, int> mix)
		{
			double colorDelta = 0;
			while (true)
			{
				colorDelta += 2 * Math.PI / 100;
				var r = (Math.Sin(colorDelta) + 1) / 2 * 255;
				var g = (Math.Sin(colorDelta * 2) + 1) / 2 * 255;
				var b = (Math.Sin(colorDelta * 3) + 1) / 2 * 255;

				if (mix != null)
				{
					r = (r + mix.Item1) / 2;
					g = (g + mix.Item2) / 2;
					b = (b + mix.Item3) / 2;
				}

				yield return Color.FromRgb((int)r, (int)g, (int)b);
			}
		}

		[Preserve(AllMembers = true)]
		class LazyReadOnlyList<V> : IReadOnlyList<V>
			where V : class
		{
			int _count;
			object _context;
			List<WeakReference<V>> _items;
			Action<int> _onAsk;
			Func<LazyReadOnlyList<V>, int, object, V> _activate;

			internal LazyReadOnlyList(
				int count, 
				object context,
				Action<int> onAsk,
				Func<LazyReadOnlyList<V>, int, object, V> activate)
			{
				_count = count;
				_context = context;
				_onAsk = onAsk;
				_activate = activate;
				_items = new List<WeakReference<V>>(
					Enumerable.Range(0, count)
					.Select(o => new WeakReference<V>(null))
				);
			}

			protected object Context
			{
				get { return _context; }
				set { _context = value; }
			}
			protected IEnumerable<WeakReference<V>> WeakItems => 
				_items;

			public V this[int index]
			{
				get
				{
					_onAsk(index);

					var weakItem = _items[index];

					V item;
					if (!weakItem.TryGetTarget(out item))
					{
						_items[index] = 
							new WeakReference<V>(
								item = _activate(this, index, _context));
					}

					return item;
				}
			}

			public int Count
				=> _count;

			public IEnumerator<V> GetEnumerator()
			{
				for (var i = 0; i < Count; i++)
					yield return this[i];
			}

			IEnumerator IEnumerable.GetEnumerator()
				=> GetEnumerator();
		}

		[Preserve(AllMembers = true)]
		public abstract partial class ListViewSpy<T> : ListViewSpy
		{
			[Preserve(AllMembers = true)] 
			abstract class Selector : DataTemplateSelector
			{
				[Preserve(AllMembers = true)] 
				internal class SelectByData : Selector
				{
					public SelectByData() : base(
						typeof(ItemViewCell.Selected.ByDataNormal), 
						typeof(ItemViewCell.Selected.ByDataAlternate)
					) { }

					protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
					{
						// RecycleElement previously placed no restraint on the type of view 
						// the resulting DataTemplate could return. So the resulting DataTemplate
						// could randomly pick a view type to render items even between appearances
						// on screen. 

						// After this fix, the DataTempate will be required to return the same
						// type of view although the type need not be a function of only the item type.
						// So the type of view a DataTemplates chooses to return can be a function of
						// the item _data_ and not only an items type.

						__counter.OnSelectTemplate++;

						// item could be either Item.Full type or Item.Half type...
						if (!(item is Item.Normal) && !(item is Item.Alternate))
							throw new ArgumentException();

						// ... but selector chooses DataTemplate strictly via item _data_.
						return ((Item)item).Id % DataTemplateModulous == 0 ? 
							_dataTemplateAlt : _dataTemplate;
					}
				}

				[Preserve(AllMembers = true)]
				internal class SelectByType : Selector
				{
					public SelectByType() : base(
						typeof(ItemViewCell.Selected.ByTypeNormal), 
						typeof(ItemViewCell.Selected.ByTypeAlternate)
					) { }

					protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
					{
						// RecycleElementAndDataTemplate requires that 
						// DataTempalte be a function of the item _type_

						__counter.OnSelectTemplate++;

						if (item is Item.Normal)
							return _dataTemplate;

						if (item is Item.Alternate)
							return _dataTemplateAlt;

						throw new ArgumentException();
					}
				}

				DataTemplate _dataTemplate;
				DataTemplate _dataTemplateAlt;

				public Selector(Type nomral, Type alternate)
				{
					// RecycleElementAndDataTemplate requires 
					// that the DataTemplate use the .ctor that takes a type
					_dataTemplate = new DataTemplate(nomral);
					_dataTemplateAlt = new DataTemplate(alternate);
				}
			}

			[Preserve(AllMembers = true)]
			abstract class ItemViewCell : ViewCell
			{
				internal abstract class Selected : ItemViewCell
				{
					internal class ByTypeNormal : ByType
					{
						static Color NextColor() { Colors.MoveNext(); return Colors.Current; }
						static readonly IEnumerator<Color> Colors = ColorGenerator(Mix).GetEnumerator();

						public ByTypeNormal() : base(NextColor()) { }
					}
					internal class ByTypeAlternate : ByType
					{
						static Color NextColor() { Colors.MoveNext(); return Colors.Current; }
						static readonly IEnumerator<Color> Colors = ColorGenerator(AltMix).GetEnumerator();

						public ByTypeAlternate() : base(NextColor()) { }

						protected override bool IsAlternate => true;
					}
					internal abstract class ByType : Selected
					{
						internal ByType(Color color)
							: base(color) { }

						protected override void OnBindingContextChanged()
						{
							base.OnBindingContextChanged();

							if (BindingContext == null)
								return;

							// check that template is a function of the item type
							var itemType = BindingContext.GetType();
							var itemIsNormalType = itemType == typeof(Item.Normal);
							var expectedTemplateType = itemIsNormalType ? typeof(ByTypeNormal) : typeof(ByTypeAlternate);

							var templateType = GetType();
							if (templateType != expectedTemplateType)
								throw new ArgumentException(
									$"BindingContext.GetType() = {itemType.Name}, " + 
									$"TemplateType {templateType.Name}!={expectedTemplateType.Name}");
						}
					}

					internal class ByDataNormal : ByData
					{
						static Color NextColor() { Colors.MoveNext(); return Colors.Current; }
						static readonly IEnumerator<Color> Colors = ColorGenerator(Mix).GetEnumerator();

						public ByDataNormal() : base(NextColor()) { }
					}
					internal class ByDataAlternate : ByData
					{
						static Color NextColor() { Colors.MoveNext(); return Colors.Current; }
						static readonly IEnumerator<Color> Colors = ColorGenerator(AltMix).GetEnumerator();

						public ByDataAlternate() : base(NextColor()) { }

						protected override bool IsAlternate => true;
					}
					internal abstract class ByData : Selected
					{
						internal ByData(Color color)
							: base(color) { }

						protected override void OnBindingContextChanged()
						{
							base.OnBindingContextChanged();

							if (BindingContext == null)
								return;

							// check that template is a function of the item data
							var isRemainderZero = BindingContext.Id % DataTemplateModulous == 0;
							var expectedItemType = isRemainderZero ? typeof(Item.Alternate) : typeof(Item.Normal);
							var expectedTemplateType = isRemainderZero ? typeof(ByDataAlternate) : typeof(ByDataNormal);

							var templateType = GetType();
							if (templateType != expectedTemplateType)
								throw new ArgumentException(
									$"Item.Id = {BindingContext?.Id}, " +
									$"TemplateType {templateType.Name}!={expectedTemplateType.Name}");
						}
					}

					internal Selected(Color color)
						: base(color) { }
				}

				internal class Constant : ItemViewCell {
					static Color NextColor() { Colors.MoveNext(); return Colors.Current; }
					static readonly IEnumerator<Color> Colors = ColorGenerator(Mix).GetEnumerator();

					public Constant() : base(NextColor()) { }
				}

				readonly int _id;
				readonly Label _label;

				ItemViewCell(Color color)
				{
					_id = __counter.CellAlloc++;

					View = _label = new Label
					{
						BackgroundColor = color,
						VerticalTextAlignment = TextAlignment.Center,
						HorizontalTextAlignment = TextAlignment.Center,
						FontSize = CellFontSize
					};

					_label.SetBinding(HeightRequestProperty, nameof(Item.Value));
				}

				Item BindingContext
					=> (Item)base.BindingContext;
				int ItemId
					=> BindingContext.Id;
				int? ItemGroupId
					=> BindingContext.GroupId;
				bool IsAlternateItem 
					=> BindingContext is Item.Alternate;
			
				protected virtual bool IsAlternate => false;

				protected override void OnBindingContextChanged()
				{
					base.OnBindingContextChanged();

					__counter.CellBind++;

					if (BindingContext == null)
						return;

					// double check that item generator returned correct type of item
					var isRemainderZero = BindingContext.Id % ItemTypeModulous == 0;
					var expectedItemType = isRemainderZero ? typeof(Item.Alternate) : typeof(Item.Normal);
					var itemType = BindingContext.GetType();
					if (itemType != expectedItemType)
						throw new ArgumentException(
							$"Item.Id = {BindingContext?.Id}, ItemType {GetType().Name}!={expectedItemType.Name}");
				}

				protected override void OnAppearing()
				{
					_label.Text = ToString();
					__counter.AttachCell(_id);
				}

				public new int Id
					=> _id;

				// cell type is (1) constant or a function of the the Item (2) data or (3) type
				public override string ToString()
					=> $"{ItemId}" + 
						(ItemGroupId == null ? "" : "/" + ItemGroupId) +
							$"{(IsAlternateItem ? "*" : "")} ->" +
								$" {_id}{(IsAlternate ? "*" : "")}";

				~ItemViewCell()
				{
					int id;
					__counter.CellFree++;
					__counter.DetachCell(_id);
					// update would be off UI thread
				}
			}

			[Preserve(AllMembers = true)]
			abstract class Item : INotifyPropertyChanged
			{
				internal class Normal : Item
				{
					internal Normal(int id, int? groupId, int height) 
						: base( id, groupId,height) { }
				};

				internal class Alternate : Item
				{
					internal Alternate(int id, int? groupId, int height) 
						: base(id, groupId, height) { }
				};

				internal static Item Create(LazyItemList list, int index, int height) {

					var id = list.ItemIdOffset + index;

					if (id % ItemTypeModulous == 0)
						return new Alternate(id, list.Id, height);

					return new Normal(id, list.Id, height);
				}

				int _allocId;

				int _id;
				int _index;
				int? _groupId;
				int _height;

				private Item(int id, int? groupId, int height)
				{
					_allocId = Interlocked.Increment(ref __counter.ItemAlloc);

					if (id % HalfHeightModulous == 0)
						height =  height / 2;

					_groupId = groupId;
					_height = height;
					_id = id;
				}

				public int Id
					=> _id;
				public int? GroupId
					=> _groupId;
				public int Value
				{
					get { return _height; }
					set
					{
						_height = value;
						OnPropertyChanged();
					}
				}

				public event PropertyChangedEventHandler PropertyChanged;
				protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
					=> PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

				public override string ToString()
					=> _groupId == null ? 
						$"{Id}, value={Value}, _alloc={_allocId}" :
						$"{Id}, group={GroupId}, value={Value}, _alloc={_allocId}";

				~Item()
				{
					Interlocked.Increment(ref __counter.ItemFree);
				}
			}

			interface IItemList : IEnumerable, IDisposable
			{
				void UpdateHeights(double multipule);
				int Count { get; }
				void Dispose();
			}

			[Preserve(AllMembers = true)]
			class LazyItemList : LazyReadOnlyList<Item>, IItemList
			{
				int _itemIdOffset;
				int? _id;
				int _count;

				internal LazyItemList(int count)
					: this(null /* grouping disabled */, 0, count) { }
				internal LazyItemList(int? id, int itemIdOffset, int count)
					: base(count, DefaultItemHeight, 
						onAsk: o => __counter.ViewModelAsk.Add(o), 
						activate: (self, subIndex, height) => 
							Item.Create(
								list: (LazyItemList)self, 
								index: subIndex, 
								height: (int)height
							) 
					)
				{
					_id = id;
					_itemIdOffset = itemIdOffset;
					_count = count;
				}

				protected int Context
				{
					get { return (int)base.Context; }
					set { base.Context = value; }
				}

				public void UpdateHeights(double multipule)
				{
					if (multipule < 1 && Context < MinimumItemHeight)
						return;

					Context = (int)(Context * multipule);

					foreach (var weakItem in WeakItems)
					{
						Item item;
						if (!(weakItem.TryGetTarget(out item)))
							continue;

						item.Value = Context;
					}
				}
				public int ItemIdOffset
					=> _itemIdOffset;
				public int? Id
					=> _id;

				public void Dispose()
				{
					foreach (var weakItem in WeakItems)
						weakItem.SetTarget(null);
				}

				public override string ToString()
					=> $"{_id}";
			}

			[Preserve(AllMembers = true)]
			class LazyGroupedItemList : LazyReadOnlyList<LazyItemList>, IItemList
			{
				internal LazyGroupedItemList(int numberOfGroups, int count)
					: base(numberOfGroups, 
						  context: null,
						  onAsk: o => __counter.ViewModelAsk.Add(o), 
						  activate: (self, groupId, context) => 
							new LazyItemList(
								id: groupId,
								itemIdOffset: groupId * (count / numberOfGroups),
								count: count / numberOfGroups
							)
						)
				{ }

				public void UpdateHeights(double multipule)
				{
					foreach (var weakItem in WeakItems)
					{
						LazyItemList group;
						if (!(weakItem.TryGetTarget(out group)))
							continue;

						group.UpdateHeights(multipule);
					}
				}

				public void Dispose()
				{
					foreach (var weakItem in WeakItems)
					{
						LazyItemList group;
						if (!(weakItem.TryGetTarget(out group)))
							continue;

						group.Dispose();
					}
				}
			}

			[Preserve(AllMembers = true)]
			class Counter
			{
				internal int CellAlloc;
				internal int CellFree;
				internal int CellBind;

				internal int ItemAlloc;
				internal int ItemFree;
				internal int OnSelectTemplate;
				internal HashSet<int> ViewModelAsk 
					= new HashSet<int>();

				HashSet<int> CellAttached 
					= new HashSet<int>();

				internal int AttachedCells
				{
					get
					{
						lock (this)
							return CellAttached.Count;
					}
				}
				internal void AttachCell(int id)
				{
					lock (this)
						CellAttached.Add(id);
				}
				internal void DetachCell(int id)
				{
					lock (this)
						CellAttached.Remove(id);
				}
			}

			[Preserve(AllMembers = true)]
			class CounterView : StackLayout
			{
				static Label CreateLabel()
					=> new Label() { FontSize = CountFontSize };

				internal void Update()
				{
					ViewCountLabel.Text
						= $"View={__counter.CellAlloc - __counter.CellFree}";
					AttachedCountLabel.Text
						= $"Atch={__counter.AttachedCells}";
					BindCountLabel.Text
						= $"Bind={__counter.CellBind}";
					ItemCountLabel.Text
						= $"Item={__counter.ItemAlloc - __counter.ItemFree}";
					AskLabel.Text
						= $"Ask={__counter.OnSelectTemplate}";
				}

				Label AttachedCountLabel = CreateLabel();
				Label ViewCountLabel = CreateLabel();
				Label BindCountLabel = CreateLabel();
				Label ItemCountLabel = CreateLabel();
				Label AskLabel = CreateLabel();

				internal CounterView()
				{
					Children.Add(ViewCountLabel);
					Children.Add(AttachedCountLabel);
					Children.Add(BindCountLabel);
					Children.Add(ItemCountLabel);
					Children.Add(AskLabel);
					Update();
				}
			}

			// Cell is activated via DataTemplate using default ctor which
			// makes it difficult to pass the counter to the cell. So we make
			// it static to give cell access and create a different generic
			// instantiation for each type of ListView to get different counters
			static Counter __counter = new Counter();

			int _appeared;
			int _disappeared;
			IItemList _itemsList;

			public ListViewSpy()
			{
				__listViewSpyAlloc++;

				var name = GetType().Name;

				var hasUnevenRows = name.Contains("UnevenRows");

				var isGrouped = name.Contains("Grouped");

				_itemsList = isGrouped ? (IItemList)
					new LazyGroupedItemList(GroupCount, ItemsCount) :
					new LazyItemList(ItemsCount);

				var strategy =
					name.Contains("RecycleElementAndDataTemplate") ? ListViewCachingStrategy.RecycleElementAndDataTemplate :
					name.Contains("RecycleElement") ? ListViewCachingStrategy.RecycleElement :
					ListViewCachingStrategy.RetainElement;

				var dataTemplate =
					strategy == ListViewCachingStrategy.RecycleElement ? new Selector.SelectByData() :
					strategy == ListViewCachingStrategy.RecycleElementAndDataTemplate ? new Selector.SelectByType() :
					new DataTemplate(typeof(ItemViewCell.Constant));

				var listView = new ListView(strategy)
				{
					HasUnevenRows = hasUnevenRows,
					// see https://github.com/xamarin/Xamarin.Forms/pull/994/files
					//RowHeight = 50,
					ItemsSource = _itemsList,
					ItemTemplate = dataTemplate,

					IsGroupingEnabled = isGrouped,
					GroupDisplayBinding = null,
					GroupShortNameBinding = null,
					GroupHeaderTemplate = null
				};
				Children.Add(listView);

				listView.AutomationId = $"{GetType().Name}:ListView";

				var counter = new CounterView();

				listView.ItemAppearing += (o, e) =>
				{
					_appeared = ((Item)e.Item).Id;
					counter.Update();
				};

				listView.ItemDisappearing += (o, e) =>
				{
					_disappeared = ((Item)e.Item).Id;
					counter.Update();
				};

				Children.Add(counter);
			}

			internal override void Down()
			{
				var target = Math.Max(_appeared, _disappeared);
				target += Math.Abs(_appeared - _disappeared) + MinScrollDelta;
				if (target >= _itemsList.Count)
					target = _itemsList.Count - 1;

				//_listView.ScrollTo(_itemsList[target], ScrollToPosition.MakeVisible, animated: true);
			}
			internal override void Up()
			{
				var target = Math.Min(_appeared, _disappeared);
				target -= Math.Abs(_appeared - _disappeared) + MinScrollDelta;
				if (target < 0)
					target = 0;

				//_listView.ScrollTo(_itemsList[target], ScrollToPosition.MakeVisible, animated: true);
			}
			internal override void UpdateHeights(double multipule)
				=> _itemsList.UpdateHeights(multipule);

			internal override void Dispose()
				=> _itemsList.Dispose();

			~ListViewSpy()
			{
				__listViewSpyFree++;
			}
		}

		[Preserve(AllMembers = true)]
		public abstract partial class ListViewSpy : StackLayout
		{
			[Preserve(AllMembers = true)]
			internal sealed class Retain :
				ListViewSpy<Retain> { }

			[Preserve(AllMembers = true)]
			internal sealed class UnevenRowsRecycleElement : 
				ListViewSpy<UnevenRowsRecycleElement> { }

			[Preserve(AllMembers = true)]
			internal sealed class UnevenRowsRecycleElementAndDataTemplate :
				ListViewSpy<UnevenRowsRecycleElementAndDataTemplate> { }

			[Preserve(AllMembers = true)]
			internal sealed class EvenRowsRecycleElement :
				ListViewSpy<EvenRowsRecycleElement> { }

			[Preserve(AllMembers = true)]
			internal sealed class EvenRowsRecycleElementAndDataTemplate :
				ListViewSpy<EvenRowsRecycleElementAndDataTemplate> { }

			[Preserve(AllMembers = true)]
			internal sealed class GroupedRetain :
				ListViewSpy<GroupedRetain> { }

			[Preserve(AllMembers = true)]
			internal sealed class GroupedUnevenRowsRecycleElement :
				ListViewSpy<GroupedUnevenRowsRecycleElement> { }

			[Preserve(AllMembers = true)]
			internal sealed class GroupedUnevenRowsRecycleElementAndDataTemplate :
				ListViewSpy<GroupedUnevenRowsRecycleElementAndDataTemplate> { }

			[Preserve(AllMembers = true)]
			internal sealed class GroupedEvenRowsRecycleElement :
				ListViewSpy<GroupedEvenRowsRecycleElement> { }

			[Preserve(AllMembers = true)]
			internal sealed class GroupedEvenRowsRecycleElementAndDataTemplate :
				ListViewSpy<GroupedEvenRowsRecycleElementAndDataTemplate> { }

			internal abstract void Down();
			internal abstract void Up();
			internal abstract void UpdateHeights(double difference);
			internal abstract void Dispose();
		}

		static int __listViewSpyAlloc;
		static int __listViewSpyFree;
		ListViewSpy[] __listViews; // Careful! Be sure you don't close over this!

		IEnumerable<ListViewSpy> ListViews()
			=> __listViews ?? Enumerable.Empty<ListViewSpy>();

		void Update()
			=> Title = $"ListViews={__listViewSpyAlloc - __listViewSpyFree}";

		Grid RecycleListViews(bool group = false) {

			// reclaim
			foreach (var o in ListViews())
				o.Dispose();

			GC.Collect();
			GC.WaitForPendingFinalizers();

			__listViews = group ? 
				new ListViewSpy[]
				{
					//new ListViewSpy.GroupedRetain(),
					//new ListViewSpy.GroupedEvenRowsRecycleElement(),
					//new ListViewSpy.GroupedEvenRowsRecycleElementAndDataTemplate(),
					//new ListViewSpy.GroupedUnevenRowsRecycleElement(),
					new ListViewSpy.GroupedUnevenRowsRecycleElementAndDataTemplate(),
				} :
				new ListViewSpy[]
				{
					//new ListViewSpy.Retain(),
					//new ListViewSpy.EvenRowsRecycleElement(),
					//new ListViewSpy.EvenRowsRecycleElementAndDataTemplate(),
					//new ListViewSpy.UnevenRowsRecycleElement(),
					new ListViewSpy.UnevenRowsRecycleElementAndDataTemplate(),
				};

			var grid = new Grid();
			foreach (var o in __listViews)
				grid.Children.AddHorizontal(o);

			Update();

			return grid;
		}

		protected override void Init()
		{
			var stackLayout = new StackLayout();
			var buttonsGrid = new Grid();

			var moreButton = new Button() { Text = "x2" };
			moreButton.Clicked += (o, s)
				=> ListViews().ForEach(x => x.UpdateHeights(2));
			buttonsGrid.Children.AddHorizontal(moreButton);

			var lessButton = new Button() { Text = $"{'\u00F7'}2" };
			lessButton.Clicked += (o, s)
				=> ListViews().ForEach(x => x.UpdateHeights(.5));
			buttonsGrid.Children.AddHorizontal(lessButton);

			var upButton = new Button() { Text = "Up" };
			upButton.Clicked += (o, s)
				=> ListViews().ForEach(x => x.Up());
			buttonsGrid.Children.AddHorizontal(upButton);

			var downButton = new Button() { Text = "Down" };
			downButton.Clicked += (o, s)
				=> ListViews().ForEach(x => x.Down());
			buttonsGrid.Children.AddHorizontal(downButton);

			var listViewGrid = new ContentView();

			var groupSwitch = new Switch();
			groupSwitch.Toggled += (o, s)
				=> listViewGrid.Content = RecycleListViews(group: s.Value);
			buttonsGrid.Children.AddHorizontal(groupSwitch);

			listViewGrid.Content = RecycleListViews(group: groupSwitch.IsToggled);

			Content = new StackLayout {
				Children = {
					listViewGrid,
					buttonsGrid,
				}
			};

			AutomationId = Id.Test;

			Update();
		}
		public static class Id
		{
			public static string WaitForever = nameof(WaitForever);
			public static string Test = nameof(Bugzilla52487);
			public static string ListView = nameof(ListViewSpy.UnevenRowsRecycleElement);
		}
#if UITEST
		[Test]
		public void Bugzilla56896Test()
		{
			RunningApp.WaitForElement(Id.Test);
			RunningApp.ScrollUp(Id.ListView);
			RunningApp.WaitForElement(Id.WaitForever);
		}
#endif
	}
}