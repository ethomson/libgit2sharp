using LibGit2Sharp.Core;
using LibGit2Sharp.Core.Handles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibGit2Sharp
{
    /// <summary>
    ///   Represents merge head from libgit2.
    /// </summary>
    public class MergeHead
    {
        private MergeHeadSafeHandle m_handle;

        /// <summary>
        ///   Construct a MergeHead for the specified id.
        /// </summary>
        /// <param name="oid">A GitOid</param>
        internal MergeHead(GitOid oid)
        {
            m_handle = Proxy.git_merge_head_from_oid(oid);
        }

        /// <summary>
        ///   Construct a MergeHead for the specified reference.
        /// </summary>
        /// <param name="reference">A handle to a GitReference</param>
        internal MergeHead(ReferenceSafeHandle reference)
        {
            m_handle = Proxy.git_merge_head_from_ref(reference);
        }

        /// <summary>
        ///   Get the handle for this MergeHead.
        /// </summary>
        internal MergeHeadSafeHandle Handle
        {
            get { return m_handle; }
        }
    }
}
