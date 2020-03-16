using System;
using System.Collections.Generic;
using System.Text;
using VacationManager.Data.Models;
using VacationManager.ViewModels;


namespace VacationManager.Services.Interfaces
{
    public interface IUserService
    {
        int Register(string firstName,
                     string lastName,
                     string username,
                     string password,
                     string confirmPassword,
                     string email,
                     string role);
        int EditUser(EditUserViewModel model);

        void DeleteUser(int id);

        int Login(string username, string password);

        User GetLoggedUser();

        User ViewUser(int id);

        void AddUserToTeam(int loggedUserId, int userToBeAddedId);

        AllUsersViewModel GetAllUsers();
    }
}
