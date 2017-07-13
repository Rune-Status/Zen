using System;
using System.Threading;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using NLog;
using NLog.Conditions;
using NLog.Config;
using NLog.Targets;
using Zen.Game;
using Zen.Net;
using Zen.Net.Handshake;
using Zen.Net.Service;
using Zen.Shared;
using Zen.Util;

namespace Zen.Core
{
    public class Program
    {
        private readonly ServerBootstrap _bootstrap = new ServerBootstrap();
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private readonly GameServer _server = new GameServer();
        private readonly ServiceManager _serviceManager = new ServiceManager();

        static Program()
        {
            Console.Title = GameConstants.Title;

            var config = new LoggingConfiguration();
            var consoleTarget = new ColoredConsoleTarget();
            var rule = new LoggingRule("*", LogLevel.Debug, consoleTarget);

            var highlightingRules = new[]
            {
                new ConsoleRowHighlightingRule
                {
                    Condition = ConditionParser.ParseExpression("level == LogLevel.Error"),
                    ForegroundColor = ConsoleOutputColor.Red
                },

                new ConsoleRowHighlightingRule
                {
                    Condition = ConditionParser.ParseExpression("level == LogLevel.Debug"),
                    ForegroundColor = ConsoleOutputColor.DarkCyan
                }
            };

            consoleTarget.Layout = @"[${date:format=hh\:mm\:ss tt}] [${logger}]: ${message}";
            foreach (var highlightRule in highlightingRules)
                consoleTarget.RowHighlightingRules.Add(highlightRule);

            config.AddTarget("console", consoleTarget);
            config.LoggingRules.Add(rule);

            LogManager.Configuration = config;
        }

        private Program(int port)
        {
            var bossGroup = new MultithreadEventLoopGroup(1);
            var workerGroup = new MultithreadEventLoopGroup();

            _bootstrap.Group(bossGroup, workerGroup)
                .Channel<TcpServerSocketChannel>()
                .ChildHandler(new ActionChannelInitializer<ISocketChannel>(ch =>
                {
                    var pipeline = ch.Pipeline;

                    pipeline.AddLast(
                        new HandshakeDecoder(),
                        new ReadTimeoutHandler(5),
                        new GameChannelHandler(_serviceManager, _server));
                }))
                .ChildOption(ChannelOption.TcpNodelay, true)
                .BindAsync(port);

            AppDomain.CurrentDomain.ProcessExit += (sender, args) =>
            {
                _bootstrap.Group().ShutdownGracefullyAsync();
                _bootstrap.ChildGroup().ShutdownGracefullyAsync();
            };
        }

        public static void Main(string[] args)
        {
            var port = args.Length == 0
                ? GameConstants.Port
                : int.Parse(args[0]);

            new Program(port).Start();
        }

        private void Start()
        {
            _logger.Info("Ready for connections!");

            /* Start the service manager. */
            _serviceManager.StartAll();

            /* Process the game world. */
            for (;;)
            {
                var start = DateUtil.CurrentTimeMillis;
                Tick();
                var elapsed = DateUtil.CurrentTimeMillis - start;
                var waitFor = 600 - elapsed;
                if (waitFor >= 0)
                    Thread.Sleep((int) waitFor);
            }
        }

        private void Tick()
        {
            _serviceManager.LoginService.RegisterNewPlayers(_server.World);
            _server.World.Tick();
        }
    }
}