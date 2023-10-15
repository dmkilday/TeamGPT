TeamGPT is a simulation of a team of humans that are able to work together to complete a given objective. Each team member has a given persona and OpenAI API is used to represent the brain of each team member.

Ultimately, the vision is that the user can give an objective to TeamGPT and the program will determine the optimal team composition, generate the team, and have the team work in parallel (each human on their own thread) to complete the associated tasks.

To run this locally, pull the project down into VSCode and copy the appsettings.template.json file to appsettings.json. Then put your OpenAI API key in the ApiKey configuration variable in appsettings.json.

Requirements:

1. .NET SDK: If you haven't already installed it, you will need to download and install the .NET SDK 7.0. This can be downloaded at https://dotnet.microsoft.com/en-us/download/dotnet/7.0.

2. Extensions: If you are using VSCode, when you bring down the project and open you may be prompted to install the recommended extensions. These are found in ./vscode/extensions.json. Go ahead and install these if you are using VSCode. The extensions I have installed are the following (not sure if they are all required).
- .NET Runtime Install Tool Extension
- C# Extension
- C# Dev Kit Extension

3. Packages: After cloning the repo for the first time you will need to run "dotnet restore" in the Terminal to download and install the required packages. These packages are referenced in the TeamGPT.csproj file and should download and install automatically when you run "dotnet restore".

How to Run:

Once you have fulfulled the requirements, you should be able to build and debug/run the program. The entry point for the program is the main method in TeamGPT.cs. In the Terminal go to the root directory of the project and run "dotnet build" to build the program. Then you can enter a debug session (set a breakpoint in TeamGPT.cs and hit F5) or type "dotnet run" to execute the program outside of debug.