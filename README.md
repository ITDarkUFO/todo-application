# Тестовое задание ToDo App
## Описание
Создать веб-приложение для управления списком задач (ToDo list). Предоставить Web API для создания, просмотра, редактирования и удаления задач, а также управление связанными данными
## Используемый стэк
 - Visual Studio
 - ASP.NET Core 8.0
 - user-secrets (на время разработки)
 - Entity Framework 8.0
 - PostgreSQL 8.2
 - Docker
 - Изображения из [Freepik](https://www.freepik.com/)

##  Начальная настройка 
Запуск можно проводить как через Visual Studio, так и через Docker, но перед первым запуском необходимо выполнить несколько действий по настройке:

 1. Разверните базу данных для проекта в PostgreSQL
 2. Зайдите через консоль в папку ***\Application***
 3. Добавьте в user-secrets адрес и данные входа в базу данных с помощью команды `dotnet user-secrets set "ConnectionStrings:Default" "Server=адрес; Database=имя-бд; Username=логин; Password=пароль;"`
 4. Установите dotnet с помощью команды `dotnet tool install --global dotnet-ef`
 5. Установите пакет EFCore Design с помощью команды `dotnet  add package Microsoft.EntityFrameworkCore.Design`
 6. Проверьте, что команда `dotnet ef` выполняется без ошибок
 7. Обновите базу данных с помощью команды `dotnet ef database update`

## Запуск приложения
После запуска приложения в браузере должен появиться начальный экран.
![Начальный экран](https://raw.githubusercontent.com/ITDarkUFO/todo-list_test-assignment/readme/img/home-page.png?token=GHSAT0AAAAAACOTWRY4MKXESU7IKXJB7VLSZU6SKPA)
Войти в систему можно либо с помощью аккаунта админа `admin -  admin`, либо зарегистрировать свой.
В дальнейшем будет иметься ввиду аккаунт админа, так как только он изначально имеет доступ к панели администрирования.


