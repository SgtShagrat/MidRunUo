using Server;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;
using PetPar = Midgard.Engines.PetSystem.PetUtility.PetPar;

namespace Midgard.Engines.PetSystem
{
    public class PetLevelGump : Gump
    {
        private static string Locked = "Locked";
        private static string Unknown = "Unknown";

        private enum Buttons
        {
            Close,

            PetHits,
            PetStam,
            PetMana,

            PetResPhys,
            PetResFire,
            PetResCold,
            PetResPois,
            PetResNrgy,

            PetMinDam,
            PetMaxDam,

            PetVirArm,

            RoarAttack,
            PetPoisonAttack,
            FireBreathAttack,
        }

        #region campi
        private BaseCreature m_Pet;
        #endregion

        #region costruttori
        public PetLevelGump( BaseCreature pet )
            : base( 0, 0 )
        {
            m_Pet = pet;

            Closable = true;
            Disposable = true;
            Dragable = true;
            Resizable = false;

            AddPage( 0 );

            AddBackground( 12, 9, 394, 526, 2620 );
            AddImageTiled( 17, 15, 384, 113, 9274 );
            AddImageTiled( 17, 136, 302, 27, 9274 );
            AddImageTiled( 17, 171, 302, 356, 9274 );
            AddImageTiled( 326, 136, 76, 27, 9274 );
            AddImageTiled( 326, 171, 76, 356, 9274 );
            AddAlphaRegion( 16, 15, 384, 511 );

            AddLabel( 22, 20, 1149, "Ability Points:" );
            AddLabel( 22, 40, 1149, "Pets Current Level:" );
            AddLabel( 22, 60, 1149, "Pets Maxium Level:" );
            AddLabel( 22, 80, 1149, "Pets Gender:" );
            AddLabel( 22, 100, 1149, "Pets Name:" );

            AddLabel( 116, 20, 64, m_Pet.AbilityPoints.ToString() );
            AddLabel( 149, 40, 64, m_Pet.Level.ToString() );
            AddLabel( 144, 60, 64, m_Pet.MaxLevel.ToString() );

            AddImage( 336, 20, 5549 );

            // AddButton( 300, 100, 2117, 2118, 1, GumpButtonType.Page, 1 );
            // AddButton( 320, 100, 2117, 2118, 1, GumpButtonType.Page, 2 );

            if( m_Pet.Female )
                AddLabel( 107, 80, 64, "Female" );
            else
                AddLabel( 107, 80, 64, "Male" );

            AddLabel( 96, 100, 64, m_Pet.Name );

            AddLabel( 22, 140, 1149, "Property Name" );
            AddLabel( 330, 140, 1149, "Amount" );

            AddPage( 1 );

            if( m_Pet.Level != 0 )
            {
                AddLabel( 60, 175, 1149, "Hit Points" );
                AddLabel( 60, 200, 1149, "Stamina" );
                AddLabel( 60, 225, 1149, "Mana" );

                AddLabel( 330, 175, 1149, string.Format( "{0}/{1}", m_Pet.HitsMax, ( PetUtility.GetParMax( PetPar.PetHits, m_Pet ) ) ) );
                AddLabel( 330, 200, 1149, string.Format( "{0}/{1}", m_Pet.StamMax, ( PetUtility.GetParMax( PetPar.PetStam, m_Pet ) ) ) );
                AddLabel( 330, 225, 1149, string.Format( "{0}/{1}", m_Pet.ManaMax, ( PetUtility.GetParMax( PetPar.PetMana, m_Pet ) ) ) );

                AddButton( 24, 175, 4005, 4006, (int)Buttons.PetHits, GumpButtonType.Reply, 0 );
                AddButton( 24, 200, 4005, 4006, (int)Buttons.PetStam, GumpButtonType.Reply, 0 );
                AddButton( 24, 225, 4005, 4006, (int)Buttons.PetMana, GumpButtonType.Reply, 0 );
            }
            else
            {
                AddLabel( 60, 175, 38, Locked );
                AddLabel( 60, 200, 38, Locked );
                AddLabel( 60, 225, 38, Locked );

                AddLabel( 330, 175, 38, Unknown );
                AddLabel( 330, 200, 38, Unknown );
                AddLabel( 330, 225, 38, Unknown );
            }

            if( m_Pet.Level >= 10 )
            {
                AddLabel( 60, 250, 1149, "Physical Resistance" );
                AddLabel( 60, 275, 1149, "Fire Resistance" );
                AddLabel( 60, 300, 1149, "Cold Resistance" );
                AddLabel( 60, 325, 1149, "Poison Resistance" );
                AddLabel( 60, 350, 1149, "Energy Resistance" );

                AddLabel( 330, 250, 1149, string.Format( "{0}/{1}", m_Pet.PhysicalResistance, ( PetUtility.GetParMax( PetPar.PetResPhys, m_Pet ) ) ) );
                AddLabel( 330, 275, 1149, string.Format( "{0}/{1}", m_Pet.FireResistance, ( PetUtility.GetParMax( PetPar.PetResFire, m_Pet ) ) ) );
                AddLabel( 330, 300, 1149, string.Format( "{0}/{1}", m_Pet.ColdResistance, ( PetUtility.GetParMax( PetPar.PetResCold, m_Pet ) ) ) );
                AddLabel( 330, 325, 1149, string.Format( "{0}/{1}", m_Pet.PoisonResistance, ( PetUtility.GetParMax( PetPar.PetResPois, m_Pet ) ) ) );
                AddLabel( 330, 350, 1149, string.Format( "{0}/{1}", m_Pet.EnergyResistance, ( PetUtility.GetParMax( PetPar.PetResNrgy, m_Pet ) ) ) );

                AddButton( 24, 250, 4005, 4006, (int)Buttons.PetResPhys, GumpButtonType.Reply, 0 );
                AddButton( 24, 275, 4005, 4006, (int)Buttons.PetResFire, GumpButtonType.Reply, 0 );
                AddButton( 24, 300, 4005, 4006, (int)Buttons.PetResCold, GumpButtonType.Reply, 0 );
                AddButton( 24, 325, 4005, 4006, (int)Buttons.PetResPois, GumpButtonType.Reply, 0 );
                AddButton( 24, 350, 4005, 4006, (int)Buttons.PetResNrgy, GumpButtonType.Reply, 0 );
            }
            else
            {
                AddLabel( 60, 250, 38, Locked );
                AddLabel( 60, 275, 38, Locked );
                AddLabel( 60, 300, 38, Locked );
                AddLabel( 60, 325, 38, Locked );
                AddLabel( 60, 350, 38, Locked );

                AddLabel( 330, 250, 38, Unknown );
                AddLabel( 330, 275, 38, Unknown );
                AddLabel( 330, 300, 38, Unknown );
                AddLabel( 330, 325, 38, Unknown );
                AddLabel( 330, 350, 38, Unknown );
            }

            if( m_Pet.Level >= 20 )
            {
                AddLabel( 60, 375, 1149, "Min Damage" );
                AddLabel( 60, 400, 1149, "Max Damage" );

                AddLabel( 330, 375, 1149, string.Format( "{0}/{1}", m_Pet.DamageMin, ( PetUtility.GetParMax( PetPar.PetMinDam, m_Pet ) ) ) );
                AddLabel( 330, 400, 1149, string.Format( "{0}/{1}", m_Pet.DamageMax, ( PetUtility.GetParMax( PetPar.PetMaxDam, m_Pet ) ) ) );

                AddButton( 24, 375, 4005, 4006, (int)Buttons.PetMinDam, GumpButtonType.Reply, 0 );
                AddButton( 24, 400, 4005, 4006, (int)Buttons.PetMaxDam, GumpButtonType.Reply, 0 );
            }
            else
            {
                AddLabel( 60, 375, 38, Locked );
                AddLabel( 60, 400, 38, Locked );

                AddLabel( 330, 375, 38, Unknown );
                AddLabel( 330, 400, 38, Unknown );
            }

            if( m_Pet.Level >= 30 )
            {
                AddLabel( 60, 425, 1149, "Armor Rating" );

                AddLabel( 330, 425, 1149, string.Format( "{0}/{1}", m_Pet.VirtualArmor, ( PetUtility.GetParMax( PetPar.PetVirArm, m_Pet ) ) ) );

                AddButton( 24, 425, 4005, 4006, (int)Buttons.PetVirArm, GumpButtonType.Reply, 0 );
            }
            else
            {
                AddLabel( 60, 425, 38, Locked );

                AddLabel( 330, 425, 38, Unknown );
            }

            // AddPage( 2 );

            if( m_Pet.Level > 25 )
            {
                AddLabel( 60, 450, 1149, "Roar Attack" );
                AddLabel( 60, 475, 1149, "Poison Attack" );
                AddLabel( 60, 500, 1149, "Fire Breath Attack" );

                AddLabel( 330, 450, 1149, string.Format( "{0}/{1}", m_Pet.RoarAttack, PetUtility.GetParMax( PetPar.RoarAttack, m_Pet ) ) );
                AddLabel( 330, 475, 1149, string.Format( "{0}/{1}", m_Pet.PetPoisonAttack, PetUtility.GetParMax( PetPar.PetPoisonAttack, m_Pet ) ) );
                AddLabel( 330, 500, 1149, string.Format( "{0}/{1}", m_Pet.FireBreathAttack, PetUtility.GetParMax( PetPar.FireBreathAttack, m_Pet ) ) );

                AddButton( 24, 450, 4005, 4006, (int)Buttons.RoarAttack, GumpButtonType.Reply, 0 );
                AddButton( 24, 475, 4005, 4006, (int)Buttons.PetPoisonAttack, GumpButtonType.Reply, 0 );
                AddButton( 24, 500, 4005, 4006, (int)Buttons.FireBreathAttack, GumpButtonType.Reply, 0 );
            }
            else
            {
                AddLabel( 60, 450, 38, Locked );
                AddLabel( 60, 475, 38, Locked );
                AddLabel( 60, 500, 38, Locked );

                AddLabel( 330, 450, 38, Unknown );
                AddLabel( 330, 475, 38, Unknown );
                AddLabel( 330, 500, 38, Unknown );
            }
        }
        #endregion

