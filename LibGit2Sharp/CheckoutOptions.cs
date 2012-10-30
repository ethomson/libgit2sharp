using LibGit2Sharp.Core;

namespace LibGit2Sharp
{
    /// <summary>
    /// Defines the checkout strategy
    /// </summary>
    public enum CheckoutStrategy
    {
        /// <summary>
        /// Checkout does not update any files in the working directory.
        /// </summary>
        Default = 1 << 0,

        /// <summary>
        /// When a file exists and is modified, replace it with a new version.
        /// </summary>
        OverwriteModified = 1 << 1,

        /// <summary>
        /// When a file does not exist in the working directory, create it.
        /// </summary>
        CreateMissing = 1 << 2,

        /// <summary>
        /// If an untracked file is found in the working dir, delete it.
        /// </summary>
        RemoveUntracked = 1 << 3,
    }
    
    /// <summary>
    /// Encapsulates options for a checkout operation
    /// </summary>
    public class CheckoutOptions
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public CheckoutOptions()
        {
            checkoutOptions = new GitCheckoutOptions();
            CheckoutStrategy =
                CheckoutStrategy.Default | 
                CheckoutStrategy.CreateMissing;
        }

        /// <summary>
        /// Strategy options for checkout
        /// </summary>
        public CheckoutStrategy CheckoutStrategy
        {
            get { return (CheckoutStrategy)checkoutOptions.CheckoutStrategy; }
            set { checkoutOptions.CheckoutStrategy = (uint) value; }
        }

        /// <summary>
        /// If true, disable filters for checkout
        /// </summary>
        public bool DisableFilters
        {
            get { return checkoutOptions.DisableFilters != 0; }
            set { checkoutOptions.DisableFilters = value ? 1 : 0; }
        }

        /// <summary>
        /// Creation mode for directories
        /// </summary>
        public int DirectoryMode
        {
            get { return checkoutOptions.DirMode; }
            set { checkoutOptions.DirMode = value; }
        }

        /// <summary>
        /// Creation mode for files
        /// </summary>
        public int FileMode
        {
            get { return checkoutOptions.FileMode; }
            set { checkoutOptions.FileMode = value; }
        }

        /// <summary>
        /// Flags to pass as the second parameter to open()
        /// </summary>
        public int FileOpenFlags
        {
            get { return checkoutOptions.FileOpenFlags; }
            set { checkoutOptions.FileOpenFlags = value; }
        }

        #region Fields

        internal GitCheckoutOptions checkoutOptions;

        #endregion
    }
}