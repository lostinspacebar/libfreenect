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
			this.kinectDeviceSelectCombo.Width = 200;
			
			//
			// connectButton
			//
			this.connectButton = new ToolStripButton();
			this.connectButton.Text = "Connect";
			this.connectButton.Width = 150;
			
			//
			// disconnectButton
			//
			this.disconnectButton = new ToolStripButton();
			this.disconnectButton.Text = "Disconnect";
			this.disconnectButton.Width = 150;
			this.disconnectButton.Enabled = false;
			
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
			this.motorControlTrack.Minimum = -31;
			this.motorControlTrack.Maximum = 31;
			this.motorControlTrack.Dock = DockStyle.Top;
			this.motorControlTrack.AutoSize = true;
			this.motorControlTrack.TickStyle = TickStyle.TopLeft;
			this.motorControlTrack.SmallChange = 1;
			this.motorControlTrack.LargeChange = 5;
			
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
			// infoPanel
			//
			this.infoPanel = new Panel();
			this.infoPanel.Width = 400;
			this.infoPanel.Height = 200;
			this.infoPanel.Margin = new Padding(0);
			
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
			// MainForm
			//
			this.FormBorderStyle = FormBorderStyle.Fixed3D;
			this.Text = "Kinect.NET Wrapper Testing Interface";
			this.AutoSize = true;
			this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			this.BackColor = Color.White;
			
			this.Controls.Add(this.mainLayoutPanel);
			
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
		private Panel infoPanel;
		private TableLayoutPanel mainLayoutPanel;
		private Font headingFont;
		private Font regularFont;
	}
}