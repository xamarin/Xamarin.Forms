﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.WPF.Controls;

namespace Xamarin.Forms.Platform.WPF
{
	public class Platform : BindableObject, INavigation, IDisposable
#pragma warning disable CS0618
		, IPlatform
#pragma warning restore
	{
		readonly FormsApplicationPage _page;

		bool _disposed;
		Page Page { get; set; }

		internal static readonly BindableProperty RendererProperty = BindableProperty.CreateAttached("Renderer", typeof(IVisualElementRenderer), typeof(Platform), default(IVisualElementRenderer));

		internal Platform(FormsApplicationPage page)
		{
			_page = page;


			var busyCount = 0;
			MessagingCenter.Subscribe(this, Page.BusySetSignalName, (Page sender, bool enabled) =>
			{
				busyCount = Math.Max(0, enabled ? busyCount + 1 : busyCount - 1);
			});

			MessagingCenter.Subscribe<Page, SnackbarArguments>(this, Page.SnackbarSignalName, OnPageSnackbar);
			MessagingCenter.Subscribe<Page, AlertArguments>(this, Page.AlertSignalName, OnPageAlert);
			MessagingCenter.Subscribe<Page, ActionSheetArguments>(this, Page.ActionSheetSignalName, OnPageActionSheet);

		}

		async void OnPageAlert(Page sender, AlertArguments options)
		{
			string content = options.Message ?? options.Title ?? string.Empty;

			FormsContentDialog dialog = new FormsContentDialog();

			if (options.Message == null || options.Title == null)
				dialog.Content = content;
			else
			{
				dialog.Title = options.Title;
				dialog.Content = options.Message;
			}

			if (options.Accept != null)
			{
				dialog.IsPrimaryButtonEnabled = true;
				dialog.PrimaryButtonText = options.Accept;
			}

			if (options.Cancel != null)
			{
				dialog.IsSecondaryButtonEnabled = true;
				dialog.SecondaryButtonText = options.Cancel;
			}

			var dialogResult = await dialog.ShowAsync();

			options.SetResult(dialogResult == LightContentDialogResult.Primary);
		}

		Timer _snackbarTimer;
		void OnPageSnackbar(Page sender, SnackbarArguments options)
		{
			if (System.Windows.Application.Current.MainWindow is FormsWindow window)
			{
				_snackbarTimer = new Timer { Interval = options.Duration };
				_snackbarTimer.Tick += delegate
				{
					window.HideSnackBar();
					_snackbarTimer.Stop();
					options.SetResult(false);
				};
				window.OnSnackbarActionExecuted += delegate
				{
					window.HideSnackBar();
					_snackbarTimer.Stop();
					options.SetResult(true);
				};
				_snackbarTimer.Start();
				window.ShowSnackBar(options.Message, options.ActionButtonText, options.Action);
			}
			else
			{
				options.SetResult(false);
			}
		}

		async void OnPageActionSheet(Page sender, ActionSheetArguments options)
		{
			var list = new System.Windows.Controls.ListView
			{
				Style = (System.Windows.Style)System.Windows.Application.Current.Resources["ActionSheetList"],
				ItemsSource = options.Buttons
			};

			var dialog = new FormsContentDialog
			{
				Content = list,
			};

			if (options.Title != null)
				dialog.Title = options.Title;

			list.SelectionChanged += (s, e) =>
			{
				if (list.SelectedItem != null)
				{
					dialog.Hide();
					options.SetResult((string)list.SelectedItem);
				}
			};

			/*_page.KeyDown += (window, e) =>
			 {
				 if (e.Key == System.Windows.Input.Key.Escape)
				 {
					 dialog.Hide();
					 options.SetResult(LightContentDialogResult.None.ToString());
				 }
			 };*/

			if (options.Cancel != null)
			{
				dialog.IsSecondaryButtonEnabled = true;
				dialog.SecondaryButtonText = options.Cancel;
			}

			if (options.Destruction != null)
			{
				dialog.IsPrimaryButtonEnabled = true;
				dialog.PrimaryButtonText = options.Destruction;
			}

			LightContentDialogResult result = await dialog.ShowAsync();
			if (result == LightContentDialogResult.Secondary)
				options.SetResult(options.Cancel);
			else if (result == LightContentDialogResult.Primary)
				options.SetResult(options.Destruction);

		}

		public static SizeRequest GetNativeSize(VisualElement view, double widthConstraint, double heightConstraint)
		{
			if (widthConstraint > 0 && heightConstraint > 0 && GetRenderer(view) != null)
			{
				IVisualElementRenderer element = GetRenderer(view);
				return element.GetDesiredSize(widthConstraint, heightConstraint);
			}

			return new SizeRequest();
		}

		public static IVisualElementRenderer GetOrCreateRenderer(VisualElement element)
		{
			if (GetRenderer(element) == null)
				SetRenderer(element, CreateRenderer(element));

			return GetRenderer(element);
		}

		public static IVisualElementRenderer CreateRenderer(VisualElement element)
		{
			IVisualElementRenderer result = Registrar.Registered.GetHandlerForObject<IVisualElementRenderer>(element) ?? new DefaultViewRenderer();
			result.SetElement(element);
			return result;
		}

		public static IVisualElementRenderer GetRenderer(VisualElement self)
		{
			return (IVisualElementRenderer)self.GetValue(RendererProperty);
		}

		public static void SetRenderer(VisualElement self, IVisualElementRenderer renderer)
		{
			self.SetValue(RendererProperty, renderer);
			self.IsPlatformEnabled = renderer != null;
		}

		internal void SetPage(Page newRoot)
		{
			if (newRoot == null)
				return;

			Page = newRoot;

#pragma warning disable CS0618 // Type or member is obsolete
			// The Platform property is no longer necessary, but we have to set it because some third-party
			// library might still be retrieving it and using it
			Page.Platform = this;
#pragma warning restore CS0618 // Type or member is obsolete

			_page.StartupPage = Page;
			Application.Current.NavigationProxy.Inner = this;
		}


		public IReadOnlyList<Page> NavigationStack
		{
			get { throw new InvalidOperationException("NavigationStack is not supported globally on Windows, please use a NavigationPage."); }
		}

		public IReadOnlyList<Page> ModalStack
		{
			get
			{
				return _page.InternalChildren.Cast<Page>().ToList();
			}
		}

		Task INavigation.PushAsync(Page root)
		{
			return ((INavigation)this).PushAsync(root, true);
		}

		Task<Page> INavigation.PopAsync()
		{
			return ((INavigation)this).PopAsync(true);
		}

		Task INavigation.PopToRootAsync()
		{
			return ((INavigation)this).PopToRootAsync(true);
		}

		Task INavigation.PushAsync(Page root, bool animated)
		{
			throw new InvalidOperationException("PushAsync is not supported globally on Windows, please use a NavigationPage.");
		}

		Task<Page> INavigation.PopAsync(bool animated)
		{
			throw new InvalidOperationException("PopAsync is not supported globally on Windows, please use a NavigationPage.");
		}

		Task INavigation.PopToRootAsync(bool animated)
		{
			throw new InvalidOperationException("PopToRootAsync is not supported globally on Windows, please use a NavigationPage.");
		}

		void INavigation.RemovePage(Page page)
		{
			throw new InvalidOperationException("RemovePage is not supported globally on Windows, please use a NavigationPage.");
		}

		void INavigation.InsertPageBefore(Page page, Page before)
		{
			throw new InvalidOperationException("InsertPageBefore is not supported globally on Windows, please use a NavigationPage.");
		}

		Task INavigation.PushModalAsync(Page page)
		{
			return ((INavigation)this).PushModalAsync(page, true);
		}

		Task<Page> INavigation.PopModalAsync()
		{
			return ((INavigation)this).PopModalAsync(true);
		}

		Task INavigation.PushModalAsync(Page page, bool animated)
		{
			if (page == null)
				throw new ArgumentNullException(nameof(page));

			var tcs = new TaskCompletionSource<bool>();

#pragma warning disable CS0618 // Type or member is obsolete
			// The Platform property is no longer necessary, but we have to set it because some third-party
			// library might still be retrieving it and using it
			page.Platform = this;
#pragma warning restore CS0618 // Type or member is obsolete

			_page.PushModal(page, animated);
			tcs.SetResult(true);
			return tcs.Task;
		}

		Task<Page> INavigation.PopModalAsync(bool animated)
		{
			var tcs = new TaskCompletionSource<Page>();
			var page = _page.PopModal(animated) as Page;
			tcs.SetResult(page);
			return tcs.Task;
		}

		void IDisposable.Dispose()
		{
			if (_disposed)
				return;

			_disposed = true;
			MessagingCenter.Unsubscribe<Page, ActionSheetArguments>(this, Page.ActionSheetSignalName);
			MessagingCenter.Unsubscribe<Page, AlertArguments>(this, Page.AlertSignalName);
			MessagingCenter.Unsubscribe<Page, SnackbarArguments>(this, Page.SnackbarSignalName);
			MessagingCenter.Unsubscribe<Page, bool>(this, Page.BusySetSignalName);
		}

		#region Obsolete 

		SizeRequest IPlatform.GetNativeSize(VisualElement view, double widthConstraint, double heightConstraint)
		{
			return GetNativeSize(view, widthConstraint, heightConstraint);
		}

		#endregion
	}
}
