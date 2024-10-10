using System;
using System.Collections.Generic;
using System.Linq;
using RPBD1.Data;
using RPBD1.models;
using RPBD1;

public class Program
{
    public static void Main()
    {
        using MonitoringContext db = new();
        DbInitializer.Initialize(db); // Инициализация базы данных

        bool continueProgram = true;
        while (continueProgram)
        {
            Console.Clear();
            Console.WriteLine("====== Меню ======");
            Console.WriteLine("1. Выборка всех данных из таблицы Equipment");
            Console.WriteLine("2. Выборка отфильтрованных данных из таблицы Equipment");
            Console.WriteLine("3. Группировка данных из таблицы CompletedWork");
            Console.WriteLine("4. Выборка данных из таблиц CompletedWork и Equipment");
            Console.WriteLine("5. Выборка данных из таблиц CompletedWork и Equipment с фильтром");
            Console.WriteLine("6. Вставка данных в таблицы (сторона Один)");
            Console.WriteLine("7. Вставка данных в таблицы (сторона Многие)");
            Console.WriteLine("8. Обновление данных");
            Console.WriteLine("9. Удаление данных из Equipment");
            Console.WriteLine("10. Удаление данных из CompletedWork");
            Console.WriteLine("11. Завершить работу программы");
            Console.WriteLine("Выберите действие (1-11):");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Console.WriteLine("====== Выборка всех данных из таблицы Equipment ======");
                    SelectAllEquipment(db);
                    Console.ReadKey();
                    Console.Clear();
                    break;
                case "2":
                    Console.WriteLine("====== Выборка отфильтрованных данных из таблицы Equipment ======");
                    SelectFilteredEquipment(db);
                    Console.ReadKey();
                    Console.Clear();
                    break;
                case "3":
                    Console.WriteLine("====== Группировка данных по стоимости из таблицы CompletedWork ======");
                    GroupCompletedWorks(db);
                    Console.ReadKey();
                    Console.Clear();
                    break;
                case "4":
                    Console.WriteLine("====== Выборка данных из таблиц CompletedWork и Equipment ======");
                    SelectJoin(db, 5);
                    Console.ReadKey();
                    Console.Clear();
                    break;
                case "5":
                    Console.WriteLine("====== Выборка данных из таблиц CompletedWork и Equipment с фильтром ======");
                    SelectJoinWithFilter(db);
                    Console.ReadKey();
                    Console.Clear();
                    break;
                case "6":
                    Console.WriteLine("====== Вставка данных в таблицы (сторона Один) ======");
                    InsertEquipmentAndMaintenanceType(db);
                    Console.ReadKey();
                    Console.Clear();
                    break;
                case "7":
                    Console.WriteLine("====== Вставка данных в таблицу CompletedWork (сторона Многие) ======");
                    InsertCompletedWork(db);
                    Console.ReadKey();
                    Console.Clear();
                    break;
                case "8":
                    Console.WriteLine("====== Обновление данных ======");
                    Update(db);
                    Console.ReadKey();
                    Console.Clear();
                    break;
                case "9":
                    Console.WriteLine("====== Удаление данных из таблицы Equipment ======");
                    DeleteEquipment(db);
                    Console.ReadKey();
                    Console.Clear();
                    break;
                case "10":
                    Console.WriteLine("====== Удаление данных из таблицы CompletedWork ======");
                    DeleteCompletedWork(db);
                    Console.ReadKey();
                    Console.Clear();
                    break;
                case "11":
                    continueProgram = false;
                    Console.WriteLine("====== Завершение работы программы ======");
                    break;
                default:
                    Console.WriteLine("Неверный выбор. Попробуйте снова.");
                    Console.ReadKey();
                    Console.Clear();
                    break;
            }

            if (continueProgram)
            {
                Console.WriteLine("Нажмите любую клавишу, чтобы вернуться в меню...");
                Console.ReadKey();
            }
        }
    }

    static void Print(string sqltext, IEnumerable<object> items)
    {
        Console.WriteLine(sqltext);
        Console.WriteLine("Записи: ");
        foreach (var item in items)
        {
            Console.WriteLine(item.ToString());
        }
        Console.WriteLine();
    }

    // 1. Выборка всех данных из таблицы Equipment
    static void SelectAllEquipment(MonitoringContext db)
    {
        var equipments = db.Equipments.ToList();

        Console.WriteLine("Все записи из таблицы Equipment:");
        foreach (var equipment in equipments)
        {
            Console.WriteLine($"ID: {equipment.EquipmentId}, Название: {equipment.Name}, Инв. номер: {equipment.InventoryNumber}, Локация: {equipment.Location}, Дата начала: {equipment.StartDate}");
        }
    }
    
    // 2. Выборка данных из таблицы Equipment с фильтром
    static void SelectFilteredEquipment(MonitoringContext db)
    {
        Console.WriteLine("Введите локацию для фильтрации:");
        string location = Console.ReadLine();

        var filteredEquipments = db.Equipments.Where(e => e.Location == location).ToList();

        Console.WriteLine($"Оборудование, отфильтрованное по местоположению ({location}):");
        foreach (var equipment in filteredEquipments)
        {
            Console.WriteLine($"ID: {equipment.EquipmentId}, Название: {equipment.Name}, Инв. номер: {equipment.InventoryNumber}, Локация: {equipment.Location}, Дата начала: {equipment.StartDate}");
        }
    }

    // 3. Группировка данных по стоимости обслуживания из таблицы CompletedWork
    static void GroupCompletedWorks(MonitoringContext db)
    {
        var groupedCompletedWorks = db.CompletedWorks
            .GroupBy(cw => cw.EquipmentId)
            .Select(g => new
            {
                EquipmentId = g.Key,
                TotalCost = g.Sum(cw => cw.ActualCost)
            })
            .ToList();

        Console.WriteLine("Группировка по оборудованию с итоговой стоимостью обслуживания:");
        foreach (var group in groupedCompletedWorks)
        {
            Console.WriteLine($"Оборудование ID: {group.EquipmentId}, Общая стоимость: {group.TotalCost}");
        }
    }

    // 4. Выборка данных из таблиц CompletedWork и Equipment (без фильтрации)
    static void SelectJoin(MonitoringContext db, int recordsNumber)
    {
        var queryLINQ = from cw in db.CompletedWorks
                        join eq in db.Equipments on cw.EquipmentId equals eq.EquipmentId
                        join mt in db.MaintenanceTypes on cw.MaintenanceTypeId equals mt.MaintenanceTypeId
                        select new
                        {
                            Оборудование = eq.Name,
                            ТипОбслуживания = mt.Description,
                            ДатаЗавершения = cw.CompletionDate,
                            Стоимость = cw.ActualCost
                        };

        Print("Результат выборки записей о завершённых работах:", queryLINQ.Take(recordsNumber).ToList());
    }

    // 5. Выборка данных из таблиц CompletedWork и Equipment с фильтром
    static void SelectJoinWithFilter(MonitoringContext db)
    {
        Console.WriteLine("Введите минимальную стоимость для фильтрации:");
        decimal minCost = Convert.ToDecimal(Console.ReadLine());

        var filteredQueryLINQ = from cw in db.CompletedWorks
                                join eq in db.Equipments on cw.EquipmentId equals eq.EquipmentId
                                where cw.ActualCost > minCost
                                select new
                                {
                                    Оборудование = eq.Name,
                                    Стоимость = cw.ActualCost
                                };

        Print($"Фильтрация завершённых работ с стоимостью более {minCost}:", filteredQueryLINQ.ToList());
    }

    // 6. Вставка данных в таблицы на стороне "Один" (Equipment и MaintenanceType)
    static void InsertEquipmentAndMaintenanceType(MonitoringContext db)
    {
        Console.WriteLine("Введите название оборудования:");
        string name = Console.ReadLine();

        Console.WriteLine("Введите инвентарный номер:");
        string inventoryNumber = Console.ReadLine();

        Console.WriteLine("Введите дату начала эксплуатации (формат ГГГГ-ММ-ДД):");
        DateOnly startDate = DateOnly.Parse(Console.ReadLine());

        Console.WriteLine("Введите локацию:");
        string location = Console.ReadLine();

        Equipment equipment = new()
        {
            Name = name,
            InventoryNumber = inventoryNumber,
            StartDate = startDate,
            Location = location
        };

        db.Equipments.Add(equipment);
        db.SaveChanges();

        Console.WriteLine("Оборудование успешно добавлено.");
    }

    // 7. Вставка данных в таблицу CompletedWork (сторона "Многие")
    static void InsertCompletedWork(MonitoringContext db)
    {
        Console.WriteLine("Введите ID оборудования:");
        int equipmentId = Convert.ToInt32(Console.ReadLine());

        Console.WriteLine("Введите ID типа обслуживания:");
        int maintenanceTypeId = Convert.ToInt32(Console.ReadLine());

        Console.WriteLine("Введите ID ответственного сотрудника:");
        int employeeId = Convert.ToInt32(Console.ReadLine());

        Console.WriteLine("Введите фактическую стоимость:");
        decimal actualCost = Convert.ToDecimal(Console.ReadLine());

        CompletedWork completedWork = new()
        {
            EquipmentId = equipmentId,
            MaintenanceTypeId = maintenanceTypeId,
            CompletionDate = DateOnly.FromDateTime(DateTime.Now),
            ResponsibleEmployeeId = employeeId,
            ActualCost = actualCost
        };

        db.CompletedWorks.Add(completedWork);
        db.SaveChanges();

        Console.WriteLine("Запись о завершённой работе успешно добавлена.");
    }

    // 8. Обновление данных
    static void Update(MonitoringContext db)
    {
        Console.WriteLine("Введите ID завершённой работы для обновления:");
        int completedWorkId = Convert.ToInt32(Console.ReadLine());

        var completedWork = db.CompletedWorks.FirstOrDefault(cw => cw.CompletedMaintenanceId == completedWorkId);
        if (completedWork != null)
        {
            Console.WriteLine("Введите новую фактическую стоимость:");
            completedWork.ActualCost = Convert.ToDecimal(Console.ReadLine());

            db.SaveChanges();
            Console.WriteLine("Данные успешно обновлены.");
        }
        else
        {
            Console.WriteLine("Запись для обновления не найдена.");
        }
    }

    // 9. Удаление данных из таблицы Equipment (сторона "Один")
    static void DeleteEquipment(MonitoringContext db)
    {
        Console.WriteLine("Введите ID оборудования для удаления:");
        int equipmentId = Convert.ToInt32(Console.ReadLine());

        var equipmentToDelete = db.Equipments.FirstOrDefault(e => e.EquipmentId == equipmentId);
        if (equipmentToDelete != null)
        {
            db.Equipments.Remove(equipmentToDelete);
            db.SaveChanges();
            Console.WriteLine("Оборудование успешно удалено.");
        }
        else
        {
            Console.WriteLine("Запись для удаления не найдена.");
        }
    }

    // 10. Удаление данных из таблицы CompletedWork (сторона "Многие")
    static void DeleteCompletedWork(MonitoringContext db)
    {
        Console.WriteLine("Введите ID завершённой работы для удаления:");
        int completedWorkId = Convert.ToInt32(Console.ReadLine());

        var completedWork = db.CompletedWorks.FirstOrDefault(cw => cw.CompletedMaintenanceId == completedWorkId);
        if (completedWork != null)
        {
            db.CompletedWorks.Remove(completedWork);
            db.SaveChanges();
            Console.WriteLine("Запись о завершённой работе успешно удалена.");
        }
        else
        {
            Console.WriteLine("Запись для удаления не найдена.");
        }
    }
}