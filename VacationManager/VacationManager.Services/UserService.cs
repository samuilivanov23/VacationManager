using System;
using System.Collections.Generic;
using System.Text;
using VacationManager.Data;
using VacationManager.Data.Models;
using VacationManager.ViewModels;
using VacationManager.Services.Interfaces;
using System.Linq;

namespace VacationManager.Services
{
    public class UserService : IUserService
    {
        private VacationManagerDbContext context;

        public UserService(VacationManagerDbContext context)
        {
            this.context = context;
        }

        public int Register(string firstName,
                             string lastName,
                             string username,
                             string password,
                             string confirmPassword,
                             string email,
                             string role)
        {

            //validating if the username or email have already been taken
            bool takenInfo = context.Users.FirstOrDefault(x => x.Username == username) != null
                || context.Users.FirstOrDefault(x => x.Email == email) != null;

            if (takenInfo)
            {
                return -1;
            }
            else if (role != "CEO" && role != "Team Lead" && role != "Developer" && role != "Unassigned")
            {
                return -2;
            }

            var user = new User()
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Username = username,
                Password = password,
                ConfirmPassword = confirmPassword,
                Role = role
            };

            //Setting up user permissions
            if(user.Role == "Developer" || user.Role == "Unassigned")
            {
                user.PermissionLevel = 0;
            }
            else if (user.Role == "Team Lead")
            {
                user.PermissionLevel = 1;
            }
            else
            {
                user.PermissionLevel = 2;
            }

            user.Team = null;

            context.Users.Add(user);
            context.SaveChanges();

            return user.Id;
        }

        public int EditUser(EditUserViewModel model)
        {
            User takenUser = context.Users.FirstOrDefault(u => u.Id == model.Id);

            //validating if the username or email have already been taken
            bool takenInfo = context.Users.FirstOrDefault(x => x.Username == model.Username) != null
                || context.Users.FirstOrDefault(x => x.Email == model.Email) != null;
            
            if(!(takenUser.Username == model.Username && takenUser.Email == model.Email))
            {
                if (takenInfo)
                {
                    return -1;
                }
                else if (model.Role != "CEO" && model.Role != "Team Lead" && model.Role != "Developer" && model.Role != "Unassigned")
                {
                    return -2;
                }
            }

            takenUser.FirstName = model.FirstName;
            takenUser.LastName = model.LastName;
            takenUser.Email = model.Email;
            takenUser.Username = model.Username;
            takenUser.Password = model.Password;
            takenUser.ConfirmPassword = model.ConfirmPassword;
            takenUser.Role = model.Role;

            //Setting up user permissions
            if (takenUser.Role == "Developer" || takenUser.Role == "Unassigned")
            {
                takenUser.PermissionLevel = 0;
            }
            else if (takenUser.Role == "Team Lead")
            {
                takenUser.PermissionLevel = 1;
            }
            else
            {
                takenUser.PermissionLevel = 2;
            }

            context.Users.Update(takenUser);
            context.SaveChanges();

            return takenUser.Id;
        }

        public void DeleteUser(int id)
        {
            User user = context.Users.FirstOrDefault(u => u.Id == id);
            if(user.TeamId != null)
            {
                Team team = context.Teams.FirstOrDefault(t => t.Id == user.TeamId);
                team.Users.Remove(user);
                context.Teams.Update(team);
            }
            
            context.Users.Remove(user);
            context.SaveChanges();
        }

        public int Login(string username, string password)
        {
            var user = context.Users.FirstOrDefault(x => x.Username == username && x.Password == password);

            if (user != null)
            {
                LoggedUserInfo.LoggedUserId = user.Id;
                return user.Id;
            }
            return -1;
        }

        public User GetLoggedUser()
        {
            var loggedUser = context.Users.FirstOrDefault(x => x.Id == LoggedUserInfo.LoggedUserId);

            return loggedUser;
        }

        public User ViewUser(int id)
        {
            var users = context.Users.Select(u => new User()
            {
                Id = u.Id,
                Username = u.Username,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                TeamId = u.TeamId,
                Team = u.Team,
                Role = u.Role,
            });

            return users.FirstOrDefault(u => u.Id == id);
        }

        public void AddUserToTeam(int loggedUserId, int userToBeAddedId)
        {
            User loggedUser = context.Users.FirstOrDefault(u => u.Id == loggedUserId);
            Team team = context.Teams.FirstOrDefault(t => t.Id == loggedUser.TeamId);
            User userToBeAdded = context.Users.FirstOrDefault(u => u.Id == userToBeAddedId);

            team.Users.Add(userToBeAdded);
            context.Users.Update(userToBeAdded);
            context.Teams.Update(team);
            context.SaveChanges();
        }

        public AllUsersViewModel GetAllUsers()
        {

            var users = context.Users.Select(u => new User()
            {
                Id = u.Id,
                Username = u.Username,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Role = u.Role,
            });

            var model = new AllUsersViewModel() { Users = users };

            return model;
        }
    }
}
