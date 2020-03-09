using System;
using System.Collections.Generic;
using System.Reflection;
using Android.Content;
using Android.Views;
using Android.Util;
using Android.App;
using ABuildVersionCodes = Android.OS.BuildVersionCodes;
using ABuild = Android.OS.Build;
using AView = Android.Views.View;
using ARelativeLayout = Android.Widget.RelativeLayout;
using FLabelRenderer = Xamarin.Forms.Platform.Android.FastRenderers.LabelRenderer;
#if __ANDROID_29__
using AToolbar = AndroidX.AppCompat.Widget.Toolbar;
#else
using AToolbar = Android.Support.V7.Widget.Toolbar;
# endif
using System.Threading;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using Xamarin.Forms.Internals;
using System.Diagnostics;
using Android.Content.Res;
using Android.Widget;
using Orientation = Android.Content.Res.Orientation;

namespace Xamarin.Forms.Platform.Android
{
	interface IPreBuildable
	{
		object Build();
	}

	interface IPreComputable
	{
		object Compute();
	}

	public static class AndroidAnticipator
	{
		static class Key
		{
			internal struct SdkVersion : IPreComputable
			{
				object IPreComputable.Compute()
					=> ABuild.VERSION.SdkInt;

				public override string ToString()
					=> $"{nameof(SdkVersion)}";
			}

			internal struct IdedResourceExists : IPreComputable
			{
				readonly internal Context Context;
				readonly internal int Id;

				internal IdedResourceExists(Context context, int id)
				{
					Context = context;
					Id = id;
				}

				object IPreComputable.Compute()
				{
					if (Id == 0)
						return false;

					using (var value = new TypedValue())
						return Context.Theme.ResolveAttribute(Id, value, true);
				}

				public override string ToString()
					=> $"{nameof(IdedResourceExists)}, '{ResourceName(Id)}'";
			}

			internal struct NamedResourceExists : IPreComputable
			{
				readonly internal Context Context;
				readonly internal string Name;
				readonly internal string Type;

				internal NamedResourceExists(Context context, string name, string type)
				{
					Context = context;
					Name = name;
					Type = type;
				}

				object IPreComputable.Compute()
				{
					var id = Context.Resources.GetIdentifier(Name, Type, Context.PackageName);
					if (id == 0)
						return false;

					using (var value = new TypedValue())
						return Context.Theme.ResolveAttribute(id, value, true);
				}

				public override string ToString()
					=> $"{nameof(NamedResourceExists)}, {Name}";
			}

			internal struct InflateResource : IPreBuildable
			{
				readonly internal Context Context;
				readonly internal int Id;

				internal InflateResource(Context context, int id)
				{
					if (id == 0)
						throw new ArgumentException("id cannot be zero");

					Context = context;
					Id = id;
				}

				object IPreBuildable.Build()
				{
					var layoutInflator = (Context as Activity)?.LayoutInflater ?? 
						LayoutInflater.FromContext(Context);

					return layoutInflator.Inflate(Id, null);
				}

				public override string ToString()
					=> $"{nameof(InflateResource)}, {ResourceName(Id)}";
			}

			internal struct ActivateView :
				IPreBuildable, IEquatable<ActivateView>
			{
				readonly internal Context Context;
				readonly internal Type Type;
				readonly internal Func<Context, object> Factory;

				internal ActivateView(Context context, Type type, Func<Context, object> activator = null)
				{
					Context = context;
					Type = type;
					Factory = activator;
				}

				object IPreBuildable.Build()
				{
					if (Factory == null)
						return Activator.CreateInstance(Type, Context);

					return Factory(Context);
				}

				public override int GetHashCode()
					=> Context.GetHashCode() ^ Type.GetHashCode();
				public bool Equals(ActivateView other)
					=> other.Context == Context && other.Type == Type;
				public override bool Equals(object other)
					=> other is ActivateView ? Equals((ActivateView)other) : false;
				public override string ToString()
					=> $"{nameof(ActivateView)}, {Type.GetTypeInfo().Name}";
			}
		}

		public static void Initialize(ContextWrapper context)
		{
			Profile.FrameBegin();

			if (context == null)
				throw new ArgumentNullException(nameof(context));

			s_scheduler.Value.Schedule(() =>
			{
				s_anticipator.AnticipateAllocation(new Key.ActivateView(context, typeof(AToolbar), o => new AToolbar(o)));

				s_scheduler.Value.Schedule(() =>
				{
					s_anticipator.AnticipateAllocation(new Key.ActivateView(context.BaseContext, typeof(ARelativeLayout), o => new ARelativeLayout(o)));

					s_scheduler.Value.Schedule(() =>
					{
						s_anticipator.AnticipateValue(new Key.SdkVersion());

						s_anticipator.AnticipateValue(new Key.IdedResourceExists(context, global::Android.Resource.Attribute.ColorAccent));
						s_anticipator.AnticipateValue(new Key.NamedResourceExists(context, "colorAccent", "attr"));

						if (FormsAppCompatActivity.ToolbarResource != 0)
							s_anticipator.AnticipateAllocation(new Key.InflateResource(context, FormsAppCompatActivity.ToolbarResource));
						if (Resource.Layout.FlyoutContent != 0)
							s_anticipator.AnticipateAllocation(new Key.InflateResource(context, Resource.Layout.FlyoutContent));

						s_scheduler.Value.Schedule(() =>
						{
							new PageRenderer(context);
							new FLabelRenderer(context);
							new ListViewRenderer(context);
						});
					});
				});
			});

			Profile.FrameEnd();
		}

