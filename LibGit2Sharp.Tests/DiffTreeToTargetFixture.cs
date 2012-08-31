        [Fact]
        /*
         * No direct git equivalent but should output
         *
         * diff --git a/file.txt b/file.txt
         * index ce01362..4f125e3 100644
         * --- a/file.txt
         * +++ b/file.txt
         * @@ -1 +1,3 @@
         *  hello
         * +world
         * +!!!
         */
        public void CanCompareASimpleTreeAgainstTheWorkDir()
        {
            var scd = BuildSelfCleaningDirectory();

            using (var repo = Repository.Init(scd.RootedDirectoryPath))
            {
                SetUpSimpleDiffContext(repo);

                TreeChanges changes = repo.Diff.Compare(repo.Head.Tip.Tree, DiffTarget.WorkingDirectory);

                var expected = new StringBuilder()
                    .Append("diff --git a/file.txt b/file.txt\n")
                    .Append("index ce01362..4f125e3 100644\n")
                    .Append("--- a/file.txt\n")
                    .Append("+++ b/file.txt\n")
                    .Append("@@ -1 +1,3 @@\n")
                    .Append(" hello\n")
                    .Append("+world\n")
                    .Append("+!!!\n");

                Assert.Equal(expected.ToString(), changes.Patch);
            }
        }

        [Fact]
        /*
         * $ git diff HEAD
         * diff --git a/file.txt b/file.txt
         * index ce01362..4f125e3 100644
         * --- a/file.txt
         * +++ b/file.txt
         * @@ -1 +1,3 @@
         *  hello
         * +world
         * +!!!
         */
        public void CanCompareASimpleTreeAgainstTheWorkDirAndTheIndex()
        {
            var scd = BuildSelfCleaningDirectory();

            using (var repo = Repository.Init(scd.RootedDirectoryPath))
            {
                SetUpSimpleDiffContext(repo);

                TreeChanges changes = repo.Diff.Compare(repo.Head.Tip.Tree, DiffTarget.BothWorkingDirectoryAndIndex);

                var expected = new StringBuilder()
                    .Append("diff --git a/file.txt b/file.txt\n")
                    .Append("index ce01362..4f125e3 100644\n")
                    .Append("--- a/file.txt\n")
                    .Append("+++ b/file.txt\n")
                    .Append("@@ -1 +1,3 @@\n")
                    .Append(" hello\n")
                    .Append("+world\n")
                    .Append("+!!!\n");

                Assert.Equal(expected.ToString(), changes.Patch);
            }
        }


        [Fact]
        /*
         * $ git diff
         *
         * $ git diff HEAD
         * diff --git a/file.txt b/file.txt
         * deleted file mode 100644
         * index ce01362..0000000
         * --- a/file.txt
         * +++ /dev/null
         * @@ -1 +0,0 @@
         * -hello
         *
         * $ git diff --cached
         * diff --git a/file.txt b/file.txt
         * deleted file mode 100644
         * index ce01362..0000000
         * --- a/file.txt
         * +++ /dev/null
         * @@ -1 +0,0 @@
         * -hello
         */
        public void ShowcaseTheDifferenceBetweenTheTwoKindOfComparison()
        {
            var scd = BuildSelfCleaningDirectory();

            using (var repo = Repository.Init(scd.RootedDirectoryPath))
            {
                SetUpSimpleDiffContext(repo);

                var fullpath = Path.Combine(repo.Info.WorkingDirectory, "file.txt");
                File.Move(fullpath, fullpath + ".bak");
                repo.Index.Stage(fullpath);
                File.Move(fullpath + ".bak", fullpath);

                FileStatus state = repo.Index.RetrieveStatus("file.txt");
                Assert.Equal(FileStatus.Removed | FileStatus.Untracked, state);


                TreeChanges wrkDirToIdxToTree = repo.Diff.Compare(repo.Head.Tip.Tree, DiffTarget.BothWorkingDirectoryAndIndex);
                var expected = new StringBuilder()
                    .Append("diff --git a/file.txt b/file.txt\n")
                    .Append("deleted file mode 100644\n")
                    .Append("index ce01362..0000000\n")
                    .Append("--- a/file.txt\n")
                    .Append("+++ /dev/null\n")
                    .Append("@@ -1 +0,0 @@\n")
                    .Append("-hello\n");

                Assert.Equal(expected.ToString(), wrkDirToIdxToTree.Patch);

                TreeChanges wrkDirToTree = repo.Diff.Compare(repo.Head.Tip.Tree, DiffTarget.WorkingDirectory);
                expected = new StringBuilder()
                    .Append("diff --git a/file.txt b/file.txt\n")
                    .Append("index ce01362..4f125e3 100644\n")
                    .Append("--- a/file.txt\n")
                    .Append("+++ b/file.txt\n")
                    .Append("@@ -1 +1,3 @@\n")
                    .Append(" hello\n")
                    .Append("+world\n")
                    .Append("+!!!\n");

                Assert.Equal(expected.ToString(), wrkDirToTree.Patch);
            }
        }

        /*
         * $ git diff --cached -- "deleted_staged_file.txt" "1/branch_file.txt" "I-do/not-exist"
         * diff --git a/deleted_staged_file.txt b/deleted_staged_file.txt
         * deleted file mode 100644
         * index 5605472..0000000
         * --- a/deleted_staged_file.txt
         * +++ /dev/null
         * @@ -1 +0,0 @@
         * -things
         */
        [Fact]
        public void CanCompareASubsetofTheTreeAgainstTheIndex()
        {
            using (var repo = new Repository(StandardTestRepoPath))
            {
                Tree tree = repo.Head.Tip.Tree;

                TreeChanges changes = repo.Diff.Compare(tree, DiffTarget.Index, new[] { "deleted_staged_file.txt", "1/branch_file.txt", "I-do/not-exist" });
                Assert.NotNull(changes);

                Assert.Equal(1, changes.Count());
                Assert.Equal("deleted_staged_file.txt", changes.Deleted.Single().Path);
            }
        }
