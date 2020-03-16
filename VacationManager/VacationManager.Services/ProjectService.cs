using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VacationManager.Data;
using VacationManager.Data.Models;
using VacationManager.Services.Interfaces;
using VacationManager.ViewModels;

namespace VacationManager.Services
{
    public class ProjectService : IProjectService
    {
        private VacationManagerDbContext context;
        private IUserService userService;

        public ProjectService(VacationManagerDbContext context, IUserService userService)
        {
            this.context = context;
            this.userService = userService;
        }

        public int CreateProject(string name, string description)
        {
            bool takenInfo = context.Projects.FirstOrDefault(p => p.Name == name) != null;

            if (takenInfo)
            {
                return -1;
            }

            var project = new Project
            {
                Name = name,
                Description = description
            };

            project.Teams = null;
            context.Projects.Add(project);
            context.SaveChanges();

            return project.Id;
        }

        public int EditProject(EditProjectViewModel model)
        {
            Project takenProject = context.Projects.FirstOrDefault(p => p.Id == model.Id);
            bool takenInfo = context.Projects.FirstOrDefault(p => p.Name == model.Name) != null;

            if(!(takenProject.Name == model.Name) && takenInfo)
            {
                return -1;
            }

            takenProject.Name = model.Name;
            takenProject.Description = model.Description;

            context.Projects.Update(takenProject);
            context.SaveChanges();

            return takenProject.Id;
        }

        public void DeleteProject(int id)
        {
            Project project = context.Projects.FirstOrDefault(p => p.Id == id);

            List<Team> teamsWorkingOnProject = context.Teams.Where(t => t.ProjectId == project.Id).ToList();

            if(teamsWorkingOnProject != null)
            {
                foreach (var team in teamsWorkingOnProject)
                {
                    team.Project = null;
                    context.Teams.Update(team);
                }
            }            

            context.Projects.Remove(project);
            context.SaveChanges();
        }

        public Project ViewProject(int id)
        {
            var projects = context.Projects.Select(p => new Project()
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description
            });

            return projects.FirstOrDefault(p => p.Id == id);
        }

        public AllProjectsViewModel GetAllProjects()
        {
            var projects = context.Projects.Select(p => new Project()
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description
            });

            var model = new AllProjectsViewModel() { Projects = projects };

            return model;
        }
    }
}
