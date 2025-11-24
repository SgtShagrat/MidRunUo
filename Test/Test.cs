using System;
using NUnit.Framework;
using Server;
using Server.Misc;
using Server.Spells;
using Server.Spells.First;
using Server.Spells.Sixth;
using Midgard.Engines.SkillSystem;
using Server.Mobiles;

namespace Test
{
	[TestFixture()]
	public class MobileTest
	{
		[SetUp]
		public void Init(){
			MapDefinitions.Configure(); // Avoids NPE on new Mobile()
			Assert.NotNull( Map.Internal.DefaultRegion );
			
			SkillCheck.Initialize();
			Assert.NotNull(Mobile.SkillCheckDirectLocationHandler);
			Midgard.Engines.SkillSystem.Core.InitCore();			
			Midgard.Engines.SkillSystem.Config.Instance = TestUtils.MockConfig();
		}
		
		[Test]
		public void CreateMobile()
		{
			Assert.NotNull( TestUtils.NewMobile() );
		}
				
		[Test]
		public void MagicResGainFromZero()
		{
			Mobile m = TestUtils.NewMobile();
			Assert.NotNull(m.Skills[ SkillName.MagicResist ]);
			
			m.Skills[ SkillName.MagicResist ].Base = 0;
			Assert.AreEqual(0, m.Skills[ SkillName.MagicResist ].BaseFixedPoint);
			
			MagerySpell ms = new MagicArrowSpell(m, null);
			ms.CheckResisted(m);
									
			Assert.AreNotEqual(0, m.Skills[ SkillName.MagicResist ].BaseFixedPoint);
		}
		
		[Test]
		public void OnCasterDamaged()
		{
			Mobile m = TestUtils.NewMobile();
			m.Skills[ SkillName.Magery ].Base = 100;
			MagerySpell ms = new EnergyBoltSpell(m, null);			
			ms.OnCasterDamaged(m, 10);
			Assert.AreEqual(0.25, oldChance(m, ms));
		}
		
		private double oldChance(Mobile mob, MagerySpell spell) {
			double diff = 50 + ((int) spell.Circle + 1) * 10;
			double wrestling = 100; // mob.Skills[ SkillName.Wrestling ].Value;
			return wrestling >= diff + 20 ? 
				0.99 :
					0.025 * (wrestling - diff + 20);
		}
		
	}
	
	public class TestUtils
	{
		public static Mobile NewMobile ()
		{
			Mobile mobile = new Mobile(Serial.Zero);
			mobile.DefaultMobileInit();
			return mobile; // Line 10063
		}
		
		public static IConfig MockConfig()
		{
			return new MockConfig();
		}
		
	}
	
	class MockConfig : IConfig {
		public bool IsEnabled
		{
			get { return true; }
			set {}
		}
	}
	
}

