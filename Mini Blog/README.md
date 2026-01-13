# MiniBlog – ASP.NET Core MVC Blog Application

MiniBlog is a clean, modern **ASP.NET Core MVC** web application built with **.NET 9**, **C# 13**, **Entity Framework Core**, and **ASP.NET Core Identity**.  
It implements a simple blogging platform with posts, categories, tags, comments, and likes, following standard MVC best practices.

---

## 🚀 Tech Stack

- **Framework:** ASP.NET Core MVC (.NET 9)
- **Language:** C# 13
- **ORM:** Entity Framework Core
- **Authentication:** ASP.NET Core Identity
- **Database:** SQLite (via EF Core Migrations)
- **UI:** Razor Views, Bootstrap (CDN)
- **Client Validation:** jQuery Validation

---

## 📁 Project Structure


---

## 🧱 Architecture Overview

The application follows the **MVC (Model–View–Controller)** pattern:

- **Controllers** handle HTTP requests and coordinate data flow
- **Models** represent the domain and database entities
- **Views** render HTML using Razor syntax
- **Entity Framework Core** manages persistence
- **Identity** handles authentication and authorization

### High-Level Architecture Diagram

```mermaid
flowchart LR
    Browser["User Browser"] --> MVC["ASP.NET Core MVC"]
    MVC --> Controllers
    Controllers --> Views
    Controllers --> DbContext["ApplicationDbContext"]
    DbContext --> Database[(SQLite / SQL DB)]
    MVC --> Identity["ASP.NET Core Identity"]
    Identity --> Database
    Views --> Browser
classDiagram
    class BlogPost {
        int Id
        string Title
        string Content
        DateTime CreatedAt
        int CategoryId
    }

    class Category {
        int Id
        string Name
    }

    class Tag {
        int Id
        string Name
    }

    class BlogPostTag {
        int BlogPostId
        int TagId
    }

    class Comment {
        int Id
        string Content
        DateTime CreatedAt
        int BlogPostId
        string UserId
    }

    class PostLike {
        int Id
        int BlogPostId
        string UserId
    }

    BlogPost --> Category
    BlogPost --> Comment
    BlogPost --> PostLike
    BlogPost --> BlogPostTag
    Tag --> BlogPostTag
sequenceDiagram
    participant U as User
    participant B as Browser
    participant C as BlogPostsController
    participant D as ApplicationDbContext
    participant V as Razor View

    U->>B: Navigate to /BlogPosts
    B->>C: GET /BlogPosts/Index
    C->>D: Query BlogPosts
    D-->>C: List of BlogPosts
    C->>V: Return View(model)
    V-->>B: Render HTML
    B-->>U: Display page
