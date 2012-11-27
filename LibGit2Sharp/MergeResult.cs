using LibGit2Sharp.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LibGit2Sharp
{
    /// <summary>
    ///   The result of a merge operation.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class MergeResult
    {
        /// <summary>
        ///   True if a fast forward merge was performaed.
        /// </summary>
        public bool isFastForward;

        /// <summary>
        ///   GitOid of the target commit when fast forward merge performed.
        /// </summary>
        internal GitOid Oid;
    }
}
