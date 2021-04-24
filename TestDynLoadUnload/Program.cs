using System;
using System.Runtime.InteropServices;

namespace TestDynLoadUnload
{
    class Program
    {
        //[DllImport("Worker1.dll", CharSet = CharSet.Unicode)]
        //public static extern int some_operation(int a, int b);

        //[DllImport("Worker2.dll", CharSet = CharSet.Unicode)]
        //public static extern int some_operation(int a, int b);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        [DllImport("kernel32.dll")]
        static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll")]
        static extern bool LoadLibraryA(string hModule);

        [DllImport("kernel32.dll")]
        static extern bool GetModuleHandleExA(int dwFlags, string ModuleName, ref IntPtr phModule);

        public delegate int some_operation(int a, int b);

        static void UnLoadDll(string name)
        {
            var hMod = IntPtr.Zero;
            var ret = GetModuleHandleExA(0, name, ref hMod);
            if (!ret)
                return;

            bool rc;
            int count = 0;
            do
            {
                rc = FreeLibrary(hMod);
            }
            while (rc && ++count < 10);
            if (count >= 10)
                throw new Exception($"Unload fialed {name}");
        }

        static void TestFromDll(string name)
        {
            var ret1 =  LoadLibraryA($"{name}.dll");
            if (!ret1)
                throw new Exception($"Unable to load {name}.dll");

            var hMod = IntPtr.Zero;
            var ret = GetModuleHandleExA(0, name, ref hMod);
            if (!ret)
                throw new Exception($"Unable to get module {name}");

            IntPtr funcaddr = GetProcAddress(hMod, "some_operation");
            if (funcaddr==IntPtr.Zero)
                throw new Exception($"Unable to get function {name}");

            var function = Marshal.GetDelegateForFunctionPointer(funcaddr, typeof(some_operation)) as some_operation;
            var result = function.Invoke(5, 3);
            Console.WriteLine(result);
        }

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Hello World!");

               // TestFromDll("Worker3");

                TestFromDll("Worker1");
                UnLoadDll("Worker1");
                UnLoadDll("Corecpp");
                TestFromDll("Worker2");
                UnLoadDll("Worker2");
                UnLoadDll("Corecpp");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
