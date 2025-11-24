using System;

using Server;
using Server.Items;
using Server.Network;

namespace Midgard.Items
{
    public abstract class BasePaganPotion : BasePotion
    {
        public static bool AlchemyIsRequiredToDrink = false;

        public abstract int Strength { get; }

        public virtual double AlchemyRequiredToDrink
        {
            get { return 0.0; }
        }

        public BasePaganPotion( int itemid, PotionEffect effect, int amount )
            : base( itemid, effect, amount )
        {
            Weight = 1.0;
        }

        public BasePaganPotion( PotionEffect effect, int amount )
            : base( 0xF0E, effect, amount )
        {
            Weight = 1.0;
        }

        public override int LabelNumber
        {
            get { return 1064000 + (int)PotionEffect; } // da 1064100 a 1064xxx
        } 

        public abstract bool DoPaganEffect( Mobile from );

        public new void PlayDrinkEffect( Mobile m )
        {
            // Il medoto va settato new rispetto quello di BasePaganPotion 
            // perche' non deve fare un azione di from.Revealing()
            // quando la pozione viene bevuta !!
            if( GetType() != typeof( PaganInvisibilityPotion ) )
            {
                m.RevealingAction( true );
            }

            if( CustomSound > 0 )
                m.PlaySound( CustomSound );
            else
                m.PlaySound( 0x2D6 );

            m.AddToBackpack( new Bottle() );

            if( m.Body.IsHuman && !m.Mounted && CustomAnim > 0 )
                m.Animate( CustomAnim, 5, 1, true, false, 0 );
            else
                m.Animate( 34, 5, 1, true, false, 0 );

            if( CustomEffects > 0 )
                m.FixedParticles( CustomEffects, 1, 62, 9923, 3, 3, EffectLayer.Waist );
        }

        public virtual int CustomSound
        {
            get { return 0; }
        }

        public virtual int CustomAnim
        {
            get { return 0; }
        }

        public virtual int CustomEffects
        {
            get { return 0; }
        }

        public override void Drink( Mobile from )
        {
            if( DoPaganEffect( from ) )
            {
                PlayDrinkEffect( from );
                Consume();
            }
        }

        public override void OnDoubleClick( Mobile from )
        {
            if( !IsChildOf( from.Backpack ) )
            {
                from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
                return;
            }

            if( AlchemyIsRequiredToDrink && AlchemyRequiredToDrink > 0 && from.Skills[ SkillName.Alchemy ].Value < AlchemyRequiredToDrink )
            {
                from.SendMessage( "You must have at least {0} in alchemy to use this potion.", AlchemyRequiredToDrink.ToString( "F1" ) );
                return;
            }

            base.OnDoubleClick( from );
        }

        public override bool CanDrink( Mobile from, bool message )
        {
            bool canDrink = from.CanBeginAction( typeof( BasePaganPotion ) );

            if( !canDrink && message )
            {
                from.PublicOverheadMessage( MessageType.Regular, 0x3B2, true, "You must wait until you can use that!" );
            }

            return canDrink;
        }

        #region tables
        private static readonly Type[] m_PaganTypes = new Type[]
                                                      {
                                                          typeof (GrandMageRefreshElixirLesser),
                                                          typeof (GrandMageRefreshElixir),
                                                          typeof (GrandMageRefreshElixirGreater),
                                                          typeof (HomericMightPotion),
                                                          typeof (HomericMightPotionGreater),
                                                          typeof (MegoInvulnerabilityPotionLesser),
                                                          typeof (MegoInvulnerabilityPotion),
                                                          typeof (MegoInvulnerabilityPotionGreater),
                                                          typeof (PhandelsFineIntellectPotion),
                                                          typeof (PhandelsFabulousIntellectPotion),
                                                          typeof (PhandelsFantasticIntellectPotion),
                                                          typeof (PaganInvisibilityPotion),
                                                          typeof (TamlaPotion),
                                                          typeof (TaintsMinorTransmutationPotion),
                                                          typeof (TaintsMajorTransmutationPotion),
                                                          typeof (Totem),
                                                          typeof (Elixir),
                                                          typeof (FlashBangPotion)
                                                      };

        private static readonly Type[] m_PaganCommonTypes = new Type[]
                                                            {
                                                                typeof (GrandMageRefreshElixirLesser),
                                                                typeof (HomericMightPotion),
                                                                typeof (MegoInvulnerabilityPotionLesser),
                                                                typeof (PhandelsFineIntellectPotion),
                                                                typeof (PaganInvisibilityPotion),
                                                                typeof (TamlaPotion),
                                                                typeof (TaintsMinorTransmutationPotion),
                                                                typeof (Totem),
                                                                typeof (Elixir),
                                                            };

        private static readonly Type[] m_PaganRareTypes = new Type[]
                                                          {
                                                              typeof (GrandMageRefreshElixirGreater),
                                                              typeof (HomericMightPotionGreater),
                                                              typeof (MegoInvulnerabilityPotionGreater),
                                                              typeof (PhandelsFantasticIntellectPotion),
                                                              typeof (PaganInvisibilityPotion),
                                                              typeof (TamlaPotion),
                                                              typeof (TaintsMajorTransmutationPotion),
                                                              typeof (Totem),
                                                              typeof (Elixir),
                                                          };
        #endregion

        public static Item RandomPaganPotion()
        {
            return Loot.Construct( m_PaganTypes );
        }

        public static Item RandomCommonPaganPotion()
        {
            return Loot.Construct( m_PaganCommonTypes );
        }

        public static Item RandomRarePotion()
        {
            return Loot.Construct( m_PaganRareTypes );
        }

        #region serialization
        public BasePaganPotion( Serial serial )
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
    }
}