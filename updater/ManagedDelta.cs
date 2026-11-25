using System;
using System.IO;

namespace Recovered;

// librsync delta wire format: https://github.com/librsync/librsync/blob/master/src/prototab.c
// Independent streaming implementation of the literal/copy instruction format.
public static class ManagedDelta
{
    public static void Apply(string basisPath, string deltaPath, string targetPath)
    {
        using var basis = File.OpenRead(basisPath);
        using var delta = File.OpenRead(deltaPath);
        if (ReadNumber(delta,4) != 0x72730236) throw new InvalidDataException("Invalid librsync delta header");
        bool created = false;
        try
        {
            using var target = new FileStream(targetPath,FileMode.CreateNew,FileAccess.Write,FileShare.None);
            created = true;
            var buffer = new byte[128 * 1024];
            while (true)
            {
                int op = delta.ReadByte();
                if (op < 0) throw new EndOfStreamException("Missing delta END opcode");
                if (op == 0) { if(delta.ReadByte()!=-1)throw new InvalidDataException("Trailing delta data"); break; }
                if (op <= 0x44)
                {
                    long length = op <= 0x40 ? op : ReadNumber(delta,1 << (op-0x41));
                    Copy(delta,target,length,buffer);
                }
                else if (op <= 0x54)
                {
                    int code = op-0x45;
                    long offset = ReadNumber(delta,1 << (code/4));
                    long length = ReadNumber(delta,1 << (code%4));
                    if(offset > basis.Length || length > basis.Length-offset)throw new InvalidDataException("Delta copy is outside basis file");
                    basis.Position = offset; Copy(basis,target,length,buffer);
                }
                else throw new InvalidDataException("Reserved delta opcode");
            }
        }
        catch { if(created)File.Delete(targetPath); throw; }
    }
    static long ReadNumber(Stream source,int size)
    {
        ulong value=0;
        for(int i=0;i<size;i++){int b=source.ReadByte();if(b<0)throw new EndOfStreamException();value=(value<<8)|(byte)b;}
        if(value>long.MaxValue)throw new InvalidDataException("Delta integer overflow");
        return (long)value;
    }
    static void Copy(Stream source,Stream target,long length,byte[] buffer)
    {
        while(length>0){int count=source.Read(buffer,0,(int)Math.Min(buffer.Length,length));if(count==0)throw new EndOfStreamException();target.Write(buffer,0,count);length-=count;}
    }
}
