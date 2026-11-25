using System;
using PatchKit.Api;
using PatchKit.Core;
using PatchKit.Network;

namespace PatchKit.Apps.Updating;

public static class TempApiUtils
{
	public static T MakeApiCall<T>(Func<Timeout, T> func)
	{
		Timeout arg = new Timeout(TimeSpan.FromSeconds(15.0));
		T result = default;
		bool flag = false;
		do
		{
			try
			{
				result = func(arg);
				flag = true;
			}
			catch (ApiServerConnectionException value)
			{
				Console.WriteLine(value);
			}
			catch (ConnectionFailure value2)
			{
				Console.WriteLine(value2);
			}
		}
		while (!flag);
		return result;
	}
}
