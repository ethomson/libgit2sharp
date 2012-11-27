using LibGit2Sharp.Tests.TestHelpers;
using LibGit2Sharp.Core;
using LibGit2Sharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace LibGit2Sharp.Tests
{
    public class MergeFixture : BaseFixture
    {
        private static String s_MasterBranch = "master";
        private static String s_Branch1Name = "Branch1";
        private static String s_Branch2Name = "Branch2";

        private static String s_FileName1Branch1 = "b11.txt";
        private static String s_FileName1Branch2 = "b12.txt";

        private static String s_FileName1Master = "m1.txt";
        private static String s_FileName2Master = "m2.txt";

        [Fact]
        public void MergeWithFastForwardResult()
        {
            SelfCleaningDirectory scd = BuildSelfCleaningDirectory();

            using (var repo = Repository.Init(scd.DirectoryPath))
            {
                SetupBranchedRepository(repo);
                SwitchToBranch(repo, s_Branch1Name);

                CreateStageAndCommitFile(repo, s_FileName1Branch1);
                SwitchToBranch(repo, s_MasterBranch);

                MergeFastForward(repo, s_Branch1Name);
            }
        }

        [Fact]
        public void MergeOneBranchWithAdd()
        {
            SelfCleaningDirectory scd = BuildSelfCleaningDirectory();

            using (var repo = Repository.Init(scd.DirectoryPath))
            {
                SetupBranchedRepository(repo);
                SwitchToBranch(repo, s_Branch1Name);

                CreateStageAndCommitFile(repo, s_FileName1Branch1);
                SwitchToBranch(repo, s_MasterBranch);

                MergeNoFastForward(repo, s_Branch1Name);
                VerifyIndex(repo, FileStatus.Added, s_FileName1Branch1);
            }
        }

        [Fact]
        public void MergeTwoBranchesWithAdd()
        {
            SelfCleaningDirectory scd = BuildSelfCleaningDirectory();

            using (var repo = Repository.Init(scd.DirectoryPath))
            {
                SetupBranchedRepository(repo);
                SwitchToBranch(repo, s_Branch1Name);

                CreateStageAndCommitFile(repo, s_FileName1Branch1);
                SwitchToBranch(repo, s_Branch2Name);

                CreateStageAndCommitFile(repo, s_FileName1Branch2);
                SwitchToBranch(repo, s_MasterBranch);

                //MergeNoFastForward(repo, new String[] { s_Branch1Name, s_Branch2Name });
                // TODO: Need verification.  Octopus not yet implemented in libgit2
            }
        }

        [Fact]
        public void MergeOneBranchWithEdit()
        {
            SelfCleaningDirectory scd = BuildSelfCleaningDirectory();

            using (var repo = Repository.Init(scd.DirectoryPath))
            {
                SetupBranchedRepository(repo);
                SwitchToBranch(repo, s_Branch1Name);

                ModifyStageAndCommitFile(repo, s_FileName1Master, "edit");
                SwitchToBranch(repo, s_MasterBranch);

                MergeNoFastForward(repo, s_Branch1Name);

                VerifyIndex(repo, FileStatus.Staged, s_FileName1Master);
            }
        }

        [Fact]
        public void MergeOneBranchWithEditConflict()
        {
            SelfCleaningDirectory scd = BuildSelfCleaningDirectory();

            using (var repo = Repository.Init(scd.DirectoryPath))
            {
                SetupBranchedRepository(repo);
                SwitchToBranch(repo, s_Branch1Name);

                ModifyStageAndCommitFile(repo, s_FileName1Master, "first");
                SwitchToBranch(repo, s_MasterBranch);

                ModifyStageAndCommitFile(repo, s_FileName1Master, "second");
                MergeNoFastForward(repo, s_Branch1Name);

                // TODO: Verify the staged files.
                // TDOO: Verify the conflicts.
                Console.WriteLine();
            }
        }

        [Fact]
        public void MergeTwoBranchesWithEdit()
        {
            SelfCleaningDirectory scd = BuildSelfCleaningDirectory();

            using (var repo = Repository.Init(scd.DirectoryPath))
            {
                SetupBranchedRepository(repo);
                SwitchToBranch(repo, s_Branch1Name);

                ModifyStageAndCommitFile(repo, s_FileName1Master, "edit");
                SwitchToBranch(repo, s_Branch2Name);

                ModifyStageAndCommitFile(repo, s_FileName2Master, "edit");
                SwitchToBranch(repo, s_MasterBranch);

                //MergeNoFastForward(repo, new String[] { s_Branch1Name, s_Branch2Name});
                // TODO: Need verification.  Octopus stategy not yet implemented in libgit2.
            }
        }

        [Fact]
        public void MergeOneBranchWithDelete()
        {
            SelfCleaningDirectory scd = BuildSelfCleaningDirectory();

            using (var repo = Repository.Init(scd.DirectoryPath))
            {
                SetupBranchedRepository(repo);
                SwitchToBranch(repo, s_Branch1Name);

                DeleteStageAndCommitFile(repo, s_FileName2Master);
                SwitchToBranch(repo, s_MasterBranch);

                MergeNoFastForward(repo, s_Branch1Name);
                VerifyIndex(repo, FileStatus.Removed, s_FileName2Master);
            }
        }

        private void MergeFastForward(Repository repo, String branchName)
        {
            MergeResult result = Merge(repo, new String[] { branchName }, GitMergeOptionFlags.GIT_MERGE_NORMAL);
            Assert.True(result.isFastForward, "ExpectedFastForward=True");
        }

        private void MergeNoFastForward(Repository repo, String branchName)
        {
            MergeNoFastForward(repo, new String[] { branchName });
        }

        private void MergeNoFastForward(Repository repo, String[] branchNames)
        {
            MergeResult result = Merge(repo, branchNames, GitMergeOptionFlags.GIT_MERGE_NO_FASTFORWARD);
            Assert.False(result.isFastForward, "Expected FastForward=False");
        }

        private MergeResult Merge(Repository repo, String[] branchNames, GitMergeOptionFlags flags)
        {
            Branch[] branches = new Branch[branchNames.Length];
            for (int i = 0; i < branchNames.Length; i++)
            {
                branches[i] = repo.Branches[branchNames[i]];
            }

            return repo.Merge.GitMerge(branches, flags);
        }

        private void CreateStageAndCommitFile(Repository repo, String filename)
        {
            CreateNewFile(repo, filename);
            StageFile(repo, filename);
            CommitChanges(repo);
        }

        private void ModifyStageAndCommitFile(Repository repo, String filename, String value)
        {
            ModifyFile(repo, filename, value);
            StageFile(repo, filename);
            CommitChanges(repo);
        }

        private void DeleteStageAndCommitFile(Repository repo, String filename)
        {
            DeleteFile(repo, filename);
            StageFile(repo, filename);
            CommitChanges(repo);
        }

        private void CreateNewFile(Repository repo, String filename)
        {
            string fullPath = Path.Combine(repo.Info.WorkingDirectory, filename);
            File.WriteAllText(fullPath, "");
        }

        private void DeleteFile(Repository repo, String filename)
        {
            string fullPath = Path.Combine(repo.Info.WorkingDirectory, filename);
            File.Delete(fullPath);
        }

        private void ModifyFile(Repository repo, String filename, String value)
        {
            string fullPath = Path.Combine(repo.Info.WorkingDirectory, filename);
            File.WriteAllText(fullPath, value);
        }

        private static Boolean AreEquivalent(HashSet<String> expected, IEnumerable<String> actual)
        {
            int actualCount = 0;

            foreach (String s in actual)
            {
                actualCount++;
                if (!expected.Contains(s))
                {
                    return false;
                }
            }

            return actualCount == expected.Count;
        }

        private static void VerifyIndexEmpty(Repository repo)
        {
            VerifyIndex(repo, FileStatus.Nonexistent, "");
        }

        private static void VerifyIndex(Repository repo, FileStatus expectedStatus, String fileName)
        {
            VerifyIndex(repo, expectedStatus, new String[] { fileName });
        }

        private static void VerifyIndex(Repository repo, FileStatus expectedStatus, String[] filenames)
        {
            HashSet<String> empty = new HashSet<String>();
            HashSet<String> expected = new HashSet<String>(filenames);
            
            RepositoryStatus status = repo.Index.RetrieveStatus();
            Assert.True(status.IsDirty);
            
            Assert.True(AreEquivalent(expectedStatus == FileStatus.Added ? expected : empty, status.Added));
            Assert.True(AreEquivalent(expectedStatus == FileStatus.Ignored ? expected : empty, status.Ignored));
            Assert.True(AreEquivalent(expectedStatus == FileStatus.Missing ? expected : empty, status.Missing));
            Assert.True(AreEquivalent(expectedStatus == FileStatus.Modified ? expected : empty, status.Modified));
            Assert.True(AreEquivalent(expectedStatus == FileStatus.Removed ? expected : empty, status.Removed));
            Assert.True(AreEquivalent(expectedStatus == FileStatus.Staged ? expected : empty, status.Staged));
            Assert.True(AreEquivalent(expectedStatus == FileStatus.Unaltered ? expected : empty, status.Untracked));
        }

        private static void VerifyConflict(Repository repo, String filename)
        {
            // TODO: Add verification that the specied filename has a conflict
        }

        private static void DumpRepositoryStatus(Repository repo, String header)
        {
            Console.WriteLine("=====" + header + "=====");
            DumpRepositoryStatus(repo);
        }

        private static void DumpRepositoryStatus(Repository repo)
        {
            RepositoryStatus status = repo.Index.RetrieveStatus();

            Console.WriteLine("Added *****");
            foreach (String s in status.Added)
                Console.WriteLine(s);

            Console.WriteLine("Modified *****");
            foreach (String s in status.Modified)
                Console.WriteLine(s);

            Console.WriteLine("Missing *****");
            foreach (String s in status.Missing)
                Console.WriteLine(s);

            Console.WriteLine("Removed *****");
            foreach (String s in status.Removed)
                Console.WriteLine(s);

            Console.WriteLine("Staged *****");
            foreach (String s in status.Staged)
                Console.WriteLine(s);

            Console.WriteLine("Untracked *****");
            foreach (String s in status.Untracked)
                Console.WriteLine(s);

            Console.WriteLine("Ignored *****");
            foreach (String s in status.Ignored)
                Console.WriteLine(s);
        }

        private static void DumpBranches(Repository repo)
        {
            foreach (var branch in repo.Branches)
            {
                Console.WriteLine(branch.Name + (branch.IsCurrentRepositoryHead ? "*" : ""));
            }
        }

        private static void SetupBranchedRepository(Repository repo)
        {
            String fullPath;

            // Create file M1.txt in master.
            fullPath = Path.Combine(repo.Info.WorkingDirectory, s_FileName1Master);
            File.WriteAllText(fullPath, "initial\n");
            repo.Index.Stage(fullPath);

            // Create file M2.txt in master.
            fullPath = Path.Combine(repo.Info.WorkingDirectory, s_FileName2Master);
            File.WriteAllText(fullPath, "initial\n");
            repo.Index.Stage(fullPath);

            // Commit the two files to master.
            repo.Commit("Initial commit", Constants.Signature, Constants.Signature);

            // Create "Branch1" off master.
            repo.CreateBranch(s_Branch1Name);

            // Create "Branch2" off master.
            repo.CreateBranch(s_Branch2Name);

            // The repository should not be dirty at this point.
            Assert.False(repo.Index.RetrieveStatus().IsDirty);
        }

        private static void SwitchToBranch(Repository repo, String branchName)
        {
            repo.Checkout(branchName);
        }

        private static void StageFile(Repository repo, String filename)
        {
            repo.Index.Stage(filename);
        }

        private static void CommitChanges(Repository repo)
        {
            repo.Commit("Comment", Constants.Signature, Constants.Signature);
        }

        private class StrategyData
        {
            private int count = 5;

            public int Count { get { return count; } }
        }
    }
}
