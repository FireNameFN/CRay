using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace CRay;

public sealed class CRayIconWindows : IDisposable {
    nint hwnd;

    nint menu;

    nint icon;

    uint id = 1;

    readonly Dictionary<uint, Action> menuItemActions = [];

    static int classId;

    Action invoke;

    public unsafe CRayIconWindows(string file) {
        Task.Factory.StartNew(() => Loop(file), TaskCreationOptions.LongRunning);
    }

    public unsafe void AddMenuItem(string label, Action action) {
        invoke += () => {
            menuItemActions.Add(id, action);

            fixed(void* data = &Utf16StringMarshaller.GetPinnableReference(label)) {
                NativeWindows.MenuItemInfo menuItemInfo = new() {
                    Size = (uint)sizeof(NativeWindows.MenuItemInfo),
                    Mask = 0x42,
                    Id = id,
                    TypeData = (nint)data
                };

                _ = NativeWindows.InsertMenuItemW(menu, id++, 1, &menuItemInfo);
            }
        };

        _ = NativeWindows.PostMessageW(hwnd, 0, 0, 0);
    }

    public unsafe void Loop(string file) {
        NativeWindows.WndProc procDelegate = Proc;

        nint proc = Marshal.GetFunctionPointerForDelegate(procDelegate);

        fixed(char* className = &Utf16StringMarshaller.GetPinnableReference($"CRAY-{classId}")) {
            NativeWindows.ClassEx classEx = new() {
                Size = (uint)sizeof(NativeWindows.ClassEx),
                Proc = proc,
                Instance = NativeWindows.GetModuleHandleW(null),
                ClassName = (nint)className
            };

            NativeWindows.RegisterClassExW(&classEx);
        }

        hwnd = NativeWindows.CreateWindowExW(0, $"CRAY-{classId++}", null, 0, 0, 0, 0, 0, 0, 0, 0, 0);

        menu = NativeWindows.CreatePopupMenu();

        using Image<Bgra32> image = Image.Load<Bgra32>(file);

        NativeWindows.BitmapInfo bitmapInfo = new() {
            Size = (uint)(sizeof(NativeWindows.BitmapInfo) - sizeof(nint)),
            Width = image.Width,
            Height = -image.Height,
            Planes = 1,
            BitCount = 32,
        };

        nint hdc = NativeWindows.GetDC(hwnd);

        nint bits;

        nint bitmap = NativeWindows.CreateDIBSection(hdc, &bitmapInfo, 0, &bits, 0, 0);

        Span<byte> span = stackalloc byte[image.Width * image.Height * sizeof(Bgra32)];
        
        image.CopyPixelDataTo(span);

        span.CopyTo(new(bits.ToPointer(), span.Length));

        NativeWindows.IconInfo iconInfo = new() {
            Icon = 1,
            Color = bitmap,
            Mask = bitmap
        };

        icon = NativeWindows.CreateIconIndirect(iconInfo);

        _ = NativeWindows.DeleteObject(bitmap);

        NativeWindows.NotifyIconData data = new() {
            Size = (uint)sizeof(NativeWindows.NotifyIconData),
            Hwnd = hwnd,
            Flags = 3,
            CallbackMessage = 0x400,
            Icon = icon
        };

        _ = NativeWindows.Shell_NotifyIconW(0, data);

        NativeWindows.Msg msg;

        Console.WriteLine("loop");

        while(NativeWindows.GetMessageW(&msg, 0, 0, 0) != 0) {
            Console.WriteLine($"msg");

            _ = NativeWindows.TranslateMessage(&msg);

            NativeWindows.DispatchMessageW(&msg);

            invoke?.Invoke();
            invoke = null;
        }

        Console.WriteLine("End");
    }

    unsafe nint Proc(nint hwnd, uint message, nint wParam, nint lParam) {
        switch(message) {
            case 0x010:
                _ = NativeWindows.DestroyWindow(hwnd);
                return 0;
            case 0x002:
                NativeWindows.PostQuitMessage(0);
                return 0;
            case 0x400 when lParam is 0x202 or 0x205:
                NativeWindows.Point point;

                _ = NativeWindows.GetCursorPos(&point);

                _ = NativeWindows.SetForegroundWindow(hwnd);

                int cmd = NativeWindows.TrackPopupMenu(menu, 0x1A2, point.X, point.Y, 0, hwnd, 0);

                NativeWindows.SendMessageW(hwnd, 0x111, cmd, 0);

                return 0;
            case 0x111:
                if(menuItemActions.TryGetValue((uint)wParam, out Action action))
                    action();

                return 0;
        }

        return NativeWindows.DefWindowProcW(hwnd, message, wParam, lParam);
    }

    public unsafe void Dispose() {
        invoke += () => {
            NativeWindows.NotifyIconData data = new() {
                Size = (uint)sizeof(NativeWindows.NotifyIconData),
                Hwnd = hwnd
            };

            _ = NativeWindows.Shell_NotifyIconW(2, data);

            _ = NativeWindows.DestroyIcon(icon);
            _ = NativeWindows.DestroyMenu(menu);
            _ = NativeWindows.DestroyWindow(hwnd);
            _ = NativeWindows.UnregisterClassW("CRAY", 0);
        };

        _ = NativeWindows.PostMessageW(hwnd, 0, 0, 0);
    }
}
