/***************************************************************************
 *                               PaladinAltarActiveComponent.cs
 *
 *   begin                : 10 January, 2010
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using Server;

namespace Midgard.Engines.Classes
{
    public class PaladinAltarActiveComponent : AltarActiveComponent
    {
        [Constructable]
        public PaladinAltarActiveComponent()
            : base( 7187 ) 
        {
        }

        public override string DefaultName
        {
            get { return "Book of Truth"; }
        }

        public override ClassSystem AltarSystem
        {
            get { return ClassSystem.Paladin; }
        }

        #region serialization
        public PaladinAltarActiveComponent( Serial serial )
            : base( serial )
        {
        }

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