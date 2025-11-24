/***************************************************************************
 *                               NecromancerAltarActiveComponent.cs
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
    public class NecromancerAltarActiveComponent : AltarActiveComponent
    {
        [Constructable]
        public NecromancerAltarActiveComponent()
            : base( 8787 )
        {
        }

        public override string DefaultName
        {
            get { return "Necromancer Tome"; } // TODO FIX
        }

        public override ClassSystem AltarSystem
        {
            get { return ClassSystem.Necromancer; }
        }

        #region serialization
        public NecromancerAltarActiveComponent( Serial serial )
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