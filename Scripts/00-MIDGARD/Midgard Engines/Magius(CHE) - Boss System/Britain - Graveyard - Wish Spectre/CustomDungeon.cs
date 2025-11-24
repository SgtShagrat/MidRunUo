using Server;
using System;
using Server.Mobiles;
using System.Collections.Generic;
using Server.Items;
using Server.Engines.PartySystem;
namespace Midgard.Engines.BossSystem.WishSpectre
{
    /// <summary>
    /// All dungeon with same DungeonGroup will be mutually exclusive. (one per time)
    /// </summary>
    [DungeonGroup("Britain Graveyard")]
    internal class CustomDungeon : Dungeon
    {
		InternalTimer timer;
        public CustomDungeon()
            : base( Core.GetRegion( "Britain Graveyard" ) )
        {
            Name = "Thumb of Exurk Alh - The Wish Spectre";
			JoinRequirements.AddRange( new DungeonJoinRequirement[] {
				new JRPlayerSkillAttackMinimum (400), //player must be minimum attack skill at.
				new JRPlayerNewbie (false), //player cannot be Newbie
			});
			RandomAreaEffects = true;
        }
		
		protected override bool OnTimersCreate ()
		{
			/*timer = new InternalTimer(this);
			timer.Start();*/
			
			return true;
		}

        protected override bool OnCreate()
        {
            return true;
        }
		protected override void OnCreatureDeath (BaseCreature Creature, Container Corpse, List<PlayerMobile> InvolvedPlayers )
		{
			var nullstageplayers = new List<PlayerMobile>();
			foreach(var player in InvolvedPlayers)
			{
				var stage = GetPlayerStageSet( player ) as WishSpectreStageSet;
				if (stage == null)
					nullstageplayers.Add(player);
			}
			if (nullstageplayers.Count>0)
			{
				DropHeadBone ( Creature, Corpse, nullstageplayers.ToArray() , 1.0 /*0.1*/ ); // 10% to first appear				
				//DropRandomBone( Creature, Corpse, nullstageplayers.ToArray() , 1.0 /*0.1*/ ); // 10% to first appear
			}

			base.OnCreatureDeath (Creature, Corpse , InvolvedPlayers);
		}
		private void DropHeadBone( BaseCreature Creature, Container Corpse, PlayerMobile[] Players, double chancetodrop)
		{
			if (Utility.RandomDouble()<=chancetodrop)
			{
				var all = Enum.GetValues(typeof(SpectralBoneTypes));
				var key = new SpectralBone( this, SpectralBoneTypes.Head );
				key.OnlyVisibleTo = Players;				
				Corpse.DropItem( key );
			}
		}
		private void DropRandomBone( BaseCreature Creature, Container Corpse, PlayerMobile[] Players, double chancetodrop)
		{
			if (Utility.RandomDouble()<=chancetodrop)
			{
				var all = Enum.GetValues(typeof(SpectralBoneTypes));
				var key = new SpectralBone( this, (SpectralBoneTypes)all.GetValue(Utility.Random(all.Length)));
				key.OnlyVisibleTo = Players;				
				Corpse.DropItem( key );
			}
		}
		
		private class InternalTimer : Timer
		{
			CustomDungeon Dungeon;
			public InternalTimer(CustomDungeon dun) : base(TimeSpan.Zero,TimeSpan.FromSeconds(5))
			{
				Dungeon = dun;
			}
			protected override void OnTick ()
			{
				base.OnTick ();
				if (Core.Debug)
					Core.Pkg.LogInfoLine("Timer of \"{0}\" cycle.",Dungeon);
			}
		}

        protected override void OnDestroy()
        {
			if (timer!=null)
				timer.Stop();
			timer = null;
        }
		#region Stages
		public void ShowGump( PlayerMobile player )
		{
			new QuestGump( this , player );
		}
		public void StartFirstStageForPlayers(PlayerMobile leader, PlayerMobile[] players)
		{
			foreach(var player in players)
				if( player != leader)
				{
					ChangePlayerStageSet( new WishSpectreStageSet( this, player ));
				}
			ChangePlayerStageSet( new WishSpectreStageSet( this, leader ));
			ShowGump (leader);
		}
		#endregion
		#region serialization
		protected override void OnSerialize (GenericWriter writer)
		{
			writer.Write((ushort)0); //version
			
		}
		protected override void OnDeserialize (GenericReader reader)
		{
			var version = reader.ReadUShort();
			
			switch(version)
			{
			case 0:
				break;
			}
		}
		#endregion
    }	
}