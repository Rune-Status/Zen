using System;
using System.Collections.Generic;
using Zen.Game.IO;
using Zen.Game.Model;
using Zen.Game.Msg.Decoder;
using Zen.Game.Msg.Encoder;
using Zen.Game.Msg.Handler;

namespace Zen.Game.Msg
{
    public class MessageRepository
    {
        public MessageRepository(LandscapeKeyTable keyTable)
        {
            Bind(new WalkMessageDecoder(39));
            Bind(new WalkMessageDecoder(77));
            Bind(new WalkMessageDecoder(215));
            Bind(new CommandMessageDecoder());
            Bind(new ChatMessageDecoder());
            Bind(new ButtonMessageDecoder(10));
            Bind(new ButtonMessageDecoder(155));
            Bind(new SwapItemsMessageDecoder());
            Bind(new EquipItemMessageDecoder());
            Bind(new RemoveItemMessageDecoder());

            Bind(new RegionChangeMessageEncoder(keyTable));
            Bind(new InterfaceRootMessageEncoder());
            Bind(new PlayerUpdateMessageEncoder());
            Bind(new InterfaceOpenMessageEncoder());
            Bind(new InterfaceCloseMessageEncoder());
            Bind(new GameMessageEncoder());
            Bind(new SkillMessageEncoder());
            Bind(new InterfaceItemsMessageEncoder());
            Bind(new InterfaceResetItemsMessageEncoder());
            Bind(new InterfaceSlottedItemsMessageEncoder());
            Bind(new InterfaceTextMessageEncoder());

            Bind(new WalkMessageHandler());
            Bind(new CommandMessageHandler());
            Bind(new ChatMessageHandler());
            Bind(new ButtonMessageHandler());
            Bind(new SwapItemsMessageHandler());
            Bind(new EquipItemMessageHandler());
            Bind(new RemoveItemMessageHandler());
        }

        public Dictionary<int, object> InCodecs { get; } = new Dictionary<int, object>();
        public Dictionary<Type, object> OutCodecs { get; } = new Dictionary<Type, object>();
        public Dictionary<Type, object> Handlers { get; } = new Dictionary<Type, object>();

        public void Bind<T>(MessageDecoder<T> decoder) where T : Message => InCodecs[decoder.Opcode] = decoder;
        public void Bind<T>(MessageEncoder<T> encoder) where T : Message => OutCodecs[encoder.MessageType] = encoder;
        public void Bind<T>(MessageHandler<T> handler) where T : Message => Handlers[handler.MessageType] = handler;

        public void Handle(Player player, Message message)
        {
            Handlers.TryGetValue(message.GetType(), out dynamic handler);
            handler?.Handle(player, (dynamic) message);
        }
    }
}