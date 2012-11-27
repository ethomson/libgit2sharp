using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibGit2Sharp.Core.Handles
{
    internal class MergeHeadSafeHandle : SafeHandleBase
    {
        protected override bool ReleaseHandle()
        {
            Proxy.git_merge_head_free(handle);
            return true;
        }
    }
}
