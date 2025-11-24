using System;
using System.Collections;
using Midgard.Engines.SpellSystem;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Regions;
using Server.Spells;
using Server.Spells.Chivalry;
using Server.Spells.Fourth;
using Server.Spells.Ninjitsu;
using Server.Spells.Seventh;
using Server.Spells.Sixth;

namespace Midgard.Regions
{
    public class CustomRegion : GuardedRegion
    {
        public RegionControl Controller { get; private set; }

        public CustomRegion( RegionControl control )
            : base( control.RegionName, control.Map, control.RegionPriority, control.RegionArea )
        {
            Disabled = !control.IsGuarded;
            Music = control.Music;
            Controller = control;
        }

        private Timer m_Timer;

        public override void OnDeath( Mobile m )
        {
            // Start a 1 second timer
            // The Timer will check if they need moving, corpse deleting etc.
            m_Timer = new MovePlayerTimer( m, Controller );
            m_Timer.Start();

            base.OnDeath( m );
        }

        private class MovePlayerTimer : Timer
        {
            private Mobile m_M;
            private RegionControl m_Controller;

            public MovePlayerTimer( Mobile mobile, RegionControl controller )
                : base( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.0 ) )
            {
                Priority = TimerPriority.FiftyMS;
                m_M = mobile;
                m_Controller = controller;
            }

            protected override void OnTick()
            {
                // Emptys the corpse and places items on ground
                if( m_M is PlayerMobile )
                {
                    if( m_Controller.EmptyPlayerCorpse )
                    {
                        if( m_M != null && m_M.Corpse != null )
                        {
                            ArrayList corpseitems = new ArrayList( m_M.Corpse.Items );

                            foreach( Item item in corpseitems )
                            {
                                if( ( item.Layer != Layer.Bank ) && ( item.Layer != Layer.Backpack ) && ( item.Layer != Layer.Hair ) &&
                                    ( item.Layer != Layer.FacialHair ) && ( item.Layer != Layer.Mount ) )
                                {
                                    if( ( item.LootType != LootType.Blessed ) )
                                    {
                                        item.MoveToWorld( m_M.Corpse.Location, m_M.Corpse.Map );
                                    }
                                }
                            }
                        }
                    }
                }
                else if( m_Controller.EmptyNPCCorpse )
                {
                    if( m_M != null && m_M.Corpse != null )
                    {
                        ArrayList corpseitems = new ArrayList( m_M.Corpse.Items );

                        foreach( Item item in corpseitems )
                        {
                            if( ( item.Layer != Layer.Bank ) && ( item.Layer != Layer.Backpack ) && ( item.Layer != Layer.Hair ) &&
                                ( item.Layer != Layer.FacialHair ) && ( item.Layer != Layer.Mount ) )
                            {
                                if( ( item.LootType != LootType.Blessed ) )
                                {
                                    item.MoveToWorld( m_M.Corpse.Location, m_M.Corpse.Map );
                                }
                            }
                        }
                    }
                }

                Mobile newnpc = null;

                // Resurrects Players
                if( m_M is PlayerMobile )
                {
                    if( m_Controller.ResPlayerOnDeath )
                    {
                        if( m_M != null )
                        {
                            m_M.Resurrect();
                            m_M.SendMessage( "You have been Resurrected" );
                        }
                    }
                }
                else if( m_Controller.ResNPCOnDeath )
                {
                    if( m_M != null && m_M.Corpse != null )
                    {
                        Type type = m_M.GetType();
                        newnpc = Activator.CreateInstance( type ) as Mobile;
                        if( newnpc != null )
                        {
                            newnpc.Location = m_M.Corpse.Location;
                            newnpc.Map = m_M.Corpse.Map;
                        }
                    }
                }

                // Deletes the corpse 
                if( m_M is PlayerMobile )
                {
                    if( m_Controller.DeletePlayerCorpse )
                    {
                        if( m_M != null && m_M.Corpse != null )
                        {
                            m_M.Corpse.Delete();
                        }
                    }
                }
                else if( m_Controller.DeleteNPCCorpse )
                {
                    if( m_M != null && m_M.Corpse != null )
                    {
                        m_M.Corpse.Delete();
                    }
                }

                // Move Mobiles
                if( m_M is PlayerMobile )
                {
                    if( m_Controller.MovePlayerOnDeath )
                    {
                        if( m_M != null )
                        {
                            m_M.Map = m_Controller.MovePlayerToMap;
                            m_M.Location = m_Controller.MovePlayerToLoc;
                        }
                    }
                }
                else if( m_Controller.MoveNPCOnDeath )
                {
                    if( newnpc != null )
                    {
                        newnpc.Map = m_Controller.MoveNPCToMap;
                        newnpc.Location = m_Controller.MoveNPCToLoc;
                    }
                }

                Stop();
            }
        }

