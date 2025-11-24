using System;
using Server;
using Server.Items;

namespace Midgard.Engines.PlagueBeastLordPuzzle
{
    public class Gland : Item
    {
        private bool m_IsOpened;

        [CommandProperty( AccessLevel.Developer )]
        public BloodWound BleedingWound { get; private set; }

        [CommandProperty( AccessLevel.Developer )]
        public Container Pack { get; private set; }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Developer )]
        public bool IsOpened
        {
            get { return m_IsOpened; }
            set
            {
                if( value )
                    m_IsOpened = true;
            }
        }

        [CommandProperty( AccessLevel.Developer )]
        public BrainTypes BrainType { get; private set; }

        public BaseOrgan Organ { get; set; }

        public override int LabelNumber { get { return 1066106; } } // gland
        public override bool DisplayWeight { get { return false; } }

        [Constructable]
        public Gland( BrainTypes type, BaseOrgan organ )
            : base( 0x1CF0 )
        {
            m_IsOpened = false;
            BrainType = type;
            Organ = organ;

            Hue = (int)BaseOrgan.GetBrainHueFromType( type );

            Stackable = false;
            Movable = true;
            Visible = false;

            Weight = 40;
        }

        public Gland( Serial serial )
            : base( serial )
        {
        }

        #region members
        public static void RevealGland( Gland gland )
        {
            if( gland != null )
            {
                if( gland.Visible )
                    gland.Visible = false;
                gland.Visible = true;
            }
        }

        public void CreateTissue( int offSetX, int offSetY )
        {
            Pack = Parent as Container;
            if( Pack == null )
                return;

            BleedingWound = new BloodWound( Organ, this );
            Pack.AddItem( BleedingWound );
            BleedingWound.Location = new Point3D( X + offSetX - 5, Y + offSetY, 0 );
            BleedingWound.CreateBandage();
        }

        public void MakeBleed( Mobile from )
        {
            if( BleedingWound != null && Organ != null )
                BleedingWound.DoBleed( from );
        }

        public override bool DropToMobile( Mobile from, Mobile target, Point3D p )
        {
            return false;
        }

        public override void OnItemLifted( Mobile from, Item item )
        {
            if( !BleedingWound.IsBleeding )
            {
                if( BleedingWound != null )
                {
                    Point3D p1 = BleedingWound.Location;
                    Console.WriteLine( "{0} {1} {2}", p1.ToString(), Location.ToString(), GetDistanceToSqrt( p1, Location ).ToString() );
                    if( GetDistanceToSqrt( p1, Location ) < 20 )
                        MakeBleed( from );
                }
            }

            base.OnItemLifted( from, item );
        }

        public static double GetDistanceToSqrt( Point3D p1, Point3D p2 )
        {
            int xDelta = p1.X - p2.X;
            int yDelta = p1.Y - p2.Y;

            return Math.Sqrt( ( xDelta * xDelta ) + ( yDelta * yDelta ) );
        }

        public override bool DropToItem( Mobile from, Item target, Point3D p )
        {
            if( BrainType != BrainTypes.None && target == Pack )
            {
                PuzzlePlagueBeastLord pbl = Pack.Parent as PuzzlePlagueBeastLord;

                Point3D p2 = PuzzlePlagueBeastLord.GetReceptorLocFromBrainType( BrainType );

                double d = GetDistanceToSqrt( p, p2 );

                if( pbl != null )
                {
                    if( pbl.IsInDebugMode )
                        pbl.Say( "You put the gland {0} tiles distant from the righ receptor", d.ToString( "F1" ) );

                    if( GetDistanceToSqrt( p, p2 ) < 5 )
                    {
                        Organ.SendLocalizedMessageTo( from, 1066107 ); // * You place the organ in the fleshy receptacle near the core *
                        pbl.CoreCheck( (int)BrainType - 1, true );
                        Movable = false;
                    }
                    else
                        pbl.CoreCheck( (int)BrainType - 1, false );
                }
            }

            return ( target == Pack && p.X != -1 && p.Y != -1 && base.DropToItem( from, target, p ) );
        }

        public override bool DropToWorld( Mobile from, Point3D p )
        {
            return false;
        }

        public override int GetLiftSound( Mobile from )
        {
            return 0x2AA;
        }
        #endregion

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version

            writer.Write( m_IsOpened );
            writer.Write( (int)BrainType );
            writer.Write( BleedingWound );
            writer.Write( Pack );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            m_IsOpened = reader.ReadBool();
            BrainType = (BrainTypes)reader.ReadInt();
            BleedingWound = (BloodWound)reader.ReadItem();
            Pack = (Container)reader.ReadItem();

            if( BleedingWound == null || Pack == null )
                Delete();
        }
        #endregion
    }
}