namespace Zen.Game.Model
{
    public class Appearance
    {
        public static readonly Appearance DefaultAppearance = new Appearance(
            Gender.Male,
            new[] {0, 10, 18, 26, 33, 36, 42},
            new[] {2, 5, 8, 11, 14});

        private static int _ticketCounter;

        public Appearance(Gender gender, int[] style, int[] colors)
        {
            Gender = gender;
            Style = style;
            Colors = colors;
        }

        public int TicketId { get; private set; } = NextTicketId();
        public Gender Gender { get; }
        public int[] Colors { get; }
        public int[] Style { get; }

        private static int NextTicketId()
        {
            if (++_ticketCounter == 0)
                _ticketCounter = 1;
            return _ticketCounter;
        }

        public void ResetTicketId() => TicketId = NextTicketId();
    }
}