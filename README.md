Classroom - API

A Classroom is implemented in C# using .NET Core. This project simulates a classroom environment with components like Teacher and Door clients. Each component interacts with a central server via RESTful APIs, managing the classroom session, student counts, and voting mechanisms to start or end the class.
Features

    Teacher Client: Allows teachers to vote to start or end a class.
    Door Client: Manages student entry, sends student count data to the server, and checks the class session status.
    Server: Handles API requests from clients and processes votes and student data.

Technologies

    C# - Main programming language.
    .NET Core - Framework used to build the server and clients.
    NSwag - Used to generate client classes automatically from Swagger documentation.
    Swagger - Provides API documentation and enables client code generation.
    NLog - Logging library for structured logging throughout the application.

Project Structure

    Server
        Controllers/ - Manages API endpoints for classroom session, voting, and student count.
        Logic/ - Contains classroom logic, voting handling, and student count management.
        Startup.cs - Configures services and middleware.

    Clients
        TeacherClient - Implements logic for teacher votes and interactions with the server.
        DoorClient - Manages student data and interacts with the server.
        Generated Client Classes - Auto-generated client classes using NSwag to communicate with the server API.

Getting Started
Prerequisites

    .NET Core SDK
    NSwag CLI - For generating client code from Swagger documentation.
    Visual Studio - For development and building the project.

Installation

    Clone the Repository

git clone https://github.com/yourusername/ClassroomManagement.git
cd ClassroomManagement

Run NSwag to Generate Client Classes Ensure the server is running locally and accessible. Then, in the terminal, run:

nswag run nswag.json

Build the Solution Open the solution in Visual Studio or use the .NET CLI:

    dotnet build

Running the Application

    Start the Server

cd ClassroomServer
dotnet run

Run the Clients

    In separate terminals, start each client:
        Teacher Client

cd TeacherClient
dotnet run

Door Client

            cd DoorClient
            dotnet run

Usage

    The Teacher Client interacts with the server to submit votes to start or end a class.
    The Door Client sends student counts to the server and checks if the class session is active.
    Log information is available in the console or log files (configured with NLog).

API Endpoints

    GET /api/classroom/status - Checks if the class session is active.
    POST /api/classroom/start - Casts a vote to start the class.
    POST /api/classroom/end - Casts a vote to end the class.
    POST /api/door/generate - Sends student count data from the door.

Troubleshooting

For any issues with client-server connectivity, verify that:

    The server is running on localhost:5000.
    NSwag client classes match the server API endpoints.
    Firewall or network restrictions are not blocking communication.

License

This project is licensed under the MIT License. See LICENSE for more details.
