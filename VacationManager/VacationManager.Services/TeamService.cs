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
    public class TeamService : ITeamService
    {
        private VacationManagerDbContext context;
        private IUserService userService;

        public TeamService(VacationManagerDbContext context, IUserService userService)
        {
            this.context = context;
            this.userService = userService;
        }

        public int CreateTeam(string name, string projectName, int creatorId)
        {
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

            Project teamProject = context.Projects.FirstOrDefault(p => p.Name == projectName);
            User creator = context.Users.FirstOrDefault(u => u.Id == creatorId);

            team.Project = teamProject;
            team.Users.Add(creator);

            team.CreatedBy = creator.Id;

            creator.Team = team;
            teamProject.Teams.Add(team);

            context.Teams.Add(team);
            context.Users.Update(creator);
            context.Projects.Update(teamProject);
            context.SaveChanges();

            return team.Id;
        }

        public int EditTeam(EditTeamViewModel model)
        {
            Team takenTeam = context.Teams.FirstOrDefault(t => t.Id == model.Id);
            Project takenProject = context.Projects.FirstOrDefault(p => p.Id == takenTeam.ProjectId);
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

            context.Teams.Update(takenTeam);
            context.SaveChanges();

            return takenTeam.Id;
        }

        public int CreateTeamByCeo(string name, string projectName, string teamLeadFirstName, string teamLeadLastName, int creatorId)
        {
            bool takenProjectInfo = context.Projects.FirstOrDefault(p => p.Name == projectName) == null;
            bool takenUserFirstNameInfo = context.Users.FirstOrDefault(u => u.FirstName == teamLeadFirstName) == null;
            bool takenUserLastNameInfo = context.Users.FirstOrDefault(u => u.LastName == teamLeadLastName) == null;
            bool takenTemaInfo = context.Teams.FirstOrDefault(t => t.Name == name) != null;

            int validationResult = ValidateData(takenProjectInfo, takenUserFirstNameInfo, takenUserLastNameInfo, takenTemaInfo);

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

        public int EditTeamByCeo(EditTeamByCeoViewModel model)
        {
            Team takenTeam = context.Teams.FirstOrDefault(t => t.Id == model.Id);
            Project takenProject = context.Projects.FirstOrDefault(p => p.Id == takenTeam.ProjectId);
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

                context.Teams.Update(takenTeam);
                context.SaveChanges();

                return takenTeam.Id;
            }
            else
            {
                return -9;
            }
        }

        public void DeleteTeam(int id)
        {
            Team team = context.Teams.FirstOrDefault(t => t.Id == id);

            List<User> teamMembers = context.Users.Where(u => u.TeamId == team.Id).ToList();

            if (teamMembers != null)
            {
                foreach(User user in teamMembers)
                {
                    user.TeamId = null;
                    user.Team = null;
                    context.Users.Update(user);
                }
            }

            context.Teams.Remove(team);
            context.SaveChanges();
        }

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

        public AllTeamsViewModel GetUserCreatedTeams(int id)
        {
            List<Team> teams = new List<Team>();
            User takenUser = context.Users.FirstOrDefault(u => u.Id == id);

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

        public Team ViewTeam(int id)
        {
            var teams = context.Teams.Select(t => new Team()
            {
                Id = t.Id,
                Name = t.Name,
                ProjectId = t.ProjectId,
                Project = t.Project,
                Users = t.Users
            });

            return teams.FirstOrDefault(t => t.Id == id);
        }

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
