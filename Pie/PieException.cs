﻿using System;
using System.Runtime.Serialization;

namespace Pie;

/// <summary>
/// General Pie-specific exceptions.
/// </summary>
public class PieException : Exception
{
    public PieException() { }
    protected PieException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    public PieException(string message) : base(message) { }
    public PieException(string message, Exception innerException) : base(message, innerException) { }
}