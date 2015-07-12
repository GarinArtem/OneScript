﻿///////////////////////////////////////////////////////////////////////
//
// Тест проверки работы таблицы значений
// 
//
///////////////////////////////////////////////////////////////////////

Перем юТест;

////////////////////////////////////////////////////////////////////
// Программный интерфейс

Функция Версия() Экспорт
	Возврат "0.1";
КонецФункции

Функция ПолучитьСписокТестов(ЮнитТестирование) Экспорт
	
	юТест = ЮнитТестирование;
	
	ВсеТесты = Новый Массив;
	
	ВсеТесты.Добавить("ТестДолжен_СоздатьДеревоЗначений");
	ВсеТесты.Добавить("ТестДолжен_СоздатьУдалитьКолонки");
	ВсеТесты.Добавить("ТестДолжен_СоздатьУдалитьСтроки");
	ВсеТесты.Добавить("ТестДолжен_ПереместитьСтроки");
	ВсеТесты.Добавить("ТестДолжен_ОтработатьСДанными");
	
	ВсеТесты.Добавить("ТестДолжен_ВыгрузитьКолонкуВМассив");
	ВсеТесты.Добавить("ТестДолжен_НайтиСтрокуВКоллекцииСтрок");
	ВсеТесты.Добавить("ТестДолжен_НайтиНесколькоСтрокВКоллекцииСтрок");
	
	ВсеТесты.Добавить("ТестДолжен_ПроверитьУровниСтрок");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьВстроенныеСвойства");
	
	Возврат ВсеТесты;
	
КонецФункции

Процедура ТестДолжен_СоздатьДеревоЗначений() Экспорт

	Перем Д;
	
	Д = Новый ДеревоЗначений;
	КоличествоДобавляемыхКолонокСтрок = 5;
	
	Для Инд = 1 По КоличествоДобавляемыхКолонокСтрок Цикл
		
		Д.Колонки.Добавить("К" + Инд);
		Д.Строки.Добавить();
		
	КонецЦикла;

	юТест.ПроверитьРавенство(Д.Колонки.Количество(), КоличествоДобавляемыхКолонокСтрок);
	юТест.ПроверитьРавенство(Д.Строки.Количество(), КоличествоДобавляемыхКолонокСтрок);
	
КонецПроцедуры

Процедура ТестДолжен_СоздатьУдалитьКолонки() Экспорт

	Перем Д;
	
	Д = Новый ДеревоЗначений;
	
	К1 = Д.Колонки.Добавить("К1");
	К2 = Д.Колонки.Добавить("К2");
	К3 = Д.Колонки.Добавить("К3");
	К4 = Д.Колонки.Добавить("К4");
	К5 = Д.Колонки.Добавить("К5");
	
	юТест.ПроверитьРавенство(Д.Скопировать().Колонки.Количество(), 5);
	
	// Удаление колонки по ссылке
	Д.Колонки.Удалить(К3);
	
	юТест.ПроверитьРавенство(Д.Колонки.Количество(), 4);
	юТест.ПроверитьРавенство(Д.Колонки.Найти("К3"), Неопределено);
	юТест.ПроверитьРавенство(Д.Колонки.Найти("К2"), К2);
	
	Строка1 = Д.Строки.Добавить();
	Строка2 = Д.Строки.Добавить();
	
	К6 = Д.Колонки.Добавить("К6");
	
	// Доступность К6 после добавления строк
	
	Попытка
		
		Строка1["К6"] = 123;
		юТест.ПроверитьРавенство(Строка1["К6"], 123);
		
	Исключение
	
		юТест.ПроверитьИстину(Ложь, "Колонка К6 не доступна!");
		
	КонецПопытки;
	
	// Удаление колонки по индексу
	Д.Колонки.Удалить(0);
	
	юТест.ПроверитьРавенство(Д.Колонки.Количество(), 4);
	юТест.ПроверитьРавенство(Д.Колонки.Найти("К1"), Неопределено);
	юТест.ПроверитьРавенство(Д.Колонки.Найти("К2"), К2);

	// Удаление колонки по имени
	Д.Колонки.Удалить("К4");
	
	юТест.ПроверитьРавенство(Д.Колонки.Количество(), 3);
	юТест.ПроверитьРавенство(Д.Колонки.Найти("К4"), Неопределено);
	юТест.ПроверитьРавенство(Д.Колонки.Найти("К2"), К2);
	
	// Недоступность удалённой колонки
	
	Попытка
	
		Значение = Строка1["К1"];
		юТест.ПроверитьИстину(Ложь, "Доступна удалённая колонка!");
		
	Исключение
	
	КонецПопытки;

