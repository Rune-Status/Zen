namespace Zen.Net.World
{
    public class Country
    {
        public const int FlagUk = 77;
        public const int FlagUsa = 225;
        public const int FlagCanada = 38;
        public const int FlagNetherlands = 161;
        public const int FlagAustralia = 16;
        public const int FlagSweden = 191;
        public const int FlagFinland = 69;
        public const int FlagIreland = 101;
        public const int FlagBelgium = 22;
        public const int FlagNorway = 162;
        public const int FlagDenmark = 58;
        public const int FlagBrazil = 31;
        public const int FlagMexico = 152;

        public Country(int flag, string name)
        {
            Flag = flag;
            Name = name;
        }

        public string Name { get; }
        public int Flag { get; }
    }
}