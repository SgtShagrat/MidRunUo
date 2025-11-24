using System;

using Midgard.Engines.Races;

using Server;
using Server.Items;

namespace Midgard.Items
{
    [RaceAllowance( typeof( MountainDwarf ) )]
    public class HoodedCloak : Cloak
    {
        public override string DefaultName { get { return "hooded cloak"; } }

        [Hue, CommandProperty( AccessLevel.GameMaster )]
        public override int Hue
        {
            get { return base.Hue; }
            set
            {
                if( InternalHood != null )
                    InternalHood.Hue = value;

                base.Hue = value;
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public Hood InternalHood { get; private set; }

        [Constructable]
        public HoodedCloak( int hue )
        {
            Weight = 5.0 + 1.0;
            Hue = hue;

            InternalHood = new Hood( this, 0x3356, hue );
        }

        [Constructable]
        public HoodedCloak() : this( 0 )
        {
        }

        public bool CheckHoodWearable( Mobile from )
        {
            return from.FindItemOnLayer( Layer.Helm ) == null;
        }

        public bool HasOurHood( Mobile from )
        {
            Item helm = from.FindItemOnLayer( Layer.Helm );

            return helm != null && helm == InternalHood;
        }

        public override void OnDoubleClick( Mobile m )
        {
            if( Parent != m )
            {
                m.SendMessage( "You must be wearing the robe to use it!" );
            }
            else
            {
                if( CheckHoodWearable( m ) )
                    EquipHood( m, true );
                else if( HasOurHood( m ) )
                    RemoveHood( m, true );
            }

            base.OnDoubleClick( m );
        }

        public void EquipHood( Mobile from, bool message )
        {
            if( InternalHood == null || InternalHood.Deleted )
                InternalHood = new Hood( this, 0x3356, Hue );

            if( message )
                from.SendMessage( "You pull the hood over your head." );

            //if( from is Midgard2PlayerMobile )
            //    ( (Midgard2PlayerMobile)from ).SetHairMods( 0, 0 );

            from.EquipItem( InternalHood );
        }

        public void RemoveHood( Mobile from, bool message )
        {
            if( HasOurHood( from ) )
            {
                from.RemoveItem( InternalHood );

                InternalHood.Internalize();

                if( message )
                    from.SendMessage( "You lowered the hood." );

                //if( from is Midgard2PlayerMobile )
                //    ( (Midgard2PlayerMobile)from ).SetHairMods( -1, -1 );
            }
        }

        public override void OnRemoved( Object parent )
        {
            Mobile mob = parent as Mobile;

            if( mob != null )
                RemoveHood( mob, false );

            base.OnRemoved( parent );
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            if( InternalHood != null && !InternalHood.Deleted )
                InternalHood.Delete();
        }

        #region serialization
        public HoodedCloak( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.Write( InternalHood );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            InternalHood = reader.ReadItem() as Hood;
        }
        #endregion
    }
}