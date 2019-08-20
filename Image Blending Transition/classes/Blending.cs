using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ImageBlendingTransition
{
	public class Blending
	{
		public static byte Opacity(byte value1, byte value2, float opacity)
		{
			return Calculations.ClampToByte(opacity * value1 + (1f - opacity) * value2);
		}

		public static Color Transparency(Color color1, Color color2, float transparency)
		{
			return Color.FromArgb(
				Opacity(color1.A, color2.A, transparency),
				Opacity(color1.R, color2.R, transparency),
				Opacity(color1.G, color2.G, transparency),
				Opacity(color1.B, color2.B, transparency)
			);
		}

		public static byte Darken(byte value1, byte value2)
		{
			return Math.Min(value1, value2);
		}

		public static Color Darken(Color color1, Color color2)
		{
			return Color.FromArgb(
				Darken(color1.A, color2.A),
				Darken(color1.R, color2.R),
				Darken(color1.G, color2.G),
				Darken(color1.B, color2.B)
			);
		}

		public static byte Multiply(byte value1, byte value2)
		{
			return Calculations.ClampToByte((value1 * value2) / 255);
		}

		public static Color Multiply(Color color1, Color color2)
		{
			return Color.FromArgb(
				Multiply(color1.A, color2.A),
				Multiply(color1.R, color2.R),
				Multiply(color1.G,  color2.G),
				Multiply(color1.B, color2.B)
			);
		}

		public static byte Lighten(byte value1, byte value2)
		{
			return Math.Max(value1, value2);
		}

		public static Color Lighten(Color color1, Color color2)
		{
			return Color.FromArgb(
				Lighten(color1.A, color2.A),
				Lighten(color1.R, color2.R),
				Lighten(color1.G, color2.G),
				Lighten(color1.B, color2.B)
			);
		}

		public static byte Screen(byte value1, byte value2)
		{
			return Calculations.ClampToByte(255 - ((255 - value1) * (255 - value2)) / 255);
		}

		public static Color Screen(Color color1, Color color2)
		{
			return Color.FromArgb(
				Screen(color1.A, color2.A),
				Screen(color1.R, color2.R),
				Screen(color1.G, color2.G),
				Screen(color1.B, color2.B)
			);
		}

		public static byte ColorBurn(byte value1, byte value2)
		{
			if (value1 == 0) return 0;

			return Calculations.ClampToByte(255 - (65025 - 255 * value2) / value1);
		}

		public static Color ColorBurn(Color color1, Color color2)
		{
			return Color.FromArgb(
				ColorBurn(color1.A, color2.A),
				ColorBurn(color1.R, color2.R),
				ColorBurn(color1.G, color2.G),
				ColorBurn(color1.B, color2.B)
			);
		}

		public static byte LinearBurn(byte value1, byte value2)
		{
			return Calculations.ClampToByte(value1 + value2 - 255);
		}

		public static Color LinearBurn(Color color1, Color color2)
		{
			return Color.FromArgb(
				LinearBurn(color1.A, color2.A),
				LinearBurn(color1.R, color2.R),
				LinearBurn(color1.G, color2.G),
				LinearBurn(color1.B, color2.B)
			);
		}

		public static byte ColorDodge(byte value1, byte value2)
		{
			if (value1 == 255) return 255;

			return Calculations.ClampToByte((255 * value2) / (255 - value1));
		}

		public static Color ColorDodge(Color color1, Color color2)
		{
			return Color.FromArgb(
				ColorDodge(color1.A, color2.A),
				ColorDodge(color1.R, color2.R),
				ColorDodge(color1.G, color2.G),
				ColorDodge(color1.B, color2.B)
			);
		}

		public static byte LinearDodge(byte value1, byte value2)
		{
			return Calculations.ClampToByte(value1 + value2);
		}

		public static Color LinearDodge(Color color1, Color color2)
		{
			return Color.FromArgb(
				LinearDodge(color1.A, color2.A),
				LinearDodge(color1.R, color2.R),
				LinearDodge(color1.G, color2.G),
				LinearDodge(color1.B, color2.B)
			);
		}

		public static byte Overlay(byte value1, byte value2)
		{
			return Calculations.ClampToByte(value2 <= 127 ?
												(value1 * value2) / 127.5f :
												255 - ((255 - value1) * (255 - value2)) / 127.5f);
		}

		public static Color Overlay(Color color1, Color color2)
		{
			return Color.FromArgb(
				Overlay(color1.A, color2.A),
				Overlay(color1.R, color2.R),
				Overlay(color1.G, color2.G),
				Overlay(color1.B, color2.B)
			);
		}

		public static byte SoftLight(byte value1, byte value2)
		{
			float floatValue1 = value1 / 255f;
			float floatValue2 = value2 / 255f;

			return Calculations.ClampToByte(floatValue1 <= 0.5 ?
					255 * ((2f * floatValue1 - 1f) * (floatValue2 - floatValue2 * floatValue2) + floatValue2) :
					255 * ((2f * floatValue1 - 1f) * (Math.Sqrt(floatValue2) - floatValue2) + floatValue2));
		}

		public static Color SoftLight(Color color1, Color color2)
		{
			return Color.FromArgb(
				SoftLight(color1.A, color2.A),
				SoftLight(color1.R, color2.R),
				SoftLight(color1.G, color2.G),
				SoftLight(color1.B, color2.B)
			);
		}

		public static byte HardLight(byte value1, byte value2)
		{
			return Calculations.ClampToByte(value1 <= 127 ?
												(value1 * value2) / 127.5f :
												255f - ((255 - value1) * (255 - value2)) / 127.5f
			);
		}

		public static Color HardLight(Color color1, Color color2)
		{
			return Color.FromArgb(
				HardLight(color1.A, color2.A),
				HardLight(color1.R, color2.R),
				HardLight(color1.G, color2.G),
				HardLight(color1.B, color2.B)
			);
		}

		public static byte Difference(byte value1, byte value2)
		{
			return Calculations.ClampToByte(Math.Abs(value1 - value2));
		}

		public static Color Difference(Color color1, Color color2)
		{
			return Color.FromArgb(
				Difference(color1.A, color2.A),
				Difference(color1.R, color2.R),
				Difference(color1.G, color2.G),
				Difference(color1.B, color2.B)
			);
		}
	}
}
