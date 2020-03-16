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

        /// <summary>
        /// This method is used for the register functionality
        /// which creates users in the database.
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="confirmPassword"></param>
        /// <param name="email"></param>
        /// <param name="role"></param>
        /// <returns></returns>
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

            //Saving the changes in the databse.
            context.Users.Add(user);
            context.SaveChanges();

            return user.Id;
        }

        /// <summary>
        /// This method is used to edit a user's information
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int EditUser(EditUserViewModel model)
        {
            //Taking the user which is going to be edited
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

            //editing the user's information
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

            //Saving the changes in the database.
            context.Users.Update(takenUser);
            context.SaveChanges();

            return takenUser.Id;
        }

        /// <summary>
        /// This method is used to delete a user from the database.
        /// </summary>
        /// <param name="id"></param>
        public void DeleteUser(int id)
        {
            //Taking the user based on the given id
            User user = context.Users.FirstOrDefault(u => u.Id == id);

            //if the user has a team, he is removed from the team.
            if(user.TeamId != null)
            {
                Team team = context.Teams.FirstOrDefault(t => t.Id == user.TeamId);
                team.Users.Remove(user);
                context.Teams.Update(team);
            }
            
            //Saving the changes from the database.
            context.Users.Remove(user);
            context.SaveChanges();
        }

        /// <summary>
        /// This method is used to create a login functionality
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public int Login(string username, string password)
        {
            //Taking the correct user based on the given username and password
            var user = context.Users.FirstOrDefault(x => x.Username == username && x.Password == password);

            //If there is such a user in the database
            //The logged user id is set to this user's id
            //Otherwise, the user is asked to type a valid login data.
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

        /// <summary>
        /// This methot is used to return a single User which is shown in the ViewUser view
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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

        /// <summary>
        /// This method is used when a team lead employee wants to add
        /// a developer without a team to his team.
        /// </summary>
        /// <param name="loggedUserId"></param>
        /// <param name="userToBeAddedId"></param>
        public void AddUserToTeam(int loggedUserId, int userToBeAddedId)
        {
            //Taking the logged user based on the loggeduserId
            User loggedUser = context.Users.FirstOrDefault(u => u.Id == loggedUserId);

            //Taking the team of the logged user
            Team team = context.Teams.FirstOrDefault(t => t.Id == loggedUser.TeamId);

            //Taking the user which is going to be added to the team based on the userToBeAddedId
            User userToBeAdded = context.Users.FirstOrDefault(u => u.Id == userToBeAddedId);

            //Adding the user to the team and saving the changes to the database
            team.Users.Add(userToBeAdded);
            context.Users.Update(userToBeAdded);
            context.Teams.Update(team);
            context.SaveChanges();
        }

        /// <summary>
        /// This method returns a model that contains all currently stored users in the database.
        /// </summary>
        /// <returns></returns>
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
