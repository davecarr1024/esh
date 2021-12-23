using System;
using System.Collections.Generic;
using System.Linq;

namespace esh
{
    public interface Piece
    {
        Side Side { get; }

        Position Position { get; }

        IEnumerable<Board> Moves(Board board);

        Piece WithPosition(Position position);
    }

    internal static class PieceUtil
    {
        private static IEnumerable<Board> LinearMoves(Piece piece, Board board, Position.Delta delta)
        {
            IList<Board> boards = new List<Board>();
            Position position = piece.Position;
            while (position.CanAdd(delta))
            {
                position = position + delta;
                Piece to_piece = board.PieceAtPosition(position);
                if (to_piece == null)
                {
                    boards.Add(board.WithPieceMoved(piece, position));
                }
                else
                {
                    if (to_piece.Side != piece.Side)
                    {
                        boards.Add(board.WithPieceMoved(piece, position));
                    }
                    break;
                }
            }
            return boards;
        }

        public static IEnumerable<Board> LinearMoves(Piece piece, Board board, IEnumerable<Position.Delta> deltas)
            => deltas.SelectMany(delta => LinearMoves(piece, board, delta));

        public enum CaptureBehavior
        {
            MOVE_OR_CAPTURE = 1,
            MOVE_ONLY = 2,
            CAPTURE_ONLY = 3,
        }

        private static bool CanMoveTo(Piece piece, Board board, Position position, CaptureBehavior capture_behavior)
        {
            Piece to_piece = board.PieceAtPosition(position);
            switch (capture_behavior)
            {
                case CaptureBehavior.MOVE_OR_CAPTURE:
                    return to_piece == null || to_piece.Side != piece.Side;
                case CaptureBehavior.MOVE_ONLY:
                    return to_piece == null;
                case CaptureBehavior.CAPTURE_ONLY:
                    return to_piece != null && to_piece.Side != piece.Side;
                default:
                    throw new NotSupportedException(capture_behavior.ToString());
            }
        }

        public static Board? DeltaMove(Piece piece, Board board, Position.Delta delta, CaptureBehavior capture_behavior)
        {
            if (piece.Position.CanAdd(delta))
            {
                Position to_position = piece.Position + delta;
                if (CanMoveTo(piece, board, to_position, capture_behavior))
                {
                    return board.WithPieceMoved(piece, to_position);
                }
            }
            return null;
        }

        public static IEnumerable<Board> DeltaMoves(
            Piece piece,
            Board board,
            IEnumerable<Position.Delta> deltas,
            CaptureBehavior capture_behavior)
            => deltas.Select(delta => DeltaMove(piece, board, delta, capture_behavior))
                     .Where(b => b.HasValue)
                     .Select(b => b.Value);
    }

    public readonly struct Pawn : Piece
    {
        public Side Side { get; }

        public Position Position { get; }

        public bool HasMoved { get; }

        private readonly static IEnumerable<Position.Delta> Deltas
            = new List<Position.Delta> {
                new Position.Delta(-1,-1),
                new Position.Delta(1,-1),
                new Position.Delta(-1,1),
                new Position.Delta(1,1),
            };

        public Pawn(Side side, Position position) : this(side, position, false) { }

        private Pawn(Side side, Position position, bool has_moved)
        {
            Side = side;
            Position = position;
            HasMoved = has_moved;
        }

        private IEnumerable<Board> MovesForCapture(int dx, int dy, Board board)
            => PieceUtil.DeltaMoves(
                    this,
                    board,
                    new List<Position.Delta> { new Position.Delta(dx, dy) },
                    PieceUtil.CaptureBehavior.CAPTURE_ONLY);

        public IEnumerable<Board> Moves(Board board)
        {
            List<Board> boards = new List<Board>();
            int dy = Side == Side.WHITE ? 1 : -1;
            boards.AddRange(PieceUtil.DeltaMoves(
                this,
                board,
                new List<Position.Delta> { new Position.Delta(0, dy) },
                PieceUtil.CaptureBehavior.MOVE_ONLY));
            if (!HasMoved)
            {
                boards.AddRange(PieceUtil.DeltaMoves(
                    this,
                    board,
                    new List<Position.Delta> { new Position.Delta(0, dy * 2) },
                    PieceUtil.CaptureBehavior.MOVE_ONLY));
            }
            boards.AddRange(MovesForCapture(-1, dy, board));
            boards.AddRange(MovesForCapture(1, dy, board));
            return boards;
        }

        public Piece WithPosition(Position position) => new Pawn(Side, position, true);

        public override bool Equals(object obj)
        {
            return obj is Pawn pawn &&
                   Side == pawn.Side &&
                   EqualityComparer<Position>.Default.Equals(Position, pawn.Position) &&
                   HasMoved == pawn.HasMoved;
        }

        public override int GetHashCode()
        {
            int hashCode = -1155411006;
            hashCode = hashCode * -1521134295 + Side.GetHashCode();
            hashCode = hashCode * -1521134295 + Position.GetHashCode();
            hashCode = hashCode * -1521134295 + HasMoved.GetHashCode();
            return hashCode;
        }
    }

