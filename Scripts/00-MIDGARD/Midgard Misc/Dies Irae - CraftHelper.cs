/***************************************************************************
 *                                  CraftHelper.cs
 *                            		-------------------
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
using System.Collections;
using System.IO;
using System.Text;

using Midgard.Gumps;
using Midgard.Items;
using Midgard.Misc;
using Server;
using Server.Engines.Craft;
using Server.Items;
using System.Collections.Generic;

namespace Midgard
{
    public static class CraftHelper
    {
        public static Item HandeBoardCraft( Type typeRes, CraftSystem craftSystem, CraftItem craftItem )
        {
            Item item = null;

            if( typeRes == null )
                typeRes = craftItem.Resources.GetAt( 0 ).ItemType;

            if( typeRes != null )
            {
                CraftResource res = CraftResources.GetFromType( typeRes );
                item = CreateBoardFromResource( res );
            }

            if( item == null )
                Console.WriteLine( "Warning: null item form HandeBoardCraft" );

            return item;
        }

        public static Item CreateBoardFromResource( CraftResource resource )
        {
            Item item;

            switch( resource )
            {
                case CraftResource.Oak:
                    item = new OakBoard();
                    break;
                case CraftResource.Walnut:
                    item = new WalnutBoard();
                    break;
                case CraftResource.Ohii:
                    item = new OhiiBoard();
                    break;
                case CraftResource.Cedar:
                    item = new CedarBoard();
                    break;
                case CraftResource.Willow:
                    item = new WillowBoard();
                    break;
                case CraftResource.Cypress:
                    item = new CypressBoard();
                    break;
                case CraftResource.Yew:
                    item = new YewBoard();
                    break;
                case CraftResource.Apple:
                    item = new AppleBoard();
                    break;
                case CraftResource.Pear:
                    item = new PearBoard();
                    break;
                case CraftResource.Peach:
                    item = new PeachBoard();
                    break;
                case CraftResource.Banana:
                    item = new BananaBoard();
                    break;
                case CraftResource.Stonewood:
                    item = new StonewoodBoard();
                    break;
                case CraftResource.Silver:
                    item = new SilverBoard();
                    break;
                case CraftResource.Blood:
                    item = new BloodBoard();
                    break;
                case CraftResource.Swamp:
                    item = new SwampBoard();
                    break;
                case CraftResource.Crystal:
                    item = new CrystalBoard();
                    break;
                case CraftResource.Elven:
                    item = new ElvenBoard();
                    break;
                case CraftResource.Elder:
                    item = new ElderBoard();
                    break;
                case CraftResource.Enchanted:
                    item = new EnchantedBoard();
                    break;
                case CraftResource.Death:
                    item = new DeathBoard();
                    break;
                default:
                    item = new Board();
                    break;
            }

            return item;
        }

        public static void HandleWeaponCraft( BaseWeapon weapon, Mobile crafter )
        {
            if( Midgard2Persistance.MidgardAdvancedCraftEnabled )
            {
                if( weapon != null )
                {
                    if( weapon.Quality == WeaponQuality.Exceptional )
                    {
                        if( Core.AOS )
                        {
                            #region bonus Armslore Itemid Black per Armi
                            double chance = ( crafter.Skills[ SkillName.Blacksmith ].Value - 90.0 ) / 100.0;

                            int valMin = 10;
                            int valMax = valMin;

                            valMax += (int)( ( crafter.Skills[ SkillName.Blacksmith ].Value - 100.0 ) / 4.0 );		// 5% da blacksmith a 120
                            valMax += (int)( ( crafter.Skills[ SkillName.ArmsLore ].Value - 50.0 ) / 10.0 );			// 5% da armslore a 100
                            valMax += (int)( ( crafter.Skills[ SkillName.ItemID ].Value - 50.0 ) / 10.0 );			// 5% da itemid a 100
                            valMax = Math.Min( valMax, 20 );													// cap a 20%

                            if( crafter.AccessLevel == AccessLevel.Developer )
                                crafter.SendMessage( "Debug HandleWeaponCraft: valMin {0}, valMax {1}, chance {2}", valMin, valMax, chance.ToString( "F2" ) );

                            if( Utility.RandomDouble() < chance )
                            {
                                int newWeaponSpeed = Utility.RandomMinMax( valMin, valMax );
                                weapon.Attributes.WeaponSpeed = Math.Max( newWeaponSpeed, weapon.Attributes.WeaponSpeed );
                            }
                            #endregion

                            #region legni
                            ////							CraftResource thisResource = CraftResources.GetFromType( resourceType );
                            ////
                            ////							switch ( thisResource )
                            ////							{
                            ////								case CraftResource.Oak:
                            ////								{											
                            ////									if (Utility.Random(100 )>60)
                            ////										weapon.Attributes.ReflectPhysical = Utility.Random( 5 ) + 5;
                            ////									break;
                            ////								}
                            ////								case CraftResource.Walnut:
                            ////								{
                            ////									if (Utility.Random(100 )>60)
                            ////										weapon.Attributes.WeaponSpeed = Utility.Random( 5 ) + 5;
                            ////									break;
                            ////								}
                            ////								case CraftResource.Cedar:
                            ////								{
                            ////									if (Utility.Random(100 )>60)
                            ////										weapon.Attributes.EnhancePotions = Utility.Random( 5 ) + 5;
                            ////									if (weapon.MaxRange>1)
                            ////										weapon.MaxRange+=2;
                            ////									break;
                            ////								}
                            ////								case CraftResource.Willow:
                            ////								{
                            ////									if (weapon.MaxRange>1)
                            ////										weapon.MaxRange-=2;
                            ////									
                            ////									weapon.Attributes.SpellChanneling = 1;
                            ////									
                            ////									if (Utility.Random(100 )>60)
                            ////										weapon.Attributes.LowerRegCost = Utility.Random( 3 ) + 2;
                            ////									break;
                            ////								}
                            ////								case CraftResource.Cypress:
                            ////								{
                            ////									if (weapon.MaxRange>1)
                            ////										weapon.MaxRange+=3;
                            ////									if (Utility.Random(100 )>60)
                            ////										weapon.Attributes.Luck = Utility.Random( 70 ) + 30;
                            ////									break;
                            ////								}
                            ////								case CraftResource.Ohii:
                            ////								{				
                            ////									if (weapon.MaxRange>1)
                            ////										weapon.MaxRange-=1;
                            ////																
                            ////									if (Utility.Random(100 )>50)
                            ////										weapon.Attributes.WeaponSpeed = Utility.Random( 8 ) + 10;
                            ////									break;
                            ////								}
                            ////								case CraftResource.Apple:
                            ////								{
                            ////									if (weapon.MaxRange>1)
                            ////										weapon.MaxRange-=1;
                            ////										
                            ////									if (Utility.Random(100 )>50)
                            ////										weapon.Attributes.WeaponSpeed = Utility.Random( 10 ) + 10;
                            ////									break;
                            ////								}
                            ////								case CraftResource.Pear:
                            ////								{
                            ////								if (weapon.MaxRange>1)
                            ////										weapon.MaxRange-=1;
                            ////									
                            ////									weapon.Attributes.SpellChanneling = 1;											
                            ////									
                            ////									if (Utility.Random(100 )>50)
                            ////										weapon.Attributes.RegenMana = Utility.Random( 3 ) + 1;
                            ////									break;
                            ////								}
                            ////								case CraftResource.Peach:
                            ////								{
                            ////									if (Utility.Random(100 )>50)
                            ////										weapon.Attributes.RegenMana = Utility.Random( 2 ) + 3;
                            ////									
                            ////									if (weapon.MaxRange>1)
                            ////										weapon.MaxRange-=1;
                            ////									
                            ////									break;
                            ////								}
                            ////								case CraftResource.Yew:
                            ////								{
                            ////								if (Utility.Random(100 )>50)
                            ////									weapon.Attributes.BonusHits = Utility.Random( 3 ) + 2;
                            ////								break;
                            ////								}
                            ////								case CraftResource.Banana:
                            ////								{
                            ////									if (weapon.MaxRange>1)
                            ////										weapon.MaxRange+=2;  
                            ////
                            ////									if (Utility.Random(100 )>40)
                            ////										weapon.Attributes.RegenStam = Utility.Random( 3 ) + 2;
                            ////									break;
                            ////								}
                            ////								// Logs Speciali
                            ////								case CraftResource.Swamp:
                            ////								{
                            ////								if (weapon.MaxRange>1)
                            ////									weapon.MaxRange+=1;
                            ////
                            ////								if (Utility.Random(100 )>40)
                            ////									weapon.Attributes.BonusStr = Utility.Random( 3 ) + 2;
                            ////									break;
                            ////								}
                            ////								case CraftResource.Stonewood:
                            ////								{
                            ////									if (weapon.MaxRange>1)
                            ////										weapon.MaxRange+=1;
                            ////										
                            ////									if (Utility.Random(100 )>40)
                            ////										weapon.Attributes.ReflectPhysical = Utility.Random( 15 ) + 5;
                            ////									break;
                            ////								}
                            ////								case CraftResource.Silver:
                            ////								{
                            ////									if (weapon.MaxRange>1)
                            ////										weapon.MaxRange+=1;
                            ////										
                            ////									if (Utility.Random(100 )>40)
                            ////										weapon.Attributes.ReflectPhysical = Utility.Random( 15 ) + 5;
                            ////									break;
                            ////								}
                            ////								case CraftResource.Blood:
                            ////								{		
                            ////									if (weapon.MaxRange>1)
                            ////										weapon.MaxRange-=2;
                            ////									
                            ////									weapon.Attributes.SpellChanneling = 1;								
                            ////									
                            ////									if (Utility.Random(100 )>40)
                            ////										weapon.Attributes.SpellDamage = Utility.Random( 5 ) + 5;
                            ////									break;
                            ////								}
                            ////								case CraftResource.Enchanted:
                            ////								{
                            ////									if (weapon.MaxRange>1)
                            ////										weapon.MaxRange+=1;
                            ////
                            ////									if (Utility.Random(100 )>40)
                            ////										weapon.Attributes.BonusInt = Utility.Random( 4 ) + 2;
                            ////									break;
                            ////								}
                            ////								case CraftResource.Elven:
                            ////								{
                            ////									if (weapon.MaxRange>1)
                            ////										weapon.MaxRange+=2;
                            ////										
                            ////									if (Utility.Random(100 )>40)
                            ////										weapon.Attributes.BonusDex = Utility.Random( 8 ) + 2;
                            ////									break;
                            ////								}
                            ////								case CraftResource.Elder:
                            ////								{
                            ////									if (weapon.MaxRange>1)
                            ////										weapon.MaxRange-=1;
                            ////									
                            ////									weapon.Attributes.SpellChanneling = 1;
                            ////									
                            ////									if (Utility.Random(100 )>40)
                            ////										weapon.Attributes.LowerManaCost = Utility.Random( 3 ) + 2;
                            ////									break;
                            ////								}
                            ////								case CraftResource.Death:
                            ////								{
                            ////									if (weapon.MaxRange>1)
                            ////										weapon.MaxRange-=2;
                            ////									
                            ////									weapon.Attributes.SpellChanneling = 1;
                            ////									
                            ////									if (Utility.Random(100 )>45)
                            ////										weapon.Attributes.ReflectPhysical = Utility.Random( 5 ) + 15;
                            ////									break;
                            ////								}
                            ////								case CraftResource.Crystal:
                            ////								{
                            ////									if (weapon.MaxRange>1)
                            ////										weapon.MaxRange+=1;
                            ////									
                            ////									weapon.Attributes.SpellChanneling = 1;
                            ////									
                            ////									if (Utility.Random(100 )>45)
                            ////										weapon.Attributes.BonusInt = Utility.Random( 3 ) + 2;
                            ////									break;
                            ////								}	
                            ////							}
                            #endregion
                        }
                        else
                        {
                            // TODO Implementare eventuale craft di berto pre-aos
                        }
                    }
                }
            }
        }

        public static void HandleArmorCraft( BaseArmor armor, Mobile crafter )
        {
            if( Midgard2Persistance.MidgardAdvancedCraftEnabled )
            {
                if( armor != null && /*!( armor is SolaretesOfSacrifice ) && */ !( armor is BaseShield ) )
                {
                    if( armor.Quality == ArmorQuality.Exceptional )
                    {
                        if( Core.AOS )
                        {
                            int bonus1 = Utility.Random( 3 ) + 1;
                            int bonus2 = Utility.Random( 3 ) + 1;

                            int numerobonus1 = 1;
                            int numerobonus2 = Utility.Random( 2 ) + 1;

                            if( crafter.Skills[ SkillName.ArmsLore ].Value >= 100 && Utility.Random( 100 ) > 25 )
                                numerobonus1 = 2;

                            if( crafter.Skills[ SkillName.ItemID ].Value >= 100 && Utility.Random( 100 ) > 25 )
                                numerobonus2 = 3;

                            if( SpecialCraftGump.nbonus1 != 0 && SpecialCraftGump.nbonus2 != 0 )
                            {
                                if( Utility.Random( 100 ) > ( 100 - SpecialCraftGump.chance ) )
                                {
                                    bonus1 = SpecialCraftGump.nbonus1;
                                    bonus2 = SpecialCraftGump.nbonus2;
                                    crafter.SendMessage( "Hai centrato il bonus di precisione." );
                                }
                                else
                                    crafter.SendMessage( "Non hai centrato il bonus di precisione." );
                            }
                            else
                                crafter.SendMessage( "Bonus di precisione non attivo" );

                            switch( bonus1 )
                            {
                                case 1:
                                    armor.Attributes.BonusStr = Math.Max( numerobonus1, armor.Attributes.BonusStr );
                                    break;
                                case 2:
                                    armor.Attributes.BonusDex = Math.Max( numerobonus1, armor.Attributes.BonusDex );
                                    break;
                                case 3:
                                    armor.Attributes.BonusInt = Math.Max( numerobonus1, armor.Attributes.BonusInt );
                                    break;
                            }

                            switch( bonus2 )
                            {
                                case 1:
                                    armor.Attributes.BonusHits = Math.Max( numerobonus2, armor.Attributes.BonusHits );
                                    break;
                                case 2:
                                    armor.Attributes.BonusStam = Math.Max( numerobonus2, armor.Attributes.BonusStam );
                                    break;
                                case 3:
                                    armor.Attributes.BonusMana = Math.Max( numerobonus2, armor.Attributes.BonusMana );
                                    break;
                            }
                        }
                        else
                        {
                            // TODO Implementare eventuale craft di berto pre-aos
                        }
                    }
                }
            }
        }

        private static Hashtable m_Table;

        private static void InitializeClilocTable()
        {
            if( m_Table == null )
                m_Table = new Hashtable();

            m_Table[ typeof( CraftableScareCrow ) ] = 1064927;
            m_Table[ typeof( CraftableTrashBarrel ) ] = 1064925;
            m_Table[ typeof( CraftableTrashChest ) ] = 1064924;
            m_Table[ typeof( WeaponBarrel1 ) ] = 1064920;
            m_Table[ typeof( WeaponBarrel2 ) ] = 1064921;
            m_Table[ typeof( WeaponBarrel3 ) ] = 1064922;
            m_Table[ typeof( WeaponBarrel4 ) ] = 1064923;
            m_Table[ typeof( GraveStone1 ) ] = 1064551;
            m_Table[ typeof( GraveStone2 ) ] = 1064551;
            m_Table[ typeof( GraveStone3 ) ] = 1064551;
            m_Table[ typeof( GraveStone4 ) ] = 1064551;
            m_Table[ typeof( GraveStone5 ) ] = 1064551;
            m_Table[ typeof( GraveStone6 ) ] = 1064551;
            m_Table[ typeof( GraveStone7 ) ] = 1064551;
            m_Table[ typeof( GraveStone8 ) ] = 1064551;
            m_Table[ typeof( GraveStone9 ) ] = 1064551;
            m_Table[ typeof( GraveStone10 ) ] = 1064551;
            m_Table[ typeof( GraveStone12 ) ] = 1064551;
            m_Table[ typeof( GraveStone14 ) ] = 1064551;
            m_Table[ typeof( GraveStone15 ) ] = 1064551;
            m_Table[ typeof( GraveStone16 ) ] = 1064551;
            m_Table[ typeof( GraveStone17 ) ] = 1064551;
            m_Table[ typeof( GraveStone18 ) ] = 1064551;
            m_Table[ typeof( GraveStone19 ) ] = 1064551;
            m_Table[ typeof( GraveStone20 ) ] = 1064551;
            m_Table[ typeof( GraveStone21 ) ] = 1064551;
            m_Table[ typeof( GraveStone22 ) ] = 1064551;
            m_Table[ typeof( GraveStone23 ) ] = 1064551;
            m_Table[ typeof( GraveStone24 ) ] = 1064551;
        }

        private static readonly Type[] m_AlwaysHued = new Type[]
		{
		};

        public static bool IsAlwaydHued( Item item )
        {
            Type itemType = item.GetType();
            bool isAlwaydHued = false;

            for( int i = 0; i < m_AlwaysHued.Length && !isAlwaydHued; i++ )
            {
                if( m_AlwaysHued[ i ].IsAssignableFrom( itemType ) )
                    isAlwaydHued = true;
            }

            return isAlwaydHued;
        }

        private static readonly Type[] m_NeverHued = new Type[]
		{
			typeof(BaseBoards)
		};

        public static bool IsNeverHued( Item item )
        {
            Type itemType = item.GetType();
            bool isNeverHued = false;

            for( int i = 0; i < m_NeverHued.Length && !isNeverHued; i++ )
            {
                if( m_NeverHued[ i ].IsAssignableFrom( itemType ) )
                    isNeverHued = true;
            }

            return isNeverHued;
        }

        private static readonly Type[] m_NeverRenamed = new Type[]
		{
			typeof(BaseBoards),
			typeof(BaseGravestone),
		};

        public static bool IsNeverRenamed( Item item )
        {
            Type itemType = item.GetType();
            bool isNeverRenamed = false;

            for( int i = 0; i < m_NeverRenamed.Length && !isNeverRenamed; i++ )
            {
                if( m_NeverRenamed[ i ].IsAssignableFrom( itemType ) )
                    isNeverRenamed = true;
            }

            return isNeverRenamed;
        }

        public static void HandleNameForNotICraftable( Type typeRes, CraftSystem craftSystem, CraftItem craftItem, Item item )
        {
            if( item != null && IsNeverRenamed( item ) )
                return;

            if( typeRes == null )
                typeRes = craftItem.Resources.GetAt( 0 ).ItemType;

            if( typeRes != null && craftSystem.RetainsColorFrom( craftItem, typeRes ) && !( item is ICraftable ) )
            {
                CraftResource res = CraftResources.GetFromType( typeRes );
                if( !CraftResources.IsStandard( res ) )
                {
                    string resName = CraftResources.GetName( res ).ToLower();

                    if( m_Table == null )
                        InitializeClilocTable();

                    if( item == null )
                        return;

                    if( !String.IsNullOrEmpty( item.Name ) )
                        item.Name = String.Format( "{0} {1}", resName, item.Name );
                    else
                    {
                        if( craftItem.NameNumber > 0 )
                        {
                            string itemNameFromCliloc;

                            if( m_Table != null && m_Table.ContainsKey( item.GetType() ) )
                                itemNameFromCliloc = StringList.Localization[ ( (int)m_Table[ item.GetType() ] ) ];
                            else
                                itemNameFromCliloc = StringList.Localization[ craftItem.NameNumber ];

                            if( !String.IsNullOrEmpty( itemNameFromCliloc ) )
                            {
                                item.Name = String.Format( "{0} {1}", resName, itemNameFromCliloc );
                            }
                        }
                    }
                }
            }
        }

        public static void HandleHueForNotICraftable( Type typeRes, CraftSystem craftSystem, CraftItem craftItem, Item item, Mobile crafter )
        {
            if( item != null && IsNeverHued( item ) )
                return;

            if( typeRes == null )
                typeRes = craftItem.Resources.GetAt( 0 ).ItemType;

            if( typeRes != null && craftSystem.RetainsColorFrom( craftItem, typeRes ) && !( item is ICraftable ) )
            {
                CraftResource res = CraftResources.GetFromType( typeRes );
                if( !CraftResources.IsStandard( res ) )
                {
                    if( item == null )
                        return;

                    if( IsAlwaydHued( item ) || !Core.AOS )
                        item.Hue = CraftResources.GetHue( res );
                    else
                    {
                        CraftContext context = craftSystem.GetContext( crafter );
                        if( context != null )
                            item.Hue = context.DoNotColor ? 0 : CraftResources.GetHue( res );
                    }
                }
            }
        }

        public static void VerifyInstrumentName_Callback( object state )
        {
            BaseInstrument instrument = (BaseInstrument)state;
            if( instrument == null )
                return;

            switch( instrument.Hue )
            {
                case 2195:
                    instrument.Resource = CraftResource.Oak;
                    break;
                case 2194:
                    instrument.Resource = CraftResource.Walnut;
                    break;
                case 2178:
                    instrument.Resource = CraftResource.Ohii;
                    break;
                case 2140:
                    instrument.Resource = CraftResource.Cedar;
                    break;
                case 2159:
                    instrument.Resource = CraftResource.Willow;
                    break;
                case 2176:
                    instrument.Resource = CraftResource.Cypress;
                    break;
                case 1921:
                    instrument.Resource = CraftResource.Yew;
                    break;
                case 2182:
                    instrument.Resource = CraftResource.Apple;
                    break;
                case 1920:
                    instrument.Resource = CraftResource.Pear;
                    break;
                case 2171:
                    instrument.Resource = CraftResource.Peach;
                    break;
                case 2295:
                    instrument.Resource = CraftResource.Banana;
                    break;
                case 2151:
                    instrument.Resource = CraftResource.Stonewood;
                    break;
                case 2169:
                    instrument.Resource = CraftResource.Silver;
                    break;
                case 2456:
                    instrument.Resource = CraftResource.Blood;
                    break;
                case 2453:
                    instrument.Resource = CraftResource.Swamp;
                    break;
                case 1931:
                    instrument.Resource = CraftResource.Crystal;
                    break;
                case 2457:
                    instrument.Resource = CraftResource.Elven;
                    break;
                case 2185:
                    instrument.Resource = CraftResource.Elder;
                    break;
                case 2187:
                    instrument.Resource = CraftResource.Enchanted;
                    break;
                case 2468:
                    instrument.Resource = CraftResource.Death;
                    break;
            }

            if( instrument.Name != null )
            {
                try
                {
                    using( TextWriter t = new StreamWriter( "Logs/BaseInstrumentsRename.txt", true ) )
                    {
                        if( instrument.Resource != CraftResource.None )
                            t.WriteLine( "<<< RENAMED: {0} - {1}", instrument.Serial, instrument.Name );
                        else
                            t.WriteLine( ">>> NOT RENAMED: {0} - {1}", instrument.Serial, instrument.Name );
                    }
                }
                catch( Exception ex )
                {
                    Console.WriteLine( ex.ToString() );
                }
            }

            if( instrument.Resource != CraftResource.None )
                instrument.Name = null;
        }

        public static void VerifyContainerName_Callback( object state )
        {
            BaseContainer container = (BaseContainer)state;
            if( container == null )
                return;

            switch( container.Hue )
            {
                case 2195:
                    container.Resource = CraftResource.Oak;
                    break;
                case 2194:
                    container.Resource = CraftResource.Walnut;
                    break;
                case 2178:
                    container.Resource = CraftResource.Ohii;
                    break;
                case 2140:
                    container.Resource = CraftResource.Cedar;
                    break;
                case 2159:
                    container.Resource = CraftResource.Willow;
                    break;
                case 2176:
                    container.Resource = CraftResource.Cypress;
                    break;
                case 1921:
                    container.Resource = CraftResource.Yew;
                    break;
                case 2182:
                    container.Resource = CraftResource.Apple;
                    break;
                case 1920:
                    container.Resource = CraftResource.Pear;
                    break;
                case 2171:
                    container.Resource = CraftResource.Peach;
                    break;
                case 2295:
                    container.Resource = CraftResource.Banana;
                    break;
                case 2151:
                    container.Resource = CraftResource.Stonewood;
                    break;
                case 2169:
                    container.Resource = CraftResource.Silver;
                    break;
                case 2456:
                    container.Resource = CraftResource.Blood;
                    break;
                case 2453:
                    container.Resource = CraftResource.Swamp;
                    break;
                case 1931:
                    container.Resource = CraftResource.Crystal;
                    break;
                case 2457:
                    container.Resource = CraftResource.Elven;
                    break;
                case 2185:
                    container.Resource = CraftResource.Elder;
                    break;
                case 2187:
                    container.Resource = CraftResource.Enchanted;
                    break;
                case 2468:
                    container.Resource = CraftResource.Death;
                    break;
            }

            if( container.Name != null )
            {
                try
                {
                    using( TextWriter t = new StreamWriter( "Logs/BaseContainerRename.txt", true ) )
                    {
                        if( container.Resource != CraftResource.None )
                            t.WriteLine( "<<< RENAMED: {0} - {1}", container.Serial, container.Name );
                        else
                            t.WriteLine( ">>> NOT RENAMED: {0} - {1}", container.Serial, container.Name );
                    }
                }
                catch( Exception ex )
                {
                    Console.WriteLine( ex.ToString() );
                }
            }

            if( container.Resource != CraftResource.None )
                container.Name = null;

            if( container.Name == "Armi e Armature" || container.Name == "Armature" || container.Name == "Vesti e Materiale Vario" ||
                container.Name == "Risorse" || container.Name == "Armi" )
            {
                container.EngravedText = container.Name;
                container.Resource = CraftResource.Iron;
                container.Hue = 0;
                try
                {
                    using( TextWriter t = new StreamWriter( "Logs/PortingContainersRename.txt", true ) )
                    {
                        t.WriteLine( "<<< RENAMED: {0} - {1}", container.Serial, container.EngravedText );
                    }
                }
                catch( Exception ex )
                {
                    Console.WriteLine( ex.ToString() );
                }
            }
        }

        public static void VerifyShields_Callback( object state )
        {
            BaseShield shield = (BaseShield)state;
            if( shield == null )
                return;

            if( shield.ArtifactRarity > 0 )
                return;

            try
            {
                using( TextWriter t = new StreamWriter( "Logs/ShieldVerification.txt", true ) )
                {
                    if( shield.Attributes.BonusStr > 0 )
                    {
                        t.WriteLine( String.Format( "Type {0} - Serial {1} - Resource {2} - BonusStr {3}", shield.GetType().Name, shield.Serial, shield.Resource, shield.Attributes.BonusStr ) );
                        shield.Attributes.BonusStr = 0;
                    }
                    if( shield.Attributes.BonusDex > 0 )
                    {
                        t.WriteLine( String.Format( "Type {0} - Serial {1} - Resource {2} - BonusDex {3}", shield.GetType().Name, shield.Serial, shield.Resource, shield.Attributes.BonusDex ) );
                        shield.Attributes.BonusDex = 0;
                    }
                    if( shield.Attributes.BonusInt > 0 )
                    {
                        t.WriteLine( String.Format( "Type {0} - Serial {1} - Resource {2} - BonusInt {3}", shield.GetType().Name, shield.Serial, shield.Resource, shield.Attributes.BonusInt ) );
                        shield.Attributes.BonusInt = 0;
                    }
                    if( shield.Attributes.BonusHits > 0 )
                    {
                        t.WriteLine( String.Format( "Type {0} - Serial {1} - Resource {2} - BonusHits {3}", shield.GetType().Name, shield.Serial, shield.Resource, shield.Attributes.BonusHits ) );
                        shield.Attributes.BonusHits = 0;
                    }
                    if( shield.Attributes.BonusStam > 0 )
                    {
                        t.WriteLine( String.Format( "Type {0} - Serial {1} - Resource {2} - BonusStam {3}", shield.GetType().Name, shield.Serial, shield.Resource, shield.Attributes.BonusStam ) );
                        shield.Attributes.BonusStam = 0;
                    }
                    if( shield.Attributes.BonusMana > 0 )
                    {
                        t.WriteLine( String.Format( "Type {0} - Serial {1} - Resource {2} - BonusMana {3}", shield.GetType().Name, shield.Serial, shield.Resource, shield.Attributes.BonusMana ) );
                        shield.Attributes.BonusMana = 0;
                    }
                }
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }
        }

        /// <summary>
        /// Statico. Metodo invocato quando si vuole loggare un testo con try{} catch{} su un file.
        /// <param name="toLog" è la stringa che si vuole scrivere sul file ></param>
        /// <param name="fileNamePath" è la path relativa alla core directory del file su cui scrivere.></param>
        /// </summary>
        public static void StringToLog( string toLog, string fileNamePath )
        {
            try
            {
                TextWriter tw = File.AppendText( fileNamePath );
                tw.WriteLine( toLog );
                tw.Close();
            }
            catch( Exception ex )
            {
                Console.Write( "Log della creazione di un loot fallito: {0}", ex );
            }
        }

        public static readonly string CraftLogPath = Path.Combine( "Logs", "MidgardCraftLog.log" );

        /// <summary>
        /// Statico. Metodo invocato quando si vuole loggare info su un oggetto craftato.
        /// </summary>
        public static void CraftLog( Item item, Mobile crafter, BaseTool tool )
        {
            if( item == null || item.Deleted )
                return;

            try
            {
                bool isRunicTool = ( tool is BaseRunicTool );

                StringBuilder sb = new StringBuilder();

                sb.AppendLine( "Crafted item:" );
                sb.AppendLine( String.Format( "\tType {0} - Hue {1} - Serial {2}", item.GetType().Name, item.Hue, item.Serial ) );
                sb.AppendLine( String.Format( "\tCrafter {0} - Account {1} - Serial {2}", crafter.Name, crafter.Account.Username, crafter.Serial ) );
                sb.AppendLine( String.Format( "\tTool {0} - IsRunic {1} - Serial {2} - Charges {3}", tool.GetType().Name, isRunicTool, tool.Serial, tool.UsesRemaining ) );

                if( isRunicTool )
                    sb.AppendLine( String.Format( "\t\tResource {0}", ( (BaseRunicTool)tool ).Resource ) );

                sb.AppendLine( String.Format( "\tDate {0} - Time {1} - Location {2} - Map {3}", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString(), crafter.Location, crafter.Map.Name ) );
                sb.AppendLine( " " );

                TextWriter tw = File.AppendText( CraftLogPath );
                tw.Write( sb );
                tw.Close();
            }
            catch( Exception ex )
            {
                Console.Write( "Warning. Craft log failed: {0}", ex );
            }
        }

        public static void VerifyFletcherTools_Callback( object state )
        {
            RunicFletcherKit tool = (RunicFletcherKit)state;
            if( tool == null )
                return;

            try
            {
                TextWriter tw = File.AppendText( "Logs/fixRuniciPrima.log" );
                tw.WriteLine( String.Format( "Seriale {0} - ItemID {1} - Resource {2} - Hue {3} - Charges {4}",
                                            tool.Serial, tool.ItemID, tool.Resource, tool.Hue, tool.UsesRemaining ) );
                tw.Close();
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }

            int maxCharges;

            switch( tool.Resource )
            {
                case CraftResource.Oak:
                    maxCharges = 45;
                    break;
                case CraftResource.Walnut:
                    maxCharges = 40;
                    break;
                case CraftResource.Ohii:
                    maxCharges = 35;
                    break;
                case CraftResource.Cedar:
                    maxCharges = 30;
                    break;
                case CraftResource.Willow:
                    maxCharges = 25;
                    break;
                case CraftResource.Cypress:
                    maxCharges = 20;
                    break;
                case CraftResource.Yew:
                    maxCharges = 15;
                    break;
                case CraftResource.Apple:
                    maxCharges = 15;
                    break;
                case CraftResource.Pear:
                    maxCharges = 15;
                    break;
                case CraftResource.Peach:
                    maxCharges = 15;
                    break;
                case CraftResource.Banana:
                    maxCharges = 15;
                    break;
                default:
                    {
                        tool.Resource = CraftResource.Oak;
                        maxCharges = 45;
                        break;
                    }
            }

            if( tool.UsesRemaining > maxCharges )
                tool.UsesRemaining = maxCharges;

            tool.Hue = CraftResources.GetHue( tool.Resource );
            tool.Name = null;

            try
            {
                TextWriter tw = File.AppendText( "Logs/fixRuniciDopo.log" );
                tw.WriteLine( String.Format( "Seriale {0} - ItemID {1} - Resource {2} - Hue {3} - Charges {4}",
                                            tool.Serial, tool.ItemID, tool.Resource, tool.Hue, tool.UsesRemaining ) );
                tw.Close();
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.ToString() );
            }
        }

        public static void VerifyWillowRunicCraftedItems_Callback( object state )
        {
            BaseRanged ranged = (BaseRanged)state;
            if( ranged == null )
                return;

            if( ranged.ArtifactRarity > 0 )
                return;

            if( !ranged.PlayerConstructed || ranged.ArtifactRarity > 0 )
                return;

            int count = 0;

            foreach( int j in Enum.GetValues( typeof( AosAttribute ) ) )
            {
                if( ranged.Attributes[ (AosAttribute)j ] > 0 )
                    count++;
            }

            foreach( int j in Enum.GetValues( typeof( AosWeaponAttribute ) ) )
            {
                if( ranged.WeaponAttributes[ (AosWeaponAttribute)j ] > 0 )
                    count++;
            }

            if( ranged.Quality == WeaponQuality.Exceptional && count > 5 ||
                ranged.Quality == WeaponQuality.Regular && count > 4 )
            {
                TextWriter tw = File.AppendText( "Logs/fixWeaponsForWillowRunics.log" );

                tw.WriteLine( String.Format( "Type: {0} - Serial {1} - Name {2} - ItemID {3} - Hue {4} - Attributes {5}",
                                           ranged.GetType().Name, ranged.Serial, ranged.Name, ranged.ItemID, ranged.Hue, count ) );

                foreach( int j in Enum.GetValues( typeof( AosAttribute ) ) )
                {
                    if( ranged.Attributes[ (AosAttribute)j ] > 0 )
                    {
                        tw.WriteLine( String.Format( "\t{0} - {1}", (AosAttribute)j, ranged.Attributes[ (AosAttribute)j ] ) );
                        ranged.Attributes[ (AosAttribute)j ] = 0;
                    }
                }

                tw.WriteLine( "" );

                foreach( int j in Enum.GetValues( typeof( AosWeaponAttribute ) ) )
                {
                    if( ranged.WeaponAttributes[ (AosWeaponAttribute)j ] > 0 )
                    {
                        tw.WriteLine( String.Format( "\t{0} - {1}", (AosWeaponAttribute)j, ranged.WeaponAttributes[ (AosWeaponAttribute)j ] ) );
                        ranged.WeaponAttributes[ (AosWeaponAttribute)j ] = 0;
                    }
                }

                tw.WriteLine( "" );

                if( ranged.Quality == WeaponQuality.Exceptional )
                {
                    if( ranged.Attributes.WeaponDamage > 35 )
                        ranged.Attributes.WeaponDamage -= 20;
                    else
                        ranged.Attributes.WeaponDamage = 15;

                    tw.WriteLine( String.Format( "IsExceptional" ) );
                }

                BaseRunicTool.ApplyAttributesTo( ranged, 3, 20, 70 );

                tw.WriteLine( "" );
                tw.WriteLine( "" );

                tw.Close();
            }
        }

        public static void VerifyManaRegenerationItems_Callback( object state )
        {
            Item item = (Item)state;
            if( item == null )
                return;

            if( item is BaseJewel )
            {
                BaseJewel jewel = (BaseJewel)item;

                if( jewel.ArtifactRarity > 0 )
                    return;

                List<AosAttribute> list = new List<AosAttribute>();
                list.Add( AosAttribute.DefendChance );
                list.Add( AosAttribute.AttackChance );
                list.Add( AosAttribute.LowerManaCost );
                list.Add( AosAttribute.WeaponDamage );
                list.Add( AosAttribute.SpellDamage );
                list.Add( AosAttribute.CastRecovery );
                // list.Add( AosAttribute.CastSpeed );

                AosAttributes att = jewel.Attributes;

                int manaregValue = att.RegenMana;

                if( manaregValue < 1 )
                    return;

                double oldIntensity = manaregValue / 5.0;

                List<AosAttribute> toRemove = new List<AosAttribute>();
                foreach( AosAttribute attribute in list )
                {
                    if( att[ attribute ] > 0 )
                        toRemove.Add( attribute );
                }

                foreach( AosAttribute attribute in toRemove )
                    list.Remove( attribute );

                if( list.Count > 0 )
                {
                    int[] randoms = new int[ list.Count ];

                    int i = 0;
                    foreach( AosAttribute attribute in list )
                    {
                        randoms[ i ] = (int)attribute;
                        i++;
                    }

                    AosAttribute random = (AosAttribute)Utility.RandomList( randoms );

                    int newValue = 0;

                    switch( (int)random )
                    {
                        case (int)AosAttribute.DefendChance:
                            newValue = (int)Math.Ceiling( oldIntensity * 15 );
                            break;
                        case (int)AosAttribute.AttackChance:
                            newValue = (int)Math.Ceiling( oldIntensity * 15 );
                            break;
                        case (int)AosAttribute.LowerManaCost:
                            newValue = (int)Math.Ceiling( oldIntensity * 8 );
                            break;
                        case (int)AosAttribute.WeaponDamage:
                            newValue = (int)Math.Ceiling( oldIntensity * 25 );
                            break;
                        case (int)AosAttribute.CastSpeed:
                            newValue = (int)Math.Ceiling( oldIntensity * 1 );
                            break;
                        case (int)AosAttribute.CastRecovery:
                            newValue = (int)Math.Ceiling( oldIntensity * 3 );
                            break;
                        case (int)AosAttribute.SpellDamage:
                            newValue = (int)Math.Ceiling( oldIntensity * 12 );
                            break;
                        default:
                            Console.WriteLine( "Warning: random att not recognized" );
                            break;
                    }

                    if( att[ random ] < 1 && newValue > 0 )
                    {
                        att[ random ] = newValue;
                        att.RegenMana = 0;
                    }
                    else
                        Console.WriteLine( "Warning: bonus not applyed to JEWEL. NewValue {0}, Att {1}, Att Value {2}", newValue, random, att[ random ] );

                    try
                    {
                        TextWriter tw = File.AppendText( "Logs/fixJewels.log" );
                        tw.WriteLine( String.Format( "Type: {0} - Serial {1} - Name {2} - ItemID {3} - Hue {4} - Manareg {5} - Attribute {6} - Value {7}",
                                                    jewel.GetType().Name, jewel.Serial, jewel.Name, jewel.ItemID, jewel.Hue, manaregValue, random, newValue ) );
                        tw.Close();
                    }
                    catch( Exception ex )
                    {
                        Console.WriteLine( ex.ToString() );
                    }
                }
                else
                {
                    try
                    {
                        TextWriter tw = File.AppendText( "Logs/fixJewelsWARNINGS.log" );
                        tw.WriteLine( String.Format( "Type: {0} - Serial {1} - Name {2} - ItemID {3} - Hue {4} - Manareg {5}",
                                                    jewel.GetType().Name, jewel.Serial, jewel.Name, jewel.ItemID, jewel.Hue, manaregValue ) );
                        tw.Close();
                    }
                    catch( Exception ex )
                    {
                        Console.WriteLine( ex.ToString() );
                    }
                }
            }
            else if( item is BaseWeapon )
            {
                BaseWeapon weapon = (BaseWeapon)item;

                if( weapon.ArtifactRarity > 0 || item is TheHorselord )
                    return;

                List<AosAttribute> list = new List<AosAttribute>();
                list.Add( AosAttribute.WeaponDamage );
                list.Add( AosAttribute.DefendChance );
                list.Add( AosAttribute.AttackChance );
                list.Add( AosAttribute.WeaponSpeed );
                // list.Add( AosAttribute.Luck );

                AosAttributes att = weapon.Attributes;

                int manaregValue = att.RegenMana;

                if( manaregValue < 1 )
                    return;

                double oldIntensity = manaregValue / 5.0;

                List<AosAttribute> toRemove = new List<AosAttribute>();
                foreach( AosAttribute attribute in list )
                {
                    if( att[ attribute ] > 0 )
                        toRemove.Add( attribute );
                }

                foreach( AosAttribute attribute in toRemove )
                    list.Remove( attribute );

                if( list.Count > 0 )
                {
                    int[] randoms = new int[ list.Count ];

                    int i = 0;
                    foreach( AosAttribute attribute in list )
                    {
                        randoms[ i ] = (int)attribute;
                        i++;
                    }

                    AosAttribute random = (AosAttribute)Utility.RandomList( randoms );

                    int newValue = 0;

                    switch( (int)random )
                    {
                        case (int)AosAttribute.DefendChance:
                            newValue = (int)Math.Ceiling( oldIntensity * 15 );
                            break;
                        case (int)AosAttribute.AttackChance:
                            newValue = (int)Math.Ceiling( oldIntensity * 15 );
                            break;
                        case (int)AosAttribute.WeaponDamage:
                            newValue = (int)Math.Ceiling( oldIntensity * 50 );
                            break;
                        case (int)AosAttribute.WeaponSpeed:
                            newValue = (int)( 6.25 * ( manaregValue - 1 ) ) + 5;
                            break;
                        default:
                            break;
                    }

                    if( att[ random ] < 1 && newValue > 0 )
                    {
                        att[ random ] = newValue;
                        att.RegenMana = 0;
                    }
                    else
                        Console.WriteLine( "Warning: bonus not applyed to weapon. NewValue {0}, Att {1}, Att Value {2}", newValue, random, att[ random ] );

                    try
                    {
                        TextWriter tw = File.AppendText( "Logs/fixWeapons.log" );
                        tw.WriteLine( String.Format( "Type: {0} - Serial {1} - Name {2} - ItemID {3} - Hue {4} - Manareg {5} - Attribute {6} - Value {7}",
                                                    weapon.GetType().Name, weapon.Serial, weapon.Name, weapon.ItemID, weapon.Hue, manaregValue, random, newValue ) );
                        tw.Close();
                    }
                    catch( Exception ex )
                    {
                        Console.WriteLine( ex.ToString() );
                    }
                }
                else
                {
                    try
                    {
                        TextWriter tw = File.AppendText( "Logs/fixWeaponsWARNINGS.log" );
                        tw.WriteLine( String.Format( "Type: {0} - Serial {1} - Name {2} - ItemID {3} - Hue {4} - Manareg {5}",
                                                    weapon.GetType().Name, weapon.Serial, weapon.Name, weapon.ItemID, weapon.Hue, manaregValue ) );
                        tw.Close();
                    }
                    catch( Exception ex )
                    {
                        Console.WriteLine( ex.ToString() );
                    }
                }
            }
        }

        #region wood runic utilities
        public static CraftResource GetRandomRunicWood()
        {
            return (CraftResource)Utility.RandomMinMax( (int)CraftResource.Oak, (int)CraftResource.Banana );
        }

        public static int GetRandomRunicWoodCharges( CraftResource resource, int divisor )
        {
            if( divisor == 0 )
            {
                Console.WriteLine( "Warning: division by 0 in GetRandomRunicWoodCharges" );
                return 0;
            }

            int charges = 0;

            switch( resource )
            {
                case CraftResource.Oak:
                    charges = 45;
                    break;
                case CraftResource.Walnut:
                    charges = 40;
                    break;
                case CraftResource.Ohii:
                    charges = 35;
                    break;
                case CraftResource.Cedar:
                    charges = 30;
                    break;
                case CraftResource.Willow:
                    charges = 25;
                    break;
                case CraftResource.Cypress:
                    charges = 20;
                    break;
                case CraftResource.Yew:
                    charges = 15;
                    break;
                case CraftResource.Apple:
                    charges = 15;
                    break;
                case CraftResource.Pear:
                    charges = 15;
                    break;
                case CraftResource.Peach:
                    charges = 15;
                    break;
                case CraftResource.Banana:
                    charges = 15;
                    break;
            }

            return (int)Math.Ceiling( charges / (double)divisor );
        }

        public static RunicFletcherKit RandomFletcherKit()
        {
            CraftResource res = GetRandomRunicWood();
            return new RunicFletcherKit( res, GetRandomRunicWoodCharges( res, 1 ) );
        }
        #endregion
    }
}