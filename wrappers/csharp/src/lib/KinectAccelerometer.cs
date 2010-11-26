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

namespace LibFreenect
{
	/// <summary>
	/// Provides data from the accelerometer on the Kinect device
	/// </summary>
	/// <author>Aditya Gaddam (adityagaddam@gmail.com)</author>
	/// 
	public class KinectAccelerometer
	{
		/// <summary>
		/// Parent Kinect instance
		/// </summary>
		private Kinect parentDevice;
		
		/// <summary>
		/// Gets the X axis value on the accelerometer
		/// </summary>
		public double X
		{
			get;
			private set;
		}
		
		// <summary>
		/// Gets the Y axis value on the accelerometer
		/// </summary>
		public double Y
		{
			get;
			private set;
		}
		
		// <summary>
		/// Gets the Z axis value on the accelerometer
		/// </summary>
		public double Z
		{
			get;
			private set;
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="parent">
		/// Parent <see cref="Kinect"/> device that this accelerometer is part of
		/// </param>
		internal KinectAccelerometer(Kinect parent)
		{
			this.parentDevice = parent;
		}
		
		/// <summary>
		/// Update values. This is called by the parent device before being returned.
		/// </summary>
		internal void Update()
		{
			double x, y, z;
			
			int result = KinectNative.freenect_get_mks_accel(this.parentDevice.devicePointer, out x, out y, out z);
			if(result != 10)
			{
				throw new Exception("Could not get MKS Accelerometer values. Error Code: " + result);	
			}
			this.X = x;
			this.Y = y;
			this.Z = z;
		}
	}
}