    public readonly struct Bishop : Piece
    {
        public Side Side { get; }
        public Position Position { get; }

        private readonly static IEnumerable<Position.Delta> Deltas
            = new List<Position.Delta> {
                new Position.Delta(-1,-1),
                new Position.Delta(1,-1),
                new Position.Delta(-1,1),
                new Position.Delta(1,1),
            };

        public Bishop(Side side, Position position)
        {
            Side = side;
            Position = position;
        }

        public IEnumerable<Board> Moves(Board board) => PieceUtil.LinearMoves(this, board, Deltas);

        public Piece WithPosition(Position position) => new Bishop(Side, position);

        public override bool Equals(object obj)
        {
            return obj is Bishop bishop &&
                   Side == bishop.Side &&
                   EqualityComparer<Position>.Default.Equals(Position, bishop.Position);
        }

        public override int GetHashCode()

        {
            int hashCode = -2040911528;
            hashCode = hashCode * -1521134295 + Side.GetHashCode();
            hashCode = hashCode * -1521134295 + Position.GetHashCode();
            return hashCode;
        }
    }

    public readonly struct Rook : Piece
    {
        public Side Side { get; }
        public Position Position { get; }

        private readonly static IEnumerable<Position.Delta> Deltas
            = new List<Position.Delta> {
                new Position.Delta(0,1),
                new Position.Delta(0,-1),
                new Position.Delta(1,0),
                new Position.Delta(-1,0),
            };

        public Rook(Side side, Position position)
        {
            Side = side;
            Position = position;
        }

        public IEnumerable<Board> Moves(Board board) => PieceUtil.LinearMoves(this, board, Deltas);

        public Piece WithPosition(Position position) => new Rook(Side, position);

        public override bool Equals(object obj)
        {
            return obj is Rook rook &&
                   Side == rook.Side &&
                   EqualityComparer<Position>.Default.Equals(Position, rook.Position);
        }

        public override int GetHashCode()

        {
            int hashCode = -2040911528;
            hashCode = hashCode * -1521134295 + Side.GetHashCode();
            hashCode = hashCode * -1521134295 + Position.GetHashCode();
            return hashCode;
        }
    }

    public readonly struct Queen : Piece
    {
        public Side Side { get; }
        public Position Position { get; }

        private readonly static IEnumerable<Position.Delta> Deltas
            = new List<Position.Delta> {
                new Position.Delta(-1,-1),
                new Position.Delta(1,-1),
                new Position.Delta(-1,1),
                new Position.Delta(1,1),
                new Position.Delta(0,1),
                new Position.Delta(0,-1),
                new Position.Delta(1,0),
                new Position.Delta(-1,0),
            };

        public Queen(Side side, Position position)
        {
            Side = side;
            Position = position;
        }

        public IEnumerable<Board> Moves(Board board) => PieceUtil.LinearMoves(this, board, Deltas);

        public Piece WithPosition(Position position) => new Queen(Side, position);

        public override bool Equals(object obj)
        {
            return obj is Queen queen &&
                   Side == queen.Side &&
                   EqualityComparer<Position>.Default.Equals(Position, queen.Position);
        }

        public override int GetHashCode()

        {
            int hashCode = -2040911528;
            hashCode = hashCode * -1521134295 + Side.GetHashCode();
            hashCode = hashCode * -1521134295 + Position.GetHashCode();
            return hashCode;
        }
    }

    public readonly struct King : Piece
    {
        public Side Side { get; }
        public Position Position { get; }

        private readonly static IEnumerable<Position.Delta> Deltas
            = new List<Position.Delta> {
                new Position.Delta(0,1),
                new Position.Delta(1,1),
                new Position.Delta(1,0),
                new Position.Delta(1,-1),
                new Position.Delta(0,-1),
                new Position.Delta(-1,-1),
                new Position.Delta(-1,0),
                new Position.Delta(-1,1),
            };

        public King(Side side, Position position)
        {
            Side = side;
            Position = position;
        }

        public IEnumerable<Board> Moves(Board board) 
            => PieceUtil.DeltaMoves(this, board, Deltas, PieceUtil.CaptureBehavior.MOVE_OR_CAPTURE);

        public Piece WithPosition(Position position) => new King(Side, position);

        public override bool Equals(object obj)
        {
            return obj is King king &&
                   Side == king.Side &&
                   EqualityComparer<Position>.Default.Equals(Position, king.Position);
        }

        public override int GetHashCode()

        {
            int hashCode = -2040911528;
            hashCode = hashCode * -1521134295 + Side.GetHashCode();
            hashCode = hashCode * -1521134295 + Position.GetHashCode();
            return hashCode;
        }
    }
}
