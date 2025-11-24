using System;
using System.Collections.Generic;
using Server;
using Server.Items;

namespace Midgard.Engines.ChessSystem
{
    public class King : BaseChessPiece
    {
        private int m_CheckSound;
        private int m_CheckMateSound;

        public override int Power
        {
            get
            {
                return 100; // Useless
            }
        }

        public King( BChessboard board, ChessColor color, Point2D position )
            : base( board, color, position )
        {
        }

        public override void InitializePiece()
        {
            Piece = new ChessMobile( this );
            Piece.Name = string.Format( "King [{0}]", Color );

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
            m_MoveSound = 1163;
            m_CaptureSound = 1162;
            m_DeathSound = 361;
            m_CheckSound = 772;
            m_CheckMateSound = 716;

            Piece.BodyValue = 148; // Skeletal mage
            Piece.Hue = Hue;
        }

        private void CreateAnimal()
        {
            m_MoveSound = 95;
            m_CaptureSound = 1223;
            m_DeathSound = 98;
            m_CheckSound = 98;
            m_CheckMateSound = 99;

            Piece.BodyValue = 213; // Polar bear
            Piece.Hue = Hue;
        }

        private void CreateFantasyGiant()
        {
            m_MoveSound = 1152;
            m_CaptureSound = 1150;
            m_DeathSound = 0;
            m_CheckSound = 1152;
            m_CheckMateSound = 1149;

            Piece.BodyValue = 311; // Shadow knight
            Piece.Hue = Hue;
        }

        private void CreateFantasy()
        {
            m_MoveSound = 610;
            m_CaptureSound = 606;
            m_DeathSound = 0;
            m_CheckSound = 604;
            m_CheckMateSound = 613;

            Piece.BodyValue = 767; // Betrayer
            Piece.Hue = Hue;
        }

        private void CreateClassic()
        {
            m_MoveSound = 1055;
            m_CaptureSound = 1068;
            m_DeathSound = 0;
            m_CheckSound = 1086;
            m_CheckMateSound = 1088;

            Piece.Female = false;
            Piece.BodyValue = 0x190;

            if( m_BChessboard.OverrideMinorHue )
                Piece.Hue = Hue;
            else
                Piece.Hue = m_BChessboard.SkinHue;
            Piece.AddItem( new ShortHair( m_BChessboard.OverrideMinorHue ? Hue : m_BChessboard.HairHue ) );

            Item item = null;

            item = new Boots( MinorHue );
            Piece.AddItem( item );

            item = new LongPants( Hue );
            Piece.AddItem( item );

            item = new FancyShirt( Hue );
            Piece.AddItem( item );

            item = new Doublet( MinorHue );
            Piece.AddItem( item );

            item = new Cloak( MinorHue );
            Piece.AddItem( item );

            item = new Scepter();
            item.Hue = MinorHue;
            Piece.AddItem( item );
        }

        public override bool CanMoveTo( Point2D newLocation, ref string err )
        {
            if( !base.CanMoveTo( newLocation, ref err ) )
                return false;

            // Verify if this is a castle
            BaseChessPiece rook = m_BChessboard[ newLocation ];

            if( rook is Rook && rook.Color == Color )
            {
                // Trying to castle
                return m_BChessboard.AllowCastle( this, rook, ref err );
            }

            int dx = newLocation.X - Position.X;
            int dy = newLocation.Y - Position.Y;

            if( Math.Abs( dx ) > 1 || Math.Abs( dy ) > 1 )
            {
                err = "The can king can move only 1 tile at a time";
                return false; // King can move only 1 tile away from its position
            }

            // Verify target piece
            BaseChessPiece piece = m_BChessboard[ newLocation ];

            if( piece == null || piece.Color != Color )
            {
                return true;
            }
            else
            {
                err = "You can't capture pieces of your same color";
                return false;
            }
        }

        public override List<Point2D> GetMoves( bool capture )
        {
            List<Point2D> moves = new List<Point2D>();

            for( int dx = -1; dx <= 1; dx++ )
            {
                for( int dy = -1; dy <= 1; dy++ )
                {
                    if( dx == 0 && dy == 0 )
                        continue; // Can't move to same spot

                    Point2D p = new Point2D( Position.X + dx, Position.Y + dy );

                    if( !m_BChessboard.IsValid( p ) )
                        continue;

                    BaseChessPiece piece = m_BChessboard[ p ];

                    if( piece == null )
                        moves.Add( p );
                    else if( capture && piece.Color != Color )
                        moves.Add( p );
                }
            }

            return moves;
        }

        public override bool IsCastle( Point2D loc )
        {
            Rook rook = m_BChessboard[ loc ] as Rook;

            string err = null;

            return rook != null && rook.Color == Color && m_BChessboard.AllowCastle( this, rook, ref err );
        }

        public void EndCastle( Point2D location )
        {
            m_HasMoved = true;

            m_Move = new Move( this, location );

            Point2D worldLocation = m_BChessboard.BoardToWorld( location );

            Piece.GoTo( worldLocation );
        }

        public void PlayCheck()
        {
            m_BChessboard.PlaySound( Piece, m_CheckSound );
            Piece.Say( "*CHECK*" );
        }

        public void PlayCheckMate()
        {
            m_BChessboard.PlaySound( Piece, m_CheckMateSound );
            Piece.Say( "CHECKMATE" );
        }

        public void PlayStaleMate()
        {
            m_BChessboard.PlaySound( Piece, m_CheckSound );
            Piece.Say( "STALEMATE" );
        }
    }
}