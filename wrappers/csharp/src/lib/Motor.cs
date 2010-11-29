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
	public class Motor
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
		/// Gets the commanded tilt value [-1.0, 1.0] for the motor. This is just the tilt
		/// value that the motor was last asked to go to through Motor.Tilt. This doesn't 
		/// correspond to the actual angle at the physical motor. For that value, see Motor.Tilt.
		/// </summary>
		public double CommandedTilt
		{
			get
			{
				return this.tilt;
			}
		}
		
		public int RawTilt
		{
			get
			{
				return (int)this.parentDevice.cachedDeviceState.TiltAngle;
			}
		}
		
		/// <summary>
		/// Gets or sets the tilt angle of the motor on the Kinect device.
		/// Accepted values are [-1.0, 1.0]. When queried, this returns the ACTUAL
		/// tilt value/status of the motor. To get the commanded tilt value after 
		/// setting this value, you can use the Motor.CommandedTilt property.
		/// </summary>
		public double Tilt
		{
			get
			{
				return this.GetMotorTilt();
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
		internal Motor(Kinect parent)
		{
			this.parentDevice = parent;
			
			// Set tilt to 0 to start
			this.Tilt = 0;
		}
		
		/// <summary>
		/// Gets the motor's actual tilt angle
		/// </summary>
		/// <returns>
		/// Actual tilt angle of the motor as it's moving
		/// </returns>
		private double GetMotorTilt()
		{
			return (((double)this.parentDevice.cachedDeviceState.TiltAngle + 31.0) / 62.0) - 1.0;
		}
		
		/// <summary>
		/// Sets the motor's tilt angle.
		/// </summary>
		/// <param name="angle">
		/// Value between [-1.0, 1.0]
		/// </param>
		private void SetMotorTilt(double angle)
		{
			if(angle < -1.0 || angle > 1.0)
			{
				throw new ArgumentOutOfRangeException("Motor tilt has to be in the range [-1.0, 1.0]");
			}
			
			Console.WriteLine("-");
			double angleReal = Math.Round(angle * 31);
			int result = KinectNative.freenect_set_tilt_degs(this.parentDevice.devicePointer, angleReal);
			if(result != 0)
			{
				throw new Exception("Coult not set motor tilt angle to " + angle + ". Error Code: " + result);
			}
			this.tilt = angle;
		}
		
		/// <summary>
		/// Different states the tilt motor can be in operation
		/// </summary>
		public enum TiltStatusOptions
		{
			Stopped 	= 0x00,
		 	AtLimit 	= 0x01,
			Moving 		= 0x04
		}
	}
}

