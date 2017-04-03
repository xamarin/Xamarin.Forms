using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Xamarin.Forms.Platform.WPF.CustomControls
{
    public class TapEventArgs : EventArgs
    {
        public bool Handled { get; set; }
    }

    public class TapExtensions
    {
        public TapExtensions()
        {
            dt.IsEnabled = false;
            dt.Stop();
            dt.Interval = TimeSpan.FromMilliseconds(200);
            dt.Tick += Dt_Tick;
        }

        private static readonly List<TapItem> taps = new List<TapItem>();
        public void Subscribe(FrameworkElement frameworkElement, Action<object, TapEventArgs> tapAction, Action<object, TapEventArgs> doubleTapAction)
        {
            if (frameworkElement == null)
                return;
            frameworkElement.MouseDown += FrameworkElement_TouchDown;
            frameworkElement.MouseUp += FrameworkElement_TouchUp;
            frameworkElement.MouseEnter += FrameworkElement_TouchEnter;
            frameworkElement.MouseUp += FrameworkElement_TouchLeave;
            taps.Add(new TapItem()
            {
                FrameworkElement = frameworkElement,
                TapAction = tapAction,
                DoubleTapAction = doubleTapAction
            });
        }

        public void Unsubscribe(FrameworkElement frameworkElement, Action<object, TapEventArgs> tapAction, Action<object, TapEventArgs> doubleTapAction)
        {
            var myTaps=taps.Where(
                i =>
                    i.FrameworkElement == frameworkElement && i.TapAction == tapAction &&
                    i.DoubleTapAction == doubleTapAction).ToList();
            foreach (TapItem tapItem in myTaps)
            {
                taps.Remove(tapItem);
            }
        }

        private bool isTouchDown = false;
        private DispatcherTimer dt = new DispatcherTimer();
        private void FrameworkElement_TouchLeave(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            isTouchDown = false;
        }

        private void FrameworkElement_TouchEnter(object sender, MouseEventArgs mouseEventArgs)
        {
            isTouchDown = false;
        }

        private void FrameworkElement_TouchUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (isTouchDown)
            {
				var eventArgs=new TapEventArgs();
                foreach (var tapItem in taps.Where(i => i.DoubleTapAction == null))
                {
                    tapItem.TapAction(tapItem.FrameworkElement, eventArgs);
                    if (eventArgs.Handled)
                    {
                        mouseButtonEventArgs.Handled = true;
                        break;
                    }
                }

                if (dt.IsEnabled)
                {
                    dt.IsEnabled = false;
                    dt.Stop();
					var dtEventArgs = new TapEventArgs();
					foreach (var tapItem in taps.Where(i => i.DoubleTapAction != null))
                    {
                        tapItem.DoubleTapAction(tapItem.FrameworkElement, dtEventArgs);
						if (dtEventArgs.Handled)
						{
							mouseButtonEventArgs.Handled = true;
							break;
						}
					}
                }
                else
                {
                    dt.Start();
                }
            }
            isTouchDown = false;
        }

        private void Dt_Tick(object sender, EventArgs e)
        {
            dt.IsEnabled = false;
            dt.Stop();

			var tapEventArgs = new TapEventArgs();
			foreach (var tapItem in taps.Where(i => i.DoubleTapAction != null))
            {
                tapItem.TapAction(tapItem.FrameworkElement, tapEventArgs);
				if (tapEventArgs.Handled)
				{
					break;
				}
			}
        }

        private void FrameworkElement_TouchDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            isTouchDown = true;
        }
    }
}