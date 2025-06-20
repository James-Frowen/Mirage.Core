﻿using System;
using System.Runtime.Serialization;

namespace Mirage
{
    /// <summary>
    /// Exception thrown if a guarded method is invoked incorrectly
    /// </summary>
    [Serializable]
    public class MethodInvocationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:MethodInvocationException"/> class
        /// </summary>
        public MethodInvocationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:MethodInvocationException"/> class
        /// </summary>
        /// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
        public MethodInvocationException(string message) : base(message)
        {
        }
    }
}
