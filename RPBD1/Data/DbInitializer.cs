using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPBD1.models;

namespace RPBD1.Data
{
    public static class DbInitializer
    {
        public static void Initialize(MonitoringContext db) // Изменил контекст на RPBDContext для вашего приложения
        {
            db.Database.EnsureCreated();

            // Проверка, занесены ли данные об оборудовании
            if (db.Equipments.Any())
            {
                Console.WriteLine("====== База данных уже инициализирована ========");
                return;
            }

            // Инициализация данных
            int equipment_number = 20;
            int employee_number = 10;
            int maintenance_type_number = 5;
            int schedule_number = 50;
            int completed_work_number = 100;

            Random randObj = new(1);

            // Заполнение таблицы сотрудников
            string[] employee_names = { "Иванов", "Петров", "Сидоров", "Кузнецов", "Смирнов", "Орлов", "Тихонов", "Григорьев", "Федоров", "Морозов" };
            string[] employee_positions = { "Инженер", "Механик", "Техник", "Мастер", "Оператор" };
            for (int i = 1; i <= employee_number; i++)
            {
                string fullName = employee_names[randObj.Next(employee_names.Length)] + " " + i;
                string position = employee_positions[randObj.Next(employee_positions.Length)];
                db.Employees.Add(new Employee { FullName = fullName, Position = position });
            }

            db.SaveChanges(); // Сохранение сотрудников

            // Заполнение таблицы оборудования
            string[] equipment_names = { "Конвейер", "Робот", "Станок", "Компрессор", "Генератор" };
            string[] locations = { "Цех 1", "Цех 2", "Цех 3", "Склад", "Отдел логистики" };
            for (int i = 1; i <= equipment_number; i++)
            {
                string name = equipment_names[randObj.Next(equipment_names.Length)] + " " + i;
                string location = locations[randObj.Next(locations.Length)];
                DateOnly startDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-randObj.Next(1000)));
                string inventoryNumber = "INV" + i.ToString("D4");

                db.Equipments.Add(new Equipment
                {
                    Name = name,
                    Location = location,
                    StartDate = startDate,
                    InventoryNumber = inventoryNumber
                });
            }

            db.SaveChanges(); // Сохранение оборудования

            // Заполнение таблицы типов обслуживания
            string[] maintenance_types = { "Ежегодное", "Капитальный ремонт", "Экстренное обслуживание", "Профилактическое обслуживание", "Замена деталей" };
            for (int i = 1; i <= maintenance_type_number; i++)
            {
                string description = maintenance_types[randObj.Next(maintenance_types.Length)] + " " + i;
                db.MaintenanceTypes.Add(new MaintenanceType { Description = description });
            }

            db.SaveChanges(); // Сохранение типов обслуживания

            // Заполнение таблицы графиков обслуживания
            int minEquipmentId = db.Equipments.FirstOrDefault().EquipmentId;
            int minEmployeeId = db.Employees.FirstOrDefault().EmployeeId;
            int minMaintenanceTypeId = db.MaintenanceTypes.FirstOrDefault().MaintenanceTypeId;

            for (int i = 1; i <= schedule_number; i++)
            {
                int equipmentId = randObj.Next(minEquipmentId, minEquipmentId + equipment_number - 1);
                int employeeId = randObj.Next(minEmployeeId, minEmployeeId + employee_number - 1);
                int maintenanceTypeId = randObj.Next(minMaintenanceTypeId, minMaintenanceTypeId + maintenance_type_number - 1);
                DateOnly scheduledDate = DateOnly.FromDateTime(DateTime.Now.AddDays(randObj.Next(1, 365)));
                decimal estimatedCost = (decimal)(randObj.NextDouble() * 1000);

                db.MaintenanceSchedules.Add(new MaintenanceSchedule
                {
                    EquipmentId = equipmentId,
                    ResponsibleEmployeeId = employeeId,
                    MaintenanceTypeId = maintenanceTypeId,
                    ScheduledDate = scheduledDate,
                    EstimatedCost = estimatedCost
                });
            }

            db.SaveChanges(); // Сохранение графиков обслуживания

            // Заполнение таблицы выполненных работ
            for (int i = 1; i <= completed_work_number; i++)
            {
                int equipmentId = randObj.Next(minEquipmentId, minEquipmentId + equipment_number - 1);
                int employeeId = randObj.Next(minEmployeeId, minEmployeeId + employee_number - 1);
                int maintenanceTypeId = randObj.Next(minMaintenanceTypeId, minMaintenanceTypeId + maintenance_type_number - 1);
                DateOnly completionDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-randObj.Next(100)));
                decimal actualCost = (decimal)(randObj.NextDouble() * 500);

                db.CompletedWorks.Add(new CompletedWork
                {
                    EquipmentId = equipmentId,
                    ResponsibleEmployeeId = employeeId,
                    MaintenanceTypeId = maintenanceTypeId,
                    CompletionDate = completionDate,
                    ActualCost = actualCost
                });
            }

            db.SaveChanges(); // Сохранение выполненных работ

            Console.WriteLine("====== База данных инициализирована ========");
        }
    }

}
