/* =========================================",
    * Файл: ...\Tests_Problem2\Program.cs,
    * Автор: Глушков Сергей,
    * Дата: 03.08.2026,
    * Описание: Эта классическая задача называется «Читатели-писатели» (Readers-Writers Problem),
    *  конкретно в варианте с приоритетом писателей (writers-preference),
    *  чтобы читатели ждали окончания записи. 
    * Суть задачи:
    *  Потоки делятся на два типа:
    *  читатели (только вызывают GetCount)
    *  и писатели (вызывают AddToCount).
    *   Читатели могут работать вместе и одновременно.
    *   Писатели работают только по одному за раз. 
    *   Если идет запись, никто другой (ни читатель, ни писатель) работать не должен.
    *  Для решения на C#:
    *  Используется  класс ReaderWriterLockSlim, который создан специально для таких ситуаций.
    *  Метод GetCount оборачивается в блок чтения (EnterReadLock / ExitReadLock).
    *  Метод AddToCount оборачивается в блок записи (EnterWriteLock / ExitWriteLock).
    *  Этот инструмент автоматически дает читать многим сразу и блокирует всех, когда кто-то пишет.
    * ========================================= */
namespace Tests_Problem2
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    class Program
    {
        static void Main()
        {
            // Запускаем несколько читателей
            for (int i = 0; i < 5; i++)
            {
                int readerId = i;
                Task.Run(async () => // Используем async
                {
                    while (true)
                    {
                        int value = Server.GetCount();
                        Console.WriteLine($"[Reader {readerId}] Read count: {value}");

                        await Task.Delay(1000); // Освобождает поток пула на время ожидания
                    }
                });
            }

            // Запускаем асинхронного писателя
            var writerTask = Task.Run(async () =>
            {
                for (int i = 0; i < 10; i++)
                {
                    Server.AddToCount(1);
                    await Task.Delay(2000); // Не блокирует поток ОС в отличие от Thread.Sleep
                }
            });

            Console.ReadLine();
        }
    }
}