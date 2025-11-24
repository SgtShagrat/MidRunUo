using System;
using System.Collections.Generic;

using Midgard.Engines.SpecialEFXSystem;
using Midgard.Mobiles;

using Server.Commands;
using Server.Mobiles;
using Server.Targeting;
using Midgard.Engines.Packager;

namespace Server.Spells.Fifth
{
    public class SummonMinionSpell : MagerySpell
    {
		#region [MinionInfo Definitions]
		private static void ConfigureMinions()
		{
			
            TimeSpan defaultsummontimer = TimeSpan.FromSeconds( 90 );
            var undeads = new MinionInfo[]
                              {
                                  new MinionInfo( typeof(Zombie), 5, -1, defaultsummontimer, 1.0, "Come back to me, my children!" ), //first minion must be 1.0
                                  new MinionInfo( typeof(Skeleton), 3, -1, defaultsummontimer, 0.9, "Come back to me, my children!" ), 
                                  new MinionInfo( typeof(Ghoul), 2, -1, defaultsummontimer, 0.8, "Come back to me, my children!" ),
                                  new MinionInfo( typeof(Mummy), 1, -1, defaultsummontimer, 0.5, "Step into the world of the livings again, my child!" ),
                                  new MinionInfo( typeof(SkeletalKnight), 1, -1, defaultsummontimer, 0.4, "Fight the livings one more time, my firend!" ),
                                  new MinionInfo( typeof(SkeletalMage), 1, -1, defaultsummontimer, 0.3, "Cast again your ethernal courses, my friend!" ),
                                  //new MinionInfo( typeof(SkeletalDragon), 1, -1, defaultsummontimer, 0.1, "Breathe your fire again, my little friend!" )
                              };

            MinionsTable.Add( typeof(LichLord),
                              new BossMinionsInfo( true /*subclass can use this minioninfo*/,
                                                   0.4 /*change for multiple differents minions at once*/,
                                                   0x38cc,
                                                   9,
                                                   0 /*specialeffectid + count + hue for appearance*/	,
                                                   false /* appearance effect may have disappearance too*/,
			                                       0.7 /* scale factor for stats + skills + damage + armour for summoned minions*/,
                                                   undeads ) );

            /*MinionsTable.Add( typeof(BoneMagi),
                              new BossMinionsInfo( true,
                                                   0.4,                                                   
                                                   0x38cc,
                                                   9,
                                                   0,                                                   
                                                   false,
			                                       0.6,
                                                   undeads ) );
			 */
            //MinionsTable.Add( typeof(Dragon), new BossMinionsInfo( true, 0.6, 0x3b5b, 9, 0, false, 0.5, new MinionInfo( typeof(Drake), 1, -1, defaultsummontimer, 1.0, "Defend the nest, my children!" ) ) );

            //MinionsTable.Add( typeof(BaseDragon), new BossMinionsInfo( true, 0.5, 0x3b5b, 9, 0, false, 0.5, new MinionInfo( typeof(Drake), 1, -1, defaultsummontimer, 1.0, "Defend the nest, my children!" ) ) );
			
            MinionsTable.Add( typeof(Balron),
                              new BossMinionsInfo( true,
                                                   0.4,
                                                   0x3b40,
                                                   16,
                                                   0,
                                                   false,
			                                       1.0,
                                                   new MinionInfo( typeof(Mongbat), 5, -1, defaultsummontimer, 1.0, "Flap into the mortal world, brothers!" ),
                                                   new MinionInfo( typeof(Imp), 3, -1, defaultsummontimer, 0.5, "Bring the doom, brothers!" ) ) );

            //MinionsTable.Add( typeof(Imp), new BossMinionsInfo( true, 0.4, 0x1fcb, 14, 0, true, 0.9, new MinionInfo( typeof(Mongbat), 2, -1, defaultsummontimer, 1.0, "Flap and destroy, brothers!" ) ) );

            MinionsTable.Add( typeof(IceFiend),
                              new BossMinionsInfo( true,
                                                   0.4,
                                                   0x1fcb,
                                                   14,
                                                   0x91d,
                                                   true,
			                                       0.8,
                                                   new MinionInfo( typeof(IceSnake), 3, -1, defaultsummontimer, 1.0, "Strip and kill'em all!" ),
                                                   new MinionInfo( typeof(IceElemental), 2, -1, defaultsummontimer, 0.6, "No water! Only ICE!" ),
                                                   new MinionInfo( typeof(IceSerpent), 1, -1, defaultsummontimer, 0.5, "Strip and kill'em all!" ) ) );

            /*MinionsTable.Add( typeof(Gargoyle),
                              new BossMinionsInfo( true,
                                                   0.4,
                                                   0x1fde,
                                                   14,
                                                   0,
                                                   true,
			                                       0.8,
                                                   new MinionInfo( typeof(FireGargoyle), 1, -1, defaultsummontimer, 1.0, "Crumble the mortals!" ),
                                                   new MinionInfo( typeof(StoneGargoyle), 1, -1, defaultsummontimer, 0.6, "Crumble the mortals!" ) ) );
			 */
            //MinionsTable.Add( typeof(Gazer), new BossMinionsInfo( true, 0.4, 0x1fcb, 14, 0, true, 0.8, new MinionInfo( typeof(Ettin), 2, -1, defaultsummontimer, 1.0, "Eyes to Heads!" ) ) );

            MinionsTable.Add( typeof(ElderGazer), new BossMinionsInfo( true, 0.4, 0x3b40, 16, 0, false, 0.8, new MinionInfo( typeof(Gazer), 2, -1, defaultsummontimer, 1.0, "See the doomed world!" ) ) );

            /*MinionsTable.Add( typeof(SkeletalDragon),
                              new BossMinionsInfo( true,
                                                   0.4,
                                                   0x38cc,
                                                   9,
                                                   0,
                                                   false,
			                                       0.7,
                                                   new MinionInfo( typeof(Skeleton), 3, -1, defaultsummontimer, 1.0, "Fight the livings one more time, my firend!" ),
                                                   new MinionInfo( typeof(SkeletalKnight), 2, -1, defaultsummontimer, 0.8, "Fight the livings one more time, my firend!" ),
                                                   new MinionInfo( typeof(SkeletalMage), 1, -1, defaultsummontimer, 0.6, "Cast again your ethernal courses, my friend!" ) ) );
			*/
            /*MinionsTable.Add( typeof(DrowMage),
                              new BossMinionsInfo( true,
                                                   0.4,
                                                   0x1fcb,
                                                   14,
                                                   Utility.RandomSnakeHue(),
                                                   true,
			                                       0.8,
                                                   new MinionInfo( typeof(Snake), 3, -1, defaultsummontimer, 1.0, "Venom is our life!" ),
                                                   new MinionInfo( typeof(GiantSerpent), 2, -1, defaultsummontimer, 0.9, "Venom is our life!" ) ) );
			 */
            //MinionsTable.Add( typeof(JukaMage), new BossMinionsInfo( true, 0.4, 0x1fcb, 14, 0, true, 0.8, new MinionInfo( typeof(Golem), 1, -1, defaultsummontimer, 1.0, "Seek and Destroy!" ) ) );

            //MinionsTable.Add( typeof(GolemController), new BossMinionsInfo( true, 0.4, 0x1fcb, 14, 0, true, 1.0, new MinionInfo( typeof(Golem), 3, -1, defaultsummontimer, 1.0, "Seek and Destroy!" ) ) );

            MinionsTable.Add( typeof(AncientWyrm),
                              new BossMinionsInfo( true,
                                                   0.4,
                                                   0x3b5b,
                                                   9,
                                                   0,
                                                   false,
			                                       0.5
                                                   ,new MinionInfo( typeof(Drake), 3, -1, defaultsummontimer, 1.0, "Defend the nest, my children!" )
                                                   ,new MinionInfo( typeof(Dragon), 2, -1, defaultsummontimer, 0.4, "Defend the nest, my children!" )
                                                   //,new MinionInfo( typeof(ShadowWyrm), 1, -1, defaultsummontimer, 0.1, "The Death came from the shadow!" )
                                                   //,new MinionInfo( typeof(WhiteWyrm), 1, -1, defaultsummontimer, 0.2, "Brother help me to defeat these mortals!" )
			                                     ) );
			
            /*MinionsTable.Add( typeof(ShadowWyrm),
                              new BossMinionsInfo( true,
                                                   0.4,
                                                   0x3b5b,
                                                   9,
                                                   0,
                                                   false,
			                                       0.7,
                                                   new MinionInfo( typeof(ShadowWisp), 5, -1, defaultsummontimer, 1.0, "Come from the shadow!" ),
                                                   new MinionInfo( typeof(ShadowKnight), 3, -1, defaultsummontimer, 0.8, "Come from the shadow!" ),
                                                   new MinionInfo( typeof(Shadowlord), 2, -1, defaultsummontimer, 0.5, "Come from the shadow!" ) ) );
			 */
            /*MinionsTable.Add( typeof(WhiteWyrm),
                              new BossMinionsInfo( true,
                                                   0.4,
                                                   0x3b5b,
                                                   9,
                                                   0,
                                                   false,
			                                       0.6,
                                                   new MinionInfo( typeof(Drake), 3, -1, defaultsummontimer, 1.0, "Defend the nest, my children!" ),
                                                   new MinionInfo( typeof(Dragon), 2, -1, defaultsummontimer, 0.6, "Defend the nest, my children!" )//,
                                                   //new MinionInfo( typeof(AncientWyrm), 1, -1, defaultsummontimer, 0.2, "Brother help me to defeat these mortals!" ) 
			                                     ) );*/
		}
		private static readonly Dictionary<Type, BossMinionsInfo> MinionsTable = new Dictionary<Type, BossMinionsInfo>();
		#endregion
	        
