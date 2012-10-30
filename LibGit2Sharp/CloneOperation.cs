using System;
using LibGit2Sharp.Core;
using LibGit2Sharp.Core.Handles;
using LibGit2Sharp.Handlers;

namespace LibGit2Sharp
{
    /// <summary>
    /// Encapsulates a clone operation.
    /// </summary>
    public class CloneOperation
    {
        /// <summary>
        /// URI for the remote repository.
        /// </summary>
        public string SourceUrl { get; private set; }

        /// <summary>
        /// Local path to clone into.
        /// </summary>
        public string DestinationPath { get; private set; }

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
        /// Create a new clone operation.
        /// </summary>
        /// <param name="sourceUrl">URI for the remote repository</param>
        /// <param name="workdirPath">Local path to clone into</param>
        public CloneOperation(string sourceUrl, string workdirPath)
        {
            SourceUrl = sourceUrl;
            DestinationPath = workdirPath;
            Bare = false;
            Checkout = true;
        }

        /// <summary>
        /// Starts a clone operation on a background thread, and polls for 
        /// progress on this thread.
        /// </summary>
        /// <returns></returns>
        public virtual Repository Execute()
        {
            GitCheckoutOptions nativeOpts = null;
            if (Checkout)
            {
                var checkoutOptions = new CheckoutOptions();
                nativeOpts = checkoutOptions.checkoutOptions;
            }

            NativeMethods.git_transfer_progress_callback cb = (TransferProgress != null) ?
                GitTransferCallbacks.GenerateCallback(TransferProgress) : null;

            try
            {
                RepositorySafeHandle repo = Bare
                                                ? Proxy.git_clone_bare(SourceUrl, DestinationPath, cb)
                                                : Proxy.git_clone(SourceUrl, DestinationPath, cb, nativeOpts);
                repo.SafeDispose();
            } catch (Exception e)
            {
                Console.WriteLine("{0}", e);
            }

            return new Repository(DestinationPath);
        }
    }
}