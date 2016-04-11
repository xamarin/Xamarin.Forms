﻿using System;
using System.Windows.Input;
using WDataTemplate = Windows.UI.Xaml.DataTemplate;
using WApplication = Windows.UI.Xaml.Application;

#if WINDOWS_UWP

namespace Xamarin.Forms.Platform.UWP
#else

namespace Xamarin.Forms.Platform.WinRT
#endif
{
	public class TextCellRenderer : ICellRenderer
	{
		public virtual WDataTemplate GetTemplate(Cell cell)
		{
			if (cell.RealParent is ListView)
			{
				if (TemplatedItemsList<ItemsView<Cell>, Cell>.GetIsGroupHeader(cell))
					return (WDataTemplate)WApplication.Current.Resources["ListViewHeaderTextCell"];

				//return (WDataTemplate) WApplication.Current.Resources["ListViewTextCell"];
			}

			return (WDataTemplate)WApplication.Current.Resources["TextCell"];
		}
	}

	public class EntryCellRendererCompleted : ICommand
	{
		public bool CanExecute(object parameter)
		{
			return true;
		}

#pragma warning disable 0067 // Revisit: Can't remove; required by interface
		public event EventHandler CanExecuteChanged;
#pragma warning restore

		public void Execute(object parameter)
		{
			var entryCell = (EntryCell)parameter;
			entryCell.SendCompleted();
		}
	}

	public class EntryCellRenderer : ICellRenderer
	{
		public virtual WDataTemplate GetTemplate(Cell cell)
		{
			return (WDataTemplate)WApplication.Current.Resources["EntryCell"];
		}
	}

	public class ViewCellRenderer : ICellRenderer
	{
		public virtual WDataTemplate GetTemplate(Cell cell)
		{
			return (WDataTemplate)WApplication.Current.Resources["ViewCell"];
		}
	}

	public class SwitchCellRenderer : ICellRenderer
	{
		public virtual WDataTemplate GetTemplate(Cell cell)
		{
			return (WDataTemplate)WApplication.Current.Resources["SwitchCell"];
		}
	}

	public class ImageCellRenderer : ICellRenderer
	{
		public virtual WDataTemplate GetTemplate(Cell cell)
		{
			//if (cell.Parent is ListView)
			//	return (WDataTemplate)WApplication.Current.Resources["ListImageCell"];
			return (WDataTemplate)WApplication.Current.Resources["ImageCell"];
		}
	}
}