using Zen.Game.Model;

namespace Zen.Game.Msg.Impl
{
    public class InterfaceRootMessage : Message
    {
        public InterfaceRootMessage(InterfaceSet.Interface @interface)
        {
            Interface = @interface;
        }

        public InterfaceSet.Interface Interface { get; }
    }
}