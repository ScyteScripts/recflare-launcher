using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace Recovered;

public static class Worker
{
    static readonly JsonSerializerOptions Json = new() { IncludeFields = true };
    public static void Main()
    {
        // Recovered libraries log to Console. Reserve stdout exclusively for RPC.
        var output = Console.Out;
        Console.SetOut(Console.Error);
        string line;
        while ((line = Console.ReadLine()) != null)
        {
            object response;
            long id = 0;
            try
            {
                if (line.Length > 1024 * 1024) throw new InvalidDataException("RPC request is too large");
                using var doc = JsonDocument.Parse(line);
                var request = doc.RootElement;
                id = request.GetProperty("id").GetInt64();
                var methodName = request.GetProperty("method").GetString();
                var method = typeof(LibpkappsNet).GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .SingleOrDefault(m => m.Name == methodName && (m.ReturnType == typeof(string) || m.Name == "initLibpkappsNet"));
                if (method == null) throw new InvalidDataException("Unknown updater operation");
                var supplied = request.GetProperty("args").EnumerateArray().ToArray();
                var parameters = method.GetParameters();
                var args = new object[parameters.Length];
                int next = 0, reference = -1;
                for (int i = 0; i < parameters.Length; i++)
                {
                    var type = parameters[i].ParameterType;
                    if (type.IsByRef)
                    {
                        reference = i;
                        args[i] = Activator.CreateInstance(type.GetElementType());
                    }
                    else
                    {
                        if (next >= supplied.Length) throw new InvalidDataException("Missing operation argument");
                        args[i] = JsonSerializer.Deserialize(supplied[next++].GetRawText(), type, Json);
                    }
                }
                if (next != supplied.Length) throw new InvalidDataException("Unexpected operation argument");
                var error = method.Invoke(null, args);
                response = new { id, error, result = reference < 0 ? null : args[reference] };
            }
            catch (Exception error)
            {
                Console.Error.WriteLine(error);
                response = new { id, error = "worker_rpc_failure", result = (object)null };
            }
            output.WriteLine(JsonSerializer.Serialize(response, Json));
            output.Flush();
        }
    }
}
