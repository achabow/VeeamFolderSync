# Veeam Folder Sync – Test Task

A minimal **one-way folder synchronisation** utility written in C#.  
It keeps a **replica** directory identical to a **source** directory, running on a configurable interval.

---

## ✅ Features

- One-way sync (source → replica)  
- Periodic execution  
- Console + file logging  
- CLI arguments only – no hard-coded paths  
- Built-in SHA-256 file comparison (no 3rd-party sync libraries)

---

## 🚀 Quick Start

### Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/download) or later

### Clone & Build

```bash
git clone https://github.com/achabow/VeeamFolderSync.git
cd VeeamFolderSync
dotnet build -c Release

## 🖥️ One-liner with `run.sh`

Instead of typing long `dotnet run …` commands you can use the convenience script:

```bash
chmod +x run.sh      # only once
./run.sh