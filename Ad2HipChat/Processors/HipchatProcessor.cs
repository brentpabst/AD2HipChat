using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Ad2HipChat.Data;
using Newtonsoft.Json.Linq;
using NLog;

namespace Ad2HipChat.Processors
{
    public class HipchatProcessor
    {
        private const string HipchatUri = "https://api.hipchat.com/v2/";
        private static readonly Logger Logger = LogManager.GetLogger("Ad2HipChat.HipChatProcessor");
        private readonly int _interval = 60000;
        private readonly IUserRepository _userRepository;

        public HipchatProcessor(IUserRepository userRepository)
        {
            _userRepository = userRepository;

            var intervalConfig = int.Parse(ConfigurationManager.AppSettings["hipchat.syncInterval"]);
            Logger.Debug("HipChat Interval from app.config: {0}", intervalConfig);

            if (intervalConfig > 0) _interval = intervalConfig;
            Logger.Debug("HipChat Interval: {0}", _interval);
        }

        public async Task Run(CancellationTokenSource token)
        {
            while (!token.IsCancellationRequested)
            {
                Logger.Debug("HipChat Processor Running");
                Logger.Trace("Cancellation Requested? " + token.IsCancellationRequested);
                try
                {
                    var client = new WebClient();
                    client.Headers.Add("Authorization", "Bearer " + ConfigurationManager.AppSettings["hipchat.accessToken"]);

                    Logger.Debug("Loading DB Users to Sync");

                    var users = _userRepository.All().ToList();

                    Logger.Debug("Found {0} users", users.Count);

                    if (users.Count == 0) Logger.Trace("No users to sync, time to sleep");

                    foreach (var user in users)
                    {
                        var syncedOk = false;

                        Logger.Trace("Syncing " + user.Principal);

                        // Invite
                        if (!user.HipChatUserId.HasValue && user.IsEnabled)
                        {
                            var request = new JObject
                            {
                                {"title", user.Title},
                                {"email", user.Email + ".test"},
                                {"name", user.FirstName + " " + user.LastName}
                            };

                            Logger.Trace(request.ToString);

                            try
                            {
                                var response = await client.UploadStringTaskAsync(new Uri(HipchatUri + "invite/user"), "POST", request.ToString());
                                Logger.Trace(response);
                                var hipchatUser = JObject.Parse(response);

                                user.HipChatUserId = hipchatUser["id"].Value<int>();

                                syncedOk = true;
                            }
                            catch (WebException we)
                            {
                                var response = we.Response as HttpWebResponse;
                                var reader = new StreamReader(response.GetResponseStream());
                                Logger.Debug(reader.ReadToEnd);
                                Logger.Error(we);
                            }
                        }

                        // Delete
                        else if (!user.IsEnabled && user.HipChatUserId.HasValue)
                        {
                            try
                            {
                                var uri = HipchatUri + "user/" + user.HipChatUserId.Value;
                                var response = await client.UploadStringTaskAsync(new Uri(uri), "DELETE", "");
                                Logger.Trace(response);

                                user.HipChatUserId = null;

                                syncedOk = true;
                            }
                            catch (WebException we)
                            {
                                var response = we.Response as HttpWebResponse;
                                var reader = new StreamReader(response.GetResponseStream());
                                Logger.Debug(reader.ReadToEnd);
                                Logger.Error(we);
                            }
                        }

                        // Edit
                        else
                        {
                            try
                            {
                                var uri = HipchatUri + "user/" + user.HipChatUserId.Value;
                                var response = await client.DownloadStringTaskAsync(new Uri(uri));
                                Logger.Trace(response);

                                var hipchatUser = JObject.Parse(response);
                                Logger.Trace(hipchatUser.ToString);

                                // Remove stuff HipChat doesn't want back... bleh
                                var fieldsToKeep = new[] { "name", "rolees", "title", "presence", "mention_name", "timezone", "email" };
                                var fieldsToDump = new List<string>();
                                foreach (var field in hipchatUser)
                                {
                                    if (!fieldsToKeep.Contains(field.Key))
                                        fieldsToDump.Add(field.Key);
                                }
                                foreach (var field in fieldsToDump)
                                    hipchatUser.Remove(field);

                                hipchatUser["title"] = user.Title;
                                hipchatUser["name"] = user.FirstName + " " + user.LastName;
                                hipchatUser["email"] = user.Email + ".test";

                                response = await client.UploadStringTaskAsync(new Uri(uri), "PUT", hipchatUser.ToString());
                                Logger.Trace(response);

                                syncedOk = true;
                            }
                            catch (WebException we)
                            {
                                var response = we.Response as HttpWebResponse;
                                var reader = new StreamReader(response.GetResponseStream());
                                Logger.Debug(reader.ReadToEnd);
                                Logger.Error(we);
                            }
                        }

                        if (syncedOk)
                        {
                            Logger.Trace(user.Principal + " is synced with HipChat, storing results...");
                            user.IsSynced = true;
                            user.SyncedOn = DateTime.UtcNow;

                            Logger.Trace("Saving user changes");
                            await _userRepository.Save(user);

                            Logger.Trace("Committing user changes.");
                            await _userRepository.Commit();
                        }
                        else
                            Logger.Trace(user.Principal + " failed to sync with HipChat, will retry...");

                        Logger.Trace("Done with " + user.Principal);
                    }

                    Logger.Debug("HipChat Processor Sleeping");
                    await Task.Delay(_interval);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                    Logger.Error(ex, "HipChat Processor Failed");

                    Logger.Trace("Calling for Token Cancellation");
                    token.Cancel();
                }
            }
            Logger.Debug("HipChat Processor is cancelling...");
        }
    }
}
