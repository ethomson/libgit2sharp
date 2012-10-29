using LibGit2Sharp.Core.Handles;
using LibGit2Sharp.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibGit2Sharp.Core
{
    /// <summary>
    ///   Class to handle the interface between libgit2 git_transfer_progress_callback function and corresponding <see cref = "TransferProgressHandler" />.
    ///   Generates a delegate that wraps the <see cref = "TransferProgressHandler" /> delegate.
    /// </summary>
    internal class GitTransferCallbacks
    {
        /// <summary>
        ///   Managed delegate to be called in response to a git_transfer_progress_callback callback from libgit2.
        /// </summary>
        private TransferProgressHandler onTransferProgress;

        /// <summary>
        ///   Constructor to set up the native callback given managed delegate.
        /// </summary>
        /// <param name="onTransferProgress"></param>
        internal GitTransferCallbacks(TransferProgressHandler onTransferProgress)
        {
            this.onTransferProgress = onTransferProgress;
        }

        /// <summary>
        ///   Generates the native git_transfer_progress_callback delegate that wraps the <see cref = "TransferProgressHandler" /> delegate.
        /// </summary>
        /// <param name="onTransferProgress">The <see cref = "TransferProgressHandler" /> delegate to call in responde to a the native git_transfer_progress_callback callback.</param>
        /// <returns>A delegate method that can be passed to native libgit2 matching git_transfer_progress_callback function.</returns>
        public static NativeMethods.git_transfer_progress_callback GenerateCallback(TransferProgressHandler onTransferProgress)
        {
            return new GitTransferCallbacks(onTransferProgress).OnGitTransferProgress;
        }

        /// <summary>
        ///   The delegate to pass to libgit2 for the git_transfer_progress_callback delegate
        /// </summary>
        /// <param name="progress"><see cref = "TransferProgressHandler" /> structure containing progress information.</param>
        /// <param name="payload">Payload data.</param>
        public void OnGitTransferProgress(ref GitTransferProgress progress, IntPtr payload)
        {
            onTransferProgress(new TransferProgress(progress));
        }
    }
}
