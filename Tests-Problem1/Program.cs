namespace Tests_Problem1;
/* =========================================",
    " * Файл: ...\Tests_Problem1\Program.cs,
    " * Автор: Глушков Сергей,
    " * Дата: 03.08.2026,
    " * Описание: Программа реализует алгоритм сжатия строк, +
    " * на сам символ и количество его повторений, +
    " * он называется Run-Length Encoding (RLE) (или кодирование длин серий). 
    " * ========================================= */

using System;
using System.Text;
class Program
{
    static void Main()
    {
        // для проверки алгоритма можно было вводить строку с консоли или задать ее в тексте программы (так как условие
        // задачи  было задано, что уже дана конкретная строка текста). Иначе можно расширять исх.данные и из тип
        Console.WriteLine("Введите исходную строку (только маленькие латинские буквы):");
        string original_string = Console.ReadLine() ?? "";
        //      string original_string = "aaaaabbbccdddddeee";        
        string compressed_string = Compress(original_string);
        string decompressed_string = Decompress(compressed_string);

        Console.WriteLine($"\nСжатая строка:        {compressed_string}");
        Console.WriteLine($"Восстановленная строка: {decompressed_string}");
        Console.ReadLine(); // строка ожидания
    }
    // В C# простая реализация со строками через оператор + приводит к потерям памяти, так как 
    // строки (string) в C# неизменяемы (immutable).
    // любая конкатенация через оператор + в цикле (например, result += current + count)
    // создаёт новую строку в памяти.
    // Поэтому необходимо использовать класс StringBuilder из пространства имен System.Text,
    // который представляет собой изменяемую (mutable) последовательность символов.
    /*  ----------------------------------------------------------------------------------- */
    // 1 шаг алгоритма -  реализуем метод сжатия строки Compress
    static string Compress(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        // выделяем предварительно память для предотвращения реаллокаций
        StringBuilder compressed = new StringBuilder(input.Length);
        int count = 1;
        char currentChar = input[0];

        for (int i = 1; i < input.Length; i++)
        {
            // если следующий символ такой же, увеличиваем счетчик
            if (input[i] == currentChar)
            {
                count++;
            }
            else
            {
                compressed.Append(currentChar);
                if (count > 1)
                    compressed.Append(count);
                currentChar = input[i];
                count = 1; // сброс счетчика
            }
        }
        compressed.Append(currentChar);
        if (count > 1)
            compressed.Append(count);
        return compressed.ToString();
    }
    /*  ----------------------------------------------------------------------------------- */
    // 2 шаг алгоритма -  реализуем метод распаковки строки Decompress
    // обращаем внимание на корректный парсинг чисел - могут быть 2 и более знаков
    static string Decompress(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;

        StringBuilder decompressed = new StringBuilder();
        int i = 0;

        while (i < input.Length)
        {
            char currentChar = input[i];
            i++;
            // считываем число повторений, может состоять из нескольких цифр
            int count = 0;
            while (i < input.Length && char.IsDigit(input[i]))
            {
                count = count * 10 + (input[i] - '0');
                i++;
            }
            // заполяем объект StringBuilder decompressed count раз
            if (count == 0) count = 1;
            decompressed.Append(new string(currentChar, count));
        }
        return decompressed.ToString();
    }
}