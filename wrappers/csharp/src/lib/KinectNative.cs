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
	/// Provides access to native libfreenect calls. These are "ugly" calls used internally 
	/// in the wrapper.
	/// </summary>
	/// <author>Aditya Gaddam (adityagaddam@gmail.com)</author>
	/// 
	class KinectNative
	{
		/// <summary>
		/// Main freenect context. There is one per session.
		/// </summary>
		private static IntPtr freenectContext = IntPtr.Zero;
		
		/// <summary>
		/// Gets a freenect context to work with.
		/// </summary>
		public static IntPtr Context
		{
			get
			{
				// Make sure we have a context
				if(KinectNative.freenectContext == IntPtr.Zero)
				{
					KinectNative.InitializeContext();
				}
				
				// Return it
				return KinectNative.freenectContext;
			}
		}
		
		/// <summary>
		/// Shuts down the context and closes any open devices.
		/// </summary>
		public static void ShutdownContext()
		{
			int result = KinectNative.freenect_shutdown(KinectNative.freenectContext);
			if(result != 0)
			{
				throw new Exception("Could not shutdown freenect context. Error Code:" + result);
			}
			KinectNative.freenectContext = IntPtr.Zero;
		}
		
		/// <summary>
		/// Initializes the freenect context
		/// </summary>
		private static void InitializeContext()
		{
			int result = KinectNative.freenect_init(ref KinectNative.freenectContext, IntPtr.Zero);
			if(result != 0)
			{
				throw new Exception("Could not initialize freenect context. Error Code:" + result);
			}
		}

		[DllImport("libfreenect")]
		public static extern int freenect_init(ref IntPtr context, IntPtr freenectUSBContext);
		
		[DllImport("libfreenect")]
		public static extern int freenect_shutdown(IntPtr context);
		
		[DllImport("libfreenect")]
		public static extern int freenect_num_devices(IntPtr context);
		
		[DllImport("libfreenect")]
		public static extern int freenect_open_device(IntPtr context, ref IntPtr device, int index);
		
		[DllImport("libfreenect")]
		public static extern int freenect_close_device(IntPtr device);
		
		[DllImport("libfreenect")]
		public static extern int freenect_set_led(IntPtr device, int option);
	}
	
	/// <summary>
	/// "Native" callback for freelect library logging
	/// </summary>
	delegate void FreenectLogCallback(out IntPtr context, Kinect.LoggingLevel logLevel, string message);
	
	/// <summary>
	/// "Native" callback for depth data
	/// </summary>
	delegate void FreenectDepthDataCallback(out IntPtr device, IntPtr depthData, Int32 timestamp);
	
	/// <summary>
	/// "Native" callback for RGB image data
	/// </summary>
	delegate void FreenectRGBDataCallback(out IntPtr device, IntPtr rgbData, Int32 timestamp);
	
	//typedef void (*freenect_depth_cb)(freenect_device *dev, void *depth, uint32_t timestamp);
	//typedef void (*freenect_rgb_cb)(freenect_device *dev, freenect_pixel *rgb, uint32_t timestamp);
	
}
