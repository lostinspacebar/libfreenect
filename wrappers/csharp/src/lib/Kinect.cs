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
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace freenect
{
	/// <summary>
	/// Kinect device. This wraps functionality associated with the entire Kinect
	/// device into a happy little bundle.
	/// </summary>
	///
	/// 
	public class Kinect
	{		
		/// <summary>
		/// Gets or sets the logging level for the Kinect library. This controls
		/// how much debugging information is sent to the logging callback
		/// </summary>
		public static LoggingLevel LogLevel
		{
			get
			{
				return Kinect.logLevel;
			}
			set
			{
				Kinect.SetLogLevel(value);
			}
		}
		
		/// <summary>
		/// Gets number of Kinect devices connected
		/// </summary>
		public static int DeviceCount
		{
			get
			{
				return Kinect.GetDeviceCount();
			}
		}
		
		/// <summary>
		/// Gets the sub-devices supported by the Kinect
		/// </summary>
		public static SubDeviceOptions SupportedSubDevices
		{
			get
			{
				return KinectNative.freenect_supported_subdevices();
			}
		}
		
		/// <summary>
		/// Gets or sets enabled sub-devices. This property is only 
		/// used when calling Kinect.Open. 
		/// </summary>
		public static SubDeviceOptions EnabledSubDevices
		{
			get;
			set;
		}
		
		/// <summary>
		/// Raised when a log item is received from the low level Kinect library.
		/// </summary>
		public static event LogEventHandler Log = delegate { };
		
		/// <summary>
		/// Gets the device index for this Kinect device. If multiple Kinect 
		/// devices are being used, this property is not guaranteed to stay 
		/// the same for a specific Kinect device.
		/// </summary>
		public int DeviceIndex
		{
			get;
			private set;
		}
		
		/// <summary>
		/// Gets the camera serial 'number' for this Kinect device. This 
		/// is guaranteed to be the same between uses when using multiple 
		/// devices.
		/// </summary>
		/// <value>
		/// The camera serial.
		/// </value>
		public string CameraSerial
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
		public LED LED
		{
			get;
			private set;
		}
		
		/// <summary>
		/// Gets the Motor instance for this Kinect device
		/// </summary>
		public Motor Motor
		{
			get;
			private set;
		}
		
		/// <summary>
		/// Gets the accelerometer for this Kinect device
		/// </summary>
		public Accelerometer Accelerometer
		{
			get;
			private set;
		}
		
		/// <summary>
		/// Gets the RGB camera for this Kinect device
		/// </summary>
		public VideoCamera VideoCamera
		{
			get;
			private set;
		}
		
		/// <summary>
		/// Gets the depth camera for this Kinect device
		/// </summary>
		public DepthCamera DepthCamera
		{
			get;
			private set;
		}
		
		/// <summary>
		/// Gets or sets the name for this Kinect Device. 
		/// </summary>
		/// <remarks>
		/// This means nothing at all to the actual library, but can be useful for 
		/// debugging/presentation reasons. The default value is "Device {Kinect.DeviceID}" 
		/// without the curly braces. For example, "Device 0" or "Device 1". 
		/// But you can make it whatever the hell you want.
		/// </remarks>
		public string Name
		{
			get;
			set;
		}
		
		/// <summary>
		/// Current logging level for the kinect session (for all devices)
		/// </summary>
		private static LoggingLevel logLevel;
		
		/// <summary>
		/// Pointer to native device object
		/// </summary>
		internal IntPtr devicePointer = IntPtr.Zero;
		
		/// <summary>
		/// Cached device state that can be used after a call to Kinect.UpdateStatus
		/// This can be used to save some USB or P/Invoke calls.
		/// </summary>
		internal FreenectTiltState cachedDeviceState;
		
		/// <summary>
		/// Initialize some static stuff for the Kinect class.
		/// </summary>
		static Kinect()
		{
			Kinect.EnabledSubDevices = SubDeviceOptions.Audio | SubDeviceOptions.Camera | SubDeviceOptions.Motor;
		}
		
		/// <summary>
		/// Ninja Constructor
		/// </summary>
		private Kinect()
		{
			
		}
		
		/// <summary>
		/// Open up a connection the Kinect with the specified ID. This also updates the 
		/// set of enabled/disabled sub-devices.
		/// </summary>
		/// <param name='id'>
		/// Index of Kinect to open.
		/// </param>
		/// <param name='enabledSubDevices'>
		/// Sub-devices to enable before opening this device
		/// </param>
		public static Kinect Open(int id, SubDeviceOptions enabledSubDevices)
		{
			// Change enabled sub-devices preferences
			Kinect.EnabledSubDevices = enabledSubDevices;
			
			// Open kinect connection
			return Kinect.Open(id);
		}
		
		/// <summary>
		/// Opens up a connection to the Kinect with the specified ID
		/// </summary>
		public static Kinect Open(int id)
		{			
			// Select sub-devices
			KinectNative.freenect_select_subdevices(KinectNative.Context, Kinect.EnabledSubDevices);
			
			// Create instance
			Kinect k = new Kinect();
			
			// Save device-index
			k.DeviceIndex = id;
			
			// Open device
			int result = KinectNative.freenect_open_device(KinectNative.Context, ref k.devicePointer, id);
			if(result != 0)
			{
				throw new Exception("Could not open connection to Kinect Device (ID=" + k.DeviceIndex + "). Error Code = " + result);
			}
			
			// Initialize rest of device (this is common regardless of how the Kinect was opened)
			k.Initialize();
			
			// All done
			return k;
		}
		
		//// <summary>
		/// Opens up a connection to the Kinect with the specified camera serial. This also 
		/// updates the enabled sub-devices before opening the Kinect.
		/// </summary>
		/// <param name='cameraSerial'>
		/// Camera serial.
		/// </param>
		/// <param name='enabledSubDevices'>
		/// Enabled sub devices.
		/// </param>
		public static Kinect Open(string cameraSerial, SubDeviceOptions enabledSubDevices)
		{
			// Update enabled sub-devices 
			Kinect.EnabledSubDevices = enabledSubDevices;
			
			// All done
			return Kinect.Open(cameraSerial);
		}
		
		/// <summary>
		/// Opens up a connection to the Kinect with the specified camera serial
		/// </summary>
		/// <param name='cameraSerial'>
		/// Camera serial.
		/// </param>
		public static Kinect Open(string cameraSerial)
		{
			// Select sub-devices
			KinectNative.freenect_select_subdevices(KinectNative.Context, Kinect.EnabledSubDevices);
			
			// Create instance
			Kinect k = new Kinect();
			
			// Save camera serial
			k.CameraSerial = cameraSerial;
			
			// Open device
			int result = KinectNative.freenect_open_device_by_camera_serial(KinectNative.Context, ref k.devicePointer, cameraSerial);
			if(result != 0)
			{
				throw new Exception("Could not open connection to Kinect Device (Camera Serial=" + k.CameraSerial + "). Error Code = " + result);
			}
			
			// Initialize rest of device (this is common regardless of how the Kinect was opened)
			k.Initialize();
			
			// All done
			return k;
		}
		
		/// <summary>
		/// Initialize sub-devices in this open Kinect device.
		/// </summary>
		private void Initialize()
		{
			// Create Accel, Motor and LED connections if MOTOR is enabled
			this.Motor = null;
			this.LED = null;
			this.Accelerometer = null;
			if(Kinect.EnabledSubDevices.HasFlag(SubDeviceOptions.Motor))
			{
				this.LED = new LED(this);
				this.Accelerometer = new Accelerometer(this);
				this.Motor = new Motor(this);
			}
			
			// Create camera connections if CAMERA is enabled
			this.VideoCamera = null;
			this.DepthCamera = null;
			if(Kinect.EnabledSubDevices.HasFlag(SubDeviceOptions.Camera))
			{
				this.VideoCamera = new VideoCamera(this);
				this.DepthCamera = new DepthCamera(this);
			}
			
			//Register the device
			KinectNative.RegisterDevice(this.devicePointer, this);
			
			// Open now
			this.IsOpen = true;
		}
		
		/// <summary>
		/// Closes the connection to this Kinect device
		/// </summary>
		public void Close()
		{
			// Stop Cameras
			if(Kinect.EnabledSubDevices.HasFlag(SubDeviceOptions.Camera))
			{
				if(this.VideoCamera.IsRunning)
				{
					this.VideoCamera.Stop();
				}
				if(this.DepthCamera.IsRunning)
				{
					this.DepthCamera.Stop();
				}
			}
			
			// Close device
			int result = KinectNative.freenect_close_device(this.devicePointer);
			if(result != 0)
			{
				throw new Exception("Could not close connection to Kinect Device (ID=" + this.DeviceIndex + "). Error Code = " + result);
			}
			
			// Dispose of child instances
			this.LED = null;
			this.Motor = null;
			this.Accelerometer = null;
			this.VideoCamera = null;
			this.DepthCamera = null;
			
			// Unegister the device
			KinectNative.UnregisterDevice(this.devicePointer);
			
			// Not open anymore
			this.IsOpen = false;
		}
		
		/// <summary>
		/// Gets updated device status from the Kinect. This updates any properties in the 
		/// child devices (Motor, LED, etc.). This method only does stuff if SubDevice.Motor
		/// is specified for Kinect.EnabledSubDevices.
		/// </summary>
		public void UpdateStatus()
		{
			// Only try to update status if the motor device is actually enabled/open
			if(Kinect.EnabledSubDevices.HasFlag(SubDeviceOptions.Motor))
			{
				// Ask for new device status
				KinectNative.freenect_update_tilt_state(this.devicePointer);
				
				// Get updated device status
				IntPtr ptr = KinectNative.freenect_get_tilt_state(this.devicePointer);
				this.cachedDeviceState = (FreenectTiltState)Marshal.PtrToStructure(ptr, typeof(FreenectTiltState));
			}
		}
		
		/// <summary>
		/// Gets information for all available Kinect devices on the USB chain.
		/// </summary>
		/// <returns>
		/// Array of device info instances
		/// </returns>
		public static DeviceInfo[] GetDevices()
		{
			IntPtr deviceInfoList = IntPtr.Zero;
			List<DeviceInfo> devices = new List<DeviceInfo>();
			
			// Get linked list pointer
			KinectNative.freenect_list_device_attributes(KinectNative.Context, out deviceInfoList);
			
			// Get the first device at the head of the list
			IntPtr deviceInfoPointer = deviceInfoList;
			
			// Go through the linked list as long as the current device pointer isn't "null"
			while(deviceInfoPointer != IntPtr.Zero)
			{			
				// Marshall into native version of struct
				DeviceInfoNative nativeInfo = (DeviceInfoNative)Marshal.PtrToStructure(deviceInfoList, typeof(DeviceInfoNative));
				
				// Add new managed version to our list
				devices.Add(new DeviceInfo(nativeInfo.serial));
				
				// Look at the child/next item in the list for next iteration
				deviceInfoPointer = nativeInfo.next;
			}
			
			// Free pointer
			KinectNative.freenect_free_device_attributes(deviceInfoList);
			
			// All done
			return devices.ToArray();
		}
		
		/// <summary>
		/// Makes the base library handle any pending USB events. Either this, or UpdateStatus
		/// should be called repeatedly.
		/// </summary>
		public static void ProcessEvents()
		{
			KinectNative.freenect_process_events(KinectNative.Context);
		}
		
		/// <summary>
		/// Makes the base library handle any pending USB events with a specified timeout.
		/// Either this, or UpdateStatus should be called repeatedly.
		/// </summary>
		public static void ProcessEvents(int seconds, int microSeconds = 0)
		{
			Timeval timeval = new Timeval();
			timeval.tv_sec = seconds;
			timeval.tv_usec = microSeconds;
			KinectNative.freenect_process_events_timeout(KinectNative.Context, ref timeval);
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
		/// Sets the logging level for the Kinect session. Support function for Kinect.LogLevel property.
		/// </summary>
		/// <param name="level">
		/// A <see cref="LogLevel"/>
		/// </param>
		private static void SetLogLevel(LoggingLevel level)
		{
			KinectNative.freenect_set_log_level(KinectNative.Context, level);
			Kinect.logLevel = level;
		}
		
		/// <summary>
		/// Logging callback.
		/// </summary>
		/// <param name="device">
		/// A <see cref="IntPtr"/>
		/// </param>
		/// <param name="logLevel">
		/// A <see cref="Kinect.LogLevelOptions"/>
		/// </param>
		/// <param name="message">
		/// A <see cref="System.String"/>
		/// </param>
		internal static void LogCallback(IntPtr device, LoggingLevel logLevel, string message)
		{
			Kinect realDevice = KinectNative.GetDevice(device);
			Kinect.Log(null, new LogEventArgs(realDevice, logLevel, message));
		}
		
		/// <summary>
		/// Native device info struct to marshal in data
		/// </summary>
		private struct DeviceInfoNative
		{
			public IntPtr next;
			[MarshalAs(UnmanagedType.LPStr)]
			public string serial;
		}
	}
}

