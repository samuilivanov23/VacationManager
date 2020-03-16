using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using VacationManager.Data;
using VacationManager.Data.Models;
using VacationManager.Services.Interfaces;
using VacationManager.ViewModels;

namespace VacationManager.Services
{
    /// <summary>
    ///This service is responsible for the team management functionality
    /// </summary>
    public class TeamService : ITeamService
    {
        private VacationManagerDbContext context;
        private IUserService userService;

        public TeamService(VacationManagerDbContext context, IUserService userService)
        {
            this.context = context;
            this.userService = userService;
        }

        /// <summary>
        ///This method is used to create a single team
        ///based on a given name, project name and creator id 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="projectName"></param>
        /// <param name="creatorId"></param>
        /// <returns></returns>
        public int CreateTeam(string name, string projectName, int creatorId)
        {
            //Validating the team information.
            //If the team name or projec name already exists in the database
            //the user is asked to type different ones.
            bool takenProjectInfo = context.Projects.FirstOrDefault(p => p.Name == projectName) == null;
            bool takenTemaInfo = context.Teams.FirstOrDefault(t => t.Name == name) != null;

            if (takenTemaInfo)
            {
                return 0;
            }
            if (takenProjectInfo)
            {
                return -1;
            }

            var team = new Team
            {
                Name = name
            };

            //Taking the project which the user selected whed creating the team 
            //and the user which is creating the team.
            Project teamProject = context.Projects.FirstOrDefault(p => p.Name == projectName);
            User creator = context.Users.FirstOrDefault(u => u.Id == creatorId);

            team.Project = teamProject;
            team.Users.Add(creator);

            team.CreatedBy = creator.Id;

            creator.Team = team;
            teamProject.Teams.Add(team);

            //Saving the changes in the database.
            context.Teams.Add(team);
            context.Users.Update(creator);
            context.Projects.Update(teamProject);
            context.SaveChanges();

            return team.Id;
        }

        /// <summary>
        /// This method is used when a Team Lead employee wants to edit his team information.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int EditTeam(EditTeamViewModel model)
        {
            //Taking the correct team which is to be edited and the project associated with it.
            Team takenTeam = context.Teams.FirstOrDefault(t => t.Id == model.Id);
            Project takenProject = context.Projects.FirstOrDefault(p => p.Id == takenTeam.ProjectId);

            //Validating the new information.
            //If one of the properties are not edited, the validation it is skipped and vice versa.
            bool takenProjectInfo = context.Projects.FirstOrDefault(p => p.Name == model.ProjectName) == null;
            bool takenTemaInfo = context.Teams.FirstOrDefault(t => t.Name == model.Name) != null;

            if(takenTeam.Project != null)
            {
                if (!(takenTeam.Name == model.Name && takenProject.Name == model.ProjectName))
                {
                    if (takenTemaInfo)
                    {
                        return 0;
                    }
                    else if (takenProjectInfo)
                    {
                        return -1;
                    }
                }
            }
            else
            {
                if (!(takenTeam.Name == model.Name))
                {
                    if (takenTemaInfo)
                    {
                        return 0;
                    }
                }
            }

            takenProject = context.Projects.FirstOrDefault(p => p.Name == model.ProjectName);

            takenTeam.Name = model.Name;

            //If the team is already working on a project, the method only edits the team.
            //Otherwise, the method edits the project list of teams whith adding the the team wich is 
            //edited in the list of teams and edits the team itself
            if(takenTeam.Project == null)
            {
                takenProject.Teams.Add(takenTeam);
                takenTeam.ProjectId = takenProject.Id;
                takenTeam.Project = takenProject;
                context.Projects.Update(takenProject);
            }
            else
            {
                takenTeam.ProjectId = takenProject.Id;
                takenTeam.Project = takenProject;
            }


            //Saving the changes in the database.
            context.Teams.Update(takenTeam);
            context.SaveChanges();

            return takenTeam.Id;
        }