КонецПроцедуры

Процедура ТестДолжен_СоздатьУдалитьСтроки() Экспорт

	Перем Д;
	
	Д = Новый ДеревоЗначений;
	
	К1 = Д.Колонки.Добавить("К1");
	К2 = Д.Колонки.Добавить("К2");
	К3 = Д.Колонки.Добавить("К3");
	К4 = Д.Колонки.Добавить("К4");
	К5 = Д.Колонки.Добавить("К5");
	
	С1 = Д.Строки.Добавить();
	С2 = Д.Строки.Добавить();
	С2_5 = Д.Строки.Добавить();
	С3 = Д.Строки.Добавить();
	С4 = Д.Строки.Добавить();
	
	Д.Строки.Удалить(С2_5);
	
	юТест.ПроверитьРавенство(Д.Скопировать().Строки.Количество(), 4);
	
	юТест.ПроверитьРавенство(Д.Строки.Получить(0), С1);
	юТест.ПроверитьРавенство(Д.Строки.Получить(1), С2);
	юТест.ПроверитьРавенство(Д.Строки.Получить(2), С3);
	юТест.ПроверитьРавенство(Д.Строки.Получить(3), С4);
	
	юТест.ПроверитьРавенство(Д.Строки[0], С1);
	юТест.ПроверитьРавенство(Д.Строки[1], С2);
	юТест.ПроверитьРавенство(Д.Строки[2], С3);
	юТест.ПроверитьРавенство(Д.Строки[3], С4);
	
	юТест.ПроверитьРавенство(Д.Строки.Индекс(С1), 0);
	юТест.ПроверитьРавенство(Д.Строки.Индекс(С2), 1);
	юТест.ПроверитьРавенство(Д.Строки.Индекс(С3), 2);
	юТест.ПроверитьРавенство(Д.Строки.Индекс(С4), 3);
	
	Обошли = Новый Соответствие;
	Для Каждого мСтрокаТаблицы Из Д.Строки Цикл
	
		Обошли.Вставить(мСтрокаТаблицы, Истина);
	
	КонецЦикла;
	
	юТест.ПроверитьИстину(Обошли.Получить(С1), "Обход бегунком");
	юТест.ПроверитьИстину(Обошли.Получить(С2), "Обход бегунком");
	юТест.ПроверитьИстину(Обошли.Получить(С3), "Обход бегунком");
	юТест.ПроверитьИстину(Обошли.Получить(С4), "Обход бегунком");
	
КонецПроцедуры

Функция ПроверитьПорядок(Д, П1, П2, П3, П4, П5)

	Массив = Новый Массив;
	Массив.Добавить(П1);
	Массив.Добавить(П2);
	Массив.Добавить(П3);
	Массив.Добавить(П4);
	Массив.Добавить(П5);

	Для Инд = 0 По 4 Цикл
	
		Если Д.Строки[Инд].Индекс <> Массив[Инд] Тогда
			Возврат Ложь;
		КонецЕсли;
	
	КонецЦикла;
	
	Возврат Истина;

КонецФункции

