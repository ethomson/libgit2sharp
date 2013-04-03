using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using LibGit2Sharp.Tests.TestHelpers;
using Xunit;

namespace LibGit2Sharp.Tests
{
    public class RefdbBackendFixture : BaseFixture
    {
        [Fact]
        public void CanReadFromRefdbBackend()
        {
            var scd = new SelfCleaningDirectory(this);

            using (Repository repository = Repository.Init(scd.RootedDirectoryPath))
            {
                MockRefdbBackend backend = new MockRefdbBackend(repository);
                repository.ReferenceDatabase.SetBackend(backend);

                backend.References["HEAD"] = new RefdbBackend.RefdbReference("refs/heads/testref");
                backend.References["refs/heads/testref"] = new RefdbBackend.RefdbReference(new ObjectId("d828f118b334a9396ca129f2c733bae8ce6faa8d"));

                Assert.True(repository.Refs["HEAD"].TargetIdentifier.Equals("refs/heads/testref"));
                Assert.True(repository.Refs["HEAD"].ResolveToDirectReference().TargetIdentifier.Equals("d828f118b334a9396ca129f2c733bae8ce6faa8d"));

                Branch branch = repository.Head;

                Console.WriteLine(branch.CanonicalName);

                Assert.True(branch.CanonicalName.Equals("refs/heads/testref"));
            }
        }

        #region MockRefdbBackend

        private class MockRefdbBackend : RefdbBackend
        {
            private readonly Repository repository;

            private readonly Dictionary<string, RefdbReference> references =
                new Dictionary<string, RefdbReference>();

            public MockRefdbBackend(Repository repository)
            {
                this.repository = repository;
            }

            protected override Repository Repository
            {
                get
                {
                    return repository;
                }
            }

            public Dictionary<string, RefdbReference> References
            {
                get
                {
                    return references;
                }
            }

            protected override RefdbBackendOperations SupportedOperations
            {
                get
                {
                    return RefdbBackendOperations.Compress | RefdbBackendOperations.ForEachGlob;
                }
            }

            public override bool Exists(string referenceName)
            {
                return references.ContainsKey(referenceName);
            }

            public override RefdbReference Lookup(string referenceName)
            {
                return references[referenceName];
            }

            public override void Write(Reference reference)
            {
                RefdbReference storage;

                if (reference is SymbolicReference)
                {
                    storage = new RefdbReference(((SymbolicReference)reference).TargetIdentifier);
                }
                else
                {
                    storage = new RefdbReference(((DirectReference)reference).Target.Id);
                }

                references.Add(reference.CanonicalName, storage);
            }

            public override void Delete(Reference reference)
            {
                references.Remove(reference.CanonicalName);
            }

            public override void Compress()
            {
            }

            public override void Free()
            {
                references.Clear();
            }
        }

        #endregion
    }
}
