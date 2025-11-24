namespace Midgard.Engines.ChessSystem
{
    /// <summary>
    /// Describes the status of the current game
    /// </summary>
    public enum GameStatus
    {
        /// <summary>
        /// The game is being setup
        /// </summary>
        Setup,
        /// <summary>
        /// White should make the next move
        /// </summary>
        WhiteToMove,
        /// <summary>
        /// Black should make the next move
        /// </summary>
        BlackToMove,
        /// <summary>
        /// A white piece is moving
        /// </summary>
        WhiteMoving,
        /// <summary>
        /// A black piece is moving
        /// </summary>
        BlackMoving,
        /// <summary>
        /// A white pawn has been promoted and the system is waiting for the user to make the decision
        /// </summary>
        WhitePromotion,
        /// <summary>
        /// A black pawn has been promoted and the system is waiting for the user to make the decision
        /// </summary>
        BlackPromotion,
        /// <summary>
        /// Game over
        /// </summary>
        Over
    }
}