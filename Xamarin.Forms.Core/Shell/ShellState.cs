//using System.Collections.ObjectModel;

//namespace Xamarin.Forms
//{
//	public class ShellState
//	{
//		internal string Route => _state.Route;
//		readonly Shell _state;
//		public ShellState(Shell state)
//		{
//			_state = state;
//			ObservableCollection<ShellItemState> shellItems = new ObservableCollection<ShellItemState>();
//			foreach (var item in state.Items)
//				shellItems.Add(new ShellItemState(item));

//			Items = new ReadOnlyCollection<ShellItemState>(shellItems);
//		}
//		public ReadOnlyCollection<ShellItemState> Items { get; }

//		public ShellRouteState RouteState { get; }

//		public ShellItemState CurrentItem
//		{
//			get
//			{
//				foreach (var item in Items)
//					if (item.Item == _state.CurrentItem)
//						return item;

//				return null;
//			}
//		}

//		// TODO DELETE ME
//		public static implicit operator ShellState(Shell shell)
//		{
//			return new ShellState(shell);
//		}
//	}
//	public class ShellItemState
//	{
//		internal ShellItem Item { get; }
//		public ReadOnlyCollection<ShellSectionState> Items { get; }

//		public ShellItemState(ShellItem item)
//		{
//			Item = item;
//			ObservableCollection<ShellSectionState> shellItems = new ObservableCollection<ShellSectionState>();
//			foreach (var section in Item.Items)
//				shellItems.Add(new ShellSectionState(section));

//			Items = new ReadOnlyCollection<ShellSectionState>(shellItems);
//		}
//		public ShellSectionState CurrentItem
//		{
//			get
//			{
//				foreach (var item in Items)
//					if (item.Section == Item.CurrentItem)
//						return item;

//				return null;
//			}
//		}

//		public string Route => Item.Route;
//	}

//	public class ShellSectionState
//	{
//		internal ShellSection Section { get; }
//		public ReadOnlyCollection<ShellContentState> Items { get; }

//		public ShellSectionState(ShellSection section)
//		{
//			Section = section;
//			ObservableCollection<ShellContentState> shellItems = new ObservableCollection<ShellContentState>();
//			foreach (var content in Section.Items)
//				shellItems.Add(new ShellContentState(content));

//			Items = new ReadOnlyCollection<ShellContentState>(shellItems);
//		}
//		public ShellContentState CurrentItem
//		{
//			get
//			{
//				foreach (var item in Items)
//					if (item.Content == Section.CurrentItem)
//						return item;

//				return null;
//			}
//		}
//		public string Route => Section.Route;
//	}

//	public class ShellContentState
//	{
//		internal ShellContent Content { get; }
//		public ShellContentState(ShellContent content)
//		{
//			Content = content;
//		}
//		public string Route => Content.Route;
//	}

//	public interface IShellStateCreator
//	{
//		ShellState CreateShellState(Shell shell);
//	}
//}
