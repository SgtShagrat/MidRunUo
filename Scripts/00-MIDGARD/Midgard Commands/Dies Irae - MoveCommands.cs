/***************************************************************************
 *                                  MoveCommands.cs
 *                            		---------------
 *  begin                	: Dicembre, 2007
 *  version					: 2.0 **VERSIONE PER RUNUO 2.0**
 *  copyright            	: Midgard Uo Shard - Matteo Visintin
 *  email                	: tocasia@alice.it
 *
 ***************************************************************************/

/***************************************************************************
 *  Info:
 * 			[dump sposta tutti gli oggetti da un contenitore ad un altro
 * 			[moveitems <exact> sposta tutti gli oggetti simili al tipo scelto
 * 				da un contenitore ad un altro. Se viene specificato il parametro
 * 				exact vengono spostati solo gli items di quel tipo.
 *
 ***************************************************************************/

using System;
using Server;
using Server.Commands;
using Server.Gumps;
using Server.Items;
using Server.Multis;
using Server.Targeting;
using Server.Spells;

namespace Midgard.Commands
{
    public class MoveCommands
    {
        #region registration
        public static void Initialize()
        {
            CommandSystem.Register( "SvuotaContenitore", AccessLevel.Player, new CommandEventHandler( Dump_OnCommand ) );
            CommandSystem.Register( "MuoviOggetti", AccessLevel.Player, new CommandEventHandler( MoveItems_OnCommand ) );
        }
        #endregion

        #region callbacks
        [Usage( "SvuotaContenitore" )]
        [Description( "Sposta gli oggetti da un contenitore ad un altro." )]
        public static void Dump_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;
            if( from == null || from.Deleted )
                return;

            if( !CanUseMoveCommands( from, true ) )
                return;

            if( e.Length == 0 )
            {
                from.SendMessage( "Seleziona un contenitore dal quale vuoi spostare gli oggetti" );
                from.Target = new DumpFromTarget( from );
            }
            else
            {
                from.SendMessage( "Command Use: [SvuotaContenitore" );
            }
        }

        [Usage( "MuoviOggetti <esatti>" )]
        [Description( "Muove gli oggetti di un contenitore in un altro." )]
        public static void MoveItems_OnCommand( CommandEventArgs e )
        {
            Mobile from = e.Mobile;

            if( from == null || from.Deleted )
                return;

            if( !CanUseMoveCommands( from, true ) )
                return;

            if( e.Length == 0 )
            {
                from.Target = new MoveItemsFromTarget( from, false );
                from.SendMessage( "Seleziona il tipo di oggetto che vuoi spostare." );
            }
            else if( e.Length == 1 )
            {
                if( Utility.InsensitiveCompare( e.GetString( 0 ), "esatti" ) == 0 )
                {
                    from.Target = new MoveItemsFromTarget( from, true );
                    from.SendMessage( "Seleziona il tipo di oggetto *esatto* che vuoi spostare." );
                }
                else
                {
                    from.SendGump( new NoticeGump( 1060637, 30720,
                        "MuoviOggetti permette di spostare degli oggetti di un tipo specifico da un contenitore ad un altro.<br>" +
                        "Per esempio permette di spostare tutti i rotoli di lana dal tuo zaino principale ad un altro.<br>" +
                        "Per fare cio', digita [MuoviOggetti, scegli un rotolo di lana nel tuo zaino principale e, successivamente," +
                        "seleziona lo zaino dove vuoi spostarli.<br>" +
                        "Il contenitore di destinazione normalmente e' posizionato all'esterno del tuo zaino, al suolo.<br>" +
                        "Se digiti <em>esatti</em> dopo aver digitato [MuoviOggetti il comando muovera' esattamente il tipo di oggetti" +
                        "corrispondente all'oggetto selezionato. Ad esempio scegliendo un arco yumi con [MuoviOggetti verranno spostati" +
                        "tutti gli archi nello zaino. Digitando [MuoviOggetti esatti , invece, verranno spostati solo gli archi yumi.",
                        0xFFC000, 420, 280, new NoticeGumpCallback( CloseNotice_Callback ), null ) );
                }
            }
            else
                from.SendMessage( "Command Use: [Dump" );
        }

