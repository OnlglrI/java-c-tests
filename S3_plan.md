# S3 — Серверная часть (FastAPI)

## Краткое описание
Сессия S3 посвящена разработке REST API сервера на FastAPI. Реализуются модели данных, аутентификация через JWT, полный CRUD для основных сущностей проекта и автоматическая документация через Swagger UI.

---

## Стек технологий
| Компонент       | Технология                          |
|-----------------|-------------------------------------|
| Фреймворк       | FastAPI                             |
| ASGI-сервер     | Uvicorn                             |
| ORM             | SQLAlchemy 2.x                      |
| База данных      | SQLite (dev) / PostgreSQL (prod)    |
| Миграции        | Alembic                             |
| Схемы/валидация | Pydantic v2                         |
| Аутентификация  | JWT (python-jose) + passlib/bcrypt  |
| Документация    | Swagger UI (встроен в FastAPI)      |

---

## Структура проекта

```
project/
├── server/
│   ├── main.py                  # Точка входа FastAPI
│   ├── config.py                # Настройки (SECRET_KEY, DB URL и т.д.)
│   ├── database.py              # Сессия SQLAlchemy, Base
│   ├── models/
│   │   ├── __init__.py
│   │   ├── user.py              # Модель User
│   │   ├── product.py           # Модель Product
│   │   └── order.py             # Модель Order
│   ├── schemas/
│   │   ├── __init__.py
│   │   ├── user.py              # Pydantic схемы для User
│   │   ├── product.py           # Pydantic схемы для Product
│   │   └── order.py             # Pydantic схемы для Order
│   ├── routers/
│   │   ├── __init__.py
│   │   ├── auth.py              # /register, /login
│   │   ├── users.py             # CRUD пользователей
│   │   ├── products.py          # CRUD товаров
│   │   └── orders.py            # CRUD заказов
│   ├── services/
│   │   ├── auth_service.py      # Логика JWT (create/verify token)
│   │   └── hash_service.py      # Хеширование паролей
│   ├── dependencies.py          # get_db, get_current_user
│   └── alembic/                 # Миграции БД
├── requirements.txt
└── README.md
```

---

## Пошаговый план реализации

### Шаг 1 — Инициализация проекта
- [ ] Создать виртуальное окружение: `python -m venv .venv`
- [ ] Установить зависимости:
  ```
  fastapi uvicorn[standard] sqlalchemy alembic pydantic[email]
  python-jose[cryptography] passlib[bcrypt] python-multipart
  ```
- [ ] Создать файл `requirements.txt`
- [ ] Инициализировать Alembic: `alembic init alembic`

### Шаг 2 — Конфигурация и база данных
- [ ] `config.py`: задать `SECRET_KEY`, `ALGORITHM = "HS256"`, `ACCESS_TOKEN_EXPIRE_MINUTES`, `DATABASE_URL`
- [ ] `database.py`: создать `engine`, `SessionLocal`, `Base`
- [ ] `dependencies.py`: функция `get_db()` (генератор сессии)

### Шаг 3 — Модели SQLAlchemy
- [ ] `User`: `id`, `username`, `email`, `hashed_password`, `role` (admin/user), `is_active`, `created_at`
- [ ] `Product`: `id`, `name`, `description`, `price`, `stock`, `created_at`
- [ ] `Order`: `id`, `user_id` (FK), `product_id` (FK), `quantity`, `total_price`, `status`, `created_at`
- [ ] Создать и применить миграцию: `alembic revision --autogenerate -m "init"` → `alembic upgrade head`

### Шаг 4 — Pydantic-схемы
- [ ] Для каждой сущности создать:
  - `XxxBase` — общие поля
  - `XxxCreate` — поля при создании (без id)
  - `XxxUpdate` — опциональные поля для PATCH
  - `XxxResponse` — поля ответа (с id, created_at)
- [ ] Настроить `model_config = ConfigDict(from_attributes=True)` для ORM-режима

