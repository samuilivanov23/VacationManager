using System;
using System.Collections.Generic;
using System.Text;
using VacationManager.ViewModels;
using VacationManager.Data.Models;

namespace VacationManager.Services.Interfaces
{
    public interface ITeamService
    {
        int CreateTeam(string name, string projectName, int teamLeadId);

        int CreateTeamByCeo(string name, string projectName, string teamLeadFirstName, string teamLeadLastName);

        int EditTeam(EditTeamViewModel model);

        int EditTeamByCeo(EditTeamByCeoViewModel model);

        void DeleteTeam(int id);

        AllTeamsViewModel GetAllTeams();

        AllTeamsViewModel GetUserCreatedTeams(int id);

        Team ViewTeam(int id);
    }
}
