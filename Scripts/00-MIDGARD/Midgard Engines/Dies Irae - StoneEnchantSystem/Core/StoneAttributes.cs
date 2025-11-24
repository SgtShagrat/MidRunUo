/***************************************************************************
 *                               StoneAttributes
 *                            -------------------
 *   begin                : 27 gennaio, 2009
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using Server;

namespace Midgard.Engines.StoneEnchantSystem
{
    [Flags]
    public enum StoneAttributeFlag
    {
        Phoenix = 0x00000001,
        Flamming = 0x00000002,
        Electrical = 0x00000004,
        Cloud = 0x00000008,
        Explosion = 0x00000010,
        Lockmind = 0x00000020,
        Spectral = 0x00000040,
        Serpent = 0x00000080,
        Perforate = 0x00000100,
        Antimagical = 0x00000200,
        Vampirical = 0x00000400,
        Mammoth = 0x00000800,
        Sonical = 0x00001000,
    }

    [PropertyObject]
    public sealed class StoneAttributes : BaseMobileAttributes
    {
        public StoneAttributes( Mobile owner )
            : base( owner )
        {
        }

        public int this[ StoneAttributeFlag attribute ]
        {
            get { return GetValue( (int)attribute ); }
        }

        public override string ToString()
        {
            return "...";
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Phoenix { get { return this[ StoneAttributeFlag.Phoenix ]; } }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Flamming { get { return this[ StoneAttributeFlag.Flamming ]; } }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Electrical { get { return this[ StoneAttributeFlag.Electrical ]; } }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Cloud { get { return this[ StoneAttributeFlag.Cloud ]; } }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Explosion { get { return this[ StoneAttributeFlag.Explosion ]; } }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Lockmind { get { return this[ StoneAttributeFlag.Lockmind ]; } }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Spectral { get { return this[ StoneAttributeFlag.Spectral ]; } }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Serpent { get { return this[ StoneAttributeFlag.Serpent ]; } }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Perforate { get { return this[ StoneAttributeFlag.Perforate ]; } }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Antimagical { get { return this[ StoneAttributeFlag.Antimagical ]; } }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Vampirical { get { return this[ StoneAttributeFlag.Vampirical ]; } }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Mammoth { get { return this[ StoneAttributeFlag.Mammoth ]; } }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Sonical { get { return this[ StoneAttributeFlag.Sonical ]; } }

        public override void ComputeValue( int bitmask )
        {
            if( Owner == null )
                return;

            List<Item> items = Owner.Items;
            int value = 0;
            StoneAttributeFlag flag = (StoneAttributeFlag)bitmask;

            foreach( Item item in items )
            {
                StoneEnchantItem state = StoneEnchantItem.Find( item );
                if( state == null || state.Definition.StoneFlag != flag )
                    continue;

                if( Config.Debug )
                    Config.Pkg.LogInfoLine( "Found IStoneEnchantItem on {0}: value {1}.", Owner.Name ?? "", state.Level );

                value += state.Level;
            }

            if( Config.Debug )
                Config.Pkg.LogInfoLine( "Debug: total {0} power: {1}.", flag, value );

            SetValue( bitmask, value );
        }

        /*
        public static StoneTypes GetStoneTypeByFlag( StoneAttributeFlag flag )
        {
            switch( flag )
            {
                case StoneAttributeFlag.Phoenix:
                    return StoneTypes.Phoenix;
                case StoneAttributeFlag.Flamming:
                    return StoneTypes.Flamming;
                case StoneAttributeFlag.Electrical:
                    return StoneTypes.Electrical;
                case StoneAttributeFlag.Cloud:
                    return StoneTypes.Cloud;
                case StoneAttributeFlag.Explosion:
                    return StoneTypes.Explosion;
                case StoneAttributeFlag.Lockmind:
                    return StoneTypes.Lockmind;
                case StoneAttributeFlag.Spectral:
                    return StoneTypes.Spectral;
                case StoneAttributeFlag.Serpent:
                    return StoneTypes.Serpent;
                case StoneAttributeFlag.Perforate:
                    return StoneTypes.Perforate;
                case StoneAttributeFlag.Antimagical:
                    return StoneTypes.Antimagical;
                case StoneAttributeFlag.Vampirical:
                    return StoneTypes.Vampirical;
                case StoneAttributeFlag.Mammoth:
                    return StoneTypes.Mammoth;
                case StoneAttributeFlag.Sonical:
                    return StoneTypes.Sonical;

                default:
                    return StoneTypes.Phoenix;
            }
        }
        */
        /*
        public static StoneAttributeFlag GetFlagFromType( StoneTypes type )
        {
            switch( type )
            {
                case StoneTypes.Phoenix:
                    return StoneAttributeFlag.Phoenix;
                case StoneTypes.Flamming:
                    return StoneAttributeFlag.Flamming;
                case StoneTypes.Electrical:
                    return StoneAttributeFlag.Electrical;
                case StoneTypes.Cloud:
                    return StoneAttributeFlag.Cloud;
                case StoneTypes.Explosion:
                    return StoneAttributeFlag.Explosion;
                case StoneTypes.Lockmind:
                    return StoneAttributeFlag.Lockmind;
                case StoneTypes.Spectral:
                    return StoneAttributeFlag.Spectral;
                case StoneTypes.Serpent:
                    return StoneAttributeFlag.Serpent;
                case StoneTypes.Perforate:
                    return StoneAttributeFlag.Perforate;
                case StoneTypes.Antimagical:
                    return StoneAttributeFlag.Antimagical;
                case StoneTypes.Vampirical:
                    return StoneAttributeFlag.Vampirical;
                case StoneTypes.Mammoth:
                    return StoneAttributeFlag.Mammoth;
                case StoneTypes.Sonical:
                    return StoneAttributeFlag.Sonical;

                default:
                    return StoneAttributeFlag.Phoenix;
            }
        }
        */
    }
}