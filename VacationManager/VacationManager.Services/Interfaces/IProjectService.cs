using System;
using System.Collections.Generic;
using System.Text;
using VacationManager.ViewModels;
using VacationManager.Data.Models;

namespace VacationManager.Services.Interfaces
{
    public interface IProjectService
    {
        int CreateProject(string name, string description);

        int EditProject(EditProjectViewModel model);

        void DeleteProject(int id);

        Project ViewProject(int id);

        AllProjectsViewModel GetAllProjects();
    }
}
