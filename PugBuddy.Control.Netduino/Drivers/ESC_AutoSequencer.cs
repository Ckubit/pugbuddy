/*
 * ESC Forward/Reverse/Brake Auto Sequencer
 *	Coded by Chris Seto August 2010
 *	<chris@chrisseto.com> 
 *	
 * Use this code for whatever you want. Modify it, redistribute it, I don't care.
 * I do ask that you please keep this header intact, however.
 * If you modfy the driver, please include your contribution below:
 * 
 * Chris Seto: Inital version (1.0)
 * Chris Seto: 1.1 release (Now a general purpose ESC abstraction layer)
 * 
 * */

using System.Threading;

namespace ElectricSpeedController_API
{
	class ESC_AutoSequencer
	{
		/// <summary>
		/// ESC handle
		/// </summary>
		private SpeedController esc;

		/// <summary>
		/// % Forward
		/// </summary>
		public int Forward
		{
			set
			{
				// Get into forward
				if (esc.DriveMode == SpeedController.DriveModes.Reverse)
					esc.DriveMode = SpeedController.DriveModes.Forward;

				// Set the throttle
				esc.Throttle = value;
			}
		}

		/// <summary>
		/// % Reverse
		/// </summary>
		public int Reverse
		{
			set
			{
				// Get into reverse
				if (esc.DriveMode == SpeedController.DriveModes.Forward)
					esc.DriveMode = SpeedController.DriveModes.Reverse;

				// Apply a little throttle (this will get us into brake mode)
				esc.Throttle = 10;
				Thread.Sleep(10);

				// Return to neutral
				esc.Throttle = 0;
				Thread.Sleep(10);

				// Now we are in reverse mode, set the throttle
				esc.Throttle = value;
			}
		}

		/// <summary>
		/// % Brake
		/// </summary>
		public int Brake
		{
			set
			{
				// Get into forward
				if (esc.DriveMode == SpeedController.DriveModes.Reverse)
					esc.DriveMode = SpeedController.DriveModes.Forward;

				// Apply a little throttle (this will get us into brake mode)
				esc.Throttle = 10;
				Thread.Sleep(10);

				// Immediately switch to reverse to get into brake mode
				esc.DriveMode = SpeedController.DriveModes.Reverse;

				// The throttle setting is now the brake setting
				esc.Throttle = value;

			}
		}

		/// <summary>
		/// Construct, get the ESC handle
		/// </summary>
		/// <param name="escHandle"></param>
		public ESC_AutoSequencer(SpeedController escHandle)
		{
			this.esc = escHandle;
		}
	}
}
