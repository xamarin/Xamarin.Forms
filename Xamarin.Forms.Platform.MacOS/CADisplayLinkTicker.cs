using System;
using System.Collections.Concurrent;
using System.Threading;
using Foundation;
using Xamarin.Forms.Internals;
using CoreVideo;
using AppKit;
using CoreAnimation;

namespace Xamarin.Forms.Platform.MacOS
{
	internal class CADisplayLinkTicker : Ticker
	{
		readonly BlockingCollection<Action> _queue = new BlockingCollection<Action>();
		CVDisplayLink _link;

		public CADisplayLinkTicker()
		{
			var thread = new Thread(StartThread);
			thread.Start();
		}

		internal new static CADisplayLinkTicker Default
		{
			get { return Ticker.Default as CADisplayLinkTicker; }
		}

		public void Invoke(Action action)
		{
			_queue.Add(action);
		}

		protected override void DisableTimer()
		{
			if (_link != null)
			{
				_link.Stop();
				//_link.RemoveFromRunLoop(NSRunLoop.Current, NSRunLoop.NSRunLoopCommonModes);
				_link.Dispose();
			}
			_link = null;
		}

		protected override void EnableTimer()
		{
			//	_link = Xamarin.Core CADisplayLink.Create(() => SendSignals());
			_link.Start();
			//_link.AddToRunLoop(NSRunLoop.Current, NSRunLoop.NSRunLoopCommonModes);
		}

		void StartThread()
		{
			while (true)
			{
				var action = _queue.Take();
				var previous = NSApplication.CheckForIllegalCrossThreadCalls;
				NSApplication.CheckForIllegalCrossThreadCalls = false;

				CATransaction.Begin();
				action.Invoke();

				while (_queue.TryTake(out action))
					action.Invoke();
				CATransaction.Commit();

				NSApplication.CheckForIllegalCrossThreadCalls = previous;
			}
		}
	}
}