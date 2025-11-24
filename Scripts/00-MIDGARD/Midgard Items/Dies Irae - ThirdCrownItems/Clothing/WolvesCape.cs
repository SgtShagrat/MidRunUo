using Midgard.Engines.Races;

using Server;
using Server.Items;
using System;

using Core=Midgard.Engines.Races.Core;

namespace Midgard.Items
{
    /// <summary>
    /// 0x3B80 Wolves Cape - Mantellina di pelle di Lupo.
    /// Da inserire nel menu craft sarto, ma solo dei sarti di razza nordica
    /// </summary>
    public class WolvesCape : BaseOuterTorso
    {
        public override string DefaultName { get { return "wolves cape"; } }

        public override CraftResource DefaultResource { get { return CraftResource.RegularLeather; } }

        [Constructable]
        public WolvesCape()
            : this( 0 )
        {
        }

        [Constructable]
        public WolvesCape( int hue )
            : base( 0x3B80, hue )
        {
            Weight = 7.0;
        }

        public override bool CanBeCraftedWith( Type t )
        {
            return t == typeof( WolfLeather );
        }

        public override bool CanBeCraftedBy( Mobile from )
        {
            return from.Race is MidgardRace && from.Race == Core.NorthernHuman;
        }

        #region serialization
        public WolvesCape( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion
    }
}