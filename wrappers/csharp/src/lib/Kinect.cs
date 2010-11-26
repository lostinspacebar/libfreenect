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
	/// Kinect device. This wraps functionality associated with the entire Kinect
	/// device into a happy little bundle.
	/// </summary>
	/// <author>Aditya Gaddam (adityagaddam@gmail.com)</author>
	/// 
	public class Kinect
	{
		/// <summary>
		/// Pointer to native device object
		/// </summary>
		internal IntPtr devicePointer = IntPtr.Zero;
		
		/// <summary>
		/// Accelerometer instance
		/// </summary>
		private KinectAccelerometer accelerometer;
		
		/// <summary>
		/// Gets the device ID for this Kinect device
		/// </summary>
		public int DeviceID
		{
			get;
			private set;
		}
		
		/// <summary>
		/// Gets whether the connection to the device is open
		/// </summary>
		public bool IsOpen
		{
			get;
			private set;
		}
		
		/// <summary>
		/// Gets the LED on this Kinect device
		/// </summary>
		public KinectLED LED
		{
			get;
			private set;
		}
		
		/// <summary>
		/// Gets the Motor instance for this Kinect device
		/// </summary>
		public KinectMotor Motor
		{
			get;
			private set;
		}
		
		/// <summary>
		/// Gets the accelerometer for this Kinect device
		/// </summary>
		public KinectAccelerometer Accelerometer
		{
			get
			{
				this.accelerometer.Update();
				return this.accelerometer;
			}
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="id">
		/// ID of the Kinect Device. This is a value in the range [0, Kinect.DeviceCount - 1]
		/// </param>
		public Kinect(int id)
		{
			// Make sure id is under  DeviceCount
			if(id >= Kinect.DeviceCount)
			{
				throw new ArgumentOutOfRangeException("The device ID has to be in the range [0, Kinect.DeviceCount - 1]");
			}
			
			// Store device ID for later
			this.DeviceID = id;
		}
		
		/// <value>
		/// Gets number of Kinect devices connected
		/// </value>
		public static int DeviceCount
		{
			get
			{
				return Kinect.GetDeviceCount();
			}
		}
		
		/// <summary>
		/// Opens up the connection to this Kinect device
		/// </summary>
		public void Open()
		{
			int result = KinectNative.freenect_open_device(KinectNative.Context, ref this.devicePointer, this.DeviceID);
			if(result != 0)
			{
				throw new Exception("Could not open connection to Kinect Device (ID=" + this.DeviceID + "). Error Code = " + result);
			}
			
			// Create child instances
			this.LED = new KinectLED(this);
			this.Motor = new KinectMotor(this);
			this.accelerometer = new KinectAccelerometer(this);
		}
		
		/// <summary>
		/// Closes the connection to this Kinect device
		/// </summary>
		public void Close()
		{
			int result = KinectNative.freenect_close_device(this.devicePointer);
			if(result != 0)
			{
				throw new Exception("Could not close connection to Kinect Device (ID=" + this.DeviceID + "). Error Code = " + result);
			}
			
			// Dispose of child instances
			this.LED = null;
			this.Motor = null;
			this.accelerometer = new KinectAccelerometer(this);
		}
		
		/// <summary>
		/// Shuts down the Kinect.NET library and closes any open devices.
		/// </summary>
		public static void Shutdown()
		{
			KinectNative.ShutdownContext();
		}
		
		/// <summary>
		/// Gets the number of Kinect devices connected 
		/// </summary>
		/// <remarks>
		/// This is just a support function for the Kinect.DeviceCount property
		/// </remarks>
		/// <returns>
		/// Number of Kinect devices connected.
		/// </returns>
		private static int GetDeviceCount()
		{		
			// Now we can just return w/e native method puts out
			return KinectNative.freenect_num_devices(KinectNative.Context);
		}
		
		/// <summary>
		/// Logging levels from the C library
		/// </summary>
		public enum LoggingLevel
		{
			Fatal = 0,
			Error,
			Warning,
			Notice,
			Info,
			Debug,
			Spew,
			Flood,
		}
		
	}
}

