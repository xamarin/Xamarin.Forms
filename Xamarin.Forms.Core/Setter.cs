using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Xamarin.Forms
{
	[ContentProperty("Value")]
	[ProvideCompiled("Xamarin.Forms.Core.XamlC.SetterValueProvider")]
	public sealed class Setter : IValueProvider
	{
		readonly ConditionalWeakTable<BindableObject, object> _originalValues = new ConditionalWeakTable<BindableObject, object>();

		/// <summary>
		///		Set the target for the property to be changed.  Can be a <c>x:Reference</c> to a <see cref="BindableObject"/>
		///		or the Name of an object in the attached scope.
		/// </summary>
		/// <example>
		/// The following example shows the two ways of targeting a named object.
		/// <code>
		///		<Setter Target="{x:Reference TargetLabel1}" Property="Label.TextColor" Value="#94b0b7" />
		///		<Setter Target="TargetLabel1" Property="Label.TextColor" Value="#94b0b7" />
		/// </code>
		/// </example>
		public object Target { get; set; }

		public BindableProperty Property { get; set; }

		public object Value { get; set; }

		object IValueProvider.ProvideValue(IServiceProvider serviceProvider)
		{
			if (Property == null)
				throw new XamlParseException("Property not set", serviceProvider);
			var valueconverter = serviceProvider.GetService(typeof(IValueConverterProvider)) as IValueConverterProvider;

			Func<MemberInfo> minforetriever =
				() =>
				{
					MemberInfo minfo = null;
					try {
						minfo = Property.DeclaringType.GetRuntimeProperty(Property.PropertyName);
					} catch (AmbiguousMatchException e) {
						throw new XamlParseException($"Multiple properties with name '{Property.DeclaringType}.{Property.PropertyName}' found.", serviceProvider, innerException: e);
					}
					if (minfo != null)
						return minfo;
					try {
						return Property.DeclaringType.GetRuntimeMethod("Get" + Property.PropertyName, new[] { typeof(BindableObject) });
					} catch (AmbiguousMatchException e) {
						throw new XamlParseException($"Multiple methods with name '{Property.DeclaringType}.Get{Property.PropertyName}' found.", serviceProvider, innerException: e);
					}
				};

			object value = valueconverter.Convert(Value, Property.ReturnType, minforetriever, serviceProvider);
			Value = value;
			return this;
		}

		internal void Apply(BindableObject scope, bool fromStyle = false)
		{
			var targetObject = FindTargetObject(scope, Target);
			if (targetObject == null)
				throw new ArgumentNullException(nameof(targetObject));
			if (Property == null)
				return;

			object originalValue = targetObject.GetValue(Property);
			if (!Equals(originalValue, Property.DefaultValue))
			{
				_originalValues.Remove(targetObject);
				_originalValues.Add(targetObject, originalValue);
			}

			var dynamicResource = Value as DynamicResource;
			if (Value is BindingBase binding)
				targetObject.SetBinding(Property, binding.Clone(), fromStyle);
			else if (dynamicResource != null)
				targetObject.SetDynamicResource(Property, dynamicResource.Key, fromStyle);
			else
			{
				if (Value is IList<VisualStateGroup> visualStateGroupCollection)
					targetObject.SetValue(Property, visualStateGroupCollection.Clone(), fromStyle);
				else
					targetObject.SetValue(Property, Value, fromStyle);
			}
		}

		internal void UnApply(BindableObject scope, bool fromStyle = false)
		{
			var targetObject = FindTargetObject(scope, Target);
			if (targetObject == null)
				throw new ArgumentNullException(nameof(targetObject));
			if (Property == null)
				return;

			object actual = targetObject.GetValue(Property);
			if (!Equals(actual, Value) && !(Value is Binding) && !(Value is DynamicResource))
			{
				//Do not reset default value if the value has been changed
				_originalValues.Remove(targetObject);
				return;
			}

			if (_originalValues.TryGetValue(targetObject, out object defaultValue))
			{
				//reset default value, unapply bindings and dynamicResource
				targetObject.SetValue(Property, defaultValue, fromStyle);
				_originalValues.Remove(targetObject);
			}
			else
				targetObject.ClearValue(Property);
		}

		BindableObject FindTargetObject(BindableObject scope, object target)
		{
			if (scope == null || target == null)
				return scope;

			if (target is BindableObject bindableObject)
				return bindableObject;

			if (scope is Element element && target is string targetName)
			{
				if (element.FindByName(targetName) is BindableObject targetObject)
					return targetObject;
			}

			return null;
		}
	}
}