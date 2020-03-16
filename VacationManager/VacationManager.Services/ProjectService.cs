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
    /// <summary>
    ///This service is responsible for the project management functionality
    /// </summary>
    public class ProjectService : IProjectService
    {
        private VacationManagerDbContext context;
        private IUserService userService;

        public ProjectService(VacationManagerDbContext context, IUserService userService)
        {
            this.context = context;
            this.userService = userService;
        }

        /// <summary>
        ///This method is used to create a single project
        ///based on a given name and description
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <returns></returns>
        public int CreateProject(string name, string description)
        {
            //validating the project info
            //if there is already a project with the given name
            //the method returns -1 and the user is asked to type different project name
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

            //Saving the changes
            context.Projects.Add(project);
            context.SaveChanges();

            return project.Id;
        }


        /// <summary>
        ///This method is used to edit a single project entity
        ///choosed by the user. It takes a model as an argument.
        /// </summary>
        public int EditProject(EditProjectViewModel model)
        {
            //Taking the current project which is to be edited.
            Project takenProject = context.Projects.FirstOrDefault(p => p.Id == model.Id);

            //Validating the project info.
            //if the name is not edited, the validation is skipped.
            //Otherwise, if the new project name already exists in the data base, 
            //type different name the user is asked to 
            bool takenInfo = context.Projects.FirstOrDefault(p => p.Name == model.Name) != null;

            if(!(takenProject.Name == model.Name) && takenInfo)
            {
                return -1;
            }

            //Setting the new information for the project
            takenProject.Name = model.Name;
            takenProject.Description = model.Description;

            //Saving the changes
            context.Projects.Update(takenProject);
            context.SaveChanges();

            return takenProject.Id;
        }

        /// <summary>
        /// This method is used to delete a given project
        /// It takes a project id. Based on this id, the correct project is removed from the datavase.
        /// </summary>
        /// <param name="id"></param>
        public void DeleteProject(int id)
        {
            //Taking the correct project which is to be deleted.
            Project project = context.Projects.FirstOrDefault(p => p.Id == id);

            List<Team> teamsWorkingOnProject = context.Teams.Where(t => t.ProjectId == project.Id).ToList();

            //Checking if there are teams working on that project.
            //If true, all teams cancel their work on the project.
            if(teamsWorkingOnProject != null)
            {
                foreach (var team in teamsWorkingOnProject)
                {
                    team.Project = null;
                    context.Teams.Update(team);
                }
            }

            //Saving the changes.
            context.Projects.Remove(project);
            context.SaveChanges();
        }


        /// <summary>
        /// This method is used to return a single project, that is going to 
        /// be showed in the ViewProject view, where the is displayed detailed info
        /// about the selected project.
        /// </summary>
        /// <param name="id"></param>
        public Project ViewProject(int id)
        {
            //Taking all the project from the database.
            var projects = context.Projects.Select(p => new Project()
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description
            });

            //Filtering the project based on the given id.
            return projects.FirstOrDefault(p => p.Id == id);
        }

        /// <summary>
        /// This method is used to get all the projects in the database.
        /// The returned collection of project is then used ibn the
        /// ViewCreatedProjects view which shows breaf ingo about all projects.
        /// </summary>
        /// <returns></returns>
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
