using ClassLibrary;
//using Microsoft.Extensions.DependencyInjection;
using NLog;
using System;

namespace DoorClient
{
    /// <summary>
    /// Represents the client logic for the Door component in the classroom simulation.
    /// Responsible for managing student generation and sending updates to the server.
    /// </summary>
    public class DoorClient
    {
        /// <summary>
        /// Instance of <see cref="DoorServiceClient"/> to interact with the Classroom server.
        /// </summary>
        private readonly ClientDoor _classClient;

        /// <summary>
        /// Random number generator used for generating random student counts.
        /// </summary>
        private readonly Random _rnd;

        /// <summary>
        /// Logger instance for logging information and debugging.
        /// </summary>
        private Logger _mLog = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="DoorClient"/> class, configuring logging and initializing server interaction.
        /// </summary>
        public DoorClient()
        {
            var config = new NLog.Config.LoggingConfiguration();

            // Configure console and file targets for logging
            var logConsole = new NLog.Targets.ConsoleTarget("logConsole");
            var logFile = new NLog.Targets.FileTarget("logFile") { FileName = "file.txt" };

            // Define logging rules
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logConsole);
            config.AddRule(LogLevel.Info, LogLevel.Fatal, logFile);

            // Apply the logging configuration
            NLog.LogManager.Configuration = config;

            // Initialize logger instance
            _mLog = LogManager.GetCurrentClassLogger();

            // Initialize ClassClient to communicate with the Classroom server
            _classClient = new ClientDoor("http://127.0.0.1:5000", new HttpClient());
            _rnd = new Random();
        }

        /// <summary>
        /// Main logic for the Door client. Generates students randomly and sends updates to the server at regular intervals.
        /// </summary>
        public void Run()
        {
           // var door = new Door { 1, 0, false, true, "Main entrance door" };
            var door = new Door() { DoorId=1,AmountOfStudents= 0, IsClosed=false, IsOpened=true,Name= "Main entrance door" };

            _mLog.Info($"Door {door.DoorId} initialized. Description: {door.Name}.");

            while (true)
            {
                try
                {
                    // Check the current session state from the server
                    bool isClassInSession = _classClient.Status();
                    if (!isClassInSession)
                    {
                        // Generate random number of students if class is not in session
                        int arrivingStudents = _rnd.Next(-6, 10);
                        door.AmountOfStudents += arrivingStudents; // Update door's student count
                        _mLog.Info($"{arrivingStudents} students have been generated at Door {door.DoorId}. Total: {door.AmountOfStudents}");

                        // Send the updated door data to the server
                        _classClient.Generate(door);
                        _mLog.Info($"Generated students have been sent to the server. Total students at Door {door.DoorId}: {door.AmountOfStudents}");
                    }
                    else
                    {
                        _mLog.Info($"Door {door.DoorId} is closed. No students can enter.");
                    }

                    // Wait a random interval before repeating the loop
                    System.Threading.Thread.Sleep(_rnd.Next(1000, 3000));
                }
                catch (Exception e)
                {
                    // Log any exceptions and delay before retrying
                    _mLog.Warn(e, "Unhandled exception caught. Will restart main loop.");
                    System.Threading.Thread.Sleep(2000);
                }
            }
        }

        /// <summary>
        /// The entry point for the Door client program.
        /// Initializes the Door client and starts the main execution loop.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            var doorClient = new DoorClient();
            doorClient.Run();
        }
    }
}
