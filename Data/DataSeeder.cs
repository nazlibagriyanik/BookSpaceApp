using bookspace.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace bookspace.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context, UserManager<User> userManager)
        {
            // Kategori ekle
            if (!context.Categories.Any())
            {
                var cat1 = new Category { Name = "Roman", Description = "Roman türündeki kitaplar" };
                var cat2 = new Category { Name = "Bilim", Description = "Bilimsel kitaplar" };
                context.Categories.AddRange(cat1, cat2);
                await context.SaveChangesAsync();
            }

            // Kitap ekle
            if (!context.Books.Any())
            {
                var roman = context.Categories.FirstOrDefault(c => c.Name == "Roman");
                var bilim = context.Categories.FirstOrDefault(c => c.Name == "Bilim");
                if (roman != null && bilim != null)
                {
                    var book1 = new Book
                    {
                        Title = "Kürk Mantolu Madonna",
                        Author = "Sabahattin Ali",
                        ISBN = "9789753638029",
                        PublicationYear = 1943,
                        Description = "Türk edebiyatının en önemli romanlarından biri.",
                        CoverImageUrl = "/images/kmm.jpeg",
                        CategoryId = roman.Id
                    };
                    var book2 = new Book
                    {
                        Title = "Zamanın Kısa Tarihi",
                        Author = "Stephen Hawking",
                        ISBN = "9780553176988",
                        PublicationYear = 1988,
                        Description = "Evrenin kökeni ve yapısı hakkında popüler bilim kitabı.",
                        CoverImageUrl = "/images/zkt.jpeg",
                        CategoryId = bilim.Id
                    };
                    context.Books.AddRange(book1, book2);
                    await context.SaveChangesAsync();
                }
            }

            // Kullanıcı ekle
            var adminEmail = "admin@bookspace.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new User { 
                    UserName = adminEmail, 
                    Email = adminEmail, 
                    EmailConfirmed = true, 
                    CreatedAt = DateTime.Now,
                    FirstName = "Admin",
                    LastName = "User"
                };
                await userManager.CreateAsync(adminUser, "Admin123!");
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // Tartışma ekle
            if (!context.Discussions.Any())
            {
                var book = context.Books.FirstOrDefault();
                if (book != null && adminUser != null)
                {
                    var discussion = new Discussion
                    {
                        Title = "Bu kitap hakkında ne düşünüyorsunuz?",
                        Content = "Romanın dili ve anlatımı hakkında görüşleriniz neler?",
                        BookId = book.Id,
                        UserId = adminUser.Id,
                        CreatedAt = DateTime.Now
                    };
                    context.Discussions.Add(discussion);
                    await context.SaveChangesAsync();

                    // Yorum ekle
                    var comment = new Comment
                    {
                        Content = "Bence çok akıcı bir roman!",
                        DiscussionId = discussion.Id,
                        UserId = adminUser.Id,
                        CreatedAt = DateTime.Now
                    };
                    context.Comments.Add(comment);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
} 