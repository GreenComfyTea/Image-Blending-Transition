using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageBlendingTransition
{
	class Calculations
	{
		public static byte AffineTransformationByte(byte value, byte oldMin, byte oldMax, byte newMin, byte newMax)
		{
			return Convert.ToByte(AffineTransformation(value, oldMin, oldMax, newMin, newMax));
		}

		public static double AffineTransformation(double value, double oldMin, double oldMax, double newMin, double newMax)
		{
			if (value < oldMin || value > oldMax)
			{
				throw new Exception("Incorrect arguments");
			}

			return ((value - oldMin) * ((newMax - newMin) / (oldMax - oldMin))) + newMin;
		}

		public static byte ClampToByte(int value)
		{
			if (value < 0) return 0;
			else if (value > 255) return 255;
			return Convert.ToByte(value);
		}

		public static byte ClampToByte(float value)
		{
			if (value < 0f) return 0;
			else if (value > 255f) return 255;
			return Convert.ToByte(value);
		}

		public static byte ClampToByte(double value)
		{
			if (value < 0d) return 0;
			else if (value > 255d) return 255;
			return Convert.ToByte(value);
		}

		public static float ClampToNormal(float value)
		{
			if (value < 0f) return 0f;
			else if (value > 255d) return 1f;
			return value;
		}

		public static float ClampToNormal(double value)
		{
			if (value < 0d) return 0f;
			else if (value > 255d) return 1f;
			return (float) value;
		}
	}
}