		#region [Magery Definitions]
		private static readonly SpellInfo m_Info = new SpellInfo( /*same as SummonCreatureSpell*/
            "Summon Minion", "Kal Xen", 266, 9040, Reagent.Bloodmoss, Reagent.MandrakeRoot, Reagent.SpidersSilk );

		public override SpellCircle Circle
        {
            get { return SpellCircle.Fifth; }        
		}
		
        public SummonMinionSpell( Mobile caster, Item scroll )
            : base( caster, scroll, m_Info )
        {
        }
		#endregion
		
		#region Package
		public static object[] Package_Info =
        {
            "Script Title", "Summon Minion Spell",
            "Enabled by Default", true,
            "Script Version", new Version( 1, 0, 0, 0 ),
            "Author name", "Magius(CHE)",
            "Creation Date", new DateTime( 2011, 5, 7 ),
            "Author mail-contact", "cheghe@tiscali.it",
            "Author home site", "http://www.magius.it",
            //"Author notes",           null,
            "Script Copyrights", "(C) Midgard Shard - Magius(CHE",
            "Provided packages", new string[] { "Server.Spells.Fifth.SummonMinionSpell" },
            /*"Required packages",       new string[]{"Midgard.Engines.SkillSystem"},*/
            //"Conflicts with packages",new string[0],
            "Research tags", new string[] { "SummonMinionSpell", "Minion", "Spell"}
        };
		
#if DEBUG
        public static bool Debug = true;
#else
		public static bool Debug = false;
#endif
		
