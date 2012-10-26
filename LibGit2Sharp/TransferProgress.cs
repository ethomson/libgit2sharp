using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibGit2Sharp.Core;

namespace LibGit2Sharp
{
    /// <summary>
    ///   Expose progress values from a fetch operation.
    /// </summary>
    public class TransferProgress
    {
        /// <summary>
        ///   Empty constructor.
        /// </summary>
        protected TransferProgress()
        { }

        /// <summary>
        ///   Constructor.
        /// </summary>
        internal TransferProgress(GitTransferProgress gitTransferProgress)
        {
            this.gitTransferProgress = gitTransferProgress;
        }

        /// <summary>
        ///   Total number of objects.
        /// </summary>
        [CLSCompliant(false)]
        public virtual uint TotalObjectCount
        {
            get
            {
                return gitTransferProgress.total_objects;
            }
        }

        /// <summary>
        ///   Number of objects indexed.
        /// </summary>
        [CLSCompliant(false)]
        public virtual uint IndexedObjectCount
        {
            get
            {
                return gitTransferProgress.indexed_objects;
            }
        }

        /// <summary>
        ///   Number of objects received.
        /// </summary>
        [CLSCompliant(false)]
        public virtual uint ReceivedObjectCount
        {
            get
            {
                return gitTransferProgress.received_objects;
            }
        }

        /// <summary>
        ///   Number of bytes received.
        /// </summary>
        [CLSCompliant(false)]
        public virtual ulong BytesReceived
        {
            get
            {
                return (ulong) gitTransferProgress.received_bytes;
            }
        }

        #region Fields

        internal GitTransferProgress gitTransferProgress;

        #endregion
    }
}
