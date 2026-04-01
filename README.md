# 🚗 CarWashStation

A web-based booking system for managing car wash appointments, built with ASP.NET Core MVC.

## 📌 Overview

CarWashStation allows customers to book available time slots for car wash services while giving administrators full control over scheduling, including blocking specific dates or time ranges.

---

## ✨ Features

### 👤 Customer

* View available time slots
* Book car wash appointments
* Prevent double-booking

### 🛠️ Admin

* Block entire days (e.g. holidays)
* Block specific time ranges (e.g. 08:00–10:00)
* View active blocks
* Remove blocks

---

## 🧠 How It Works

* The system generates available time slots (e.g. 08:00–16:00)
* Booked slots are automatically excluded
* Blocked slots (full day or time range) are also excluded
* Only valid and available times are shown to users

---

## 🏗️ Tech Stack

* ASP.NET Core MVC
* Entity Framework Core
* SQLite (or SQL Server depending on setup)
* Razor Views
* Bootstrap (UI styling)

---

## 📂 Project Structure

```
CarWashStation/
│
├── Controllers/
│   ├── BookingController.cs
│   └── AdminController.cs
│
├── Models/
│   ├── Booking.cs
│   └── BlockedSlot.cs
│
├── Views/
│   ├── Booking/
│   ├── Admin/
│   └── Shared/
│
├── Data/
│   └── ApplicationDbContext.cs
│
└── Program.cs
```

---

## ⚙️ Setup & Installation

### 1. Clone the repository

```bash
git clone https://github.com/your-username/CarWashStation.git
cd CarWashStation
```

### 2. Install dependencies

```bash
dotnet restore
```

### 3. Apply database migrations

```bash
dotnet ef database update
```

### 4. Run the application

```bash
dotnet run
```

Then open:

```
https://localhost:5001
```

---

## 🗓️ Blocking System

Admins can create blocks in two ways:

### 🔴 Full Day

* Leave time fields empty
* Blocks the entire date

### 🟡 Time Range

* Set **StartTime** and **EndTime**
* Example: `08:00 → 10:00`
* All slots within that range are unavailable

---

## 📸 UI Example

* Add blocking form
* Active blocking list
* Time range display (e.g. `08:00 - 10:00`)

---

## 🚀 Future Improvements

* Authentication & role-based access
* Email/SMS booking confirmations
* Calendar view
* Multi-bay support (multiple cars at once)
* Payment integration

---



## 👨‍💻 Author

Your Name
GitHub: https://github.com/your-username

---
