using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace esh
{
    public readonly struct Board
    {
        public IReadOnlyCollection<Piece> Pieces { get; }

        public Board(IList<Piece> pieces)
        {
            Pieces = new ReadOnlyCollection<Piece>(pieces);
            if (Pieces.Select(piece => piece.Position).Distinct().Count() != Pieces.Count())
            {
                throw new ArgumentException("duplicate positions");
            }
        }

        public override bool Equals(object obj) => obj is Board board && Equals(board);

        public bool Equals(Board board) => Pieces.Equals(board.Pieces);

        public override int GetHashCode() => Pieces.GetHashCode();

        public Board WithPiece(Piece piece)
        {
            IList<Piece> pieces = new List<Piece>(Pieces);
            pieces.Add(piece);
            return new Board(pieces);
        }

        public Board WithoutPiece(Piece piece) => new Board(new List<Piece>(Pieces.Where(p => p != piece)));

        public Board WithPieceMoved(Piece piece, Position position)
            => WithoutPiece(PieceAtPosition(position))
                .WithoutPiece(piece)
                .WithPiece(piece.WithPosition(position));

        public IEnumerable<Piece> PiecesForSide(Side side) => Pieces.Where(piece => piece.Side == side);

        public Piece PieceAtPosition(Position position) => Pieces.FirstOrDefault(piece => piece.Position == position);

        public IEnumerable<Board> MovesForPiece(Piece piece) => piece.Moves(this);

        public IEnumerable<Board> MovesForSide(Side side)
        {
            Board board = this;
            return PiecesForSide(side).SelectMany(piece => board.MovesForPiece(piece));
        }
    }
}
