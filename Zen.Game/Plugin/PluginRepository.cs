using System;
using System.Collections.Generic;
using System.Linq;
using Zen.Game.Model.Player;
using Zen.Game.Plugin.Type;
using Zen.Util;

namespace Zen.Game.Plugin
{
    public class PluginRepository
    {
        private readonly Dictionary<int, IButtonPlugin> _buttonPlugins = new Dictionary<int, IButtonPlugin>();
        private readonly Dictionary<string, ICommandPlugin> _commandPlugins = new Dictionary<string, ICommandPlugin>();

        public PluginRepository()
        {
            #region Load button plugins

            foreach (var type in AssemblyUtil.GetTypesWithInterface<IButtonPlugin>())
            {
                var plugin = Activator.CreateInstance(type) as IButtonPlugin;
                plugin?.Initiate(this);
            }

            #endregion

            #region Load command plugins

            foreach (var type in AssemblyUtil.GetTypesWithInterface<ICommandPlugin>())
            {
                var plugin = Activator.CreateInstance(type) as ICommandPlugin;
                plugin?.Initiate(this);
            }

            #endregion
        }

        public bool OnButtonClick(Player player, int id, int slot, int parameter)
        {
            var plugin = _buttonPlugins.FirstOrDefault(x => x.Key == id).Value;
            if (plugin == null)
                return false;

            plugin.OnButtonClick(player, id, slot, parameter);
            return true;
        }

        public bool OnCommand(Player player, string keyword, string[] arguments)
        {
            var plugin = _commandPlugins.FirstOrDefault(x => x.Key == keyword).Value;
            if (plugin == null)
                return false;

            plugin.OnCommand(player, keyword, arguments);
            return true;
        }

        public void Register(IButtonPlugin plugin, params int[] parents) => parents.ToList()
            .ForEach(id => _buttonPlugins[id] = plugin);

        public void Register(ICommandPlugin plugin, params string[] keywords) => keywords.ToList()
            .ForEach(keyword => _commandPlugins[keyword] = plugin);
    }
}