using System.Diagnostics;
using System.Runtime.InteropServices;

namespace VM;

internal class NativeMemoryManager
{
    #region WINAPI IMPORTS
    [DllImport("kernel32")]
    public static extern IntPtr VirtualAlloc(IntPtr lpAddress, nuint dwSize, nuint flAllocationType, nuint flProtect);

    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    static extern bool VirtualFree(IntPtr lpAddress, int dwSize, nuint dwFreeType);

    [DllImport("kernel32.dll", SetLastError = false)]
    static extern IntPtr HeapAlloc(IntPtr hHeap, uint dwFlags, UIntPtr dwBytes);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool HeapFree(IntPtr hHeap, uint dwFlags, IntPtr lpMem);
    #endregion

    #region LIBC IMPORTS
    [DllImport("libc", EntryPoint = "malloc")]
    internal static extern IntPtr Malloc(IntPtr size);

    [DllImport("libc", EntryPoint = "free")]
    private static unsafe extern void free(nint* ptr);

    [DllImport("libc", EntryPoint = "errno")]
    private static extern int errono();
    #endregion

    /// <summary>
    /// Specificies the max region size the manager can allocate.
    /// </summary>
    private static readonly nuint MAX_REGION_SIZE = Environment.Is64BitProcess ? (nuint)0x4000 : (nuint)0x1000;

    /// <summary>
    /// Retrieves the last error code the system omitted.
    /// </summary>
    private static int LastErrorCode => RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? Marshal.GetLastWin32Error() : errono();

    internal NativeMemoryManager()
    {
        
    }

    /// <summary>
    /// Allocates a new region in a random page.
    /// </summary>
    /// <param name="regionSize">Size of the region to allocate</param>
    /// <returns>a pointer to a newly allocated region in the memory. If region size is 0, a null pointer will be returned.</returns>
    internal static unsafe nint* AllocateNewRegion(nuint regionSize)
    {
        //Check if requested region size is 0
        if(regionSize == 0x0)
            return null;

        //Check if the requested region size is out of the permittable pageable range.
        if (regionSize > MAX_REGION_SIZE)
            return null;

        //Intereop call to VirtualAlloc
        //Access Flags -> R, W, X
        nint* allocatedRegion = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
                                (nint*)VirtualAlloc(IntPtr.Zero, (uint)regionSize, 0x1000 | 0x2000, 0x40)
                                : (nint*)Malloc((nint)regionSize);

        //Handle the allocation failure
#if DEBUG
        Debug.Assert(allocatedRegion != null);
#endif
#if RELEASE
        if(allocatedRegion == null)
        {
            Console.WriteLine($"{LastErrorCode}");
            return null;
        }
#endif

        return allocatedRegion;
    }

    /// <summary>
    /// Allocates a new heap.
    /// </summary>
    /// <param name="heapSize">Size of the region to allocate</param>
    /// <returns>a pointer to a newly allocated heap in the memory. If heap size is 0, a null pointer will be returned.</returns>
    internal static unsafe nint* AllocateNewHeap(nuint heapSize)
    {
        //Check if requested heap size is 0
        if (heapSize == 0x0)
            return null;

        //Check if the requested heap size is out of the permittable pageable range.
        if (heapSize > MAX_REGION_SIZE)
            return null;

        //Intereop call to HeapAlloc
        //Access Flags -> R, W, X
        nint* allocatedHeap = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ?
                                (nint*)HeapAlloc(0, 0x1000 | 0x2000, (uint)heapSize)
                                : (nint*)Malloc((nint)heapSize);

        //Handle the allocation failure
#if DEBUG
        Debug.Assert(allocatedHeap != null);
#endif
#if RELEASE
        if(allocatedHeap == null)
        {
            Console.WriteLine($"{LastErrorCode}");
            return null;
        }
#endif

        return allocatedHeap;
    }

    /// <summary>
    /// Deallocates a region by the supplied pointer.
    /// </summary>
    /// <param name="regionPointer">a pointer to the region to be deallocated</param>
    /// <returns>a boolean defining the result of the deallocation</returns>
    internal static unsafe bool DeallocateRegion(ref nint* regionPointer)
    {
        if(regionPointer == null)
            return false;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            if (!VirtualFree((nint)regionPointer, 0, 0x00008000 /*MEM_RELEASE*/))
                return false;

            regionPointer = null;
        }
        else
        {
            free(regionPointer);
            regionPointer = null;
        }

        return regionPointer == null;
    }

    /// <summary>
    /// Deallocates a heap by the supplied pointer.
    /// </summary>
    /// <param name="heapPointer">a pointer to the heap to be deallocated</param>
    /// <returns>a boolean defining the result of the deallocation</returns>
    internal static unsafe bool DeallocateHeap(ref nint* heapPointer)
    {
        if (heapPointer == null)
            return false;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            if (!HeapFree((nint)heapPointer, 0, 0x00008000 /*MEM_RELEASE*/))
                return false;

            heapPointer = null;
        }
        else
        {
            free(heapPointer);
            heapPointer = null;
        }

        return heapPointer == null;
    }
}