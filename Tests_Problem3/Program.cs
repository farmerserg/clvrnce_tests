/* ========================================= ",
    * Файл: ...\Tests_Problem3\Program.cs,
    * Автор: Глушков Сергей,
    * Дата: 03.08.2026,
    * Описание: Программа читает лог-файл, приводит записи к единому виду (TSV с табуляцией)
    * и пишет валидные строки в выходной файл, а невалидные — в problems.txt
     * Логика обработки:
     * •	Дата: 
     *       o	Формат 1: DD.MM.YYYY → преобразуем в DD-MM-YYYY.
     *       o	Формат 2: YYYY-MM-DD → преобразуем в DD-MM-YYYY.
     * •	Уровень логирования: маппинг INFORMATION → INFO, WARNING → WARN, остальные (INFO, ERROR, DEBUG) оставляем без изменений. Если уровень не из разрешённого списка — запись считается невалидной.
     * •	ВызвавшийМетод: берём из формата 2; если нет — подставляем DEFAULT.
     * •	Разделитель в выходном файле: табуляция (\t).
     * •	Невалидные строки: пишутся в problems.txt в исходном виде
     * ========================================= */
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic;
class Program
{
    static void Main()
    {
        // программа использует лог-файл "input.txt", пишет валидные и невалидные строки в разные файлы
        // чтобы файл "input.txt" был всегда доступен, необходимо:
        // - поместить его в одну папку с проектом (там где файл "Program.cs"
        // - в VS в обозревателе решений установить для "input.txt" свойство
        //  «Копировать в выходной каталог» (Copy to Output Directory)
        //  в значение «Копировать, более позднюю версию». Тогда файл будет всегда лежать рядом с программой,
        //  а путь к нему сократится до:string inputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");
        //
        // Вычисляем правильные полные пути к файлам
        string inputPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "input.txt");
        string basePath = Path.GetDirectoryName(inputPath);
        string outputPath = Path.Combine(basePath, "output.txt"); // или "output.tsv", если нужен TSV
        string problemsPath = Path.Combine(basePath, "problems.txt");
        // открываем текстовые файлы для чтения и записи, используя улучшенный синтаксис using

        using var reader = new StreamReader(inputPath);
        using var writer = new StreamWriter(outputPath);
        using var problemWriter = new StreamWriter(problemsPath);
        // будем построчно читать текст и записывать результат -  валидные и невалидные строки в разные файлы 
        string? line; // создаем переменную для хранения строки или значения null
        //  считываем в цикле строки из входного файла, пока не дойдем до нулевого значения строки
        while ((line = reader.ReadLine()) != null)
        {
            string? outputLine = ProcessLine(line); // Передаем считанную строку в метод ProcessLine для обработки. Результат сохраняется в outputLine.

            if (outputLine != null)
                writer.WriteLine(outputLine); // если обработка успешно, записываем результат в основной выходной файл
            else
                problemWriter.WriteLine(line); // иначе  запись в файл для проблемных данных
        }

        Console.WriteLine("Готово! Проверь файлы output.txt и problems.txt.");// сообщение о завершении обработки
    }
    
    // напишем метод для анализа и стандартизации строк логов при помощи регулярных выражений regex
    static string? ProcessLine(string line)
        {
        try
        {
            //  регулярное выражение для формата 1, ищет логи вида:
            // Format 1: "10.03.2025 15:14:49.523 INFORMATION Версия программы: '3.4.0.48729'"
            Regex format1 = new Regex(@"^(?<date>\d{2}\.\d{2}\.\d{4}) (?<time>\d{2}:\d{2}:\d{2}\.\d+) (?<level>[A-Z]+) (?<message>.+)$");
            /*
             * Использует именованные группы(?< имя > ...) для захвата конкретных частей:
             * (?< date > ...) — дата в формате ДД.ММ.ГГГГ.
             * (?< time > ...) — время с миллисекундами.
             * (?< level > ...) — уровень лога заглавными буквами(например, INFORMATION).
             * (?< message > ...) — весь оставшийся текст сообщения.
            */
            //  регулярное выражение для формата 2, ищет логи вида:
            // Format 2: "2025-03-10 15:14:51.5882| INFO|11|MobileComputer.GetDeviceId| Код устройства: '@MINDEO-M40-D-410244015546'"
            Regex format2 = new Regex(@"^(?<date>\d{4}-\d{2}-\d{2}) (?<time>\d{2}:\d{2}:\d{2}\.\d+)\| (?<level>[A-Z]+)\|.*?\|(?<method>.*?)\| (?<message>.+)$");
            /*
             * Также разбивает строку на именованные группы, но адаптирован под разделители в виде вертикальной черты |:
             * (?<date>...) — дата в формате ГГГГ-ММ-ДД.
             * (?<level>...) — уровень лога (например, INFO).
             * (?<method>...) — имя метода, который записал лог (например, MobileComputer.GetDeviceId).
             * \|.*?\| — пропускает системные идентификаторы (например, ID потока |11|).
            */
            Match match1 = format1.Match(line); //проверка на соответствие 1-му шаблону
            Match match2 = format2.Match(line); //проверка на соответствие 2-му шаблону

            string date, time, level, method, message;
            
            //  возврат при совпадении извлеченных данных:
            if (match1.Success)
            {
                date = DateTime.ParseExact(match1.Groups["date"].Value, "dd.MM.yyyy", CultureInfo.InvariantCulture)
                              .ToString("yyyy-MM-dd");
                time = match1.Groups["time"].Value;
                level = NormalizeLevel(match1.Groups["level"].Value);
                method = "DEFAULT\t\t\t";
                message = match1.Groups["message"].Value;
            }
            else if (match2.Success)
            {
                date = DateTime.ParseExact(match2.Groups["date"].Value, "yyyy-MM-dd", CultureInfo.InvariantCulture)
                              .ToString("yyyy-MM-dd");
                time = match2.Groups["time"].Value;
                level = NormalizeLevel(match2.Groups["level"].Value);
                method = match2.Groups["method"].Value.Trim();
                message = match2.Groups["message"].Value;
            }
            else
            {
                return null; // невалидная строка
            }

            return $"{date}\t{time}\t{level}\t{method}\t{message}";
        }
        catch
        {
            return null;
        }
    }
    static string NormalizeLevel(string rawLevel)
    {
        return rawLevel switch
        {
            "INFORMATION" => "INFO",
            "INFO" => "INFO",
            "WARNING" => "WARN",
            "WARN" => "WARN",
            "DEBUG" => "DEBUG",
            "ERROR" => "ERROR",
            _ => "INFO" // по умолчанию INFO
        };
    }
}

