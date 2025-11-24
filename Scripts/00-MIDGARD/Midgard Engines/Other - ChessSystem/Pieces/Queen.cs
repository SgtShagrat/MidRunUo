using System;
using System.Collections.Generic;
using Server;
using Server.Items;

namespace Midgard.Engines.ChessSystem
{
    /// <summary>
    /// Summary description for Queen.
    /// </summary>
    public class Queen : BaseChessPiece
    {
        public static int GetGumpID( ChessColor color )
        {
            return color == ChessColor.Black ? 2344 : 2377;
        }

        public override int Power
        {
            get
            {
                return 9;
            }
        }


        public Queen( BChessboard board, ChessColor color, Point2D position )
            : base( board, color, position )
        {
        }

        public override void InitializePiece()
        {
            Piece = new ChessMobile( this );
            Piece.Name = string.Format( "Queen [{0}]", Color );

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
            m_MoveSound = 896;
            m_CaptureSound = 382;
            m_DeathSound = 1202;

            Piece.BodyValue = 310; // Wailing banshee
            Piece.Hue = Hue;
        }

        private void CreateAnimal()
        {
            m_MoveSound = 123;
            m_CaptureSound = 120;
            m_DeathSound = 124;

            Piece.BodyValue = 216; // Cow
            Piece.Hue = Hue;
        }

        private void CreateFantasyGiant()
        {
            m_MoveSound = 883;
            m_CaptureSound = 880;
            m_DeathSound = 888;

            Piece.BodyValue = 174; // Semidar
            Piece.Hue = Hue;
        }

        private void CreateFantasy()
        {
            m_MoveSound = 1200;
            m_CaptureSound = 1201;
            m_DeathSound = 1202;

            Piece.BodyValue = 149; // Succubus
            Piece.Hue = Hue;
        }

        private void CreateClassic()
        {
            m_MoveSound = 823;
            m_CaptureSound = 824;
            m_DeathSound = 814;

            Piece.Female = true;
            Piece.BodyValue = 0x191;

            if( m_BChessboard.OverrideMinorHue )
                Piece.Hue = Hue;
            else
                Piece.Hue = m_BChessboard.SkinHue;
            Piece.AddItem( new LongHair( m_BChessboard.OverrideMinorHue ? Hue : m_BChessboard.HairHue ) );

            Item item = null;

            item = new FancyDress( Hue );
            Piece.AddItem( item );

            item = new Sandals( MinorHue );
            Piece.AddItem( item );

            item = new Scepter();
            item.Hue = MinorHue;
            Piece.AddItem( item );
        }

        public override bool CanMoveTo( Point2D newLocation, ref string err )
        {
            if( !base.CanMoveTo( newLocation, ref err ) )
                return false;

            int dx = newLocation.X - Position.X;
            int dy = newLocation.Y - Position.Y;

            if( dx == 0 || dy == 0 )
            {
                // Straight movement
                if( Math.Abs( dx ) > 1 ) // If it's just 1 step no need to check for intermediate pieces
                {
                    int direction = dx > 0 ? 1 : -1;

                    // Moving along X axis
                    for( int i = 1; i < Math.Abs( dx ); i++ )
                    {
                        int offset = direction * i;

                        if( m_BChessboard[ Position.X + offset, Position.Y ] != null )
                        {
                            err = "The queen can't move over other pieces";
                            return false;
                        }
                    }
                }
                else if( Math.Abs( dy ) > 1 )
                {
                    // Moving along Y axis
                    int direction = dy > 0 ? 1 : -1;

                    for( int i = 1; i < Math.Abs( dy ); i++ )
                    {
                        int offset = direction * i;

                        if( m_BChessboard[ Position.X, Position.Y + offset ] != null )
                        {
                            err = "The queen can't move over other pieces";
                            return false;
                        }
                    }
                }
            }
            else
            {
                // Diagonal movement
                if( Math.Abs( dx ) != Math.Abs( dy ) )
                {
                    err = "The queen moves only on straight lines or diagonals";
                    return false; // Uneven
                }

                if( Math.Abs( dx ) > 1 )
                {
                    int xDirection = dx > 0 ? 1 : -1;
                    int yDirection = dy > 0 ? 1 : -1;

                    for( int i = 1; i < Math.Abs( dx ); i++ )
                    {
                        int xOffset = xDirection * i;
                        int yOffset = yDirection * i;

                        if( m_BChessboard[ Position.X + xOffset, Position.Y + yOffset ] != null )
                        {
                            err = "The queen can't move over other pieces";
                            return false;
                        }
                    }
                }
            }

            // Verify target piece
            BaseChessPiece piece = m_BChessboard[ newLocation ];

            if( piece == null || piece.Color != Color )
                return true;
            else
            {
                err = "You can't capture pieces of your same color";
                return false;
            }
        }

        public override List<Point2D> GetMoves( bool capture )
        {
            List<Point2D> moves = new List<Point2D>();

            int[] xDirection = new int[] { -1, 1, -1, 1, 1, -1, 0, 0 };
            int[] yDirection = new int[] { -1, 1, 1, -1, 0, 0, 1, -1 };

            for( int i = 0; i < 8; i++ )
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
    }
}