		internal static Package Pkg;
		
        public static bool Enabled { get { return Pkg.Enabled; } set { Pkg.Enabled = value; } }

        public static void Package_Configure()
        {
            Pkg = Midgard.Engines.Packager.Core.Singleton[ typeof( SummonMinionSpell ) ];
			
			ConfigureMinions();
        }

        public static void Package_Initialize()
        {
			CommandSystem.Register( "CastMinions", AccessLevel.Seer, new CommandEventHandler( CastMinions_OnCommand ) );
		}
		
		#endregion
		
        #region CastMinions Command
        [Usage( "CastMinions" )]
        [Description( "Cause target to cast its minions" )]
        private static void CastMinions_OnCommand( CommandEventArgs e )
        {
            e.Mobile.SendMessage( "Target the mod that will be forced to cast minions." );
            e.Mobile.Target = new CastMinionsTarget();
        }

        private class CastMinionsTarget : Target
        {
            public CastMinionsTarget()
                : base( 15, false, TargetFlags.None )
            {
            }

            protected override void OnTarget( Mobile from, object targ )
            {
                if( ( targ is BaseCreature ) )
                {
					if (CanSummonOtherMinions( (Mobile) targ ))
					{
						var spell = new SummonMinionSpell( (Mobile) targ, null );
						spell.Cast();
					}
					else
						from.SendMessage( "That is creature cannot cast other minions." );
                }
                else
                    from.SendMessage( "That is not a valid creature." );
            }
        }
        #endregion

