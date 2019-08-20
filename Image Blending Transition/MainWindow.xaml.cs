using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace ImageBlendingTransition
{
	public partial class MainWindow : Window
	{
		private MyImage MyImage1 { get; set; }
		private MyImage MyImage2 { get; set; }
		private MyImage MyImageResult { get; set; }
		private DispatcherTimer TransitionTimer { get; set; } = new DispatcherTimer();

		public MainWindow()
		{
			InitializeComponent();

			TransitionTimer.Tick += TransitionTimerTickAsync;
			TransitionTimer.Interval = TimeSpan.FromSeconds(0.00000025d);
		}

		private void EnableUI()
		{
			EnableSourceUI();
			EnableSaveButton();
			EnablePerformButton();
			EnableModeChoice();
		}

		private void DisableUI()
		{
			DisableSourceUI();
			DisableSaveButton();
			DisablePerformButton();
			DisableModeChoice();
		}

		private void EnableSourceUI()
		{
			loadSourceImage1_Button.IsEnabled = true;
			loadSourceImage2_Button.IsEnabled = true;
		}

		private void DisableSourceUI()
		{
			loadSourceImage1_Button.IsEnabled = false;
			loadSourceImage2_Button.IsEnabled = false;
		}



		private void EnableSaveButton()
		{
			saveResultImage_Button.IsEnabled = true;
		}

		private void DisableSaveButton()
		{
			saveResultImage_Button.IsEnabled = false;
		}

		private void EnablePerformButton()
		{
			Perform_Button.IsEnabled = true;
		}

		private void DisablePerformButton()
		{
			Perform_Button.IsEnabled = false;
		}

		private void EnableModeChoice()
		{
			blendingMode_ComboBox.IsEnabled = true;
			transitionMode_ComboBox.IsEnabled = true;
		}

		private void DisableModeChoice()
		{
			blendingMode_ComboBox.IsEnabled = false;
			transitionMode_ComboBox.IsEnabled = false;
		}

		private void DisposeSecondSourceAndResult()
		{
			sourceImage2.Source = null;
			MyImage2 = null;

			resultImage.Source = null;
			MyImageResult = null;
		}

		private void LoadFirstSourceImage(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

			if (openFileDialog.ShowDialog() == true)
			{
				if (openFileDialog.FileName.Length > 0)
				{
					DisableUI();

					WriteableBitmap writeableBitmap = BitmapFactory.ConvertToPbgra32Format(BitmapFactory.FromStream(new MemoryStream(File.ReadAllBytes(openFileDialog.FileName))));

					sourceImage1.Source = writeableBitmap;
					MyImage1 = new MyImage(writeableBitmap);

					DisposeSecondSourceAndResult();

					EnableSourceUI();
				}
			}
		}

		private void LoadSecondSourceImage(object sender, RoutedEventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
			openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

			if (openFileDialog.ShowDialog() == true)
			{
				if (openFileDialog.FileName.Length > 0)
				{
					DisableSourceUI();

					WriteableBitmap writeableBitmap = BitmapFactory.ConvertToPbgra32Format(BitmapFactory.FromStream(new MemoryStream(File.ReadAllBytes(openFileDialog.FileName))));

					if (writeableBitmap.PixelWidth != MyImage1.Width || writeableBitmap.PixelHeight != MyImage1.Height)
					{
						MessageBox.Show("Second Image Must have the Same Size as First Image!");
						EnableSourceUI();
						return;
					}

					sourceImage2.Source = writeableBitmap;
					MyImage2 = new MyImage(writeableBitmap);

					EnableSourceUI();
					EnablePerformButton();
					EnableModeChoice();
				}
			}
		}

		private void SaveResultImage(object sender, RoutedEventArgs e)
		{
			if (IsInitialized)
			{
				SaveFileDialog saveFileDialog = new SaveFileDialog();
				saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
				saveFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

				if (saveFileDialog.ShowDialog() == true)
				{
					if (saveFileDialog.FileName.Length > 0)
					{
						using (FileStream stream = new FileStream(saveFileDialog.FileName, FileMode.Create))
						{
							PngBitmapEncoder encoder = new PngBitmapEncoder();
							encoder.Frames.Add(BitmapFrame.Create(MyImageResult.WriteableBitmap));
							encoder.Save(stream);
						}
					}
				}
			}
		}

		private void Perform(object sender, RoutedEventArgs e)
		{
			if (IsInitialized)
			{
				DisableUI();
				MyImageResult = MyImage1.Clone();

				TransitionModes transitionMode = (TransitionModes)transitionMode_ComboBox.SelectedIndex;

				if (transitionMode != TransitionModes.NoTransition)
				{
					resultImage.Source = MyImageResult.WriteableBitmap;
				}
				TransitionTimer.Start();
			}
		}

		private void Perform(BlendingModes blendingMode, TransitionModes transitionMode, float opacity, float previousPercentage = 0f, float percentage = 1f)
		{
			switch (blendingMode)
			{
				case BlendingModes.Opacity:
					MyImageResult.Opacity(MyImage2, opacity, transitionMode, previousPercentage, percentage);
					break;
				case BlendingModes.Darken:
					MyImageResult.Darken(MyImage2, transitionMode, previousPercentage, percentage);
					break;
				case BlendingModes.Multiply:
					MyImageResult.Multiply(MyImage2, transitionMode, previousPercentage, percentage);
					break;
				case BlendingModes.Lighten:
					MyImageResult.Lighten(MyImage2, transitionMode, previousPercentage, percentage);
					break;
				case BlendingModes.Screen:
					MyImageResult.Screen(MyImage2, transitionMode, previousPercentage, percentage);
					break;
				case BlendingModes.ColorBurn:
					MyImageResult.ColorBurn(MyImage2, transitionMode, previousPercentage, percentage);
					break;
				case BlendingModes.LinearBurn:
					MyImageResult.LinearBurn(MyImage2, transitionMode, previousPercentage, percentage);
					break;
				case BlendingModes.ColorDodge:
					MyImageResult.ColorDodge(MyImage2, transitionMode, previousPercentage, percentage);
					break;
				case BlendingModes.LinearDodge:
					MyImageResult.LinearDodge(MyImage2, transitionMode, previousPercentage, percentage);
					break;
				case BlendingModes.Overlay:
					MyImageResult.Overlay(MyImage2, transitionMode, previousPercentage, percentage);
					break;
				case BlendingModes.SoftLight:
					MyImageResult.SoftLight(MyImage2, transitionMode, previousPercentage, percentage);
					break;
				case BlendingModes.HardLight:
					MyImageResult.HardLight(MyImage2, transitionMode, previousPercentage, percentage);
					break;
				case BlendingModes.Difference:
					MyImageResult.Difference(MyImage2, transitionMode, previousPercentage, percentage);
					break;
			}

			WriteableBitmap writeableBitmap;

			if (transitionMode == TransitionModes.Fade)
			{
				writeableBitmap = MyImageResult.Fade(MyImage1, percentage);
			}
			else
			{
				writeableBitmap = MyImageResult.WriteableBitmap.Clone();
				writeableBitmap.Freeze();
			}

			Dispatcher.Invoke(() =>
			{
				resultImage.Source = writeableBitmap;
			});

		}

		private async void TransitionTimerTickAsync(object sender, EventArgs e)
		{
			TransitionTimer.Stop();

			if (progressBar.Value >= 1f)
			{
				progressBar.Value = 0d;
				EnableUI();
				return;
			}

			BlendingModes blendingMode = (BlendingModes)blendingMode_ComboBox.SelectedIndex;
			TransitionModes transitionMode = (TransitionModes)transitionMode_ComboBox.SelectedIndex;
			float opacity = Convert.ToSingle(opacity_Slider.Value);

			if (transitionMode == TransitionModes.NoTransition)
			{
				await Task.Run(() => Perform(blendingMode, transitionMode, opacity));
				progressBar.Value = 0d;
				EnableUI();
				return;
			}

			float previousPercentage = Convert.ToSingle(progressBar.Value);
			float percentage = Convert.ToSingle(progressBar.Value += 0.025d);

			await Task.Run(() => Perform(blendingMode, transitionMode, opacity, previousPercentage, percentage));
			TransitionTimer.Start();
		}

		private void OpacityChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (IsInitialized) {
				opacity_TextBox.Text = opacity_Slider.Value.ToString("0.000");
			}
		}

		private void BlendingModeChanged(object sender, SelectionChangedEventArgs e)
		{
			if (IsInitialized)
			{
				BlendingModes blendingMode = (BlendingModes)blendingMode_ComboBox.SelectedIndex;

				if (blendingMode == BlendingModes.Opacity)
				{
					opacity_Slider.IsEnabled = true;
				}
				else
				{
					opacity_Slider.IsEnabled = false;
				}
			}
		}
	}
}
