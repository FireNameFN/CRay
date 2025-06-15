using System;
using System.Threading;
using CRay;

Console.WriteLine("Hello, World!");

CRayIcon.Initialize();

CRayIcon icon = new();

icon.AddMenuItem("Click", () => Console.WriteLine("Click"));

Console.WriteLine(2);

Thread.Sleep(Timeout.Infinite);
