using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Zen.Game;
using Zen.Game.IO;
using Zen.Game.Model;
using Zen.Net.Login;
using Zen.Shared;

namespace Zen.Net.Service
{
    public class LoginService
    {
        public LoginService(PlayerSerializer serializer)
        {
            Serializer = serializer;
        }

        public PlayerSerializer Serializer { get; }
        private BlockingCollection<Job> Jobs { get; } = new BlockingCollection<Job>();
        private Queue<SessionPlayerPair> NewPlayers { get; } = new Queue<SessionPlayerPair>();
        private Queue<Player> OldPlayers { get; } = new Queue<Player>();

        public void AddLoginRequest(LoginSession session, LoginRequest request)
        {
            Jobs.Add(new LoginJob(session, request));
        }

        public void AddLogoutRequest(Player player)
        {
            lock (OldPlayers)
            {
                OldPlayers.Enqueue(player);
            }
        }

        public void RegisterNewPlayers(GameWorld world)
        {
            lock (NewPlayers)
            {
                SessionPlayerPair pair;
                while (NewPlayers.Count > 0 && (pair = NewPlayers.Dequeue()) != null)
                    if (world.GetPlayer(pair.Player.Username) != null)
                        pair.Session.SendLoginFailure(LoginConstants.StatusAlreadyOnline);
                    else if (!world.AddPlayer(pair.Player))
                        pair.Session.SendLoginFailure(LoginConstants.StatusWorldFull);
                    else
                        pair.Session.SendLoginSuccess(LoginConstants.StatusOk, pair.Player);
            }

            lock (OldPlayers)
            {
                Player player;
                while (OldPlayers.Count > 0 && (player = OldPlayers.Dequeue()) != null)
                {
                    world.RemovePlayer(player);
                    Jobs.Add(new LogoutJob(player));
                }
            }
        }

        [SuppressMessage("ReSharper", "FunctionNeverReturns")]
        public void Start()
        {
            for (;;)
                Jobs.Take().Perform(this);
        }

        internal abstract class Job
        {
            public abstract void Perform(LoginService service);
        }

        internal class LoginJob : Job
        {
            public LoginJob(LoginSession session, LoginRequest request)
            {
                Session = session;
                Request = request;
            }

            private LoginSession Session { get; }
            private LoginRequest Request { get; }

            public override void Perform(LoginService service)
            {
                var result = service.Serializer.Load(Request.Username, Request.Password);
                var status = result.Status;

                if (status != LoginConstants.StatusOk)
                    Session.SendLoginFailure(status);
                else
                {
                    lock (service.NewPlayers)
                    {
                        service.NewPlayers.Enqueue(new SessionPlayerPair(Session, result.Player));
                    }
                }
            }
        }

        internal class LogoutJob : Job
        {
            public LogoutJob(Player player)
            {
                Player = player;
            }

            private Player Player { get; }

            public override void Perform(LoginService service)
            {
                service.Serializer.Save(Player);
            }
        }

        internal class SessionPlayerPair
        {
            public SessionPlayerPair(LoginSession session, Player player)
            {
                Session = session;
                Player = player;
            }

            public LoginSession Session { get; }
            public Player Player { get; }
        }
    }
}