using System;

namespace PatchKit.Core.CSharp;

public delegate void HandleExceptionDelegate<T>(T e) where T : Exception;
