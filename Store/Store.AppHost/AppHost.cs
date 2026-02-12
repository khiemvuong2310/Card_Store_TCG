var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL database
var postgres = builder.AddPostgres("postgres")
    .WithImage("postgres", "16")
    .WithEnvironment("POSTGRES_DB", "cardstore");

var cardstoredb = postgres.AddDatabase("cardstoredb");

// Add Redis cache
var redis = builder.AddRedis("redis");

// Add CardStore API project with database reference
builder.AddProject<Projects.CardStore>("cardstore")
    .WithReference(cardstoredb)
    .WithReference(redis);

builder.Build().Run();
