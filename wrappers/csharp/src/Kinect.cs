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
	/// Kinect device. This wraps functionality associated with the entire Kinect
	/// device into a happy little bundle.
	/// </summary>
	/// <author>Aditya Gaddam (adityagaddam@gmail.com)</author>
	/// 
	public class Kinect
	{
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="id">
		/// ID of the Kinect Device. This is a value in the range [0, Kinect.DeviceCount - 1]
		/// </param>
		public Kinect(int id)
		{
			
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
		/// Gets the number of Kinect devices connected (Support function for the DeviceCount property)
		/// </summary>
		/// <returns>
		/// Number of Kinect devices connected.
		/// </returns>
		private static int GetDeviceCount()
		{
			return 1;
		}
	}
}

