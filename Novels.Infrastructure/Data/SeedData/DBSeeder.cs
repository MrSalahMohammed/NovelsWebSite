using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Novels.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Novels.Infrastructure.Data.SeedData
{
    public static class DBSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            var context = services.GetRequiredService<AppDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();

            await SeedRolesAsync(roleManager);
            var author = await SeedAuthorAsync(userManager, context);
            var reader = await SeedReaderAsync(userManager);
            var categories = await SeedCategoriesAsync(context);
            var tags = await SeedTagsAsync(context);
            var novel = await SeedNovelsAsync(context, author, categories, tags);
            await SeedReviewAndProgressAsync(context, reader, novel);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole<int>> roleManager)
        {
            foreach (var role in new[] { Roles.Author, Roles.Reader, Roles.Admin })
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole<int>(role));
            }
        }

        private static async Task<AuthorProfile> SeedAuthorAsync(
            UserManager<ApplicationUser> userManager, AppDbContext context)
        {
            const string email = "author@test.local";
            var existing = await userManager.FindByEmailAsync(email);
            if (existing?.AuthorProfile is not null)
                return existing.AuthorProfile;

            var user = existing;
            if (user is null)
            {
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    RecoveryEmail = email,
                    FName = "Ahmed",
                    LName = "Test-Author",
                    EmailConfirmed = true,
                    PhoneNumber = "+201005202390"
                };
                var result = await userManager.CreateAsync(user, "Test@1234");
                if (!result.Succeeded)
                    throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            await userManager.AddToRoleAsync(user, Roles.Author);

            var profile = new AuthorProfile { UserId = user.Id, Bio = "Seeded test author." };
            context.AuthorProfiles.Add(profile);
            await context.SaveChangesAsync();
            return profile;
        }

        private static async Task<ApplicationUser> SeedReaderAsync(UserManager<ApplicationUser> userManager)
        {
            const string email = "reader@test.local";
            var existing = await userManager.FindByEmailAsync(email);
            if (existing is not null)
                return existing;

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FName = "Sara",
                LName = "Test-Reader",
                RecoveryEmail = email,
                EmailConfirmed = true,
                PhoneNumber = "+201206242517"
            };
            var result = await userManager.CreateAsync(user, "Test@1234");
            if (!result.Succeeded)
                throw new InvalidOperationException(string.Join(", ", result.Errors.Select(e => e.Description)));

            await userManager.AddToRoleAsync(user, Roles.Reader);
            return user;
        }

        private static async Task<List<Category>> SeedCategoriesAsync(AppDbContext context)
        {
            var names = new[] { "Fantasy", "Romance", "Sci-Fi", "Action", "Mystery" };
            var existingNames = await context.Categories
                .Where(c => names.Contains(c.Name))
                .Select(c => c.Name)
                .ToListAsync();

            var toAdd = names.Except(existingNames)
                .Select(n => new Category { Name = n })
                .ToList();

            if (toAdd.Count > 0)
            {
                context.Categories.AddRange(toAdd);
                await context.SaveChangesAsync();
            }

            return await context.Categories.Where(c => names.Contains(c.Name)).ToListAsync();
        }

        private static async Task<List<Tag>> SeedTagsAsync(AppDbContext context)
        {
            var names = new[] { "Reincarnation", "Slow Burn", "Strong Lead", "Dark", "Comedy" };
            var existingNames = await context.Tags
                .Where(t => names.Contains(t.Name))
                .Select(t => t.Name)
                .ToListAsync();

            var toAdd = names.Except(existingNames)
                .Select(n => new Tag { Name = n })
                .ToList();

            if (toAdd.Count > 0)
            {
                context.Tags.AddRange(toAdd);
                await context.SaveChangesAsync();
            }

            return await context.Tags.Where(t => names.Contains(t.Name)).ToListAsync();
        }

        private static async Task<Novel> SeedNovelsAsync(
            AppDbContext context, AuthorProfile author, List<Category> categories, List<Tag> tags)
        {
            var existing = await context.Novels
                .Include(n => n.Chapters)
                .FirstOrDefaultAsync(n => n.Slug == "the-eternal-throne");
            if (existing is not null)
                return existing;

            var novel = new Novel
            {
                Name = "The Eternal Throne",
                Slug = "the-eternal-throne",
                Description = "A test novel seeded for local development.",
                Status = NovelStatus.Ongoing,
                AuthorProfileId = author.Id,
                Categories = categories.Where(c => c.Name is "Fantasy" or "Action").ToList(),
                Tags = tags.Where(t => t.Name is "Strong Lead" or "Dark").ToList(),
                Chapters = new List<Chapter>
                {
                    new() { ChapterNumber = 1, Name = "The Beginning", Content = "Test content for chapter 1.", IsPublished = true },
                    new() { ChapterNumber = 2, Name = "The Fall", Content = "Test content for chapter 2.", IsPublished = true },
                }
            };

            context.Novels.Add(novel);
            await context.SaveChangesAsync();
            return novel;
        }

        private static async Task SeedReviewAndProgressAsync(
            AppDbContext context, ApplicationUser reader, Novel novel)
        {
            var reviewExists = await context.Reviews
                .AnyAsync(r => r.NovelId == novel.Id && r.ReaderId == reader.Id);

            if (!reviewExists)
            {
                context.Reviews.Add(new Review
                {
                    NovelId = novel.Id,
                    ReaderId = reader.Id,
                    Score = 5,
                    Comment = "Seeded test review — great pacing so far."
                });
            }

            var progressExists = await context.ReadingProgresses
                .AnyAsync(rp => rp.NovelId == novel.Id && rp.ReaderId == reader.Id);

            if (!progressExists && novel.Chapters.Count > 0)
            {
                var firstChapter = novel.Chapters.OrderBy(c => c.ChapterNumber).First();
                context.ReadingProgresses.Add(new ReadingProgress
                {
                    NovelId = novel.Id,
                    ReaderId = reader.Id,
                    LastChapterId = firstChapter.Id,
                    LastReadAt = DateTime.UtcNow
                });
            }

            if (!reviewExists || (!progressExists && novel.Chapters.Count > 0))
                await context.SaveChangesAsync();
        }
    }
}


