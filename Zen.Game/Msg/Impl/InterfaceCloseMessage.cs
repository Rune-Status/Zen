﻿using Zen.Game.Model;

namespace Zen.Game.Msg.Impl
{
    public class InterfaceCloseMessage : Message
    {
        public InterfaceCloseMessage(InterfaceSet.Interface @interface)
        {
            Interface = @interface;
        }

        public InterfaceSet.Interface Interface { get; }
    }
}