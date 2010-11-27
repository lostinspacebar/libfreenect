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
	public class Accelerometer
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
			get
			{
				return this.MKS.X;
			}
		}
		
		// <summary>
		/// Gets the Y axis value on the accelerometer
		/// </summary>
		public double Y
		{
			get
			{
				return this.MKS.Y;
			}
		}
		
		// <summary>
		/// Gets the Z axis value on the accelerometer
		/// </summary>
		public double Z
		{
			get
			{
				return this.MKS.Z;
			}
		}
		
		/// <summary>
		/// Returns raw accelerometer values. There are 819 values per G. Therefore
		/// the range for this should be [410, 3686]
		/// </summary>
		public Values Raw
		{
			get
			{
				return this.GetRawAccelerometerValues();
			}
		}
		
		/// <summary>
		/// Gets MKS accelerometer values. These are values in m/(s*s).
		/// </summary>
		public Values MKS
		{
			get
			{
				return this.GetMKSAccelerometerValues();
			}
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="parent">
		/// Parent <see cref="Kinect"/> device that this accelerometer is part of
		/// </param>
		internal Accelerometer(Kinect parent)
		{
			this.parentDevice = parent;
		}
		
		/// <summary>
		/// Gets MKS accelerometer values. Support function for the KinectAccelerometer.MKS property.
		/// </summary>
		/// <returns>
		/// <see cref="AccelerometerValues"/> with X, Y, Z populated with MKS accel readings.
		/// </returns>
		private Values GetMKSAccelerometerValues()
		{
			Values values = new Values();
			
			int result = KinectNative.freenect_get_mks_accel(this.parentDevice.devicePointer, out values.X, out values.Y, out values.Z);
			if(result != 10)
			{
				throw new Exception("Could not get MKS Accelerometer values. Error Code: " + result);	
			}
			
			return values;
		}
		
		/// <summary>
		/// Gets raw accelerometer values. Support function for KinectAccelerometer.Raw property.
		/// </summary>
		/// <returns>
		/// <see cref="AccelerometerValues"/> with X, Y, Z populated with Raw accel readings.
		/// </returns>
		private Values GetRawAccelerometerValues()
		{
			Values values = new Values();
			
			return values;
		}
		
		/// <summary>
		/// Set of accelerometer values.
		/// </summary>
		public struct Values
		{
			public double X;
			public double Y;
			public double Z;
		}
	}
}

