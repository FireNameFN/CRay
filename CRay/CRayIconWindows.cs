using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using System.Threading;
using System.Threading.Tasks;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace CRay;

public sealed class CRayIconWindows : ICRayIcon {
    public bool Visible {
        get => visible;
        set {
            visible = value;

            UpdateStatus();
        }
    }
    bool visible = true;

    public bool Attention {
        get => attention;
        set {
            attention = value;

            UpdateStatus();
        }
    }
    bool attention;

    public string IconsPath {
        get => iconsPath;
        set {
            if(iconsPath == value)
                return;

            iconsPath = value;
            
            Invoke(() => {
                _ = NativeWindows.DestroyIcon(icon);
                _ = NativeWindows.DestroyIcon(attentionIcon);

                icon = GetIcon(IconName);
                attentionIcon = GetIcon(AttentionIconName);

                ModifyIcon();
            });
        }
    }
    string iconsPath;


    public string IconName {
        get => iconName;
        set {
            if(iconName == value)
                return;

            iconName = value;

            Invoke(() => {
                _ = NativeWindows.DestroyIcon(icon);

                icon = GetIcon(IconName);

                ModifyIcon();
            });
        }
    }
    string iconName;

    public string AttentionIconName {
        get => attentionIconName;
        set {
            if(attentionIconName == value)
                return;

            attentionIconName = value;

            Invoke(() => {
                _ = NativeWindows.DestroyIcon(attentionIcon);

                attentionIcon = GetIcon(AttentionIconName);

                ModifyIcon();
            });
        }
    }
    string attentionIconName;

    int status;

    nint hwnd;

    nint menu;

    nint icon;

    nint attentionIcon;

    uint id = 1;

    readonly Dictionary<uint, Action> menuItemActions = [];

    static int globalClassId;

    int classId;

    Action invoke;

    public unsafe CRayIconWindows(string iconsPath, string iconName) {
        this.iconsPath = iconsPath;
        this.iconName = iconName;

        Task.Factory.StartNew(Loop, TaskCreationOptions.LongRunning);
    }

    public unsafe void AddMenuItem(string label, Action action) {
        Invoke(() => {
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
        });
    }

    unsafe void Loop() {
        nint proc = Marshal.GetFunctionPointerForDelegate((NativeWindows.WndProc)Proc);

        classId = Interlocked.Increment(ref globalClassId);

        fixed(char* className = &Utf16StringMarshaller.GetPinnableReference($"CRAY-{classId}")) {
            NativeWindows.ClassEx classEx = new() {
                Size = (uint)sizeof(NativeWindows.ClassEx),
                Proc = proc,
                Instance = NativeWindows.GetModuleHandleW(null),
                ClassName = (nint)className
            };

            NativeWindows.RegisterClassExW(&classEx);
        }

        hwnd = NativeWindows.CreateWindowExW(0, $"CRAY-{classId}", null, 0, 0, 0, 0, 0, 0, 0, 0, 0);

        menu = NativeWindows.CreatePopupMenu();

        icon = GetIcon(iconName);

        ModifyIcon(true);

        NativeWindows.Msg msg;

        while(NativeWindows.GetMessageW(&msg, 0, 0, 0) != 0) {
            _ = NativeWindows.TranslateMessage(&msg);

            NativeWindows.DispatchMessageW(&msg);

            invoke?.Invoke();
            invoke = null;
        }
    }

    void Invoke(Action action) {
        invoke += action;

        _ = NativeWindows.PostMessageW(hwnd, 0, 0, 0);
    }

    unsafe nint GetIcon(string name) {
        string file = $"{name}.png";

        if(!File.Exists(file))
            return 0;

        using Image<Bgra32> image = Image.Load<Bgra32>(Path.Combine(IconsPath, file));

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

        nint icon = NativeWindows.CreateIconIndirect(iconInfo);

        _ = NativeWindows.DeleteObject(bitmap);

        return icon;
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

    unsafe void UpdateStatus() {
        Invoke(() => {
            int status = Visible ? Attention ? 2 : 1 : 0;

            if(this.status == status)
                return;

            if(status == 0 && this.status > 0) {
                NativeWindows.NotifyIconData data = new() {
                    Size = (uint)sizeof(NativeWindows.NotifyIconData),
                    Hwnd = hwnd
                };

                _ = NativeWindows.Shell_NotifyIconW(2, data);
                
                return;
            }

            ModifyIcon(status > 0 && this.status == 0);

            this.status = status;
        });
    }

    unsafe void ModifyIcon(bool create = false) {
        if(!Visible)
            return;

        NativeWindows.NotifyIconData data = new() {
            Size = (uint)sizeof(NativeWindows.NotifyIconData),
            Hwnd = hwnd,
            Flags = 3,
            CallbackMessage = 0x400,
            Icon = Attention ? attentionIcon : icon
        };

        _ = NativeWindows.Shell_NotifyIconW(create ? 0u : 1u, data);
    }

    public unsafe void Dispose() {
        Invoke(invoke += () => {
            if(Visible) {
                NativeWindows.NotifyIconData data = new() {
                    Size = (uint)sizeof(NativeWindows.NotifyIconData),
                    Hwnd = hwnd
                };

                _ = NativeWindows.Shell_NotifyIconW(2, data);
            }

            _ = NativeWindows.DestroyIcon(icon);
            _ = NativeWindows.DestroyMenu(menu);
            _ = NativeWindows.DestroyWindow(hwnd);
            _ = NativeWindows.UnregisterClassW($"CRAY-{classId}", 0);
        });
    }
}
