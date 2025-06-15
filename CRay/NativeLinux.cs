using System.Runtime.InteropServices;

namespace CRay;

static partial class NativeLinux {
    const string LibraryName = "libayatana-appindicator3.so.1";

    [LibraryImport(LibraryName)]
    public static partial nint gtk_init(int count, nint args);

    [LibraryImport(LibraryName)]
    public static partial void gtk_main();

    [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf8)]
    public static partial void g_signal_connect_data(nint instance, string signal, nint handler, nint data, nint destroy, int flags);

    [LibraryImport(LibraryName)]
    public static partial nint gtk_menu_new();

    [LibraryImport(LibraryName)]
    public static partial void gtk_menu_shell_append(nint menu, nint item);

    [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf8)]
    public static partial nint gtk_menu_item_new_with_mnemonic(string label);

    [LibraryImport(LibraryName)]
    public static partial void gtk_widget_show(nint widget);

    [LibraryImport(LibraryName)]
    public static partial void gtk_widget_destroy(nint widget);

    [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf8)]
    public static partial nint app_indicator_new_with_path(string id, string name, int category, string path);

    [LibraryImport(LibraryName)]
    public static partial void app_indicator_set_status(nint indicator, int status);

    [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf8)]
    public static partial void app_indicator_set_icon_theme_path(nint indicator, string path);

    [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf8)]
    public static partial void app_indicator_set_icon(nint indicator, string name);

    [LibraryImport(LibraryName, StringMarshalling = StringMarshalling.Utf8)]
    public static partial void app_indicator_set_attention_icon(nint indicator, string name);

    [LibraryImport(LibraryName)]
    public static partial void app_indicator_set_menu(nint indicator, nint menu);
}