        public override bool TravelRestricted
        {
            get { return Controller.TravelRestricted; }
        }

        public override bool IsDisabled()
        {
            if( !Controller.IsGuarded != Disabled )
                Controller.IsGuarded = !Disabled;

            return Disabled;
        }

        public override bool AllowBeneficial( Mobile from, Mobile target )
        {
            if( ( !Controller.AllowBenefitPlayer && target is PlayerMobile ) || ( !Controller.AllowBenefitNPC && target is BaseCreature ) )
            {
                from.SendMessage( "You cannot perform benificial acts on your target." );
                return false;
            }

            return base.AllowBeneficial( from, target );
        }

        public override bool AllowHarmful( Mobile from, Mobile target )
        {
            if( ( !Controller.AllowHarmPlayer && target is PlayerMobile ) || ( !Controller.AllowHarmNPC && target is BaseCreature ) )
            {
                from.SendMessage( "You cannot perform harmful acts on your target." );
                return false;
            }

            return base.AllowHarmful( from, target );
        }

        public override bool AllowHousing( Mobile from, Point3D p )
        {
            return Controller.AllowHousing;
        }

        public override bool AllowSpawn()
        {
            return Controller.AllowSpawn;
        }

        public override bool CanUseStuckMenu( Mobile m )
        {
            if( !Controller.CanUseStuckMenu )
                m.SendMessage( "You cannot use the Stuck menu here." );

            return Controller.CanUseStuckMenu;
        }

        public override bool OnDamage( Mobile m, ref int damage )
        {
            if( !Controller.CanBeDamaged )
            {
                m.SendMessage( "You cannot be damaged here." );
            }

            return Controller.CanBeDamaged;
        }

        public override bool OnResurrect( Mobile m )
        {
            if( !Controller.CanRessurect && m.AccessLevel == AccessLevel.Player )
                m.SendMessage( "You cannot ressurect here." );

            return Controller.CanRessurect;
        }

        public override bool OnBeginSpellCast( Mobile from, ISpell s )
        {
            if( from.AccessLevel == AccessLevel.Player )
            {
                bool restricted = Controller.IsRestrictedSpell( s );
                if( restricted )
                {
                    from.SendMessage( "You cannot cast that spell here." );
                    return false;
                }

                //if ( s is EtherealSpell && !CanMountEthereal ) Grr, EthereealSpell is private :<
                if( !Controller.CanMountEthereal && ( (Spell)s ).Info.Name == "Ethereal Mount" )
                //Hafta check with a name compare of the string to see if ethy
                {
                    from.SendMessage( "You cannot mount your ethereal here." );
                    return false;
                }

                if( Controller.TravelRestricted )
                {
                    if( ( s is GateTravelSpell || s is RecallSpell || s is MarkSpell || s is SacredJourneySpell || s is NaturesPassageSpell ) )
                    {
                        from.SendLocalizedMessage( 501802 ); // Thy spell doth not appear to work...
                        return false;
                    }
                }
            }

            //return base.OnBeginSpellCast( from, s );
            return true; //Let users customize spells, not rely on weather it's guarded or not.
        }

        public override bool OnDecay( Item item )
        {
            return Controller.ItemDecay;
        }

        public override bool OnHeal( Mobile m, ref int heal )
        {
            if( !Controller.CanHeal )
            {
                m.SendMessage( "You cannot be healed here." );
            }

            return Controller.CanHeal;
        }

