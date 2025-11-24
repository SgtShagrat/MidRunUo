using Server;
using Server.Items;

namespace Midgard.Engines.OldCraftSystem
{
    public interface ISmeltable
    {
        bool Resmelt( Mobile from );
    }

    public interface IRepairable
    {
        bool Repair( Mobile from, BaseTool tool );
    }

    public interface IFortificable
    {
        void Fortify( Mobile from, Item combinedWith );

        bool IsFortified { get; }
        double FortifyArmor { get; }
    }
}