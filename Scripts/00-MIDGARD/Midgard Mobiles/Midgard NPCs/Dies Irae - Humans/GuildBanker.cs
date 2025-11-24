/***************************************************************************
 *                                  GuildBanker.cs
 *                            		--------------
 *  begin                	: Novembre, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info
 * 
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Guilds;
using Server.Items;

namespace Server.Mobiles
{
    public class GuildBanker : Banker
    {
        public override bool NoHouseRestrictions { get { return true; } }
        public override bool IsActiveVendor { get { return false; } }
        public override NpcGuild NpcGuild { get { return NpcGuild.None; } }

        [CommandProperty( AccessLevel.Seer )]
        public string BankerGuildName
        {
            get
            {
                if( Guild != null && !Guild.Disbanded )
                {
                    if( !String.IsNullOrEmpty( Guild.Name ) )
                        return Guild.Name;
                    else
                        return "No Name";
                }
                else
                    return "No Guild";
            }
            set
            {
                BaseGuild newGuild = BaseGuild.FindByName( value );

                if( newGuild != null && !newGuild.Disbanded && newGuild != Guild )
                    Guild = newGuild;
                else
                    Say( true, "Hey, That is not a valid guild." );
            }
        }

        [Constructable]
        public GuildBanker()
        {
            Title = null;
        }

        public GuildBanker( Serial serial )
            : base( serial )
        {
        }

        public virtual void UpdateGuidClothing()
        {
            if( Backpack == null )
                return;

            if( Guild == null || Guild.Disbanded )
                return;

            Item item = FindItemOnLayer( Layer.Shoes );

            if( item != null )
                item.Delete();

            // remove robe
            item = FindItemOnLayer( Layer.OuterTorso );

            if( item != null )
                item.Delete();

            // add uniform to pack if existant
            Guild g = Guild as Guild;
            if( g != null && !g.Disbanded )
                Midgard.Misc.GuildsHelper.AddUniformToPack( this, g );

            for( int i = 0; i < Backpack.Items.Count; i++ )
            {
                try
                {
                    item = Backpack.Items[ i ];
                    if( item != null && !item.Deleted && item.Movable )
                        AddItem( item );
                }
                catch( Exception ex )
                {
                    Console.WriteLine( ex.ToString() );
                }
            }

            // if no shoes are equipped, re-equip sandals
            item = FindItemOnLayer( Layer.Shoes );

            if( item == null )
                AddItem( new Sandals( GetShoeHue() ) );
        }

        public override bool CheckMidgardTown()
        {
            return false;
            // don't make any town cloth
        }

        public override void OnGuildChange( BaseGuild oldGuild )
        {
            if( Guild == null || Guild.Disbanded )
                return;

            if( !Deleted )
            {
                UpdateGuidClothing();

                Say( true, String.Format( "Yessir, I'll serve {0} guild", ( String.IsNullOrEmpty( Guild.Name ) ? "a" : Guild.Name ) ) );
            }
        }

        public override void OnSpeech( SpeechEventArgs e )
        {
            if( !e.Handled && e.Mobile != null && e.Mobile.InRange( Location, 12 ) && e.Mobile.InLOS( this ) )
            {
                bool bankerSpeech = false;
                for( int i = 0; i < e.Keywords.Length && !bankerSpeech; ++i )
                {
                    int key = e.Keywords[ i ];
                    if( key == 0x0000 || key == 0x0001 || key == 0x0002 || key == 0x0003 )
                        bankerSpeech = true;
                }

                if( e.Mobile.AccessLevel == AccessLevel.Player && bankerSpeech && Guild != null && ( e.Mobile.Guild == null || e.Mobile.Guild != Guild ) )
                {
                    Say( true, String.Format( "I'm sorry. I'll serve only guild members of {0} guild.",
                                            ( String.IsNullOrEmpty( Guild.Name ) ? "my" : Guild.Name ) ) );
                    e.Handled = true;
                }
            }

            base.OnSpeech( e );
        }

        public override void InitOutfit()
        {
            Utility.AssignRandomHair( this, HairHue );
            Utility.AssignRandomFacialHair( this, HairHue );

            AddItem( new Robe( GetRandomHue() ) );
            AddItem( new Sandals( GetShoeHue() ) );

            Container pack = Backpack;

            if( pack == null )
            {
                pack = new Backpack();
                pack.Movable = false;
                AddItem( pack );
            }
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            if( Guild != null && !Guild.Disbanded )
            {
                if( String.IsNullOrEmpty( Guild.Name ) )
                    list.Add( "guild banker" );
                else
                    list.Add( String.Format( "guild banker for {0} guild", Guild.Name ) );
            }
        }

        public override void AddCustomContextEntries( Mobile from, List<ContextMenuEntry> list )
        {
            if( from.Alive && from.InLOS( this ) && from.Guild != null && from.Guild == Guild )
                list.Add( new OpenGuildBankEntry( from, this ) );

            base.AddCustomContextEntries( from, list );
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
    }

    public class OpenGuildBankEntry : ContextMenuEntry
    {
        private Mobile m_Banker;

        public OpenGuildBankEntry( Mobile from, Mobile banker )
            : base( 6105, 12 )
        {
            m_Banker = banker;
        }

        public override void OnClick()
        {
            Mobile from = Owner.From;

            if( !from.Alive || !from.InLOS( this ) )
                return;
            else
                from.BankBox.Open();
        }
    }
}