		public static void ReportUnused()
		{
			s_anticipator.ReportUnused();
		}

		internal static ABuildVersionCodes SdkVersion
			=> (ABuildVersionCodes)s_anticipator.Compute(new Key.SdkVersion());

		internal static bool IdedResourceExists(Context context, int id)
			=> (bool)s_anticipator.Compute(new Key.IdedResourceExists(context, id));

		internal static bool NamedResourceExists(Context context, string name, string type)
			=> (bool)s_anticipator.Compute(new Key.NamedResourceExists(context, name, type));

		internal static AView InflateResource(Context context, int id)
			=> (AView)s_anticipator.Allocate(new Key.InflateResource(context, id));

		internal static AView ActivateView(Context context, Type type)
			=> (AView)s_anticipator.Allocate(new Key.ActivateView(context, type));

		static string ResourceName(int id)
			=> id != 0 && s_resourceNames.Value.TryGetValue(id, out var name) ? name : id.ToString();

		static Lazy<Scheduler> s_scheduler;
		static Lazy<Dictionary<int, string>> s_resourceNames;
		static Anticipator s_anticipator;

		static AndroidAnticipator()
		{
			Profile.FrameBegin("AndroidAnticipator");

			Profile.FramePartition("Scheduler");
			s_scheduler = new Lazy<Scheduler>(() => new Scheduler());

			Profile.FramePartition("Anticipator");
			s_anticipator = new Anticipator(s_scheduler);

			Profile.FramePartition("Resource Names");
			s_resourceNames = new Lazy<Dictionary<int, string>>(() => new Dictionary<int, string>
			{
				[FormsAppCompatActivity.ToolbarResource] = nameof(FormsAppCompatActivity.ToolbarResource),
				[global::Android.Resource.Attribute.ColorAccent] = nameof(global::Android.Resource.Attribute.ColorAccent),
				[Resource.Layout.FlyoutContent] = nameof(Resource.Layout.FlyoutContent),
			});

			Profile.FrameEnd("AndroidAnticipator");
		}
	}

	sealed class Anticipator
	{
		sealed class Warehouse
		{
			static ConcurrentBag<object> ActivateBag(object key)
				=> new ConcurrentBag<object>();

			readonly ConcurrentDictionary<object, object> _dictionary;
			readonly Func<object, ConcurrentBag<object>> _activateBag;
			readonly ConcurrentDictionary<object, int> _timeToActivate;
			readonly Lazy<Scheduler> _scheduler;

			internal Warehouse(Lazy<Scheduler> scheduler)
			{
				_activateBag = ActivateBag;
				_dictionary = new ConcurrentDictionary<object, object>();
				_timeToActivate = new ConcurrentDictionary<object, int>();
				_scheduler = scheduler;
			}

			ConcurrentBag<object> Get(object key)
				=> (ConcurrentBag<object>)_dictionary.GetOrAdd(key, _activateBag);

			void Set(object key, object value)
				=> Get(key).Add(value);

			bool TryGet(object key, out object value)
			{
				var result = Get(key).TryTake(out value);

				if (result)
				{
					_timeToActivate.TryRemove(value, out var ms);
					s_savings += ms;
					Profile.WriteLog("WAREHOUSE HIT: {0}, ms={1}", key, ms);
				}
				else
				{
					Profile.WriteLog("WAREHOUSE MISS: {0}", key);
				}

				return result;
			}

			public object Get<T>(T key = default)
				where T : IPreBuildable
			{
				if (!TryGet(key, out var value))
					return key.Build();

				return value;
			}

			public void Anticipate<T>(T key = default)
				where T : IPreBuildable
			{
				_scheduler.Value.Schedule(() =>
				{
					try
					{
						var stopwatch = new Stopwatch();
						stopwatch.Start();
						var value = key.Build();
						if (value == null)
						{
							Profile.WriteLog("WEARHOUSED NULL BUILD: {0}", key);
							return;
						}
						var ticks = stopwatch.ElapsedTicks;
						var ms = TimeSpan.FromTicks(ticks).Milliseconds;

						_timeToActivate.TryAdd(value, ms);
						Set(key, value);
						Profile.WriteLog("Wearhoused: {0}, ms={1}", key, ms);
					}
					catch (Exception ex)
					{
						Profile.WriteLog("WEARHOUSE EXCEPTION: {0}: {1}", key, ex);
					}
				});
			}

