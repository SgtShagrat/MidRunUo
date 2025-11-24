using System;
using Server;
namespace Midgard.Engines.BossSystem
{
	internal class PlayerRecord
	{
		public DateTime LastCompletition{get;private set;}
		public ushort CompletedCount{get;private set;}
		public PlayerRecord (GenericReader reader)
		{
			Deserialize( reader );
		}
		public PlayerRecord (DateTime lastCompletition, ushort completedCount)
		{
			LastCompletition = lastCompletition;
			CompletedCount = completedCount;
		}
		public void Serialize(GenericWriter writer)
		{
			writer.Write((ushort)0);//version
			
			writer.Write(CompletedCount);
			writer.WriteDeltaTime( LastCompletition );
		}
		public void Deserialize(GenericReader reader)
		{
			var version = reader.ReadUShort();
			
			switch(version)
			{
			case 0:
				CompletedCount = reader.ReadUShort();
				LastCompletition = reader.ReadDeltaTime();
				break;
			}
		}
	}
}

