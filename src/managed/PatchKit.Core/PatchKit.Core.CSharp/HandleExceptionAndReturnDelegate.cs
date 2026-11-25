using System;

namespace PatchKit.Core.CSharp;

public delegate object HandleExceptionAndReturnDelegate<T>(T e) where T : Exception;
