namespace Xamarin.Forms
{
	class NativeBinding : Binding
	{
		string _updateSourceEvent;

		public NativeBinding(string path, BindingMode mode = BindingMode.Default, IValueConverter converter = null, object converterParameter = null, string stringFormat = null, object source = null, string updateSourceEvent = null) : base(path, mode, converter, converterParameter, stringFormat, source)
		{
			_updateSourceEvent = updateSourceEvent;
		}

		public string UpdateSourceEvent
		{
			get { return _updateSourceEvent; }
			set
			{
				ThrowIfApplied();
				_updateSourceEvent = value;
			}
		}

		internal override BindingBase Clone()
		{
			return new NativeBinding(Path, Mode) { Converter = Converter, ConverterParameter = ConverterParameter, StringFormat = StringFormat, Source = Source, UpdateSourceEvent = UpdateSourceEvent };
		}

	}
}

