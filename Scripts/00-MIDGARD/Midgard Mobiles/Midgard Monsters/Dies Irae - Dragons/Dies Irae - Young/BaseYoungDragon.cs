/***************************************************************************
 *                                  BaseYoungDragon.cs
 *                            		-------------------
 *  begin                	: Dicembre, 2006
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 *		Classe astratta base per implementare i Draghi **GIOVANI** su Midgard.
 * 
 ***************************************************************************/

using System.Collections.Generic;

using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Midgard.Mobiles
{
    public abstract class BaseYoungDragon : BaseCreature
    {
        #region costanti
        // Costanti per i colpi speciali
        public const double PercEarthQuake = 0.10;

        // Costanti per le frasi dette dal drago
        public const int Colore = 33; // Rosso
        public const int Font = 1;
        #endregion

        #region costruttori
        public BaseYoungDragon( AIType aiType, FightMode mode )
            : base( aiType, mode, 10, 1, 0.2, 0.4 )
        {
        }

        public BaseYoungDragon( AIType aiType )
            : this( aiType, FightMode.Closest )
        {
        }

        public BaseYoungDragon()
            : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
        {
        }

        public BaseYoungDragon( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override void GenerateLoot()
        {
            AddLoot( LootPack.UltraRich, 1 );
            AddLoot( LootPack.HighScrolls, Utility.RandomMinMax( 5, 15 ) );
            AddLoot( LootPack.Gems, Utility.RandomMinMax( 5, 15 ) );
        }

        public override int GetIdleSound()
        {
            return 0x2D3;
        }

        public override int GetHurtSound()
        {
            return 0x2D1;
        }

        // Il Breath viene riscritto come segue
        public override bool HasBreath { get { return true; } }
        public override double BreathDamageScalar { get { return 0.05; } }
        public override int BreathEffectHue { get { return Hue; } }
        public override double BreathMinDelay { get { return 10.0; } }
        public override double BreathMaxDelay { get { return 15.0; } }
        public override int BreathColdDamage { get { return 20; } }
        public override int BreathFireDamage { get { return 20; } }
        public override int BreathEnergyDamage { get { return 20; } }
        public override int BreathPhysicalDamage { get { return 20; } }
        public override int BreathPoisonDamage { get { return 20; } }

        // Risorse carvate
        public override HideType HideType { get { return HideType.Barbed; } }
        public override int Hides { get { return 20; } }
        public override int Meat { get { return 10; } }
        public override ScaleType ScaleType { get { return (ScaleType)Utility.Random( 4 ); } }
        public override int Scales { get { return 5; } }

        // Mappe del tesoro
        public override double TreasureMapChance { get { return 0.75; } }
        public override int TreasureMapLevel { get { return 3; } }

        // Anti Bard props
        public override bool Uncalmable { get { return true; } }
        public override bool Unprovokable { get { return true; } }

        // Altre caratteristiche
        public override bool ReacquireOnMovement { get { return true; } }
        public override bool AutoDispel { get { return false; } }
        public override Poison PoisonImmune { get { return Poison.Regular; } }
        public override Poison HitPoison { get { return Poison.Lesser; } }
        public override bool AlwaysMurderer { get { return false; } }
        public override bool CanMoveOverObstacles { get { return true; } }
        public override bool CanOpenDoors { get { return false; } }
        public override bool CanRummageCorpses { get { return true; } }
        public override bool IsScaryToPets { get { return true; } }

        public override void OnDeath( Container c )
        {
            base.OnDeath( c );

            if( !IsBonded )
                c.DropItem( new DragonBlood( Utility.RandomMinMax( 5, 10 ) ) );
        }

        public override void OnGaveMeleeAttack( Mobile defender )
        {
            base.OnGaveMeleeAttack( defender );

            if( Utility.RandomDouble() <= PercEarthQuake )
            {
                BaseXmlSpawner.PublicOverheadMobileMessage( this, MessageType.Emote, Colore, Font,
                                                            "il Giovane Drago sbatte furiosamente la coda a terra!", true );
                Earthquake();
            }

            defender.Stam -= Utility.RandomMinMax( 3, 6 );
            defender.Mana -= Utility.RandomMinMax( 3, 6 );
        }

        public override void OnGotMeleeAttack( Mobile attacker )
        {
            base.OnGotMeleeAttack( attacker );

            attacker.Stam -= Utility.RandomMinMax( 3, 6 );
            attacker.Mana -= Utility.RandomMinMax( 3, 6 );
        }

        private void Earthquake()
        {
            Map map = Map;

            if( map == null )
                return;

            List<Mobile> targets = new List<Mobile>();

            foreach( Mobile m in GetMobilesInRange( 6 ) )
            {
                if( m == this || !CanBeHarmful( m ) )
                    continue;

                if( m is BaseCreature && ( ( (BaseCreature)m ).Controlled || ( (BaseCreature)m ).Summoned || ( (BaseCreature)m ).Team != Team ) )
                    targets.Add( m );
                else if( m.Player )
                    targets.Add( m );
            }

            PlaySound( 0x2F3 );

            for( int i = 0; i < targets.Count; ++i )
            {
                Mobile m = targets[ i ];

                double damage = m.Hits * 0.6;

                if( damage < 10.0 )
                    damage = 10.0;
                else if( damage > 40.0 )
                    damage = 40.0;

                DoHarmful( m );

                AOS.Damage( m, this, (int)damage, 100, 0, 0, 0, 0 );

                if( m.Alive && m.Body.IsHuman && !m.Mounted )
                    m.Animate( 20, 7, 1, true, false, 0 ); // take hit
            }
        }
        #endregion

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