using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace CRay;

public sealed class CRayIconLinux : ICRayIcon {
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

            NativeLinux.app_indicator_set_icon_theme_path(indicator, value);
        }
    }
    string iconsPath;

    public string IconName {
        get => iconName;
        set {
            if(iconName == value)
                return;

            iconName = value;

            NativeLinux.app_indicator_set_icon(indicator, value);
        }    
    }
    string iconName;

    public string AttentionIconName {
        get => attentionIconName;
        set {
            if(attentionIconName == value)
                return;

            attentionIconName = value;

            NativeLinux.app_indicator_set_icon(indicator, value);
        }    
    }
    string attentionIconName;

    readonly nint indicator;

    readonly nint menu;

    int status;

    public CRayIconLinux(string iconsPath, string iconName) {
        indicator = NativeLinux.app_indicator_new_with_path("test", iconName, 0, iconsPath);

        menu = NativeLinux.gtk_menu_new();

        NativeLinux.app_indicator_set_menu(indicator, menu);

        UpdateStatus();
    }

    public CRayIconLinux(string iconName = "icon") : this(AppContext.BaseDirectory, iconName) { }

    public static void Initialize() {
        NativeLinux.gtk_init(0, 0);

        Task.Factory.StartNew(NativeLinux.gtk_main, TaskCreationOptions.LongRunning);
    }

    public unsafe void AddMenuItem(string label, Action action) {
        nint item = NativeLinux.gtk_menu_item_new_with_mnemonic(label);

        NativeLinux.gtk_widget_show(item);

        nint handler = Marshal.GetFunctionPointerForDelegate(action);

        NativeLinux.g_signal_connect_data(item, "button-press-event", handler, 0, 0, 0);

        NativeLinux.gtk_menu_shell_append(menu, item);
    }

    void UpdateStatus() {
        int status = Visible ? Attention ? 2 : 1 : 0;

        if(this.status == status)
            return;

        this.status = status;

        NativeLinux.app_indicator_set_status(indicator, status);
    }

    public void Dispose() {
        Visible = false;

        NativeLinux.gtk_widget_destroy(menu);
    }
}
