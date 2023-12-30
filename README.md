# PasteBinLight

PasteBinLight — это онлайн-сервис, который позволяет пользователям загружать и хранить текстовые файлы в облаке. Вы можете использовать его для обмена кодом, файлами конфигурации, журналами, заметками и другими текстовыми данными. Ближайший аналог — pastebin.com

PasteBinLight is an online service that allows users to upload and store text files in the cloud. You can use it to share code, configuration files, logs, notes, and other text data. The closest analogue is Pastebin.

**Предупреждение / Warning**

**🛑 В данный момент 12.30.2023 Веб приложение работает не корректно. Это связано с тем что аккаунт aws был закрыт. Вы можете использовать свои собственные ключи. Ключи и строки подключения не несут ценной информации (по возможности используйте свои собственные), в случае подозрительной активности ключю будут удалены 🛑**

**🛑 Currently 12.30.2023 The web application is not working correctly. This is due to the fact that the aws account was closed. You can use your own keys. Keys and connection strings do not contain valuable information (use your own if possible); in case of suspicious activity, the key will be deleted 🛑**

# Предыстория / Background

Мы хотим создать веб-приложение, которое позволит пользователям быстро и удобно обмениваться текстовыми фрагментами. Наше вдохновение — это сервис Pastebin, который предоставляет подобный функционал, но имеет ряд недостатков: перегруженный интерфейс, много рекламы — очень много.

We want to create a web application that allows users to quickly and easily share text fragments. Our inspiration is the Pastebin service, which provides similar functionality, but has a number of disadvantages: an overloaded interface, a lot of advertising - a lot.

# Цели проекта / Project goals

Сделать веб-приложение с простым и интуитивным интерфейсом и без надоедливой и мешающей рекламы. Наш продукт должен быть доступным для всех желающих.

Create a web application with a simple and intuitive interface and without annoying and disturbing advertising. Our product should be accessible to everyone.

# Целевая аудитория проекта / Target audience of the project

Наш продукт направлен на широкий круг пользователей, которые хотят воспользоваться нашим инструментом для различных целей: обучения, работы, развлечения и т.д. Мы ориентируемся не только на разработчиков или людей в IT, но и на всех, кто имеет дело с текстом в интернете.

Our product is aimed at a wide range of users who want to use our tool for various purposes: education, work, entertainment, etc. We focus not only on developers or people in IT, but also on everyone who deals with text on the Internet.

# Используемые технологии / Technologies used

• Чистая архитектура / Clean architecture
• DDD
• Rest
• Mediatr
• CQRS (CQR)

1) .Net 7.0
2) ASP.NET WebApi
3) MS SQL
4) Amazon S3
5) Docker
6) Heroku
7) Brevo
8) Vercel

# Используемые библиотеки / Libraries used

**Проект/Project WebApi:**

1) Microsoft.AspNetCore.Authentication.JwtBearer Version 7.0.13
2) Microsoft.AspNetCore.OpenApi Version 7.0.13
3) Microsoft.EntityFrameworkCore.Design Version 7.0.13
4) Serilog.AspNetCore Version 7.0.0
5) Serilog.Sinks.Console Version 5.0.0
6) Serilog.Sinks.File Version 5.0.0
7) Swashbuckle.AspNetCore Version 6.5.0
8) Swashbuckle.AspNetCore.Annotations Version 6.5.0
9) Swashbuckle.AspNetCore.Filters Version 7.0.12
10) Swashbuckle.AspNetCore.Swagger Version 6.5.0

**Проект/Project Application:**

1) FluentResults Version 3.15.2
2) FluentValidation.AspNetCore Version 11.3.0
3) Mapster Version 7.4.0
4) MediatR Version 12.1.1
5) Microsoft.IdentityModel.Tokens Version 7.0.3
6) System.IdentityModel.Tokens.Jwt Version 7.0.3

**Проект/Project Infrastructure:**

1) AWSSDK.S3 Version 3.7.205.24
2) Microsoft.EntityFrameworkCore Version 7.0.13
3) Microsoft.EntityFrameworkCore.SqlServer Version 7.0.13
4) Microsoft.EntityFrameworkCore.Tools Version 7.0.13
5) Newtonsoft.Json Version 13.0.3
6) QRCoder Version 1.4.3
7) System.Drawing.Common Version 7.0.0
8) Telegram.Bot Version 19.0.0

