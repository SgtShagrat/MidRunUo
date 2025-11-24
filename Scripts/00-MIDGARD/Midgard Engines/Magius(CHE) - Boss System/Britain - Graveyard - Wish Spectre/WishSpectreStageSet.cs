using System;
using Server.Mobiles;
using System.Collections.Generic;
using Server;
namespace Midgard.Engines.BossSystem.WishSpectre
{
	internal class WishSpectreStageSet : DungeonStageSet
	{
		public WishSpectreStageSet (Dungeon dungeon,PlayerMobile player) : base(dungeon,player)
		{
		}
		public SpectralBone GetBone(SpectralBoneTypes type)
		{
			foreach(var item in Player.Items)
			{
				var bo = item as SpectralBone;			
				if ( bo != null && bo.Is(type))
					return bo;
			}
			return null;
		}
		public bool HasBone (SpectralBoneTypes type)
		{			
			return GetBone(type)!=null;
		}
		public int Bones
		{
			get
			{
				var tot = new List<Type>();
				foreach(var item in Player.Items)
				{
					var bo = item as SpectralBone;
					if ( bo != null && !tot.Contains(bo.GetType()))
					{
						tot.Add(bo.GetType());
					}
				}
				return tot.Count;
			}
		}
		#region serialization
		public WishSpectreStageSet (Dungeon dungeon, PlayerMobile player, GenericReader reader) : base(dungeon,player,reader)
		{
		}
		
		public override void Serialize (GenericWriter writer)
		{
			writer.Write((ushort)0); //version
		}
		protected override void Deserialize (GenericReader reader)
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

