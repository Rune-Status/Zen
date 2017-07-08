using DotNetty.Buffers;
using Zen.Builder;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Msg.Encoder
{
    public class InterfaceItemsMessageEncoder : MessageEncoder<InterfaceItemsMessage>
    {
        public override GameFrame Encode(IByteBufferAllocator alloc, InterfaceItemsMessage message)
        {
            var items = message.Items;

            var builder = new GameFrameBuilder(alloc, 105, FrameType.VariableShort);
            builder.Put(DataType.Int, (message.Id << 16) | message.Slot);
            builder.Put(DataType.Short, message.Type);
            builder.Put(DataType.Short, items.Length);

            foreach (var item in items)
            {
                if (item == null)
                {
                    builder.Put(DataType.Byte, DataTransformation.Subtract, 0);
                    builder.Put(DataType.Short, 0);
                }
                else
                {
                    var amount = item.Amount;
                    if (amount >= 255)
                    {
                        builder.Put(DataType.Byte, DataTransformation.Subtract, 255);
                        builder.Put(DataType.Int, amount);
                    }
                    else
                    {
                        builder.Put(DataType.Byte, DataTransformation.Subtract, amount);
                    }
                    builder.Put(DataType.Short, item.Id + 1);
                }
            }

            return builder.ToGameFrame();
        }
    }
}