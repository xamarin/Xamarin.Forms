using System;
using System.Collections.ObjectModel;
using NUnit.Framework;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class ObservableWrapperTests : BaseTestFixture
	{
		[Test]
		public void Constructor ()
		{
			var observableCollection = new ObservableCollection<View> ();
			var wrapper = new ObservableWrapper<View, Button> (observableCollection);

			Assert.IsEmpty (wrapper);

			Assert.Throws<ArgumentNullException> (() => new ObservableWrapper<View, View> (null));
		}

		[Test]
		public void TracksExternallyAdded ()
		{
			var observableCollection = new ObservableCollection<View> ();
			var wrapper = new ObservableWrapper<View, Button> (observableCollection);

			var child = new Button ();

			wrapper.Add (child);

			Assert.AreEqual (child, wrapper[0]);
			Assert.AreEqual (child, observableCollection[0]);
		}

		[Test]
		public void TracksExternallyAddedSameType ()
		{
			var observableCollection = new ObservableCollection<View> ();
			var wrapper = new ObservableWrapper<View, View> (observableCollection);

			var child = new Button ();

			wrapper.Add (child);

			Assert.AreEqual (child, wrapper[0]);
			Assert.AreEqual (child, observableCollection[0]);
		}

		[Test]
		public void ReadOnly ()
		{
			var observableCollection = new ObservableCollection<View> ();
			var wrapper = new ObservableWrapper<View, Button> (observableCollection);

			Assert.False (wrapper.IsReadOnly);

			wrapper.Add (new Button ());

			wrapper.IsReadOnly = true;

			Assert.True (wrapper.IsReadOnly);

			Assert.Throws<NotSupportedException> (() => wrapper.Remove (wrapper[0]));
			Assert.Throws<NotSupportedException> (() => wrapper.Add (new Button ()));
			Assert.Throws<NotSupportedException> (() => wrapper.RemoveAt (0));
			Assert.Throws<NotSupportedException> (() => wrapper.Insert (0, new Button ()));
			Assert.Throws<NotSupportedException> (wrapper.Clear);
		}

		[Test]
		public void Indexer ()
		{
			var observableCollection = new ObservableCollection<View> ();
			var wrapper = new ObservableWrapper<View, Button> (observableCollection);

			wrapper.Add (new Button ());

			var newButton = new Button ();

			wrapper[0] = newButton;

			Assert.AreEqual (newButton, wrapper[0]);
		}

		[Test]
		public void IndexerSameType ()
		{
			var observableCollection = new ObservableCollection<View> ();
			var wrapper = new ObservableWrapper<View, View> (observableCollection);

			wrapper.Add (new Button ());

			var newButton = new Button ();

			wrapper[0] = newButton;

			Assert.AreEqual (newButton, wrapper[0]);
		}

		[Test]
		public void INCCSimpleAdd ()
		{
			var oc = new ObservableCollection<View> ();
			var wrapper = new ObservableWrapper<View, View> (oc);

			var child = new Button ();

			Button addedResult = null;
			int addIndex = -1;
			wrapper.CollectionChanged += (sender, args) => {
				addedResult = args.NewItems[0] as Button;
				addIndex = args.NewStartingIndex;
			};

			wrapper.Add (child);

			Assert.AreEqual (0, addIndex);
			Assert.AreEqual (child, addedResult);
		}

		[Test]
		public void INCCComplexAdd ()
		{
			var oc = new ObservableCollection<View> ();
			var wrapper = new ObservableWrapper<View, Button> (oc);

			oc.Add (new Stepper ());

			var child = new Button ();

			Button addedResult = null;
			int addIndex = -1;
			wrapper.CollectionChanged += (sender, args) => {
				addedResult = args.NewItems[0] as Button;
				addIndex = args.NewStartingIndex;
			};

			wrapper.Add (child);

			Assert.AreEqual (0, addIndex);
			Assert.AreEqual (child, addedResult);
		}

		[Test]
		public void INCCSimpleRemove ()
		{
			var oc = new ObservableCollection<View> ();
			var wrapper = new ObservableWrapper<View, Button> (oc);

			var child = new Button ();
			wrapper.Add (child);

			Button removedResult = null;
			int removeIndex = -1;
			wrapper.CollectionChanged += (sender, args) => {
				removedResult = args.OldItems[0] as Button;
				removeIndex = args.OldStartingIndex;
			};

			wrapper.Remove (child);

			Assert.AreEqual (0, removeIndex);
			Assert.AreEqual (child, removedResult);
		}

		[Test]
		public void INCCComplexRemove ()
		{
			var oc = new ObservableCollection<View> ();
			var wrapper = new ObservableWrapper<View, Button> (oc);

			oc.Add (new Stepper ());

			var child = new Button ();
			wrapper.Add (child);

			Button removedResult = null;
			int removeIndex = -1;
			wrapper.CollectionChanged += (sender, args) => {
				removedResult = args.OldItems[0] as Button;
				removeIndex = args.OldStartingIndex;
			};

			wrapper.Remove (child);

			Assert.AreEqual (child, removedResult);
			Assert.AreEqual (0, removeIndex);
		}

		[Test]
		public void INCCComplexRemoveLast ()
		{
			var oc = new ObservableCollection<View> ();
			var wrapper = new ObservableWrapper<View, Button> (oc);

			oc.Add (new Stepper ());

			wrapper.Add (new Button ());
			wrapper.Add (new Button ());
			var child = new Button ();
			wrapper.Add (child);

			Button removedResult = null;
			int removeIndex = -1;
			wrapper.CollectionChanged += (sender, args) => {
				removedResult = args.OldItems[0] as Button;
				removeIndex = args.OldStartingIndex;
			};

			wrapper.Remove (child);

			Assert.AreEqual (child, removedResult);
			Assert.AreEqual (2, removeIndex);
		}

		[Test]
		public void INCCReplace ()
		{
			var oc = new ObservableCollection<View> ();
			var wrapper = new ObservableWrapper<View, Button> (oc);

			var child1 = new Button ();
			var child2 = new Button ();

			wrapper.Add (child1);

			int index = -1;
			Button oldItem = null;
			Button newItem = null;
			wrapper.CollectionChanged += (sender, args) => {
				index = args.NewStartingIndex;
				oldItem = args.OldItems[0] as Button;
				newItem = args.NewItems[0] as Button;
			};

			wrapper[0] = child2;

			Assert.AreEqual (0, index);
			Assert.AreEqual (child1, oldItem);
			Assert.AreEqual (child2, newItem);
		}
	}
}
