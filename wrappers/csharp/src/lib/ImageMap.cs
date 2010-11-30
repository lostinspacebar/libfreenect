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

namespace LibFreenect
{
	/// <summary>
	/// Represents a map of rgb values from the RGBCamera
	/// </summary>
	/// <author>Aditya Gaddam (adityagaddam@gmail.com)</author>
	/// 
	public class ImageMap
	{
		
		/// <summary>
		/// Returns the Color value at the specified x and y coordinates
		/// </summary>
		/// <param name="x">
		/// A <see cref="System.Int32"/>
		/// </param>
		/// <param name="y">
		/// A <see cref="System.Int32"/>
		/// </param>
		public Color this[int x, int y]
		{
			get
			{
				uint value = this.Data[y * this.Width + x];
				return Color.FromArgb((int)(value >> 16), (int)(value >> 8) & 0xFF, (int)value & 0xFF);
			}
		}
		
		/// <summary>
		/// Gets the width of the depth map
		/// </summary>
		public int Width
		{
			get;
			private set;
		}
		
		/// <summary>
		/// Gets the height of the depth map
		/// </summary>
		public int Height
		{
			get;
			private set;
		}
		
		/// <summary>
		/// Gets the raw data in the ImageMap. This data is in a 1-dimensional 
		/// array so it's easy to work with in unsafe code. 
		/// </summary>
		public byte[] Data;
		
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="width">
		/// Width of the image map
		/// </param>
		/// <param name="height">
		/// Height of the image map
		/// </param>
		public ImageMap(int width, int height)
		{
			this.Width = width;
			this.Height = height;
		}
	}
}

