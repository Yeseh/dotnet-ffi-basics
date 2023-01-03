using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;

namespace Wrapper;

public class CObject : IDisposable
{
    private readonly ObjectHandle handle;

    private IntPtr innerHandle; 

    public CObject(string name)
    {
        innerHandle = IntPtr.Zero;
        var objPtr = Native.object_new(name, innerHandle);
        handle = new ObjectHandle(objPtr);
    }

    internal ObjectHandle NativeHandle
    {
        get
        {
            if (handle.IsInvalid)
            {
                throw new ObjectDisposedException(typeof(CObject).FullName);
            }

            return handle;
        }
    }

    public string Name
    {
        get
        {
            if (handle.IsInvalid)
            {
                throw new ObjectDisposedException(typeof(CObject).FullName);
            }

            var namePtr = Native.object_get_name(handle);
            var str = Marshal.PtrToStringAnsi(namePtr);

            return str; 
        }
    }

    public object? Inner
    {
        get
        {
            if (handle.IsInvalid)
            {
                throw new ObjectDisposedException(typeof(CObject).FullName);
            }

            var innerPtr = Native.object_get_inner(handle);

            return innerPtr == IntPtr.Zero 
                ? null 
                : GCHandle.FromIntPtr(innerPtr).Target;
        }

        set
        {
            if (handle.IsInvalid)
            {
                throw new ObjectDisposedException(typeof(CObject).FullName);
            }

            var oldInner = Native.object_get_inner(handle);

            innerHandle = value == null
                ? IntPtr.Zero
                : (IntPtr)GCHandle.Alloc(value);

            Native.object_set_inner(handle, innerHandle);

            if (oldInner != IntPtr.Zero)
            {
                GCHandle.FromIntPtr(oldInner).Free();
            }
        }
    }

    public void Dispose()
    {
        if (handle.IsInvalid)
        {
            handle.Dispose();
        }

        if (innerHandle != IntPtr.Zero)
        {
            GCHandle.FromIntPtr(innerHandle).Free();
        }
    }

    internal class ObjectHandle : SafeHandleZeroOrMinusOneIsInvalid 
    {
        public ObjectHandle(IntPtr handle)
            : base(true)
        {
            SetHandle(handle);
        }

        protected override bool ReleaseHandle()
        {
            Native.object_delete(handle);
            return true;
        }
    }

    static class Native
    {
        [DllImport("LibraryLib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr object_new(string library_name, IntPtr inner);

        [DllImport("LibraryLib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr object_get_name(ObjectHandle handle);
        
        [DllImport("LibraryLib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern void object_delete(IntPtr handle);
        
        [DllImport("LibraryLib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern void object_set_inner(ObjectHandle handle, IntPtr inner);
        
        [DllImport("LibraryLib.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern IntPtr object_get_inner(ObjectHandle handle);
    }
}
