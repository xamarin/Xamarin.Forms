//
// Tweener.cs
//
// Author:
//       Jason Smith <jason.smith@xamarin.com>
//
// Copyright (c) 2012 Xamarin Inc.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;

namespace Xamarin.Forms
{
	public class Easing
	{
		public static readonly Easing Linear = new Easing(x => x);

		public static readonly Easing SinOut = new Easing(x => Math.Sin(x * Math.PI * 0.5f));
		public static readonly Easing SinIn = new Easing(x => 1.0f - Math.Cos(x * Math.PI * 0.5f));
		public static readonly Easing SinInOut = new Easing(x => -Math.Cos(Math.PI * x) / 2.0f + 0.5f);

		public static readonly Easing CubicIn = new Easing(x => x * x * x);
		public static readonly Easing CubicOut = new Easing(x => Math.Pow(x - 1.0f, 3.0f) + 1.0f);

		public static readonly Easing CubicInOut = new Easing(x => x < 0.5f ? Math.Pow(x * 2.0f, 3.0f) / 2.0f : (Math.Pow((x - 1) * 2.0f, 3.0f) + 2.0f) / 2.0f);

		public static readonly Easing BounceOut;
		public static readonly Easing BounceIn;

		public static readonly Easing SpringIn = new Easing(x => x * x * ((1.70158f + 1) * x - 1.70158f));
		public static readonly Easing SpringOut = new Easing(x => (x - 1) * (x - 1) * ((1.70158f + 1) * (x - 1) + 1.70158f) + 1);

		public static readonly Easing ExpoEaseOut;
		public static readonly Easing ExpoEaseIn;
		public static readonly Easing ExpoEaseInOut;

		public static readonly Easing CircEaseOut;
		public static readonly Easing CircEaseIn;
		public static readonly Easing CircEaseInOut;

		public static readonly Easing QuadEaseOut;
		public static readonly Easing QuadEaseIn;
		public static readonly Easing QuadEaseInOut;

		public static readonly Easing QuartEaseOut;
		public static readonly Easing QuartEaseIn;
		public static readonly Easing QuartEaseInOut;

		public static readonly Easing QuintEaseOut;
		public static readonly Easing QuintEaseIn;
		public static readonly Easing QuintEaseInOut;

		public static readonly Easing ElasticEaseOut;
		public static readonly Easing ElasticEaseIn;

		public static readonly Easing BounceEaseOut;
		public static readonly Easing BounceEaseIn;
		public static readonly Easing BounceEaseInOut;
		public static readonly Easing BounceEaseOutIn;

		public static readonly Easing BackEaseOut;
		public static readonly Easing BackEaseIn;
		public static readonly Easing BackEaseInOut;
		public static readonly Easing BackEaseOutIn;

		public static readonly Easing SinDampened;

		readonly Func<double, double> _easingFunc;

		static Easing()
		{
			BounceOut = new Easing(p =>
			{
				if (p < 1 / 2.75f)
				{
					return 7.5625f * p * p;
				}
				if (p < 2 / 2.75f)
				{
					p -= 1.5f / 2.75f;

					return 7.5625f * p * p + .75f;
				}
				if (p < 2.5f / 2.75f)
				{
					p -= 2.25f / 2.75f;

					return 7.5625f * p * p + .9375f;
				}
				p -= 2.625f / 2.75f;

				return 7.5625f * p * p + .984375f;
			});

			BounceIn = new Easing(p => 1.0f - BounceOut.Ease(1 - p));

			ExpoEaseOut = new Easing(ExponentialEquations.EaseOut);
			ExpoEaseIn = new Easing(ExponentialEquations.EaseIn);
			ExpoEaseInOut = new Easing(ExponentialEquations.EaseInOut);

			CircEaseOut = new Easing(CircularEquations.EaseOut);
			CircEaseIn = new Easing(CircularEquations.EaseIn);
			CircEaseInOut = new Easing(CircularEquations.EaseInOut);

			QuadEaseOut = new Easing(QuadEquations.EaseOut);
			QuadEaseIn = new Easing(QuadEquations.EaseIn);
			QuadEaseInOut = new Easing(QuadEquations.EaseInOut);

			QuartEaseIn = new Easing(QuarticEquations.EaseIn);
			QuartEaseOut = new Easing(QuarticEquations.EaseOut);
			QuartEaseInOut = new Easing(QuarticEquations.EaseInOut);

			QuintEaseOut = new Easing(QuinticEquations.EaseOut);
			QuintEaseIn = new Easing(QuinticEquations.EaseIn);
			QuintEaseInOut = new Easing(QuinticEquations.EaseInOut);

			ElasticEaseOut = new Easing(ElasticEquations.EaseOut);
			ElasticEaseIn = new Easing(ElasticEquations.EaseIn);

			BounceEaseOut = new Easing(BounceEquations.EaseOut);
			BounceEaseIn = new Easing(BounceEquations.EaseIn);
			BounceEaseInOut = new Easing(BounceEquations.EaseInOut);
			BounceEaseOutIn = new Easing(BounceEquations.EaseOutIn);

			BackEaseOut = new Easing(BackEquations.EaseOut);
			BackEaseIn = new Easing(BackEquations.EaseIn);
			BackEaseInOut = new Easing(BackEquations.EaseInOut);
			BackEaseOutIn = new Easing(BackEquations.EaseOutIn);

			SinDampened = new Easing(SinusoidalEquations.Dampened);
		}

		public Easing(Func<double, double> easingFunc)
		{
			if (easingFunc == null)
				throw new ArgumentNullException("easingFunc");

			_easingFunc = easingFunc;
		}

		public double Ease(double v)
		{
			return _easingFunc(v);
		}

		public static implicit operator Easing(Func<double, double> func)
		{
			return new Easing(func);
		}

