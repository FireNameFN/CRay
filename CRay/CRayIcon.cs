using System;

namespace CRay;

public sealed class CRayIcon : IDisposable {
    public bool Visible { get => implementation.Visible; set => implementation.Visible = value; }

    readonly ICRayIcon implementation;

    public CRayIcon() {
        if(OperatingSystem.IsWindows())
            implementation = new CRayIconLinux();
        else if(OperatingSystem.IsLinux())
            implementation = new CRayIconLinux();
        else
            throw new NotSupportedException();
    }

    public void Dispose() {
        implementation.Dispose();
    }
}