		public static Mobile[] GetSummonedMinions(Mobile Caster)
		{
			return Caster.AllFollowers.ToArray();				
		}
		
		public static bool CanSummonOtherMinions(Mobile Caster)
		{
			if (!SummonMinionSpell.Pkg.Enabled)
				return false;

			var summoneds = GetSummonedMinions( Caster );
			var binfo = GetBossMinionsOf( Caster );
			if(binfo == null)
				return false;
			if (summoneds.Length==0)
				return true;
			
			//calculate remains
			foreach(var info in binfo.Minions)
			{
				var already = 0;
				foreach(var mob in summoneds)
					if ( mob.GetType() == info.MinionsType )
						already++;
				
				if ( already < info.MinionsCount )
					return true; //can cast this minion
			}
			
			return false; //too many mionions
		}
		
        public override bool CheckCast()
        {
			if (!SummonMinionSpell.Pkg.Enabled)
				return false;
			
            if( !base.CheckCast() )
                return false;

            BossMinionsInfo bossinfo = GetBossMinionsOf( Caster );

            if( bossinfo == null )
                return false; //no valid entry for this boss

            return true;
        }

        public override void OnCast()
        {
            if( CheckSequence() )
            {
                MinionInfo[] minioninfos = GetMinionsOf( Caster );
                BossMinionsInfo bossinfo = GetBossMinionsOf( Caster );

                string lastmessage = null;
                int appeared = 0;
                foreach( MinionInfo minfo in minioninfos )
                {					
                    int appearednow = 0;
                    for( int h = 0; h < minfo.MinionsCount; h++ )
                    {						
                        if( Utility.RandomDouble() <= minfo.Chance2Appear || h == (minfo.MinionsCount-1)  ) //last minion always appear
                        {
                            int px = Caster.Location.X + Utility.Random( -5, 11 );
                            int py = Caster.Location.Y + Utility.Random( -5, 11 );
                            var p = new Point3D( px, py, Caster.Map.GetAverageZ( px, py ) );

                            if( ! SpellHelper.FindValidSpawnLocation( Caster.Map, ref p, true ) )
                            {
                                p = Caster.Location;
                                SpellHelper.FindValidSpawnLocation( Caster.Map, ref p, true );
                            }

                            new InternalTimer( minfo, Caster, p ).Start();

                            Caster.MovingParticles( new Entity( Serial.Zero, p, Caster.Map ), 0x379F, 10, 0, false, false, 0, 0, 0x216 );
                            //Effects.SendLocationParticles ( EffectItem.Create( p, Caster.Map , TimeSpan.FromSeconds(3) ), 0x3728, 10, 10, Utility.RandomMetalHue(), 0, 2023, 0  );
                            EFXItem efx = SpecialEFX.CreateEffect( p, Caster.Map, bossinfo.EffectId, bossinfo.EffectCount, TimeSpan.FromSeconds( 3 ) );
                            efx.Hue = bossinfo.EffectHue == -1 ? Utility.RandomNondyedHue() : bossinfo.EffectHue;
                            if( bossinfo.PlayReverseEffect )
                            {
                                efx.OnSequenceCompleted += delegate
                                                               {
                                                                   if( efx.Tag == null )
                                                                   {
                                                                       SpecialEFX.ReverseEfx( efx );
                                                                       efx.Tag = true;
                                                                   }
                                                               };
                            }

                            Effects.PlaySound( p, Caster.Map, 0x216 );
                            //Effects.SendLocationEffect ( p, Caster.Map , 0x1FE2, 5 , 2);

                            appeared++;
                            appearednow++;
                        }
                    }
                    if( appearednow > 0 )
                    {
                        /*if ( minfo.SummonSound > -1)
							minion.PlaySound( minfo.SummonSound );
						else
							minion.PlaySound( Caster.GetIdleSound() );
						*/
                        if( !string.IsNullOrEmpty( minfo.CasterMessage ) )
                            lastmessage = minfo.CasterMessage;
                        if( Utility.RandomDouble() > bossinfo.MultipleMinionsAppearChance )
                            break; //no multiple different minions
                    }
                }

                if( appeared > 0 )
                {
                    if( !string.IsNullOrEmpty( lastmessage ) )
                        Caster.Emote( lastmessage );

                    Caster.PlaySound( Caster.GetAngerSound() );
                }
            }

            FinishSequence();
        }