        public override bool OnSkillUse( Mobile m, int skill )
        {
            bool restricted = Controller.IsRestrictedSkill( skill );

            if( restricted && m.AccessLevel == AccessLevel.Player )
            {
                m.SendMessage( "You cannot use that skill here." );
                return false;
            }

            return base.OnSkillUse( m, skill );
        }

        public override void OnExit( Mobile m )
        {
            if( Controller.ShowExitMessage )
                m.SendMessage( "You have left {0}", Name );

            if( Controller.CannotRun )
            {
                if( m.NetState != null && !TransformationSpellHelper.UnderTransformation( m, typeof( AnimalForm ) ) &&
                    !TransformationSpellHelper.UnderTransformation( m, typeof( ReaperFormSpell ) ) )
                {
                    m.SendMessage( "Well done. Now you can run if you would..." );
                    m.Send( SpeedControl.Disable );
                }
            }

            base.OnExit( m );
        }

        public override void OnEnter( Mobile m )
        {
            if( Controller.ShowEnterMessage )
                m.SendMessage( "You have entered {0}", Name );

            if( Controller.CannotRun )
            {
                if( m.NetState != null && !TransformationSpellHelper.UnderTransformation( m, typeof( AnimalForm ) ) &&
                    m.AccessLevel < AccessLevel.GameMaster )
                {
                    m.SendMessage( "You cannot run here..." );
                    m.Send( SpeedControl.WalkSpeed );
                }
            }

            if( Controller.CannotMount )
            {
                m.SendMessage( "May be dungerous mounting here..." );
                Dismount( m );
            }

            base.OnEnter( m );
        }

        public override void OnLocationChanged( Mobile m, Point3D oldLocation )
        {
            base.OnLocationChanged( m, oldLocation );

            if( Controller.CannotMount )
            {
                Dismount( m );
            }
        }

        public override bool OnMoveInto( Mobile m, Direction d, Point3D newLocation, Point3D oldLocation )
        {
            if( !Controller.CanEnter && !Contains( oldLocation ) )
            {
                m.SendMessage( "You cannot enter this area." );
                return false;
            }

            if( Controller.CheckCombat && SpellHelper.CheckCombat( m ) && !Contains( oldLocation ) )
            {
                m.SendMessage( "You cannot enter while fighting." );
                return false;
            }

            return true;
        }

        public override TimeSpan GetLogoutDelay( Mobile m )
        {
            if( m.AccessLevel == AccessLevel.Player )
                return Controller.PlayerLogoutDelay;

            return base.GetLogoutDelay( m );
        }

        public override bool OnDoubleClick( Mobile m, object o )
        {
            if( o is BasePotion && !Controller.CanUsePotions )
            {
                m.SendMessage( "You cannot drink potions here." );
                return false;
            }

            if( o is Corpse )
            {
                Corpse c = (Corpse)o;

                bool canLoot;

                if( c.Owner == m )
                    canLoot = Controller.CanLootOwnCorpse;
                else if( c.Owner is PlayerMobile )
                    canLoot = Controller.CanLootPlayerCorpse;
                else
                    canLoot = Controller.CanLootNPCCorpse;

                if( !canLoot )
                    m.SendMessage( "You cannot loot that corpse here." );

                if( m.AccessLevel >= AccessLevel.GameMaster && !canLoot )
                {
                    m.SendMessage( "This is unlootable but you are able to open that with your Godly powers." );
                    return true;
                }

                return canLoot;
            }

            return base.OnDoubleClick( m, o );
        }

        public override void AlterLightLevel( Mobile m, ref int global, ref int personal )
        {
            if( Controller.LightLevel >= 0 )
                global = Controller.LightLevel;
            else
                base.AlterLightLevel( m, ref global, ref personal );
        }

        private static void Dismount( Mobile m )
        {
            if( m.NetState != null && m.Mount != null )
            {
                IMount mount = m.Mount;

                if( mount != null )
                {
                    m.SendMessage( "You have to dismount! NOW!" );
                    mount.Rider = null;
                }
            }
        }
    }
}