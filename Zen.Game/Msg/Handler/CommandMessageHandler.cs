using Org.BouncyCastle.Security;
using Zen.Fs.Definition;
using Zen.Game.Model;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Msg.Handler
{
    public class CommandMessageHandler : MessageHandler<CommandMessage>
    {
        public override void Handle(Player player, CommandMessage message)
        {
            var name = message.Name;
            var arguments = message.Arguments;

            if (name.Equals("item"))
            {
                var id = int.Parse(arguments[0]);
                var amount = 1;
                if (arguments.Length > 1)
                    amount = int.Parse(arguments[1]);

                player.Inventory.Add(new Item(id, amount));
                player.SendGameMessage($"Spawned {amount:N0} x {ItemDefinition.ForId(id).Name}");
            }

            if (name.Equals("random_stats"))
            {
                var random = new SecureRandom();
                for (var id = 0; id < 24; id++)
                    player.SkillSet.SetLevelAndExperience((Skill) id, random.Next(1, 99));
            }

            if (name.Equals("equip"))
            {
                var id = int.Parse(arguments[0]);
                var slot = int.Parse(arguments[1]);

                player.Equipment.Set(slot, new Item(id));
            }
        }
    }
}