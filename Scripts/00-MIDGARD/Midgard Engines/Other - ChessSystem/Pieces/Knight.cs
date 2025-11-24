using System;
using System.Collections.Generic;
using Server;
using Server.Items;

namespace Midgard.Engines.ChessSystem
{
    public class Knight : BaseChessPiece
    {
        public static int GetGumpID( ChessColor color )
        {
            return color == ChessColor.Black ? 2342 : 2335;
        }

        public override int Power
        {
            get
            {
                return 3;
            }
        }


        public Knight( BChessboard board, ChessColor color, Point2D position )
            : base( board, color, position )
        {
        }

        public override void InitializePiece()
        {
            Piece = new ChessMobile( this );
            Piece.Name = string.Format( "Knight [{0}]", Color );

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
            m_MoveSound = 588;
            m_CaptureSound = 1164;
            m_DeathSound = 416;

            Piece.Female = false;
            Piece.BodyValue = 0x190;
            Piece.AddItem( new HoodedShroudOfShadows( Hue ) );

            Server.Mobiles.SkeletalMount mount = new Server.Mobiles.SkeletalMount();
            mount.Hue = MinorHue;
            mount.Rider = Piece;

            Piece.Direction = Facing;
        }

        private void CreateAnimal()
        {
            m_MoveSound = 183;
            m_CaptureSound = 1011;
            m_DeathSound = 185;

            Piece.BodyValue = 292; // Pack llama
            Piece.Hue = Hue;
        }

        private void CreateFantasyGiant()
        {
            m_MoveSound = 875;
            m_CaptureSound = 378;
            m_DeathSound = 879;

            Piece.BodyValue = 315; // Flesh renderer
            Piece.Hue = Hue;
        }

        private void CreateFantasy()
        {
            m_MoveSound = 762;
            m_CaptureSound = 758;
            m_DeathSound = 759;

            Piece.BodyValue = 101; // Centaur
            Piece.Hue = Hue;
        }

        private void CreateClassic()
        {
            m_MoveSound = 588;
            m_CaptureSound = 168;
            m_DeathSound = 170;

            Piece.Female = false;
            Piece.BodyValue = 0x190;

            if( m_BChessboard.OverrideMinorHue )
                Piece.Hue = Hue;
            else
                Piece.Hue = m_BChessboard.SkinHue;
            Piece.AddItem( new PonyTail( m_BChessboard.OverrideMinorHue ? Hue : m_BChessboard.HairHue ) );

            Item item = null;

            item = new PlateLegs();
            item.Hue = Hue;
            Piece.AddItem( item );

            item = new PlateChest();
            item.Hue = Hue;
            Piece.AddItem( item );

            item = new PlateArms();
            item.Hue = Hue;
            Piece.AddItem( item );

            item = new PlateGorget();
            item.Hue = Hue;
            Piece.AddItem( item );

            item = new PlateGloves();
            item.Hue = Hue;
            Piece.AddItem( item );

            item = new Doublet( MinorHue );
            Piece.AddItem( item );

            item = new Lance();
            item.Hue = MinorHue;
            Piece.AddItem( item );

            Server.Mobiles.Horse horse = new Server.Mobiles.Horse();
            horse.BodyValue = 200;
            horse.Hue = MinorHue;

            horse.Rider = Piece;

            Piece.Direction = Facing;
        }

        public override bool CanMoveTo( Point2D newLocation, ref string err )
        {
            if( !base.CanMoveTo( newLocation, ref err ) )
                return false;

            // Care only about absolutes for knights
            int dx = Math.Abs( newLocation.X - Position.X );
            int dy = Math.Abs( newLocation.Y - Position.Y );

            if( !( ( dx == 1 && dy == 2 ) || ( dx == 2 && dy == 1 ) ) )
            {
                err = "Knights can only make L shaped moves (2-3 tiles length)";
                return false; // Wrong move
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

            for( int dx = -2; dx <= 2; dx++ )
            {
                for( int dy = -2; dy <= 2; dy++ )
                {
                    if( !( ( Math.Abs( dx ) == 1 && Math.Abs( dy ) == 2 ) || ( Math.Abs( dx ) == 2 && Math.Abs( dy ) == 1 ) ) )
                        continue;

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

    }
}