			public void ReportUnused()
			{
				foreach (var pair in _dictionary)
				{
					foreach (var value in ((ConcurrentBag<object>)pair.Value))
					{
						_timeToActivate.TryRemove(value, out var ms);
						Profile.WriteLog("WEARHOUSE UNUSED: {0}, ms={1}", pair.Key, ms);
						(value as IDisposable)?.Dispose();
					}
				}
			}
		}

		sealed class Cache
		{
			readonly ConcurrentDictionary<object, object> _dictionary;
			readonly ConcurrentDictionary<object, int> _timeToCompute;
			readonly Lazy<Scheduler> _scheduler;

			internal Cache(Lazy<Scheduler> scheduler)
			{
				_scheduler = scheduler;
				_timeToCompute = new ConcurrentDictionary<object, int>();
				_dictionary = new ConcurrentDictionary<object, object>();
			}

			void Set(object key, object value)
				=> _dictionary.TryAdd(key, value);

			bool TryGet(object key, out object value)
			{
				var result = _dictionary.TryGetValue(key, out value);
				if (!result)
					return false;

				if (!_timeToCompute.TryRemove(key, out var ms))
					return true;

				s_savings += ms;
				Profile.WriteLog("CACHE HIT: {0}, ms={1}", key, ms);
				return true;
			}

			public object Get<T>(T key = default)
				where T : IPreComputable
			{
				if (!TryGet(key, out var value))
					return key.Compute();

				return value;
			}

			public void Anticipate<T>(T key = default)
				where T : IPreComputable
			{
				_scheduler.Value.Schedule(() =>
				{
					try
					{
						var stopwatch = new Stopwatch();
						stopwatch.Start();
						var value = key.Compute();
						var ticks = stopwatch.ElapsedTicks;
						var ms = TimeSpan.FromTicks(ticks).Milliseconds;

						_timeToCompute.TryAdd(key, ms);
						Set(key, value);
						Profile.WriteLog("Cashed: {0}, ms={1}", key, ms);
					}
					catch (Exception ex)
					{
						Profile.WriteLog("CACHE EXCEPTION: {0}: {1}", key, ex);
					}
				});
			}

			public void ReportUnused()
			{
				foreach (var pair in _dictionary)
				{
					if (_timeToCompute.ContainsKey(pair.Key))
						Profile.WriteLog("CACHE UNUSED: {0}", pair.Key);
				}
			}
		}

		static int s_savings = 0;

		readonly Lazy<Scheduler> _scheduler;
		readonly Lazy<Cache> _cache;
		readonly Lazy<Warehouse> _heap;

		internal Anticipator(Lazy<Scheduler> scheduler)
		{
			Profile.FrameBegin("Anticipator .ctor");

			_scheduler = scheduler;

			Profile.FramePartition("Warehouse");
			_heap = new Lazy<Warehouse>(() => new Warehouse(_scheduler));

			Profile.FramePartition("Cache");
			_cache = new Lazy<Cache>(() => new Cache(_scheduler));

			Profile.FrameEnd("Anticipator .ctor");
		}

		internal void AnticipateAllocation<T>(T key = default)
			where T : IPreBuildable
			=> _heap.Value.Anticipate(key);

		internal object Allocate<T>(T key = default)
			where T : IPreBuildable
			=> _heap.Value.Get(key);

		internal void AnticipateValue<T>(T key = default)
			where T : IPreComputable
			=> _cache.Value.Anticipate(key);

		internal object Compute<T>(T key = default)
			where T : IPreComputable
			=> _cache.Value.Get(key);

		public void ReportUnused()
		{
			_cache.Value.ReportUnused();
			_heap.Value.ReportUnused();

			Profile.WriteLog("ANTICIPATOR: SAVINGS {0}ms", s_savings);
		}
	}

	sealed class Scheduler
	{
		private static readonly TimeSpan LoopTimeOut = TimeSpan.FromSeconds(5.0);
		private readonly Thread _thread;
		private readonly AutoResetEvent _work;
		private readonly Lazy<ConcurrentQueue<Action>> _actions;

		internal Scheduler()
		{
			Profile.FrameBegin("Scheduler .ctor");
			_actions = new Lazy<ConcurrentQueue<Action>>(() =>
				new ConcurrentQueue<Action>());

			_work = new AutoResetEvent(false);

			Profile.FramePartition("ParameterizedThreadStart");
			_thread = new Thread(new ParameterizedThreadStart(Loop));
			_thread.Start(_work);
			Profile.FrameEnd("Scheduler .ctor");
		}

		private void Loop(object argument)
		{
			var autoResetEvent = (AutoResetEvent)argument;

			while (autoResetEvent.WaitOne(LoopTimeOut))
			{
				while (_actions.Value.Count > 0)
				{
					Action action;
					if (_actions.Value.TryDequeue(out action))
					{
						if (action == null)
							return;

						action();
					}
				}
			}
		}


		internal void Schedule(Action action)
		{
			if (action == null)
				throw new ArgumentNullException(nameof(action));

			_actions.Value.Enqueue(action);
			_work.Set();
		}
	}
}
 