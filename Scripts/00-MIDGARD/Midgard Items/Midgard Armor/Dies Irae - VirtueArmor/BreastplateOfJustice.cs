/***************************************************************************
 *                                   BreastplateOfJustice.cs
 *                            		---------------------
 *  begin               	: Dicembre, 2006
 * 	version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info
 * 			Armatura per la fazione good.
 * 			Busto Maschile.
 * 
 * 	
 * 
 ***************************************************************************/

/*
using Server.Factions;

namespace Server.Items
{
    public class LegionaryBreastplateOfJustice : BaseArmor
    {
        #region proprietà
        public override int LabelNumber { get { return 1064310; } }

        public override int BasePhysicalResistance { get { return 5; } }
        public override int BaseFireResistance { get { return 3; } }
        public override int BaseColdResistance { get { return 2; } }
        public override int BasePoisonResistance { get { return 3; } }
        public override int BaseEnergyResistance { get { return 2; } }

        public override int InitMinHits { get { return 50; } }
        public override int InitMaxHits { get { return 65; } }

        public override int AosStrReq { get { return 95; } }
        public override int OldStrReq { get { return 60; } }

        public override int OldDexBonus { get { return -8; } }

        public override int ArmorBase { get { return 40; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Plate; } }

        public override bool AllowFemaleWearer { get { return false; } }
        #endregion

        #region costruttori
        [Constructable]
        public LegionaryBreastplateOfJustice()
            : base( 0x2B08 )
        {
            //			Name = "Breastplate Of Justice";
            Weight = 10.0;
        }

        public LegionaryBreastplateOfJustice( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override bool OnEquip( Mobile from )
        {
            return Validate( from ) && base.OnEquip( from );
        }

        public bool Validate( Mobile m )
        {
            if( !Core.AOS || m == null || !m.Player )
                return true;

            Faction f = Faction.Find( m );
            if( f is LegioImperialis )
            {
                return true;
            }
            else
            {
                m.SendLocalizedMessage( 1064336 ); // Only an good Legionary can wear this!
                return false;
            }
        }
        #endregion

        #region serialize-deserialize
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
*/