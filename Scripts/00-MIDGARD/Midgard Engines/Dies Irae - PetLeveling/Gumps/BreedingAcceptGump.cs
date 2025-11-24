using System;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Midgard.Engines.PetSystem
{
    public class BreedingAcceptGump : Gump
    {
        private Mobile m_Pet1;
        private Mobile m_Pet2;

        public BreedingAcceptGump( Mobile pet1, Mobile pet2 )
            : base( 0, 0 )
        {
            Closable = false;
            Disposable = false;
            Dragable = true;
            Resizable = false;

            m_Pet1 = pet1;
            m_Pet2 = pet2;

            BaseCreature bc = (BaseCreature)m_Pet1;

            AddPage( 0 );
            AddBackground( 25, 22, 318, 324, 9380 );
            AddLabel( 52, 27, 1149, @"Breeding Request" );
            AddButton( 46, 321, 4005, 4006, 1, GumpButtonType.Reply, 0 );
            AddLabel( 78, 322, 1149, @"Baby Stat Table" );
            AddLabel( 52, 60, 1149, @"Requested By:" );
            AddLabel( 52, 80, 1149, @"Requesters Pet:" );
            AddLabel( 52, 100, 1149, @"Your Pet:" );
            AddHtml( 49, 122, 271, 161, @"<CENTER><U>Breeding Request</U><BR><BR>Someone has requested to breed thier pet with one of yours. If you deside to accept this offer both you and the other pet owner will get a baby from this union.<BR><BR>If you accept and breeding is successful both pets (Mother/Father) will be stabled in the animal breeders stable untill the birth of the babies. This takes three days. After that both pets will not be able to breed again for 60 days. The animal breeder will give you a claim ticket, When the three days our up you can then return to the animal breeder and drop your ticket on them, They will charge a five thousand gold coin fee for thier services. Then you will get back your pet and the baby.<BR><BR>Do you accept?", false, true );
            AddButton( 50, 290, 4023, 4024, 2, GumpButtonType.Reply, 0 );
            AddButton( 83, 290, 4017, 4018, 3, GumpButtonType.Reply, 0 );

            if( bc.ControlMaster != null )
                AddLabel( 144, 60, 64, bc.ControlMaster.Name );

            if( m_Pet1 != null )
                AddLabel( 158, 80, 64, m_Pet1.Name );

            if( m_Pet2 != null )
                AddLabel( 117, 100, 64, m_Pet2.Name );
        }

        public override void OnResponse( NetState state, RelayInfo info )
        {
            Mobile from = state.Mobile;

            BaseCreature bc1 = (BaseCreature)m_Pet1;
            BaseCreature bc2 = (BaseCreature)m_Pet2;

            Mobile cm1 = bc1.ControlMaster;
            Mobile cm2 = bc2.ControlMaster;

            if( from == null )
                return;

            if( info.ButtonID == 1 )
            {
                from.CloseGump( typeof( BreedingAcceptGump ) );

                from.SendGump( new BreedingAcceptGump( m_Pet1, m_Pet2 ) );
            }

            if( info.ButtonID == 2 )
            {
                Mobile breeder = null;
                Mobile owner = null;

                int ai = 0;

                if( bc1.AI == AIType.AI_Mage && bc2.AI == AIType.AI_Mage )
                    ai = 1;
                if( bc1.AI == AIType.AI_Melee && bc2.AI == AIType.AI_Melee )
                    ai = 2;

                #region calcolo delle medie dei due pet genitori
                int xstr = bc1.RawStr + bc2.RawStr;
                int xdex = bc1.RawDex + bc2.RawDex;
                int xint = bc1.RawInt + bc2.RawInt;

                int xhits = bc1.HitsMax + bc2.HitsMax;
                int xstam = bc1.StamMax + bc2.StamMax;
                int xmana = bc1.ManaMax + bc2.ManaMax;

                int xphys = bc1.PhysicalResistance + bc2.PhysicalResistance;
                int xfire = bc1.FireResistance + bc2.FireResistance;
                int xcold = bc1.ColdResistance + bc2.ColdResistance;
                int xnrgy = bc1.EnergyResistance + bc2.EnergyResistance;
                int xpois = bc1.PoisonResistance + bc2.PoisonResistance;

                int xdmin = bc1.DamageMin + bc2.DamageMin;
                int xdmax = bc1.DamageMax + bc2.DamageMax;

                int xmlev = bc1.Level + bc2.Level;

                int newStr = xstr / 2;
                int newDex = xdex / 2;
                int newInt = xint / 2;

                int newHits = xhits / 2;
                int newStam = xstam / 2;
                int newMana = xmana / 2;

                int newPhys = xphys / 2;
                int newFire = xfire / 2;
                int newCold = xcold / 2;
                int newNrgy = xnrgy / 2;
                int newPois = xpois / 2;

                int newDmin = xdmin / 2;
                int newDmax = xdmax / 2;

                int newMlev = xmlev / 2;
                #endregion

                #region bonus di generazione
                int babyStr = newStr + Utility.RandomMinMax( 0, 1 );
                int babyDex = newDex + Utility.RandomMinMax( 0, 1 );
                int babyInt = newInt + Utility.RandomMinMax( 0, 1 );

                int babyHits = newHits + Utility.RandomMinMax( 0, 2 );
                int babyStam = newStam + Utility.RandomMinMax( 0, 2 );
                int babyMana = newMana + Utility.RandomMinMax( 0, 2 );

                int babyPhys = newPhys + Utility.RandomMinMax( 0, 2 );
                int babyFire = newFire + Utility.RandomMinMax( 0, 2 );
                int babyCold = newCold + Utility.RandomMinMax( 0, 2 );
                int babyNrgy = newNrgy + Utility.RandomMinMax( 0, 2 );
                int babyPois = newPois + Utility.RandomMinMax( 0, 2 );

                int babyDmin = newDmin + Utility.RandomMinMax( 0, 1 );
                int babyDmax = newDmax + Utility.RandomMinMax( 0, 1 );

                int babyMlev = newMlev + Utility.RandomMinMax( 1, 3 );
                #endregion

                int stats = babyStr + babyDex + babyInt + babyHits + babyStam + babyMana + babyPhys + babyFire + babyCold + babyNrgy + babyPois + babyDmin + babyDmax + babyMlev;
                int newPrice = stats * 3;
                int babyPrice = newPrice;

                int chance = ( stats <= 1500 ? 1500 : stats );

                #region normalizzazione del baby
                if( babyStr >= PetUtility.GetParMax( PetUtility.PetPar.PetStr, bc1 ) )
                    babyStr = PetUtility.GetParMax( PetUtility.PetPar.PetStr, bc1 );

                if( babyDex >= PetUtility.GetParMax( PetUtility.PetPar.PetDex, bc1 ) )
                    babyDex = PetUtility.GetParMax( PetUtility.PetPar.PetDex, bc1 );

                if( babyInt >= PetUtility.GetParMax( PetUtility.PetPar.PetInt, bc1 ) )
                    babyInt = PetUtility.GetParMax( PetUtility.PetPar.PetInt, bc1 );

                if( babyHits >= PetUtility.GetParMax( PetUtility.PetPar.PetHits, bc1 ) )
                    babyHits = PetUtility.GetParMax( PetUtility.PetPar.PetHits, bc1 );

                if( babyStam >= PetUtility.GetParMax( PetUtility.PetPar.PetStam, bc1 ) )
                    babyStam = PetUtility.GetParMax( PetUtility.PetPar.PetStam, bc1 );

                if( babyMana >= PetUtility.GetParMax( PetUtility.PetPar.PetMana, bc1 ) )
                    babyMana = PetUtility.GetParMax( PetUtility.PetPar.PetMana, bc1 );

                if( babyPhys >= PetUtility.GetParMax( PetUtility.PetPar.PetResPhys, bc1 ) )
                    babyPhys = PetUtility.GetParMax( PetUtility.PetPar.PetResPhys, bc1 );

                if( babyFire >= PetUtility.GetParMax( PetUtility.PetPar.PetResFire, bc1 ) )
                    babyFire = PetUtility.GetParMax( PetUtility.PetPar.PetResFire, bc1 );

                if( babyCold >= PetUtility.GetParMax( PetUtility.PetPar.PetResCold, bc1 ) )
                    babyCold = PetUtility.GetParMax( PetUtility.PetPar.PetResCold, bc1 );

                if( babyNrgy >= PetUtility.GetParMax( PetUtility.PetPar.PetResNrgy, bc1 ) )
                    babyNrgy = PetUtility.GetParMax( PetUtility.PetPar.PetResNrgy, bc1 );

                if( babyPois >= PetUtility.GetParMax( PetUtility.PetPar.PetResPois, bc1 ) )
                    babyPois = PetUtility.GetParMax( PetUtility.PetPar.PetResPois, bc1 );

                if( babyDmin >= PetUtility.GetParMax( PetUtility.PetPar.PetMinDam, bc1 ) )
                    babyDmin = PetUtility.GetParMax( PetUtility.PetPar.PetMinDam, bc1 );

                if( babyDmax >= PetUtility.GetParMax( PetUtility.PetPar.PetMaxDam, bc1 ) )
                    babyDmax = PetUtility.GetParMax( PetUtility.PetPar.PetMaxDam, bc1 );

                if( babyMlev >= PetUtility.GetParMax( PetUtility.PetPar.PetMaxLev, bc1 ) )
                    babyMlev = PetUtility.GetParMax( PetUtility.PetPar.PetMaxLev, bc1 );
                #endregion

                #region controllo breeeder
                foreach( Mobile m in from.GetMobilesInRange( 5 ) )
                {
                    if( m is AnimalBreeder )
                        breeder = m;

                    if( m == cm1 )
                        owner = m;
                }

                if( breeder == null )
                {
                    from.SendMessage( "You must be near an animal breeder in order to breed your pet." );

                    if( cm1 != null )
                        cm1.SendMessage( "The owner of the other pet is too far away from the animal breeder." );
                }
                else if( owner == null )
                {
                    from.SendMessage( "The owner of the other pet is not near by." );

                    if( cm1 != null )
                        cm1.SendMessage( "You are to far away from the other pet owner." );
                }
                    #endregion

                else if( Utility.Random( chance ) < 1500 )
                {
                    #region primo ticket
                    if( cm1 != null )
                    {
                        PetClaimTicket pct = new PetClaimTicket();
                        pct.AI = ai;
                        pct.Owner = cm1;
                        pct.Pet = m_Pet1;
                        pct.Str = babyStr;
                        pct.Dex = babyDex;
                        pct.Int = babyInt;
                        pct.Hits = babyHits;
                        pct.Stam = babyStam;
                        pct.Mana = babyMana;
                        pct.Phys = babyPhys;
                        pct.Fire = babyFire;
                        pct.Cold = babyCold;
                        pct.Nrgy = babyNrgy;
                        pct.Pois = babyPois;
                        pct.Dmin = babyDmin;
                        pct.Dmax = babyDmax;
                        pct.Mlev = babyMlev;
                        pct.Gen = bc1.Generation;
                        pct.Price = babyPrice;
                        cm1.AddToBackpack( pct );

                        breeder.SayTo( cm1, "Ill hold onto your pet for you while its mating." );
                        breeder.SayTo( cm1, "Return here in three days and the show me that claim ticket i gave to you." );
                        cm1.SendMessage( "They have accepted your offer." );

                        bc1.ControlTarget = null;
                        bc1.ControlOrder = OrderType.Stay;
                        bc1.Internalize();
                        bc1.IsStabled = true;
                        bc1.SetControlMaster( null );
                    }
                    #endregion

                    #region secondo ticket
                    if( cm2 != null )
                    {
                        PetClaimTicket pct = new PetClaimTicket();
                        pct.AI = ai;
                        pct.Owner = cm2;
                        pct.Pet = m_Pet2;
                        pct.Str = babyStr;
                        pct.Dex = babyDex;
                        pct.Int = babyInt;
                        pct.Hits = babyHits;
                        pct.Stam = babyStam;
                        pct.Mana = babyMana;
                        pct.Phys = babyPhys;
                        pct.Fire = babyFire;
                        pct.Cold = babyCold;
                        pct.Nrgy = babyNrgy;
                        pct.Pois = babyPois;
                        pct.Dmin = babyDmin;
                        pct.Dmax = babyDmax;
                        pct.Mlev = babyMlev;
                        pct.Gen = bc2.Generation;
                        pct.Price = babyPrice;
                        cm2.AddToBackpack( pct );

                        breeder.SayTo( cm2, "Ill hold onto your pet for you while its mating." );
                        breeder.SayTo( cm2, "Return here in three days and the show me that claim ticket i gave to you." );
                        cm2.SendMessage( "You accept their offer." );

                        bc2.ControlTarget = null;
                        bc2.ControlOrder = OrderType.Stay;
                        bc2.Internalize();
                        bc2.IsStabled = true;
                        bc2.SetControlMaster( null );
                    }
                    #endregion

                    bc1.MatingDelay = DateTime.Now + TimeSpan.FromDays( PetUtility.GetParMax( PetUtility.PetPar.PetMinBreedDelay, bc1 ) );
                    bc2.MatingDelay = DateTime.Now + TimeSpan.FromDays( PetUtility.GetParMax( PetUtility.PetPar.PetMinBreedDelay, bc2 ) );
                }
                else
                {
                    if( cm1 != null && cm2 != null )
                    {
                        cm1.SendMessage( "Breeding Failed: It is hard to successfully mate to strong pets together, You will have to wait twelve hours to try again." );
                        cm2.SendMessage( "Breeding Failed: It is hard to successfully mate to strong pets together, You will have to wait twelve hours to try again." );
                        bc1.MatingDelay = DateTime.Now + TimeSpan.FromHours( 12.0 );
                        bc2.MatingDelay = DateTime.Now + TimeSpan.FromHours( 12.0 );
                    }
                }
            }

            if( info.ButtonID == 3 )
            {
                from.SendMessage( "You have declined thier offer." );

                if( cm1 != null )
                {
                    cm1.SendMessage( "They have declined your offer" );
                }
            }
        }
    }
}