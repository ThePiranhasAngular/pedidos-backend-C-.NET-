DO $EF$
BEGIN
    IF NOT EXISTS(SELECT 1 FROM pg_namespace WHERE nspname = 'ThePirahns') THEN
        CREATE SCHEMA "ThePirahns";
    END IF;
END $EF$;


CREATE TABLE "ThePirahns"."Orders" (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "Status" text NOT NULL,
    "Total" numeric NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Orders" PRIMARY KEY ("Id")
);


CREATE TABLE "ThePirahns"."Products" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "Description" text,
    "Price" numeric NOT NULL,
    "Stock" integer NOT NULL,
    "IsActive" boolean NOT NULL,
    CONSTRAINT "PK_Products" PRIMARY KEY ("Id")
);


CREATE TABLE "ThePirahns"."Users" (
    "Id" uuid NOT NULL,
    "Name" text NOT NULL,
    "Email" text NOT NULL,
    "PasswordHash" text NOT NULL,
    "Role" text NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
);


CREATE TABLE "ThePirahns"."OrderItems" (
    "Id" uuid NOT NULL,
    "OrderId" uuid NOT NULL,
    "ProductId" uuid NOT NULL,
    "Quantity" integer NOT NULL,
    "Price" numeric NOT NULL,
    CONSTRAINT "PK_OrderItems" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_OrderItems_Orders_OrderId" FOREIGN KEY ("OrderId") REFERENCES "ThePirahns"."Orders" ("Id") ON DELETE CASCADE
);


CREATE INDEX "IX_OrderItems_OrderId" ON "ThePirahns"."OrderItems" ("OrderId");


CREATE UNIQUE INDEX "IX_Users_Email" ON "ThePirahns"."Users" ("Email");


