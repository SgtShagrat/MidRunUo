/***************************************************************************
 *                                     Kerberos.cs
 *                            		-------------------
 *  begin                	: Gennaio, 2007
 *  version					: 1.0
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 * 
 * 	Info
 *			Carnefice e sorvegliante dei lavori forzati.
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Server;
using Server.Items;
using Server.ContextMenus;
using Server.Mobiles;
using Server.Network;

namespace Midgard.Engines.HardLabour
{
    [CorpseName( "kerberos corpse" )]
    public class Kerberos : BaseCreature
    {
        private static readonly string PickaxeString = "Please Master give me a pickaxe";
        private static readonly string ResString = "Please Master Ress Me";
        private static readonly string HelpString = "Please Master Help Me";
        private static readonly string SlopString = "Please Master Give Me Something To Eat";

        [Constructable]
        public Kerberos()
            : base( AIType.AI_Melee, FightMode.Aggressor, 22, 1, 0.2, 1.0 )
        {
            // Stats
            SetStr( 3900, 4000 );
            SetDex( 3900, 4000 );
            SetInt( 3150, 3250 );

            SetHits( 66, 125 );
            SetDamage( 30, 50 );

            CantWalk = true;
            CanHearGhosts = true;
            Blessed = true;

            // Resistances
            SetResistance( ResistanceType.Physical, 40, 50 );
            SetResistance( ResistanceType.Fire, 40, 50 );
            SetResistance( ResistanceType.Cold, 40, 50 );
            SetResistance( ResistanceType.Poison, 40, 50 );
            SetResistance( ResistanceType.Energy, 40, 50 );

            // Skills
            SetSkill( SkillName.Macing, 85.0, 105.5 );
            SetSkill( SkillName.MagicResist, 165.0, 195.5 );
            SetSkill( SkillName.Swords, 85.0, 105.5 );
            SetSkill( SkillName.Tactics, 115.0, 119.5 );
            SetSkill( SkillName.Wrestling, 190, 200 );
            SetSkill( SkillName.Parry, 1050.0, 1100.0 );

            // Fama e Karma
            Fame = 5000;
            Karma = -10000;

            // Hair
            HairItemID = 0x203B;

            // Speech & Skin hues
            SpeechHue = Utility.RandomDyedHue();
            Hue = Utility.RandomSkinHue();

            VirtualArmor = 70;

            // Nome, Sesso, Titolo  e BodyValue
            Name = "Kerberos";
            Title = ", the Midgard Jailor";
            Female = false;
            Body = 400;

            // Equip:
            // Cloak
            Cloak cloak = new Cloak( 1172 );
            cloak.LootType = LootType.Blessed;
            cloak.Movable = false;
            AddItem( cloak );

            // Boots
            Boots boots = new Boots( 1 );
            boots.Movable = false;
            AddItem( boots );

            // SurCoat
            Surcoat surcoat = new Surcoat( 1172 );
            surcoat.Movable = false;
            AddItem( surcoat );

            // Sleeves Leather
            LeatherArms sleeves = new LeatherArms();
            sleeves.Movable = false;
            sleeves.Hue = 2406;
            AddItem( sleeves );

            // Leggins Leather
            LeatherLegs legs = new LeatherLegs();
            legs.Movable = false;
            legs.Hue = 2406;
            AddItem( legs );

            // Gorget
            LeatherGorget gorget = new LeatherGorget();
            gorget.Movable = false;
            gorget.Hue = 2406;
            AddItem( gorget );

            // Gloves
            LeatherGloves gloves = new LeatherGloves();
            gloves.Movable = false;
            gloves.Hue = 2406;
            AddItem( gloves );

            // Tunic
            LeatherBustierArms tunic = new LeatherBustierArms();
            tunic.Movable = false;
            tunic.Hue = 2406;
            AddItem( tunic );

            // Cappello
            TricorneHat hat = new TricorneHat( 1172 );
            hat.Movable = false;
            hat.Hue = 2406;
            AddItem( hat );

            // Capelli
            HairItemID = Utility.RandomList( 0x203B, 0x2049, 0x2048, 0x203C );

            AddItem( new KerberosWhip() );
        }

        public override bool HandlesOnSpeech( Mobile from )
        {
            if( from.InRange( Location, 12 ) )
            {
                return true;
            }

            return base.HandlesOnSpeech( from );
        }

        public override void OnSpeech( SpeechEventArgs e )
        {
            if( !e.Handled && e.Mobile.InRange( this.Location, 12 ) )
            {
                Midgard2PlayerMobile m2pm = e.Mobile as Midgard2PlayerMobile;
                if( m2pm == null )
                    return;

                if( Insensitive.Equals( e.Speech, PickaxeString ) )
                {
                    bool lamer = false;

                    // Controllo che non abbia un picconi nello zaino e in mano
                    Container pack = m2pm.Backpack;
                    Item[] lamerPicks;
                    if( pack != null )
                    {
                        lamerPicks = pack.FindItemsByType( typeof( SlavePickaxe ), true );
                        if( lamerPicks.Length > 0 )
                            lamer = true;
                    }

                    List<Item> items = new List<Item>( m2pm.Items );
                    foreach( Item i in items )
                    {
                        if( i is SlavePickaxe )
                        {
                            lamer = true;
                            break;
                        }
                    }

                    if( lamer )
                    {
                        Say( "Go to Hell, Slave! Your Pickaxe is not ruined. Your Bullshit will be punished!" );
                        m2pm.Emote( "A Whiplash from {0} hit you", Name );
                        if( m2pm.Alive )
                            Server.Spells.SpellHelper.Damage( TimeSpan.FromSeconds( 0.5 ), m2pm, m2pm,
                                                              Utility.RandomMinMax( 50, 80 ), 0, 100, 0, 0, 0 );
                    }
                    else
                    {
                        Say( "Take this tool, Slave!" );
                        m2pm.AddToBackpack( new SlavePickaxe() );
                    }

                    e.Handled = true;
                }
                else if( Insensitive.Equals( e.Speech,  ResString.ToLower() ) )
                {
                    if( m2pm.Alive == false )
                    {
                        Say( "Come Back to Life. There is a lot of Work for you! Mhauauauau" );
                        m2pm.PlaySound( 0x214 );
                        m2pm.FixedEffect( 0x376A, 10, 16 );
                        m2pm.Resurrect();
                    }
                }
                else if( Insensitive.Equals( e.Speech, HelpString ) )
                {
                    m2pm.SendGump( new HardLabourNotifierGump( m2pm.Minerals2Mine ) );
                    e.Handled = true;
                }
                if( Insensitive.Equals( e.Speech, SlopString ) )
                {
                    bool isLamer = false;

                    // Controllo che non abbia cibo nello zaino
                    Container pack = m2pm.Backpack;
                    if( pack != null )
                    {
                        List<Item> lamerSlop = new List<Item>( pack.FindItemsByType( typeof( StaleBreadLoaf ), true ) );
                        isLamer = ( lamerSlop.Count > 0 ) ? true : false;
                    }

                    if( isLamer )
                    {
                        Say( "Go Away, Slave! Eat your slop before bothering me!" );
                        m2pm.Emote( "A Whiplash from {0} hit you", Name );
                        if( m2pm.Alive )
                        {
                            Server.Spells.SpellHelper.Damage( TimeSpan.FromSeconds( 0.5 ), m2pm, m2pm, Utility.RandomMinMax( 50, 80 ), 0, 100, 0, 0, 0 );
                        }
                    }
                    else
                    {
                        Say( "This Good Slop is for You, Slave! BUAHAHHAH" );
                        m2pm.AddToBackpack( new StaleBreadLoaf( 12 ) );
                        m2pm.AddToBackpack( new SpoiledMeat( 8 ) );
                    }

                    e.Handled = true;
                }
            }

            base.OnSpeech( e );
        }

        public override bool OnDragDrop( Mobile from, Item dropped )
        {
            Midgard2PlayerMobile m2pm = from as Midgard2PlayerMobile;

            if( m2pm != null )
            {
                if( dropped is IronOre )
                {
                    IronOre iron = (IronOre)dropped;

                    if( iron.Amount < m2pm.Minerals2Mine )
                    {
                        m2pm.Minerals2Mine -= iron.Amount;
                        iron.Delete();
                        Say( "Slave if you want to be free, you need {0} more ore!", m2pm.Minerals2Mine.ToString() );
                        m2pm.Emote( "Kerberos spit on your face!" );
                        return false;
                    }
                    else
                    {
                        Say( "{0}, your Guilt has been expired...", m2pm.Name );
                        m2pm.SendMessage( "Your Guilt has been expired..." );
                        m2pm.Minerals2Mine = 0;
                        m2pm.HardLabourInfo = string.Empty;
                        m2pm.HardLabourCondamner = string.Empty;
                        iron.Delete();

                        // Deleto tutti i picconi negli items
                        SlavePickaxe sp;
                        List<Item> items = new List<Item>( m2pm.Items );
                        for( int i = 0; i < items.Count; i++ )
                        {
                            if( items[ i ] is SlavePickaxe )
                            {
                                sp = items[ i ] as SlavePickaxe;
                                if( sp != null )
                                {
                                    items[ i ].Delete();
                                }
                            }
                        }

                        List<Item> packitems = new List<Item>( m2pm.Backpack.Items );
                        for( int i = 0; i < packitems.Count; i++ )
                        {
                            if( packitems[ i ] is SlavePickaxe )
                            {
                                sp = packitems[ i ] as SlavePickaxe;
                                if( sp != null )
                                {
                                    packitems[ i ].Delete();
                                }
                            }
                        }

                        // Lo mando a un Moongate con la divisa da galeotto
                        m2pm.Map = Map.Felucca;
                        switch( Utility.Random( 8 ) )
                        {
                            case 0:
                                m2pm.Location = ( new Point3D( 2708, 693, 0 ) );
                                break;
                            case 1:
                                m2pm.Location = ( new Point3D( 4476, 1281, 0 ) );
                                break;
                            case 2:
                                m2pm.Location = ( new Point3D( 1344, 1994, 0 ) );
                                break;
                            case 3:
                                m2pm.Location = ( new Point3D( 1507, 3769, 0 ) );
                                break;
                            case 4:
                                m2pm.Location = ( new Point3D( 780, 754, 0 ) );
                                break;
                            case 5:
                                m2pm.Location = ( new Point3D( 1833, 2942, -22 ) );
                                break;
                            case 6:
                                m2pm.Location = ( new Point3D( 651, 2066, 0 ) );
                                break;
                            case 7:
                                m2pm.Location = ( new Point3D( 3556, 2150, 26 ) );
                                break;
                        }
                        return false;
                    }
                }
            }

            return base.OnDragDrop( from, dropped );
        }

        public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
        {
            base.GetContextMenuEntries( from, list );

            list.Add( new HardLabourHelpEntry( from, true ) );
        }

        #region serial-deserial
        public Kerberos( Serial serial )
            : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 ); // version
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
        #endregion

        private class HardLabourHelpEntry : ContextMenuEntry
        {
            private Mobile m_Requester;

            public HardLabourHelpEntry( Mobile m, bool enabled )
                : base( 1055, 3 ) // Master help me
            {
                m_Requester = m;
                if( !enabled )
                    Flags |= CMEFlags.Disabled;
            }

            public override void OnClick()
            {
                Midgard2PlayerMobile m2pm = m_Requester as Midgard2PlayerMobile;
                if( m2pm != null )
                {
                    m2pm.CloseGump( typeof( HardLabourNotifierGump ) );
                    m2pm.SendGump( new HardLabourNotifierGump( m2pm.Minerals2Mine ) );
                }
            }
        }
    }
}