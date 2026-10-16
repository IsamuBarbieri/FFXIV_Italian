using System.Runtime.InteropServices;
using Dalamud.Game.Text.SeStringHandling;

namespace FFXIVItalian.TextInspector;

internal static class NativeText
{
    // OriginalTextPointer may belong to another plugin. Never dereference it directly.
    public static byte[]? ReadBytes(nint address)
    {
        if (address == 0) return null;
        var buffer = new byte[4096];
        ReadProcessMemory(new nint(-1), address, buffer, (nuint)buffer.Length, out var bytesRead);
        var length = Array.IndexOf(buffer, (byte)0, 0, (int)bytesRead);
        if (length < 0) return null;
        return buffer[..length];
    }

    public static unsafe string? Read(nint address)
    {
        var buffer = ReadBytes(address);
        if (buffer is null) return null;
        try
        {
            fixed (byte* pointer = buffer)
                return TranslationIndex.CleanForDisplay(SeString.Parse(pointer, buffer.Length).TextValue);
        }
        catch (Exception)
        {
            return null;
        }
    }

    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool ReadProcessMemory(nint process, nint address, [Out] byte[] buffer, nuint size, out nuint bytesRead);
}
