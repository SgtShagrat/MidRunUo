// #define DebugBaseDragon
/***************************************************************************
 *                                  BaseDragon.cs
 *                            		-------------------
 *  begin                	: Dicembre, 2006
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 *			Classe astratta base per implementare i Draghi **ADULTI** su Midgard.
 * 
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Midgard.Engines.SpellSystem;

using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Midgard.Mobiles
{
    public abstract class BaseDragon : BaseCreature
    {
        // Costanti per il loot speciale
        public const double PercMinorArtifact = 0.01;
        public const double PercPowerScroll = 0.01;

        // Costanti per i colpi speciali
        public const double PercEarthQuake = 0.25;

        // Costanti per le frasi dette dal drago
        public const int Colore = 0x25; // Rosso
        public const int Font = 1;

        #region costruttori
        public BaseDragon( AIType aiType, FightMode mode )
            : base( aiType, mode, 10, 1, 0.2, 0.4 )
        {
        }

        public BaseDragon( AIType aiType )
            : this( aiType, FightMode.Closest )
        {
        }

        public BaseDragon()
            : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
        }

        public BaseDragon( Serial serial )
            : base( serial )
        {
        }
        #endregion

        public override void GenerateLoot()
        {
            AddLoot( LootPack.FilthyRich, 1 );
            AddLoot( LootPack.HighScrolls, Utility.RandomMinMax( 5, 10 ) );
            AddLoot( LootPack.Gems, Utility.RandomMinMax( 5, 10 ) );
        }

        // Il Breath viene riscritto come segue
        public override bool HasBreath { get { return true; } }
        public override double BreathDamageScalar { get { return 0.05; } }
        public override int BreathEffectHue { get { return Hue; } }
        public override double BreathMinDelay { get { return 5.0; } }
        public override double BreathMaxDelay { get { return 10.0; } }
        public override int BreathColdDamage { get { return 20; } }
        public override int BreathFireDamage { get { return 20; } }
        public override int BreathEnergyDamage { get { return 20; } }
        public override int BreathPhysicalDamage { get { return 20; } }
        public override int BreathPoisonDamage { get { return 20; } }

        // Risorse carvate
        public override HideType HideType { get { return HideType.Barbed; } }
        public override int Hides { get { return 40; } }
        public override int Meat { get { return 20; } }
        public override ScaleType ScaleType { get { return (ScaleType)Utility.Random( 4 ); } }
        public override int Scales { get { return 10; } }

        // Mappe del tesoro
        public override double TreasureMapChance { get { return 0.75; } }
        public override int TreasureMapLevel { get { return 4; } }

        // Anti Bard props
        public override bool BardImmune { get { return true; } }
        public override bool Uncalmable { get { return true; } }
        public override bool Unprovokable { get { return true; } }

        // Altre caratteristiche
        public override bool ReacquireOnMovement { get { return true; } }
        public override bool AutoDispel { get { return true; } }
        public override double AutoDispelChance { get { return 0.2; } }

        public override Poison PoisonImmune { get { return Poison.Regular; } }
        public override Poison HitPoison { get { return Utility.RandomBool() ? Poison.Lesser : Poison.Greater; } }
        public override bool AlwaysMurderer { get { return true; } }
        public override bool CanMoveOverObstacles { get { return true; } }
        public override bool CanOpenDoors { get { return false; } }
        public override bool CanRummageCorpses { get { return true; } }
        public override bool IsScaryToPets { get { return true; } }

        public override void AlterMeleeDamageTo( Mobile to, ref int damage )
        {
            if( to is BaseCreature )
                damage = damage * 2;

            base.AlterMeleeDamageTo( to, ref damage );
        }

        public override void OnDeath( Container c )
        {
            base.OnDeath( c );

            if( Utility.RandomDouble() <= PercMinorArtifact )
                DemonKnight.DistributeArtifact( this, (Item)Activator.CreateInstance( Paragon.OldArtifacts[ Utility.Random( Paragon.OldArtifacts.Length ) ] ) );

            c.DropItem( new DragonBlood( Utility.RandomMinMax( 10, 13 ) ) );
        }

        public override void OnGaveMeleeAttack( Mobile defender )
        {
            base.OnGaveMeleeAttack( defender );

            if( Utility.RandomDouble() <= PercEarthQuake )
            {
                PublicOverheadMessage( MessageType.Regular, 0x3C3, true, "* earth sunks around the dragon *" );
                Earthquake();
            }

            defender.Stam -= Utility.RandomMinMax( 1, 3 );
            defender.Mana -= Utility.RandomMinMax( 4, 8 );
        }

        private void Earthquake()
        {
            Map map = Map;

            if( map == null )
                return;

            List<Mobile> targets = new List<Mobile>();

            foreach( Mobile m in GetMobilesInRange( 8 ) )
            {
                if( m == this || !CanBeHarmful( m ) )
                    continue;

                if( m is BaseCreature && ( ( (BaseCreature)m ).Controlled || ( (BaseCreature)m ).Summoned || ( (BaseCreature)m ).Team != Team ) )
                    targets.Add( m );
                else if( m.Player )
                    targets.Add( m );
            }

            PlaySound( 0x2F3 );

            foreach( Mobile m in targets )
            {
                double damage = m.Hits * 0.4;

                if( damage < 10.0 )
                    damage = 10.0;
                else if( damage > 40.0 )
                    damage = 40.0;

                DoHarmful( m );

                MidgardSpellHelper.Damage( TimeSpan.Zero, m, this, damage, SpellType.Impact );
            }
        }

        #region Serialize-Deserialize
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
    }
}