        private static void CloseNotice_Callback( Mobile from, object state )
        {
        }
        #endregion

        public static bool CanUseMoveCommands( Mobile from, bool sendMessage )
        {
            if( from == null || from.Deleted )
                return false;

            if( from.AccessLevel > AccessLevel.Seer )
                return true;

            string state = string.Empty;

            if( !from.Alive )
                state = "morto";
            else if( from.Hidden )
                state = "nascosto";
            else if( from.Criminal )
                state = "criminale";
            else if( from.Paralyzed )
                state = "paralizzato";
            else if( from.Frozen )
                state = "paralizzato";
            else if( SpellHelper.CheckCombat( from ) )
                state = "in un combattimento";
            else if( Server.Multis.BaseHouse.FindHouseAt( from ) == null )
                state = "fuori da una casa";

            if( state.Length > 0 && sendMessage )
                from.SendMessage( "Non puoi usare questo comando mentre sei {0}", state );

            return state.Length == 0;
        }

        #region dump members
        private static Type[] m_NotValidContainersToDumpFrom = new Type[]
		{
			typeof(QuestHolder),
			typeof(BagOfSending),
			typeof(Corpse)
		};

        public static bool CannotBeChoosenAsDumpSource( Item i )
        {
            return Array.LastIndexOf( m_NotValidContainersToDumpFrom, i.GetType() ) > -1;
        }

        private static Type[] m_NotValidContainersToDumpTo = new Type[]
		{
			typeof(QuestHolder),
			typeof(BagOfSending),
			typeof(Corpse)
		};

        public static bool CannotBeChoosenAsDumpTarget( Item i )
        {
            return Array.LastIndexOf( m_NotValidContainersToDumpTo, i.GetType() ) > -1;
        }

        private static Type[] m_NotValidItemsToDump = new Type[]
		{
			typeof(QuestHolder),
		};

        public static bool CannotBeDumped( Item i )
        {
            if( i.QuestItem || !i.Movable )
                return true;

            return Array.LastIndexOf( m_NotValidItemsToDump, i.GetType() ) > -1;
        }
        #endregion

        #region dump targets
        private class DumpFromTarget : Target
        {
            public DumpFromTarget( Mobile from )
                : base( -1, true, TargetFlags.None )
            {
            }

            protected override void OnTarget( Mobile from, object o )
            {
                if( from == null || from.Deleted )
                    return;

                if( !( o is Item ) || CannotBeChoosenAsDumpSource( (Item)o ) )
                {
                    from.SendMessage( "Non puo' essere selezionato per lo spostamento." );
                    return;
                }

                if( o is Container )
                {
                    Container cont = o as Container;
                    Container pack = from.Backpack;

                    if( pack == null || pack.Deleted )
                        return;

                    if( cont is LockableContainer && ( (LockableContainer)cont ).Locked )
                        from.SendMessage( "Non puoi spostare oggetti da un contenitore chiuso a chiave." );
                    else if( !cont.IsAccessibleTo( from ) )
                        from.SendMessage( "Non e' accessibile." );
                    else if( cont.IsChildOf( pack ) || cont == pack )
                    {
                        from.SendMessage( "Seleziona il contenitore dove scaricare gli oggetti" );
                        from.Target = new DumpToTarget( from, cont );
                    }
                    else
                        from.SendMessage( "Il contenitore dal quale scaricare deve essere nel tuo zaino o deve essere il tuo zaino." );
                }
                else
                {
                    from.SendMessage( "Non e' un contenitore valido." );
                }
            }
        }

