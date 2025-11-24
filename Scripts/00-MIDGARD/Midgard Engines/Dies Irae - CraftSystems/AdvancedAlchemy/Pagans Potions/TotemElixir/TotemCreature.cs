/***************************************************************************
 *                               TotemCreature
 *                            -------------------
 *   begin                : 24 dicembre, 2008
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections;

using Midgard.Items;

using Server.Gumps;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName( "a Totem corpse" )]
    public class TotemCreature : BaseFamiliar
    {
        public override bool BardImmune { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Greater; } }
        public override bool Commandable { get { return true; } }
        public override bool IsDispellable { get { return false; } }

        private DateTime m_NextPickup;
        private DateTime m_NextHeal;

        public TotemCreature()
        {
            Name = "a Totem";

            Body = 74;
            BaseSoundID = 422;
            Hue = 746;

            // Stats
            SetStr( 200 );
            SetDex( 100 );
            SetInt( 75 );

            SetHits( 200 );
            SetStam( 100 );
            SetMana( 75 );

            // Damage e DamageType
            SetDamage( 10, 15 );
            SetDamageType( ResistanceType.Physical, 100 );

            // Resistenze
            SetResistance( ResistanceType.Physical, 90, 100 );
            SetResistance( ResistanceType.Fire, 90, 100 );
            SetResistance( ResistanceType.Fire, 90, 100 );
            SetResistance( ResistanceType.Poison, 90, 100 );
            SetResistance( ResistanceType.Energy, 90, 100 );

            // Skills
            SetSkill( SkillName.Wrestling, 120.0 );
            SetSkill( SkillName.Macing, 150.0 );
            SetSkill( SkillName.Tactics, 50.0 );
            SetSkill( SkillName.MagicResist, 160.0 );
            SetSkill( SkillName.Magery, 75.0 );

            // ControlSlots
            ControlSlots = 1;

            // Strong Backpack per portare oggetti
            Container pack = Backpack;
            if( pack != null )
                pack.Delete();

            pack = new StrongBackpack();
            pack.Movable = false;
            AddItem( pack );
        }

        public TotemCreature( Serial serial )
            : base( serial )
        {
        }

        public override void OnThink()
        {
            base.OnThink();

            // Controlla e setta il ControlMaster del Totem
            Mobile master = ControlMaster;
            if( master == null )
                return;

            #region heal
            if( DateTime.Now > m_NextHeal )
            {
                // Setta la curata successiva
                m_NextHeal = DateTime.Now + TimeSpan.FromSeconds( Utility.RandomMinMax( 5, 10 ) );
                if( master.Poisoned )
                {
                    Poison p = master.Poison;
                    if( p != null )
                    {
                        // La percentuale che il Totem curi dal veleno è del 75% scalabile con EP
                        double chance = BasePotion.Scale( master, 0.75 );

                        if( chance > Utility.RandomDouble() )
                        {
                            if( master.CurePoison( this ) )
                            {
                                // Se funziona cura il veleno
                                master.LocalOverheadMessage( MessageType.Regular, 0x3B2, true,
                                                             "Il tuo totem avverte che sei avvelenato e ti cura!" );
                                return;
                            }
                            else
                            {
                                // Qualcosa è andato storto nella cura del veleno...
                                master.SendMessage( "Il tuo totem fallisce nel curarti dal veleno!" );
                                return;
                            }
                        }
                        else
                        {
                            // Il totem non riesce a curare dal veleno il suo Master
                            master.SendMessage( "Il tuo totem fallisce nel curarti dal veleno!" );
                            return;
                        }
                    }
                    else
                    {
                        // Qualcosa è andato storto nel rilevamento del veleno
                        return;
                    }
                }
                else if( master.Hits < 30 )
                {
                    // La percentuale che il Totem curi dal veleno è del 75% scalabile con EP
                    double chance = BasePotion.Scale( master, 0.75 );

                    int toHeal = Utility.RandomMinMax( BasePotion.Scale( master, 30 ), BasePotion.Scale( master, 40 ) );

                    if( chance > Utility.RandomDouble() )
                    {
                        // Se funziona cura 30-40 hp scalati secondo EP
                        master.Heal( toHeal );
                        master.LocalOverheadMessage( MessageType.Regular, 0x3B2, true,
                                                     "Il tuo totem avverte che sei ferito e ti cura!" );
                        return;
                    }
                    else
                    {
                        // Qualcosa è andato storto nella cura
                        master.SendMessage( "Il tuo totem fallisce nel curarti!" );
                        return;
                    }
                }
            }
            #endregion

            #region loot
            if( DateTime.Now > m_NextPickup )
            {
                m_NextPickup = DateTime.Now + TimeSpan.FromSeconds( Utility.RandomMinMax( 5, 10 ) );

                Container pack = Backpack;
                if( pack == null )
                    return;

                ArrayList list = new ArrayList();
                foreach( Item item in GetItemsInRange( 2 ) )
                {
                    if( item.Movable && item.Stackable )
                        list.Add( item );
                }

                int pickedUp = 0;
                for( int i = 0; i < list.Count; ++i )
                {
                    Item item = (Item)list[ i ];
                    if( !pack.CheckHold( this, item, false, true ) )
                        return;

                    bool rejected;
                    LRReason reject;
                    NextActionTime = DateTime.Now;
                    Lift( item, item.Amount, out rejected, out reject );
                    if( rejected )
                        continue;
                    Drop( this, Point3D.Zero );
                    if( ++pickedUp == 3 )
                        break;
                }
            }
            #endregion
        }

        public override void BeginRelease( Mobile from )
        {
            Container pack = Backpack;
            if( pack != null && pack.Items.Count > 0 )
                from.SendGump( new WarningGump( 1060635, 30720, 1061672, 32512, 420, 280,
                                              new WarningGumpCallback( ConfirmRelease_Callback ), null ) );
            else
                EndRelease( from );
        }

        #region Pack Animal Methods
        public override bool OnBeforeDeath()
        {
            if( !base.OnBeforeDeath() )
            {
                return false;
            }
            PackAnimal.CombineBackpacks( this );
            return true;
        }

        public override DeathMoveResult GetInventoryMoveResultFor( Item item )
        {
            return DeathMoveResult.MoveToCorpse;
        }

        public override bool IsSnoop( Mobile from )
        {
            if( PackAnimal.CheckAccess( this, from ) )
                return false;

            return base.IsSnoop( from );
        }

        public override bool OnDragDrop( Mobile from, Item item )
        {
            if( CheckFeed( from, item ) )
                return true;

            if( PackAnimal.CheckAccess( this, from ) )
            {
                AddToBackpack( item );
                return true;
            }

            return base.OnDragDrop( from, item );
        }

        public override bool CheckNonlocalDrop( Mobile from, Item item, Item target )
        {
            return PackAnimal.CheckAccess( this, from );
        }

        public override bool CheckNonlocalLift( Mobile from, Item item )
        {
            return PackAnimal.CheckAccess( this, from );
        }

        public override void OnDoubleClick( Mobile from )
        {
            PackAnimal.TryPackOpen( this, from );
        }
        #endregion

        private void ConfirmRelease_Callback( Mobile from, bool okay, object state )
        {
            if( okay )
            {
                EndRelease( from );
            }
        }

        #region serial deserial
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

        public void StartRebind( Mobile from )
        {
            Container pack = from.Backpack;
            if( pack != null )
            {
                Elixir el = pack.FindItemByType( typeof( Elixir ) ) as Elixir;
                if( el != null )
                {
                    Container totempack = Backpack;
                    if( totempack != null && totempack.Items.Count > 0 )
                    {
                        from.SendGump( new WarningGump( 1060635, 30720, 1061672, 32512, 420, 280, new WarningGumpCallback( ConfirmRebind_Callback ), null ) );
                    }
                    else
                    {
                        EndRebind( from );
                    }
                }
            }
        }

        private void ConfirmRebind_Callback( Mobile from, bool okay, object state )
        {
            if( okay )
            {
                EndRebind( from );

                Totem totem = new Totem();
                totem.AddIdentifier( from );

                from.AddToBackpack( totem );
            }
        }

        private void EndRebind( Mobile from )
        {
            if( !Deleted && Controlled && from == ControlMaster && from.CheckAlive() )
            {
                Effects.SendLocationParticles( EffectItem.Create( Location, Map, EffectItem.DefaultDuration ), 0x3728, 1, 13, 2100, 3, 5042, 0 );
                from.PlaySound( 0x201 );
                Delete();
            }
        }
    }
}