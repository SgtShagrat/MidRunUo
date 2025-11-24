/***************************************************************************
 *                               WeaponBarrels.cs
 *
 *   begin                : 21 October, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;
using Server.Items;

namespace Midgard.Items
{
    public abstract class BaseWeaponBarrel : LockableContainer
    {
        public override int LabelNumber { get { return 1064920; } } // barrel with weapons
        public override int DefaultGumpID { get { return 0x3E; } }

        public BaseWeaponBarrel( int itemID )
            : base( itemID )
        {
        }

        public BaseWeaponBarrel( Serial serial )
            : base( serial )
        {
        }

        public override bool OnDragDrop( Mobile from, Item dropped )
        {
            if( dropped == null || dropped.Deleted || from == null )
                return false;

            if( ( dropped is BaseWeapon ) )
                return base.OnDragDrop( from, dropped );
            else
            {
                from.SendMessage( "You can only place weapons in this barrel!" );
                return false;
            }
        }

        public override bool OnDragDropInto( Mobile from, Item item, Point3D p )
        {
            if( item == null || item.Deleted || from == null )
                return false;

            if( ( item is BaseWeapon ) )
                return base.OnDragDropInto( from, item, p );
            else
            {
                from.SendMessage( "You can only place weapons in this barrel!" );
                return false;
            }
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    [Furniture]
    [Flipable( 0x2C7E, 0x2C7F )]
    public class WeaponBarrel1 : BaseWeaponBarrel
    {
        [Constructable]
        public WeaponBarrel1()
            : base( 0x2C7E )
        {
        }

        public WeaponBarrel1( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    [Furniture]
    public class WeaponBarrel2 : BaseWeaponBarrel
    {
        public override int LabelNumber
        {
            get { return 1064921; } // barrel with weapons
        }

        [Constructable]
        public WeaponBarrel2()
            : base( 0x2C80 )
        {
        }

        public WeaponBarrel2( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    [Furniture]
    public class WeaponBarrel3 : BaseWeaponBarrel
    {
        public override int LabelNumber
        {
            get { return 1064922; } // barrel with weapons
        }

        [Constructable]
        public WeaponBarrel3()
            : base( 0x2C81 )
        {
        }

        public WeaponBarrel3( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }

    [Furniture]
    public class WeaponBarrel4 : BaseWeaponBarrel
    {
        public override int LabelNumber
        {
            get { return 1064923; } // barrel with weapons
        }

        [Constructable]
        public WeaponBarrel4()
            : base( 0x2C82 )
        {
        }

        public WeaponBarrel4( Serial serial )
            : base( serial )
        {
        }

        #region serial-deserial
        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}