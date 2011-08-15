/*
 * Traxxas XL5 Speed Controller settings
 *	Coded by Chris Seto August 2010
 *	<chris@chrisseto.com> 
 *	
 * Use this code for whatever you want. Modify it, redistribute it, I don't care.
 * I do ask that you please keep this header intact, however.
 * If you modfy the driver, please include your contribution below:
 * 
 * Chris Seto: Inital release (1.0)
 * Chris Seto: Bugfix: ramp iteration timings (1.1)
 * 
 * */

namespace ElectricSpeedController_API
{
	public class TRAXXAS_XL5 : SpeedControllers
	{
		/// <summary>
		/// Wave timings
		/// </summary>
		public override int[] range
		{
			get
			{
				return new int[2]
				{
					1000,
					2000,
				};
			}
		}


		/// <summary>
		/// how long to let the ESC sit after init
		/// </summary>
		public override int armPeriod
		{
			get
			{
				return 500;
			}
		}

		/// <summary>
		/// How long to wait between ramp iterations
		/// </summary>
		public override int rampIterationPause
		{
			get
			{
				return 1;
			}
		}
		
	}


}