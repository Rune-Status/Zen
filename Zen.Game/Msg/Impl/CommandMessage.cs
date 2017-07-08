using System;

namespace Zen.Game.Msg.Impl
{
    public class CommandMessage : Message
    {
        public CommandMessage(string keyword)
        {
            var parts = keyword.Split();

            Name = parts[0].ToLower();
            Arguments = new string[parts.Length - 1];

            Array.Copy(parts, 1, Arguments, 0, Arguments.Length);
        }

        public string[] Arguments { get; }
        public string Name { get; }
    }
}