using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LibGit2Sharp
{
    /// <summary>
    ///   Options controller git merge behavior.
    /// </summary>
    [Flags]
    public enum GitMergeOptionFlags
    {
        /// <summary>
        ///   No merge flags.  Use default behavior.
        /// </summary>
        GIT_MERGE_NORMAL = 0,

        /// <summary>
        ///   Don't perform a fast forward merge.
        /// </summary>
        GIT_MERGE_NO_FASTFORWARD = (1 << 0),
    }
}
