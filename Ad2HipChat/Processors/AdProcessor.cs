using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using Ad2HipChat.Data;
using Ad2HipChat.Services;
using NLog;

namespace Ad2HipChat.Processors
{
    public class AdProcessor
    {
        private static readonly Logger Logger = LogManager.GetLogger("Ad2HipChat.AdProcessor");
        private readonly int _interval = 60000;
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;

        public AdProcessor(IUserService userService, IUserRepository userRepository)
        {
            _userRepository = userRepository;
            _userService = userService;

            var intervalConfig = int.Parse(ConfigurationManager.AppSettings["ad.syncInterval"]);
            Logger.Debug("AD Interval from app.config: {0}", intervalConfig);

            if (intervalConfig > 0) _interval = intervalConfig;
            Logger.Debug("AD Interval: {0}", _interval);
        }

        public async void Run(CancellationTokenSource token)
        {
            while (!token.IsCancellationRequested)
            {
                Logger.Debug("AD Processor Running");
                Logger.Trace("Cancellation Requested? " + token.IsCancellationRequested);
                try
                {
                    Logger.Debug("Loading AD Users");
                    var users = _userService.GetAllUsers().ToList();

                    Logger.Debug("Found {0} users", users.Count());

                    // Save New Users or Update Existing Users
                    foreach (var user in users)
                    {
                        Logger.Trace("Looking for " + user.UserPrincipalName);
                        var dbUser = await _userRepository.Get(user.UserPrincipalName);
                        if (dbUser == null)
                        {
                            Logger.Trace(user.UserPrincipalName + " Not Found in the database");
                            // New User
                            var u = new User()
                            {
                                Principal = user.UserPrincipalName,
                                Email = user.EmailAddress,
                                FirstName = user.GivenName,
                                LastName = user.Surname,
                                IsEnabled = user.Enabled ?? false,
                                IsSynced = false,
                                AddedOn = DateTime.UtcNow,
                                UpdatedOn = DateTime.UtcNow,
                                History = new List<UserHistory>() { new UserHistory()
                                {
                                   ChangedOn = DateTime.UtcNow,
                                    FirstName = user.GivenName,
                                    LastName = user.Surname,
                                    Email = user.EmailAddress,
                                    IsEnabled = user.Enabled ?? false
                                }}
                            };
                            Logger.Trace("Adding new user " + user.UserPrincipalName);
                            await _userRepository.Save(u);
                        }
                        else
                        {
                            Logger.Trace(user.UserPrincipalName + " Found in the database, updating");

                            // Existing User
                            if (dbUser.IsEnabled != (user.Enabled ?? false)
                                || dbUser.Email != user.EmailAddress
                                || dbUser.FirstName != user.GivenName
                                || dbUser.LastName != user.Surname)
                            {
                                Logger.Trace(user.UserPrincipalName + " database and AD records differ, syncing");
                                // User has changed
                                dbUser.IsEnabled = (user.Enabled ?? false);
                                dbUser.Email = user.EmailAddress;
                                dbUser.FirstName = user.GivenName;
                                dbUser.LastName = user.Surname;
                                dbUser.UpdatedOn = DateTime.UtcNow;
                                dbUser.IsSynced = false;

                                dbUser.History.Add(new UserHistory
                                {
                                    ChangedOn = DateTime.UtcNow,
                                    FirstName = user.GivenName,
                                    LastName = user.Surname,
                                    Email = user.EmailAddress,
                                    IsEnabled = user.Enabled ?? false
                                });

                                Logger.Trace("Updating existing user " + user.UserPrincipalName);
                                await _userRepository.Save(dbUser);
                            }
                            else
                            {
                                Logger.Trace("No user changes, passing " + user.UserPrincipalName);
                            }
                        }
                    }
                    Logger.Debug("Committing user changes and additions");
                    await _userRepository.Commit();

                    Logger.Debug("AD Processor Sleeping");
                    Thread.Sleep(_interval);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, "AD Processor Failed");

                    Logger.Trace("Calling for Token Cancellation");
                    token.Cancel();
                }
            }
        }
    }
}