        public override void OnResponse( NetState state, RelayInfo info )
        {
            Mobile from = state.Mobile;

            if( from == null )
                return;

            switch( info.ButtonID )
            {
                case (int)Buttons.Close: break;

                case (int)Buttons.PetHits: PetUtility.CheckParameter( from, m_Pet, PetPar.PetHits, true, true ); break;
                case (int)Buttons.PetStam: PetUtility.CheckParameter( from, m_Pet, PetPar.PetStam, true, true ); break;
                case (int)Buttons.PetMana: PetUtility.CheckParameter( from, m_Pet, PetPar.PetMana, true, true ); break;

                case (int)Buttons.PetResPhys: PetUtility.CheckParameter( from, m_Pet, PetPar.PetResPhys, true, true ); break;
                case (int)Buttons.PetResFire: PetUtility.CheckParameter( from, m_Pet, PetPar.PetResFire, true, true ); break;
                case (int)Buttons.PetResCold: PetUtility.CheckParameter( from, m_Pet, PetPar.PetResCold, true, true ); break;
                case (int)Buttons.PetResPois: PetUtility.CheckParameter( from, m_Pet, PetPar.PetResPois, true, true ); break;
                case (int)Buttons.PetResNrgy: PetUtility.CheckParameter( from, m_Pet, PetPar.PetResNrgy, true, true ); break;

                case (int)Buttons.PetMinDam: PetUtility.CheckParameter( from, m_Pet, PetPar.PetMinDam, true, true ); break;
                case (int)Buttons.PetMaxDam: PetUtility.CheckParameter( from, m_Pet, PetPar.PetMaxDam, true, true ); break;

                case (int)Buttons.PetVirArm: PetUtility.CheckParameter( from, m_Pet, PetPar.PetVirArm, true, true ); break;

                case (int)Buttons.RoarAttack: PetUtility.CheckParameter( from, m_Pet, PetPar.RoarAttack, true, true ); break;
                case (int)Buttons.PetPoisonAttack: PetUtility.CheckParameter( from, m_Pet, PetPar.PetPoisonAttack, true, true ); break;
                case (int)Buttons.FireBreathAttack: PetUtility.CheckParameter( from, m_Pet, PetPar.FireBreathAttack, true, true ); break;
            }
        }
    }
}