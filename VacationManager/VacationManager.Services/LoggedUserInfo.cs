using System;
using System.Collections.Generic;
using System.Text;

namespace VacationManager.Services
{
    /// <summary>
    /// This is a static class which holds the logged user id so it can be accessed from anywhere.
    /// </summary>
    public static class LoggedUserInfo
    {
        /// <summary>
        /// The id of the logged user (by default it is 0, meaning there is no logged in user)
        /// </summary>
        public static int LoggedUserId = 0;
    }
}
