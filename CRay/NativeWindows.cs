using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CRay;

static partial class NativeWindows {
    const string LibraryName = "Shell32";

    [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf16)]
    public static partial nint GetModuleHandleW(string module);

    [LibraryImport(LibraryName)]
    public static unsafe partial ushort RegisterClassExW(ClassEx* classEx);

    [LibraryImport(LibraryName)]
    public static partial nint DefWindowProcW(nint hwnd, uint message, nint wParam, nint lParam);

    [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf16)]
    public static partial nint CreateWindowExW(uint exStyle, string className, string windowName, uint style, int x, int y, int width, int height, nint parent, nint menu, nint instance, nint param);

    [LibraryImport(LibraryName)]
    public static unsafe partial int GetMessageW(Msg* msg, nint hwnd, uint filterMin, uint filterMax);

    [LibraryImport(LibraryName)]
    public static unsafe partial int TranslateMessage(Msg* msg);

    [LibraryImport(LibraryName)]
    public static unsafe partial nint DispatchMessage(Msg* msg);

    [LibraryImport(LibraryName)]
    public static partial int DestroyWindow(nint hwnd);

    [LibraryImport(LibraryName)]
    public static partial void PostQuitMessage(int code);

    [LibraryImport(LibraryName)]
    public static unsafe partial int GetCursorPos(Point* point);

    [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf16)]
    public static unsafe partial uint ExtractIconExW(string file, int index, nint* large, nint* small, uint icons);

    [LibraryImport(LibraryName)]
    public static partial int Shell_NotifyIconW(uint message, NotifyIconData data);

    public struct ClassEx {
        public uint Size { get; set; }

        public uint Style { get; set; }

        public nint Proc { get; set; }

        public int ClsExtra { get; set; }

        public int WndExtra { get; set; }

        public nint Instance { get; set; }

        public nint Icon { get; set; }

        public nint Cursor { get; set; }

        public nint Background { get; set; }

        public nint MenuName { get; set; }

        public nint ClassName { get; set; }

        public nint IconSm { get; set; }
    }

    public struct Msg {
        public nint Hwnd { get; set; }

        public uint Message { get; set; }

        public nint WParam { get; set; }

        public nint LParam { get; set; }

        public uint Time { get; set; }

        public Point Point { get; set; }

        public uint Private { get; set; }
    }

    public struct Point {
        public int X { get; set; }

        public int Y { get; set; }
    }

    public struct NotifyIconData {
        public uint Size { get; set; }

        public nint Hwnd { get; set; }

        public uint Id { get; set; }

        public uint Flags { get; set; }

        public uint CallbackMessage { get; set; }

        public nint Icon { get; set; }

        public Tip Tip { get; set; }

        public uint State { get; set; }

        public uint StateMask { get; set; }

        public Info Info { get; set; }

        public uint Version { get; set; }

        public InfoTitle InfoTitle { get; set; }

        public uint InfoFlags { get; set; }

        public Guid GuidItem { get; set; }

        public nint BaloonIcon { get; set; }
    }

    [InlineArray(128)]
    public struct Tip {
        public char Char { get; set; }
    }

    [InlineArray(256)]
    public struct Info {
        public char Char { get; set; }
    }

    [InlineArray(64)]
    public struct InfoTitle {
        public char Char { get; set; }
    }
}
