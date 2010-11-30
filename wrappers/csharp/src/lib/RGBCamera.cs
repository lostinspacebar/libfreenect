/*
 * This file is part of the OpenKinect Project. http://www.openkinect.org
 *
 * Copyright (c) 2010 individual OpenKinect contributors. See the CONTRIB file
 * for details.
 *
 * This code is licensed to you under the terms of the Apache License, version
 * 2.0, or, at your option, the terms of the GNU General Public License,
 * version 2.0. See the APACHE20 and GPL2 files for the text of the licenses,
 * or the following URLs:
 * http://www.apache.org/licenses/LICENSE-2.0
 * http://www.gnu.org/licenses/gpl-2.0.txt
 *
 * If you redistribute this file in source form, modified or unmodified, you
 * may:
 *   1) Leave this header intact and distribute it under the same terms,
 *      accompanying it with the APACHE20 and GPL20 files, or
 *   2) Delete the Apache 2.0 clause and accompany it with the GPL2 file, or
 *   3) Delete the GPL v2 clause and accompany it with the APACHE20 file
 * In all cases you must keep the copyright notice intact and include a copy
 * of the CONTRIB file.
 *
 * Binary distributions must follow the binary distribution requirements of
 * either License.
 */

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace LibFreenect
{
	/// <summary>
	/// Provides access to the RGB camera on the Kinect
	/// </summary>
	/// <author>Aditya Gaddam (adityagaddam@gmail.com)</author>
	/// 
	public class RGBCamera
	{
		/// <summary>
		/// RGB format size
		/// </summary>
		internal static int rgbFormatSize = 640 * 480 * 3;
		
		/// <summary>
		/// Parent Kinect instance
		/// </summary>
		private Kinect parentDevice;
		
		/// <summary>
		/// Current data format
		/// </summary>
		private DataFormatOptions dataFormat;
		
		/// <summary>
		/// Gets whether the RGB camera is streaming data
		/// </summary>
		public bool IsRunning
		{
			get;
			private set;
		}
		
		/// <summary>
		/// Event raised when RGB data (an image) has been received.
		/// </summary>
		public event DataReceivedEventHandler DataReceived = delegate { };
		
		/// <summary>
		/// Gets or sets the data format this camera will send images in.
		/// </summary>
		/// <value>
		/// Gets or sets the 'dataFormat' member
		/// </value>
		public RGBCamera.DataFormatOptions DataFormat
		{
			get
			{
				return this.dataFormat;
			}
			set
			{
				this.SetDataFormat(value);
			}
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="parent">
		/// Parent <see cref="Kinect"/> device that this rgb camera is part of
		/// </param>
		internal RGBCamera(Kinect parent)
		{
			// Save parent device
			this.parentDevice = parent;
			
			// Not running by default
			this.IsRunning = false;
			
			// Set format to RGB by default
			this.DataFormat = DataFormatOptions.RGB;
			
			// Setup callbacks
			KinectNative.freenect_set_rgb_callback(parent.devicePointer, new FreenectRGBDataCallback(RGBCamera.HandleDataReceived));
		}
		
		/// <summary>
		/// Starts streaming RGB data from this camera
		/// </summary>
		public void Start()
		{
			int result = KinectNative.freenect_start_rgb(this.parentDevice.devicePointer);
			if(result != 0)
			{
				throw new Exception("Could not start RGB stream. Error Code: " + result);
			}
			this.IsRunning = true;
		}
		
		/// <summary>
		/// Stops streaming RGB data from this camera
		/// </summary>
		public void Stop()
		{
			int result = KinectNative.freenect_stop_rgb(this.parentDevice.devicePointer);
			if(result != 0)
			{
				throw new Exception("Could not stop RGB stream. Error Code: " + result);
			}
			this.IsRunning = false;
		}
		
		/// <summary>
		/// Sets the RGBCamera's data format. Support function for RGBCamera.DataFormat
		/// </summary>
		/// <param name="format">
		/// A <see cref="RGBCamera.DataFormatOptions"/>
		/// </param>
		private void SetDataFormat(RGBCamera.DataFormatOptions format)
		{
			int result = KinectNative.freenect_set_rgb_format(this.parentDevice.devicePointer, format);
			if(result != 0)
			{
				throw new Exception("Could not switch to RGB format " + format + ". Error Code: " + result);
			}
			this.dataFormat = format;
		}
		
		/// <summary>
		/// Handle image data received from the camera
		/// </summary>
		/// <param name="imageData">
		/// Unmanaged pointer to image data
		/// </param>
		/// <param name="timestamp">
		/// Unix timestamp for teh image data received
		/// </param>
		private void HandleImageDataReceived(IntPtr imageData, Int32 timestamp)
		{
			// Convert image data to a Bitmap
			Bitmap b = this.RGBtoBitmap(imageData);
			
			// UNIX timestamp -> Datetime
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(timestamp);
			
			// Raise event
			this.DataReceived(null, new RGBCamera.DataReceivedEventArgs(dateTime, b));
		}
		
		/// <summary>
		/// Converts unmanaged 24bpp RGB data to a Bitmap for C# consumption
		/// </summary>
		/// <param name="imageData">
		/// Unmanaged pointer to image data
		/// </param>
		/// <returns>
		/// Bitmap version of the RGB data
		/// </returns>
		private Bitmap RGBtoBitmap(IntPtr imageData)
		{
			Bitmap bmp = new Bitmap(640, 480, PixelFormat.Format24bppRgb);
			BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, 640, 480), ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
			unsafe
			{
				byte *src = (byte *)imageData;
				byte *dest = (byte *)bmpData.Scan0;
				int i;
				for(i = 0; i < RGBCamera.rgbFormatSize; i += 3)
				{
					dest[i] = src[i + 2];
					dest[i + 1] = src[i + 1];
					dest[i + 2] = src[i];
				}
			}
			bmp.UnlockBits(bmpData);
			return bmp;
		}
		
		/// <summary>
		/// Converts unmanaged Bayer image data to a Bitmap for C# consumption
		/// </summary>
		/// <param name="imageData">
		/// A <see cref="IntPtr"/>
		/// </param>
		/// <returns>
		/// A <see cref="Bitmap"/>
		/// </returns>
		private Bitmap BayerToBitmap(IntPtr imageData)
		{
			throw new NotImplementedException("Bayer format for teh RGB camera isn't done yet. But coming soon!");
		}
		
		/// <summary>
		/// Static callback for C function. This finds out which device the callback was meant for 
		/// and calls that specific RGBcamera's image handler.
		/// </summary>
		/// <param name="device">
		/// A <see cref="IntPtr"/>
		/// </param>
		/// <param name="imageData">
		/// A <see cref="IntPtr"/>
		/// </param>
		/// <param name="timestamp">
		/// A <see cref="Int32"/>
		/// </param>
		private static void HandleDataReceived(IntPtr device, IntPtr imageData, Int32 timestamp)
		{
			Kinect realDevice = KinectNative.GetDevice(device);
			realDevice.RGBCamera.HandleImageDataReceived(imageData, timestamp);
		}
		
		/// <summary>
		/// Format for RGB data coming in
		/// </summary>
		public enum DataFormatOptions
		{
			RGB = 0,
			Bayer = 1
		}
		
		/// <summary>
		/// Delegate for rgb camera data events
		/// </summary>
		public delegate void DataReceivedEventHandler(object sender, DataReceivedEventArgs e);
		
		/// <summary>
		/// RGB camera data
		/// </summary>
		public class DataReceivedEventArgs
		{
			/// <summary>
			/// Gets the timestamp for this image
			/// </summary>
			public DateTime Timestamp
			{
				get;
				private set;
			}
			
			/// <summary>
			/// Gets the image data
			/// </summary>
			public Bitmap Image
			{
				get;
				private set;
			}
			
			public DataReceivedEventArgs(DateTime timestamp, Bitmap b)
			{
				this.Timestamp = timestamp;
				this.Image = b;
			}
		}
	}
}

