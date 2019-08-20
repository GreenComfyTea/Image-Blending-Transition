using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ImageBlendingTransition
{
    public class ByteArray
    {
		public byte[] PrimitiveArray { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public int Channels { get; set; }
		public int Stride { get; set; }

		public ByteArray(ByteArray byteArray)
		{
			PrimitiveArray = (byte[]) byteArray.PrimitiveArray.Clone();
			Width = byteArray.Width;
			Height = byteArray.Height;
			Channels = byteArray.Channels;
			Stride = byteArray.Stride;
		}

		public ByteArray(int width, int height, int channels)
		{
			Width = width;
			Height = height;
			Channels = channels;
			Stride = CalculateStride(width, channels);

			PrimitiveArray = new byte[Stride * Height];
		}

		public ByteArray Clone()
		{
			return new ByteArray(this);
		}

		public int GetIndex(int x, int y)
		{
			return y * Stride + x * Channels;
		}

		public Point GetCoordinates(int index)
		{
			int x = (index / Channels) % Width;
			int y = index / (Width * Channels);

			return new Point(x, y);
		}

		public Color GetPixelColor(int x, int y)
		{
			if (Channels == 0)
			{
				return Colors.Black;
			}

			int index = GetIndex(x, y);

			if (Channels == 1)
			{
				byte intensity = PrimitiveArray[index];
				return Color.FromRgb(intensity, intensity, intensity);
			}

			if (Channels == 3)
			{
				byte blue = PrimitiveArray[index++];
				byte green = PrimitiveArray[index++];
				byte red = PrimitiveArray[index];
				return Color.FromRgb(red, green, blue);
			}

			if (Channels == 4)
			{
				byte blue = PrimitiveArray[index++];
				byte green = PrimitiveArray[index++];
				byte red = PrimitiveArray[index++];
				byte alpha = PrimitiveArray[index];
				return Color.FromArgb(alpha, red, green, blue);
			}

			return Colors.Black;
		}

		public static int CalculateStride(int width, int channels)
		{
			return width * channels;
		}
	}
}
