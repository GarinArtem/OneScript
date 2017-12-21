﻿/*----------------------------------------------------------
This Source Code Form is subject to the terms of the 
Mozilla Public License, v.2.0. If a copy of the MPL 
was not distributed with this file, You can obtain one 
at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using System;
using System.IO;
using System.Text;

using ScriptEngine.HostedScript.Library;
using ScriptEngine.HostedScript.Library.Binary;
using ScriptEngine.Machine;
using ScriptEngine.Machine.Contexts;



/// <summary>
/// 
/// Объект предназначен для удобной записи различных типов данных в приемник.
/// Необходимо соблюдать следующий порядок работы с данным объектом:
/// 
///  - Создать или получить приемник даных.
///  - Вызвать из приемника экземпляр объекта ЗаписьДанных.
///  - Выполнить требуемые действия с помощью объекта.
///  - Закрыть экземпляр объекта ЗаписьДанных.
/// При необходимости использовать другие методы для работы с данными, требуется сначала закрыть экземпляр объекта ЗаписьДанных с помощью метода Закрыть, выполнить необходимые действия над данными, установить требуемую позицию для чтения из приемника и создать новый экземпляр ЗаписьДанных.
/// </summary>
[ContextClass("ЗаписьДанных", "DataWriter")]
class DataWriter : AutoContext<DataWriter>, IDisposable
{

    private Encoding _workingEncoding;
    private IValue _userVisibleEncoding;
    private BinaryWriter _binaryWriter;
    private readonly bool _writeBOM;
    
    public DataWriter(string fileName, IValue textEncoding, ByteOrderEnum? byteOrder, string lineSplitter, bool append, string convertibleSplitterOfLines, bool writeBOM)
    {
        ByteOrder = byteOrder?? ByteOrderEnum.LittleEndian;
        LineSplitter = lineSplitter?? "\r\n";
        ConvertibleSplitterOfLines = convertibleSplitterOfLines;
        _writeBOM = writeBOM;
        TextEncoding = textEncoding;

        var fileSubsystem = new FileStreamsManager();
        var fileStreamContext = append ? fileSubsystem.OpenForAppend(fileName) : fileSubsystem.OpenForWrite(fileName);

        _binaryWriter = new BinaryWriter(fileStreamContext.GetUnderlyingStream(), _workingEncoding);

        Converter = new EndianBitConverter();
        Converter.IsLittleEndian = byteOrder == ByteOrderEnum.LittleEndian;

    }

    public DataWriter(IStreamWrapper streamObj, IValue textEncoding, ByteOrderEnum? byteOrder, string lineSplitter, string convertibleSplitterOfLines, bool writeBOM)
    {
        ByteOrder = byteOrder?? ByteOrderEnum.LittleEndian;
        LineSplitter = lineSplitter ?? "\r\n";
        ConvertibleSplitterOfLines = convertibleSplitterOfLines;
        _writeBOM = writeBOM;
        TextEncoding = textEncoding;

        _binaryWriter = new BinaryWriter(streamObj.GetUnderlyingStream(), _workingEncoding);

        Converter = new EndianBitConverter();
        Converter.IsLittleEndian = byteOrder == ByteOrderEnum.LittleEndian;
    }

    /// <summary>
    /// 
    /// Создает объект ЗаписьДанных для записи в указанный файл. Если файл с таким именем не существует, он будет создан. Параметр &lt;Дописать&gt; определяет, будут ли данные записаны в начало или в конец файла.
    /// После завершения работы с объектом, до закрытия потока, переданного в конструктор, объект необходимо закрыть с помощью метода Закрыть или НачатьЗакрытие. При этом используемый файл будет закрыт автоматически.
    /// </summary>
    ///
    /// <param name="fileName">
    /// Имя файла, в который будет выполнена запись. </param>
    /// <param name="textEncoding">
    /// Кодировка текста для создаваемого экземпляра ЗаписьДанных. Если не задана, то используется UTF-8.
    /// Значение по умолчанию: UTF8. Типы: КодировкаТекста (TextEncoding), Строка (String) </param>
    /// <param name="byteOrder">
    /// Порядок байтов, используемый по умолчанию для кодирования целых чисел при записи в поток.
    /// Значение по умолчанию: LittleEndian. </param>
    /// <param name="lineSplitter">
    /// Разделитель по умолчанию для строк, записываемых в поток. Если разделитель строк не задан, то используется строка ПС.
    /// Значение по умолчанию: ПС. </param>
    /// <param name="append">
    /// Определяет, будут ли данные записаны в начало или в конец файла:
    /// 
    ///  - Если Истина, то при открытии существующего файла запись будет выполнена в конец файла.
    ///  - Иначе данные будут записываться с начала файла, перезаписывая существующие данные.
    /// Если заданный файл не существует, будет создан новый файл с указанным именем и значение параметра не повлияет на поведение конструктора.
    /// Значение по умолчанию: Ложь. </param>
    /// <param name="convertibleSplitterOfLines">
    /// Определяет разделение строк в файле для конвертации в стандартный перевод строк ПС.
    /// Значение по умолчанию: ВК + ПС. </param>
    /// <param name="writeBOM">
    /// Если в начало файла или потока требуется записать метку порядка байтов (BOM) для используемой кодировки текста, то данный параметр должен иметь значение Истина.
    /// Значение по умолчанию: Ложь. </param>
    ///
    [ScriptConstructor(Name = "На основании имени файла")]
    public static IRuntimeContextInstance Constructor(string fileName, IValue textEncoding = null, ByteOrderEnum? byteOrder = null, string lineSplitter = null, bool append = false, string convertibleSplitterOfLines = null, bool writeBOM = false)
    {
        return new DataWriter(fileName, textEncoding, byteOrder, lineSplitter, append, convertibleSplitterOfLines, writeBOM);
    }

    /// <summary>
    /// 
    /// Объект создается для записи в заданный поток.
    /// После завершения работы с объектом, до закрытия потока, переданного в конструктор, объект необходимо закрыть с помощью метода Закрыть или НачатьЗакрытие.
    /// </summary>
    ///
    /// <param name="stream">
    /// Поток, в который производится запись данных. Типы: Поток (Stream), ПотокВПамяти (MemoryStream), ФайловыйПоток (FileStream) </param>
    /// <param name="textEncoding">
    /// Устанавливает кодировку текста для создаваемого экземпляра ЗаписьДанных. Если не задано, используется кодировка UTF-8.
    /// Значение по умолчанию: UTF8. Типы: КодировкаТекста (TextEncoding), Строка (String) </param>
    /// <param name="byteOrder">
    /// Порядок байтов, используемый по умолчанию для кодирования целых чисел при записи в поток.
    /// Значение по умолчанию: LittleEndian. </param>
    /// <param name="lineSplitter">
    /// Разделитель по умолчанию для строк, записываемых в поток. Если разделитель строк не задан, то используется строка ПС.
    /// Значение по умолчанию: ПС. </param>
    /// <param name="convertibleSplitterOfLines">
    /// Определяет разделение строк в файле для конвертации в стандартный перевод строк ПС.
    /// Значение по умолчанию: ВК + ПС. </param>
    /// <param name="writeBOM">
    /// Если в начало файла или потока требуется записать метку порядка байтов (BOM) для используемой кодировки текста, то данный параметр должен иметь значение Истина.
    /// Значение по умолчанию: Ложь. </param>
    ///
    [ScriptConstructor(Name = "На основании потока")]
    public static IRuntimeContextInstance Constructor1(IValue stream, IValue textEncoding = null, ByteOrderEnum? byteOrder = null, string lineSplitter = null, string convertibleSplitterOfLines = null, bool writeBOM = false)
    {
        var streamObj = stream.AsObject() as IStreamWrapper;
        if (streamObj == null)
        {
            throw RuntimeException.InvalidArgumentType(nameof(stream));
        }

        return new DataWriter(streamObj, textEncoding, byteOrder, lineSplitter, convertibleSplitterOfLines, writeBOM);
    }

    private EndianBitConverter Converter { get; }

    /// <summary>
    /// 
    /// Кодировка текста по-умолчанию для данного экземпляра ЗаписьДанных.
    /// Кодировка может быть задана как в виде значения перечисления КодировкаТекста, так и в виде строки с указанием названия кодировки.
    /// </summary>
    /// <value>КодировкаТекста (TextEncoding), Строка (String)</value>
    [ContextProperty("КодировкаТекста", "TextEncoding")]
    public IValue TextEncoding
    {
        get { return _userVisibleEncoding; }
        set
        {
            _workingEncoding = TextEncodingEnum.GetEncoding(value, _writeBOM);
            _userVisibleEncoding = value;
        }
    }
    
    /// <summary>
    /// 
    /// Конвертируемый разделитель строк. Этот параметр влияет на поведение метода ЗаписатьСимволы.
    /// </summary>
    /// <value>Строка (String)</value>
    [ContextProperty("КонвертируемыйРазделительСтрок", "ConvertibleSplitterOfLines")]
    public string ConvertibleSplitterOfLines { get; set; }

    /// <summary>
    /// 
    /// Порядок байтов по умолчанию.
    /// </summary>
    /// <value>ПорядокБайтов (ByteOrder)</value>
    [ContextProperty("ПорядокБайтов", "ByteOrder")]
    public ByteOrderEnum ByteOrder { get; private set; }


    /// <summary>
    /// 
    /// Разделитель строк по-умолчанию. Это свойство влияет на поведение метода ЗаписатьСтроку.
    /// </summary>
    /// <value>Строка (String)</value>
    [ContextProperty("РазделительСтрок", "LineSplitter")]
    public string LineSplitter { get; set; }
    
    /// <summary>
    /// 
    /// Вызывает метод СброситьБуферы. Если целевой поток был создан при создании объекта ЗаписьДанных, целевой поток также закрывается.
    /// </summary>
    ///
    ///
    [ContextMethod("Закрыть", "Close")]
    public void Close()
    {
        _binaryWriter.Close();
    }


    /// <summary>
    /// 
    /// Записывает данные в целевой поток.
    /// </summary>
    ///
    /// <remarks>
    /// 
    /// Запись двоичных данных
    /// </remarks>
    ///
    /// <param name="binaryDataOrReadResult">
    /// Записать экземпляр объекта ДвоичныеДанные в поток. </param>
    ///
    [ContextMethod("Записать", "Write")]
    public void Write(IValue binaryDataOrReadResult)
    {
        var binData = binaryDataOrReadResult.AsObject() as BinaryDataContext;
        if (binData == null) //TODO: Поддержкать класс РезультатЧтенияДанных
            throw RuntimeException.InvalidArgumentType();

        _binaryWriter.Write(binData.Buffer, 0, binData.Size());
    }
    
    /// <summary>
    /// 
    /// Записывает один байт в целевой поток.
    /// </summary>
    ///
    /// <param name="number">
    /// Целое число, которое будет записано в целевой поток. Значение числа должно находиться в диапазоне от 0 до 255. </param>
    ///
    [ContextMethod("ЗаписатьБайт", "WriteByte")]
    public void WriteByte(byte number)
    {
        _binaryWriter.Write(number);
    }
    
    /// <summary>
    /// 
    /// Записать байты из буфера двоичных данных в целевой поток.
    /// </summary>
    ///
    /// <remarks>
    /// 
    /// Запись части байтов из буфера
    /// </remarks>
    ///
    /// <param name="buffer">
    /// Буфер двоичных данных, который используется в качестве источника данных для записи в целевой поток. </param>
    /// <param name="positionInBuffer">
    /// Позиция в буфере, начиная с которой выполняется чтение байтов для записи в целевой поток. </param>
    /// <param name="number">
    /// Количество байтов, которые требуется записать в целевой поток. </param>
    ///
    [ContextMethod("ЗаписатьБуферДвоичныхДанных", "WriteBinaryDataBuffer")]
    public void WriteBinaryDataBuffer(BinaryDataBuffer buffer, int positionInBuffer = 0, int number = 0)
    {
        if(positionInBuffer == 0 && number == 0)
            _binaryWriter.Write(buffer.Bytes, 0, buffer.Count());
        else
            _binaryWriter.Write(buffer.Bytes, positionInBuffer, number);
    }


    /// <summary>
    /// 
    /// Записывает символы заданной строки в целевой поток.
    /// </summary>
    ///
    /// <param name="line">
    /// Строка, символы которой будут записаны в поток. </param>
    /// <param name="encoding">
    /// Определяет кодировку текста для записи строки. Если не установлена, используется кодировка, заданная для данного объекта ЗаписьДанных.
    /// Кодировка может быть задана как в виде значения перечисления КодировкаТекста, так и в виде строки с указанием названия кодировки.</param>
    ///
    [ContextMethod("ЗаписатьСимволы", "WriteChars")]
    public void WriteChars(string line, IValue encoding = null)
    {
        if(encoding == null)
            _binaryWriter.Write(line);
        else
        {
            var enc = TextEncodingEnum.GetEncoding(encoding, _writeBOM);
            var bytes = enc.GetBytes(line);
            _binaryWriter.Write(bytes,0,bytes.Length);
        }
    }


    /// <summary>
    /// 
    /// Записывает строку в целевой поток.
    /// Сначала записываются все символы строки, затем - разделитель строк.
    /// </summary>
    ///
    /// <param name="line">
    /// Строка, которая будет записана в поток. </param>
    /// <param name="encoding">
    /// Определяет кодировку текста для записи строки. Если не установлена, используется кодировка, заданная для данного объекта ЗаписьДанных.
    /// Кодировка может быть задана как в виде значения перечисления КодировкаТекста, так и в виде строки с указанием названия кодировки.
    /// Значение по умолчанию: Неопределено. Типы: КодировкаТекста (TextEncoding), Строка (String) </param>
    /// <param name="lineSplitter">
    /// Указывает строку, являющуюся разделителем строк в потоке после записи символов строк. Если параметр не указан, используется разделитель строк, указанный для текущего экземпляра объекта ЗаписьДанных.
    /// Значение по умолчанию: Неопределено. </param>
    ///
    [ContextMethod("ЗаписатьСтроку", "WriteLine")]
    public void WriteLine(string line, IValue encoding = null, string lineSplitter = null)
    {
        // TODO: Для экономии времени не поддерживаем пока конвертируемый разделитель строк
        // Кому надо - попросит PR.

        if (encoding == null)
            _binaryWriter.Write(line);
        else
        {
            var enc = TextEncodingEnum.GetEncoding(encoding, _writeBOM);
            var bytes = enc.GetBytes(line);
            _binaryWriter.Write(bytes, 0, bytes.Length);
        }

        if(lineSplitter == null)
            _binaryWriter.Write(LineSplitter);
        else
            _binaryWriter.Write(lineSplitter);
    }

    private byte[] GetBytes<T>(Converter<T, byte[]> converterOverload, T value, ByteOrderEnum byteOrder = ByteOrderEnum.LittleEndian)
    {
        byte[] bytes;
        if (byteOrder == ByteOrder)
            bytes = converterOverload(value);
        else
        {
            var cnv = new EndianBitConverter();
            cnv.IsLittleEndian = byteOrder == ByteOrderEnum.LittleEndian;
            bytes = converterOverload(value);
        }

        return bytes;
    }

    /// <summary>
    /// 
    /// Записывает 16-разрядное число в целевой поток.
    /// </summary>
    ///
    /// <param name="number">
    /// Число, которое будет записано в целевой поток.
    /// Значение числа должно находиться в диапазоне от 0 до 65535. </param>
    /// <param name="byteOrder">
    /// Порядок байтов, который будет использован для кодировки числа при записи. Если не установлен, то будет использован порядок байтов, заданный для текущего экземпляра объекта ЗаписьДанных.
    /// Значение по умолчанию: Неопределено. </param>
    ///
    [ContextMethod("ЗаписатьЦелое16", "WriteInt16")]
    public void WriteInt16(short number, ByteOrderEnum byteOrder = ByteOrderEnum.LittleEndian)
    {
        byte[] buffer;
        if (byteOrder == ByteOrder)
            buffer = Converter.GetBytes(number);
        else
        {
            var cnv = new EndianBitConverter();
            cnv.IsLittleEndian = byteOrder == ByteOrderEnum.LittleEndian;
            buffer = cnv.GetBytes(number);
        }

        _binaryWriter.Write(buffer, 0, buffer.Length);
    }
    
    /// <summary>
    /// 
    /// Записать целое 32-битное число в целевой поток.
    /// </summary>
    ///
    /// <param name="number">
    /// Целое число, которое будет записано в целевой поток. Значение числа должно находиться в диапазоне от 0 до 2^32-1. </param>
    /// <param name="byteOrder">
    /// Порядок байтов, который будет использован для кодировки числа при записи. Если не установлен, то будет использован порядок байтов, заданный для текущего экземпляра объекта ЗаписьДанных.
    /// Значение по умолчанию: Неопределено. </param>
    ///
    [ContextMethod("ЗаписатьЦелое32", "WriteInt32")]
    public void WriteInt32(int number, ByteOrderEnum byteOrder = ByteOrderEnum.LittleEndian)
    {
        byte[] buffer;
        if (byteOrder == ByteOrder)
            buffer = Converter.GetBytes(number);
        else
        {
            var cnv = new EndianBitConverter();
            cnv.IsLittleEndian = byteOrder == ByteOrderEnum.LittleEndian;
            buffer = cnv.GetBytes(number);
        }

        _binaryWriter.Write(buffer, 0, buffer.Length);
    }


    /// <summary>
    /// 
    /// Записывает целое 16-битное число в целевой поток.
    /// </summary>
    ///
    /// <param name="number">
    /// Целое число, которое будет записано в целевой поток. Значение числа должно находиться в диапазоне от 0 до 2^64-1. </param>
    /// <param name="byteOrder">
    /// Порядок байтов, который будет использован для кодировки числа при записи. Если не установлен, то будет использован порядок байтов, заданный для текущего экземпляра объекта ЗаписьДанных.
    /// Значение по умолчанию: Неопределено. </param>
    ///
    [ContextMethod("ЗаписатьЦелое64", "WriteInt64")]
    public void WriteInt64(int number, IValue byteOrder = null)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// Сбрасывает все внутренние буферы в целевой поток, после чего вызывает метод СброситьБуферы целевого потока.
    /// </summary>
    ///
    [ContextMethod("СброситьБуферы", "Flush")]
    public void Flush()
    {
        throw new NotImplementedException();
    }


    /// <summary>
    /// 
    /// Возвращает целевой поток, в который выполняется запись.
    /// </summary>
    ///
    ///
    /// <returns name="Stream"/>
    ///
    [ContextMethod("ЦелевойПоток", "TargetStream")]
    public IValue TargetStream()
    {
        return new GenericStream(_streamObj.BaseStream);
    }

    public void Dispose()
    {
        _streamObj.Dispose();
    }
}
