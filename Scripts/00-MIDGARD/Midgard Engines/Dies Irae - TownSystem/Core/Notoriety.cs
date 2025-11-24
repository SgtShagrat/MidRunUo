/***************************************************************************
 *                               Notoriety.cs
 *
 *   begin                : 16 January, 2010
 *   author               :	Dies Irae	
 *   email                : tocasia@alice.it
 *   copyright            : (C) Midgard Shard - Dies Irae			
 *
 ***************************************************************************/

using System;

using Server;
using Server.Items;
using Server.Mobiles;

namespace Midgard.Engines.MidgardTownSystem
{
    public class Notoriety
    {
        public static readonly bool TownCorpseHandlerEnabled = false;
        public static readonly bool TownCreatureHandlerEnabled = true;
        public static readonly bool TownMobileHandlerEnabled = true;

        /// <summary>
        /// Handler per la notorietà tra Mobile e cittadino che lo guarda.
        /// <returns>true se il metodo ha gestito la notorietà</returns>
        /// <returns><paramref name="noto"/> è la notorietà finale del Mobile target visto da source</returns>
        /// </summary>
        public static bool HandleTownMobileNotoriety( Mobile source, Mobile target, out int noto )
        {
            noto = -1;

            if( !TownMobileHandlerEnabled || !IsOnCivilianMap( source ) )
                return false;

            // se il target non e' ne un giocatore o ne una creatura controllata 
            // allora non valautare il suo stato cittadino
            if( !target.Player && ( target is BaseCreature && !( (BaseCreature)target ).Controlled ) )
                return false;

            // Se sono un giocatore o una creatura controllata
            if( source.Player || ( source is BaseCreature && ( (BaseCreature)source ).Controlled ) )
            {
                bool isYoung = ( target is PlayerMobile && ( ( (PlayerMobile)target ).Young ) || ( ( DateTime.Now - target.CreationTime ) < Config.YoungHours ) );

                // Se un non cittadino entra in una citta' e' SEMPRE canBeAttacked se non criminale
                //if( !isYoung && !TownHelper.IsCitizenOfAnyTown( target ) && TownHelper.InInAnyCity( target ) )
                //    noto = Server.Notoriety.CanBeAttacked;

                // Se sto guardando un semplice cittadino (o una creatura di un cittadino)...
                // NB: La noto per gildati vs gildati è gia' stata risolta
                // se sono in guerra le due cittadinanze...
                if( AreOpposedCitizens( source, target ) )
                {
                    // if( target.Guild != null && IsInHisOwnCity( source ) )
                    //    return false;

                    // se target non e' nella sua città allora in ogni caso sono nemici
                    if( !TownHelper.IsInHisOwnCity( target ) )
                        noto = Server.Notoriety.Enemy;
                }

                    // Se sono concittadini allora sono alleati.
                else if( AreCoCitizens( source, target ) )
                {
                    TownSystem t = TownSystem.Find( target );

                    // Solo se la citta' non e' permared allora co-cittadini sono alleati
                    if( t != null && !t.IsMurdererTown )
                        noto = Server.Notoriety.Ally;
                }
            }
#if DebugTownHelper
            Config.Pkg.LogInfoLine( "HandleTownMobileNotoriety: {0}", noto.ToString() );
#endif
            return noto != -1;
        }

        /// <summary>
        /// Handler per la notorietà tra Corpse e cittadino che lo guarda.
        /// <returns>true se il metodo ha gestito la notorietà</returns>
        /// <returns>noto è la notorietà finale del Corpse target visto da source</returns>
        /// </summary>
        public static bool HandleTownCorpseNotoriety( Mobile source, Corpse corpse, out int noto )
        {
            noto = -1;

            if( !TownCorpseHandlerEnabled )
                return false;

            Midgard2PlayerMobile m2PmSource = source as Midgard2PlayerMobile;
            if( m2PmSource == null )
                return false;

            if( !IsOnCivilianMap( corpse ) )
                return false;

            Midgard2PlayerMobile m2Pm = null;
            if( corpse.Owner is BaseCreature && corpse.Owner != null )
            {
                Mobile master = ( (BaseCreature)( corpse.Owner ) ).GetMaster();
                if( master != null )
                    m2Pm = master as Midgard2PlayerMobile;
            }
            else if( corpse.Owner is Midgard2PlayerMobile )
                m2Pm = corpse.Owner as Midgard2PlayerMobile;

            // Se un non cittadino ed è in una città è SEMPRE canBeAttacked se non criminale
            //if( !TownHelper.IsCitizenOfAnyTown( m2Pm ) && TownHelper.InInAnyCity( m2Pm ) )
            //    noto = Server.Notoriety.CanBeAttacked;

            if( m2Pm != null )
            {
                if( AreOpposedCitizens( m2PmSource, m2Pm ) && !TownHelper.IsInHisOwnCity( corpse ) )
                    noto = Server.Notoriety.Enemy;
                else if( AreCoCitizens( m2PmSource, m2Pm ) )
                    noto = Server.Notoriety.Ally;
            }

#if DebugTownHelper
            Config.Pkg.LogInfoLine( "HandleTownCorpseNotoriety: {0}", noto.ToString() );
#endif
            return noto != -1;
        }

