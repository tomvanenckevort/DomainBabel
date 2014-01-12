using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace DomainBabel
{
    /// <summary>
    /// HTML minification filter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class HtmlMinifyFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Result executing event handler in which HTML minification gets applied.
        /// </summary>
        /// <param name="filterContext">Filter context</param>
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            if (!filterContext.IsChildAction &&
                !filterContext.HttpContext.Response.IsRequestBeingRedirected &&                 
                filterContext.HttpContext.Request.RawUrl.EndsWith("/", StringComparison.OrdinalIgnoreCase))
            {
                filterContext.HttpContext.Response.Filter = new WhitespaceFilter(filterContext.HttpContext.Response.Filter);
            }

            base.OnResultExecuting(filterContext);
        }

        /// <summary>
        /// Stream which removes any whitespace.
        /// </summary>
        internal class WhitespaceFilter : Stream
        {
            /// <summary>
            /// Regular expression pattern to filter out the whitespace and line breaks.
            /// </summary>
            private static readonly Regex Pattern = new Regex(@"\s{2,}|\r|\n+", RegexOptions.Multiline | RegexOptions.Compiled);

            /// <summary>
            /// Stream instance used for reading and writing the whitespace filter.
            /// </summary>
            private readonly Stream stream;

            /// <summary>
            /// Initializes a new instance of the <see cref="WhitespaceFilter"/> class.
            /// </summary>
            /// <param name="stream">Stream on which the white space filter gets applied</param>
            public WhitespaceFilter(Stream stream)
            {
                this.stream = stream;
            }

            /// <summary>
            /// Whether stream supports reading.
            /// </summary>
            public override bool CanRead
            {
                get { return true; }
            }

            /// <summary>
            /// Whether stream supports seeking.
            /// </summary>
            public override bool CanSeek
            {
                get { return true; }
            }

            /// <summary>
            /// Whether stream supports writing.
            /// </summary>
            public override bool CanWrite
            {
                get { return true; }
            }

            /// <summary>
            /// Length of the stream.
            /// </summary>
            public override long Length
            {
                get { return 0; }
            }

            /// <summary>
            /// Get or set the current position of the stream.
            /// </summary>
            public override long Position { get; set; }

            /// <summary>
            /// Flushes the stream.
            /// </summary>
            public override void Flush()
            {
                this.stream.Flush();
            }

            /// <summary>
            /// Read a number of bytes and put them in a buffer.
            /// </summary>
            /// <param name="buffer">Buffer to put read bytes in</param>
            /// <param name="offset">Number of bytes to skip</param>
            /// <param name="count">Number of bytes to read</param>
            /// <returns>Number of bytes read</returns>
            public override int Read(byte[] buffer, int offset, int count)
            {
                return this.stream.Read(buffer, offset, count);
            }

            /// <summary>
            /// Set the current position of the stream.
            /// </summary>
            /// <param name="offset">Number of bytes to skip</param>
            /// <param name="origin">The reference point to use to obtain new position</param>
            /// <returns>Current position set</returns>
            public override long Seek(long offset, SeekOrigin origin)
            {
                return this.stream.Seek(offset, origin);
            }

            /// <summary>
            /// Set the length of the stream.
            /// </summary>
            /// <param name="value">Length to set</param>
            public override void SetLength(long value)
            {
                this.stream.SetLength(value);
            }

            /// <summary>
            /// Closes the stream.
            /// </summary>
            public override void Close()
            {
                this.stream.Close();
            }

            /// <summary>
            /// Write a number of bytes to stream, after applying white space filter.
            /// </summary>
            /// <param name="buffer">Buffer of bytes to write to stream</param>
            /// <param name="offset">Number of bytes to skip</param>
            /// <param name="count">Number of bytes to write</param>
            public override void Write(byte[] buffer, int offset, int count)
            {
                string content = Encoding.Default.GetString(buffer);

                content = Pattern.Replace(content, string.Empty);

                byte[] output = Encoding.Default.GetBytes(content);

                this.stream.Write(output, 0, output.GetLength(0));
            }
        }
    }
}