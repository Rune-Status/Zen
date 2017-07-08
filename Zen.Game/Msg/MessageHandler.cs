﻿using System;
using Zen.Game.Model;

namespace Zen.Game.Msg
{
    public abstract class MessageHandler<T> where T : Message
    {
        public Type MessageType => typeof(T);

        public abstract void Handle(Player player, T message);
    }
}