        /// <summary>
        /// This method is used when the CEO wants to create a team.
        /// Here the CEO has to type who is going to be the team leader.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="projectName"></param>
        /// <param name="teamLeadFirstName"></param>
        /// <param name="teamLeadLastName"></param>
        /// <param name="creatorId"></param>
        /// <returns></returns>
        public int CreateTeamByCeo(string name, string projectName, string teamLeadFirstName, string teamLeadLastName, int creatorId)
        {
            //Validating the data
            bool takenProjectInfo = context.Projects.FirstOrDefault(p => p.Name == projectName) == null;
            bool takenUserFirstNameInfo = context.Users.FirstOrDefault(u => u.FirstName == teamLeadFirstName) == null;
            bool takenUserLastNameInfo = context.Users.FirstOrDefault(u => u.LastName == teamLeadLastName) == null;
            bool takenTemaInfo = context.Teams.FirstOrDefault(t => t.Name == name) != null;

            int validationResult = ValidateData(takenProjectInfo, takenUserFirstNameInfo, takenUserLastNameInfo, takenTemaInfo);
            
            //If some of the data already exists in the database
            //the method returns a nonpositive number (<=0).
            //Otherwise, the team is created.
            if(validationResult <= 0)
            {
                return validationResult;
            }
            else
            {
                var team = new Team
                {
                    Name = name
                };

                Project teamProject = (Project)context.Projects.FirstOrDefault(p => p.Name == projectName);
                User teamLead = context.Users.FirstOrDefault(u => u.FirstName == teamLeadFirstName && u.LastName == teamLeadLastName);

                //Checking if a team lead with such combination of first and last name even exists in the database.
                //If true, the team is created with the correct team lead.
                //Otherwise, the user is asked to select correct user.
                if (teamLead != null)
                {
                    if (teamLead.TeamId != null)
                    {
                        return -8;
                    }

                    team.Project = teamProject;
                    team.Users.Add(teamLead);
                    team.CreatedBy = creatorId;

                    teamLead.Team = team;
                    teamProject.Teams.Add(team);

                    context.Teams.Add(team);
                    context.Users.Update(teamLead);
                    context.Projects.Update(teamProject);
                    context.SaveChanges();

                    return team.Id;
                }
                else
                {
                    return -9;
                }
            }
        }

        /// <summary>
        /// This method is used when the CEO edits a given team.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int EditTeamByCeo(EditTeamByCeoViewModel model)
        {
            //Taking the correct team which is to be edited and the project associated with it.
            Team takenTeam = context.Teams.FirstOrDefault(t => t.Id == model.Id);
            Project takenProject = context.Projects.FirstOrDefault(p => p.Id == takenTeam.ProjectId);

            //Validating the new team information
            //If one of the properties are not edited, the validation it is skipped and vice versa.
            bool takenProjectInfo = context.Projects.FirstOrDefault(p => p.Name == model.ProjectName) == null;
            bool takenUserFirstNameInfo = context.Users.FirstOrDefault(u => u.FirstName == model.TeamLeadFirstName) == null;
            bool takenUserLastNameInfo = context.Users.FirstOrDefault(u => u.LastName == model.TeamLeadLastName) == null;
            bool takenTemaInfo = context.Teams.FirstOrDefault(t => t.Name == model.Name) != null;

            if (takenTeam.Project != null)
            {
                if (!(takenTeam.Name == model.Name || takenProject.Name == model.ProjectName))
                {
                    int validationResult = ValidateData(takenProjectInfo, takenUserFirstNameInfo, takenUserLastNameInfo, takenTemaInfo);
                    
                    if (validationResult <= 0)
                    {
                        return validationResult;
                    }
                }
            }
            else
            {
                if (!(takenTeam.Name == model.Name))
                {
                    int validationResult = ValidateData(takenProjectInfo, takenUserFirstNameInfo, takenUserLastNameInfo, takenTemaInfo);

                    if (validationResult <= 0)
                    {
                        return validationResult;
                    }
                }
            }

            takenProject = context.Projects.FirstOrDefault(p => p.Name == model.ProjectName);
            User teamLead = context.Users.FirstOrDefault(u => u.FirstName == model.TeamLeadFirstName && u.LastName == model.TeamLeadLastName);
            User pastTeamLead = context.Users.FirstOrDefault(u => u.TeamId == takenTeam.Id && u.Role == "Team Lead");

            if (pastTeamLead != null && pastTeamLead != teamLead)
            {
                pastTeamLead.Team = null;
                pastTeamLead.TeamId = null;
                takenTeam.Users.Remove(pastTeamLead);
            }

            if (teamLead != null)
            {
                bool isTeamLeadValid = teamLead.TeamId != takenTeam.Id;
                if (teamLead.TeamId != null && isTeamLeadValid)
                {
                    return -8;
                }

                if (isTeamLeadValid)
                {
                    teamLead.TeamId = takenTeam.Id;
                    teamLead.Team = takenTeam;
                    takenTeam.Users.Add(teamLead);
                    context.Users.Update(teamLead);
                }

                takenTeam.Name = model.Name;

                if (takenTeam.Project == null)
                {
                    takenProject.Teams.Add(takenTeam);
                    takenTeam.ProjectId = takenProject.Id;
                    takenTeam.Project = takenProject;
                    context.Projects.Update(takenProject);
                }
                else if(takenTeam.Project != takenProject)
                {
                    takenTeam.ProjectId = takenProject.Id;
                    takenTeam.Project = takenProject;
                }

                //Saving the changes in the database.
                context.Teams.Update(takenTeam);
                context.SaveChanges();

                return takenTeam.Id;
            }
            else
            {
                return -9;
            }
        }

