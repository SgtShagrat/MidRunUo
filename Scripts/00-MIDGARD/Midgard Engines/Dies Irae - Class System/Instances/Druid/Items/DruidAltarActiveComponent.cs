/***************************************************************************
 *                               DruidAltarActiveComponent.cs
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
    public class DruidAltarActiveComponent : AltarActiveComponent
    {
        [Constructable]
        public DruidAltarActiveComponent()
            : base( 11595 )
        {
        }

        public override string DefaultName
        {
            get { return "Ancient Druid Tome of Nature"; }
        }

        public override ClassSystem AltarSystem
        {
            get { return ClassSystem.Druid; }
        }

        #region serialization
        public DruidAltarActiveComponent( Serial serial )
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