Процедура ТестДолжен_ПереместитьСтроки() Экспорт
	
	Перем Д;
	
	Д = Новый ДеревоЗначений;
	
	Д.Колонки.Добавить("Индекс");
	
	Для Инд = 1 По 5 Цикл
		Д.Строки.Добавить().Индекс = Инд;
	КонецЦикла;
	
	Д = Д.Скопировать();
	
	С1 = Д.Строки[0];
	С2 = Д.Строки[1];
	С3 = Д.Строки[2];
	С4 = Д.Строки[3];
	С5 = Д.Строки[4];
	
	// Проверим крайние случаи
	Д.Строки.Сдвинуть(С1, -1);
	юТест.ПроверитьРавенство(Д.Строки.Индекс(С1), 4);
	
	Д.Строки.Сдвинуть(С1, 1);
	юТест.ПроверитьРавенство(Д.Строки.Индекс(С1), 0);
	
	// Проверим цикличность
	Д.Строки.Сдвинуть(С1, 5);
	юТест.ПроверитьРавенство(Д.Строки.Индекс(С1), 0);
	
	Д.Строки.Сдвинуть(С1, 10);
	юТест.ПроверитьРавенство(Д.Строки.Индекс(С1), 0);
	
	Д.Строки.Сдвинуть(С1, 15);
	юТест.ПроверитьРавенство(Д.Строки.Индекс(С1), 0);
	
	Д.Строки.Сдвинуть(С1, -5);
	юТест.ПроверитьРавенство(Д.Строки.Индекс(С1), 0);
	
	Д.Строки.Сдвинуть(С1, -10);
	юТест.ПроверитьРавенство(Д.Строки.Индекс(С1), 0);
	
	Д.Строки.Сдвинуть(С1, -15);
	юТест.ПроверитьРавенство(Д.Строки.Индекс(С1), 0);
	
	// Проверим обычное перемещение
	
	Д.Строки.Сдвинуть(С2, -1);
	ПроверитьПорядок(Д.Скопировать(), 2, 1, 3, 4, 5);
	
	Д.Строки.Сдвинуть(С1, 2);
	ПроверитьПорядок(Д.Скопировать(), 2, 3, 4, 1, 5);
	
КонецПроцедуры

Процедура ТестДолжен_ОтработатьСДанными() Экспорт

	Перем Д;
	
	Д = Новый ДеревоЗначений;
	
	Д.Колонки.Добавить("Количество");
	Д.Колонки.Добавить("Цена");
	Д.Колонки.Добавить("Сумма");
	
	Для Инд = 1 По 5 Цикл
	
		НоваяСтрока = Д.Строки.Добавить();
		НоваяСтрока.Количество = Инд;
	
	КонецЦикла;
	
	Цены = Новый Массив;
	Цены.Добавить(100);
	Цены.Добавить(50);
	Цены.Добавить(30);
	Цены.Добавить(32.3);
	Цены.Добавить(16);
	
	Д.Строки.ЗагрузитьКолонку(Цены, "Цена");
	
	мСумма = 0;
	Для Каждого мСтрока Из Д.Строки Цикл
	
		мСтрока.Сумма = мСтрока.Количество * мСтрока.Цена;
		мСумма = мСумма + мСтрока.Сумма;
	
	КонецЦикла;
	
	юТест.ПроверитьРавенство(Д.Строки.Итог("Сумма"), мСумма);
	
КонецПроцедуры

Процедура ТестДолжен_ВыгрузитьКолонкуВМассив() Экспорт
	
	Д = Новый ДеревоЗначений;
	
	Д.Колонки.Добавить("Ключ");
	Д.Колонки.Добавить("Значение");
	
	ЭталонКлючей = Новый Массив;
	ЭталонЗначений = Новый Массив;
	
	Для Сч = 1 По 5 Цикл
		С = Д.Строки.Добавить();
		С.Ключ = "Ключ" + Строка(Сч);
		С.Значение = Сч;
		
		ЭталонКлючей.Добавить(С.Ключ);
		ЭталонЗначений.Добавить(С.Значение);
		
	КонецЦикла;
	
	юТест.ПроверитьИстину(МассивыИдентичны(Д.Строки.ВыгрузитьКолонку("Ключ"), ЭталонКлючей), "Массивы ключей должны совпадать");
	юТест.ПроверитьИстину(МассивыИдентичны(Д.Строки.ВыгрузитьКолонку("Значение"), ЭталонЗначений), "Массивы значений должны совпадать");
	
