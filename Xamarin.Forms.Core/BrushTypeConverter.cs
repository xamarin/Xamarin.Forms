using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Xamarin.Forms
{
	public class BrushTypeConverter : TypeConverter
    {
        public const string LinearGradient = "linear-gradient";
        public const string RadialGradient = "radial-gradient";
        public const string Rgb = "rgb";
        public const string Rgba = "rgba";
        public const string Hsl = "hsl";
        public const string Hsla = "hsla";

        readonly ColorTypeConverter _colorTypeConverter = new ColorTypeConverter();

        public override object ConvertFromInvariantString(string value)
        {
            if (value != null)
            {
                value = value.Trim();

                if (value.StartsWith(LinearGradient) || value.StartsWith(RadialGradient))
                {
                    var gradientBrushParser = new GradientBrushParser();
                    var brush = gradientBrushParser.Parse(value);

                    if (brush != null)
                        return brush;
                }

                if (value.StartsWith(Rgb) || value.StartsWith(Rgba) || value.StartsWith(Hsl) || value.StartsWith(Hsla))
                {
                    return _colorTypeConverter.ConvertFromInvariantString(value);
                }
            }

            return new SolidColorBrush(Color.Default);
        }
		      
        public class GradientBrushParser
        {
            readonly ColorTypeConverter _colorConverter = new ColorTypeConverter();

            GradientBrush _gradient;
            string[] _parts;
            int _position;

            public GradientBrush Parse(string css)
            {
                if (string.IsNullOrWhiteSpace(css))
                {
                    return _gradient;
                }

                _parts = css.Replace("\r\n", "").Split(new[] { '(', ')', ',' }, StringSplitOptions.RemoveEmptyEntries);

                while (_position < _parts.Length)
                {
                    var part = _parts[_position].Trim();

					// Hex Color
					if(part.StartsWith("#", StringComparison.Ordinal))
					{
                        var parts = part.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        var color = (Color)_colorConverter.ConvertFromInvariantString(parts[0]);

                        if (TryParseOffsets(parts, out var offsets))
                            AddGradientStops(color, offsets);
                        else
                            AddGradientStop(color);
                    }

					// Color by name
                    var colorParts = part.Split('.');
                    if (colorParts[0] == "Color")
                    {
                        var parts = part.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        var color = (Color)_colorConverter.ConvertFromInvariantString(parts[0]);

                        if (TryParseOffsets(parts, out var offsets))
                            AddGradientStops(color, offsets);
                        else
                            AddGradientStop(color);
                    }

					// Color (Rgb, Rgba, Hsl, Hsla)
                    if (part.Equals(Rgb, StringComparison.OrdinalIgnoreCase)
						|| part.Equals(Rgba, StringComparison.OrdinalIgnoreCase)
                        || part.Equals(Hsl, StringComparison.OrdinalIgnoreCase)
                        || part.Equals(Hsla, StringComparison.OrdinalIgnoreCase))
					{
                        part.Trim();
                        var colorString = new StringBuilder(part);

                        colorString.Append('(');
                        colorString.Append(GetNextPart());
                        colorString.Append(',');
                        colorString.Append(GetNextPart());
                        colorString.Append(',');
                        colorString.Append(GetNextPart());

                        if (part == Rgba || part == Hsla)
                        {
                            colorString.Append(',');
                            colorString.Append(GetNextPart());
                        }

                        colorString.Append(')');

                        var color = (Color)_colorConverter.ConvertFromInvariantString(colorString.ToString());
                        var parts = GetNextPart().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        if (TryParseOffsets(parts, out var offsets))
                            AddGradientStops(color, offsets);
                        else
                        {
                            AddGradientStop(color);
                            _position--;
                        }
                    }

					// LinearGradient
					if (part == LinearGradient)
					{
                        var direction = GetNextPart().Trim();
						var hasAngle = TryParseAngle(direction, out var angle);

						if (hasAngle)
                            CreateLinearGradient(angle);
                        else
                        {
                            CreateLinearGradient(0);
                            _position--;
                        }
                    }

                    // RadialGradient
                    if (part == RadialGradient)
                    {
                        CreateRadialGradient(new Point(0.5, 0.5));
                    }

                    _position++;
                }

                return _gradient;
            }
						
            string GetNextPart()
            {
                _position++;

                if (!(_position < _parts.Length))
                    return string.Empty;

                return _parts[_position];
            }

            void CreateLinearGradient(double angle)
            {
                _gradient = new LinearGradientBrush
                {
                    GradientStops = new GradientStopCollection()
                };
            }

            void CreateRadialGradient(Point center)
            {
                _gradient = new RadialGradientBrush
                {
                    Center = center,
                    GradientStops = new GradientStopCollection()
                };
            }

            void AddGradientStop(Color color, float? offset = null)
            {
                if (_gradient == null)
                {
                    CreateLinearGradient(0);
                }

                var stop = new GradientStop
                {
                    Color = color,
                    Offset = offset ?? -1
                };

                _gradient.GradientStops.Add(stop);
            }

            void AddGradientStops(Color color, IEnumerable<float> offsets)
            {
                foreach (var offset in offsets)
                    AddGradientStop(color, offset);
            }

            bool TryParseAngle(string part, out double angle)
            {
                if (TryParseNumber(part, "deg", out var degrees))
                {
                    angle = (180 + degrees) % 360;
                    return true;
                }

                if (TryParseNumber(part, "turn", out var turn))
                {
                    angle = 180 + (360 * turn);
                    return true;
                }

                angle = 0;
                return false;
            }
			
            bool TryParseNumber(string part, string unit, out float result)
            {
                if (part.EndsWith(unit))
                {
                    var index = part.LastIndexOf(unit, StringComparison.OrdinalIgnoreCase);
                    var number = part.Substring(0, index);

                    if (float.TryParse(number, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
                    {
                        result = value;
                        return true;
                    }
                }

                result = 0;
                return false;
            }

            bool TryParseOffset(string part, out float result)
            {
                if (part != null)
                {
					// Using percentage
                    if (TryParseNumber(part, "%", out var value))
                    {
                        result = Math.Min(value / 100, 1f);
                        return true;
                    }

					// Using px
                    if (TryParseNumber(part, "px", out result))
                    {
                        return true;
                    }
                }

                result = 0;
                return false;
            }

            bool TryParseOffsets(string[] parts, out float[] result)
            {
                var offsets = new List<float>();

                foreach (var part in parts)
                {
                    if (TryParseOffset(part, out var offset))
                        offsets.Add(offset);
                }

                result = offsets.ToArray();
                return result.Length > 0;
            }
        }
    }
}