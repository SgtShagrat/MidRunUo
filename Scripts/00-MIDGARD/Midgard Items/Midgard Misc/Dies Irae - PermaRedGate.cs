/***************************************************************************
 *                               PermaRedGate.cs
 *
 *   begin                : 22 luglio 2010
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;
using Server.Mobiles;

namespace Midgard.Items
{
    public class PermaRedGate : Item
    {
        public override string DefaultName
        {
            get { return "a resurrection gate"; }
        }

        [Constructable]
        public PermaRedGate()
            : base( 0xF6C )
        {
            Movable = false;
            Hue = 37;
            Light = LightType.Circle300;
        }

        public override bool OnMoveOver( Mobile m )
        {
            Midgard2PlayerMobile playerMobile = m as Midgard2PlayerMobile;
            if( playerMobile == null )
                return false;

            if( !playerMobile.PermaRed )
            {
                m.PlaySound( 0x214 );
                m.FixedEffect( 0x376A, 10, 16 );

                playerMobile.TempPermaRed = true;
                m.SendMessage( 37, "Until the lands shrunk into darkness you will be known throughout the land as a murderous brigand." );
            }
            else
            {
                m.SendMessage( 37, "Thou're already a known murderer." );
            }

            return false;
        }

        public override void OnSingleClick( Mobile from )
        {
            LabelTo( from, "a bloody red gate" );
        }

        #region mod by Dies Irae
        public PermaRedGate( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}