		/// <summary>
		/// Returns cloned MinionInfo modificed with % and count based on already summon minions 
		/// </summary>
		/// <param name="caster">
		/// A <see cref="Mobile"/>
		/// </param>
		/// <returns>
		/// A <see cref="MinionInfo[]"/>
		/// </returns>
        public static MinionInfo[] GetMinionsOf( Mobile Caster )
        {
            var minions = new List<MinionInfo>();
            BossMinionsInfo bossinfo = GetBossMinionsOf( Caster );
            if( bossinfo == null )
				return minions.ToArray();
			
			//adapt minionsinfo
			var summoneds = GetSummonedMinions(Caster);
			if (summoneds.Length == 0)
			{
				minions.AddRange( bossinfo.Minions );
			}
			else
			{
				foreach(var info in bossinfo.Minions)
				{
					var already = 0;
					foreach(var mob in summoneds)
						if ( mob.GetType() == info.MinionsType )
							already++;
					
					var newcount = info.MinionsCount - already;
					if (newcount > 0)
						minions.Add( info.Clone(newcount));
				}
			}			

            minions.Sort( ( a, b ) => a.Chance2Appear.CompareTo( b.Chance2Appear ) );
            return minions.ToArray();
        }

        public static BossMinionsInfo GetBossMinionsOf( Mobile caster )
        {
            BossMinionsInfo bosstoreturn = null;
            foreach( var boss in MinionsTable )
            {
                if( boss.Key == caster.GetType() || ( boss.Value.CanBeUsedBySubclasses && caster.GetType().IsSubclassOf( boss.Key ) ) )
                {
                    bosstoreturn = boss.Value;
                    //do not break here, in this way we can override bossinfo for subclasses
                }
            }

            return bosstoreturn;
        }

        #region Nested type: BossMinionsInfo
        public class BossMinionsInfo
        {
            public readonly bool CanBeUsedBySubclasses;
            public readonly int EffectCount;
            public readonly int EffectHue;
            public readonly int EffectId;
            public readonly MinionInfo[] Minions;
            public readonly double MultipleMinionsAppearChance;
			public readonly double MinionStatScale;
            public readonly bool PlayReverseEffect;

            public BossMinionsInfo( bool canBeUsedBySubclasses, double multipleMinionsAppearChance, int effectId, int effectCount, int effectHue, bool playReverseEffect, double minionStatScale, params MinionInfo[] minions )
            {
                PlayReverseEffect = playReverseEffect;
                EffectId = effectId;
                EffectCount = effectCount;
                EffectHue = effectHue;
                Minions = minions;
                CanBeUsedBySubclasses = canBeUsedBySubclasses;
                MultipleMinionsAppearChance = multipleMinionsAppearChance;
				MinionStatScale = minionStatScale;
            }
        }
        #endregion

