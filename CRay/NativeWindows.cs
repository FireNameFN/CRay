using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace CRay;

static partial class NativeWindows {
    const string KernelLibraryName = "Kernel32";

    const string UserLibraryName = "User32";

    const string GdiLibraryName = "Gdi32";

    const string ShellLibraryName = "Shell32";

    [LibraryImport(KernelLibraryName, StringMarshalling = StringMarshalling.Utf16)]
    public static partial nint GetModuleHandleW(string module);

    [LibraryImport(UserLibraryName)]
    public static unsafe partial ushort RegisterClassExW(ClassEx* classEx);

    [LibraryImport(UserLibraryName)]
    public static partial nint DefWindowProcW(nint hwnd, uint message, nint wParam, nint lParam);

    [LibraryImport(UserLibraryName, StringMarshalling = StringMarshalling.Utf16)]
    public static partial nint CreateWindowExW(uint exStyle, string className, string windowName, uint style, int x, int y, int width, int height, nint parent, nint menu, nint instance, nint param);

    [LibraryImport(UserLibraryName)]
    public static partial int UpdateWindow(nint hwnd);

    [LibraryImport(UserLibraryName)]
    public static unsafe partial int GetMessageW(Msg* msg, nint hwnd, uint filterMin, uint filterMax);

    [LibraryImport(UserLibraryName)]
    public static unsafe partial int TranslateMessage(Msg* msg);

    [LibraryImport(UserLibraryName)]
    public static unsafe partial nint DispatchMessageW(Msg* msg);

    [LibraryImport(UserLibraryName)]
    public static partial void PostQuitMessage(int code);

    [LibraryImport(UserLibraryName)]
    public static unsafe partial int GetCursorPos(Point* point);

    [LibraryImport(UserLibraryName)]
    public static partial int SetForegroundWindow(nint hwnd);

    [LibraryImport(UserLibraryName)]
    public static unsafe partial int TrackPopupMenu(nint menu, uint flags, int x, int y, int reserved, nint hwnd, nint rect);

    [LibraryImport(UserLibraryName)]
    public static partial nint SendMessageW(nint hwnd, uint msg, nint wParam, nint lParam);

    [LibraryImport(UserLibraryName)]
    public static partial nint GetDC(nint hwnd);

    [LibraryImport(UserLibraryName)]
    public static partial nint CreateIconIndirect(IconInfo iconInfo);

    [LibraryImport(UserLibraryName)]
    public static partial nint CreatePopupMenu();

    [LibraryImport(UserLibraryName)]
    public static unsafe partial int InsertMenuItemW(nint menu, uint item, int byPosition, MenuItemInfo* menuItemInfo);

    [LibraryImport(UserLibraryName)]
    public static partial int PostMessageW(nint hwnd, uint msg, nint wParam, nint lParam);

    [LibraryImport(UserLibraryName)]
    public static partial int DestroyIcon(nint icon);

    [LibraryImport(UserLibraryName)]
    public static partial int DestroyMenu(nint menu);

    [LibraryImport(UserLibraryName)]
    public static partial int DestroyWindow(nint hwnd);

    [LibraryImport(UserLibraryName, StringMarshalling = StringMarshalling.Utf16)]
    public static partial int UnregisterClassW(string className, nint instance);

    [LibraryImport(GdiLibraryName)]
    public static unsafe partial nint CreateDIBSection(nint hdc, BitmapInfo* info, uint usage, nint* bits, nint section, uint offset);

    [LibraryImport(GdiLibraryName)]
    public static partial int DeleteObject(nint obj);

    [LibraryImport(ShellLibraryName)]
    public static partial int Shell_NotifyIconW(uint message, NotifyIconData data);

    public delegate nint WndProc(nint hwnd, uint message, nint wParam, nint lParam);

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

    public struct IconInfo {
        public int Icon { get; set; }

        public uint X { get; set; }

        public uint Y { get; set; }

        public nint Mask { get; set; }

        public nint Color { get; set; }
    }

    public struct MenuItemInfo {
        public uint Size { get; set; }

        public uint Mask { get; set; }

        public uint Type { get; set; }

        public uint State { get; set; }

        public uint Id { get; set; }

        public nint SubMenu { get; set; }

        public nint Checked { get; set; }

        public nint Unchecked { get; set; }

        public nint ItemData { get; set; }

        public nint TypeData { get; set; }

        public uint Cch { get; set; }

        public nint Item { get; set; }
    }

    public struct BitmapInfo {
        public uint Size { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public ushort Planes { get; set; }

        public ushort BitCount { get; set; }

        public uint Compression { get; set; }

        public uint SizeImage { get; set; }

        public int XPerMeter { get; set; }

        public int YPerMeter { get; set; }

        public int ClrUser { get; set; }

        public int ClrImportant { get; set; }

        public nint Colors { get; set; }
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
