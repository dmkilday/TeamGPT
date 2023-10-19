// using Serilog;

// using TeamGPT.Utilities;
// using TeamGPT.Tasks;
// using System.Dynamic;

// namespace TeamGPT.Models
// {
//     public class Human_legacy
//     {
//         private readonly ApplicationSettings _settings;
//         private readonly Logger _logger;
//         private readonly string _logFilePath; // Store the log file path
//         public string Name { get; private set; } 
//         private Brain Brain;
//         public Team Team { get; private set; }
//         public List<Objective> Objectives { get; private set; }
//         public Tasks.Objective? CurrentObjective { get; private set; }

//         public Human(ApplicationSettings settings, string name, Persona persona)
//         {
//             this._settings = settings;
//             this._logger = settings.LoggerInstance;

//             // Store the log file path named after the human & configure their logger
//             _logFilePath = $"logs/{name}.log";
//             _logger.ConfigureLogFile(_logFilePath);

//             // Set remaining properties
//             this.Brain = new(_settings, this, persona);
//             this.Name = name;
//             this.Objectives = new();
            
//             _logger.Log(Logger.CustomLogLevel.Information, Name, "I'm alive and reporting for duty!");
//         }

//         public Persona? GetPersona()
//         {
//             return this.Brain.Persona;
//         }

//         public void Assign(Objective objective, Human assignee)
//         {
//             assignee.ReceiveAssignment(objective);
//             assignee.Work(); // Manually do this for now, but will use a timer in the future.
//         }

//         public Human GetAssignee(Objective objective)
//         {
//             // TODO: Get assignee from chat GPT.
//             Random random = new Random();
//             int maxMemberIndex = this.Team.Members.Count - 1;
//             int randomIndex = random.Next(0, maxMemberIndex);  // Randomly selects a member index
//             Human assignee = Team.Members[randomIndex];
//             return assignee;
//         }

//         public Objective GetWorkableObjective()
//         {
//             Objective objective = null;

//             // Check if the list is not empty before attempting to find the highest priority objective
//             if (this.Objectives.Count > 0)
//             {
//                 // Use LINQ to order the list by Priority in descending order
//                 objective = this.Objectives.OrderByDescending(obj => obj.Priority).First();
//                 this._logger.Log(Logger.CustomLogLevel.Information, this.Name, $"I grabbed objective '{objective.Goal}' from my to-do list.");
//             }
//             else
//             {
//                 // Handle the case where the list is empty
//                 this._logger.Log(Logger.CustomLogLevel.Information, this.Name, "I currently have no objectives in my to-do list.");
//             }

//             return objective;
//         } 

//         public void ReceiveAssignment(Objective objective)
//         {
//             // Add to my to-do list of objectives
//             this.Objectives.Add(objective);
//             objective.Assign(this); // Also, assign the objective to me (bi-directional reference)
            
//             // Now that I have an objective to work towards, I can plan my work to meet the objective.
//             // Brain.Decide(Goal) -> OAI.DecideWorkType() -> Returns Type of Work

//             // Not that we have a work plan we can have WorkManager create the work (WorkManager.Work()), then can Work.Handle().
//             // Work.Handle() will execute the work on the Human thread. 

//             // The objective is in my to-do list now, so the next step is to determine what to do.
//             // Here are my options.
//             // 1. Decompose: This objective is too high-level for any one person to complete, so I need to decompose it. 
//             // 2. Wait: I can't work on this because there is an external start dependency (deducable through logic). Go to next.
//             // 3. Do (Individual): I can do this task on my own. 
//             // 4. Do (Collaborative): I can do this task, but I need input from others (1 or more).
//             // Note: After completing the objective, submit to assigner for feedback. (need InReview status)

//             // Set the current objective I'm working on
//             this.CurrentObjective = objective;

//             this._logger.Log(Logger.CustomLogLevel.Information, this.Name, $"I have received the objective '{objective.Goal}'");

