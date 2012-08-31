﻿using System.IO;
using System.Linq;
        private static readonly string subBranchFilePath = Path.Combine("1", "branch_file.txt");
        /*
         * $ git diff 9fd738e..HEAD -- "1" "2/"
         * diff --git a/1/branch_file.txt b/1/branch_file.txt
         * new file mode 100755
         * index 0000000..45b983b
         * --- /dev/null
         * +++ b/1/branch_file.txt
         * @@ -0,0 +1 @@
         * +hi
         */
        [Fact]
        public void CanCompareASubsetofTheTreeAgainstOneOfItsAncestor()
        {
            using (var repo = new Repository(StandardTestRepoPath))
            {
                Tree tree = repo.Head.Tip.Tree;
                Tree ancestor = repo.Lookup<Commit>("9fd738e").Tree;

                TreeChanges changes = repo.Diff.Compare(ancestor, tree, new[]{ "1", "2/" });
                Assert.NotNull(changes);

                Assert.Equal(1, changes.Count());
                Assert.Equal(subBranchFilePath, changes.Added.Single().Path);
            }
        }

                Assert.Equal(new[] { "1.txt", subBranchFilePath, "README", "branch_file.txt", "deleted_staged_file.txt", "deleted_unstaged_file.txt", "modified_staged_file.txt", "modified_unstaged_file.txt", "new.txt" },