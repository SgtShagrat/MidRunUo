/***************************************************************************
 *                               BaseItemDisplayAttributes.cs
 *
 *   begin                : 30 luglio 2011
 *   author               :	Dies Irae
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae		
 *
 ***************************************************************************/

using Server;
using Server.Items;

namespace Midgard
{
    [PropertyObject]
    public abstract class BaseItemDisplayAttributes
    {
        public virtual Item Owner { get; private set; }

        protected BaseItemDisplayAttributes( Item owner )
        {
            Owner = owner;
        }

        public override string ToString()
        {
            return "...";
        }
    }

    public class BaseWeaponAttribute : BaseItemDisplayAttributes
    {
        public new BaseWeapon Owner { get; private set; }

        public BaseWeaponAttribute( Item owner )
            : base( owner )
        {
            if( owner is BaseWeapon )
                Owner = (BaseWeapon)owner;
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool Debug
        {
            get { return Owner.Debug; }
            set { Owner.Debug = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool CheckForAttackSkillOnSwing
        {
            get { return Owner.CheckForAttackSkillOnSwing; }
        }

        #region classes
        [CommandProperty( AccessLevel.GameMaster )]
        public bool IsXmlHolyWeapon
        {
            get { return Owner.IsXmlHolyWeapon; }
        }
        #endregion

        #region hit chance
        [CommandProperty( AccessLevel.GameMaster )]
        public double HitRateBonus
        {
            get { return Owner.HitRateBonus; }
            set { Owner.HitRateBonus = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public double EvasionBonus
        {
            get { return Owner.EvasionBonus; }
            set { Owner.EvasionBonus = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int HitRateMaterialBonus
        {
            get { return Owner.OldMaterialWrestHitBonus; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int EvasionMaterialBonus
        {
            get { return Owner.OldMaterialWrestEvaBonus; }
        }
        #endregion

        #region damage
        [CommandProperty( AccessLevel.GameMaster )]
        public string Dice
        {
            get { return Owner.Dice; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int MinDiceDmg
        {
            get { return Owner.MinDiceDamage; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int MaxDiceDmg
        {
            get { return Owner.MaxDiceDamage; }
        }
        #endregion
    }
}