namespace Zen.Game.Model.Player
{
    public class Appearance
    {
        public static readonly Appearance DefaultAppearance = new Appearance(
            Gender.Male,
            new[] {3, 14, 18, 26, 34, 38, 42},
            new[] {3, 16, 16, 0, 0});

        private static int _ticketCounter;

        public Appearance(Gender gender, int[] style, int[] colors)
        {
            Gender = gender;
            Style = style;
            Colors = colors;
        }

        public int TicketId { get; private set; } = NextTicketId();

        public Gender Gender { get; set; }
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