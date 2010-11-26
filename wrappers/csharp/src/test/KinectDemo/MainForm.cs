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
using System.Windows.Forms;
using LibFreenect;

namespace GUITest
{
	public class MainForm : Form
	{
		// Kinect Object
		private Kinect kinect;
		
		/// <summary>
		/// Constructor
		/// </summary>
		public MainForm() : base()
		{
			// Initialize GUI Elements
			this.InitializeComponents();
			
			// Check for kinect devices
			if(Kinect.DeviceCount == 0)
			{
				MessageBox.Show("There are no Kinect devices connected to your system. Please reopen the program after you have atleast 1 Kinect connected.");
				Application.Exit();
			}
			
			// Atleast one connected, fill the select box with w/e devices available
			for(int i = 0; i < Kinect.DeviceCount; i++)
			{
				this.kinectDeviceSelectCombo.Items.Add("Device " + i);
			}
			this.kinectDeviceSelectCombo.SelectedIndex = 0;
			
			// Enable the toolbar so we can do things...
			this.mainToolbar.Enabled = true;
		}
		
		private void Connect()
		{
			this.kinect = new Kinect(this.kinectDeviceSelectCombo.SelectedIndex);
			this.kinect.Open();
			this.bottomPanel.Enabled = true;
			
			// Set LED to none
			this.ledControlCombo.SelectedIndex = 0;
			
			// Set Motor to 0
			this.motorControlTrack.Value = 0;
			
			// enable disconnect
			this.connectButton.Enabled = false;
			this.disconnectButton.Enabled = true;
			
			// enable update timer
			this.infoUpdateTimer.Enabled = true;
		}
		
		private void Disconnect()
		{
			// Disable update timer first
			this.infoUpdateTimer.Enabled = false;
			
			// Disconnect from the device
			this.kinect.Close();
			
			// Disable controls again
			this.bottomPanel.Enabled = false;
			
			// enable disconnect
			this.connectButton.Enabled = true;
			this.disconnectButton.Enabled = false;
		}
		
		/// <summary>
		/// Sets the LED on w/e Kinect device is open right now
		/// </summary>
		/// <param name="colorName">
		/// String version of any of the values in KinectLED.ColorOption. Example "Green".
		/// </param>
		private void SetLED(string colorName)
		{
			if(this.kinect == null)
			{
				return;	
			}
			this.kinect.LED.Color = (KinectLED.ColorOption)Enum.Parse(typeof(KinectLED.ColorOption), colorName);
		}
		
		/// <summary>
		/// Sets the tilt angle on w/e Kinect device is open right now
		/// </summary>
		/// <param name="value">
		/// Tilt angle between -1.0 and 1.0
		/// </param>
		private void SetMotorTilt(int value)
		{
			if(this.kinect == null)
			{
				return;	
			}
			this.kinect.Motor.Tilt = (double)value / 100.0;
		}
		
		/// <summary>
		/// Update info panel
		/// </summary>
		private void UpdateInfoPanel()
		{
			if(this.kinect == null)
			{
				return;
			}
			this.motorTiltStatusLabel.Text = "Motor Tilt: " + this.kinect.Motor.Tilt.ToString();
			this.accelXLabel.Text = "Accel X: " + this.kinect.Accelerometer.X.ToString();
			this.accelYLabel.Text = "Accel Y: " + this.kinect.Accelerometer.Y.ToString();
			this.accelZLabel.Text = "Accel Z: " + this.kinect.Accelerometer.Z.ToString();
		}
		
