using Recovered;

// Verify launcher endpoint routing without making network requests.
var routing = typeof(ManagedDelta).Assembly.GetType("StudioPatch.ServiceRouting")!;
var resolve = routing.GetMethod("ResolveUrl")!;
var oldProfile = Environment.GetEnvironmentVariable("RECROOM_STUDIO_PATCH_CONFIG");
var routingProfile = Path.Combine(Path.GetTempPath(), "studio-routing-"+Guid.NewGuid().ToString("N")+".ini");
File.WriteAllText(routingProfile, "");
try {
 Environment.SetEnvironmentVariable("RECROOM_STUDIO_PATCH_CONFIG", routingProfile);
 foreach(var (input, expected) in new[] {
  ("https://ns.rec.net/api/config", "https://ns.recflare.net/api/config"),
  ("https://recroom.com/terms-of-service", "https://recflare.net/terms-of-service"),
  ("https://img.rec.net/image?x=1", "https://img.recflare.net/image?x=1"),
  ("https://notrecroom.com/path", "https://notrecroom.com/path"),
  ("https://rec.net.example.com/path", "https://rec.net.example.com/path"),
  ("https://api2.patchkit.net/1/apps/a", "https://api2.patchkit.net/1/apps/a")
 }) if((string)resolve.Invoke(null,new object[]{input})! != expected)throw new Exception("Routing mismatch: "+input);
 Console.WriteLine("Passed 6 managed endpoint routing fixtures.");
 var testHash=Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(System.Text.Encoding.UTF8.GetBytes("test-template"))).ToLowerInvariant();
 File.WriteAllText(routingProfile,"[Network]\nPatchKitAppSecretSha256="+testHash+"\n");
 foreach(var host in new[]{"api2.patchkit.net","api-cache.patchkit.net"}) {
  var input="https://"+host+"/1/apps/test-template/versions/latest/id";
  if((string)resolve.Invoke(null,new object[]{input})! != "https://studio.recflare.net/1/apps/test-template/versions/latest/id")throw new Exception("Missing default distribution routing");
 }
 Console.WriteLine("Passed 2 blank-profile distribution routing fixtures.");
} finally { Environment.SetEnvironmentVariable("RECROOM_STUDIO_PATCH_CONFIG", oldProfile); File.Delete(routingProfile); }


var temporaryParent = System.IO.Path.GetFullPath(System.IO.Path.GetTempPath());
var root = System.IO.Path.GetFullPath(System.IO.Path.Combine(temporaryParent,"studio-delta-tests-"+Guid.NewGuid().ToString("N")));
if(!root.StartsWith(temporaryParent,StringComparison.OrdinalIgnoreCase))throw new Exception("Invalid fixture directory");
Directory.CreateDirectory(root);
try
{
    var basis = System.IO.Path.Combine(root,"basis");
    var delta = System.IO.Path.Combine(root,"delta");
    var output = System.IO.Path.Combine(root,"output");
    File.WriteAllBytes(basis,"abcdefghijklmnopqrstuvwxyz"u8.ToArray());
    int count=0;
    void Check(byte[] instructions,byte[] expected)
    {
        File.WriteAllBytes(delta,new byte[]{0x72,0x73,0x02,0x36}.Concat(instructions).ToArray());
        ManagedDelta.Apply(basis,delta,output);
        if(!File.ReadAllBytes(output).SequenceEqual(expected))throw new Exception("Delta output mismatch");
        File.Delete(output);count++;
    }
    Check(new byte[]{3,(byte)'x',(byte)'y',(byte)'z',0},"xyz"u8.ToArray());
    for(int width=0;width<4;width++)
    {
        var bytes=new List<byte>{(byte)(0x41+width)};
        bytes.AddRange(new byte[(1<<width)-1]);bytes.Add(3);bytes.AddRange("xyz"u8.ToArray());bytes.Add(0);
        Check(bytes.ToArray(),"xyz"u8.ToArray());
    }
    for(int a=0;a<4;a++)for(int b=0;b<4;b++)
    {
        var bytes=new List<byte>{(byte)(0x45+4*a+b)};
        bytes.AddRange(new byte[(1<<a)-1]);bytes.Add(2);
        bytes.AddRange(new byte[(1<<b)-1]);bytes.Add(4);bytes.Add(0);
        Check(bytes.ToArray(),"cdef"u8.ToArray());
    }
    foreach(var malformed in new byte[][]{new byte[]{0x55,0},new byte[]{0x45,250,10,0},new byte[]{3,1},new byte[]{0,1},new byte[]{0x44,255,255,255,255,255,255,255,255}})
    {
        File.WriteAllBytes(delta,new byte[]{0x72,0x73,0x02,0x36}.Concat(malformed).ToArray());
        try{ManagedDelta.Apply(basis,delta,output);throw new Exception("Malformed delta accepted");}
        catch(Exception error) when(error is IOException || error is InvalidDataException){if(File.Exists(output))throw new Exception("Malformed delta left output");count++;}
    }
    // Existing output must survive a rejected patch; no overwrite or cleanup of it.
    File.WriteAllText(output,"preserve");
    try{ManagedDelta.Apply(basis,delta,output);throw new Exception("Existing output overwritten");}
    catch(IOException){if(File.ReadAllText(output)!="preserve")throw new Exception("Existing output lost");count++;}
    Console.WriteLine($"Passed {count} managed delta protocol fixtures.");
}
finally { Directory.Delete(root,true); }