КонецПроцедуры

Функция МассивыИдентичны(Знач Проверяемый, Знач Эталон)
	Если Проверяемый.Количество() <> Эталон.Количество() Тогда
		Возврат Ложь;
	КонецЕсли;
	
	Для Сч = 0 По Проверяемый.Количество()-1 Цикл
		Если Проверяемый[Сч] <> Эталон[Сч] Тогда
			Возврат Ложь;
		КонецЕсли;
	КонецЦикла;
	
	Возврат Истина;
	
КонецФункции

Процедура ТестДолжен_НайтиСтрокуВКоллекцииСтрок() Экспорт

	Д = Новый ДеревоЗначений;
	
	Д.Колонки.Добавить("Ключ");
	Д.Колонки.Добавить("Значение");
	
	Для Сч = 1 По 5 Цикл
		С = Д.Строки.Добавить();
		С.Ключ = "Ключ" + Строка(Сч);
		С.Значение = Сч;
	КонецЦикла;
	
	ИскомаяСтрока = Д.Строки.Найти("Ключ2", "Ключ");
	юТест.ПроверитьЛожь(ИскомаяСтрока = Неопределено, "Строка должна быть найдена");
	юТест.ПроверитьРавенство(ИскомаяСтрока.Значение, 2);
	
КонецПроцедуры

Процедура ТестДолжен_НайтиНесколькоСтрокВКоллекцииСтрок() Экспорт
	
	Д = Новый ДеревоЗначений;
	
	Д.Колонки.Добавить("Ключ");
	Д.Колонки.Добавить("Значение");
	
	Для Сч = 1 По 8 Цикл
		С = Д.Строки.Добавить();
		
		Если Сч % 2 Тогда
			С.Ключ = "Истина";
			С.Значение = Истина;
		Иначе
			С.Ключ = "Ложь";
			С.Значение = Ложь;
		КонецЕсли;
		
	КонецЦикла;
	
	КлючиПоиска = Новый Структура("Ключ,Значение", "Истина", Истина);
	НайденныеСтроки = Д.Строки.НайтиСтроки(КлючиПоиска);
	
	юТест.ПроверитьРавенство(НайденныеСтроки.Количество(), 4, "Количество строк должно совпадать с эталоном");
	
	Для Каждого Стр Из НайденныеСтроки Цикл
		юТест.ПроверитьРавенство(Стр.Ключ, "Истина");
		юТест.ПроверитьРавенство(Стр.Значение, Истина);
	КонецЦикла;
	
КонецПроцедуры

Процедура ПроверитьУровеньДерева(Знач Дерево, Знач Уровень, Знач Счетчик)

	Перем С;
	С = Дерево.Строки.Добавить();
	
	юТест.ПроверитьРавенство(С.Уровень(), Уровень, "Ошибка в вычислении уровня!");
	
	Если Счетчик > 0 Тогда
	
		ПроверитьУровеньДерева(С, Уровень + 1, Счетчик - 1);
		
	КонецЕсли;

КонецПроцедуры

Процедура ТестДолжен_ПроверитьУровниСтрок() Экспорт

	Перем Д;
	
	Д = Новый ДеревоЗначений;
	КоличествоУровнейДляПроверки = 5;
	
	ПроверитьУровеньДерева(Д, 0, КоличествоУровнейДляПроверки);

КонецПроцедуры

Процедура ТестДолжен_ПроверитьВстроенныеСвойства() Экспорт

	Перем Д;
	
	Д = Новый ДеревоЗначений;
	
	СтрокаРодитель = Д.Строки.Добавить();
	СтрокаПотомок = СтрокаРодитель.Строки.Добавить();
	
	юТест.ПроверитьРавенство(СтрокаРодитель.Родитель, Неопределено, "Родитель корневой строки");
	юТест.ПроверитьРавенство(СтрокаРодитель, СтрокаПотомок.Родитель, "Правильная работа Родителя");

КонецПроцедуры
