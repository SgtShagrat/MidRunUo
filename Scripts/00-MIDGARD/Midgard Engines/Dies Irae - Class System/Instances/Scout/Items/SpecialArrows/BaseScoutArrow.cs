using System;

using Server;
using Server.Engines.Craft;
using Server.Items;

using Midgard.Engines.Classes;

namespace Midgard.Items
{
    public abstract class BaseScoutArrow : Item, ISpecialAmmo, ICraftable
    {
        private static readonly AmnoEntry[] m_ScoutAmmoTypes = new AmnoEntry[]
		{
			new AmnoEntry( typeof( IncendiaryArrow ), 3904, 1654, "incendiary arrows", 30.0, 5.0, 0x3B64, 0 ),
			new AmnoEntry( typeof( AcidArrow ), 3904, 2130, "acid arrows", 50.0, 5.0, 0xF42, 2130 ),
			new AmnoEntry( typeof( DisarmingArrow ), 3904, 2199, "disarming arrows", 70.0, 10.0, 0xF42, 2199 ),
			new AmnoEntry( typeof( DismountingArrow ), 3904, 1784, "dismounting arrows", 70.0, 10.0, 0xF42, 1784 ),
			new AmnoEntry( typeof( StuningArrow ), 3904, 1908, "stuning arrows", 90.0, 10.0, 0xF42, 1908 )
		};

        public override double DefaultWeight { get { return 0.1; } }

        public static AmnoEntry[] ScoutAmmoTypes { get { return m_ScoutAmmoTypes; } }

		public BaseScoutArrow() : this( 1 )
        {
        }

		public BaseScoutArrow( int amount ) : base( 0xF3F )
        {
            Stackable = true;
            Amount = amount;

            CrafterSkill = 0.0;
        }

        public virtual void OnHit( BaseRanged baseRanged, Mobile attacker, Mobile defender, double damageBonus )
        {
            if( !ClassSystem.IsScout( attacker ) )
            {
                attacker.SendMessage( ( attacker.Language == "ITA" ? "Questa freccia non sembra efficace nelle tue mani.": "This arrow isn't so effective in your hands." ) );
                return;
            }

            const double frecciatrovata = 0.4;

            if( !defender.Player && ( defender.Body.IsAnimal || defender.Body.IsMonster ) && frecciatrovata >= Utility.RandomDouble() )
            {
                Item item = (Item)Activator.CreateInstance( GetType() );
                BaseScoutArrow freccia = (BaseScoutArrow)item;
                freccia.Amount = 1;
                freccia.CrafterSkill = CrafterSkill;

                defender.AddToBackpack( freccia );
            }

            Consume();
        }

        public virtual void OnMiss( BaseRanged baseRanged, Mobile attacker, Mobile defender )
        {
            double tiro = Utility.RandomDouble();
            const double frecciatrovata = 0.4;
            const double frecciafallita = 0.2;

            if( tiro < frecciafallita )
            {
                //fallimento critico
                attacker.SendMessage( ( attacker.Language == "ITA" ? "Che colpo orribile!": "What an awful shot!" ) );
                int range = tiro < 0.05 ? ( tiro < 0.025 ? 4 : 3 ) : 2;
                foreach( Mobile mobile in defender.GetMobilesInRange( range ) )
                {
                    if( !attacker.CanBeHarmful( mobile ) || !attacker.InLOS( mobile ) )
                        continue;

					if (mobile != defender && mobile != attacker )
					{
                    baseRanged.OnHit( attacker, mobile, this );
                    attacker.SendMessage( ( attacker.Language == "ITA" ? "Hai colpito {0}!": "You hit {0}!" ), mobile.Name );

                    if( mobile.Player )
                    {
                        mobile.SendMessage( ( attacker.Language == "ITA" ? "Sei stato colpito per errore!": "You've been mistaken hit!" ) );
                        attacker.DoHarmful( mobile, true );
                        if( mobile.Combatant == null )
                            mobile.Combatant = attacker;
                    }
                    break;
                }
            }
			}
            else if( tiro < frecciatrovata )
            {
                Item item = (Item)Activator.CreateInstance( GetType() );
                BaseScoutArrow freccia = (BaseScoutArrow)item;
                freccia.Amount = 1;
                freccia.CrafterSkill = CrafterSkill;

                double t = Utility.RandomDouble()*2+0.5;

                Point3D target = new Point3D( attacker.X + (int)( ( defender.X + Utility.RandomMinMax( -1, 1 ) - attacker.X )*t ), attacker.Y + (int)( ( defender.Y + Utility.RandomMinMax( -1, 1 ) -attacker.Y )*t ), defender.Z );
                
                if( attacker.InLOS( target ) )
                    freccia.MoveToWorld( target, defender.Map );
                else
                    freccia.MoveToWorld( new Point3D( defender.X, defender.Y, defender.Z ), defender.Map );
            }

            Consume();
        }

        public virtual bool OnFired( BaseRanged bow, Mobile attacker, Mobile defender )
        {
            attacker.MovingEffect( defender, bow.GetEntryAnimIdByType( attacker, bow.AmmoType ), 18, 1, false, false, bow.GetEntryHueByType( attacker, bow.AmmoType ), 0 );
            return true;
        }

        public virtual bool CheckSkill()
        {
            double chance = ( CrafterSkill / 200.0 ) + 0.25; // 0.25 -> 0.75

            if( Parent is Mobile && ( (Mobile)Parent ).PlayerDebug )
                ( (Mobile)Parent ).SendMessage( "You had {0:F1}% of success with this arrow.", chance * 100 );

            return Utility.RandomDouble() <= chance;
        }

        public override void OnAfterDuped( Item newItem )
        {
            //if( newItem is BaseScoutArrow )
            //	( (BaseScoutArrow)newItem ).CrafterSkill = CrafterSkill;

            if( CrafterSkill == 0 )
                CrafterSkill = ( (BaseScoutArrow)newItem ).CrafterSkill;
            else if( ( (BaseScoutArrow)newItem ).CrafterSkill == 0 )
                ( (BaseScoutArrow)newItem ).CrafterSkill = CrafterSkill;

			//base.OnAfterDuped( newItem );
        }

        #region serialization
		public BaseScoutArrow( Serial serial ) : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( 0 ); // version

            writer.Write( CrafterSkill );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            CrafterSkill = reader.ReadDouble();
        }
        #endregion

        #region ICraftable Members

        public int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes,
                            BaseTool tool, CraftItem craftItem, int resHue )
        {
            if( makersMark )
                Crafter = from;

            PlayerConstructed = true;
            CrafterSkill = from.Skills[ craftSystem.MainSkill ].Value;

            return quality;
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool PlayerConstructed { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Crafter { get; set; }

        [CommandProperty( AccessLevel.GameMaster )]
        public double CrafterSkill { get; set; }

        #endregion
    }
}