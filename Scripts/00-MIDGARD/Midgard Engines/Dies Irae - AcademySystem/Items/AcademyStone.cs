/***************************************************************************
 *                               AcademyStone.cs
 *
 *   begin                : 05 novembre 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;
using Server.Gumps;
using Server.Network;

namespace Midgard.Engines.Academies
{
    public class MidgardAcademyStone : Item
    {
        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator )]
        public AcademySystem System { get; set; }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator )]
        public static bool AcademyAccessEnabled
        {
            get { return AcademySystem.AcademyAccessEnabled; }
            set { AcademySystem.AcademyAccessEnabled = value; }
        }

        public override bool DisplayWeight
        {
            get { return false; }
        }

        [Constructable]
        public MidgardAcademyStone()
            : this( AcademySystem.SerpentsHoldAcademy )
        {
        }

        [Constructable]
        public MidgardAcademyStone( AcademySystem system )
            : base( 0xEDE )
        {
            Movable = false;

            System = system;
            if( System != null )
                Name = string.Format( "The {0}", System.Definition.AcademyName );
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( System == null )
                return;

            if( from.AccessLevel > AccessLevel.GameMaster )
            {
                from.SendGump( new PropertiesGump( from, this ) );
                from.SendGump( new AcademySystemInfoGump( System, from ) );
                return;
            }

            if( from.InRange( GetWorldLocation(), 2 ) )
            {
                if( !AcademySystem.AcademyAccessEnabled && from.AccessLevel < AccessLevel.Administrator )
                {
                    from.SendMessage( "Academy access has been temporary disabled. Try later..." );
                }
                else
                {
                    AcademySystem fromSystem = AcademySystem.Find( from );

                    if( fromSystem == null )
                    {
                        if( System.IsCandidate( from ) )
                            from.SendMessage( "Thou art already a candidate for this Academy!" );
                        else if( System.IsEligible( from ) )
                            from.SendGump( new ConfirmJoinGump( from, System ) );
                        else
                            from.SendMessage( "Thou are not eligible to become a member of this Academy." );
                    }
                    else
                    {
                        if( fromSystem == System || from.AccessLevel > AccessLevel.Counselor )
                            from.SendGump( new AcademySystemInfoGump( System, from ) );
                        else
                            from.SendMessage( "You are not allowed to access this Academy Stone." );
                    }
                }
            }
            else
                from.LocalOverheadMessage( MessageType.Regular, 0x3B2, 1019045 ); // I can't reach that.
        }

        #region serial-deserial
        public MidgardAcademyStone( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            AcademySystem.WriteReference( writer, System );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            System = AcademySystem.ReadReference( reader );
        }
        #endregion
    }
}