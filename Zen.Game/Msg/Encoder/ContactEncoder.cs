using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using DotNetty.Buffers;
using Zen.Builder;
using Zen.Game.Model.Player;
using Zen.Game.Model.Player.Communication;
using Zen.Game.Msg.Impl;
using Zen.Util;

namespace Zen.Game.Msg.Encoder
{
    public class ContactEncoder : MessageEncoder<ContactMessage>
    {

        public override GameFrame Encode(IByteBufferAllocator alloc, ContactMessage contact)
        {
            var player = contact.Player;
            GameFrameBuilder builder = null;
            switch (contact.Type)
            {
                case ContactMessage.UpdateStatusType:
                    builder = new GameFrameBuilder(alloc, 197).Put(DataType.Int, 1);
                    break;
                case ContactMessage.IgnoreListType:
                    builder = new GameFrameBuilder(alloc, 126, FrameType.VariableShort);
                    foreach (var ignored in player.ContactManager.Ignored)
                    {
                        if (ignored.Length == 0)
                            continue;
                        builder.Put(DataType.Long, StringUtil.stringToLong(ignored));
                    }
                    break;
                case ContactMessage.UpdateFriendType:
                    builder = new GameFrameBuilder(alloc, 62, FrameType.VariableByte);
                    builder.Put(DataType.Long, StringUtil.stringToLong(contact.Name));
                    builder.Put(DataType.Short, contact.WorldId);

                    var c = player.ContactManager.Contacts[contact.Name];
                    if (c != null)
                    {
                        // TODO: ranks
                        builder.Put(DataType.Byte, 1);
                    }
                    else
                    {
                        builder.Put(DataType.Byte, 0);
                    }
                    if (contact.IsOnline)
                    {
                        builder.PutString("World " + contact.WorldId);
                    }
                    break;
            }
            return builder.ToGameFrame();
        }
    }
}
