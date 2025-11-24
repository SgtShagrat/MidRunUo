/***************************************************************************
 *                                   SolaretesOfSacrifice.cs
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
 * 			Sandali.
 * 
 * 	
 * 
 ***************************************************************************/

/*
using Server.Factions;

namespace Server.Items
{
    [FlipableAttribute( 0x2B12, 0x2B13 )]
    public class LegionarySolaretesOfSacrifice : BaseArmor
    {
        #region proprietà
        public override int LabelNumber { get { return 1064313; } }

        public override int BasePhysicalResistance { get { return 0; } }
        public override int BaseFireResistance { get { return 0; } }
        public override int BaseColdResistance { get { return 0; } }
        public override int BasePoisonResistance { get { return 0; } }
        public override int BaseEnergyResistance { get { return 0; } }

        public override int PhysicalResistance { get { return 0; } }
        public override int FireResistance { get { return 0; } }
        public override int ColdResistance { get { return 0; } }
        public override int PoisonResistance { get { return 0; } }
        public override int EnergyResistance { get { return 0; } }

        public override int InitMinHits { get { return 50; } }
        public override int InitMaxHits { get { return 65; } }

        public override int AosStrReq { get { return 20; } }

        public override int OldStrReq { get { return 20; } }
        public override int OldDexBonus { get { return -3; } }

        public override int ArmorBase { get { return 20; } }

        public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Plate; } }
        #endregion

        #region costruttori
        [Constructable]
        public LegionarySolaretesOfSacrifice()
            : base( 0x2B12 )
        {
            //			Name = "Solaretes Of Sacrifice";
            Weight = 2.0;
        }

        public LegionarySolaretesOfSacrifice( Serial serial )
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
            if( m == null || !m.Player )
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