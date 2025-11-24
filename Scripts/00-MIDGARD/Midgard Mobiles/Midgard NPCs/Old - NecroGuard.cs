using System;
using Server.Items;

namespace Server.Mobiles 
{ 
   public class NecroGuard : BaseCreature 
   { 
      [Constructable] 
      public NecroGuard() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 ) 
      { 
         	SpeechHue = Utility.RandomDyedHue(); 
         	Hue = 802; 

         	if ( this.Female = Utility.RandomBool() ) 
         		{ 
            	Body = 0x191; 
            	Name = NameList.RandomName( "female" ); 
            	Title = "the Necromancer Guard"; 
		            	
		         } 
         	else 
         		{ 
            	Body = 0x190; 
            	Name = NameList.RandomName( "male" ); 
            	Title = "the Necromancer Guard"; 
         
         		} 

         	SetStr( 1550, 1700 ); 
         	SetDex( 1550, 1750 ); 
         	SetInt( 1300, 2000 ); 

         	SetHits( 4000, 5000 ); 

         	SetDamage( 300, 350 ); 

         	SetSkill( SkillName.EvalInt, 120 ); 
			SetSkill( SkillName.MagicResist,215.5 ); 
         	SetSkill( SkillName.Anatomy, 120 ); 
         	SetSkill( SkillName.Tactics, 120 ); 
         	SetSkill( SkillName.Wrestling, 120 ); 
         	SetSkill( SkillName.Meditation, 120); 
			SetSkill( SkillName.Magery, 400); 
         	Fame = 10000; 
         	Karma = -10000; 

         	VirtualArmor = 70; 

         

         	Item hair = new Item( Utility.RandomList( 0x203B, 0x2049, 0x2048, 0x203C ) ); 
         	hair.Hue = 1153; 
         	hair.Layer = Layer.Hair; 
         	hair.Movable = false; 
         	AddItem( hair ); 
	
		Item shroud = new HoodedShroudOfShadows();

		shroud.Movable = false;

		AddItem( shroud );

		Scythe weapon = new Scythe();

		weapon.Skill = SkillName.Wrestling;
		weapon.Hue = 38;
		weapon.Movable = false;

		AddItem( weapon );

		new Horse().Rider=this;
	}

		public override bool OnBeforeDeath()
		{
			IMount mount = this.Mount;

			if ( mount != null )
				mount.Rider = null;

			if ( mount is Mobile )
				((Mobile)mount).Delete();

			return base.OnBeforeDeath();
		}

       


      	public override bool AlwaysMurderer{ get{ return true; } } 
		public override bool BardImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }

		public NecroGuard( Serial serial ) : base( serial ) 
		{ 
		} 
      
		public override void Serialize( GenericWriter writer ) 
		{ 
			base.Serialize( writer ); 

			writer.Write( (int) 0 ); // version 
		} 

		public override void Deserialize( GenericReader reader ) 
		{ 
			base.Deserialize( reader ); 

			int version = reader.ReadInt(); 
		} 
	} 
}