        /// <summary>
        /// This method is used to delete a team.
        /// </summary>
        /// <param name="id"></param>
        public void DeleteTeam(int id)
        {
            Team team = context.Teams.FirstOrDefault(t => t.Id == id);

            List<User> teamMembers = context.Users.Where(u => u.TeamId == team.Id).ToList();

            //If the team has any team members, they are removed from the team
            if (teamMembers != null)
            {
                foreach(User user in teamMembers)
                {
                    user.TeamId = null;
                    user.Team = null;
                    context.Users.Update(user);
                }
            }

            //Saving the changes in the database.
            context.Teams.Remove(team);
            context.SaveChanges();
        }

        /// <summary>
        /// This method is used to return a model that contains all the teams currently stored in the databse.
        /// </summary>
        /// <returns></returns>
        public AllTeamsViewModel GetAllTeams()
        {
            var teams = context.Teams.Select(t => new Team()
            {
                Id = t.Id,
                Name = t.Name,
                ProjectId = t.ProjectId,
                Project = t.Project,
                Users = t.Users
            });

            var model = new AllTeamsViewModel() { Teams = teams };

            return model;
        }

        /// <summary>
        /// This method is used to retudn a model whith all the teams thet are
        /// created by the currently logged user (if the user is 'Team Lead' or 'CEO')
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AllTeamsViewModel GetUserCreatedTeams(int id)
        {
            List<Team> teams = new List<Team>();
            User takenUser = context.Users.FirstOrDefault(u => u.Id == id);

            //If the user is CEO the filtering is based on the CreatedBy property.
            //Otherwuse, the filtering is based on the TeamId of the currently logged user. (which is a team lead type of employee)
            if(takenUser.Role == "CEO")
            {
                teams = context.Teams.Where(t => t.CreatedBy == takenUser.Id).ToList();
            }
            else
            {
                teams = context.Teams.Where(t => t.Id == takenUser.TeamId).ToList();
            }

            var model = new AllTeamsViewModel() { Teams = teams };

            return model;
        }

        /// <summary>
        /// This method is used to return a single team about which a detailed info
        /// is shown in the ViewTeam view.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Team ViewTeam(int id)
        {
            //Taking all the teams from the database,
            var teams = context.Teams.Select(t => new Team()
            {
                Id = t.Id,
                Name = t.Name,
                ProjectId = t.ProjectId,
                Project = t.Project,
                Users = t.Users
            });

            //Filthering the needed user based on the given id.
            return teams.FirstOrDefault(t => t.Id == id);
        }

        /// <summary>
        /// This method is used for validating all the data
        /// when creating or editing a user.
        /// </summary>
        /// <param name="takenProjectInfo"></param>
        /// <param name="takenUserFirstNameInfo"></param>
        /// <param name="takenUserLastNameInfo"></param>
        /// <param name="takenTemaInfo"></param>
        /// <returns></returns>
        private int ValidateData(bool takenProjectInfo, bool takenUserFirstNameInfo, bool takenUserLastNameInfo, bool takenTemaInfo)
        {
            if (takenTemaInfo)
            {
                return 0;
            }
            if (takenProjectInfo && takenUserFirstNameInfo && takenUserLastNameInfo)
            {
                return -1;
            }
            else if (takenProjectInfo && takenUserFirstNameInfo)
            {
                return -2;
            }
            else if (takenProjectInfo && takenUserLastNameInfo)
            {
                return -3;
            }
            else if (takenProjectInfo)
            {
                return -4;
            }
            else if (takenUserFirstNameInfo && takenUserLastNameInfo)
            {
                return -5;
            }
            else if (takenUserFirstNameInfo)
            {
                return -6;
            }
            else if (takenUserLastNameInfo)
            {
                return -7;
            }
            else
            {
                return 1;
            }
        }
    }
}
