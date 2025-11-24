using System.Collections.Generic;
using Server;

namespace Midgard.Engines.ChessSystem
{
    /// <summary>
    /// Defines the two colors used in the chess game
    /// </summary>
    public enum ChessColor
    {
        Black,
        White
    }

    /// <summary>
    /// This abstract class defines the basic features of a chess piece
    /// </summary>
    public abstract class BaseChessPiece
    {
        #region Variables

        /// <summary>
        /// The <see cref="BChessboard"/> object parent of this chess piece
        /// </summary>
        protected BChessboard m_BChessboard;

        /// <summary>
        /// Specifies if this piece has been killed
        /// </summary>
        protected bool m_Dead;

        /// <summary>
        /// Specifies if the piece has already been moved or not
        /// </summary>
        protected bool m_HasMoved;

        /// <summary>
        /// The move this piece is performing
        /// </summary>
        protected Move m_Move;

        /// <summary>
        /// The sound made when the piece moves
        /// </summary>
        protected int m_MoveSound;

        /// <summary>
        /// The sound made when the piece captures
        /// </summary>
        protected int m_CaptureSound;

        /// <summary>
        /// The sound made when the piece is captured
        /// </summary>
        protected int m_DeathSound;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the NPC corresponding to this chess piece
        /// </summary>
        public ChessMobile Piece { get; protected set; }

        /// <summary>
        /// Gets the color of this piece
        /// </summary>
        public ChessColor Color { get; protected set; }

        /// <summary>
        /// Gets the color of the enemy
        /// </summary>
        public ChessColor EnemyColor
        {
            get
            {
                if( Color == ChessColor.Black )
                    return ChessColor.White;
                else
                    return ChessColor.Black;
            }
        }

        /// <summary>
        /// Gets or sets the position of this chess piece.
        /// This does NOT move the chess piece on the chess board
        /// </summary>
        public Point2D Position { get; set; }

        /// <summary>
        /// Gets the facing for the NPC when it's standing
        /// </summary>
        public virtual Direction Facing
        {
            get
            {
                if( m_BChessboard.Orientation == BoardOrientation.NorthSouth )
                {
                    if( Color == ChessColor.Black )
                        return Direction.South;
                    else
                        return Direction.North;
                }
                else
                {
                    if( Color == ChessColor.Black )
                        return Direction.East;
                    else
                        return Direction.West;
                }
            }
        }

        /// <summary>
        /// Gets the hue used to color items for this piece
        /// </summary>
        public virtual int Hue
        {
            get
            {
                if( Color == ChessColor.Black )
                    return m_BChessboard.BlackHue;
                else
                    return m_BChessboard.WhiteHue;
            }
        }

        /// <summary>
        /// Gets the opposite hue for this piece
        /// </summary>
        public virtual int SecondaryHue
        {
            get
            {
                if( Color == ChessColor.Black )
                    return m_BChessboard.WhiteHue;
                else
                    return m_BChessboard.BlackHue;
            }
        }

        public virtual int MinorHue
        {
            get
            {
                if( Color == ChessColor.Black )
                    return m_BChessboard.BlackMinorHue;
                else
                    return m_BChessboard.WhiteMinorHue;
            }
        }

        /// <summary>
        /// Gets the power value of this piece
        /// </summary>
        public abstract int Power { get; }

        /// <summary>
        /// States whether this piece has already moved
        /// </summary>
        public virtual bool HasMoved
        {
            get { return m_HasMoved; }
        }

        /// <summary>
        /// Specifies if this piece can be captured by a pawn en passant
        /// </summary>
        public virtual bool AllowEnPassantCapture
        {
            get { return false; }
            set { }
        }

        #endregion

        /// <summary>
        /// Creates a new chess piece object
        /// </summary>
        /// <param name="board">The <see cref="BChessboard"/> object hosting this piece</param>
        /// <param name="color">The color of this piece</param>
        /// <param name="position">The initial position on the board</param>
        public BaseChessPiece( BChessboard board, ChessColor color, Point2D position )
        {
            m_BChessboard = board;
            Color = color;
            Position = position;

            m_HasMoved = false;

            CreatePiece();
        }

        #region NPC Creation

        /// <summary>
        /// Creates the NPC that will represent this piece and places it in the correct world location
        /// </summary>
        protected virtual void CreatePiece()
        {
            InitializePiece();
            Point3D loc = new Point3D( m_BChessboard.BoardToWorld( Position ), m_BChessboard.Z );

            if( m_BChessboard.OverrideMinorHue )
                Piece.SolidHueOverride = Hue;

            Piece.MoveToWorld( loc, m_BChessboard.Map );
            Piece.FixedParticles( 14089, 1, 15, 5012, Hue, 2, EffectLayer.Waist );
        }

        /// <summary>
        /// Creates and initializes the chess piece NPC
        /// </summary>
        /// <returns></returns>
        public abstract void InitializePiece();

