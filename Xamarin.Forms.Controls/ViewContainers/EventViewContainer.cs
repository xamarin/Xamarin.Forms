using System;

namespace Xamarin.Forms.Controls
{
	internal class EventViewContainer<T> : ViewContainer<T>
		where T : View
	{
		Label _eventLabel;
		string _formsMember;
		int _numberOfTimesFired;

		public EventViewContainer(Enum formsMember, T view) : base(formsMember, view)
		{
			_numberOfTimesFired = 0;

			_formsMember = formsMember.ToString();

			_eventLabel = new Label
			{
				AutomationId = formsMember + "EventLabel",
				Text = "Event: " + _formsMember + " (none)"
			};

			ContainerLayout.Children.Add(_eventLabel);
		}

		public void EventFired()
		{
			_numberOfTimesFired++;
			_eventLabel.Text = "Event: " + _formsMember + " (fired " + _numberOfTimesFired + ")";
		}
	}
}