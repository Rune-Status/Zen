﻿using Zen.Game.Model.Player;
using Zen.Game.Plugin.Type;

namespace Zen.Game.Plugin.Buttons
{
    public class EmoteButtonPlugin : IButtonPlugin
    {
        public void Initiate(PluginRepository repository)
        {
            repository.Register(this, Interface.Emotes);
        }

        public void OnButtonClick(Player player, int id, int slot, int parameter)
        {
        }
    }
}