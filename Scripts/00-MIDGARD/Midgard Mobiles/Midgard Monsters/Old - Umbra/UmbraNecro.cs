using System;
using Server.Items;

namespace Server.Mobiles 
{ 
   public class UmbraNecro : BaseCreature, IUmbraFolk
   { 
      [Constructable] 
      public UmbraNecro() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 ) 
      { 
         	SpeechHue = Utility.RandomDyedHue(); 
         	Hue = 802; 

         	if ( this.Female = Utility.RandomBool() ) 
         		{ 
            	Body = 0x191; 
            	Name = NameList.RandomName( "female" ); 		            	
		        } 
         	else 
         		{ 
            	Body = 0x190; 
            	Name = NameList.RandomName( "male" );        
         		} 

           	Title = "the Umbra Necromancer"; 

	        SetStr( 155, 170 ); 
         	SetDex( 95, 135 ); 
         	SetInt( 130, 200 ); 

         	SetHits( 250, 300 );

         	SetDamage( 10, 23 ); 

         	SetSkill( SkillName.EvalInt, 95.0, 115.5 ); 
         	SetSkill( SkillName.Magery, 100.0, 120.5 ); 
         	SetSkill( SkillName.MagicResist, 175.0, 215.5 ); 
         	SetSkill( SkillName.Anatomy, 85.0, 95.5 ); 
         	SetSkill( SkillName.Macing, 95.0, 105.5 );
         	SetSkill( SkillName.Tactics, 95.0, 107.5 ); 
         	SetSkill( SkillName.Wrestling, 105.0, 117.5 ); 
         	SetSkill( SkillName.Meditation, 95.6, 105.4 ); 
         	
         	Fame = 0; 
         	Karma = -10000; 

         	VirtualArmor = 40; 

         

         	Item hair = new Item( Utility.RandomList( 0x203B, 0x2049, 0x2048, 0x203C ) ); 
         	hair.Hue = 1153; 
         	hair.Layer = Layer.Hair; 
         	hair.Movable = false; 
         	AddItem( hair );
         	
         	switch ( Utility.Random( 2 ))
         		{ 
				case 0:
				Item shroud = new HoodedShroudOfShadows();
				shroud.Movable = false;
				shroud.Hue = Utility.RandomList (1157, 1157, 1157, 1157, 1157, 1157, 1175, 1175, 1175, 1175, 1175, 1175, 1257);
				AddItem( shroud );
				break;

				case 1:
				BoneGloves gloves = new BoneGloves();
				gloves.Hue = Utility.RandomList (1157, 1157, 1157, 1157, 1157, 1157, 1175, 1175, 1175, 1175, 1175, 1175, 1257);
				gloves.LootType = LootType.Blessed;
				AddItem( gloves );

				Robe tunic = new Robe();
				tunic.Hue = Utility.RandomList (1157, 1157, 1157, 1157, 1157, 1157, 1175, 1175, 1175, 1175, 1175, 1175, 1257);
				tunic.LootType = LootType.Blessed;
				AddItem( tunic );

				BoneHelm helm = new BoneHelm();
				helm.Hue = Utility.RandomList (1157, 1157, 1157, 1157, 1157, 1157, 1175, 1175, 1175, 1175, 1175, 1175, 1257);
				helm.LootType = LootType.Blessed;
				AddItem( helm );
				
				Cloak cloak = new Cloak();
				cloak.Hue = Utility.RandomList (1157, 1157, 1157, 1157, 1157, 1157, 1175, 1175, 1175, 1175, 1175, 1175, 1257);
				cloak.LootType = LootType.Blessed;
				AddItem( cloak );				

				break;
				}

		Sandals sandals = new Sandals();
		sandals.Hue = Utility.RandomList (1157, 1157, 1157, 1157, 1157, 1157, 1175, 1175, 1175, 1175, 1175, 1175, 1257);
		sandals.LootType = LootType.Blessed;
		AddItem( sandals );
		
        PackGem(); 
        PackGold( 325, 375 ); 
        PackMagicItems( 1, 5 ); 
        PackSlayer(); 		
        
           				if ( 0.5 >= Utility.RandomDouble() )
						PackItem( new ExecutionersCap(3) );

	}

      	public override bool AlwaysMurderer{ get{ return true; } } 
//		public override bool BardImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Greater; } }

		public UmbraNecro( Serial serial ) : base( serial ) 
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

    public interface IUmbraFolk
    {
    }
}