		/// <summary>
		/// Initialize GUI Elements
		/// </summary>
		private void InitializeComponents()
		{
			this.SuspendLayout();
			
			//
			// headingFont
			//
			this.headingFont = new Font(this.Font.Name, 11, FontStyle.Bold);
			
			//
			// regularFont
			//
			this.regularFont = new Font(this.Font.Name, 9);
			
			//
			// kinectDeviceSelectCombo
			//
			this.kinectDeviceSelectCombo = new ToolStripComboBox();
			this.kinectDeviceSelectCombo.DropDownStyle = ComboBoxStyle.DropDownList;
			this.kinectDeviceSelectCombo.Width = 200;
			
			//
			// connectButton
			//
			this.connectButton = new ToolStripButton();
			this.connectButton.Text = "Connect";
			this.connectButton.Width = 150;
			this.connectButton.Click += delegate(object sender, EventArgs e) {
				this.Connect();
			};
			
			//
			// disconnectButton
			//
			this.disconnectButton = new ToolStripButton();
			this.disconnectButton.Text = "Disconnect";
			this.disconnectButton.Width = 150;
			this.disconnectButton.Enabled = false;
			this.disconnectButton.Click += delegate(object sender, EventArgs e) {
				this.Disconnect();
			};
			
			//
			// mainToolbar
			//
			this.mainToolbar = new ToolStrip();
			this.mainToolbar.Dock = DockStyle.Top;
			this.mainToolbar.Items.Add(this.kinectDeviceSelectCombo);
			this.mainToolbar.Items.Add(this.connectButton);
			this.mainToolbar.Items.Add(this.disconnectButton);
			this.mainToolbar.AutoSize = false;
			this.mainToolbar.Height = 30;
			this.mainToolbar.Renderer = new ToolStripSystemRenderer();
			this.mainToolbar.Enabled = false;
			
			//
			// rgbPanel
			//
			this.rgbPanel = new Panel();
			this.rgbPanel.Width = 400;
			this.rgbPanel.Height = 300;
			this.rgbPanel.Margin = new Padding(0);
			
			//
			// depthPanel
			//
			this.depthPanel = new Panel();
			this.depthPanel.Width = 400;
			this.depthPanel.Height = 300;
			this.depthPanel.Margin = new Padding(0);
			
			//
			// topPanel
			//
			this.topPanel = new TableLayoutPanel();
			this.topPanel.ColumnCount = 2;
			this.topPanel.RowCount = 1;
			this.topPanel.Controls.Add(this.rgbPanel, 0, 0);
			this.topPanel.Controls.Add(this.depthPanel, 1, 0);
			this.topPanel.AutoSize = true;
			this.topPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.topPanel.Margin = new Padding(0);
			
			//
			// ledControlLabel
			//
			this.ledControlLabel = new Label();
			this.ledControlLabel.Text = "LED Color:";
			this.ledControlLabel.AutoSize = true;
			this.ledControlLabel.Dock = DockStyle.Top;
			this.ledControlLabel.Font = this.headingFont;
			this.ledControlLabel.BorderStyle = BorderStyle.None;
			
			//
			// ledControlCombo
			//
			this.ledControlCombo = new ComboBox();
			this.ledControlCombo.DropDownStyle = ComboBoxStyle.DropDownList;
			foreach(string color in Enum.GetNames(typeof(KinectLED.ColorOption)))
			{
				this.ledControlCombo.Items.Add(color);
			}
			this.ledControlCombo.Dock = DockStyle.Top;
			this.ledControlCombo.Font = this.regularFont;
			this.ledControlCombo.SelectedIndexChanged += delegate(object sender, EventArgs e) {
				this.SetLED(this.ledControlCombo.SelectedItem.ToString());
			};
			
			//
			// motorControlLabel
			//
			this.motorControlLabel = new Label();
			this.motorControlLabel.Text = "Motor Tilt Angle:";
			this.motorControlLabel.AutoSize = true;
			this.motorControlLabel.Dock = DockStyle.Top;
			this.motorControlLabel.Font = this.headingFont;
			this.motorControlLabel.BorderStyle = BorderStyle.None;
			
			//
			// motorControlTrack
			//
			this.motorControlTrack = new TrackBar();
			this.motorControlTrack.Minimum = -100;
			this.motorControlTrack.Maximum = 100;
			this.motorControlTrack.Dock = DockStyle.Top;
			this.motorControlTrack.AutoSize = true;
			this.motorControlTrack.TickStyle = TickStyle.TopLeft;
			this.motorControlTrack.SmallChange = 1;
			this.motorControlTrack.LargeChange = 5;
			this.motorControlTrack.ValueChanged += delegate(object sender, EventArgs e) {
				this.SetMotorTilt(this.motorControlTrack.Value);
			};
			
			//
			// controlsPanel
			//
			this.controlsPanel = new Panel();
			this.controlsPanel.Width = 300;
			this.controlsPanel.Height = 200;
			this.controlsPanel.Margin = new Padding(0);
			this.controlsPanel.Padding = new Padding(7);
			this.controlsPanel.Controls.Add(this.motorControlTrack);
			this.controlsPanel.Controls.Add(this.motorControlLabel);
			this.controlsPanel.Controls.Add(this.ledControlCombo);
			this.controlsPanel.Controls.Add(this.ledControlLabel);
			
			//
			// motorTiltStatusLabel
			//
			this.motorTiltStatusLabel = new Label();
			this.motorTiltStatusLabel.Text = "Motor Tilt: ";
			this.motorTiltStatusLabel.AutoSize = true;
			this.motorTiltStatusLabel.Dock = DockStyle.Top;
			this.motorTiltStatusLabel.Font = this.headingFont;
			this.motorTiltStatusLabel.BorderStyle = BorderStyle.None;
			
			//
			// accelXLabel
			//
			this.accelXLabel = new Label();
			this.accelXLabel.Text = "Accel X: ";
			this.accelXLabel.AutoSize = true;
			this.accelXLabel.Dock = DockStyle.Top;
			this.accelXLabel.Font = this.headingFont;
			this.accelXLabel.BorderStyle = BorderStyle.None;
			
			//
			// accelYLabel
			//
			this.accelYLabel = new Label();
			this.accelYLabel.Text = "Accel Y: ";
			this.accelYLabel.AutoSize = true;
			this.accelYLabel.Dock = DockStyle.Top;
			this.accelYLabel.Font = this.headingFont;
			this.accelYLabel.BorderStyle = BorderStyle.None;
			
			//
			// accelZLabel
			//
			this.accelZLabel = new Label();
			this.accelZLabel.Text = "Accel Z: ";
			this.accelZLabel.AutoSize = true;
			this.accelZLabel.Dock = DockStyle.Top;
			this.accelZLabel.Font = this.headingFont;
			this.accelZLabel.BorderStyle = BorderStyle.None;
			
			//
			// infoPanel
			//
			this.infoPanel = new TableLayoutPanel();
			this.infoPanel.ColumnCount = 2;
			this.infoPanel.RowCount = 3;
			this.infoPanel.Width = 500;
			this.infoPanel.Height = 200;
			this.infoPanel.Margin = new Padding(0);
			this.infoPanel.Padding = new Padding(7);
			this.infoPanel.Controls.Add(this.accelZLabel, 1, 2);
			this.infoPanel.Controls.Add(this.accelYLabel, 1, 1);
			this.infoPanel.Controls.Add(this.accelXLabel, 1, 0);
			this.infoPanel.Controls.Add(this.motorTiltStatusLabel, 0, 0);
			this.infoPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50.0f));
			this.infoPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50.0f));
			
			//
			// bottomPanel
			//
			this.bottomPanel = new TableLayoutPanel();
			this.bottomPanel.BackColor = Color.White;
			this.bottomPanel.ColumnCount = 2;
			this.bottomPanel.RowCount = 1;
			this.bottomPanel.Controls.Add(this.infoPanel, 1, 0);
			this.bottomPanel.Controls.Add(this.controlsPanel, 0, 0);
			this.bottomPanel.AutoSize = true;
			this.bottomPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.bottomPanel.Margin = new Padding(0);
			this.bottomPanel.Enabled = false;
			
			//
			// mainLayoutPanel
			//
			this.mainLayoutPanel = new TableLayoutPanel();
			this.mainLayoutPanel.ColumnCount = 1;
			this.mainLayoutPanel.RowCount = 2;
			this.mainLayoutPanel.Controls.Add(this.topPanel, 0, 0);
			this.mainLayoutPanel.Controls.Add(this.bottomPanel, 0, 1);
			this.mainLayoutPanel.AutoSize = true;
			this.mainLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.mainLayoutPanel.Margin = new Padding(0);
			
			//
			// infoUpdateTimer
			//
			this.infoUpdateTimer = new Timer();
			this.infoUpdateTimer.Interval = 100;
			this.infoUpdateTimer.Enabled = false;
			this.infoUpdateTimer.Tick += delegate(object sender, EventArgs e) {
				this.UpdateInfoPanel();
			};
			
			//
			// MainForm
			//
			this.FormBorderStyle = FormBorderStyle.Fixed3D;
			this.Text = "Kinect.NET Wrapper Testing Interface";
			this.AutoSize = true;
			this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.BackColor = Color.White;
			this.Controls.Add(this.mainToolbar);
			this.Controls.Add(this.mainLayoutPanel);
			this.FormClosing += delegate(object sender, FormClosingEventArgs e) {
				Kinect.Shutdown();
			};
			
			// Update
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		
		/// <summary>
		/// Driver
		/// </summary>
		/// <param name="args">
		/// A <see cref="System.String[]"/>
		/// </param>
		public static void Main (string[] args)
		{
			Application.Run(new MainForm());
		}
		
		
		// GUI Elements
		private ToolStrip mainToolbar;
		private ToolStripComboBox kinectDeviceSelectCombo;
		private ToolStripButton connectButton;
		private ToolStripButton disconnectButton;
		private TableLayoutPanel topPanel;
		private Panel rgbPanel;
		private Panel depthPanel;
		private TableLayoutPanel bottomPanel;
		private Panel controlsPanel;
		private Label ledControlLabel;
		private ComboBox ledControlCombo;
		private Label motorControlLabel;
		private TrackBar motorControlTrack;
		private TableLayoutPanel infoPanel;
		private Label motorTiltStatusLabel;
		private Label accelXLabel;
		private Label accelYLabel;
		private Label accelZLabel;
		private TableLayoutPanel mainLayoutPanel;
		private Font headingFont;
		private Font regularFont;
		private Timer infoUpdateTimer;
	}
}