//             this.CurrentObjective.IsDecomposable = this.DetermineDecomposability(objective);
//             if (this.CurrentObjective.IsDecomposable != null)
//             {
//                 // If my objective is decomposable, then identify sub-objectives and assign them
//                 if (this.CurrentObjective.IsDecomposable == true)
//                 {
//                     this._logger.Log(Logger.CustomLogLevel.Information, this.Name, $"I am decomposing the objective '{objective.Goal}'");

//                     // Generate sub objectives
//                     List<Objective> sub_objectives = this.Decompose(objective);

//                     // Assign the sub objectives
//                     foreach (Objective sub_objective in sub_objectives)
//                     {
//                         this._logger.Log(Logger.CustomLogLevel.Information, this.Name, $"For objective '{objective.Goal}', I created sub-objective '{sub_objective.Goal}'");

//                         // Determine who the assignee should be
//                         Human assignee = GetAssignee(sub_objective);

//                         // Assign the objective
//                         this.Assign(sub_objective, assignee);

//                         this._logger.Log(Logger.CustomLogLevel.Information, this.Name, $"Assigned sub-objective '{sub_objective.Goal}' to {assignee.Name}");
//                     }
//                 }
//             }
//         }

//         public bool? DetermineDecomposability(Objective objective)
//         {
//             return false;
//         }

//         public List<Objective> Decompose(Objective objective)
//         {
//             // Create empty list of objectives
//             List<Objective> sub_objectives = new();
            
//             // Stub out the list for now
//             // TODO: Call ChatGPT to get list.
//             sub_objectives.Add(new Objective(this._settings, objective, this, "Sub Objective #1"));
//             sub_objectives.Add(new Objective(this._settings, objective, this, "Sub Objective #2"));
//             sub_objectives.Add(new Objective(this._settings, objective, this, "Sub Objective #3"));
//             return sub_objectives;
//         }

//         // Initiate process to work on the current objective 
//         private void Work()
//         {
//             if (this.CurrentObjective != null)
//             {
//                 if(!this.CurrentObjective.IsAchieved)
//                 {
//                     this.Do(CurrentObjective);
//                 }
//             }
//         }

//         private void Do(Tasks.Objective objective)
//         {
//             // Set the current activity I'm working on
//             this.CurrentObjective = objective;

//             // Updatedthe objective status to in-progress
//             objective.Activate();
            
//             // Query my brain
//             this._logger.Log(Logger.CustomLogLevel.Information, this.Name, $"Starting work on objective '{objective.Goal}'...");
//             string response = this.Brain.Think(objective.Goal).Result;

//             // Set outcome of objective
//             objective.Complete(response);
//             this._logger.Log(Logger.CustomLogLevel.Information, this.Name, $"Completed objective '{objective.Goal}'.");
//             this._logger.Log(Logger.CustomLogLevel.Information, this.Name, $"Objective outcome was '{objective.Outcome}'.");

//             // Clear current activity
//             this.CurrentObjective = null;
//         }

//         public void JoinTeam(Team team)
//         {
//             this.Team = team;
//             this.Team.AddMember(this);
//         }

//         public async Task<Team> DefineTeam(Objective objective)
//         {
//             Team team = await this.Brain.DefineTeam(objective);
//             return team;
//         }

//         public override string ToString()
//         {
//             string background = this.Brain.Persona.Background == null ? "" : string.Join(", ", this.Brain.Persona.Background);
//             string proclivities = this.Brain.Persona.Proclivities == null ? "" : string.Join(", ", this.Brain.Persona.Proclivities);
//             string knowlegdeDomains = this.Brain.Persona.KnowledgeDomains == null ? "" : string.Join(", ", this.Brain.Persona.KnowledgeDomains);
//             string skills = this.Brain.Persona.Skills == null ? "" : string.Join(", ", this.Brain.Persona.Skills);
//             return $"{this.Name}:\n" +
//                     $"\tBackground: {background}\n" +
//                     $"\tProclivities: {proclivities}\n" +
//                     $"\tKnowledge Domains: {knowlegdeDomains}\n" +
//                     $"\tSkills: {skills}";
//         }
//     }
// }