        private class DumpToTarget : Target
        {
            private Container m_Container;

            public DumpToTarget( Mobile from, Container container )
                : base( -1, true, TargetFlags.None )
            {
                m_Container = container;
            }

            protected override void OnTarget( Mobile from, object o )
            {
                if( from == null || from.Deleted )
                    return;

                if( !( o is Item ) || CannotBeChoosenAsDumpTarget( (Item)o ) )
                {
                    from.SendMessage( "Non puo' essere scelto per lo spostamento di oggetti." );
                    return;
                }

                if( o is Container )
                {
                    Container cont = o as Container;

                    if( cont == m_Container )
                        from.SendMessage( "Il contenitore scelto deve essere diverso da quello di partenza." );
                    else if( cont.IsChildOf( m_Container ) )
                        from.SendMessage( "Non puoi scegliere un contenitore contenuto in quello di partenza." );
                    else if( cont is LockableContainer && ( (LockableContainer)cont ).Locked )
                        from.SendMessage( "Non puoi muovere oggetti in un contenitore chiuso a chiave." );
                    else if( !cont.IsAccessibleTo( from ) )
                        from.SendMessage( "Non e' accessibile." );
                    else if( cont.Parent != null && cont.Parent is Mobile )
                        from.SendMessage( "Non e' accessibile." );
                    else
                    {
                        Item[] items = m_Container.FindItemsByType( typeof( Item ), true );

                        bool hasBeenDropped = true;

                        for( int i = 0; i < items.Length && hasBeenDropped; i++ )
                        {
                            Item item = items[ i ];

                            if( item == null || item.Deleted || CannotBeDumped( item ) )
                                continue;

                            if( item.Movable && item.IsAccessibleTo( from ) )
                                hasBeenDropped = cont.TryDropItem( from, item, false );

                            if( !hasBeenDropped )
                                from.SendMessage( "Il contenitore e' pieno." );
                        }
                    }
                }
                else
                {
                    from.SendMessage( "Non e' un contenitore valido." );
                }
            }
        }
        #endregion

        #region moveItems members
        public static bool InOwnHouse( Mobile from )
        {
            BaseHouse house = BaseHouse.FindHouseAt( from );
            return ( house != null && house.IsOwner( from ) );
        }

        private static Type[] m_NotValidContainersToMoveFrom = new Type[]
		{
			typeof(QuestHolder),
			typeof(BagOfSending),
			typeof(Corpse)
		};

        public static bool CannotBeChoosenAsMoveSource( Item i )
        {
            return Array.LastIndexOf( m_NotValidContainersToMoveFrom, i.GetType() ) > -1;
        }

        private static Type[] m_NotValidContainersToMoveTo = new Type[]
		{
			typeof(QuestHolder),
			typeof(BagOfSending),
			typeof(Corpse)
		};

        public static bool CannotBeChoosenAsMoveTarget( Item i )
        {
            return Array.LastIndexOf( m_NotValidContainersToMoveTo, i.GetType() ) > -1;
        }

        private static Type[] m_NotValidItemsToMove = new Type[]
		{
			typeof(QuestHolder),
		};

        public static bool CannotBeMoved( Item i )
        {
            if( i.QuestItem || !i.Movable )
                return true;

            return Array.LastIndexOf( m_NotValidItemsToMove, i.GetType() ) > -1;
        }
        #endregion

        #region moveItems targets
        private class MoveItemsFromTarget : Target
        {
            private bool m_ExactType;
            // private Type m_Type;

            public MoveItemsFromTarget( Mobile from, bool exactType )
                : base( -1, true, TargetFlags.None )
            {
                m_ExactType = exactType;
            }

