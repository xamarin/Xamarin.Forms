using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Xamarin.Forms
{
	[ContentProperty("StateTriggers")]
	public class CompositeStateTrigger : StateTriggerBase
	{
		public CompositeStateTrigger()
		{
			StateTriggers = new ObservableCollection<StateTriggerBase>();

			UpdateState();
		}

		public ObservableCollection<StateTriggerBase> StateTriggers
		{
			get { return (ObservableCollection<StateTriggerBase>)GetValue(StateTriggersProperty); }
			set { SetValue(StateTriggersProperty, value); }
		}

		public static readonly BindableProperty StateTriggersProperty =
			BindableProperty.Create(nameof(StateTriggers), typeof(ObservableCollection<StateTriggerBase>), typeof(CompositeStateTrigger), null,
				propertyChanged: OnStateTriggersPropertyChanged);
		
		public CompositeOperator Operator
		{
			get { return (CompositeOperator)GetValue(OperatorProperty); }
			set { SetValue(OperatorProperty, value); }
		}

		public static readonly BindableProperty OperatorProperty =
			BindableProperty.Create(nameof(Operator), typeof(CompositeOperator), typeof(CompositeStateTrigger), CompositeOperator.And,
				propertyChanged: OnOperatorPropertyChanged);
		
		static void OnStateTriggersPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
		{
			CompositeStateTrigger trigger = (CompositeStateTrigger)bindable;

			if (oldvalue is INotifyCollectionChanged)
				(oldvalue as INotifyCollectionChanged).CollectionChanged -= trigger.OnCompositeTriggerCollectionChanged;
	
			if (newvalue is INotifyCollectionChanged)
				(newvalue as INotifyCollectionChanged).CollectionChanged += trigger.OnCompositeTriggerCollectionChanged;
		
			if (newvalue is IEnumerable<StateTriggerBase>)
			{
				foreach (var item in newvalue as IEnumerable<StateTriggerBase>)
				{
					if (!(item is StateTrigger))
						trigger.SetValue(StateTriggersProperty, oldvalue); 
				}

				trigger.OnCompositeTriggerCollectionChanged(newvalue,
					new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, (newvalue as IEnumerable<StateTriggerBase>).ToList()));
			}

			trigger.UpdateState();
		}

		static void OnOperatorPropertyChanged(BindableObject bindable, object oldvalue, object newvalue)
		{
			CompositeStateTrigger trigger = (CompositeStateTrigger)bindable;
			trigger.UpdateState();
		}

		protected override void OnBindingContextChanged()
		{
			base.OnBindingContextChanged();

			object bc = BindingContext;

			foreach (var stateTrigger in StateTriggers)
				SetInheritedBindingContext(stateTrigger, bc);
		}

		void OnCompositeTriggerCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.NewItems != null)
			{
				foreach (var item in e.NewItems)
				{
					if (item is StateTriggerBase stateTrigger)
						stateTrigger.IsActiveChanged += OnStateTriggerIsActiveChanged;
				}
			}
			if (e.OldItems != null)
			{
				foreach (var item in e.OldItems)
				{
					if (item is StateTriggerBase stateTrigger)
						stateTrigger.IsActiveChanged -= OnStateTriggerIsActiveChanged;
				}
			}

			UpdateState();
		}

		void OnStateTriggerIsActiveChanged(object sender, System.EventArgs e)
		{
			UpdateState();
		}

		void UpdateState()
		{
			if (!StateTriggers.Any())
			{
				SetActive(false);
			}
			else if (Operator == CompositeOperator.Or)
			{
				bool isActive = GetIsActiveFromStateTriggers().Where(t => t).Any();
				SetActive(isActive);
			}
			else if (Operator == CompositeOperator.And)
			{
				bool isActive = !GetIsActiveFromStateTriggers().Where(t => !t).Any();
				SetActive(isActive);
			}
		}

		IEnumerable<bool> GetIsActiveFromStateTriggers()
		{
			foreach (var trigger in StateTriggers)
			{
				yield return trigger.IsActive;
			}
		}
	}

	public enum CompositeOperator
	{
		And,
		Or
	}
}
