﻿using Zen.Game.Model;

namespace Zen.Game.IO.Json
{
    public abstract class Column
    {
        public abstract void Load(dynamic playerObject, Player player);

        public abstract void Save(dynamic playerObject, Player player);
    }
}