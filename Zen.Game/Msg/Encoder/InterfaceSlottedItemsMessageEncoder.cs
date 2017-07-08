using DotNetty.Buffers;
using Zen.Builder;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Msg.Encoder
{
    public class InterfaceSlottedItemsMessageEncoder : MessageEncoder<InterfaceSlottedItemsMessage>
    {
        public override GameFrame Encode(IByteBufferAllocator alloc, InterfaceSlottedItemsMessage message)
        {
            var items = message.Items;

            var builder = new GameFrameBuilder(alloc, 22, FrameType.VariableShort);
            builder.Put(DataType.Int, (message.Id << 16) | message.Slot);
            builder.Put(DataType.Short, message.Type);

            foreach (var slottedItem in items)
            {
                var slot = slottedItem.Slot;
                builder.PutSmart(slot);

                var item = slottedItem.Item;
                if (item == null)
                {
                    builder.Put(DataType.Short, 0);
                }
                else
                {
                    var amount = item.Amount;
                    builder.Put(DataType.Short, item.Id + 1);
                    if (amount >= 255)
                    {
                        builder.Put(DataType.Byte, 255);
                        builder.Put(DataType.Int, amount);
                    }
                    else
                    {
                        builder.Put(DataType.Byte, amount);
                    }
                }
            }

            return builder.ToGameFrame();
        }
    }
}