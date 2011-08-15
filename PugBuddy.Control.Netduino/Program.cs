using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;
using SecretLabs.NETMF.Hardware.Netduino;
using System.IO;

//RC
using ElectricSpeedController_API;
//using PPM_Decoder_API;
using Servo_API;

//using NetduinoLibrary.Actuators;

namespace PugBuddy.Control.Netduino
{
    public class Program
    {
        //Webserver constant
            const string WebFolder = "\\SD\\Web";

        //RC constants
            /// <summary>
            /// Steering servo
            /// (Traxxas 2056)
            /// </summary>
            private static Servo steering;

            /// <summary>
            /// ESC
            /// (Traxxas XL-5)
            /// </summary>
            private static SpeedController xl5;

            /// <summary>
            /// ESC speed limit for the RC control loop
            /// </summary>
            private const int speedLimit = 100;


        //END RC constants

        public static void Main()
        {

            //RC stuff

                // Setup steering servo
                steering = new Servo(Pins.GPIO_PIN_D9);

                // Setup ESC
                xl5 = new SpeedController(Pins.GPIO_PIN_D10, new TRAXXAS_XL5());
                xl5.DriveMode = SpeedController.DriveModes.Forward;

            //End RC stuff


            Listener webServer = new Listener(RequestReceived);
 
            OutputPort led = new OutputPort(Pins.ONBOARD_LED, false);
            while (true)
            {
                // Blink LED to show we're still responsive
                led.Write(!led.Read());
                Thread.Sleep(500);
            }

        }

        private static bool Navigate(string forwardReverse, int time, int distance, int speed, string direction, int degrees)
        {
            //// Sweep the wheels from left to right
            //for (int i = 0; i <= 30; i++)
            //{
            //    steering.Degree = i * 6;
            //    Thread.Sleep(30);
            //}

            //// Center steering
            //steering.Degree = 90.5;


            //// Turn Left really fast
            //for (int i = 0; i <= 15; i++)
            //{
            //    steering.Degree = i * 10;
            //    Thread.Sleep(30);
            //}


            // Center steering
            steering.Degree = 90.5;
           



            #region notes
                // A really burst with a left turn - forward&200&20&Left&0

                // Short fast reverse to the right - http://192.168.1.12:8080/reverse&100&20&Right&150&5

                // Short fast forward to the right - http://192.168.1.12:8080/forward&100&20&Right&150&5

            #endregion 


            // Turn steering wheel in directions and the degrees needed
            //
            // LEFT  - Between 0 - 90.5 degrees
            // RIGHT - Between 90.5 - 180 degrees
            if (direction == "Left")
            {
                // If out of range for direction, adjust to work
                if (degrees > 91 || degrees < 0) { degrees = 45; }
            }
            if (direction == "Right")
            {
                // If out of range for direction, adjust to work
                if (degrees < 90 || degrees > 180) { degrees = 120; }
            }
            steering.Degree = degrees;

            if (forwardReverse.ToLower() == "/forward")
            {
                xl5.DriveMode = SpeedController.DriveModes.Forward;
            }
            else if (forwardReverse.ToLower() == "/reverse")
            {
                xl5.DriveMode = SpeedController.DriveModes.Reverse;
            }

            int throttle = 0;
            
            for (int j = 20; j <= distance; j++)
            {
                throttle = j;
                if (j > speedLimit) { throttle = speedLimit; }  // speed limit
                xl5.Throttle = throttle;
                if (time <= 30) { time = 30; }  // 30 is min
                Thread.Sleep(time);
            }


            // Center steering
            steering.Degree = 90.5;
            // Return throttle to stop
            xl5.Throttle = 0;

            return true;


        }

        private static void RequestReceived(Request request)
        {
            // Use this for a really basic check that it's working
            //request.SendResponse("<html><body><p>Request from " + request.Client.ToString() + " received at " + DateTime.Now.ToString() + "</p><p>Method: " + request.Method + "<br />URL: " + request.URL +"</p></body></html>");

            //Break apart the request
            //request.URL.ToString()
            string[] commands = request.URL.Split('&');
            try
            {
                int cmdCount = 0;
                foreach (string c in commands)
                {
                    cmdCount++;
                }
                if (cmdCount != 6){request.SendResponse("<html><body>Invalid Number of Commands</body></html>");}
            }
            catch (Exception ex)
            {
                request.SendResponse("<html><body>" + ex + "</body></html>");
            }


            try
            {
                string direction = commands[0];
                int t = Int32.Parse(commands[1]);
                int d = Int32.Parse(commands[2]);
                string dir = commands[3].ToString();
                int deg = Int32.Parse(commands[4]);
                int spe = Int32.Parse(commands[5]);


                // sent to output
                if (direction.ToLower() == "/forward" || direction.ToLower() == "/reverse")
                {
                    //TrySendFile(request);
                    //OutputPort D0 = new OutputPort(Pins.GPIO_PIN_D0, false);
                    //D0.Write(true);

                    if (Navigate(direction, t, d, spe, dir, deg))
                    {
                        request.SendResponse("<html><body><p>FORWARD<BR><BR>Request from " + request.Client.ToString() + " received at " + DateTime.Now.ToString() + "</p><p>Method: " + request.Method + "<br />URL: " + request.URL + "</p></body></html>");
                    }
                    else { request.Send404(); }

                }
                //else if (direction == "/backward" || direction == "/test2.html") 
                //{


                //    //xl5.DriveMode = SpeedController.DriveModes.Reverse;
                //    //for (int j = 50; j >= 0; j--)
                //    //{
                //    //    xl5.Throttle = j;
                //    //    Thread.Sleep(30);
                //    //}


                //    request.SendResponse("<html><body><p>BACKWARD<BR><BR>Request from " + request.Client.ToString() + " received at " + DateTime.Now.ToString() + "</p><p>Method: " + request.Method + "<br />URL: " + request.URL + "</p></body></html>");

                //}
                else
                {
                    request.SendResponse("<html><body><p>UNKNOWN COMMAND<BR><BR>Request from " + request.Client.ToString() + " received at " + DateTime.Now.ToString() + "</p><p>Method: " + request.Method + "<br />URL: " + request.URL + "</p></body></html>");
                }


                // Send a file
                //TrySendFile(request);
            }
            catch (Exception ex)
            {
                request.SendResponse("<html><body>" + ex + "</body></html>");
            }
        }

        /// <summary>
        /// Look for a file on the SD card and send it back if it exists
        /// </summary>
        /// <param name="request"></param>
        private static void TrySendFile(Request request)
        {
            // Replace / with \
            string filePath = WebFolder + request.URL.Replace('/', '\\');

            if (File.Exists(filePath))
                request.SendFile(filePath);
            else
                request.Send404();
        }

    }
}
