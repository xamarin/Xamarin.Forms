using NUnit.Framework;
using System;
using System.Linq;

namespace Xamarin.Forms.Core.UnitTests
{
	[TestFixture]
	public class AcceleratorUnitTests : BaseTestFixture
	{

		[Test]
		public void AcceleratorThrowsOnEmptyString()
		{
			Assert.Throws<ArgumentNullException>(() => Accelerator.FromString(""));
		}

		[Test]
		public void AcceleratorThrowsOnNull()
		{
			Assert.Throws<ArgumentNullException>(() => Accelerator.FromString(null));
		}

		[Test]
		public void AcceleratorFromString()
		{
			string shourtCutKeyBinding = "ctrl+A";
			var accelerator = Accelerator.FromString(shourtCutKeyBinding);

			Assert.AreEqual(shourtCutKeyBinding, accelerator.ToString());
		}

		[Test]
		public void AcceleratorFromOnlyLetter()
		{
			string shourtCutKeyBinding = "A";
			var accelerator = Accelerator.FromString(shourtCutKeyBinding);

			Assert.AreEqual(accelerator.Keys.Count(), 1);
			Assert.AreEqual(accelerator.Keys.ElementAt(0), shourtCutKeyBinding);
		}

		[Test]
		public void AcceleratorFromLetterAndModifier()
		{
			string modifier = "ctrl";
			string key = "A";
			string shourtCutKeyBinding = $"{modifier}+{key}";
			var accelerator = Accelerator.FromString(shourtCutKeyBinding);

			Assert.AreEqual(accelerator.Keys.Count(), 1);
			Assert.AreEqual(accelerator.Modififiers.Count(), 1);
			Assert.AreEqual(accelerator.Keys.ElementAt(0), key);
			Assert.AreEqual(accelerator.Modififiers.ElementAt(0), modifier);
		}


		[Test]
		public void AcceleratorFromLetterAnd2Modifier()
		{
			string modifier = "ctrl";
			string modifier1Alt= "alt";
			string key = "A";
			string shourtCutKeyBinding = $"{modifier}+{modifier1Alt}+{key}";
			var accelerator = Accelerator.FromString(shourtCutKeyBinding);

			Assert.AreEqual(accelerator.Keys.Count(), 1);
			Assert.AreEqual(accelerator.Modififiers.Count(), 2);
			Assert.AreEqual(accelerator.Keys.ElementAt(0), key);
			Assert.AreEqual(accelerator.Modififiers.ElementAt(0), modifier);
			Assert.AreEqual(accelerator.Modififiers.ElementAt(1), modifier1Alt);
		}
	}
}
