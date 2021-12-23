namespace esh
{
    public enum Side
    {
        WHITE = 1,
        BLACK = 2,
    }

    public static class SideUtil
    {
        public static Side OpponentOf(Side side) => side == Side.WHITE ? Side.BLACK : Side.WHITE;
    }
}
