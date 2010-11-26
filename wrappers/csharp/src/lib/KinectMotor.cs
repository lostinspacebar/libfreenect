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
	/// Provides control over the Motor on the Kinect
	/// </summary>
	/// <author>Aditya Gaddam (adityagaddam@gmail.com)</author>
	/// 
	public class KinectMotor
	{
		/// <summary>
		/// Parent Kinect instance
		/// </summary>
		private Kinect parentDevice;
		
		/// <summary>
		/// Current tilt angle of the motor
		/// </summary>
		private double tilt;
		
		/// <summary>
		/// Gets or sets the tilt angle of the motor on the Kinect device.
		/// Accepted values are [-31, 31] in 1 degree increments
		/// </summary>
		/// <value>Gets or sets 'tilt' field</value>
		public double Tilt
		{
			get
			{
				return this.tilt;
			}
			set
			{
				this.SetMotorTilt(value);
			}
		}
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="parent">
		/// Parent <see cref="Kinect"/> device that this Motor is part of
		/// </param>
		internal KinectMotor(Kinect parent)
		{
			this.parentDevice = parent;
			
			// Set tilt to 0 to start
			this.Tilt = 0;
		}
		
		/// <summary>
		/// Sets the motor's tilt angle.
		/// </summary>
		/// <param name="angle">
		/// Value between -31 and 31
		/// </param>
		private void SetMotorTilt(double angle)
		{
			if(angle < -31 || angle > 31)
			{
				throw new ArgumentOutOfRangeException("Motor tilt has to be in the range [-31, 31]");
			}
			int result = KinectNative.freenect_set_tilt_degs(this.parentDevice.devicePointer, angle);
			if(result != 0)
			{
				throw new Exception("Coult not set motor tilt angle to " + angle + ". Error Code: " + result);
			}
			this.tilt = angle;
		}
	}
}

