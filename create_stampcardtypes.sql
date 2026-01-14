-- Create StampCardTypes table
CREATE TABLE IF NOT EXISTS "StampCardTypes" (
    "Id" SERIAL PRIMARY KEY,
    "StoreId" INTEGER NOT NULL REFERENCES "Stores"("Id"),
    "Name" VARCHAR(255) NOT NULL,
    "Description" TEXT,
    "RequiredStamps" INTEGER NOT NULL DEFAULT 10,
    "RewardDescription" TEXT,
    "IsActive" BOOLEAN NOT NULL DEFAULT TRUE,
    "CreatedAt" TIMESTAMP,
    "UpdatedAt" TIMESTAMP,
    "CreatedBy" TEXT,
    "UpdatedBy" TEXT,
    "IsDeleted" BOOLEAN NOT NULL DEFAULT FALSE,
    "DeletedAt" TIMESTAMP,
    "DeletedBy" TEXT
);

-- Add CardTypeId to StampCards if not exists
ALTER TABLE "StampCards" ADD COLUMN IF NOT EXISTS "CardTypeId" INTEGER REFERENCES "StampCardTypes"("Id");

-- Create index
CREATE INDEX IF NOT EXISTS "IX_StampCardTypes_StoreId" ON "StampCardTypes"("StoreId");
CREATE INDEX IF NOT EXISTS "IX_StampCards_CardTypeId" ON "StampCards"("CardTypeId");

