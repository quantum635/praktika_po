DROP DATABASE IF EXISTS administration;
CREATE DATABASE administration;
USE administration;

CREATE TABLE Roles
(
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE Users
(
    id INT AUTO_INCREMENT PRIMARY KEY,
    login VARCHAR(50) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    salt VARCHAR(255) NOT NULL,
    surname VARCHAR(100) NOT NULL,
    first_name VARCHAR(100) NOT NULL,
    patronymic VARCHAR(100) NOT NULL,
    role_id INT NOT NULL,
    FOREIGN KEY(role_id) REFERENCES Roles(id)
);

CREATE TABLE Departments
(
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(150) NOT NULL UNIQUE
);

CREATE TABLE Executors
(
    id INT AUTO_INCREMENT PRIMARY KEY,
    surname VARCHAR(100) NOT NULL,
    first_name VARCHAR(100) NOT NULL,
    patronymic VARCHAR(100) NOT NULL,
    department_id INT NOT NULL,
    FOREIGN KEY(department_id) REFERENCES Departments(id)
);

CREATE TABLE Sections
(
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE Topics
(
    id INT AUTO_INCREMENT PRIMARY KEY,
    section_id INT NOT NULL,
    topic_name VARCHAR(200) NOT NULL,
    department_id INT NOT NULL,
    executor_id INT NOT NULL,
    deadline_days INT NOT NULL DEFAULT 30,
    image_path VARCHAR(255),
    FOREIGN KEY(section_id) REFERENCES Sections(id),
    FOREIGN KEY(department_id) REFERENCES Departments(id),
    FOREIGN KEY(executor_id) REFERENCES Executors(id)
);

CREATE TABLE Statuses
(
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE Applicants
(
    id INT AUTO_INCREMENT PRIMARY KEY,
    surname VARCHAR(100) NOT NULL,
    first_name VARCHAR(100) NOT NULL,
    patronymic VARCHAR(100) NOT NULL,
    address VARCHAR(255)
);

CREATE TABLE Appeals
(
    id INT AUTO_INCREMENT PRIMARY KEY,
    register_date DATE NOT NULL,
    applicant_id INT NOT NULL,
    topic_id INT NOT NULL,
    description TEXT,
    status_id INT NOT NULL,
    deadline DATE NOT NULL,
    response_date DATE NULL,
    FOREIGN KEY(applicant_id) REFERENCES Applicants(id),
    FOREIGN KEY(topic_id) REFERENCES Topics(id),
    FOREIGN KEY(status_id) REFERENCES Statuses(id)
);

INSERT INTO Roles (id, name) VALUES
(1, 'Пользователь'),
(2, 'Менеджер'),
(3, 'Администратор');

INSERT INTO Users 
(id, login, password_hash, salt, surname, first_name, patronymic, role_id)
VALUES
(1, 'user1', 'd72f938ecd77aabc49389141b710b60d352102b0ad39cb96130f012fb9232dbe', '2cfc50764222b5dad85ee89e26965eb4', 'Орлов', 'Алексей', 'Сергеевич', 1),
(2, 'manager1', '2f65b86627daf3c4448379c150441ee0aefbc9c1193cdfe844f02e4e145a654e', '62d2bf9834670e481ed51adc315673ab', 'Белова', 'Марина', 'Викторовна', 2),
(3, 'admin1', '0b974d4c92898db0378f9b7b5246ef58c34889468506cf1cd8968d7813c3d621', 'eeaf9bdf33144468eeb8719654177d8f', 'Соколов', 'Дмитрий', 'Андреевич', 3),
(4, 'manager2', '8af547401784297f2b42dcea1dc49494f5dfedb0e154f7e5bdb4c13524f42fb8', 'd3474dab1851f64f1bda0a15a53d9426', 'Кузнецова', 'Елена', 'Павловна', 2),
(5, 'admin2', '4251ec60fb2d25a8fb6caa28f423bd0b959ad91749d2d7302ab387655ea1c3d9', 'b1f3b7f1d783cb2dd823f60844adddd3', 'Васильев', 'Игорь', 'Николаевич', 3);

INSERT INTO Departments (id, name) VALUES
(1, 'Отдел жилищно-коммунального хозяйства'),
(2, 'Отдел благоустройства'),
(3, 'Отдел земельных отношений'),
(4, 'Социальный отдел'),
(5, 'Отдел дорожного хозяйства');

INSERT INTO Executors
(id, surname, first_name, patronymic, department_id)
VALUES
(1, 'Иванов', 'Андрей', 'Викторович', 5),
(2, 'Морозов', 'Сергей', 'Павлович', 1),
(3, 'Смирнова', 'Наталья', 'Олеговна', 2),
(4, 'Фёдорова', 'Ольга', 'Александровна', 4),
(5, 'Иванов', 'Максим', 'Евгеньевич', 3);

INSERT INTO Sections (id, name) VALUES
(1, 'Дороги'),
(2, 'ЖКХ'),
(3, 'Благоустройство'),
(4, 'Социальные вопросы'),
(5, 'Земля');

INSERT INTO Topics
(id, section_id, topic_name, department_id, executor_id, image_path)
VALUES
(1,1,'Ремонт дорожного покрытия',5,1,'images/road.png'),
(2,1,'Установка дорожных знаков',5,1,'images/sign.png'),
(3,1,'Очистка дорог в зимний период',5,1,'images/snow.png'),
(4,2,'Отопление многоквартирных домов',1,2,'images/heating.png'),
(5,2,'Вывоз бытовых отходов',1,2,'images/trash.png'),
(6,2,'Аварийные ситуации водоснабжения',1,2,'images/water.png'),
(7,3,'Освещение улиц',2,3,'images/light.png'),
(8,3,'Озеленение территории',2,3,'images/tree.png'),
(9,3,'Установка детских площадок',2,3,'images/playground.png'),
(10,4,'Помощь малоимущим гражданам',4,4,'images/help.png'),
(11,4,'Предоставление льгот',4,4,'images/benefit.png'),
(12,4,'Консультация граждан',4,4,'images/info.png'),
(13,5,'Оформление земельных участков',3,5,'images/land.png'),
(14,5,'Изменение границ участка',3,5,'images/border.png'),
(15,5,'Разрешение на использование земли',3,5,'images/permit.png');

INSERT INTO Statuses (id,name) VALUES
(1,'Новое'),
(2,'В работе'),
(3,'Выполнено'),
(4,'Просрочено');

INSERT INTO Applicants
(id, surname, first_name, patronymic, address)
VALUES
(1,'Иванов','Алексей','Петрович','г. Нижний Ломов'),
(2,'Петрова','Мария','Сергеевна','г. Нижний Ломов'),
(3,'Сидоров','Николай','Андреевич','с. Кривошеевка'),
(4,'Кузнецова','Ольга','Викторовна','г. Нижний Ломов'),
(5,'Орлов','Сергей','Владимирович','г. Нижний Ломов'),
(6,'Фёдоров','Михаил','Александрович','г. Нижний Ломов'),
(7,'Никитина','Елена','Романовна','г. Нижний Ломов'),
(8,'Васильев','Андрей','Игоревич','г. Нижний Ломов'),
(9,'Смирнов','Павел','Олегович','г. Нижний Ломов'),
(10,'Белова','Наталья','Викторовна','г. Нижний Ломов');

INSERT INTO Appeals
(id, register_date, applicant_id, topic_id, description, status_id, deadline, response_date)
VALUES
(1,'2026-11-18',1,1,'Ремонт дорожного покрытия на улице Ленина',2,'2026-12-18',NULL),
(2,'2026-11-18',2,5,'Проблема с вывозом мусора',2,'2026-12-18',NULL),
(3,'2026-11-19',3,7,'Не работает освещение улицы',1,'2026-12-19',NULL),
(4,'2026-11-19',4,4,'Отсутствует отопление',3,'2026-12-19','2026-11-25'),
(5,'2026-11-20',5,8,'Благоустройство двора',2,'2026-12-20',NULL),
(6,'2026-11-20',6,13,'Оформление земельного участка',2,'2026-11-23',NULL),
(7,'2026-11-20',7,10,'Запрос социальной помощи',2,'2026-11-21',NULL),
(8,'2026-11-15',8,2,'Отсутствует дорожный знак',3,'2026-12-15','2026-11-20'),
(9,'2026-11-10',9,9,'Необходима детская площадка',4,'2026-11-19',NULL),
(10,'2026-11-24',10,6,'Аварийное водоснабжение',2,'2026-12-24',NULL),
(11,'2026-11-12',1,3,'Очистка дороги от снега',4,'2026-11-17',NULL),
(12,'2026-11-20',2,14,'Изменение границ земельного участка',1,'2026-11-22',NULL);
