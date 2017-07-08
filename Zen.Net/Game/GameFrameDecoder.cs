using System.Collections.Generic;
using System.IO;
using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using Zen.Builder;

namespace Zen.Net.Game
{
    public class GameFrameDecoder : ByteToMessageDecoder
    {
        private static readonly int[] Sizes = new int[256];
        private int _opcode, _size;

        private State _state = State.ReadOpcode;
        private bool _variable;

        static GameFrameDecoder()
        {
            Sizes[0] = 0;
            Sizes[1] = -3;
            Sizes[2] = -3;
            Sizes[3] = 2; // Attack NPC
            Sizes[4] = -3;
            Sizes[5] = -3;
            Sizes[6] = -3;
            Sizes[7] = -3;
            Sizes[8] = -3;
            Sizes[9] = -3;
            Sizes[10] = 4; // Actionbuttons #2
            Sizes[11] = -3;
            Sizes[12] = -3;
            Sizes[13] = -3;
            Sizes[14] = -3;
            Sizes[15] = -3;
            Sizes[16] = -3;
            Sizes[17] = -3;
            Sizes[18] = -3;
            Sizes[19] = -3;
            Sizes[20] = -3;
            Sizes[21] = 4; // Camera
            Sizes[22] = 1; // Focus
            Sizes[23] = 4; // Enter amount
            Sizes[24] = -3;
            Sizes[25] = -3;
            Sizes[26] = -3;
            Sizes[27] = 16; // Item on item
            Sizes[28] = -3;
            Sizes[29] = -3;
            Sizes[30] = 2; // Fourth click NPC (trade slayermaster).
            Sizes[31] = -3;
            Sizes[32] = -3;
            Sizes[33] = -3;
            Sizes[34] = 8; // Add ignore
            Sizes[35] = -3;
            Sizes[36] = -3;
            Sizes[37] = -3;
            Sizes[38] = -3;
            Sizes[39] = -1; // Walk
            Sizes[40] = -3;
            Sizes[41] = -3;
            Sizes[42] = -3;
            Sizes[43] = -3;
            Sizes[44] = -1; // Command
            Sizes[45] = -3;
            Sizes[46] = -3;
            Sizes[47] = -3;
            Sizes[48] = -3;
            Sizes[49] = -3;
            Sizes[50] = -3;
            Sizes[51] = -3;
            Sizes[52] = -3;
            Sizes[53] = 6; // Interface option #9
            Sizes[54] = -3;
            Sizes[55] = 8; // Equip item
            Sizes[56] = -3;
            Sizes[57] = 8; // Delete friend
            Sizes[58] = -3;
            Sizes[59] = -3;
            Sizes[60] = -3;
            Sizes[61] = -3;
            Sizes[62] = -3;
            Sizes[63] = -3;
            Sizes[64] = 6; // Interface option #8
            Sizes[65] = -3;
            Sizes[66] = 6; // Pick up item
            Sizes[67] = -3;
            Sizes[68] = 2; // Attack player
            Sizes[69] = -3;
            Sizes[70] = -3;
            Sizes[71] = 2; // Trade player
            Sizes[72] = -3;
            Sizes[73] = -3;
            Sizes[74] = -3;
            Sizes[75] = 6; // Mouse click
            Sizes[76] = -3;
            Sizes[77] = -1; // Walk
            Sizes[78] = 2; // Second click NPC
            Sizes[79] = 12; // Swapping inventory places in shop, bank and duel
            Sizes[80] = -3;
            Sizes[81] = 8; // Unequip item
            Sizes[82] = -3;
            Sizes[83] = -3;
            Sizes[84] = 6; // Object third click
            Sizes[85] = -3;
            Sizes[86] = -3;
            Sizes[87] = -3;
            Sizes[88] = -3;
            Sizes[89] = -3;
            Sizes[90] = -3;
            Sizes[91] = -3;
            Sizes[92] = 2; // Inventory item examine.
            Sizes[93] = 0; // Ping
            Sizes[94] = 2; // ?
            Sizes[95] = -3;
            Sizes[96] = -3;
            Sizes[97] = -3;
            Sizes[98] = 4; // Toggle sound setting
            Sizes[99] = 10; // ?
            Sizes[100] = -3;
            Sizes[101] = -3;
            Sizes[102] = -3;
            Sizes[103] = -3;
            Sizes[104] = 8; // Join clan chat
            Sizes[105] = -3;
            Sizes[106] = 2; // Follow player
            Sizes[107] = -3;
            Sizes[108] = -3;
            Sizes[109] = -3;
            Sizes[110] = 0; // Region loading, size varies
            Sizes[111] = 2; // Grand Exchange item search
            Sizes[112] = -3;
            Sizes[113] = -3;
            Sizes[114] = -3;
            Sizes[115] = 10; // Use item on npc
            Sizes[116] = -3;
            Sizes[117] = -3;
            Sizes[118] = -3;
            Sizes[119] = -3;
            Sizes[120] = 8; // Add friend
            Sizes[121] = -3;
            Sizes[122] = -3;
            Sizes[123] = -3;
            Sizes[124] = 6; //Interface option #3
            Sizes[125] = -3;
            Sizes[126] = -3;
            Sizes[127] = -3;
            Sizes[128] = -3;
            Sizes[129] = -3;
            Sizes[130] = -3;
            Sizes[131] = -3;
            Sizes[132] = 6; // Actionbuttons #3
            Sizes[133] = -3;
            Sizes[134] = 14; // Item on object
            Sizes[135] = 8; // Drop item
            Sizes[136] = -3;
            Sizes[137] = 4; // Unknown, nothing major
            Sizes[138] = -3;
            Sizes[139] = -3;
            Sizes[140] = -3;
            Sizes[141] = -3;
            Sizes[142] = -3;
            Sizes[143] = -3;
            Sizes[144] = -3;
            Sizes[145] = -3;
            Sizes[146] = -3;
            Sizes[147] = -3;
            Sizes[148] = 2; // Third click NPC
            Sizes[149] = -3;
            Sizes[150] = -3;
            Sizes[151] = -3;
            Sizes[152] = -3;
            Sizes[153] = 8; // Inventory click item #2 (check RC pouch)
            Sizes[154] = -3;
            Sizes[155] = 6; // Actionbutton
            Sizes[156] = 8; // Inventory click item (food etc)
            Sizes[157] = 3; // Privacy options
            Sizes[158] = -3;
            Sizes[159] = -3;
            Sizes[160] = -3;
            Sizes[161] = 8; // Item right click option #1 (rub/empty)
            Sizes[162] = 8; // Clan chat kick
            Sizes[163] = -3;
            Sizes[164] = -3;
            Sizes[165] = -3;
            Sizes[166] = 6; // Interface option #7
            Sizes[167] = -1;
            Sizes[168] = 6; // Interface option #6
            Sizes[169] = -3;
            Sizes[170] = -3;
            Sizes[171] = -3;
            Sizes[172] = -3;
            Sizes[173] = -3;
            Sizes[174] = -3;
            Sizes[175] = -3;
            Sizes[176] = -3;
            Sizes[177] = 2; // Junk, no real purpose
            Sizes[178] = -3;
            Sizes[179] = -3;
            Sizes[180] = 2; // Accept trade (chatbox)
            Sizes[181] = -3;
            Sizes[182] = -3;
            Sizes[183] = -3;
            Sizes[184] = 0; // Close interface
            Sizes[185] = -3;
            Sizes[186] = -3;
            Sizes[187] = -3;
            Sizes[188] = 9; // Clan ranks
            Sizes[189] = -3;
            Sizes[190] = -3;
            Sizes[191] = -3;
            Sizes[192] = -3;
            Sizes[193] = -3;
            Sizes[194] = 6; // Object second click
            Sizes[195] = 8; // Magic on player
            Sizes[196] = 6; // Interface option #2
            Sizes[197] = -3;
            Sizes[198] = -3;
            Sizes[199] = 6; //Interface option #4
            Sizes[200] = -3;
            Sizes[201] = -1; // Send PM
            Sizes[202] = -3;
            Sizes[203] = -3;
            Sizes[204] = -3;
            Sizes[205] = -3;
            Sizes[206] = 8; // Operate item
            Sizes[207] = -3;
            Sizes[208] = -3;
            Sizes[209] = -3;
            Sizes[210] = -3;
            Sizes[211] = -3;
            Sizes[212] = -3;
            Sizes[213] = 8; // Delete ignore
            Sizes[214] = -3;
            Sizes[215] = -1; // Walk
            Sizes[216] = -3;
            Sizes[217] = -3;
            Sizes[218] = 2; // Fifth click NPC
            Sizes[219] = -3;
            Sizes[220] = -3;
            Sizes[221] = -3;
            Sizes[222] = -3;
            Sizes[223] = -3;
            Sizes[224] = -3;
            Sizes[225] = -3;
            Sizes[226] = -3;
            Sizes[227] = -3;
            Sizes[228] = -3;
            Sizes[229] = -3;
            Sizes[230] = -3;
            Sizes[231] = 9; // Swap item slot
            Sizes[232] = -3;
            Sizes[233] = -3;
            Sizes[234] = 6; //Interface option #5
            Sizes[235] = -3;
            Sizes[236] = -3;
            Sizes[237] = -1; // Public chat
            Sizes[238] = -3;
            Sizes[239] = 8; // Magic on NPC
            Sizes[240] = -3;
            Sizes[241] = -3;
            Sizes[242] = -3;
            Sizes[243] = 6; // Screen type (fullscreen, small HD etc)
            Sizes[244] = 8; // Enter text
            Sizes[245] = 0; // Idle logout
            Sizes[246] = -3;
            Sizes[247] = 6; // Object 4th option
            Sizes[248] = -3;
            Sizes[249] = -3;
            Sizes[250] = -3;
            Sizes[251] = -3;
            Sizes[252] = -3;
            Sizes[253] = -3;
            Sizes[254] = 6; // First click object
            Sizes[255] = -3;
        }

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            if (_state == State.ReadOpcode)
            {
                if (!input.IsReadable())
                    return;

                _opcode = input.ReadByte() & 0xFF;
                _size = Sizes[_opcode];

                if (_size == -3)
                    throw new IOException($"Illegal opcode {_opcode}.");

                _variable = _size == -1;
                _state = _variable ? State.ReadSize : State.ReadPayload;
            }

            if (_state == State.ReadSize)
            {
                if (!input.IsReadable())
                    return;

                _size = input.ReadByte() & 0xFF;
                _state = State.ReadPayload;
            }

            if (_state == State.ReadPayload)
            {
                if (input.ReadableBytes < _size)
                    return;

                var payload = input.ReadBytes(_size);
                _state = State.ReadOpcode;

                output.Add(new GameFrame(_opcode,
                    _variable ? FrameType.VariableByte : FrameType.Fixed, payload));
            }
        }

        private enum State
        {
            ReadOpcode,
            ReadSize,
            ReadPayload
        }
    }
}