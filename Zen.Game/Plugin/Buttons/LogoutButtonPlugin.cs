﻿using Zen.Game.Model;
using Zen.Game.Plugin.Type;

namespace Zen.Game.Plugin.Buttons
{
    public class LogoutButtonPlugin : IButtonPlugin
    {
        public void Initiate(PluginRepository repository)
        {
            repository.Register(this, Interfaces.Logout);
        }

        public void OnButtonClick(Player player, int id, int slot, int parameter)
        {
            if (slot == 6)
            {
                player.Logout();
            }
        }
    }
}