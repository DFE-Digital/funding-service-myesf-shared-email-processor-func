namespace Pds.Shared.EmailProcessor.Func.Exceptions
{
    /// <summary>
    /// Throttling exception.
    /// </summary>
    /// <seealso cref="System.Exception" />
    public class ThrottleException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThrottleException"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        public ThrottleException(string errorMessage)
            : base(errorMessage)
        {
        }
    }
}