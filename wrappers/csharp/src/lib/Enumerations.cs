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
using System.Collections.Generic;

namespace freenect
{
	/// <summary>
	/// Data formats for the video camera
	/// </summary>
	public enum VideoFormat : int
	{
		/// <summary>
		/// Decompressed RGB image. 3 bytes per pixel.
		/// </summary>
		RGB 				= 0,
		/// <summary>
		/// Raw bayer image data from Kinect.
		/// </summary>
		Bayer 				= 1,
		/// <summary>
		/// 8-bit infrared data. Each pixel gets 1 byte.
		/// </summary>
		Infrared8Bit 		= 2,
		/// <summary>
		/// 10-bit infrared data. Each pixel gets 2 bytes.
		/// </summary>
		Infrared10Bit 		= 3,
		/// <summary>
		/// 10-bit packed infrared data. This data doesn't 
		/// necessarily fall on any byte boundaries.
		/// </summary>
		InfraredPacked10Bit = 4,
		/// <summary>
		/// YUV RGB data
		/// </summary>
		YUVRGB 				= 5,
		/// <summary>
		/// YUV raw data
		/// </summary>
		YUVRaw 				= 6
	}
	
	/// <summary>
	/// Data formats for the depth camera
	/// </summary>
	public enum DepthFormat : int
	{
		/// <summary>
		/// 11-bit intensity values per pixel. You will receive 16-bit values (2 bytes) per pixel,
		/// but only 11-bits from these are useful. Data is structured as 
		/// [5 dont cares|11 data bits][5 dont cares|11 data bits]
		/// </summary>
		Depth11Bit 						= 0,
		/// <summary>
		/// Same as Depth11Bit, except 10-bit values.
		/// </summary>
		Depth10Bit 						= 1,
		/// <summary>
		/// Packed 11-bit values per pixel. This means, byte boundaries aren't respected. 
		/// Data is represented as [11 data bits][11 data bits] etc. This isn't very useful 
		/// in most cases.
		/// </summary>
		DepthPacked11Bit 				= 2,
		/// <summary>
		/// Same as DepthPacked11bit, except 10-bit values
		/// </summary>
		DepthPacked10Bit 				= 3,
		/// <summary>
		/// Processed depth data in millimeters (mm). Aligned to 640x480 RGB image.
		/// These will be 16-bit (2-byte) values.
		/// </summary>
		DepthProcessedAndRegistered 	= 4,
		/// <summary>
		/// Same as DepthProcessedAndRegistered, but the depth data isn't aligned with 
	 	/// the RGB image. These will be 16-bit (2-byte) values.
		/// </summary>
		DepthProcessed					= 5
	}
	
	/// <summary>
	/// Resolution settings.
	/// 
	/// LOW = QVGA (320x240)
	/// MEDIUM = VGA (640x480 for video, 640x488 for IR)
	/// HIGH = SXGA (1280x1024)
	/// </summary>
	public enum Resolution : int
	{
		/// <summary>
		/// QVGA (320x240)
		/// </summary>
		Low 	= 0,
		/// <summary>
		/// VGA (640x480)
		/// </summary>
		Medium 	= 1,
		/// <summary>
		/// SXGA (1280x1024)
		/// </summary>
		High 	= 2
	}
	
	/// <summary>
	/// LED colors. None means LED is off.
	/// </summary>
	public enum LEDColor : int
	{
		None    		= 0,
		Green  			= 1,
		Red    			= 2,
		Yellow 			= 3,
		BlinkYellow 	= 4,
		BlinkGreen 		= 5,
		BlinkRedYellow	= 6
	}
	
	/// <summary>
	/// Different states the tilt motor can be in operation
	/// </summary>
	public enum MotorTiltStatus : int
	{
		Stopped 	= 0x00,
	 	AtLimit 	= 0x01,
		Moving 		= 0x04
	}
	
	/// <summary>
	/// Logging levels from the C library
	/// </summary>
	public enum LoggingLevel : int
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
	
	/// <summary>
	/// Sub-device in the Kinect device.
	/// </summary>
	public enum SubDevice : int
	{
		Motor 		= 0x01,
		Camera 		= 0x02,
		Audio 		= 0x04
	}
}