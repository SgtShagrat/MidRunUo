/***************************************************************************
 *                                  HitchingPostAddon.cs
 *                            		--------------------
 *  begin                	: Marzo, 2008
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Palo per legare gli animali.
 * 
 ***************************************************************************/

using System;
using System.IO;

using Midgard.Engines.PetSystem;

using Server;
using Server.Items;
using Server.Mobiles;
using Server.Multis;
using Server.Spells;
using Server.Targeting;

namespace Midgard.Items
{
    [Flipable( 0x14E8, 0x14E7 )]
    public class HitchingPostComponent : AddonComponent
    {
        public static readonly int MaxHitchPerHouse = 2;

        public HitchingPostComponent()
            : this( 0x14E7 )
        {
            IsPublic = false;
        }

        public HitchingPostComponent( int itemID )
            : base( itemID )
        {
        }

        public HitchingPostComponent( Serial serial )
            : base( serial )
        {
        }

        public override bool ForceShowProperties
        {
            get { return ObjectPropertyList.Enabled; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public BaseCreature Hitched { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile HitchOwner { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool IsPublic { get; set; }

        public override void AddNameProperties( ObjectPropertyList list )
        {
            base.AddNameProperties( list );

            if( Hitched != null && !String.IsNullOrEmpty( Hitched.Name ) )
            {
                list.Add( 1060658, "Hitched\t{0}", Hitched.Name );

                Mobile master = HitchOwner;

                if( master != null && !String.IsNullOrEmpty( master.Name ) )
                    list.Add( 1060659, "Property of\t{0}", master.Name );
            }
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( !from.CheckAlive() )
                return;

            if( from.InRange( GetWorldLocation(), 2 ) )
            {
                BaseHouse house = BaseHouse.FindHouseAt( from );

                if( IsPublic )
                {
                    if( Hitched == null )
                    {
                        from.SendMessage( "Target the pet you want to hitch." );
                        from.BeginTarget( 3, true, TargetFlags.None, new TargetCallback( Hitch_OnTarget ) );
                    }
                    else
                    {
                        if( HitchOwner == from )
                        {
                            from.SendMessage( "Target the pet you want to release from hitching post." );
                            from.BeginTarget( 3, true, TargetFlags.None, new TargetCallback( UnHitch_OnTarget ) );
                        }
                    }
                }
                else if( house != null )
                {
                    if( Hitched == null )
                    {
                        if( house.IsOwner( from ) || house.IsCoOwner( from ) || house.IsFriend( from ) )
                        {
                            from.SendMessage( "Target the pet you want to hitch." );
                            from.BeginTarget( 3, true, TargetFlags.None, new TargetCallback( Hitch_OnTarget ) );
                        }
                        else
                            from.SendMessage( "You must own this house or be a friend's owner in order to hitch pet." );
                    }
                    else if( ( HitchOwner == from || ( house.IsOwner( from ) ) ) )
                    {
                        from.SendMessage( "Target the pet you want to release from hitching post." );
                        from.BeginTarget( 3, true, TargetFlags.None, new TargetCallback( UnHitch_OnTarget ) );
                    }
                    else
                        from.SendMessage( "You must own this house or be a friend's owner in order to release hitched pet." );
                }
                else
                {
                    from.SendMessage( "This hitching post is broken." );
                    if( Core.Debug || from.AccessLevel > AccessLevel.Player )
                        from.SendMessage( "DEBUG: item error! This post must be setted as PUBLIC or PUT into a house!" );
                }
            }
            else
                from.SendLocalizedMessage( 500486 ); //That is too far away.
        }

        public virtual void UnHitch_OnTarget( Mobile from, object targeted )
        {
            if( from == null || targeted == null )
                return;

            if( !from.InRange( GetWorldLocation(), 2 ) )
            {
                from.SendMessage( "You are too far away from the hitching post." );
                return;
            }

            BaseCreature pet = targeted as BaseCreature;

            if( pet == null )
                from.SendMessage( "That is not your pet." );
            else if( !from.CheckAlive() )
                from.SendMessage( "You cannot do anything while dead." );
            else if( SpellHelper.CheckCombat( from ) )
                from.SendMessage( "You cannot hitch your pet while your fighting." );
            else if( !pet.InRange( GetWorldLocation(), 2 ) )
                from.SendMessage( "Your creature is too far away from the hitching post." );
            else if( HitchOwner == from && pet == Hitched )
            {
                DoUnHitch( this, from, pet );
                from.SendMessage( "You have successfully reclaimed your friend." );
            }
        }

        public static void DoUnHitch( HitchingPostComponent addon, Mobile owner, BaseCreature pet )
        {
            if( pet == null || addon == null )
                return;

            pet.IsHitched = false;

            addon.Hitched = null;
            addon.HitchOwner = null;

            pet.SetControlMaster( owner );

            pet.CantWalk = false;
            pet.IsStabled = false;

            DoHitchLog( String.Format( "UNHITCH >>> PetName: {0} - Serial {1} - Controller {2} - Account {3} - DateTime {4}.",
                                     String.IsNullOrEmpty( pet.Name ) ? pet.GetType().Name : pet.Name, pet.Serial,
                                     String.IsNullOrEmpty( owner.Name ) ? owner.Serial.ToString() : owner.Name, owner.Account,
                                     DateTime.Now ), "Logs/HitchLog.log" );
        }

        public virtual void Hitch_OnTarget( Mobile from, object targeted )
        {
            if( from == null || targeted == null )
                return;

            if( !from.InRange( GetWorldLocation(), 2 ) )
            {
                from.SendMessage( "You are too far away from the hitching post." );
                return;
            }

            BaseCreature pet = targeted as BaseCreature;

            if( pet == null )
                from.SendMessage( "You can choose only creatures." );
            else if( !from.CheckAlive() )
                from.SendMessage( "You cannot do anything while dead." );
            else if( !pet.InRange( GetWorldLocation(), 2 ) )
                from.SendMessage( "Your creature is too far away from the hitching post." );
            else if( SpellHelper.CheckCombat( from ) )
                from.SendMessage( "You cannot do that while fighting." );
            else if( pet.Body.IsHuman )
                from.SendMessage( "That person gives you a dirty look." );
            else if( pet.ControlMaster != from )
                from.SendMessage( "This is not your pet." );
            else if( PetUtility.IsPackAnimal( pet ) && ( pet.Backpack != null && pet.Backpack.Items.Count > 0 ) )
                from.SendMessage( "You must unload your pets backpack first." );
            else if( pet.IsDeadPet )
                from.SendMessage( "You cannot hitch the dead." );
            else if( pet.Summoned )
                from.SendMessage( "You cannot shrink a summoned creature." );
            else if( pet.Combatant != null && pet.InRange( pet.Combatant, 12 ) && pet.Map == pet.Combatant.Map )
                from.SendMessage( "Your pet is fighting so you cannot hitch it yet." );
            else if( pet.BodyMod != 0 )
                from.SendMessage( "You cannot hitch your pet while it's polymorphed." );
            else if( pet.Controlled && pet.ControlMaster == from )
            {
                DoHitch( this, from, pet );
                from.SendMessage( "You have successfully hitched your friend." );
            }
        }

        public static void DoHitch( HitchingPostComponent addon, Mobile owner, BaseCreature pet )
        {
            if( pet == null || addon == null )
                return;

            pet.IsHitched = true;
            pet.HitchingPost = addon;

            addon.Hitched = pet;
            addon.HitchOwner = owner;

            pet.SetControlMaster( null );

            pet.CantWalk = true;
            pet.IsStabled = true;

            DoHitchLog( String.Format( "HITCH >>> PetName: {0} - Serial {1} - Controller {2} - Account {3} - DateTime {4}.",
                                     String.IsNullOrEmpty( pet.Name ) ? pet.GetType().Name : pet.Name, pet.Serial,
                                     String.IsNullOrEmpty( owner.Name ) ? owner.Serial.ToString() : owner.Name, owner.Account,
                                     DateTime.Now ), "Logs/HitchLog.log" );
        }

        public override void OnChop( Mobile from )
        {
            if( IsPublic && from.AccessLevel == AccessLevel.Player )
                from.SendMessage( "You cannot redeed this hitching post." );

            if( Hitched != null )
                from.SendMessage( "You cannot redeed this hitching post while a pet is hitchet." );
            else
                base.OnChop( from );
        }

        public static bool CheckHouseHitch( BaseHouse house )
        {
            if( house == null || house.Deleted || house.Map == null )
                return false;

            int numHitches = 0;
            foreach( Item item in house.Addons )
            {
                if( item is HitchingPostAddon && house.Contains( item ) )
                    numHitches++;
            }

            return numHitches < MaxHitchPerHouse;
        }

        public static void DoHitchLog( string toLog, string path )
        {
            try
            {
                TextWriter tw = File.AppendText( path );
                tw.WriteLine( toLog );
                tw.Close();
            }
            catch
            {
            }
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 1 ); // version

            // Versione 1
            writer.Write( IsPublic );

            // Versione 0
            writer.Write( Hitched );
            writer.Write( HitchOwner );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            switch( version )
            {
                case 1:
                    IsPublic = reader.ReadBool();
                    goto case 0;
                case 0:
                    Hitched = (BaseCreature)reader.ReadMobile();
                    HitchOwner = reader.ReadMobile();
                    break;
            }
        }
    }

    public class HitchingPostAddon : BaseAddon
    {
        [Constructable]
        public HitchingPostAddon()
            : this( 0 )
        {
        }

        [Constructable]
        public HitchingPostAddon( int hue )
        {
            AddComponent( new HitchingPostComponent( 0x14E7 ), 0, 0, 0 );
            Hue = hue;
        }

        public HitchingPostAddon( Serial serial )
            : base( serial )
        {
        }

        public override BaseAddonDeed Deed
        {
            get { return new HitchingPostDeed(); }
        }

        public override bool RetainDeedHue
        {
            get { return true; }
        }

        public BaseCreature HitchedCreature
        {
            get
            {
                HitchingPostComponent comp = Components[ 0 ] as HitchingPostComponent;

                if( comp != null && !comp.Deleted )
                    return comp.Hitched;

                return null;
            }
        }

        public override AddonFitResult CouldFit( IPoint3D p, Map map, Mobile from, ref BaseHouse house )
        {
            // NB: deve essere fatto PRIMA di CheckHouseHitch perche' se no house e' null
            AddonFitResult result = base.CouldFit( p, map, from, ref house );

            if( !HitchingPostComponent.CheckHouseHitch( house ) )
            {
                from.SendMessage( "You cannot place another hitching post in this house." );
                return AddonFitResult.Blocked;
            }

            return result;
        }

        public override void Delete()
        {
            HitchingPostComponent comp = Components[ 0 ] as HitchingPostComponent;

            if( comp != null && !comp.Deleted )
            {
                BaseCreature pet = comp.Hitched;
                Mobile master = comp.HitchOwner;

                if( pet != null && master != null )
                {
                    ShrinkItem si = new ShrinkItem();
                    si.MobType = pet.GetType();
                    si.Pet = pet;
                    si.PetOwner = master;

                    if( pet is BaseMount )
                        si.MountID = ( (BaseMount)pet ).ItemID;

                    pet.IsShrinked = false;
                    pet.Controlled = true;
                    pet.ControlMaster = null;
                    pet.Internalize();

                    pet.OwnerAbandonTime = DateTime.MinValue;
                    pet.IsStabled = true;

                    master.BankBox.DropItem( si );

                    HitchingPostComponent.DoHitchLog( String.Format( "DELETE >>> PetName: {0} - Serial {1} - Controller {2} - Account {3} - DateTime {4}.",
                                                                   String.IsNullOrEmpty( pet.Name ) ? pet.GetType().Name : pet.Name, pet.Serial,
                                                                   String.IsNullOrEmpty( master.Name ) ? master.Serial.ToString() : master.Name, master.Account,
                                                                   DateTime.Now ), "Logs/HitchLog.log" );
                }
            }

            base.Delete();
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
    }

    public class HitchingPostDeed : BaseAddonDeed
    {
        [Constructable]
        public HitchingPostDeed()
        {
            Name = "Hitching Post Deed";
        }

        public HitchingPostDeed( Serial serial )
            : base( serial )
        {
        }

        public override BaseAddon Addon
        {
            get { return new HitchingPostAddon( Hue ); }
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
    }
}