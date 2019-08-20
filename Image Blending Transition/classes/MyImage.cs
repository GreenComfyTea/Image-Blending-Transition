using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ImageBlendingTransition
{
	class MyImage
	{
		public WriteableBitmap WriteableBitmap { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }
		public int Channels { get; set; }
		public ByteArray ByteArray { get; set; }

		private bool IsBlendingCalculated { get; set; }

		public MyImage()
		{
			WriteableBitmap = null;
			Width = -1;
			Height = -1;
			Channels = -1;
			ByteArray = null;
		}

		public MyImage(WriteableBitmap writeableBitmap)
		{
			WriteableBitmap = writeableBitmap;
			Width = WriteableBitmap.PixelWidth;
			Height = WriteableBitmap.PixelHeight;
			Channels = ((WriteableBitmap.Format.BitsPerPixel + 7) / 8);

			ByteArray = new ByteArray(Width, Height, Channels);

			WriteableBitmap.CopyPixels(ByteArray.PrimitiveArray, ByteArray.Stride, 0);
		}

		private void Init(MyImage myImage)
		{
			WriteableBitmap = myImage.WriteableBitmap.Clone();
			Width = myImage.Width;
			Height = myImage.Height;
			Channels = myImage.Channels;
			ByteArray = new ByteArray(myImage.ByteArray);
		}

		public MyImage Clone()
		{
			MyImage myImage = new MyImage();
			myImage.Init(this);
			return myImage;
		}

		public void Opacity(MyImage myImage2, float opacity, TransitionModes transitionMode = TransitionModes.NoTransition, float previousPercentage = 0f, float percentage = 1f)
		{
			if(IsBlendingCalculated)
			{
				return;
			}

			percentage = Calculations.ClampToNormal(percentage);
			previousPercentage = Calculations.ClampToNormal(previousPercentage);

			InitBoundaries(previousPercentage, percentage,
				out float previousLinearBoundaryX,			out float linearBoundaryX,			out float previousReversedLinearBoundaryX,		out float reversedLinearBoundaryX,
				out float previousLinearBoundaryY,			out float linearBoundaryY,			out float previousReversedLinearBoundaryY,		out float reversedLinearBoundaryY,
				out float previousRightHalfLinearBoundaryX, out float rightHalfLinearBoundaryX, out float previousLeftHalfLinearBoundaryX,		out float leftHalfLinearBoundaryX,
				out float previousLowerHalfLinearBoundaryY, out float lowerHalfLinearBoundaryY, out float previousHigherHalfLinearBoundaryY,	out float higherHalfLinearBoundaryY,
				out float previusRadiusBoundary,			out float radiusBoundary);

			int[] nums = Enumerable.Range(0, ByteArray.PrimitiveArray.Length / Channels).Select(i => i * Channels).ToArray();
			Parallel.ForEach(nums, i =>
			{
				Point coordinates = ByteArray.GetCoordinates(i);

				bool isValid = ValidateBoundaries(coordinates, transitionMode,
					previousLinearBoundaryX,			linearBoundaryX,			previousReversedLinearBoundaryX,	reversedLinearBoundaryX,
					previousLinearBoundaryY,			linearBoundaryY,			previousReversedLinearBoundaryY,	reversedLinearBoundaryY,
					previousLeftHalfLinearBoundaryX,	leftHalfLinearBoundaryX,	previousRightHalfLinearBoundaryX,	rightHalfLinearBoundaryX,
					previousHigherHalfLinearBoundaryY,	higherHalfLinearBoundaryY,	previousLowerHalfLinearBoundaryY,	lowerHalfLinearBoundaryY,	
					previusRadiusBoundary,				radiusBoundary);

				if (!isValid) return;

				for (int channel = 0; channel < Channels; channel++)
				{
					ByteArray.PrimitiveArray[i + channel] =  Calculations.ClampToByte(Blending.Opacity(ByteArray.PrimitiveArray[i + channel], myImage2.ByteArray.PrimitiveArray[i + channel], opacity));
				}
			});

			WriteableBitmap writeableBitmap = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr32, null, ByteArray.PrimitiveArray, ByteArray.Stride));
			WriteableBitmap = writeableBitmap;
		}

		public void Darken(MyImage myImage2, TransitionModes transitionMode = TransitionModes.NoTransition, float previousPercentage = 0f, float percentage = 1f)
		{
			if (IsBlendingCalculated)
			{
				return;
			}

			percentage = Calculations.ClampToNormal(percentage);
			previousPercentage = Calculations.ClampToNormal(previousPercentage);

			InitBoundaries(previousPercentage, percentage,
				out float previousLinearBoundaryX, out float linearBoundaryX, out float previousReversedLinearBoundaryX, out float reversedLinearBoundaryX,
				out float previousLinearBoundaryY, out float linearBoundaryY, out float previousReversedLinearBoundaryY, out float reversedLinearBoundaryY,
				out float previousRightHalfLinearBoundaryX, out float rightHalfLinearBoundaryX, out float previousLeftHalfLinearBoundaryX, out float leftHalfLinearBoundaryX,
				out float previousLowerHalfLinearBoundaryY, out float lowerHalfLinearBoundaryY, out float previousHigherHalfLinearBoundaryY, out float higherHalfLinearBoundaryY,
				out float previusRadiusBoundary, out float radiusBoundary);

			int[] nums = Enumerable.Range(0, ByteArray.PrimitiveArray.Length / Channels).Select(i => i * Channels).ToArray();
			Parallel.ForEach(nums, i =>
			{
				Point coordinates = ByteArray.GetCoordinates(i);

				bool isValid = ValidateBoundaries(coordinates, transitionMode,
					previousLinearBoundaryX, linearBoundaryX, previousReversedLinearBoundaryX, reversedLinearBoundaryX,
					previousLinearBoundaryY, linearBoundaryY, previousReversedLinearBoundaryY, reversedLinearBoundaryY,
					previousLeftHalfLinearBoundaryX, leftHalfLinearBoundaryX, previousRightHalfLinearBoundaryX, rightHalfLinearBoundaryX,
					previousHigherHalfLinearBoundaryY, higherHalfLinearBoundaryY, previousLowerHalfLinearBoundaryY, lowerHalfLinearBoundaryY,
					previusRadiusBoundary, radiusBoundary);

				if (!isValid) return;

				for (int channel = 0; channel < Channels; channel++)
				{
					ByteArray.PrimitiveArray[i + channel] = Calculations.ClampToByte(Blending.Darken(ByteArray.PrimitiveArray[i + channel], myImage2.ByteArray.PrimitiveArray[i + channel]));
				}
			});

			WriteableBitmap writeableBitmap = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr32, null, ByteArray.PrimitiveArray, ByteArray.Stride));
			WriteableBitmap = writeableBitmap;
		}

		public void Multiply(MyImage myImage2, TransitionModes transitionMode = TransitionModes.NoTransition, float previousPercentage = 0f, float percentage = 1f)
		{
			if (IsBlendingCalculated)
			{
				return;
			}

			percentage = Calculations.ClampToNormal(percentage);
			previousPercentage = Calculations.ClampToNormal(previousPercentage);

			InitBoundaries(previousPercentage, percentage,
				out float previousLinearBoundaryX, out float linearBoundaryX, out float previousReversedLinearBoundaryX, out float reversedLinearBoundaryX,
				out float previousLinearBoundaryY, out float linearBoundaryY, out float previousReversedLinearBoundaryY, out float reversedLinearBoundaryY,
				out float previousRightHalfLinearBoundaryX, out float rightHalfLinearBoundaryX, out float previousLeftHalfLinearBoundaryX, out float leftHalfLinearBoundaryX,
				out float previousLowerHalfLinearBoundaryY, out float lowerHalfLinearBoundaryY, out float previousHigherHalfLinearBoundaryY, out float higherHalfLinearBoundaryY,
				out float previusRadiusBoundary, out float radiusBoundary);

			int[] nums = Enumerable.Range(0, ByteArray.PrimitiveArray.Length / Channels).Select(i => i * Channels).ToArray();
			Parallel.ForEach(nums, i =>
			{
				Point coordinates = ByteArray.GetCoordinates(i);

				bool isValid = ValidateBoundaries(coordinates, transitionMode,
					previousLinearBoundaryX, linearBoundaryX, previousReversedLinearBoundaryX, reversedLinearBoundaryX,
					previousLinearBoundaryY, linearBoundaryY, previousReversedLinearBoundaryY, reversedLinearBoundaryY,
					previousLeftHalfLinearBoundaryX, leftHalfLinearBoundaryX, previousRightHalfLinearBoundaryX, rightHalfLinearBoundaryX,
					previousHigherHalfLinearBoundaryY, higherHalfLinearBoundaryY, previousLowerHalfLinearBoundaryY, lowerHalfLinearBoundaryY,
					previusRadiusBoundary, radiusBoundary);

				if (!isValid) return;

				for (int channel = 0; channel < Channels; channel++)
				{
					ByteArray.PrimitiveArray[i + channel] = Calculations.ClampToByte(Blending.Multiply(ByteArray.PrimitiveArray[i + channel], myImage2.ByteArray.PrimitiveArray[i + channel]));
				}
			});

			WriteableBitmap writeableBitmap = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr32, null, ByteArray.PrimitiveArray, ByteArray.Stride));
			WriteableBitmap = writeableBitmap;
		}

		public void Lighten(MyImage myImage2, TransitionModes transitionMode = TransitionModes.NoTransition, float previousPercentage = 0f, float percentage = 1f)
		{
			if (IsBlendingCalculated)
			{
				return;
			}

			percentage = Calculations.ClampToNormal(percentage);
			previousPercentage = Calculations.ClampToNormal(previousPercentage);

			InitBoundaries(previousPercentage, percentage,
				out float previousLinearBoundaryX, out float linearBoundaryX, out float previousReversedLinearBoundaryX, out float reversedLinearBoundaryX,
				out float previousLinearBoundaryY, out float linearBoundaryY, out float previousReversedLinearBoundaryY, out float reversedLinearBoundaryY,
				out float previousRightHalfLinearBoundaryX, out float rightHalfLinearBoundaryX, out float previousLeftHalfLinearBoundaryX, out float leftHalfLinearBoundaryX,
				out float previousLowerHalfLinearBoundaryY, out float lowerHalfLinearBoundaryY, out float previousHigherHalfLinearBoundaryY, out float higherHalfLinearBoundaryY,
				out float previusRadiusBoundary, out float radiusBoundary);

			int[] nums = Enumerable.Range(0, ByteArray.PrimitiveArray.Length / Channels).Select(i => i * Channels).ToArray();
			Parallel.ForEach(nums, i =>
			{
				Point coordinates = ByteArray.GetCoordinates(i);

				bool isValid = ValidateBoundaries(coordinates, transitionMode,
					previousLinearBoundaryX, linearBoundaryX, previousReversedLinearBoundaryX, reversedLinearBoundaryX,
					previousLinearBoundaryY, linearBoundaryY, previousReversedLinearBoundaryY, reversedLinearBoundaryY,
					previousLeftHalfLinearBoundaryX, leftHalfLinearBoundaryX, previousRightHalfLinearBoundaryX, rightHalfLinearBoundaryX,
					previousHigherHalfLinearBoundaryY, higherHalfLinearBoundaryY, previousLowerHalfLinearBoundaryY, lowerHalfLinearBoundaryY,
					previusRadiusBoundary, radiusBoundary);

				if (!isValid) return;

				for (int channel = 0; channel < Channels; channel++)
				{
					ByteArray.PrimitiveArray[i + channel] = Calculations.ClampToByte(Blending.Lighten(ByteArray.PrimitiveArray[i + channel], myImage2.ByteArray.PrimitiveArray[i + channel]));
				}
			});

			WriteableBitmap writeableBitmap = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr32, null, ByteArray.PrimitiveArray, ByteArray.Stride));
			WriteableBitmap = writeableBitmap;
		}

		public void Screen(MyImage myImage2, TransitionModes transitionMode = TransitionModes.NoTransition, float previousPercentage = 0f, float percentage = 1f)
		{
			if (IsBlendingCalculated)
			{
				return;
			}

			percentage = Calculations.ClampToNormal(percentage);
			previousPercentage = Calculations.ClampToNormal(previousPercentage);

			InitBoundaries(previousPercentage, percentage,
				out float previousLinearBoundaryX, out float linearBoundaryX, out float previousReversedLinearBoundaryX, out float reversedLinearBoundaryX,
				out float previousLinearBoundaryY, out float linearBoundaryY, out float previousReversedLinearBoundaryY, out float reversedLinearBoundaryY,
				out float previousRightHalfLinearBoundaryX, out float rightHalfLinearBoundaryX, out float previousLeftHalfLinearBoundaryX, out float leftHalfLinearBoundaryX,
				out float previousLowerHalfLinearBoundaryY, out float lowerHalfLinearBoundaryY, out float previousHigherHalfLinearBoundaryY, out float higherHalfLinearBoundaryY,
				out float previusRadiusBoundary, out float radiusBoundary);

			int[] nums = Enumerable.Range(0, ByteArray.PrimitiveArray.Length / Channels).Select(i => i * Channels).ToArray();
			Parallel.ForEach(nums, i =>
			{
				Point coordinates = ByteArray.GetCoordinates(i);

				bool isValid = ValidateBoundaries(coordinates, transitionMode,
					previousLinearBoundaryX, linearBoundaryX, previousReversedLinearBoundaryX, reversedLinearBoundaryX,
					previousLinearBoundaryY, linearBoundaryY, previousReversedLinearBoundaryY, reversedLinearBoundaryY,
					previousLeftHalfLinearBoundaryX, leftHalfLinearBoundaryX, previousRightHalfLinearBoundaryX, rightHalfLinearBoundaryX,
					previousHigherHalfLinearBoundaryY, higherHalfLinearBoundaryY, previousLowerHalfLinearBoundaryY, lowerHalfLinearBoundaryY,
					previusRadiusBoundary, radiusBoundary);

				if (!isValid) return;

				for (int channel = 0; channel < Channels; channel++)
				{
					ByteArray.PrimitiveArray[i + channel] = Calculations.ClampToByte(Blending.Screen(ByteArray.PrimitiveArray[i + channel], myImage2.ByteArray.PrimitiveArray[i + channel]));
				}
			});

			WriteableBitmap writeableBitmap = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr32, null, ByteArray.PrimitiveArray, ByteArray.Stride));
			WriteableBitmap = writeableBitmap;
		}

		public void ColorBurn(MyImage myImage2, TransitionModes transitionMode = TransitionModes.NoTransition, float previousPercentage = 0f, float percentage = 1f)
		{
			if (IsBlendingCalculated)
			{
				return;
			}

			percentage = Calculations.ClampToNormal(percentage);
			previousPercentage = Calculations.ClampToNormal(previousPercentage);

			InitBoundaries(previousPercentage, percentage,
				out float previousLinearBoundaryX, out float linearBoundaryX, out float previousReversedLinearBoundaryX, out float reversedLinearBoundaryX,
				out float previousLinearBoundaryY, out float linearBoundaryY, out float previousReversedLinearBoundaryY, out float reversedLinearBoundaryY,
				out float previousRightHalfLinearBoundaryX, out float rightHalfLinearBoundaryX, out float previousLeftHalfLinearBoundaryX, out float leftHalfLinearBoundaryX,
				out float previousLowerHalfLinearBoundaryY, out float lowerHalfLinearBoundaryY, out float previousHigherHalfLinearBoundaryY, out float higherHalfLinearBoundaryY,
				out float previusRadiusBoundary, out float radiusBoundary);

			int[] nums = Enumerable.Range(0, ByteArray.PrimitiveArray.Length / Channels).Select(i => i * Channels).ToArray();
			Parallel.ForEach(nums, i =>
			{
				Point coordinates = ByteArray.GetCoordinates(i);

				bool isValid = ValidateBoundaries(coordinates, transitionMode,
					previousLinearBoundaryX, linearBoundaryX, previousReversedLinearBoundaryX, reversedLinearBoundaryX,
					previousLinearBoundaryY, linearBoundaryY, previousReversedLinearBoundaryY, reversedLinearBoundaryY,
					previousLeftHalfLinearBoundaryX, leftHalfLinearBoundaryX, previousRightHalfLinearBoundaryX, rightHalfLinearBoundaryX,
					previousHigherHalfLinearBoundaryY, higherHalfLinearBoundaryY, previousLowerHalfLinearBoundaryY, lowerHalfLinearBoundaryY,
					previusRadiusBoundary, radiusBoundary);

				if (!isValid) return;

				for (int channel = 0; channel < Channels; channel++)
				{
					ByteArray.PrimitiveArray[i + channel] = Calculations.ClampToByte(Blending.ColorBurn(ByteArray.PrimitiveArray[i + channel], myImage2.ByteArray.PrimitiveArray[i + channel]));
				}
			});

			WriteableBitmap writeableBitmap = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr32, null, ByteArray.PrimitiveArray, ByteArray.Stride));
			WriteableBitmap = writeableBitmap;
		}

		public void LinearBurn(MyImage myImage2, TransitionModes transitionMode = TransitionModes.NoTransition, float previousPercentage = 0f, float percentage = 1f)
		{
			if (IsBlendingCalculated)
			{
				return;
			}

			percentage = Calculations.ClampToNormal(percentage);
			previousPercentage = Calculations.ClampToNormal(previousPercentage);

			InitBoundaries(previousPercentage, percentage,
				out float previousLinearBoundaryX, out float linearBoundaryX, out float previousReversedLinearBoundaryX, out float reversedLinearBoundaryX,
				out float previousLinearBoundaryY, out float linearBoundaryY, out float previousReversedLinearBoundaryY, out float reversedLinearBoundaryY,
				out float previousRightHalfLinearBoundaryX, out float rightHalfLinearBoundaryX, out float previousLeftHalfLinearBoundaryX, out float leftHalfLinearBoundaryX,
				out float previousLowerHalfLinearBoundaryY, out float lowerHalfLinearBoundaryY, out float previousHigherHalfLinearBoundaryY, out float higherHalfLinearBoundaryY,
				out float previusRadiusBoundary, out float radiusBoundary);

			int[] nums = Enumerable.Range(0, ByteArray.PrimitiveArray.Length / Channels).Select(i => i * Channels).ToArray();
			Parallel.ForEach(nums, i =>
			{
				Point coordinates = ByteArray.GetCoordinates(i);

				bool isValid = ValidateBoundaries(coordinates, transitionMode,
					previousLinearBoundaryX, linearBoundaryX, previousReversedLinearBoundaryX, reversedLinearBoundaryX,
					previousLinearBoundaryY, linearBoundaryY, previousReversedLinearBoundaryY, reversedLinearBoundaryY,
					previousLeftHalfLinearBoundaryX, leftHalfLinearBoundaryX, previousRightHalfLinearBoundaryX, rightHalfLinearBoundaryX,
					previousHigherHalfLinearBoundaryY, higherHalfLinearBoundaryY, previousLowerHalfLinearBoundaryY, lowerHalfLinearBoundaryY,
					previusRadiusBoundary, radiusBoundary);

				if (!isValid) return;

				for (int channel = 0; channel < Channels; channel++)
				{
					ByteArray.PrimitiveArray[i + channel] = Calculations.ClampToByte(Blending.LinearBurn(ByteArray.PrimitiveArray[i + channel], myImage2.ByteArray.PrimitiveArray[i + channel]));
				}
			});

			WriteableBitmap writeableBitmap = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr32, null, ByteArray.PrimitiveArray, ByteArray.Stride));
			WriteableBitmap = writeableBitmap;
		}

		public void ColorDodge(MyImage myImage2, TransitionModes transitionMode = TransitionModes.NoTransition, float previousPercentage = 0f, float percentage = 1f)
		{
			if (IsBlendingCalculated)
			{
				return;
			}

			percentage = Calculations.ClampToNormal(percentage);
			previousPercentage = Calculations.ClampToNormal(previousPercentage);

			InitBoundaries(previousPercentage, percentage,
				out float previousLinearBoundaryX, out float linearBoundaryX, out float previousReversedLinearBoundaryX, out float reversedLinearBoundaryX,
				out float previousLinearBoundaryY, out float linearBoundaryY, out float previousReversedLinearBoundaryY, out float reversedLinearBoundaryY,
				out float previousRightHalfLinearBoundaryX, out float rightHalfLinearBoundaryX, out float previousLeftHalfLinearBoundaryX, out float leftHalfLinearBoundaryX,
				out float previousLowerHalfLinearBoundaryY, out float lowerHalfLinearBoundaryY, out float previousHigherHalfLinearBoundaryY, out float higherHalfLinearBoundaryY,
				out float previusRadiusBoundary, out float radiusBoundary);

			int[] nums = Enumerable.Range(0, ByteArray.PrimitiveArray.Length / Channels).Select(i => i * Channels).ToArray();
			Parallel.ForEach(nums, i =>
			{
				Point coordinates = ByteArray.GetCoordinates(i);

				bool isValid = ValidateBoundaries(coordinates, transitionMode,
					previousLinearBoundaryX, linearBoundaryX, previousReversedLinearBoundaryX, reversedLinearBoundaryX,
					previousLinearBoundaryY, linearBoundaryY, previousReversedLinearBoundaryY, reversedLinearBoundaryY,
					previousLeftHalfLinearBoundaryX, leftHalfLinearBoundaryX, previousRightHalfLinearBoundaryX, rightHalfLinearBoundaryX,
					previousHigherHalfLinearBoundaryY, higherHalfLinearBoundaryY, previousLowerHalfLinearBoundaryY, lowerHalfLinearBoundaryY,
					previusRadiusBoundary, radiusBoundary);

				if (!isValid) return;

				for (int channel = 0; channel < Channels; channel++)
				{
					ByteArray.PrimitiveArray[i + channel] = Calculations.ClampToByte(Blending.ColorDodge(ByteArray.PrimitiveArray[i + channel], myImage2.ByteArray.PrimitiveArray[i + channel]));
				}
			});

			WriteableBitmap writeableBitmap = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr32, null, ByteArray.PrimitiveArray, ByteArray.Stride));
			WriteableBitmap = writeableBitmap;
		}

		public void LinearDodge(MyImage myImage2, TransitionModes transitionMode = TransitionModes.NoTransition, float previousPercentage = 0f, float percentage = 1f)
		{
			if (IsBlendingCalculated)
			{
				return;
			}

			percentage = Calculations.ClampToNormal(percentage);
			previousPercentage = Calculations.ClampToNormal(previousPercentage);

			InitBoundaries(previousPercentage, percentage,
				out float previousLinearBoundaryX, out float linearBoundaryX, out float previousReversedLinearBoundaryX, out float reversedLinearBoundaryX,
				out float previousLinearBoundaryY, out float linearBoundaryY, out float previousReversedLinearBoundaryY, out float reversedLinearBoundaryY,
				out float previousRightHalfLinearBoundaryX, out float rightHalfLinearBoundaryX, out float previousLeftHalfLinearBoundaryX, out float leftHalfLinearBoundaryX,
				out float previousLowerHalfLinearBoundaryY, out float lowerHalfLinearBoundaryY, out float previousHigherHalfLinearBoundaryY, out float higherHalfLinearBoundaryY,
				out float previusRadiusBoundary, out float radiusBoundary);

			int[] nums = Enumerable.Range(0, ByteArray.PrimitiveArray.Length / Channels).Select(i => i * Channels).ToArray();
			Parallel.ForEach(nums, i =>
			{
				Point coordinates = ByteArray.GetCoordinates(i);

				bool isValid = ValidateBoundaries(coordinates, transitionMode,
					previousLinearBoundaryX, linearBoundaryX, previousReversedLinearBoundaryX, reversedLinearBoundaryX,
					previousLinearBoundaryY, linearBoundaryY, previousReversedLinearBoundaryY, reversedLinearBoundaryY,
					previousLeftHalfLinearBoundaryX, leftHalfLinearBoundaryX, previousRightHalfLinearBoundaryX, rightHalfLinearBoundaryX,
					previousHigherHalfLinearBoundaryY, higherHalfLinearBoundaryY, previousLowerHalfLinearBoundaryY, lowerHalfLinearBoundaryY,
					previusRadiusBoundary, radiusBoundary);

				if (!isValid) return;

				for (int channel = 0; channel < Channels; channel++)
				{
					ByteArray.PrimitiveArray[i + channel] = Calculations.ClampToByte(Blending.LinearDodge(ByteArray.PrimitiveArray[i + channel], myImage2.ByteArray.PrimitiveArray[i + channel]));
				}
			});

			WriteableBitmap writeableBitmap = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr32, null, ByteArray.PrimitiveArray, ByteArray.Stride));
			WriteableBitmap = writeableBitmap;
		}

		public void Overlay(MyImage myImage2, TransitionModes transitionMode = TransitionModes.NoTransition, float previousPercentage = 0f, float percentage = 1f)
		{
			if (IsBlendingCalculated)
			{
				return;
			}

			percentage = Calculations.ClampToNormal(percentage);
			previousPercentage = Calculations.ClampToNormal(previousPercentage);

			InitBoundaries(previousPercentage, percentage,
				out float previousLinearBoundaryX, out float linearBoundaryX, out float previousReversedLinearBoundaryX, out float reversedLinearBoundaryX,
				out float previousLinearBoundaryY, out float linearBoundaryY, out float previousReversedLinearBoundaryY, out float reversedLinearBoundaryY,
				out float previousRightHalfLinearBoundaryX, out float rightHalfLinearBoundaryX, out float previousLeftHalfLinearBoundaryX, out float leftHalfLinearBoundaryX,
				out float previousLowerHalfLinearBoundaryY, out float lowerHalfLinearBoundaryY, out float previousHigherHalfLinearBoundaryY, out float higherHalfLinearBoundaryY,
				out float previusRadiusBoundary, out float radiusBoundary);

			int[] nums = Enumerable.Range(0, ByteArray.PrimitiveArray.Length / Channels).Select(i => i * Channels).ToArray();
			Parallel.ForEach(nums, i =>
			{
				Point coordinates = ByteArray.GetCoordinates(i);

				bool isValid = ValidateBoundaries(coordinates, transitionMode,
					previousLinearBoundaryX, linearBoundaryX, previousReversedLinearBoundaryX, reversedLinearBoundaryX,
					previousLinearBoundaryY, linearBoundaryY, previousReversedLinearBoundaryY, reversedLinearBoundaryY,
					previousLeftHalfLinearBoundaryX, leftHalfLinearBoundaryX, previousRightHalfLinearBoundaryX, rightHalfLinearBoundaryX,
					previousHigherHalfLinearBoundaryY, higherHalfLinearBoundaryY, previousLowerHalfLinearBoundaryY, lowerHalfLinearBoundaryY,
					previusRadiusBoundary, radiusBoundary);

				if (!isValid) return;

				for (int channel = 0; channel < Channels; channel++)
				{
					ByteArray.PrimitiveArray[i + channel] = Calculations.ClampToByte(Blending.Overlay(ByteArray.PrimitiveArray[i + channel], myImage2.ByteArray.PrimitiveArray[i + channel]));
				}
			});

			WriteableBitmap writeableBitmap = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr32, null, ByteArray.PrimitiveArray, ByteArray.Stride));
			WriteableBitmap = writeableBitmap;
		}

		public void SoftLight(MyImage myImage2, TransitionModes transitionMode = TransitionModes.NoTransition, float previousPercentage = 0f, float percentage = 1f)
		{
			if (IsBlendingCalculated)
			{
				return;
			}

			percentage = Calculations.ClampToNormal(percentage);
			previousPercentage = Calculations.ClampToNormal(previousPercentage);

			InitBoundaries(previousPercentage, percentage,
				out float previousLinearBoundaryX, out float linearBoundaryX, out float previousReversedLinearBoundaryX, out float reversedLinearBoundaryX,
				out float previousLinearBoundaryY, out float linearBoundaryY, out float previousReversedLinearBoundaryY, out float reversedLinearBoundaryY,
				out float previousRightHalfLinearBoundaryX, out float rightHalfLinearBoundaryX, out float previousLeftHalfLinearBoundaryX, out float leftHalfLinearBoundaryX,
				out float previousLowerHalfLinearBoundaryY, out float lowerHalfLinearBoundaryY, out float previousHigherHalfLinearBoundaryY, out float higherHalfLinearBoundaryY,
				out float previusRadiusBoundary, out float radiusBoundary);

			int[] nums = Enumerable.Range(0, ByteArray.PrimitiveArray.Length / Channels).Select(i => i * Channels).ToArray();
			Parallel.ForEach(nums, i =>
			{
				Point coordinates = ByteArray.GetCoordinates(i);

				bool isValid = ValidateBoundaries(coordinates, transitionMode,
					previousLinearBoundaryX, linearBoundaryX, previousReversedLinearBoundaryX, reversedLinearBoundaryX,
					previousLinearBoundaryY, linearBoundaryY, previousReversedLinearBoundaryY, reversedLinearBoundaryY,
					previousLeftHalfLinearBoundaryX, leftHalfLinearBoundaryX, previousRightHalfLinearBoundaryX, rightHalfLinearBoundaryX,
					previousHigherHalfLinearBoundaryY, higherHalfLinearBoundaryY, previousLowerHalfLinearBoundaryY, lowerHalfLinearBoundaryY,
					previusRadiusBoundary, radiusBoundary);

				if (!isValid) return;

				for (int channel = 0; channel < Channels; channel++)
				{
					ByteArray.PrimitiveArray[i + channel] = Calculations.ClampToByte(Blending.SoftLight(ByteArray.PrimitiveArray[i + channel], myImage2.ByteArray.PrimitiveArray[i + channel]));
				}
			});

			WriteableBitmap writeableBitmap = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr32, null, ByteArray.PrimitiveArray, ByteArray.Stride));
			WriteableBitmap = writeableBitmap;
		}

		public void HardLight(MyImage myImage2, TransitionModes transitionMode = TransitionModes.NoTransition, float previousPercentage = 0f, float percentage = 1f)
		{
			if (IsBlendingCalculated)
			{
				return;
			}

			percentage = Calculations.ClampToNormal(percentage);
			previousPercentage = Calculations.ClampToNormal(previousPercentage);

			InitBoundaries(previousPercentage, percentage,
				out float previousLinearBoundaryX, out float linearBoundaryX, out float previousReversedLinearBoundaryX, out float reversedLinearBoundaryX,
				out float previousLinearBoundaryY, out float linearBoundaryY, out float previousReversedLinearBoundaryY, out float reversedLinearBoundaryY,
				out float previousRightHalfLinearBoundaryX, out float rightHalfLinearBoundaryX, out float previousLeftHalfLinearBoundaryX, out float leftHalfLinearBoundaryX,
				out float previousLowerHalfLinearBoundaryY, out float lowerHalfLinearBoundaryY, out float previousHigherHalfLinearBoundaryY, out float higherHalfLinearBoundaryY,
				out float previusRadiusBoundary, out float radiusBoundary);

			int[] nums = Enumerable.Range(0, ByteArray.PrimitiveArray.Length / Channels).Select(i => i * Channels).ToArray();
			Parallel.ForEach(nums, i =>
			{
				Point coordinates = ByteArray.GetCoordinates(i);

				bool isValid = ValidateBoundaries(coordinates, transitionMode,
					previousLinearBoundaryX, linearBoundaryX, previousReversedLinearBoundaryX, reversedLinearBoundaryX,
					previousLinearBoundaryY, linearBoundaryY, previousReversedLinearBoundaryY, reversedLinearBoundaryY,
					previousLeftHalfLinearBoundaryX, leftHalfLinearBoundaryX, previousRightHalfLinearBoundaryX, rightHalfLinearBoundaryX,
					previousHigherHalfLinearBoundaryY, higherHalfLinearBoundaryY, previousLowerHalfLinearBoundaryY, lowerHalfLinearBoundaryY,
					previusRadiusBoundary, radiusBoundary);

				if (!isValid) return; ;

				for (int channel = 0; channel < Channels; channel++)
				{
					ByteArray.PrimitiveArray[i + channel] = Calculations.ClampToByte(Blending.HardLight(ByteArray.PrimitiveArray[i + channel], myImage2.ByteArray.PrimitiveArray[i + channel]));
				}
			});

			WriteableBitmap writeableBitmap = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr32, null, ByteArray.PrimitiveArray, ByteArray.Stride));
			WriteableBitmap = writeableBitmap;
		}

		public void Difference(MyImage myImage2, TransitionModes transitionMode = TransitionModes.NoTransition, float previousPercentage = 0f, float percentage = 1f)
		{
			if (IsBlendingCalculated)
			{
				return;
			}

			percentage = Calculations.ClampToNormal(percentage);
			previousPercentage = Calculations.ClampToNormal(previousPercentage);

			InitBoundaries(previousPercentage, percentage,
				out float previousLinearBoundaryX, out float linearBoundaryX, out float previousReversedLinearBoundaryX, out float reversedLinearBoundaryX,
				out float previousLinearBoundaryY, out float linearBoundaryY, out float previousReversedLinearBoundaryY, out float reversedLinearBoundaryY,
				out float previousRightHalfLinearBoundaryX, out float rightHalfLinearBoundaryX, out float previousLeftHalfLinearBoundaryX, out float leftHalfLinearBoundaryX,
				out float previousLowerHalfLinearBoundaryY, out float lowerHalfLinearBoundaryY, out float previousHigherHalfLinearBoundaryY, out float higherHalfLinearBoundaryY,
				out float previusRadiusBoundary, out float radiusBoundary);

			int[] nums = Enumerable.Range(0, ByteArray.PrimitiveArray.Length / Channels).Select(i => i * Channels).ToArray();
			Parallel.ForEach(nums, i =>
			{
				Point coordinates = ByteArray.GetCoordinates(i);

				bool isValid = ValidateBoundaries(coordinates, transitionMode,
					previousLinearBoundaryX, linearBoundaryX, previousReversedLinearBoundaryX, reversedLinearBoundaryX,
					previousLinearBoundaryY, linearBoundaryY, previousReversedLinearBoundaryY, reversedLinearBoundaryY,
					previousLeftHalfLinearBoundaryX, leftHalfLinearBoundaryX, previousRightHalfLinearBoundaryX, rightHalfLinearBoundaryX,
					previousHigherHalfLinearBoundaryY, higherHalfLinearBoundaryY, previousLowerHalfLinearBoundaryY, lowerHalfLinearBoundaryY,
					previusRadiusBoundary, radiusBoundary);

				if (!isValid) return;

				for (int channel = 0; channel < Channels; channel++)
				{
					ByteArray.PrimitiveArray[i + channel] = Calculations.ClampToByte(Blending.Difference(ByteArray.PrimitiveArray[i + channel], myImage2.ByteArray.PrimitiveArray[i + channel]));
				}
			});

			WriteableBitmap writeableBitmap = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr32, null, ByteArray.PrimitiveArray, ByteArray.Stride));
			WriteableBitmap = writeableBitmap;
		}

		public WriteableBitmap Fade(MyImage myImage1, float percentage = 1f)
		{
			percentage = Calculations.ClampToNormal(percentage);

			ByteArray tempByteArray = ByteArray.Clone();

			int[] nums = Enumerable.Range(0, ByteArray.PrimitiveArray.Length / Channels).Select(i => i * Channels).ToArray();
			Parallel.ForEach(nums, i =>
			{
				for (int channel = 0; channel < Channels; channel++)
				{
					tempByteArray.PrimitiveArray[i + channel] = Blending.Opacity(
						tempByteArray.PrimitiveArray[i + channel],
						myImage1.ByteArray.PrimitiveArray[i + channel],
						percentage);
				}
			});

			WriteableBitmap writeableBitmap = new WriteableBitmap(BitmapSource.Create(Width, Height, 96, 96, PixelFormats.Bgr32, null, tempByteArray.PrimitiveArray, tempByteArray.Stride));
			writeableBitmap.Freeze();
			return writeableBitmap;
		}

		private void InitBoundaries(float previousPercentage, float percentage,
									out float previousLinearBoundaryX,			out float linearBoundaryX,			out float previousReversedLinearBoundaryX,		out float reversedLinearBoundaryX,
									out float previousLinearBoundaryY,			out float linearBoundaryY,			out float previousReversedLinearBoundaryY,		out float reversedLinearBoundaryY,
									out float previousRightHalfLinearBoundaryX, out float rightHalfLinearBoundaryX, out float previousLeftHalfLinearBoundaryX,		out float leftHalfLinearBoundaryX,
									out float previousLowerHalfLinearBoundaryY, out float lowerHalfLinearBoundaryY, out float previousHigherHalfLinearBoundaryY,	out float higherHalfLinearBoundaryY,
									out float previusRadiusBoundary,			out float radiusBoundary)
		{
			previousLinearBoundaryX = Width * previousPercentage;
			linearBoundaryX = Width * percentage;

			previousReversedLinearBoundaryX = Width * (1f - previousPercentage);
			reversedLinearBoundaryX = Width * (1f - percentage);

			previousLinearBoundaryY = Height * previousPercentage;
			linearBoundaryY = Height * percentage;

			previousReversedLinearBoundaryY = Height * (1f - previousPercentage);
			reversedLinearBoundaryY = Height * (1f - percentage);

			float widthHalf = Convert.ToSingle(Width / 2);
			float heightHalf = Convert.ToSingle(Height / 2);

			previousLeftHalfLinearBoundaryX = widthHalf - widthHalf * previousPercentage;
			leftHalfLinearBoundaryX = widthHalf - widthHalf * percentage;

			previousRightHalfLinearBoundaryX = widthHalf + widthHalf * previousPercentage;
			rightHalfLinearBoundaryX = widthHalf + widthHalf * percentage;

			previousHigherHalfLinearBoundaryY = heightHalf - heightHalf * previousPercentage;
			higherHalfLinearBoundaryY = heightHalf - heightHalf * percentage;

			previousLowerHalfLinearBoundaryY = heightHalf + heightHalf * previousPercentage;
			lowerHalfLinearBoundaryY = heightHalf + heightHalf * percentage;

			double maxRadius = Math.Sqrt(Math.Pow(widthHalf, 2) + Math.Pow(heightHalf, 2));

			previusRadiusBoundary = Convert.ToSingle(previousPercentage * maxRadius);
			radiusBoundary = Convert.ToSingle(percentage * maxRadius);
		}

		private bool ValidateBoundaries(Point coordinates, TransitionModes transitionMode,
			float previousLinearBoundaryX, float linearBoundaryX, float previousReversedLinearBoundaryX, float reversedLinearBoundaryX,
			float previousLinearBoundaryY, float linearBoundaryY, float previousReversedLinearBoundaryY, float reversedLinearBoundaryY,
			float previousLeftHalfLinearBoundaryX, float leftHalfLinearBoundaryX, float previousRightHalfLinearBoundaryX, float rightHalfLinearBoundaryX,
			float previousHigherHalfLinearBoundaryY, float higherHalfLinearBoundaryY, float previousLowerHalfLinearBoundaryY, float lowerHalfLinearBoundaryY,
			float previusRadiusBoundary, float radiusBoundary)
		{
			bool isValid = true;

			switch (transitionMode)
			{
				case TransitionModes.NoTransition:
					break;
				case TransitionModes.WipeLeftToRight:
					if (coordinates.X < previousLinearBoundaryX || coordinates.X >= linearBoundaryX) isValid = false;
					break;
				case TransitionModes.WipeRightToLeft:
					if (coordinates.X >= previousReversedLinearBoundaryX || coordinates.X < reversedLinearBoundaryX) isValid = false;
					break;
				case TransitionModes.WipeTopToBottom:
					if (coordinates.Y < previousLinearBoundaryY || coordinates.Y >= linearBoundaryY) isValid = false;
					break;
				case TransitionModes.WipeBottomToTop:
					if (coordinates.Y >= previousReversedLinearBoundaryY || coordinates.Y < reversedLinearBoundaryY) isValid = false;
					break;
				case TransitionModes.HorizontalSplit:
					if(coordinates.Y < higherHalfLinearBoundaryY
						|| (previousLowerHalfLinearBoundaryY != previousHigherHalfLinearBoundaryY
						&& coordinates.Y >= previousHigherHalfLinearBoundaryY
						&& coordinates.Y <= previousLowerHalfLinearBoundaryY)
						|| coordinates.Y > lowerHalfLinearBoundaryY) isValid = false;
					break;
				case TransitionModes.VerticalSplit:
					if (coordinates.X < leftHalfLinearBoundaryX
						|| (previousLeftHalfLinearBoundaryX != previousRightHalfLinearBoundaryX
						&& coordinates.X >= previousLeftHalfLinearBoundaryX
						&& coordinates.X <= previousRightHalfLinearBoundaryX)
						|| coordinates.X > rightHalfLinearBoundaryX) isValid = false;
					break;
				case TransitionModes.Fade:
					IsBlendingCalculated = true;
					break;
				case TransitionModes.Rectangle:
					if (coordinates.Y < higherHalfLinearBoundaryY
						|| coordinates.Y >= lowerHalfLinearBoundaryY
						|| coordinates.X < leftHalfLinearBoundaryX
						|| coordinates.X >= rightHalfLinearBoundaryX
						|| ((previousLowerHalfLinearBoundaryY != previousHigherHalfLinearBoundaryY
						|| previousLeftHalfLinearBoundaryX != previousRightHalfLinearBoundaryX)
						&& coordinates.Y >= previousHigherHalfLinearBoundaryY
						&& coordinates.Y < previousLowerHalfLinearBoundaryY
						&& coordinates.X >= previousLeftHalfLinearBoundaryX
						&& coordinates.X < previousRightHalfLinearBoundaryX
						)) isValid = false;
					break;
				case TransitionModes.Circle:
					double radius = Math.Sqrt(Math.Pow(coordinates.X - Convert.ToDouble(Width / 2), 2) + Math.Pow(coordinates.Y - Convert.ToDouble(Height / 2), 2));
					if (radius < previusRadiusBoundary || radius >= radiusBoundary) isValid = false;
					break;
			}

			return isValid;
		}
	}
}