using System;
using System.Collections.Generic;
using Server;

namespace Midgard.Engines.ChessSystem
{
    public class Rook : BaseChessPiece
    {
        private bool m_Castle;

        public static int GetGumpID( ChessColor color )
        {
            return color == ChessColor.Black ? 2340 : 2333;
        }

        public override int Power
        {
            get
            {
                return 5;
            }
        }

        public Rook( BChessboard board, ChessColor color, Point2D position )
            : base( board, color, position )
        {
        }

        public override void InitializePiece()
        {
            Piece = new ChessMobile( this );
            Piece.Name = string.Format( "Rook [{0}]", Color.ToString() );

            switch( m_BChessboard.ChessSet )
            {
                case ChessSet.Classic:
                    CreateClassic();
                    break;

                case ChessSet.Fantasy:
                    CreateFantasy();
                    break;

                case ChessSet.FantasyGiant:
                    CreateFantasyGiant();
                    break;

                case ChessSet.Animal:
                    CreateAnimal();
                    break;

                case ChessSet.Undead:
                    CreateUndead();
                    break;
            }
        }

        private void CreateUndead()
        {
            m_MoveSound = 880;
            m_CaptureSound = 357;
            m_DeathSound = 1156;

            Piece.BodyValue = 154; // Mummy
            Piece.Hue = Hue;
        }

        private void CreateAnimal()
        {
            m_MoveSound = 159;
            m_CaptureSound = 158;
            m_DeathSound = 162;

            Piece.BodyValue = 29; // Gorilla
            Piece.Hue = Hue;
        }

        private void CreateFantasyGiant()
        {
            m_MoveSound = 767;
            m_CaptureSound = 367;
            m_DeathSound = 371;

            Piece.BodyValue = 312; // Abysmal Horror
            Piece.Hue = Hue;
        }

        private void CreateFantasy()
        {
            m_MoveSound = 461;
            m_CaptureSound = 463;
            m_DeathSound = 465;

            Piece.Female = false;
            Piece.BodyValue = 55; // Troll
            Piece.Hue = Hue;
        }

        private void CreateClassic()
        {
            m_MoveSound = 287;
            m_CaptureSound = 268;
            m_DeathSound = 269;

            Piece.Female = false;
            Piece.BodyValue = 14;

            Piece.Hue = Hue;
        }

        public override bool CanMoveTo( Point2D newLocation, ref string err )
        {
            if( !base.CanMoveTo( newLocation, ref err ) )
                return false;

            // Verify if this is a castle
            BaseChessPiece king = m_BChessboard[ newLocation ];

            if( king is King && king.Color == Color )
            {
                // Trying to castle
                return m_BChessboard.AllowCastle( king, this, ref err );
            }

            int dx = newLocation.X - Position.X;
            int dy = newLocation.Y - Position.Y;

            // Rooks can only move in one direction
            if( dx != 0 && dy != 0 )
            {
                err = "Rooks can only move on straight lines";
                return false;
            }

            if( dx != 0 )
            {
                // Moving on the X axis
                int direction = dx > 0 ? 1 : -1;

                if( Math.Abs( dx ) > 1 )
                {
                    // Verify that the cells in between are empty
                    for( int i = 1; i < Math.Abs( dx ); i++ ) // Start 1 tile after the rook, and stop one tile before destination
                    {
                        int offset = direction * i;

                        if( m_BChessboard[ Position.X + offset, Position.Y ] != null )
                        {
                            err = "Rooks can't move over pieces";
                            return false; // There's a piece on the 
                        }
                    }
                }

                // Verify if there's a piece to each at the end
                BaseChessPiece piece = m_BChessboard[ newLocation ];

                if( piece == null || piece.Color != Color )
                    return true;
                else
                {
                    err = "You can't capture pieces of your same color";
                    return false;
                }
            }
            else
            {
                // Moving on the Y axis
                int direction = dy > 0 ? 1 : -1;

                if( Math.Abs( dy ) > 1 )
                {
                    // Verify that the cells in between are empty
                    for( int i = 1; i < Math.Abs( dy ); i++ )
                    {
                        int offset = direction * i;

                        if( m_BChessboard[ Position.X, Position.Y + offset ] != null )
                        {
                            err = "The rook can't move over other pieces";
                            return false; // Piece on the way
                        }
                    }
                }

                // Verify for piece at end
                BaseChessPiece piece = m_BChessboard[ newLocation ];

                if( piece == null || piece.Color != Color )
                    return true;
                else
                {
                    err = "You can't capture pieces of your same color";
                    return false;
                }
            }
        }

        public override List<Point2D> GetMoves( bool capture )
        {
            List<Point2D> moves = new List<Point2D>();

            int[] xDirection = new int[] { -1, 1, 0, 0 };
            int[] yDirection = new int[] { 0, 0, 1, -1 };

            for( int i = 0; i < 4; i++ )
            {
                int xDir = xDirection[ i ];
                int yDir = yDirection[ i ];

                int offset = 1;

                while( true )
                {
                    Point2D p = new Point2D( Position.X + offset * xDir, Position.Y + offset * yDir );

                    if( !m_BChessboard.IsValid( p ) )
                        break;

                    BaseChessPiece piece = m_BChessboard[ p ];

                    if( piece == null )
                    {
                        moves.Add( p );
                        offset++;
                        continue;
                    }

                    if( capture && piece.Color != Color )
                    {
                        moves.Add( p );
                        break;
                    }

                    break;
                }
            }

            return moves;
        }

        public override bool IsCastle( Point2D loc )
        {
            King king = m_BChessboard[ loc ] as King;

            string err = null;

            return king != null && king.Color == Color && m_BChessboard.AllowCastle( king, this, ref err );
        }

        public void Castle()
        {
            m_Castle = true;

            int dx = 0;

            if( Position.X == 0 )
                dx = 3;
            else if( Position.X == 7 )
                dx = -2;

            Move move = new Move( this, new Point2D( Position.X + dx, Position.Y ) );

            MoveTo( move );
        }

        public override void OnMoveOver()
        {
            if( !m_Castle )
                base.OnMoveOver();
            else
            {
                m_Castle = false;

                m_BChessboard.ApplyMove( m_Move );

                King king = m_BChessboard.GetKing( Color ) as King;

                int dx;

                if( Position.X == 3 )
                    dx = -2;
                else
                    dx = 2;

                if( king != null )
                    king.EndCastle( new Point2D( king.Position.X + dx, king.Position.Y ) );
            }
        }
    }
}