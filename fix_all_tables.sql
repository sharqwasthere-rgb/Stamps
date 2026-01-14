-- Add missing columns to StampCards
ALTER TABLE "StampCards" ADD COLUMN IF NOT EXISTS "UpdatedAt" TIMESTAMP;

-- Add missing columns to Transactions  
ALTER TABLE "Transactions" ADD COLUMN IF NOT EXISTS "CreatedAt" TIMESTAMP;
ALTER TABLE "Transactions" ADD COLUMN IF NOT EXISTS "UpdatedAt" TIMESTAMP;
ALTER TABLE "Transactions" ADD COLUMN IF NOT EXISTS "CreatedBy" TEXT;
ALTER TABLE "Transactions" ADD COLUMN IF NOT EXISTS "UpdatedBy" TEXT;
ALTER TABLE "Transactions" ADD COLUMN IF NOT EXISTS "IsDeleted" BOOLEAN DEFAULT FALSE;
ALTER TABLE "Transactions" ADD COLUMN IF NOT EXISTS "DeletedAt" TIMESTAMP;
ALTER TABLE "Transactions" ADD COLUMN IF NOT EXISTS "DeletedBy" TEXT;

