/*
 * Electric Speed Controller NETMF Driver
 *	Coded by Chris Seto August 2010
 *	<chris@chrisseto.com> 
 *	
 * Use this code for whatever you want. Modify it, redistribute it, I don't care.
 * I do ask that you please keep this header intact, however.
 * If you modfy the driver, please include your contribution below:
 * 
 * Chris Seto: Inital release (1.0)
 * Chris Seto: Second release (1.1)
 * Chris Seto: Third release (1.2)
 * Chris Seto: Fourth release (1.3) (Code cleanup, MAJOR overhaul)
 * Chris Seto: Netduino port (1.3 -> Netduino)
 * 
 * */

using System;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using System.Threading;

namespace ElectricSpeedController_API
{
	// This is the parent class that will hold speed controller settings
	public abstract class SpeedControllers
	{
		// Timing range
		public abstract int[] range
		{
			get;
		}

		// How long to wait after setup to exit constructor
		public abstract int armPeriod
		{
			get;
		}

		// Ramp iteration timings
		public abstract int rampIterationPause
		{
			get;
		}
	}


	// This is the actual driver
	public class SpeedController
	{
		/// <summary>
		/// PWM handle
		/// </summary>
		private PWM esc;
		
		/// <summary>
		/// Timing ranges
		/// </summary>
		public int[] range = new int[3];
		
		/// <summary>
		/// Invert power setting
		/// </summary>
		public bool inverted = false;

		/// <summary>
		/// ESC model settings
		/// </summary>
		readonly SpeedControllers escModel;

		/// <summary>
		/// ESC drive modes
		/// </summary>
		public enum DriveModes
		{
			Reverse,
			Forward,
		}

		/// <summary>
		/// Active drive mode
		/// </summary>
		public DriveModes DriveMode = DriveModes.Forward;

		/// <summary>
		/// Last known throttle setting
		/// This is used internally and can be a positive or negative int
		/// referring to the actual throttle setting
		/// </summary>
		private int currentThrottle = 0;

		/// <summary>
		/// 0-100% throttle setting
		/// </summary>
		public int Throttle
		{
			set
			{
				// Range checks
				if (value > 100)
					value = 100;

				if (value < 0)
					value = 0;

				// Are we inverted?
				if (inverted)
					value = 100 - value;

				// If we are going reverse, invert the throttle
				if (DriveMode == DriveModes.Reverse)
					value = value * -1;

				// Ramp the signal.
				// I hate doing it this way, but I don't see any better option
				if (value < currentThrottle)
				{
					// Ramping down
					for (int set = currentThrottle; set >= value; set--)
					{
						esc.SetPulse(20000, (uint)map(set, -100, 100, escModel.range[0], escModel.range[1]));
						Thread.Sleep(escModel.rampIterationPause);
					}
				}
				else if (value > currentThrottle)
				{
					// Ramping up
					for (int set = currentThrottle; set <= value; set++)
					{
						esc.SetPulse(20000, (uint)map(set, -100, 100, escModel.range[0], escModel.range[1]));
						Thread.Sleep(escModel.rampIterationPause);
					}
				}

				// Set the throttle level
				currentThrottle = value;
			}

			get
			{
				// Get the absolute value
				if (currentThrottle < 0)
					return currentThrottle * -1;
				else
					return currentThrottle;

			}
		}

		/// <summary>
		/// Create the PWM pin, set it low and configure 
		/// timing settings
		/// </summary>
		/// <param name="pin"></param>
		public SpeedController(Cpu.Pin pin, SpeedControllers escModel)
		{
			// Set up the PWM port
			esc = new PWM((Cpu.Pin)pin);

			// Bind the ESC settings
			this.escModel = escModel;

			// Pull the throttle all the way out
			Throttle = 0;

			// Arm the ESC
			Thread.Sleep(escModel.armPeriod);
		}

		public void Dispose()
		{
			this.disengage();
			esc.Dispose();
		}

		/// <summary>
		/// Disengage ESC.
		/// Behavior is dependant on ESC make and model.
		/// </summary>
		public void disengage()
		{
			esc.SetDutyCycle(0);
		}

		/// <summary>
		/// Used internally to map a percentage to a timing range
		/// </summary>
		/// <param name="x"></param>
		/// <param name="in_min"></param>
		/// <param name="in_max"></param>
		/// <param name="out_min"></param>
		/// <param name="out_max"></param>
		/// <returns></returns>
		private long map(int x, int in_min, int in_max, int out_min, int out_max)
		{
			return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
		}
	}
}
