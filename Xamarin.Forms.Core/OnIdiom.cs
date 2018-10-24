using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms
{
	[ContentProperty("Idioms")]
	public class OnIdiom<T>
	{
		readonly Dictionary<TargetIdiom, T> _legacyValues = new Dictionary<TargetIdiom, T>();

		bool _hasDefault;		

		T _default;
		

		public OnIdiom()
		{
			Idioms = new List<OnIdiomSetting>();
		}

		public T Phone
		{
			get => GetLegacyValue(TargetIdiom.Phone);
			set => SetLegacyValue(TargetIdiom.Phone, value);
		}

		public T Tablet
		{
			get => GetLegacyValue(TargetIdiom.Tablet);
			set => SetLegacyValue(TargetIdiom.Tablet, value);
		}

		public T Desktop
		{
			get => GetLegacyValue(TargetIdiom.Desktop);
			set => SetLegacyValue(TargetIdiom.Desktop, value);
		}

		public T TV
		{
			get => GetLegacyValue(TargetIdiom.TV);
			set => SetLegacyValue(TargetIdiom.TV, value);
		}

		public T Watch
		{
			get => GetLegacyValue(TargetIdiom.Watch);
			set => SetLegacyValue(TargetIdiom.Watch, value);
		}

		public T Default
		{
			get { return _default; }
			set
			{
				_hasDefault = true;
				_default = value;
			}
		}

		public IList<OnIdiomSetting> Idioms { get; private set; }

#pragma warning disable RECS0108 // Warns about static fields in generic types
		static readonly IValueConverterProvider s_valueConverter = DependencyService.Get<IValueConverterProvider>();
#pragma warning restore RECS0108 // Warns about static fields in generic types

		public static implicit operator T(OnIdiom<T> onIdiom)
		{
			var currentIdiom = Device.Idiom.ToString("G");
			foreach (var idiomSetting in onIdiom.Idioms)
			{				
				if (!ContainsIdiomMatch(idiomSetting.Idiom, currentIdiom))
					continue;
				if (s_valueConverter == null)
					continue;
				return (T)s_valueConverter.Convert(idiomSetting.Value, typeof(T), null, null);
			}

			//has any legacy value been set?
			if (onIdiom._legacyValues.Count == 0)
				return onIdiom._hasDefault ? onIdiom._default : default(T);

			//legacy fallback
#pragma warning disable 0618, 0612
			if (!onIdiom._legacyValues.TryGetValue(Device.Idiom, out var legacyValue))
			{
				legacyValue = onIdiom._hasDefault ? onIdiom._default : default(T);
			}
			return legacyValue;
#pragma warning restore 0618, 0612
		}

		static bool ContainsIdiomMatch(IList<string> idiomList, string currentIdiom)
		{
			if (idiomList == null) return false;
			for (int i = 0; i < idiomList.Count; i++)
			{
				var current = idiomList[i];
				if (current.Equals(currentIdiom, StringComparison.OrdinalIgnoreCase))
				{
					return true;
				}
			}
			return false;
		}

		T GetLegacyValue( TargetIdiom idiom )
		{
			return _legacyValues.TryGetValue(idiom, out var value) 
				? value : default(T);
		}

		void SetLegacyValue(TargetIdiom idiom, T value)
		{
			_legacyValues[idiom] = value;
		}
	}

	[ContentProperty("Value")]
	public class OnIdiomSetting
	{
		[TypeConverter(typeof(ListStringTypeConverter))]
		public IList<string> Idiom { get; set; }
		public object Value { get; set; }
	}
}
