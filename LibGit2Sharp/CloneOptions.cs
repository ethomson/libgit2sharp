using LibGit2Sharp.Handlers;

namespace LibGit2Sharp
{
    /// <summary>
    /// Encapsulates options for a clone operation.
    /// </summary>
    public class CloneOptions
    {
        /// <summary>
        /// If true, do a bare clone; otherwise, a full clone with working directory.
        /// </summary>
        public bool Bare { get; set; }

        /// <summary>
        /// If true, perform a checkout of the remote's default branch. This only applies
        /// to non-bare clones. Defaults to true.
        /// </summary>
        public bool Checkout { get; set; }

        /// <summary>
        /// Delegate for reporting network transfer progress.
        /// </summary>
        public TransferProgressHandler TransferProgress { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public CloneOptions()
        {
            Checkout = true;
        }
    }
}