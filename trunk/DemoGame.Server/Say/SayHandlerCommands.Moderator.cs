﻿using System;
using System.Linq;
using NetGore;
using NetGore.Features.Banning;

namespace DemoGame.Server
{
    /// <summary>
    /// <see cref="SayHandlerCommands"/> for <see cref="UserPermissions.Moderator"/>.
    /// </summary>
    public partial class SayHandlerCommands
    {
        /// <summary>
        /// Sends a message globally to the entire world.
        /// </summary>
        /// <param name="message">The message to announce.</param>
        [SayHandlerCommand("Announce", UserPermissions.Moderator)]
        public void Announce(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            World.Send(GameMessage.CommandAnnounce, ServerMessageType.GUIChat, message);
        }

        /// <summary>
        /// Warps the user to the player specified.
        /// </summary>
        /// <param name="userName">The name of the player to approach.</param>
        [SayHandlerCommand("Approach", UserPermissions.Moderator)]
        public void Approach(string userName)
        {
            // Get the user we want
            var target = World.FindUser(userName);

            // Check that the user could be found
            if (target == null)
            {
                User.Send(GameMessage.CommandGeneralUnknownUser, ServerMessageType.GUIChat, userName);
                return;
            }

            // Target user was found, so teleport the user that issued the command to the target user
            User.Teleport(target.Map, target.Position);
        }

        /// <summary>
        /// Displays all ban information for a user.
        /// </summary>
        /// <param name="username">The name of the user to show the ban info for.</param>
        [SayHandlerCommand("BanHistory", UserPermissions.Moderator)]
        public void BanHistory(string username)
        {
            var banInfos = BanningManager.Instance.GetAccountBanInfo(username);

            var count = banInfos.Count();
            UserChat(string.Format("All bans on user `{0}`: {1}", username, count));

            var i = 1;
            foreach (var banInfo in banInfos)
            {
                UserChat(banInfo, i++, count);
            }
        }

        /// <summary>
        /// Displays the active ban information for a user.
        /// </summary>
        /// <param name="username">The name of the user to show the ban info for.</param>
        [SayHandlerCommand("BanInfo", UserPermissions.Moderator)]
        public void BanInfo(string username)
        {
            var banInfos = BanningManager.Instance.GetAccountBanInfo(username).Where(x => !x.Expired);

            var count = banInfos.Count();
            UserChat(string.Format("Active bans on user `{0}`: {1}", username, count));

            var i = 1;
            foreach (var banInfo in banInfos)
            {
                UserChat(banInfo, i++, count);
            }
        }

        /// <summary>
        /// Bans a user.
        /// </summary>
        /// <param name="username">The name of the user to ban.</param>
        /// <param name="duration">For how long to ban the user. For duration format, see <see cref="DurationParser"/>.</param>
        /// <param name="reason">The reason the user is to be banned.</param>
        [SayHandlerCommand("BanUser", UserPermissions.Moderator)]
        public void BanUser(string username, string duration, string reason)
        {
            // Check for valid parameters
            if (!GameData.UserName.IsValid(username))
            {
                User.Send(GameMessage.CommandGeneralInvalidUser, ServerMessageType.GUI, username);
                return;
            }

            if (string.IsNullOrEmpty(duration))
            {
                User.Send(GameMessage.CommandGeneralInvalidParameter, ServerMessageType.GUI, "duration");
                return;
            }

            // Parse the duration
            TimeSpan dur;
            string parseFailReason;
            if (!DurationParser.TryParse(duration, out dur, out parseFailReason))
            {
                User.Send(GameMessage.CommandGeneralInvalidParameterEx, ServerMessageType.GUI, "duration", parseFailReason);
                return;
            }

            // Perform the ban and notify the issuer if it was successful or not
            BanManagerFailReason banFailReason;
            if (!BanningManager.Instance.TryAddUserBan(username, dur, reason, User.Name, out banFailReason))
            {
                // Ban failed
                User.Send(GameMessage.BanUserFailed, ServerMessageType.GUI, username, banFailReason.GetDetailedString());
            }
            else
            {
                // Ban successful
                User.Send(GameMessage.BanUserSuccessful, ServerMessageType.GUI, username);
            }
        }

        /// <summary>
        /// Kicks the specified user from the world.
        /// </summary>
        /// <param name="userName">The player to kick.</param>
        /// <param name="reason">The reason the player is being kicked.</param>
        [SayHandlerCommand("Kick", UserPermissions.Moderator)]
        public void Kick(string userName, string reason)
        {
            // Get the user we want
            var target = World.FindUser(userName);

            // Check that the user could be found
            if (target == null)
            {
                User.Send(GameMessage.CommandGeneralUnknownUser, ServerMessageType.GUIChat, userName);
                return;
            }

            // User was found, so disconnect them and give the reason for the disconnect
            target.Conn.Disconnect(GameMessage.DisconnectUserKicked, reason);
        }

        /// <summary>
        /// Causes you to kill yourself.
        /// </summary>
        [SayHandlerCommand("Suicide", UserPermissions.Moderator)]
        [SayHandlerCommand("Seppuku", UserPermissions.Moderator)]
        public void Suicide()
        {
            User.Kill();
        }
    }
}