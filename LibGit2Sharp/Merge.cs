using LibGit2Sharp.Core;
using LibGit2Sharp.Core.Handles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibGit2Sharp
{
    /// <summary>
    /// 
    /// </summary>
    public class Merge
    {
        private readonly Repository repo;

        /// <summary>
        ///   Needed for mocking purposes.
        /// </summary>
        protected Merge()
        { }

        /// <summary>
        ///   Instantiate a merge dispatcher for the specified repo.
        /// </summary>
        /// <param name="repo"></param>
        internal Merge(Repository repo)
        {
            this.repo = repo;
        }

        /// <summary>
        ///   Merge the branch into the current branch.
        /// </summary>
        /// <param name="branch">The source branch to merge</param>
        /// <param name="flags">Merge options</param>
        /// <returns>The result or the merge or throws on failure</returns>
        public virtual MergeResult GitMerge(Branch branch, GitMergeOptionFlags flags)
        {
            return GitMerge(new Branch[] { branch }, flags);
        }

        /// <summary>
        ///   Merge the branches into the current branch.
        /// </summary>
        /// <param name="branches">The source branches to merge</param>
        /// <param name="flags">Merge options</param>
        /// <returns>The result of the merge or throws on failure</returns>
        public virtual MergeResult GitMerge(Branch[] branches, GitMergeOptionFlags flags)
        {
            MergeHeadSafeHandle[] mergeHeadHandles = new MergeHeadSafeHandle[branches.Length];
            for (int i = 0; i < branches.Length; i++)
            {
                using (ReferenceSafeHandle handle = RetrieveReferencePtr(branches[i].CanonicalName))
                {
                    MergeHead mergeHead = new MergeHead(handle);
                    mergeHeadHandles[i] = mergeHead.Handle;
                }
            }

            MergeResult result = Proxy.git_merge(repo.Handle, mergeHeadHandles, flags);

            return result;
        }

        internal ReferenceSafeHandle RetrieveReferencePtr(string referenceName, bool shouldThrowIfNotFound = true)
        {
            return Proxy.git_reference_lookup(repo.Handle, referenceName, shouldThrowIfNotFound);
        }
    }
}
