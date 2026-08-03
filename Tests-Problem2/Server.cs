using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests_Problem2

{
    public static class Server
    {
        private static int count = 0; //общая переменная count
        private static readonly ReaderWriterLockSlim locker = new(LockRecursionPolicy.NoRecursion); //механизм синхронизации и безопасности

   // Метод GetCount() использует режим EnterReadLock /ExitReadLock
        public static int GetCount()
        {
            locker.EnterReadLock();
            try
            {
                return count;
            }
            finally
            {
                locker.ExitReadLock();
            }
        }
    // Метод AddToCount(int value) использует режим EnterWriteLock/ ExitWriteLock в блоке try/finally
        public static void AddToCount(int value)
        {
            locker.EnterWriteLock();
            try
            {
                Console.WriteLine("[Writer] Start writing...");
                Thread.Sleep(2000);
                count += value;
                Console.WriteLine("[Writer] Added {0} to count", value);
            }
            finally
            {
                locker.ExitWriteLock();
            }
        }
    }
}
