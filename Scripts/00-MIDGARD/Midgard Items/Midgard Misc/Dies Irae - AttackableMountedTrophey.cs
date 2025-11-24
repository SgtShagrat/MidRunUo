using Server.Mobiles;

namespace Server.Items
{
    public abstract class BaseSpawningMountedTrophey : Item
    {
        public abstract BaseCreature Monster { get; }

        private static bool DeleteMount = true;

        public override bool HandlesOnMovement { get { return true; } }

        public BaseSpawningMountedTrophey( int itemID )
            : base( itemID )
        {
            Weight = 20.0;
            Movable = false;
        }

        private int[] m_Messages = new int[] { 1045135, 1046000, 1046005, 1046010 };

        public override void OnMovement( Mobile from, Point3D oldLocation )
        {
            if( from.InRange( this, 3 ) && from is PlayerMobile )
            {
                SendLocalizedMessageTo( from, Utility.RandomList( m_Messages ) );
            }
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( from.InRange( GetWorldLocation(), 3 ) )
            {
                SendLocalizedMessageTo( from, 1046017 ); //Thank you kind stranger! You have freed me.  I knew you could do it.
                Effects.SendLocationEffect( Location, Map, 0x3728, 20, 10 );
                Effects.PlaySound( Location, Map, 0x11C );
                Monster.MoveToWorld( Location, Map );

                if( DeleteMount )
                    Delete();
            }
            else
                SendLocalizedMessageTo( from, 1007061 ); //Barely a flesh wound. Canst thou not do better?
        }

        #region serialization
        public BaseSpawningMountedTrophey( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    [Furniture]
    [Flipable( 0x1E61, 0x1E68 )]
    public class SpawningMountedTropheyGreatHeart : Item
    {
        public BaseCreature Monster { get { return new GreatHart(); } }

        public override string DefaultName { get { return "a mounted stag trophey (do not touch)"; } }

        [Constructable]
        public SpawningMountedTropheyGreatHeart()
            : base( 0x1E61 )
        {
        }

        #region serialization
        public SpawningMountedTropheyGreatHeart( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    [Furniture]
    [Flipable( 0x1E63, 0X1E6A )]
    public class SpawningMountedTropheyOgre : Item
    {
        public BaseCreature Monster { get { return new Ogre(); } }

        public override string DefaultName { get { return "a mounted ogre trophey (do not touch)"; } }

        [Constructable]
        public SpawningMountedTropheyOgre()
            : base( 0x1E63 )
        {
        }

        #region serialization
        public SpawningMountedTropheyOgre( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    [Furniture]
    [Flipable( 0x1E64, 0X1E6B )]
    public class SpawningMountedTropheyOrc : Item
    {
        public BaseCreature Monster { get { return new Orc(); } }

        public override string DefaultName { get { return "a mounted orc trophey (do not touch)"; } }

        [Constructable]
        public SpawningMountedTropheyOrc()
            : base( 0x1E64 )
        {
        }

        #region serialization
        public SpawningMountedTropheyOrc( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}