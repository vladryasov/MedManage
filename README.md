# MedManage

Платформа для управления медицинскими товарами и закупками. Организации могут публиковать объявления о наличии/потребности в товарах, искать их и отправлять запросы на покупку.

## Технологический стек

### Бэкенд
- .NET 10, ASP.NET Core
- Entity Framework Core 10 + PostgreSQL 16
- JWT Bearer (HttpOnly cookie)
- AutoMapper, MailKit, BCrypt, Castle DynamicProxy

### Фронтенд
- React 19, TypeScript 6
- Vite 8, Ant Design 6
- TanStack React Query 5, React Router 7

### Инфраструктура
- Docker Compose
- GitHub Container Registry (ghcr.io)

## Быстрый старт (Docker)

### 1. Клонировать репозиторий

```bash
git clone https://github.com/vladryasov/MedManage.git
cd MedManage
```

### 2. Настроить переменные окружения

```bash
cp .env.example .env
```

Заполнить `.env` — см. раздел «Переменные окружения» ниже.

### 3. Запустить

```bash
docker compose -p medmanage up -d --build
```

- **Frontend:** http://localhost:5173
- **API (Swagger):** http://localhost:5151

---

## Переменные окружения

| Переменная | Обязательно | Описание |
|---|---|---|
| `JWT_KEY` | Да | Ключ подписи JWT (256 бит, 32+ символов). Сгенерировать: `openssl rand -base64 32` |
| `ADMIN_EMAIL` | Да | Email администратора, создаётся при первом запуске |
| `ADMIN_PASSWORD` | Да | Пароль администратора |
| `GOOGLE_CLIENT_ID` | Для email | ID Google OAuth2-приложения |
| `GOOGLE_CLIENT_SECRET` | Для email | Секрет OAuth2-приложения |
| `GOOGLE_REFRESH_TOKEN` | Для email | Refresh-токен для Gmail API |
| `GOOGLE_EMAIL` | Для email | Email отправителя (Gmail) |
| `SMTP_FROM_NAME` | Нет | Отображаемое имя отправителя |

> Email-уведомления опциональны. Без них приложение работает, но не отправляет письма.

### Настройка Gmail SMTP

1. Перейти в [Google Cloud Console](https://console.cloud.google.com)
2. Создать проект → **APIs & Services** → **Library** → включить **Gmail API**
3. **Credentials** → **Create Credentials** → **OAuth 2.0 Client ID** → тип **Desktop App**
4. В `redirect_uri` добавить `https://developers.google.com/oauthplayground`
5. Сохранить `GOOGLE_CLIENT_ID` и `GOOGLE_CLIENT_SECRET`
6. Открыть [OAuth Playground](https://developers.google.com/oauthplayground)
7. Шестерёнка (Settings) → **Use your own OAuth credentials** → вставить ID и Secret
8. В поле **Scopes** ввести `https://mail.google.com/` → **Authorize APIs**
9. Войти в свой Gmail → разрешить доступ
10. Нажать **Exchange authorization code for tokens**
11. Скопировать **Refresh Token** → это значение для `GOOGLE_REFRESH_TOKEN`
12. `GOOGLE_EMAIL` — ваш Gmail, с которого будут отправляться письма

---

## Обновление приложения

При пуше в ветку `main` GitHub Actions автоматически собирает образы и загружает их в ghcr.io.

На сервере достаточно выполнить:

```bash
docker compose -p medmanage pull && docker compose -p medmanage up -d
```

Контейнеры перезапустятся с новыми образами.

---

## Разработка

### Запуск бэкенда

```bash
# Убедиться, что PostgreSQL запущен (строка подключения в appsettings.json)
cd MedManage.WebAPI
dotnet run
```

Swagger доступен по адресу http://localhost:5151.

### Запуск фронтенда

```bash
cd client
npm install
npm run dev
```

Vite-сервер на http://localhost:5173, запросы к `/api` проксируются на `localhost:5151`.

### Docker-сборка локально

Если нужно собрать образы без пуша в реестр:

```bash
# Backend
docker build -f MedManage.WebAPI/Dockerfile -t medmanage-backend .

# Frontend
docker build -f client/Dockerfile -t medmanage-frontend .
```

---

## Доступы

| Сервис | Адрес |
|---|---|
| Frontend | http://localhost:5173 |
| Swagger (API) | http://localhost:5151 |
| PostgreSQL | localhost:5432 |

### Роли пользователей

| Роль | Значение |
|---|---|
| CommonUser | 1 |
| Admin | 2 |
| SpecialUser | 3 |

---

## CI/CD

Workflow `.github/workflows/docker-publish.yml`:

- **Триггер:** push в `main` или тег `v*`
- **Действие:** сборка и публикация образов в `ghcr.io/<owner>/<repo>/backend` и `/frontend`
- **Теги:** `latest` при пуше в main, `{версия},latest` при теге
