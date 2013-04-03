using System;
using System.IO;
using System.Runtime.InteropServices;
using LibGit2Sharp.Core;
using LibGit2Sharp.Core.Handles;

namespace LibGit2Sharp
{
    /// <summary>
    ///   Base class for all custom managed backends for the libgit2 reference database.
    /// </summary>
    public abstract class RefdbBackend
    {
        protected abstract Repository Repository
        {
            get;
        }

        /// <summary>
        ///   In your subclass, override this member to provide the list of actions your backend supports.
        /// </summary>
        protected abstract RefdbBackendOperations SupportedOperations
        {
            get;
        }

        public abstract bool Exists(string referenceName);

        public abstract RefdbReference Lookup(string referenceName);

        public abstract void Write(Reference reference);

        public abstract void Delete(Reference reference);

        public abstract void Compress();

        public abstract void Free();

        public class RefdbReference
        {
            public RefdbReference(string target)
            {
                Type = GitReferenceType.Symbolic;
                Symbolic = target;
            }

            public RefdbReference(ObjectId target)
            {
                Type = GitReferenceType.Oid;
                Oid = target;
            }

            internal GitReferenceType Type
            {
                get;
                private set;
            }

            public ObjectId Oid
            {
                get;
                private set;
            }

            public string Symbolic
            {
                get;
                private set;
            }
        }

        private IntPtr nativeBackendPointer;

        internal IntPtr GitRefdbBackendPointer
        {
            get
            {
                if (IntPtr.Zero == nativeBackendPointer)
                {
                    var nativeBackend = new GitRefdbBackend();
                    nativeBackend.Version = 1;

                    // The "free" entry point is always provided.
                    nativeBackend.Exists = BackendEntryPoints.ExistsCallback;
                    nativeBackend.Write = BackendEntryPoints.WriteCallback;
                    nativeBackend.Lookup = BackendEntryPoints.LookupCallback;
                    nativeBackend.Free = BackendEntryPoints.FreeCallback;

                    var supportedOperations = this.SupportedOperations;

                    if ((supportedOperations & RefdbBackendOperations.Compress) != 0)
                    {
                        nativeBackend.Compress = BackendEntryPoints.CompressCallback;
                    }

                    if ((supportedOperations & RefdbBackendOperations.ForEachGlob) != 0)
                    {
                        nativeBackend.ForeachGlob = BackendEntryPoints.ForeachGlobCallback;
                    }

                    nativeBackend.GCHandle = GCHandle.ToIntPtr(GCHandle.Alloc(this));
                    nativeBackendPointer = Marshal.AllocHGlobal(Marshal.SizeOf(nativeBackend));
                    Marshal.StructureToPtr(nativeBackend, nativeBackendPointer, false);
                }

                return nativeBackendPointer;
            }
        }

        private static class BackendEntryPoints
        {
            // Because our GitOdbBackend structure exists on the managed heap only for a short time (to be marshaled
            // to native memory with StructureToPtr), we need to bind to static delegates. If at construction time
            // we were to bind to the methods directly, that's the same as newing up a fresh delegate every time.
            // Those delegates won't be rooted in the object graph and can be collected as soon as StructureToPtr finishes.
            public static readonly GitRefdbBackend.exists_callback ExistsCallback = Exists;
            public static readonly GitRefdbBackend.lookup_callback LookupCallback = Lookup;
            public static readonly GitRefdbBackend.foreach_callback ForeachCallback = Foreach;
            public static readonly GitRefdbBackend.foreach_glob_callback ForeachGlobCallback = ForeachGlob;
            public static readonly GitRefdbBackend.write_callback WriteCallback = Write;
            public static readonly GitRefdbBackend.delete_callback DeleteCallback = Delete;
            public static readonly GitRefdbBackend.compress_callback CompressCallback = Compress;
            public static readonly GitRefdbBackend.free_callback FreeCallback = Free;

            private static int Exists(
                out IntPtr exists,
                IntPtr backend,
                IntPtr namePtr)
            {
                RefdbBackend refdbBackend = GCHandle.FromIntPtr(Marshal.ReadIntPtr(backend, GitRefdbBackend.GCHandleOffset)).Target as RefdbBackend;

                exists = (IntPtr)0;

                if (refdbBackend != null)
                {
                    try
                    {
                        string referenceName = Utf8Marshaler.FromNative(namePtr);

                        if (refdbBackend.Exists(referenceName))
                            exists = (IntPtr)1;

                        return 0;
                    }
                    catch (Exception ex)
                    {
                        Proxy.giterr_set_str(GitErrorCategory.Reference, ex);
                    }
                }

                return (int)GitErrorCode.Error;
            }

