using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using RuntimeDeviceType = Xamarin.Essentials.DeviceType;
using XUnitFilter = UnitTests.HeadlessRunner.Xunit.XUnitFilter;

namespace Xamarin.Platform.Handlers.DeviceTests
{
    public static class Traits
    {
        public const string DeviceType = "DeviceType";
        public const string InteractionType = "InteractionType";
        public const string UI = "UI";
        public const string FileProvider = "FileProvider";

        public static class Hardware
        {
        }

        public static class DeviceTypes
        {
            public const string Physical = "Physical";
            public const string Virtual = "Virtual";

            internal static string ToExclude =>
                DeviceInfo.DeviceType == RuntimeDeviceType.Physical ? Virtual : Physical;
        }

        public static class InteractionTypes
        {
            public const string Human = "Human";
            public const string Machine = "Machine";

            internal static string ToExclude => Human;
        }

        public static class FeatureSupport
        {
            public const string Supported = "Supported";
            public const string NotSupported = "NotSupported";

            internal static string ToExclude(bool hasFeature) =>
                hasFeature ? NotSupported : Supported;
        }

        public static List<XUnitFilter> GetCommonTraits(params XUnitFilter[] additionalFilters)
        {
            var filters = new List<XUnitFilter>
            {
            };

            if (additionalFilters != null && additionalFilters.Any())
                filters.AddRange(additionalFilters);

            return filters;
        }

        public static IEnumerable<string> GetSkipTraits(IEnumerable<string> additionalFilters = null)
        {
            return new string[0];
        }
    }
}
