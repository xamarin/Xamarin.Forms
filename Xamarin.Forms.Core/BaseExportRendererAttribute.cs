using System;

namespace Xamarin.Forms.Core
{
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public abstract class BaseExportRendererAttribute : HandlerAttribute
	{
		protected int MajorVersion { get; set; }

		#region MinimumSdkVersion
		int _minimumSdkVersion = int.MinValue;
		public int MinimumSdkVersion
		{
			get { return _minimumSdkVersion; }
			set
			{
				if (_minimumSdkVersion == value)
					return;

				_minimumSdkVersion = value;

				if (_minimumSdkVersion > MaximumSdkVersion)
					throw new ArgumentException("Minimum SDK version must be less than or equal to the maximum SDK version.");
			}
		}
		#endregion

		#region MaximumSdkVersion
		int _maximumSdkVersion = int.MaxValue;
		public int MaximumSdkVersion
		{
			get { return _maximumSdkVersion; }
			set
			{
				if (_maximumSdkVersion == value)
					return;

				_maximumSdkVersion = value;

				if (_maximumSdkVersion < MinimumSdkVersion)
					throw new ArgumentException("Maximum SDK version must be greater than or equal to the minimum SDK version.");
			}
		}
		#endregion

		protected BaseExportRendererAttribute(Type handler, Type target) : base(handler, target)
		{
		}

		public override bool ShouldRegister()
		{
			if (MinimumSdkVersion <= MajorVersion && MajorVersion <= MaximumSdkVersion)
				return base.ShouldRegister();

			return false;
		}
	}
}