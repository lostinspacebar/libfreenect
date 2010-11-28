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
using System.Runtime.InteropServices;

namespace LibFreenect
{
	/// <summary>
	/// Provides access to the depth camera on the Kinect
	/// </summary>
	/// <author>Aditya Gaddam (adityagaddam@gmail.com)</author>
	/// 
	public class DepthCamera
	{
		/// <summary>
		/// Depth format size
		/// </summary>
		internal const int depthDataSize = 640 * 480;
		
		/// <summary>
		/// Parent Kinect instance
		/// </summary>
		private Kinect parentDevice;
		
		/// <summary>
		/// Current data format
		/// </summary>
		private DataFormatOptions dataFormat;
		
		/// <summary>
		/// Gets whether the depth camera is streaming data
		/// </summary>
		public bool IsRunning
		{
			get;
			private set;
		}
		
		/// <summary>
		/// Event raised when depth data has been received.
		/// </summary>
		public event DataReceivedEventHandler DataReceived = delegate { };
		
		/// <summary>
		/// Gets or sets the data format this camera will send depth data in.
		/// </summary>
		/// <value>
		/// Gets or sets the 'dataFormat' member
		/// </value>
		public DataFormatOptions DataFormat
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
		/// Parent <see cref="Kinect"/> device that this depth camera is part of
		/// </param>
		internal DepthCamera(Kinect parent)
		{
			// Save parent device
			this.parentDevice = parent;
			
			// Not running by default
			this.IsRunning = false;
			
			// Set format to 11 bit by default
			this.DataFormat = DataFormatOptions.Format11Bit;
			
			// Setup callbacks
			KinectNative.freenect_set_depth_callback(parent.devicePointer, new FreenectDepthDataCallback(DepthCamera.HandleDataReceived));
		}
		
		/// <summary>
		/// Starts streaming depth data from this camera
		/// </summary>
		public void Start()
		{
			int result = KinectNative.freenect_start_depth(this.parentDevice.devicePointer);
			if(result != 0)
			{
				throw new Exception("Could not start depth stream. Error Code: " + result);
			}
			this.IsRunning = true;
		}
		
		/// <summary>
		/// Stops streaming depth data from this camera
		/// </summary>
		public void Stop()
		{
			int result = KinectNative.freenect_stop_depth(this.parentDevice.devicePointer);
			if(result != 0)
			{
				throw new Exception("Could not depth RGB stream. Error Code: " + result);
			}
			this.IsRunning = false;
		}
		
		/// <summary>
		/// Sets the DepthCameras's data format. Support function for DepthCamera.DataFormat
		/// </summary>
		/// <param name="format">
		/// A <see cref="DepthCamera.DataFormatOptions"/>
		/// </param>
		private void SetDataFormat(DepthCamera.DataFormatOptions format)
		{
			int result = KinectNative.freenect_set_depth_format(this.parentDevice.devicePointer, format);
			if(result != 0)
			{
				throw new Exception("Could not switch to depth format " + format + ". Error Code: " + result);
			}
			this.dataFormat = format;
		}
		
		private static DepthMap CreateDepthMap11Bit(UInt16[] depthData)
		{
			DepthMap depthMap = new DepthMap(640, 480);
			depthMap.Data = depthData;
			return depthMap;
		}
		
		private static DepthMap CreateDepthMap10Bit(UInt16[] depthData)
		{
			throw new NotImplementedException("10 bit data not implemented yet. Soon though!");
		}
		
		private static DepthMap CreateDepthMapPacked11Bit(UInt16[] depthData)
		{
			throw new NotImplementedException("Packed 11 bit data not implemented yet. Soon though!");
		}
		
		private static DepthMap CreateDepthMapPacked10Bit(UInt16[] depthData)
		{
			throw new NotImplementedException("Packed 10 bit data not implemented yet. Soon though!");
		}
		
		/// <summary>
		/// Static callback for C function. This finds out which device the callback was meant for 
		/// and calls that specific DepthCamera's depth data handler.
		/// </summary>
		/// <param name="device">
		/// A <see cref="IntPtr"/>
		/// </param>
		/// <param name="depthData">
		/// A <see cref="IntPtr"/>
		/// </param>
		/// <param name="timestamp">
		/// A <see cref="Int32"/>
		/// </param>
		private static void HandleDataReceived(IntPtr device, UInt16[] depthData, Int32 timestamp)
		{
			Kinect realDevice = KinectNative.GetDevice(device);
			DepthMap depthMap = null;
			
			switch(realDevice.DepthCamera.DataFormat)
			{
			case DataFormatOptions.Format11Bit:
				depthMap = CreateDepthMap11Bit(depthData);
				break;
			case DataFormatOptions.Format10Bit:
				depthMap = CreateDepthMap10Bit(depthData);
				break;
			case DataFormatOptions.FormatPacked11Bit:
				depthMap = CreateDepthMapPacked11Bit(depthData);
				break;
			case DataFormatOptions.FormatPacked10Bit:
				depthMap = CreateDepthMapPacked10Bit(depthData);
				break;
			}
			
			// UNIX timestamp -> Datetime
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0).AddSeconds(timestamp);
			
			// Raise event
			realDevice.DepthCamera.DataReceived(realDevice, new DataReceivedEventArgs(dateTime, depthMap));
		}
		
		/// <summary>
		/// Format for Depth data coming in
		/// </summary>
		public enum DataFormatOptions
		{
			Format11Bit = 0,
			Format10Bit = 1,
			FormatPacked11Bit = 2,
			FormatPacked10Bit = 3
		}
		
		/// <summary>
		/// Delegate for depth camera data events
		/// </summary>
		public delegate void DataReceivedEventHandler(object sender, DataReceivedEventArgs e);
		
		/// <summary>
		/// Depth camera data
		/// </summary>
		public class DataReceivedEventArgs
		{
			/// <summary>
			/// Gets the timestamp for when this depth data was received
			/// </summary>
			public DateTime Timestamp
			{
				get;
				private set;
			}
			
			/// <summary>
			/// Gets the depth data 
			/// </summary>
			public DepthMap DepthMap
			{
				get;
				private set;
			}
			
			/// <summary>
			/// constructor
			/// </summary>
			/// <param name="timestamp">
			/// A <see cref="DateTime"/>
			/// </param>
			/// <param name="depthMap">
			/// A <see cref="DepthMap"/>
			/// </param>
			public DataReceivedEventArgs(DateTime timestamp, DepthMap depthMap)
			{
				this.Timestamp = timestamp;
				this.DepthMap = depthMap;
			}
		}
	}
}