        /// <summary>
        /// Handler per la notorietà tra <see cref="BaseCreature"/> (pet) e mobile source che lo guarda.
        /// <returns>true se il metodo ha gestito la notorietà</returns>
        /// <returns>noto è la notorietà finale della <see cref="BaseCreature"/> target vista da source</returns>
        /// </summary>
        public static bool HandleTownCreatureNotoriety( Mobile source, Mobile target, out int noto )
        {
            noto = -1;

            if( !TownCreatureHandlerEnabled || !IsOnCivilianMap( target ) )
                return false;

            // se il target non e' ne un giocatore o ne una creatura controllata allora non valautare il suo stato cittadino
            if( !target.Player && ( target is BaseCreature && !( (BaseCreature)target ).Controlled ) )
                return false;

            // Se un NON cittadino (anche creatura) entra in una citta' e' SEMPRE canBeAttacked se non criminale
            //if( !TownHelper.IsCitizenOfAnyTown( target ) && TownHelper.InInAnyCity( target ) )
            //    noto = Server.Notoriety.CanBeAttacked;

                // se sono invece citta' alleate
            else if( AreCoCitizens( source, target ) )
                noto = Server.Notoriety.Ally;

#if DebugTownHelper
            Config.Pkg.LogInfoLine( "HandleTownCreatureNotoriety: {0}", noto.ToString() );
#endif
            return noto != -1;
        }

        /// <summary>
        /// Metodo invocato per verificare se due <see cref="TownSystem"/> sono in guerra.
        /// </summary>
        public static bool AreSystemsAtWar( TownSystem attSystem, TownSystem defSystem )
        {
            return attSystem != null && attSystem.IsEnemyTo( defSystem );
        }

        /// <summary>
        /// Metodo invocato per verificare se due <see cref="TownSystem"/> sono alleati.
        /// </summary>
        public static bool AreAlliedSystems( TownSystem attSystem, TownSystem defSystem )
        {
            return attSystem != null && attSystem.IsAlliedTo( defSystem );
        }

        /// <summary>
        /// Metodo invocato per verificare se due Mobile sono della stessa città.
        /// </summary>
        public static bool AreCoCitizens( Mobile mob1, Mobile mob2 )
        {
#if DebugTownHelper
            Config.Pkg.LogInfoLine( "AreCoCitizens: IsCitizenOfAnyTown {0} - AreAlliedSystems {1}",
                IsCitizenOfAnyTown( mob1 ).ToString(),
                AreAlliedSystems( TownSystem.Find( mob1, mob1 is BaseCreature ),
                TownSystem.Find( mob2, mob2 is BaseCreature ) ) );
#endif

            if( TownHelper.IsCitizenOfAnyTown( mob1 ) )
            {
                if( mob1 == mob2 )
                    return true;

                if( AreAlliedSystems( TownSystem.Find( mob1, mob1 is BaseCreature ), TownSystem.Find( mob2, mob2 is BaseCreature ) ) )
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Metodo invocato per verificare se due Mobile sono di città opposte.
        /// </summary>
        public static bool AreOpposedCitizens( Mobile mob1, Mobile mob2 )
        {
            return TownHelper.IsCitizenOfAnyTown( mob1 ) && AreSystemsAtWar( TownSystem.Find( mob1, mob1 is BaseCreature ), TownSystem.Find( mob2, mob2 is BaseCreature ) );
        }

        /// <summary>
        /// Check per verificare che un oggetto generico sia sotto le regole delle cittadinanze
        /// </summary>
        public static bool IsOnCivilianMap( object toCheck )
        {
            if( toCheck is Mobile )
                return Array.IndexOf( Config.TownWarsMaps, ( (Mobile)toCheck ).Map ) > -1;
            else if( toCheck is Item )
                return Array.IndexOf( Config.TownWarsMaps, ( (Item)toCheck ).Map ) > -1;
            else
                return false;
        }
    }
}