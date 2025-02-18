using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Tabloid.Models;

namespace Tabloid.Data;

public class TabloidDbContext : IdentityDbContext<IdentityUser>
{
    private readonly IConfiguration _configuration;

    public DbSet<UserProfile> UserProfiles { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Reaction> Reactions { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Subscription> Subscriptions { get; set; }
    public DbSet<PostTag> PostTags { get; set; }
    public DbSet<PostReaction> PostReactions { get; set; }

    public TabloidDbContext(DbContextOptions<TabloidDbContext> context, IConfiguration config)
        : base(context)
    {
        _configuration = config;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // -------------------------------------------
        // Existing Identity Role and User Seeds
        // -------------------------------------------
        modelBuilder
            .Entity<IdentityRole>()
            .HasData(
                new IdentityRole
                {
                    Id = "c3aaeb97-d2ba-4a53-a521-4eea61e59b35",
                    Name = "Admin",
                    NormalizedName = "admin",
                }
            );

        modelBuilder
            .Entity<IdentityUser>()
            .HasData(
                new IdentityUser
                {
                    Id = "dbc40bc6-0829-4ac5-a3ed-180f5e916a5f",
                    UserName = "Administrator",
                    Email = "admina@strator.comx",
                    PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(
                        null,
                        _configuration["AdminPassword"]
                    ),
                },
                new IdentityUser
                {
                    Id = "d8d76512-74f1-43bb-b1fd-87d3a8aa36df",
                    UserName = "JohnDoe",
                    Email = "john@doe.comx",
                    PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(
                        null,
                        _configuration["AdminPassword"]
                    ),
                },
                new IdentityUser
                {
                    Id = "a7d21fac-3b21-454a-a747-075f072d0cf3",
                    UserName = "JaneSmith",
                    Email = "jane@smith.comx",
                    PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(
                        null,
                        _configuration["AdminPassword"]
                    ),
                },
                new IdentityUser
                {
                    Id = "c806cfae-bda9-47c5-8473-dd52fd056a9b",
                    UserName = "AliceJohnson",
                    Email = "alice@johnson.comx",
                    PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(
                        null,
                        _configuration["AdminPassword"]
                    ),
                },
                new IdentityUser
                {
                    Id = "9ce89d88-75da-4a80-9b0d-3fe58582b8e2",
                    UserName = "BobWilliams",
                    Email = "bob@williams.comx",
                    PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(
                        null,
                        _configuration["AdminPassword"]
                    ),
                },
                new IdentityUser
                {
                    Id = "d224a03d-bf0c-4a05-b728-e3521e45d74d",
                    UserName = "EveDavis",
                    Email = "Eve@Davis.comx",
                    PasswordHash = new PasswordHasher<IdentityUser>().HashPassword(
                        null,
                        _configuration["AdminPassword"]
                    ),
                }
            );

        modelBuilder
            .Entity<IdentityUserRole<string>>()
            .HasData(
                new IdentityUserRole<string>
                {
                    RoleId = "c3aaeb97-d2ba-4a53-a521-4eea61e59b35",
                    UserId = "dbc40bc6-0829-4ac5-a3ed-180f5e916a5f",
                },
                new IdentityUserRole<string>
                {
                    RoleId = "c3aaeb97-d2ba-4a53-a521-4eea61e59b35",
                    UserId = "d8d76512-74f1-43bb-b1fd-87d3a8aa36df",
                }
            );

        modelBuilder
            .Entity<UserProfile>()
            .HasData(
                new UserProfile
                {
                    Id = 1,
                    IdentityUserId = "dbc40bc6-0829-4ac5-a3ed-180f5e916a5f",
                    FirstName = "Admina",
                    LastName = "Strator",
                    ImageLocation = "https://robohash.org/numquamutut.png?size=150x150&set=set1",
                    CreateDateTime = new DateTime(2022, 1, 25),
                    IsActive = true
                },
                new UserProfile
                {
                    Id = 2,
                    IdentityUserId = "d8d76512-74f1-43bb-b1fd-87d3a8aa36df",
                    FirstName = "John",
                    LastName = "Doe",
                    ImageLocation = "https://robohash.org/nisiautemet.png?size=150x150&set=set1",
                    CreateDateTime = new DateTime(2023, 2, 2),
                    IsActive = true
                },
                new UserProfile
                {
                    Id = 3,
                    IdentityUserId = "a7d21fac-3b21-454a-a747-075f072d0cf3",
                    FirstName = "Jane",
                    LastName = "Smith",
                    ImageLocation =
                        "https://robohash.org/molestiaemagnamet.png?size=150x150&set=set1",
                    CreateDateTime = new DateTime(2022, 3, 15),
                    IsActive = true
                },
                new UserProfile
                {
                    Id = 4,
                    IdentityUserId = "c806cfae-bda9-47c5-8473-dd52fd056a9b",
                    FirstName = "Alice",
                    LastName = "Johnson",
                    ImageLocation =
                        "https://robohash.org/deseruntutipsum.png?size=150x150&set=set1",
                    CreateDateTime = new DateTime(2023, 6, 10),
                    IsActive = true
                },
                new UserProfile
                {
                    Id = 5,
                    IdentityUserId = "9ce89d88-75da-4a80-9b0d-3fe58582b8e2",
                    FirstName = "Bob",
                    LastName = "Williams",
                    ImageLocation =
                        "https://robohash.org/quiundedignissimos.png?size=150x150&set=set1",
                    CreateDateTime = new DateTime(2023, 5, 15),
                    IsActive = true
                },
                new UserProfile
                {
                    Id = 6,
                    IdentityUserId = "d224a03d-bf0c-4a05-b728-e3521e45d74d",
                    FirstName = "Eve",
                    LastName = "Davis",
                    ImageLocation = "https://robohash.org/hicnihilipsa.png?size=150x150&set=set1",
                    CreateDateTime = new DateTime(2022, 10, 18),
                    IsActive = true
                }
            );

        // -------------------------------------------
        // NEW SEEDS FOR YOUR OTHER ENTITIES
        // -------------------------------------------

        // 1) Categories
        modelBuilder
            .Entity<Category>()
            .HasData(
                new Category { Id = 1, Name = "Technology" },
                new Category { Id = 2, Name = "Travel" },
                new Category { Id = 3, Name = "Food" },
                new Category { Id = 4, Name = "Music" },
                new Category { Id = 5, Name = "Sports" }
            );

        // 2) Reactions
        modelBuilder
            .Entity<Reaction>()
            .HasData(
                new Reaction
                {
                    Id = 1,
                    Name = "Like",
                    Icon = "👍",
                },
                new Reaction
                {
                    Id = 2,
                    Name = "Love",
                    Icon = "❤️",
                },
                new Reaction
                {
                    Id = 3,
                    Name = "Wow",
                    Icon = "😮",
                }
            );

        // 3) Tags
        modelBuilder
            .Entity<Tag>()
            .HasData(
                new Tag { Id = 1, Name = "CSharp" },
                new Tag { Id = 2, Name = "DotNet" },
                new Tag { Id = 3, Name = "TravelTips" },
                new Tag { Id = 4, Name = "Recipes" },
                new Tag { Id = 5, Name = "Concerts" }
            );

        // 4) Posts
        modelBuilder
            .Entity<Post>()
            .HasData(
                new Post
                {
                    Id = 1,
                    AuthorId = 2, // John Doe
                    Title = "Hello World in C#",
                    SubTitle = "Your Very First Program",
                    CategoryId = 1, // Technology
                    PublishingDate = new DateTime(2023, 3, 20),
                    HeaderImage = "https://picsum.photos/200/300",
                    Content =
                        "Console.WriteLine(\"Hello World!\"); "
                        + "This is a simple introduction to C# programming...",
                    IsApproved = true,
                },
                new Post
                {
                    Id = 2,
                    AuthorId = 3, // Jane Smith
                    Title = "The Best Pizza Recipe",
                    SubTitle = "Cheesy Goodness",
                    CategoryId = 3, // Food
                    PublishingDate = new DateTime(2023, 4, 1),
                    HeaderImage = "https://picsum.photos/200/300?grayscale",
                    Content =
                        "Dough, sauce, cheese — what else could you want? "
                        + "This post explores the perfect pizza recipe...",
                    IsApproved = true,
                },
                new Post
                {
                    Id = 3,
                    AuthorId = 4, // Alice Johnson
                    Title = "Hiking the Alps",
                    SubTitle = "A Breathtaking Adventure",
                    CategoryId = 2, // Travel
                    PublishingDate = new DateTime(2023, 5, 10),
                    HeaderImage = "https://picsum.photos/200/300?random=1",
                    Content = "Grab your boots! The Alps offer an unbeatable hiking experience...",
                    IsApproved = true,
                },
                new Post
                {
                    Id = 4,
                    AuthorId = 3, // Bob Williams
                    Title = "Top 10 Rock Concerts of 2023",
                    SubTitle = "Get Ready to Rock!",
                    CategoryId = 4, // Music
                    PublishingDate = new DateTime(2023, 8, 1),
                    HeaderImage = "https://picsum.photos/200/300?random=2",
                    Content = "Let's break down the greatest rock concerts of the year...",
                    IsApproved = true,
                },
                new Post
                {
                    Id = 5,
                    AuthorId = 5, // Bob Williams
                    Title = "Top 10 Rock Concerts of 2023",
                    SubTitle = "Get Ready to Rock!",
                    CategoryId = 4, // Music
                    PublishingDate = new DateTime(2023, 8, 1),
                    HeaderImage = "https://picsum.photos/200/300?random=2",
                    Content = "Let's break down the greatest rock concerts of the year...",
                    IsApproved = true,
                }
            );

        // 5) Comments
        modelBuilder
            .Entity<Comment>()
            .HasData(
                new Comment
                {
                    Id = 1,
                    AuthorId = 3, // Jane Smith
                    PostId = 1, // Post: Hello World in C#
                    Subject = "Nice intro!",
                    Content = "This is super helpful, thanks for sharing.",
                    CreationDate = new DateTime(2023, 3, 22),
                },
                new Comment
                {
                    Id = 2,
                    AuthorId = 2, // John Doe
                    PostId = 2, // Post: The Best Pizza Recipe
                    Subject = "Yum!",
                    Content = "I tried this recipe, and it was delicious!",
                    CreationDate = new DateTime(2023, 4, 2),
                },
                new Comment
                {
                    Id = 3,
                    AuthorId = 6, // Eve Davis
                    PostId = 4, // Post: Top 10 Rock Concerts
                    Subject = "Which was #1?",
                    Content = "Loved your list. Which one was your favorite?",
                    CreationDate = new DateTime(2023, 8, 2),
                }
            );

        // 6) Subscriptions
        modelBuilder
            .Entity<Subscription>()
            .HasData(
                new Subscription
                {
                    Id = 1,
                    SubscriberId = 2, // John Doe
                    AuthorId = 3, // Jane Smith
                    SubscriptionStartDate = new DateTime(2023, 8, 2)
                },
                new Subscription
                {
                    Id = 2,
                    SubscriberId = 3, // Jane Smith
                    AuthorId = 2, // John Doe
                    SubscriptionStartDate = new DateTime(2023, 9, 2)
                },
                new Subscription
                {
                    Id = 3,
                    SubscriberId = 2, // John Doe
                    AuthorId = 4, // Alice Johnson
                    SubscriptionStartDate = new DateTime(2023, 10, 2)
                }
            );

        // 7) PostTag (bridge table)
        modelBuilder
            .Entity<PostTag>()
            .HasData(
                new PostTag
                {
                    Id = 1,
                    PostId = 1, // Hello World in C#
                    TagId = 1, // CSharp
                },
                new PostTag
                {
                    Id = 2,
                    PostId = 1, // Hello World in C#
                    TagId = 2, // DotNet
                },
                new PostTag
                {
                    Id = 3,
                    PostId = 3, // Hiking the Alps
                    TagId = 3, // TravelTips
                },
                new PostTag
                {
                    Id = 4,
                    PostId = 2, // The Best Pizza Recipe
                    TagId = 4, // Recipes
                },
                new PostTag
                {
                    Id = 5,
                    PostId = 4, // Top 10 Rock Concerts
                    TagId = 5, // Concerts
                }
            );
    }
}
