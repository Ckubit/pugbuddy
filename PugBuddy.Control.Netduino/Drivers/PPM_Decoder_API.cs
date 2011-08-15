/*
 *	PPM Decoder API
 *	Coded by Chris Seto August 2010
 *	<chris@chrisseto.com> 
 *	
 * Use this code for whatever you want. Modify it, redistribute it, I don't care.
 * I do ask that you please keep this header intact, however.
 * If you modfy the driver, please include your contribution below:
 * 
 * Chris Seto: Inital release (1.0)
 * Chris Seto: Netduino port (1.0 -> Netduino branch)
 * 
 * */

using System;
using Microsoft.SPOT.Hardware;

namespace PPM_Decoder_API
{
	class PPM_Channel
	{
		/// <summary>
		/// Input port handle
		/// </summary>
		private InterruptPort inputHandle;

		/// <summary>
		/// Timing settings for the input wave. These are mostly standard
		/// </summary>
		private int[] inputRange = new int[2]
		{
			1000,
			2000,
		};

		/// <summary>
		/// Range settings for the output
		/// </summary>
		private int[] outputRange = new int[2]
		{
			-100,
			100,
		};

		/// <summary>
		/// Raw, unprocessed  
		/// </summary>
		public int internalRead = 0;

		/// <summary>
		/// The time the pulse went high
		/// </summary>
		private long ticks;

		// Get the PPM reading. -100 to 100 inclusive
		public int Read
		{
			get
			{
				// Get the time the signal is high and map it
				 int reading = (int)map((internalRead / 10), inputRange[0], inputRange[1], outputRange[0], outputRange[1]);

				// Range checks
				if (reading < outputRange[0])
					 reading = outputRange[0];

				if (reading > outputRange[1])
					 reading = outputRange[1];

				return reading;
			}
		}

		/// <summary>
		/// Init the PPM decoder
		/// </summary>
		/// <param name="inputPin"></param>
		public PPM_Channel(Cpu.Pin inputPin)
		{
			// Setup the interrupt
			inputHandle = new InterruptPort((Cpu.Pin)inputPin, false, Port.ResistorMode.Disabled, Port.InterruptMode.InterruptEdgeBoth);
			inputHandle.OnInterrupt += new NativeEventHandler(inputHandle_OnInterrupt);
		}

		/// <summary>
		/// Handle the interrupt
		/// </summary>
		/// <param name="data1"></param>
		/// <param name="state"></param>
		/// <param name="time"></param>
		void inputHandle_OnInterrupt(uint data1, uint state, DateTime time)
		{
			// If state == 0, then we are at the end of the pulse,
			// so we calulate the time diff. If not, we are at the start of the pulse, 
			// so we just mark the start time
			if (state == 0)
				internalRead = (int)(time.Ticks - ticks);
			else
				ticks = time.Ticks;
		}

		/// <summary>
		/// Allow for adjustment of the timing settings
		/// </summary>
		/// <param name="negative"></param>
		/// <param name="neutral"></param>
		/// <param name="positive"></param>
		public void calibrateInput(int negative, int positive)
		{
			inputRange[0] = negative;
			inputRange[1] = positive;
		}

		/// <summary>
		/// Allow for output calibration
		/// </summary>
		/// <param name="negative"></param>
		/// <param name="positive"></param>
		public void calibrateOutput(int negative, int positive)
		{
			outputRange[0] = negative;
			outputRange[1] = positive;
		}

		/// <summary>
		/// Used internally to map a number from one scale to another
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
