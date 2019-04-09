using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace DeepCopy
{
    public static class MemoryOperation
    {
        public static unsafe string GetMemberAddress(Object obj)
        {
            int i;
            int* ptr = &i;
            IntPtr addr = (IntPtr)ptr;
            GCHandle h = GCHandle.Alloc(obj, GCHandleType.Pinned);
            addr = h.AddrOfPinnedObject();
            Console.WriteLine();
            h.Free();
            //获取地址
            return addr.ToString("x");
        }

        [StructLayout(LayoutKind.Sequential)]
        class Student
        {
            public int Age { get; set; }

            public string tt { get; set; }
        }
    }
}