        #region Nested type: InternalTimer
        private class InternalTimer : Timer
        {
            private readonly Mobile Caster;
            private readonly Point3D Destination;
            private readonly MinionInfo Minion;
            private readonly TimeSpan SummonDuration;

            public InternalTimer( MinionInfo minion, Mobile caster, Point3D dest )
                : base( TimeSpan.FromSeconds( 2.0 ) )
            {
                Minion = minion;
                Caster = caster;
                Destination = dest;

                TimeSpan mageryduration = TimeSpan.FromSeconds( Caster.Skills[ SkillName.Magery ].Value );
                SummonDuration = minion.SummonDuration;
                if( SummonDuration == TimeSpan.Zero )
                    SummonDuration = mageryduration;
            }
			
            protected override void OnTick()
            {
                var minion = (BaseCreature) Activator.CreateInstance( Minion.MinionsType );

                BaseCreature.Summon( minion, Caster, Destination, Minion.SummonSound, SummonDuration );
				
				ScaleMinion(Caster, minion , Minion);
				
				if (SummonMinionSpell.Debug)
					SummonMinionSpell.Pkg.LogInfoLine("SummonMinionSpell: {0} has now {1} followers.",Caster.Name, Caster.Followers);
				
                minion.PlaySound( 0x1FD );
                minion.FixedParticles( 0x3728, 10, 4, 2023, EffectLayer.Head );

                Stop();
            }
			private void ScaleMinion(Mobile Caster, Mobile Minion, MinionInfo info)
			{
				var binfo = GetBossMinionsOf ( Caster );
				if (binfo == null)
					return;
				var scale = binfo.MinionStatScale;
				if( scale != 1.0 )
				{
					var bc = Minion as BaseCreature;
					var hits = Minion.HitsMax;
					bc.RawStr = (int)(bc.RawStr * scale);
					bc.SetHitsInvariant( (int)Math.Floor( (double)hits * scale ) );					
	
					var stam = bc.StamMax;
					bc.RawDex = (int)(bc.RawDex * scale);
					bc.SetStam ( (int)Math.Floor( (double)stam * scale ) ); 
	
					var mana = bc.ManaMax;
					bc.RawInt = (int)(bc.RawInt * scale);
					bc.SetMana ( (int)Math.Floor( (double)stam * scale ) ); 
					
					foreach(Skill sk in bc.Skills)
						if (sk!=null)
							sk.Base *= scale;
					
					bc.VirtualArmor = (int)Math.Floor( (double)Minion.VirtualArmor * scale );
					
					bc.SetDamage(
					             (int)Math.Floor( (double)bc.DamageMin * scale ),
					             (int)Math.Floor( (double)bc.DamageMax * scale )
					             );
					             
				}
			}
        }
        #endregion

        #region Nested type: MinionInfo
        public class MinionInfo
        {
            public readonly string CasterMessage;
            public readonly double Chance2Appear;
            public readonly int MinionsCount;
            public readonly Type MinionsType;
            public readonly TimeSpan SummonDuration;
            public readonly int SummonSound;

            public MinionInfo( Type minionsType, int minionsCount, int summonsound, TimeSpan summonduration, double chancetoappear, string casterMessage )
            {
                MinionsType = minionsType;
                MinionsCount = minionsCount;
                SummonSound = summonsound;
                SummonDuration = summonduration;
                Chance2Appear = chancetoappear;
                CasterMessage = casterMessage;
            }
			public MinionInfo Clone(int minionsCount)
			{
				return new MinionInfo (MinionsType,minionsCount,SummonSound,SummonDuration,Chance2Appear,CasterMessage);
			}
			public MinionInfo Clone(int minionsCount, double chancetoappear)
			{
				return new MinionInfo (MinionsType,minionsCount,SummonSound,SummonDuration,chancetoappear,CasterMessage);				
			}
        }
        #endregion
    }
}