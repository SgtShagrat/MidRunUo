using Server;

namespace Midgard.Engines.StoneEnchantSystem
{
    public interface IStoneEnchantItem
    {
        StoneEnchantItem StoneEnchantItemState { get; set; }
        SkillName[] SkillsRequiredToEnchant { get; }
    }

    [PropertyObject]
    public class StoneEnchantItem
    {
        public static SkillName[] SkillsRequiredToEnchantMetal = new SkillName[]
                                                                 {
                                                                     SkillName.ItemID, SkillName.ArmsLore, SkillName.Blacksmith
                                                                 };

        public static SkillName[] SkillsRequiredToEnchantLeather = new SkillName[]
                                                                   {
                                                                       SkillName.ItemID, SkillName.ArmsLore, SkillName.Tailoring
                                                                   };

        public static SkillName[] SkillsRequiredToEnchantWood = new SkillName[]
                                                                {
                                                                    SkillName.ItemID, SkillName.ArmsLore, SkillName.Carpentry
                                                                };

        public static SkillName[] SkillsRequiredToEnchantWoodenBows = new SkillName[]
                                                                      {
                                                                          SkillName.ItemID, SkillName.ArmsLore, SkillName.Fletching
                                                                      };

        public StoneEnchantItem( Item item, StoneTypes stoneType, int level, int hue )
        {
            Item = item;
            StoneType = stoneType;
            Level = level;
            Originalhue = hue;
        }

        public StoneEnchantItem( GenericReader reader )
        {
            int version = reader.ReadEncodedInt();

            switch( version )
            {
                case 1:
                    goto case 0;
                case 0:
                    {
                        Item = reader.ReadItem();
                        StoneType = (StoneTypes)reader.ReadInt();
                        Level = reader.ReadInt();
                        Originalhue = reader.ReadInt();
                        break;
                    }
            }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator )]
        public Item Item { get; private set; }

        private StoneTypes m_StoneType;

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator )]
        public StoneTypes StoneType
        {
            get { return m_StoneType; }
            set
            {
                if( value != m_StoneType )
                {
                    m_StoneType = value;

                    if( Item != null )
                        Item.Hue = StoneEnchantHelper.GetHueFromStoneType( m_StoneType );
                }
            }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator )]
        public int Level { get; private set; }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator )]
        public int Originalhue { get; private set; }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator )]
        public StoneDefinition Definition
        {
            get { return StoneDefinition.GetFromIndex( (int)StoneType - 1 ); }
        }

        [CommandProperty( AccessLevel.GameMaster, AccessLevel.Administrator )]
        public bool HasExpired
        {
            get { return Item == null || Item.Deleted; }
        }

        public void Attach()
        {
            if( Item is IStoneEnchantItem )
                ( (IStoneEnchantItem)Item ).StoneEnchantItemState = this;
        }

        public void Detach()
        {
            if( Item is IStoneEnchantItem )
                ( (IStoneEnchantItem)Item ).StoneEnchantItemState = null;
        }

        public void Serialize( GenericWriter writer )
        {
            writer.WriteEncodedInt( 1 );

            writer.Write( Item );
            writer.Write( (int)StoneType );
            writer.Write( Level );
            writer.Write( Originalhue );
        }

        public static StoneEnchantItem Find( Item item )
        {
            if( item is IStoneEnchantItem )
            {
                StoneEnchantItem state = ( (IStoneEnchantItem)item ).StoneEnchantItemState;

                if( state != null && state.HasExpired )
                {
                    state.Detach();
                    state = null;
                }

                return state;
            }

            return null;
        }

        public static Item Imbue( Item item, StoneTypes type, int level )
        {
            if( !( item is IStoneEnchantItem ) )
                return item;

            StoneEnchantItem state = Find( item );

            if( state == null )
            {
                state = new StoneEnchantItem( item, type, level, item.Hue );
                state.Attach();

                item.Hue = StoneEnchantHelper.GetHueFromStoneType( type );
            }

            return item;
        }

        private static readonly string[] m_StringLevels = new string[]
                                                          {
                                                              "none", "low", "medium", "high", "supreme", "defiant"
                                                          };

        public string StringLevel
        {
            get { return m_StringLevels[ Level ]; }
        }

        public void DisplayInfo( Mobile to )
        {
            if( Definition != null && Definition.Suffix != null )
            {
                if( Level > 0 && Level <= EnchantStone.MaxStoneLevel )
                    to.SendMessage( "This is magic with {0} {1} power on it (level {2}).", StringLevel, Definition.Suffix, Level );
                else
                    to.SendMessage( "This is magic with {0} power on it.", Definition.Suffix );
            }
        }

        public void ExplainResist( Mobile to )
        {
            if( Definition != null && Definition.Suffix != null )
                to.SendMessage( "Your {0} magical power is now {1}%!", Definition.Suffix, to.GetStonePower( (int)Definition.StoneFlag ) );
        }

        public override string ToString()
        {
            return "...";
        }

        public static int GetValue( Item item, CustomResType type )
        {
            StoneEnchantItem state = Find( item );
            if( state == null || state.Definition.ResistStoneType != type )
                return 0;

            if( Config.Debug )
                Config.Pkg.LogInfoLine( "Found IStoneEnchantItem on {0}: value {1}.", item.GetType().Name, state.Level );

            return state.Level;
        }

        public void Invalidate( Mobile from, bool message )
        {
            from.UpdateStonePower( (int)Definition.StoneFlag );

            if( message )
                ExplainResist( from );
        }

        public virtual void OnHit( Mobile attacker, Mobile defender, Item hitFrom, Item hitTo, int baseDamage )
        {
            if( Definition != null )
                Definition.OnHit( attacker, defender, hitFrom, hitTo, baseDamage );
        }
    }
}