**Проект/Project Tests:**

1) FluentAssertions
2) Moq

# Для установки и запуска проекта вам нужно / To install and run the project you need:

1) Скачать и установить среду разработки / Download and install the development environment.
2) Скачать и установить базу данных MS SQL Server 2023 или выше / Download and install MS SQL Server 2023 or higher database.
3) Клонировать репозиторий проекта с GitHub / Clone the project repository from GitHub.
4) Выполнить миграцию базы данных с помощью команды Update-Database в консоли диспетчера пакетов / Perform a database migration using the Update-Database command in the Package Manager Console.

P.s. Вы можете использовать докер образ, но он работает не идиально, я все еще планирую над ним работать. You can use a docker image, but it doesn't work perfectly, I'm still planning to work on it.

# Бизнес логика / Business logic

1) Регистрация через имя пользователя/пароль/электронную почту. Входящие данные проверяются в бизнес-логике и доменные слои. После того как пользователь вводит свои данные, На их электронную почту будет отправлено подтверждающее сообщение.

Registration via username/password/email. Incoming data is validated at the business logic and domain layers. After the user enters their data, a confirmation message is sent to their email.

2) Вход - по логину/паролю. Если данные правильно, пользователю выдается JWT.

Login - via username/password. If the data is correct, the user is issued a JWT.

3) Создание текстового блока: пользователь создает текстовый блок. и настраивает его по типу: частный/публичный, время самоуничтожения и т. д., и загружает его в система.

Text block creation: The user creates a text block and configures it according to type: private/public, self-destruct time, etc., and uploads it to the system.

4) Хранение текста в облаке: После создания текста блокировать, сервер загружает этот текст в облачное хранилище и получает уникальный URL для доступа к этому тексту блокировать. Его конфигурация и URL-адрес хранятся в файле локальная база данных (на данный момент).

Text storage in the cloud: After creating a text block, the server uploads this text to cloud storage and receives a unique URL for accessing this text block. Its configuration and URL are stored in the local database (for now).

5) Генерация нового URL-адреса. Сервер генерирует новый уникальный URL-адрес и QR-код, связанные с данные конфигурации в локальной базе данных. Этот новый URL-адрес и QR-код возвращаются пользователю.

Generation of a new URL: The server generates a new unique URL and QR code that are linked to the configuration data in the local database. This new URL and QR code are returned to the user.

6) Сервер возвращает URL-адрес пользователя и QR-код для текст. Пользователь может скопировать URL-адрес или QR-код, поделитесь ими с другими людьми или перейдите по ссылке, чтобы просмотреть их текст.

The server returns the user’s URL and QR code for the text. The user can copy the URL or QR code, share them with other people, or follow the link to view their text.

7) Доступ к текстовому блоку: Когда пользователь следует новый URL-адрес, сервер проверяет, является ли этот URL-адрес находится в локальной базе данных. Если это так, сервер делает запрос к облачному хранилищу для получения текста и отображает его пользователю.

Access to the text block: When the user follows the new URL, the server checks whether this URL is in the local database. If it is, the server makes a request to the cloud storage to retrieve the text and displays it to the user.

8) Деактивация и удаление текстовых блоков: Текст блоки и ссылки деактивируются и удаляются из систему по истечении заданного пользователем времени.

Deactivation and deletion of text blocks: Text blocks and links are deactivated and removed from the system after the user-specified time.

9) Если пользователь авторизован, он также может редактировать или удалять свой текст, оценивать тексты других пользователей и просматривать и управлять сохраненными текстами в своем профиле.

If the user is authorized, they can also edit or delete their text, rate other users’ texts, and view and manage their saved texts in their profile.

# Авторы и соавторы / Authors and co-authors
Erzhan — основатель, teamlead и главный backend-разработчик / founder, teamlead and chief backend developer GitHub: ErzhanKub

Zhyrgalbek — проект менеджер и backend-разработчик / project manager and backend developer GitHub: Zhyrgalbek1

Kanat — автотестер и backend-разработчик / autotester and backend developer GitHub: SadyrbekovKanat
