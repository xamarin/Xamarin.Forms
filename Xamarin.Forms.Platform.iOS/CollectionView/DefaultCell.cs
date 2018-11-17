﻿using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace Xamarin.Forms.Platform.iOS
{
	public abstract class DefaultCell : ItemsViewCell
	{
		public UILabel Label { get; }

		protected NSLayoutConstraint Constraint { get; set; }

		[Export("initWithFrame:")]
		protected DefaultCell(CGRect frame) : base(frame)
		{
			CollectionView.VerifyCollectionViewFlagEnabled(nameof(DefaultCell));

			Label = new UILabel(frame)
			{
				TextColor = UIColor.Black,
				Lines = 1,
				Font = UIFont.PreferredBody,
				TranslatesAutoresizingMaskIntoConstraints = false
			};

			ContentView.BackgroundColor = UIColor.Clear;

			InitializeContentConstraints(Label);
		}

		public override void ConstrainTo(nfloat constant)
		{
			Constraint.Constant = constant;
		}
	}
}