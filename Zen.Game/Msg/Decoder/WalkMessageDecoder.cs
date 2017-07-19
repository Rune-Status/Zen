using Zen.Builder;
using Zen.Game.Msg.Impl;

namespace Zen.Game.Msg.Decoder
{
    public class WalkMessageDecoder : MessageDecoder<WalkMessage>
    {
        public WalkMessageDecoder(int opcode) : base(opcode)
        {
        }

        public override WalkMessage Decode(GameFrame frame)
        {
            var reader = new GameFrameReader(frame);

            var anticheat = frame.Opcode == 39;
            var stepCount = (reader.GetLength() - (anticheat ? 19 : 5)) / 2 + 1;
            var running = reader.GetUnsigned(DataType.Byte, transformation: DataTransformation.Add) == 1;
            var x = (int)reader.GetUnsigned(DataType.Short);
            var y = (int)reader.GetUnsigned(DataType.Short, transformation: DataTransformation.Add);

            var steps = new WalkMessage.Step[stepCount];
            steps[0] = new WalkMessage.Step(x, y);
            for (var i = 1; i < stepCount; i++)
            {
                var stepX = x + (int)reader.GetSigned(DataType.Byte, transformation: DataTransformation.Add);
                var stepY = y + (int)reader.GetSigned(DataType.Byte, transformation: DataTransformation.Subtract);
                steps[i] = new WalkMessage.Step(stepX, stepY);
            }

            return new WalkMessage(steps, running);
        }
    }
}