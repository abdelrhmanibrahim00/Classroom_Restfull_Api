using ClassLibrary;
using NLog;
using System;
using System.Collections.Generic;
using System.Threading;

namespace TeacherClient
{
    /// <summary>
    /// Represents the client logic for the Teacher component in the classroom simulation.
    /// Responsible for voting on class start and end based on the server's state.
    /// </summary>
    class TeacherClient
    {
        /// <summary>
        /// Service client for interacting with the Classroom server's teacher-related endpoints.
        /// </summary>
        private readonly TeacherServiceClient _serviceClient;

        /// <summary>
        /// Random number generator for voting decisions.
        /// </summary>
        private static readonly Random random = new Random();

        /// <summary>
        /// Logger instance for logging teacher actions and debugging.
        /// </summary>
        private readonly Logger mLog = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The Teacher object representing the current teacher.
        /// </summary>
        private Teacher teacher;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeacherClient"/> class, configuring logging and initializing the teacher.
        /// </summary>
        /// <param name="serviceClient">An instance of <see cref="TeacherServiceClient"/> for server communication.</param>
        public TeacherClient(TeacherServiceClient serviceClient)
        {
            _serviceClient = serviceClient;
            ConfigureLogging();
            teacher = new Teacher
            {
                TeacherId = 0,
                HasVotedToStart = false,
                HasVotedToEnd = false,
                Name = GetRandomName()
            };
        }

        /// <summary>
        /// Configures logging settings for the application, including console output.
        /// </summary>
        private void ConfigureLogging()
        {
            var config = new NLog.Config.LoggingConfiguration();
            var console = new NLog.Targets.ConsoleTarget("console")
            {
                Layout = @"${date:format=HH\:mm\:ss}|${level}| ${message} ${exception}"
            };
            config.AddTarget(console);
            config.AddRuleForAllLevels(console);
            LogManager.Configuration = config;
        }

        /// <summary>
        /// Generates a random name for the teacher from a predefined list.
        /// </summary>
        /// <returns>A randomly selected name as a string.</returns>
        private static string GetRandomName()
        {
            var names = new List<string> { "Alice", "Bob", "Charlie", "Diana", "Eve" };
            return names[random.Next(names.Count)];
        }

        /// <summary>
        /// Main loop for the teacher client, managing voting on class start and end based on server state.
        /// </summary>
        public void Run()
        {
            while (true)
            {
                try
                {
                    // Assign a unique ID to the teacher if not yet assigned
                    if (teacher.TeacherId == 0)
                    {
                        teacher.TeacherId = _serviceClient.GetUniqueId();
                        mLog.Info($"The teacher {teacher.Name} has been assigned ID {teacher.TeacherId}.");
                    }

                    // Check if class session is active and vote accordingly
                    if (!_serviceClient.Status())
                    {
                        mLog.Info("Checking if there are enough students to start...");
                        if (_serviceClient.Enoughstudents())
                        {
                            mLog.Info("Starting voting to begin the class...");
                            StartTeacherVoting(teacher);
                            _serviceClient.Start(teacher);
                            mLog.Info("Vote to start the class sent.");
                        }
                    }
                    else
                    {
                        mLog.Info("Starting voting to end the class...");
                        EndTeacherVoting(teacher);
                        _serviceClient.End(teacher);
                        mLog.Info("Vote to end the class sent.");
                    }

                    Thread.Sleep(5000);  // Simulate class duration
                }
                catch (Exception e)
                {
                    mLog.Warn(e, "Unhandled exception caught. Retrying...");
                    Thread.Sleep(2000);
                }
            }
        }

        /// <summary>
        /// Assigns a random vote for starting the class to the teacher and logs the vote.
        /// </summary>
        /// <param name="teacher">The teacher object casting the vote.</param>
        private void StartTeacherVoting(Teacher teacher)
        {
            teacher.HasVotedToStart = random.NextDouble() >= 0.6;
            Thread.Sleep(2000);
            mLog.Info($"Teacher {teacher.Name} voted to start: {teacher.HasVotedToStart}");
        }

        /// <summary>
        /// Assigns a random vote for ending the class to the teacher and logs the vote.
        /// </summary>
        /// <param name="teacher">The teacher object casting the vote.</param>
        private void EndTeacherVoting(Teacher teacher)
        {
            teacher.HasVotedToEnd = random.NextDouble() >= 0.6;
            Thread.Sleep(2000);

            mLog.Info($"Teacher {teacher.Name} voted to end: {teacher.HasVotedToEnd}");
        }

        /// <summary>
        /// The entry point for the Teacher client program.
        /// Initializes the Teacher client and starts the main execution loop.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            var httpClient = new HttpClient();
            var serviceClient = new TeacherServiceClient("http://127.0.0.1:5000", httpClient);
            var teacherClient = new TeacherClient(serviceClient);
            teacherClient.Run();
        }
    }
}
