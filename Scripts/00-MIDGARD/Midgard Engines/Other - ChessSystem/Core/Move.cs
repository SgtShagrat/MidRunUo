using Server;

namespace Midgard.Engines.ChessSystem
{
    /// <summary>
    /// Describes a piece move
    /// </summary>
    public class Move
    {
        private bool m_EnPassant;

        /// <summary>
        /// Gets the initial position of the piece
        /// </summary>
        public Point2D From { get; private set; }

        /// <summary>
        /// Gets the target destination of the move
        /// </summary>
        public Point2D To { get; private set; }

        /// <summary>
        /// Gets the chess piece performing this move
        /// </summary>
        public BaseChessPiece Piece { get; private set; }

        /// <summary>
        /// Gets the piece captured by this move
        /// </summary>
        public BaseChessPiece CapturedPiece { get; private set; }

        /// <summary>
        /// Specifies if this move captures a piece
        /// </summary>
        public bool Capture
        {
            get { return CapturedPiece != null; }
        }

        /// <summary>
        /// The color of the player making this move
        /// </summary>
        public ChessColor Color
        {
            get { return Piece.Color; }
        }

        /// <summary>
        /// Gets the color of the opponent of the player who made the move
        /// </summary>
        public ChessColor EnemyColor
        {
            get { return Piece.EnemyColor; }
        }

        /// <summary>
        /// Specifies if the capture is made EnPassant
        /// </summary>
        public bool EnPassant
        {
            get { return m_EnPassant; }
            set { m_EnPassant = value; }
        }

        /// <summary>
        /// Creates a new Move object without capturing a piece
        /// </summary>
        /// <param name="piece">The chess piece performing the move</param>
        /// <param name="target">The target location of the move</param>
        public Move( BaseChessPiece piece, Point2D target )
        {
            Piece = piece;
            From = Piece.Position;
            To = target;
            CapturedPiece = Piece.GetCaptured( target, ref m_EnPassant );
        }
    }
}