        /// <summary>
        /// Rebuilds the NPC applying any changes made to the appearance
        /// </summary>
        public virtual void Rebuild()
        {
            Die( false );
            CreatePiece();
            m_Dead = false;
        }

        #endregion

        #region Piece Movement

        /// <summary>
        /// Verifies if this piece can move to a specified location.
        /// </summary>
        /// <param name="newLocation">The new location</param>
        /// <param name="err">Will hold the eventual error message</param>
        /// <returns>True if the move is allowed, false otherwise.</returns>
        public virtual bool CanMoveTo( Point2D newLocation, ref string err )
        {
            if( newLocation == Position )
            {
                err = "Can't move to the same spot";
                return false; // Same spot isn't a valid move
            }

            // Base version, check only for out of bounds
            if( newLocation.X >= 0 && newLocation.Y >= 0 && newLocation.X < 8 && newLocation.Y < 8 )
            {
                return true;
            }
            else
            {
                err = "Can't move out of chessboard";
                return false;
            }
        }

        /// <summary>
        /// Moves the chess piece to the specified position. This function assumes that a previous call
        /// to <see cref="CanMoveTo"/>() has been made and the move has been authorized.
        /// </summary>
        /// <param name="move">The move performed</param>
        public virtual void MoveTo( Move move )
        {
            m_HasMoved = true;

            m_Move = move;

            Point2D worldLocation = m_BChessboard.BoardToWorld( move.To );

            if( move.Capture )
            {
                m_BChessboard.PlaySound( Piece, m_CaptureSound );

                // It's a capture, do an effect
                Piece.MovingParticles( move.CapturedPiece.Piece, m_BChessboard.AttackEffect, 5, 0, false, true, Hue, 2, 0, 1, 4006,
                                        EffectLayer.Waist, 0 );
            }
            else
            {
                m_BChessboard.PlaySound( Piece, m_MoveSound );
            }

            Piece.GoTo( worldLocation );
        }

        /// <summary>
        /// This function is called by the NPC when its move is over
        /// </summary>
        public virtual void OnMoveOver()
        {
            m_BChessboard.OnMoveOver( m_Move );

            if( m_Move.Capture )
            {
                Piece.FixedParticles( m_BChessboard.CaptureEffect, 1, 15, 5012, SecondaryHue, 2, EffectLayer.Waist );
                m_Move.CapturedPiece.Die( true );
            }

            m_Move = null;

            Piece.Direction = Facing;
        }

        /// <summary>
        /// Gets the list of possible moves this piece can perform
        /// </summary>
        /// <param name="capture">Specifies whether the moves should include squares where a piece would be captured</param>
        /// <returns>An <c>List<Point2D></c> objects of Point2D values</returns>
        public abstract List<Point2D> GetMoves( bool capture );

        /// <summary>
        /// Gets the piece that this piece would capture when moving to a specific location.
        /// This function assumes that the square can be reached.
        /// </summary>
        /// <param name="at">The target location with the potential capture</param>
        /// <param name="enpassant">Will hold a value stating whether this move is made en passant</param>
        /// <returns>A <see cref="BaseChessPiece"/> if a capture is possible, null otherwise</returns>
        public virtual BaseChessPiece GetCaptured( Point2D at, ref bool enpassant )
        {
            enpassant = false;

            BaseChessPiece piece = m_BChessboard[ at ];

            if( piece != null && piece.Color != Color )
                return piece;
            else
                return null;
        }

        /// <summary>
        /// Verifies if a given move would be a castle
        /// </summary>
        /// <param name="loc">The target location</param>
        /// <returns>True if the move is a castle</returns>
        public virtual bool IsCastle( Point2D loc )
        {
            return false;
        }

        #endregion

        #region Deletion and killing

        /// <summary>
        /// This function is invoked whenever a piece NPC is deleted
        /// </summary>
        public virtual void OnPieceDeleted()
        {
            if( !m_Dead )
            {
                m_BChessboard.OnStaffDelete();
            }
        }

        /// <summary>
        /// This function is invoked when the piece is captured and the NPC should be removed from the board
        /// </summary>
        /// <param name="sound">Specifies if to play the death sound</param>
        public virtual void Die( bool sound )
        {
            if( sound ) // Use sound for bolt too - sound is used in normal gameplay
            {
                if( m_BChessboard.BoltOnDeath )
                    Piece.BoltEffect( SecondaryHue );

                m_BChessboard.PlaySound( Piece, m_DeathSound );
            }

            m_Dead = true;
            Piece.Delete();
        }

        /// <summary>
        /// Forces the deletion of this piece
        /// </summary>
        public virtual void ForceDelete()
        {
            if( Piece != null && !Piece.Deleted )
            {
                m_Dead = true;
                Piece.Delete();
            }
        }

        #endregion
    }
}