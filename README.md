TeamGPT is a simulation of a team of humans that are able to work together to complete a given objective. Each team member has a given persona and OpenAI API is used to represent the brain of each team member.

Ultimately, the vision is that the user can give an objective to TeamGPT and the program will determine the optimal team composition, generate the team, and have the team work in parallel (each human on their own thread) to complete the associated tasks.

To run this locally, pull the project down into VSCode and copy the appsettings.template.json file to appsettings.json. Then put your OpenAI API key in the ApiKey configuration variable in appsettings.json.