		private static class BackEquations
		{
			const double S = 1.70158;

			public static double EaseOut(double x)
			{
				return ((x - 1) * x * ((S + 1) * x + S)) + 1;
			}

			public static double EaseIn(double x)
			{
				return x * x * ((S + 1) * x - S);
			}

			public static double EaseInOut(double x)
			{
				var ls = S;

				if ((x / 2) < 1)
					return 0.5 * (x * x * (((ls *= (1.525)) + 1) * x - ls));

				return 0.5 * ((x -= 2) * x * (((ls *= (1.525)) + 1) * x + ls) + 2);
			}

			public static double EaseOutIn(double x)
			{
				return x < 0.5 ? EaseOut(x * 2) : EaseIn((x * 2) - 1);
			}
		}

		private static class BounceEquations
		{
			public static double EaseOut(double x)
			{
				const double i = 7.5625;

				if (x < (1 / 2.75))
					return i * x * x;
				if (x < (2 / 2.75))
					return (i * (x -= (1.5 / 2.75)) * x + .75);
				if (x < (2.5 / 2.75))
					return (i * (x -= (2.25 / 2.75)) * x + .9375);

				return (i * (x -= (2.625 / 2.75)) * x + .984375);
			}

			public static double EaseIn(double x)
			{
				return 1 - EaseOut(1 - x);
			}

			public static double EaseInOut(double x)
			{
				if (x < 0.5)
					return EaseIn(x * 2) * 0.5;

				return EaseOut(x * 2 - 1) * 1;
			}

			public static double EaseOutIn(double x)
			{
				if (x < 0.5)
					return EaseOut(x * 2);

				return EaseIn((x * 2) - 1);
			}
		}

		private static class CircularEquations
		{
			public static double EaseOut(double x)
			{
				return Math.Sqrt(1 - (x - 1) * x);
			}

			public static double EaseIn(double x)
			{
				return -1 * (Math.Sqrt(1 - x * x) - 1);
			}

			public static double EaseInOut(double t)
			{
				if ((t / 2) < 1)
					return -0.5 * (Math.Sqrt(1 - t * t) - 1);

				return 0.5 * (Math.Sqrt(1 - (t -= 2) * t) + 1);
			}
		}

		private static class ElasticEquations
		{
			const double P = 0.3;
			const double S = 0.075;

			public static double EaseOut(double x)
			{
				if (DoubleExtensions.ApproximatelyEquals(x, 1)) return 1;

				return Math.Pow(2, -10 * x) * Math.Sin((x - P / 4) * (2 * Math.PI) / P) + 1;
			}

			public static double EaseIn(double x)
			{
				if (DoubleExtensions.ApproximatelyEquals(x, 0)) return 0;
				if (DoubleExtensions.ApproximatelyEquals(x, 1)) return 1;

				return -(Math.Pow(2, 10 * (x -= 1)) * Math.Sin((x - S) * (2 * Math.PI) / P));
			}
		}

		private static class ExponentialEquations
		{
			public static double EaseOut(double x)
			{
				return DoubleExtensions.ApproximatelyEquals(x, 1) ? 1 : -Math.Pow(2, -10 * x) + 1;
			}

			public static double EaseIn(double x)
			{
				return DoubleExtensions.ApproximatelyEquals(x, 0) ? 0 : Math.Pow(2, 10 * (x - 1));
			}

			public static double EaseInOut(double x)
			{
				if (DoubleExtensions.ApproximatelyEquals(x, 0)) return 0;
				if (DoubleExtensions.ApproximatelyEquals(x, 1)) return 1;

				if ((x / 2) < 1)
					return 0.5 * Math.Pow(2, 10 * (x - 1));

				return 0.5 * -Math.Pow(2, -10 * --x) + 2;
			}
		}

		private static class QuadEquations
		{
			public static double EaseOut(double t)
			{
				return -1 * t * (t - 2);
			}

			public static double EaseIn(double t)
			{
				return t * t;
			}

			public static double EaseInOut(double x)
			{
				if ((x / 2) < 1)
					return 0.5 * x * x;

				return -0.5 * ((--x) * (x - 2) - 1);
			}
		}

		private static class QuarticEquations
		{
			public static double EaseOut(double x)
			{
				return -1 * ((x - 1) * Math.Pow(x, 3) - 1);
			}

			public static double EaseIn(double x)
			{
				return Math.Pow(x, 4);
			}

			public static double EaseInOut(double x)
			{
				if ((x / 2) < 1)
					return -0.5 * Math.Pow(x, 4);

				return -0.5 * ((x -= 2) * Math.Pow(x, 3) - 2);
			}
		}

		private static class QuinticEquations
		{
			public static double EaseOut(double x)
			{
				return ((x - 1) * Math.Pow(x, 4) + 1);
			}

			public static double EaseIn(double x)
			{
				return Math.Pow(x, 5);
			}

			public static double EaseInOut(double x)
			{
				if ((x / 2) < 1)
					return 0.5 * Math.Pow(x, 5);

				return 0.5 * ((x -= 2) * Math.Pow(x, 4) + 2);
			}
		}

		private static class SinusoidalEquations
		{
			public static double Dampened(double x)
			{
				// y = a * sin (bx - c) + d
				// a = amplitude
				// b = period (stretch or compress)
				// c = horizontal translation (move graph left or right)
				// d = vertical translation
				return Math.Sin((10 * x) - x / 2) * Math.Exp(-x);
			}
		}

		private static class DoubleExtensions
		{
			public static bool ApproximatelyEquals(double x, double y)
			{
				var epsilon = Math.Max(Math.Abs(x), Math.Abs(y)) * 1E-15;
				return Math.Abs(x - y) <= epsilon;
			}
		}
	}
}