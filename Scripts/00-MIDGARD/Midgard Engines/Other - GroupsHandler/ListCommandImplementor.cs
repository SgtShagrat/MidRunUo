using Server;
using Server.Commands.Generic;

namespace Midgard.Engines.GroupsHandler
{
    public class ListCommandImplementor : BaseCommandImplementor
    {
        public ListCommandImplementor()
        {
            SupportRequirement = CommandSupport.Global;
            SupportsConditionals = true;
            AccessLevel = AccessLevel.GameMaster;
            Description = "Invokes the command on all appropriate items selected from HandleGroups gump." +
                          "Optional condition arguments can further restrict the set of items.";
        }
    }
}