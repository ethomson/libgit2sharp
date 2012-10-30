using System.IO;
using LibGit2Sharp.Tests.TestHelpers;
using Xunit;
using Xunit.Extensions;

namespace LibGit2Sharp.Tests
{
    public class CloneOperationFixture : BaseFixture
    {
        [Theory]
        [InlineData("http://github.com/libgit2/TestGitRepository")]
        [InlineData("https://github.com/libgit2/TestGitRepository")]
        [InlineData("git://github.com/libgit2/TestGitRepository")]
        //[InlineData("git@github.com:libgit2/TestGitRepository")]
        public void CanClone(string url)
        {
            var scd = BuildSelfCleaningDirectory();
            var cloneop = new CloneOperation(url, scd.RootedDirectoryPath);
            using (Repository repo = cloneop.Execute())
            {
                string dir = repo.Info.Path;
                Assert.True(Path.IsPathRooted(dir));
                Assert.True(Directory.Exists(dir));

                Assert.NotNull(repo.Info.WorkingDirectory);
                Assert.Equal(Path.Combine(scd.RootedDirectoryPath, ".git" + Path.DirectorySeparatorChar), repo.Info.Path);
                Assert.False(repo.Info.IsBare);

                Assert.True(File.Exists(Path.Combine(scd.RootedDirectoryPath, "master.txt")));
            }
        }

        [Theory]
        [InlineData("http://github.com/libgit2/TestGitRepository")]
        [InlineData("https://github.com/libgit2/TestGitRepository")]
        [InlineData("git://github.com/libgit2/TestGitRepository")]
        //[InlineData("git@github.com:libgit2/TestGitRepository")]
        public void CanCloneBarely(string url)
        {
            var scd = BuildSelfCleaningDirectory();
            var cloneop = new CloneOperation(url, scd.RootedDirectoryPath)
                              {
                                  Bare = true,
                              };
            using (Repository repo = cloneop.Execute())
            {
                string dir = repo.Info.Path;
                Assert.True(Path.IsPathRooted(dir));
                Assert.True(Directory.Exists(dir));

                Assert.Null(repo.Info.WorkingDirectory);
                Assert.Equal(scd.RootedDirectoryPath+ Path.DirectorySeparatorChar, repo.Info.Path);
                Assert.True(repo.Info.IsBare);
            }
        }

        private const string TestRepoUrl = "git://github.com/libgit2/TestGitRepository";

        [Fact]
        public void WontCheckoutIfAskedNotTo()
        {
            var scd = BuildSelfCleaningDirectory();
            var cloneop = new CloneOperation(TestRepoUrl, scd.RootedDirectoryPath)
                              {
                                  Checkout = false
                              };
            using (Repository repo = cloneop.Execute())
            {
                Assert.False(File.Exists(Path.Combine(scd.RootedDirectoryPath, "master.txt")));
            }
        }

        [Fact]
        public void CallsTransferProgress()
        {
            bool wasCalled = false;

            var scd = BuildSelfCleaningDirectory();
            var cloneop = new CloneOperation(TestRepoUrl, scd.RootedDirectoryPath)
                              {
                                  TransferProgress = (_) => wasCalled = true
                              };
            using (Repository repo = cloneop.Execute())
            {
                Assert.True(wasCalled);
            }
        }
    }
}