            private static int Lookup(
                out IntPtr referencePtr,
                IntPtr backend,
                IntPtr namePtr)
            {
                RefdbBackend refdbBackend = GCHandle.FromIntPtr(Marshal.ReadIntPtr(backend, GitRefdbBackend.GCHandleOffset)).Target as RefdbBackend;

                ReferenceDatabaseSafeHandle refdbHandle = refdbBackend.Repository.ReferenceDatabase.Handle;

                referencePtr = (IntPtr)0;

                if (refdbBackend != null)
                {
                    try
                    {
                        string referenceName = Utf8Marshaler.FromNative(namePtr);

                        RefdbReference reference = refdbBackend.Lookup(referenceName);

                        if (reference == null)
                            return (int)GitErrorCode.NotFound;

                        if (reference.Type == GitReferenceType.Symbolic)
                        {
                            referencePtr = Proxy.git_reference__alloc(refdbHandle, referenceName, null, reference.Symbolic);
                        }
                        else
                        {
                            referencePtr = Proxy.git_reference__alloc(refdbHandle, referenceName, reference.Oid, null);
                        }

                        if (referencePtr != (IntPtr)0)
                            return 0;
                    }
                    catch (Exception ex)
                    {
                        Proxy.giterr_set_str(GitErrorCategory.Reference, ex);
                    }
                }

                return (int)GitErrorCode.Error;
            }

            private static int Foreach(
                IntPtr backend,
                UIntPtr list_flags,
                GitRefdbBackend.foreach_callback_callback callback,
                IntPtr data)
            {
                return (int)GitErrorCode.Error;
            }

            private static int ForeachGlob(
                IntPtr backend,
                IntPtr globPtr,
                UIntPtr list_flags,
                GitRefdbBackend.foreach_callback_callback callback,
                IntPtr data)
            {
                return (int)GitErrorCode.Error;
            }

            private static int Write(
                IntPtr backend,
                IntPtr referencePtr)
            {
                RefdbBackend refdbBackend = GCHandle.FromIntPtr(Marshal.ReadIntPtr(backend, GitRefdbBackend.GCHandleOffset)).Target as RefdbBackend;
                ReferenceSafeHandle referenceHandle = GCHandle.FromIntPtr(Marshal.ReadIntPtr(referencePtr)).Target as ReferenceSafeHandle;
                Reference reference = Reference.BuildFromPtr<Reference>(referenceHandle, refdbBackend.Repository);

                if (refdbBackend != null)
                {
                    try
                    {
                        refdbBackend.Write(reference);
                        return 0;
                    }
                    catch (Exception ex)
                    {
                        Proxy.giterr_set_str(GitErrorCategory.Reference, ex);
                    }
                }

                return (int)GitErrorCode.Error;
            }

            private static int Delete(
                IntPtr backend,
                IntPtr referencePtr)
            {
                RefdbBackend refdbBackend = GCHandle.FromIntPtr(Marshal.ReadIntPtr(backend, GitRefdbBackend.GCHandleOffset)).Target as RefdbBackend;
                ReferenceSafeHandle referenceHandle = GCHandle.FromIntPtr(Marshal.ReadIntPtr(referencePtr)).Target as ReferenceSafeHandle;
                Reference reference = Reference.BuildFromPtr<Reference>(referenceHandle, refdbBackend.Repository);

                if (refdbBackend != null)
                {
                    try
                    {
                        refdbBackend.Delete(reference);
                        return 0;
                    }
                    catch (Exception ex)
                    {
                        Proxy.giterr_set_str(GitErrorCategory.Reference, ex);
                    }
                }

                return (int)GitErrorCode.Error;
            }

            private static int Compress(IntPtr backend)
            {
                RefdbBackend refdbBackend = GCHandle.FromIntPtr(Marshal.ReadIntPtr(backend, GitRefdbBackend.GCHandleOffset)).Target as RefdbBackend;

                if (refdbBackend != null)
                {
                    try
                    {
                        refdbBackend.Compress();
                    }
                    catch (Exception ex)
                    {
                        Proxy.giterr_set_str(GitErrorCategory.Reference, ex);
                    }
                }

                return (int)GitErrorCode.Error;
            }

            private static void Free(IntPtr backend)
            {
                RefdbBackend refdbBackend = GCHandle.FromIntPtr(Marshal.ReadIntPtr(backend, GitRefdbBackend.GCHandleOffset)).Target as RefdbBackend;
                refdbBackend.Free();
            }
        }

        /// <summary>
        ///   Flags used by subclasses of RefdbBackend to indicate which operations they support.
        /// </summary>
        [Flags]
        protected enum RefdbBackendOperations
        {
            /// <summary>
            ///   This RefdbBackend declares that it supports the Compress method.
            /// </summary>
            Compress = 1,

            /// <summary>
            ///   This RefdbBackend declares that it supports the ForeachGlob method.
            /// </summary>
            ForEachGlob = 2,
        }
    }
}
