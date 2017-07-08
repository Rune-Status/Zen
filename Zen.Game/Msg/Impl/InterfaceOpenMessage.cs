using Zen.Game.Model;

namespace Zen.Game.Msg.Impl
{
    public class InterfaceOpenMessage : Message
    {
        public InterfaceOpenMessage(InterfaceSet.Interface @interface)
        {
            Interface = @interface;
        }

        public InterfaceSet.Interface Interface { get; }
    }
}