### Шаг 5 — Сервисы аутентификации
- [ ] `hash_service.py`: `hash_password(plain)`, `verify_password(plain, hashed)`
- [ ] `auth_service.py`:
  - `create_access_token(data: dict, expires_delta)` → JWT строка
  - `decode_access_token(token: str)` → payload dict

### Шаг 6 — Зависимость `get_current_user`
- [ ] В `dependencies.py` добавить `get_current_user(token: str = Depends(oauth2_scheme), db = Depends(get_db))`:
  - Декодировать JWT
  - Найти пользователя в БД
  - Бросить `HTTPException 401` если невалидно
- [ ] Добавить `get_current_admin` — проверка роли

### Шаг 7 — Роутер: Аутентификация (`/auth`)
- [ ] `POST /auth/register` — создание пользователя (хеш пароля, роль по умолчанию "user")
- [ ] `POST /auth/login` — проверка credentials, возврат `{"access_token": "...", "token_type": "bearer"}`
- [ ] `GET /auth/me` — информация о текущем пользователе (requires auth)

### Шаг 8 — Роутер: Пользователи (`/users`)
- [ ] `GET /users/` — список пользователей (только admin)
- [ ] `GET /users/{id}` — получить пользователя
- [ ] `PUT /users/{id}` — обновить пользователя
- [ ] `DELETE /users/{id}` — удалить пользователя (только admin)

### Шаг 9 — Роутер: Товары (`/products`)
- [ ] `GET /products/` — список товаров (публичный или с auth)
- [ ] `GET /products/{id}` — получить товар
- [ ] `POST /products/` — создать товар (admin)
- [ ] `PUT /products/{id}` — обновить товар (admin)
- [ ] `DELETE /products/{id}` — удалить товар (admin)

### Шаг 10 — Роутер: Заказы (`/orders`)
- [ ] `GET /orders/` — список заказов текущего пользователя
- [ ] `GET /orders/{id}` — получить заказ
- [ ] `POST /orders/` — создать заказ (автоматически считать `total_price`)
- [ ] `PUT /orders/{id}` — обновить статус заказа
- [ ] `DELETE /orders/{id}` — отменить/удалить заказ

### Шаг 11 — Сборка `main.py`
- [ ] Создать `FastAPI()` приложение
- [ ] Подключить все роутеры через `app.include_router(...)`
- [ ] Настроить CORS (`CORSMiddleware`) — разрешить локальный клиент
- [ ] Создать таблицы при старте: `Base.metadata.create_all(bind=engine)`
- [ ] Запуск: `uvicorn server.main:app --reload`

### Шаг 12 — Проверка через Swagger UI
- [ ] Открыть `http://localhost:8000/docs`
- [ ] Протестировать все эндпоинты вручную
- [ ] Убедиться в корректности статус-кодов (200, 201, 400, 401, 403, 404, 422)

---

## Ключевые технические решения

- **JWT в заголовке**: `Authorization: Bearer <token>` — стандарт OAuth2
- **Хеширование паролей**: bcrypt через `passlib` — никогда не хранить plain-text
- **ORM-режим Pydantic**: `from_attributes=True` позволяет сериализовать SQLAlchemy объекты напрямую
- **Alembic**: все изменения схемы только через миграции, не через `create_all` в проде
- **CORS**: обязателен для работы Tkinter-клиента через `requests` (если браузер — строго, если requests — нет, но лучше настроить)
- **Роли**: поле `role: Enum("admin", "user")` в модели User, проверка через dependency

---

## Критерии завершения (Definition of Done)

- [ ] Все эндпоинты возвращают корректные HTTP статус-коды
- [ ] Аутентификация работает: регистрация → логин → получение токена → доступ к защищённым маршрутам
- [ ] CRUD для User, Product, Order полностью работает
- [ ] Неавторизованные запросы получают `401 Unauthorized`
- [ ] Пользователь без роли admin получает `403 Forbidden` на admin-маршрутах
- [ ] Swagger UI доступен и документирует все эндпоинты
- [ ] База данных заполняется и сохраняет данные между перезапусками
- [ ] `requirements.txt` актуален

