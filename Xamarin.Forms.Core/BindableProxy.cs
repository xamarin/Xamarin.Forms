using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace Xamarin.Forms
{
	class BindableProxy : BindableObject, IDisposable
	{
		readonly object _targetObject;
		readonly string _targetProperty;
		readonly string _targetEventName;
		readonly PropertyInfo _propInfo;
		List<MethodInfo> _setMethodsInfo;
		List<MethodInfo> _getMethodsInfo;
		List<Type> _possibleParameterTypes;
		INativeValueConverter nativeValueConverter;
		Action<object, object> _callbackSetValue;
		Func<object> _callbackGetValue;
		bool _isSettingFromProxy;

		NativeViewEventListener _eventListener;
		bool _disposed;


		public BindableProxy(object target, PropertyInfo targetPropInfo, string eventName = null)
		{
			if (target == null)
				throw new ArgumentNullException(nameof(target));
			if (targetPropInfo == null)
				throw new ArgumentException("targetProperty should not be null or empty", nameof(targetPropInfo));
			_targetObject = target;
			_targetProperty = targetPropInfo.Name;
			_targetEventName = eventName;

			_propInfo = targetPropInfo;

			Init();
		}

		public BindableProxy(object target, string targetProp, Action<object, object> setValue = null, Func<object> getValue = null, string eventName = null)
		{
			if (target == null)
				throw new ArgumentNullException(nameof(target));
			if (string.IsNullOrEmpty(targetProp))
				throw new ArgumentException("targetProperty should not be null or empty", nameof(targetProp));
			_targetProperty = targetProp;
			_targetObject = target;
			_callbackSetValue = setValue;
			_callbackGetValue = getValue;
			_targetEventName = eventName;

			_propInfo = TargetObjectType.GetProperty(_targetProperty);

			Init();
		}

		public BindableProperty Property;
		public Type TargetPropertyType => _propInfo?.PropertyType;
		public Type TargetObjectType => _targetObject?.GetType();
		public string TargetPropertyName => _targetProperty;

		public void OnTargetPropertyChanged(object valueFromNative = null)
		{
			if (_isSettingFromProxy)
				return;
			object convertedValue = null;
			//this comes converted, or not.. 
			var currentValue = GetValue(Property);

			var nativeValue = GetTargetValue();

			if (valueFromNative == null)
				valueFromNative = nativeValue;

			if (nativeValueConverter != null)
				convertedValue = nativeValueConverter.ConvertBack(valueFromNative, TargetPropertyType, null, CultureInfo.CurrentUICulture);

			var finalValue = convertedValue ?? valueFromNative;
			if (finalValue.Equals(currentValue))
				return;

			SetValueCore(Property, finalValue);
		}

		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					UnSubscribeTwoWay();

					RemoveBinding(Property);
					BindingContext = null;
				}
			}
			_disposed = true;
		}

		void Init()
		{
			if (_propInfo == null)
				FindPossibleMethods(TargetPropertyName, TargetObjectType, out _getMethodsInfo, out _setMethodsInfo, out _possibleParameterTypes);

			Property = BindableProperty.Create(TargetPropertyName, typeof(object), typeof(BindableProxy), propertyChanged: (bo, o, n) => ((BindableProxy)bo).OnPropertyChanged(o, n));

			FindNativeValueConverter();
			SubscribeTwoWay();
		}

		static void FindPossibleMethods(string targetProp, Type targetObjectType, out List<MethodInfo> gets, out List<MethodInfo> sets, out List<Type> parameterTypes)
		{
			gets = new List<MethodInfo>();
			sets = new List<MethodInfo>();
			parameterTypes = new List<Type>();
			var setMethodName = $"Set{targetProp}";
			var getMethodName = $"Get{targetProp}";

			foreach (var method in targetObjectType.GetRuntimeMethods())
			{
				if (method.DeclaringType != targetObjectType)
					continue;

				if (method.Name == setMethodName)
				{
					sets.Add(method);
					foreach (var parameter in method.GetParameters())
					{
						parameterTypes.Add(parameter.ParameterType);
					}
				}

				if (method.Name == getMethodName)
				{
					gets.Add(method);
					parameterTypes.Add(method.ReturnType);
				}
			}
		}

		void SubscribeTwoWay()
		{
			if (!string.IsNullOrEmpty(_targetEventName))
			{
				_eventListener = new NativeViewEventListener(_targetObject, _targetEventName, _targetProperty);
				_eventListener.NativeViewEventFired += NativeViewEventFired;
			}

			var inpc = _targetObject as INotifyPropertyChanged;
			if (inpc != null)
				inpc.PropertyChanged += NativeViewPropertyChanged;
		}

		void UnSubscribeTwoWay()
		{
			if (_eventListener != null)
			{
				_eventListener.NativeViewEventFired -= NativeViewEventFired;
				_eventListener.Dispose();
				_eventListener = null;
			}

			var inpc = _targetObject as INotifyPropertyChanged;
			if (inpc != null)
			{
				inpc.PropertyChanged -= NativeViewPropertyChanged;
			}
		}

		void NativeViewEventFired(object sender, NativeViewEventFiredEventArgs e)
		{
			if (_isSettingFromProxy)
				return;
			OnTargetPropertyChanged();
		}

		void NativeViewPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName.Equals(TargetPropertyName))
				OnTargetPropertyChanged();
		}

		void FindNativeValueConverter()
		{
			if (TargetPropertyType != null)
				nativeValueConverter = Registrar.Registered.GetHandler<INativeValueConverter>(TargetPropertyType);

			if (nativeValueConverter == null && _possibleParameterTypes != null)
			{
				foreach (var item in _possibleParameterTypes)
				{
					nativeValueConverter = Registrar.Registered.GetHandler<INativeValueConverter>(item);
				}
			}

			if (nativeValueConverter == null)
				Log.Warning("NativeBinding", $"Converter not found for {TargetPropertyType}");
		}

		void OnPropertyChanged(object oldValue, object newValue)
		{
			if (_callbackSetValue != null)
				_callbackSetValue(oldValue, newValue);
			else
				SetTargetValue(newValue);
		}

		void SetTargetValue(object value)
		{
			if (value == null)
				return;

			bool wasSet = TrySetValueOnTarget(value);

			if (!wasSet)
				throw new InvalidCastException($"Can't bind properties of different types. The target property is {TargetPropertyType}, and the value is {value.GetType()}");
		}

		bool TrySetValueOnTarget(object value)
		{
			bool wasSet = false;
			object convertedValue = null;

			if (nativeValueConverter != null)
				convertedValue = nativeValueConverter.Convert(value, TargetPropertyType, null, CultureInfo.CurrentUICulture);

			if (_propInfo != null)
				wasSet = SetPropertyInfo(convertedValue ?? value);

			if (_setMethodsInfo != null && !wasSet)
				wasSet = SetSetMethodInfo(convertedValue ?? value);

			return wasSet;
		}

		object GetTargetValue()
		{
			if (_callbackGetValue != null)
				return _callbackGetValue();

			if (_propInfo != null)
				return ReadPropertyInfo();

			if (_getMethodsInfo != null)
				return ReadGetMethodInfo();

			return null;
		}

		bool SetSetMethodInfo(object value)
		{
			bool wasSet = false;

			foreach (var setMethod in _setMethodsInfo)
			{
				try
				{
					setMethod.Invoke(_targetObject, new object[] { value });
					wasSet = true;
					break;
				}
				catch (ArgumentException)
				{
					System.Diagnostics.Debug.WriteLine("Failed to convert");
				}
			}

			return wasSet;
		}

		object ReadGetMethodInfo()
		{
			foreach (var getMethod in _getMethodsInfo)
			{
				try
				{
					var possibleValue = getMethod.Invoke(_targetObject, new object[] { });
					if (possibleValue != null)
						return possibleValue;
					break;
				}
				catch (Exception ex)
				{
					throw (ex);
				}
			}
			return null;
		}

		object ReadPropertyInfo()
		{
			if (!_propInfo.CanRead)
			{
				System.Diagnostics.Debug.WriteLine($"No GetMethod found for {TargetPropertyName}");
				return null;
			}

			var obj = _propInfo.GetValue(_targetObject);
			return obj;
		}

		bool SetPropertyInfo(object value)
		{
			if (!TargetPropertyType.IsAssignableFrom(value.GetType()))
				return false;

			if (!_propInfo.CanWrite)
			{
				System.Diagnostics.Debug.WriteLine($"No SetMethod found for {TargetPropertyName}");
				return false;
			}

			try
			{
				_isSettingFromProxy = true;
				_propInfo.SetValue(_targetObject, value);
				_isSettingFromProxy = false;
			}
			catch (Exception ex)
			{
				System.Diagnostics.Debug.WriteLine($"{ex}");
				return false;
			}

			return true;
		}
	}
}

