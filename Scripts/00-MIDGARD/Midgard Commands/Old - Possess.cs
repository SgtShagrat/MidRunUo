/***************************************************************************
 *                               Old - Possess.cs
 *
 *   begin                : 30 settembre 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Reflection;

using Server;
using Server.Commands;
using Server.Mobiles;
using Server.Targeting;

namespace Midgard.Commands
{
    public class Possess
    {
        #region tables
        private static readonly Layer[] ItemLayers = {Layer.FirstValid, Layer.TwoHanded, Layer.Shoes, Layer.Pants,
                                                      Layer.Shirt, Layer.Helm, Layer.Gloves, Layer.Ring, Layer.Neck, Layer.Hair,
                                                      Layer.Waist, Layer.InnerTorso, Layer.Bracelet, Layer.FacialHair, Layer.MiddleTorso,
                                                      Layer.Earrings, Layer.Arms, Layer.Cloak, Layer.OuterTorso, Layer.OuterLegs, Layer.Mount };


        private static readonly string[] PropsToChange = {
                                                             "RawDex",
                                                             "RawInt",
                                                             "RawStr",
                                                             "Body",
                                                             "BodyMod",
                                                             "BodyValue",
                                                             "Criminal",
                                                             "Direction",
                                                             "DisplayGuildTitle",
                                                             "EmoteHue",
                                                             "FacialHairHue",
                                                             "FacialHairItemID",
                                                             "Fame",
                                                             "Female",
                                                             "Guild",
                                                             "GuildFealty",
                                                             "GuildTitle",
                                                             "HairItemID",
                                                             "HairHue",
                                                             "Hits",
                                                             "Hue",
                                                             "Karma",
                                                             "Kills",
                                                             "MagicDamageAbsorb",
                                                             "Mana",
                                                             "MeleeDamageAbsorb",
                                                             "Name",
                                                             "NameHue",
                                                             "NameMod",
                                                             "Paralyzed",
                                                             "Poison",
                                                             "ShortTermMurders",
                                                             "SpeechHue",
                                                             "Stam",
                                                             "Title",
                                                             "VirtualArmor",
                                                             "VirtualArmorMod",
                                                             "Warmode",
                                                             "WhisperHue",
                                                             "YellHue"
                                                         };
        #endregion

        public static void Initialize()
        {
            CommandSystem.Register( "Possess", AccessLevel.Counselor, new CommandEventHandler( Possess_OnCommand ) );
            CommandSystem.Register( "Unpossess", AccessLevel.Counselor, new CommandEventHandler( UnPossess_OnCommand ) );
        }

        [Usage( "Possess" )]
        [Description( "Allows staff member to 'possess' any NPC mobile." )]
        private static void Possess_OnCommand( CommandEventArgs e )
        {
            e.Mobile.BeginTarget( 16, false, TargetFlags.None, new TargetCallback( PossessOnTarget ) );
            e.Mobile.SendMessage( "Choose a valid NPC you would possess!" );
        }

        [Usage( "Unpossess" )]
        [Description( "Reverses efect of previous possess command." )]
        private static void UnPossess_OnCommand( CommandEventArgs e )
        {
            Midgard2PlayerMobile from = (Midgard2PlayerMobile)e.Mobile;
            if( from.PossessStorageMob == null )
            {
                from.SendMessage( "You are not currently possessing any mobile." );
                return;
            }

            Mobile storageMob = from.PossessStorageMob;
            Mobile possessed = from.PossessMob;

            MoveItems( from, possessed );

            possessed.Location = from.Location;
            possessed.Direction = from.Direction;
            possessed.Map = from.Map;
            possessed.Frozen = false;

            Copy( storageMob, from );

            storageMob.Delete();

            from.PossessMob = null;
            from.PossessStorageMob = null;
            from.Hidden = true;
        }

        private static void PossessOnTarget( Mobile owner, object o )
        {
            Midgard2PlayerMobile from = owner as Midgard2PlayerMobile;
            if( from == null )
                return;

            if( !( o is Mobile ) )
            {
                from.SendMessage( "That is not a valid NPC." );
                return;
            }

            Mobile m = (Mobile)o;
            if( m.Player )
            {
                from.SendMessage( "You cannot possess other players!" );
                return;
            }

            if( from.PossessMob != null )
            {
                // unpossess first
                Mobile storageMob1 = from.PossessStorageMob;
                Mobile dummy = from.PossessMob;
                MoveItems( from, dummy );
                dummy.Location = from.Location;
                dummy.Direction = from.Direction;
                dummy.Map = from.Map;
                dummy.Frozen = false;

                Copy( storageMob1, from );

                storageMob1.Delete();

                from.PossessMob = null;
                from.PossessStorageMob = null;
                from.Hidden = true;
            }

            // create internal mobile that will store our changed props etc
            Mobile storageMob = new Mobile();

            from.PossessMob = m;
            from.PossessStorageMob = storageMob;

            Copy( from, storageMob );

            Copy( m, from );

            from.Location = m.Location;

            // Move target to internal map
            m.Frozen = true;
            m.Map = Map.Internal;

            from.Hidden = false;
        }

        private static readonly Type m_TypeOfMobile = typeof( Mobile );

        private static void CopyProps( Mobile from, Mobile to )
        {
            try
            {
                foreach( string t in PropsToChange )
                {
                    PropertyInfo pi = m_TypeOfMobile.GetProperty( t, BindingFlags.GetField | BindingFlags.Instance | BindingFlags.Public );
                    pi.SetValue( to, pi.GetValue( from, null ), null );
                }
            }
            catch( Exception e )
            {
                Console.WriteLine( "Possess: CopyProps Exception: {0}", e.Message );
            }
        }

        private static void CopySkills( Mobile from, Mobile to )
        {
            try
            {
                for( int i = 0; i < from.Skills.Length; i++ )
                    to.Skills[ i ].Base = from.Skills[ i ].Base;
            }
            catch( Exception e )
            {
                Console.WriteLine( "Possess: CopySkills Exception: {0}", e.Message );
            }
        }

        private static void MoveItems( Mobile from, Mobile to )
        {
            if( from == null || from.Items == null )
                return;

            List<Item> possessItems = new List<Item>( from.Items );
            try
            {
                foreach( Item item in possessItems )
                {
                    if( Array.IndexOf( ItemLayers, item.Layer ) != -1 )
                        to.EquipItem( item );
                }
            }
            catch( Exception e )
            {
                Console.WriteLine( "Possess: MoveItems Exception: {0}", e.Message );
            }
        }

        private static void Copy( Mobile from, Mobile to )
        {
            CopyProps( from, to );
            CopySkills( from, to );
            MoveItems( from, to );
        }
    }
}
