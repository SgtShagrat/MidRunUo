//#define DebugTiamatChampion
/***************************************************************************
 *                                  TiamatChampion.cs
 *                            		-------------------
 *  begin                	: Dicembre, 2006
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			Tiamat e' il signore assoluto dei draghi.
 * 			Questa versione e' un BaseChampion.
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;

using Server;
using Server.Engines.CannedEvil;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Engines.XmlSpawner2;

namespace Midgard.Mobiles
{
    public class TiamatChampion : BaseChampion
    {
        public override ChampionSkullType SkullType { get { return ChampionSkullType.Power; } }
        public override Type[] UniqueList
        {
            get { return new Type[] { }; }
        }

        public override Type[] SharedList
        {
            get { return new Type[] { }; }
        }

        public override Type[] DecorativeList
        {
            get { return new Type[] { }; }
        }

        public override MonsterStatuetteType[] StatueTypes
        {
            get { return new MonsterStatuetteType[] { }; }
        }

        #region costanti
        // Costanti per il loot speciale
#if( DebugTiamatChampion )
		public const double PercArtifact = 1.0;
#else
        public const double PercArtifact = 0.5;
#endif
        // Costanti per i colpi speciali
        public const double PercEarthQuake = 0.5;

        // Costanti per il reveal
        public const double PercReveal = 0.01;
        public const int RaggioReveal = 10;

        // Costanti per le frasi dette dal drago
        public const int Colore = 33; // Rosso
        public const int Font = 1;
        #endregion

        #region costruttori
        [Constructable]
        public TiamatChampion()
            : base( AIType.AI_Mage )
        {
            Name = "Tiamat";
            Title = ", the Supreme Lord of Dragons";
            Hue = 2994;		// Arancio intenso
            Body = 46;		// Ancient Wyrm
            BaseSoundID = 362;

            SetStr( 898, 1030 );
            SetDex( 68, 200 );
            SetInt( 488, 620 );

            SetHits( 70000 );
            SetMana( 5000 );

            SetDamage( 29, 35 );

            SetDamageType( ResistanceType.Physical, 20 );
            SetDamageType( ResistanceType.Fire, 20 );
            SetDamageType( ResistanceType.Cold, 20 );
            SetDamageType( ResistanceType.Poison, 20 );
            SetDamageType( ResistanceType.Energy, 20 );

            SetResistance( ResistanceType.Physical, 90, 100 );
            SetResistance( ResistanceType.Fire, 90, 100 );
            SetResistance( ResistanceType.Cold, 90, 100 );
            SetResistance( ResistanceType.Poison, 90, 100 );
            SetResistance( ResistanceType.Energy, 90, 100 );

            SetSkill( SkillName.EvalInt, 120.1, 130.0 );
            SetSkill( SkillName.Magery, 120.1, 130.0 );
            SetSkill( SkillName.Meditation, 110.0, 120.0 );
            SetSkill( SkillName.MagicResist, 150.5, 200.0 );
            SetSkill( SkillName.Tactics, 70.1, 100.0 );
            SetSkill( SkillName.Wrestling, 160.1, 180.0 );

            Fame = 200000;
            Karma = -50000;  // Caotico Malvagio

            VirtualArmor = 140;

            Tamable = false;

            // Mosse speciali dell' ATS
            FireBreathAttack = 200;
            PetPoisonAttack = 200;
            RoarAttack = 200;

            #region xmlattach
            // Attachment EnemyMastery
            XmlEnemyMastery wyrmMastery = new XmlEnemyMastery( "WhiteWyrm", 100, 1000 );
            wyrmMastery.Name = "WyrmMastery";
            XmlAttach.AttachTo( this, wyrmMastery );

            XmlEnemyMastery dragonMastery = new XmlEnemyMastery( "Dragon", 100, 1000 );
            dragonMastery.Name = "DragonMastery";
            XmlAttach.AttachTo( this, dragonMastery );

            XmlEnemyMastery nightmareMastery = new XmlEnemyMastery( "Nightmare", 100, 1000 );
            nightmareMastery.Name = "NightmareMastery";
            XmlAttach.AttachTo( this, nightmareMastery );

            XmlEnemyMastery kirinMastery = new XmlEnemyMastery( "Kirin", 100, 1000 );
            kirinMastery.Name = "KirinMastery";
            XmlAttach.AttachTo( this, kirinMastery );

            XmlEnemyMastery unicornMastery = new XmlEnemyMastery( "Unicorn", 100, 1000 );
            unicornMastery.Name = "UnicornMastery";
            XmlAttach.AttachTo( this, unicornMastery );
            #endregion
        }

        public TiamatChampion( Serial serial )
            : base( serial )
        {
        }
        #endregion

        #region metodi
        public override void GenerateLoot()
        {
            AddLoot( LootPack.SuperBoss, 2 );
            AddLoot( LootPack.HighScrolls, Utility.RandomMinMax( 20, 50 ) );
            AddLoot( LootPack.Gems, Utility.RandomMinMax( 20, 50 ) );
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
        public override double BreathDamageScalar { get { return 0.007; } }
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
        public override int Hides { get { return 80; } }
        public override int Meat { get { return 40; } }
        public override ScaleType ScaleType { get { return (ScaleType)Utility.Random( 4 ); } }
        public override int Scales { get { return 20; } }

        // Mappe del tesoro
        public override double TreasureMapChance { get { return 0.75; } }
        public override int TreasureMapLevel { get { return 5; } }

        // Anti Bard props
        public override bool Uncalmable { get { return true; } }
        public override bool Unprovokable { get { return true; } }

        // Altre caratteristiche
        public override bool ReacquireOnMovement { get { return true; } }
        public override bool AutoDispel { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Regular; } }
        public override Poison HitPoison { get { return Utility.RandomBool() ? Poison.Greater : Poison.Deadly; } }
        public override bool AlwaysMurderer { get { return true; } }
        public override bool CanMoveOverObstacles { get { return true; } }
        public override bool CanOpenDoors { get { return false; } }
        public override bool CanRummageCorpses { get { return true; } }
        public override bool IsScaryToPets { get { return true; } }

        public override void OnThink()
        {
            if( Utility.RandomDouble() <= PercReveal )
            {
                BaseXmlSpawner.PublicOverheadMobileMessage( this, MessageType.Emote, Colore, Font,
                                                            "*il drago cerca attentamente attorno a se...*", true );
                IPooledEnumerable inRange = Map.GetMobilesInRange( Location, RaggioReveal );
                foreach( Mobile trg in inRange )
                {
                    if( trg != null )
                    {
                        if( trg.Hidden && trg.AccessLevel == AccessLevel.Player )
                        {
                            trg.RevealingAction();
                            trg.SendMessage( "{0} ti ha scovato!", Name );
                        }
                    }
                }
            }
            base.OnThink();
        }

        public override void OnDeath( Container c )
        {
            base.OnDeath( c );

            // Aggiunto al pack un artefatto maggiore
            if( Utility.RandomDouble() <= PercArtifact )
            {
                DemonKnight.DistributeArtifact( this );
            }
        }

        public override void OnGaveMeleeAttack( Mobile defender )
        {
            base.OnGaveMeleeAttack( defender );

            if( Utility.RandomDouble() <= PercEarthQuake )
            {
                BaseXmlSpawner.PublicOverheadMobileMessage( this, MessageType.Emote, Colore, Font,
                                                            "il Drago sbatte furiosamente la coda a terra!", true );
                Earthquake();
            }

            defender.Stam -= Utility.RandomMinMax( 5, 10 );
            defender.Mana -= Utility.RandomMinMax( 5, 10 );
        }

        public override void OnGotMeleeAttack( Mobile attacker )
        {
            base.OnGotMeleeAttack( attacker );

            attacker.Stam -= Utility.RandomMinMax( 5, 10 );
            attacker.Mana -= Utility.RandomMinMax( 5, 10 );
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

            for( int i = 0; i < targets.Count; ++i )
            {
                Mobile m = (Mobile)targets[ i ];

                double damage = m.Hits * 0.6;

                if( damage < 10.0 )
                    damage = 10.0;
                else if( damage > 75.0 )
                    damage = 75.0;

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
