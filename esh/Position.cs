using System;

namespace esh
{
    public readonly struct Position
    {
        public int X { get; }

        public int Y { get; }

        public Position(int x, int y)
        {
            if (x < 0 || x >= 8 || y < 0 || y >= 8)
            {
                throw new ArgumentOutOfRangeException();
            }
            X = x;
            Y = y;
        }

        public override bool Equals(object obj) => obj is Position position && Equals(position);

        public bool Equals(Position p) => p.X == X && p.Y == Y;

        public override int GetHashCode() => (X, Y).GetHashCode();

        public static bool operator ==(Position lhs, Position rhs) => lhs.Equals(rhs);

        public static bool operator !=(Position lhs, Position rhs) => !(lhs == rhs);

        public bool CanAdd(Delta delta) => X + delta.X >= 0 && X + delta.X < 8 && Y + delta.Y >= 0 && Y + delta.Y < 8;

        public static Position operator +(Position position, Delta delta)
        {
            if (!position.CanAdd(delta))
            {
                throw new ArgumentOutOfRangeException();
            }
            return new Position(position.X + delta.X, position.Y + delta.Y);
        }

        public static Position operator -(Position position, Delta delta) => position + -delta;

        public readonly struct Delta
        {
            public int X { get; }

            public int Y { get; }

            public Delta(int x, int y) { X = x; Y = y; }

            public override int GetHashCode() => (X, Y).GetHashCode();

            public override bool Equals(object obj) => obj is Delta d && Equals(d);

            public bool Equals(Delta d) => X == d.X && Y == d.Y;

            public static bool operator ==(Delta lhs, Delta rhs) => lhs.Equals(rhs);

            public static bool operator !=(Delta lhs, Delta rhs) => !(lhs == rhs);

            public static Delta operator -(Delta delta) => new Delta(-delta.X, -delta.Y);
        }
    }
}
