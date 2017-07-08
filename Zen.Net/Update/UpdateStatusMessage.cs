namespace Zen.Net.Update
{
    public class UpdateStatusMessage
    {
        public const int StatusOk = 0;
        public const int StatusOutOfDate = 6;

        public UpdateStatusMessage(int status)
        {
            Status = status;
        }

        public int Status { get; }
    }
}