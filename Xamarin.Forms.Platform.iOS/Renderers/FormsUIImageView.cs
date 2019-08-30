using CoreGraphics;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public class FormsUIImageView : UIImageView
	{
		const string AnimationLayerName = "FormsUIImageViewAnimation";
		FormsCAKeyFrameAnimation _animation;
		bool _autoPlay;

		public FormsUIImageView(CGRect frame) : base(frame)
		{
		}

		public override CGSize SizeThatFits(CGSize size)
		{
			if (Image == null && Animation != null)
			{
				return new CoreGraphics.CGSize(Animation.Width, Animation.Height);
			}

			return base.SizeThatFits(size);
		}

		public bool AutoPlay
		{
			get { return _autoPlay; }
			set
			{
				_autoPlay = value;
				if (_animation != null)
				{
					Layer.Speed = _autoPlay ? 1.0f : 0.0f;
				}
			}
		}

		public FormsCAKeyFrameAnimation Animation
		{
			get { return _animation; }
			set
			{
				if (_animation != null)
				{
					Layer.RemoveAnimation(AnimationLayerName);
					_animation.Dispose();
				}

				_animation = value;
				if (_animation != null)
				{
					Layer.AddAnimation(_animation, AnimationLayerName);
					Layer.Speed = AutoPlay ? 1.0f : 0.0f;
				}

				Layer.SetNeedsDisplay();
			}
		}

		public override bool IsAnimating
		{
			get
			{
				if (_animation != null)
					return Layer.Speed != 0.0f;
				else
					return base.IsAnimating;
			}
		}

		public override void StartAnimating()
		{
			if (_animation != null && Layer.Speed == 0.0f)
			{
				Layer.RemoveAnimation(AnimationLayerName);
				Layer.AddAnimation(_animation, AnimationLayerName);
				Layer.Speed = 1.0f;
			}
			else
			{
				base.StartAnimating();
			}
		}

		public override void StopAnimating()
		{
			if (_animation != null && Layer.Speed != 0.0f)
			{
				Layer.RemoveAnimation(AnimationLayerName);
				Layer.AddAnimation(_animation, AnimationLayerName);
				Layer.Speed = 0.0f;
			}
			else
			{
				base.StopAnimating();
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && _animation != null)
			{
				Layer.RemoveAnimation(AnimationLayerName);
				_animation.Dispose();
				_animation = null;
			}

			base.Dispose(disposing);
		}
	}
}