            protected override void OnTarget( Mobile from, object o )
            {
                if( o is Container || o is Mobile )
                {
                    from.SendMessage( "Puoi muovere solo oggetti e NON contenitori." );
                    return;
                }

                Type itemType = o.GetType();
                Type baseType = itemType.BaseType;

                Item item = o as Item;
                if( item == null || item.Deleted )
                    return;

                Container parentItem = (Container)item.Parent;

                if( parentItem == null )
                    from.SendMessage( "Questo comando serve per muovere oggetti da un contenitore ad un altro. Seleziona un oggetto in un contenitore." );
                else if( CannotBeChoosenAsMoveSource( parentItem ) )
                    from.SendMessage( "Non e' un oggetto valido." );
                else if( !parentItem.IsChildOf( from.Backpack ) && parentItem != from.Backpack )
                {
                    if( !InOwnHouse( from ) || !BaseHouse.CheckAccessible( from, parentItem ) )
                        from.SendMessage( "Puoi selezionare solo oggetti nel tuo zaino, in un suo sotto-contenitore o nella tua casa." );
                }
                else if( !parentItem.IsAccessibleTo( from ) )
                    from.SendMessage( "Non e' accessibile." );
                else
                {
                    if( baseType == typeof( Item ) || m_ExactType )
                        baseType = o.GetType();

                    from.SendMessage( "Ora seleziona il contenitore dove vuoi spostare gli oggetti." );
                    from.Target = new MoveItemsToTarget( from, parentItem, baseType );
                }
            }
        }

        private class MoveItemsToTarget : Target
        {
            private Container m_ContainerFrom;
            private Type m_Type;

            public MoveItemsToTarget( Mobile from, Container container, Type type )
                : base( -1, true, TargetFlags.None )
            {
                m_Type = type;
                m_ContainerFrom = container;
            }

            protected override void OnTarget( Mobile from, object o )
            {
                if( from == null || from.Deleted )
                    return;

                if( !( o is Item ) || CannotBeChoosenAsMoveTarget( (Item)o ) )
                {
                    from.SendMessage( "Non e' un oggetto valido." );
                    return;
                }

                if( o is Container )
                {
                    Container cont = o as Container;
                    Container pack = from.Backpack;

                    if( pack == null || pack.Deleted )
                        return;

                    if( cont == m_ContainerFrom )
                        from.SendMessage( "Il contenitore scelto deve essere diverso da quello di partenza." );
                    else if( m_ContainerFrom.IsChildOf( pack ) && ( cont.IsChildOf( pack ) || cont == pack ) )
                        from.SendMessage( "Non puoi muovere oggetti tra contenitori nel tuo zaino o da un contenitore nel tuo ziano. Posiziona uno dei due per terra." );
                    else if( cont.IsChildOf( m_ContainerFrom ) )
                        from.SendMessage( "Non puoi muovere oggetti in un sotto-contenitore di quello di partenza. Posiziona uno dei due per terra." );
                    else if( cont is LockableContainer && ( (LockableContainer)cont ).Locked )
                        from.SendMessage( "Non puoi muovere oggetti in un contenitore chiuso a chiave." );
                    else if( !cont.IsAccessibleTo( from ) )
                        from.SendMessage( "Non e' accessibile." );
                    else if( cont.Parent != null && cont.Parent is Mobile )
                        from.SendMessage( "Non e' accessibile." );
                    else
                    {
                        Item[] items = m_ContainerFrom.FindItemsByType( m_Type, true );

                        bool hasBeenDropped = true;

                        for( int i = 0; i < items.Length && hasBeenDropped; i++ )
                        {
                            Item item = items[ i ];

                            if( item == null || item.Deleted || CannotBeMoved( item ) )
                                continue;

                            if( item.Movable && item.IsAccessibleTo( from ) )
                                hasBeenDropped = cont.TryDropItem( from, item, false );

                            if( !hasBeenDropped )
                                from.SendMessage( "Il contenitore e' pieno." );
                        }
                    }
                }
                else
                {
                    from.SendMessage( "Non e' un contenitore valido." );
                }
            }
        }
        #endregion
    }
}